using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace сSharpProject
{
    class Web_api
    {
        private string login = "sa10552";
        private string password = "2VsW5aMn";

        public void ProcessXml()
        {
            string result;
            string[] xmlFiles = ParseXml(@"C:\Users\andreydruzhinin\Desktop\message");

            for (int i = 0; i < xmlFiles.Length; i++)
            {
                System.Threading.Thread.Sleep(1000);
                result = GetPdf(xmlFiles[i]);
                Console.WriteLine(result);
            }
            Console.ReadLine();
        }

        private string GetPdf(string fullPath)
        {
            byte[] xml = File.ReadAllBytes(fullPath);
            string secret = GetMd5Hash(xml.Length.ToString() + ":" + login + ":" + GetMd5Hash(password));
            string url = @"https://www.alta.ru/xml-preview/api/?login=" + login + "&secret=" + secret;
            string result;            

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.ContentType = "application/xml";
            req.ContentLength = xml.Length;
            req.Method = "POST";

            Console.WriteLine(req.ContentLength);

            //запись данных в запрос
            using (Stream s = req.GetRequestStream())
            {
                s.Write(xml, 0, xml.Length);
            }

            //запрос на конвертацию
            //в случае некорректного запроса - 401 ошибка
            

                using (HttpWebResponse resp = (HttpWebResponse)req.GetResponse())
                {
                    if (resp.StatusCode == HttpStatusCode.OK)
                    {
                        //возврат pdf для файлов размером менее 100 КБ
                        if (resp.ContentType.StartsWith("application/pdf"))
                        {
                            MemoryStream ms = new MemoryStream();
                            resp.GetResponseStream().CopyTo(ms);
                            byte[] pdf = ms.ToArray();
                            ms.Close();
                            result = fullPath.Remove(fullPath.LastIndexOf(".")) + ".pdf";
                            File.WriteAllBytes(result, pdf);
                            return result;
                        }
                        //постановка в очередь для файлов более 100 КБ и возврат id для нового запроса
                        else if (resp.ContentType.StartsWith("application/json"))
                        {
                            string json = string.Empty;
                            string id = string.Empty;
                            Encoding enc = Encoding.UTF8;

                            using (Stream st = resp.GetResponseStream())
                            using (StreamReader sr = new StreamReader(st, enc))
                            {
                                json = sr.ReadToEnd();
                            }

                            JsonResponse jr;

                            try
                            {
                                jr = (JsonResponse)JsonConvert.DeserializeObject(json, typeof(JsonResponse));
                            }
                            catch
                            {
                                throw new JsonException();
                            }

                            id = jr.id;

                            string jSecret = GetMd5Hash(id + ":" + login + ":" + GetMd5Hash(password));
                            string jUrl = @"https://www.alta.ru/xml-preview/api/?login=" + login + "&secret=" + jSecret + "&id=" + id;

                            //запрос на получение файла в очереди
                            HttpWebRequest jReq = (HttpWebRequest)WebRequest.Create(jUrl);
                            jReq.Method = "GET";
                            HttpWebResponse jResp;

                            //10 циклов по 6 секунд - ждем возврата файла в течение минуты
                        for (int i = 0; i < 10; i++)
                            {
                                System.Threading.Thread.Sleep(6000);

                                    jResp = (HttpWebResponse)jReq.GetResponse();
                                                                       
                                    if (jResp.ContentType.StartsWith("application/pdf"))                                       
                                        {
                                        Console.WriteLine("js");
                                            MemoryStream ms = new MemoryStream();
                                            jResp.GetResponseStream().CopyTo(ms);
                                            byte[] pdf = ms.ToArray();
                                            ms.Close();
                                            result = fullPath.Remove(fullPath.LastIndexOf(".")) + ".pdf";
                                            File.WriteAllBytes(result, pdf);
                                            return result;
                                        }

                                    jResp.Close();

                            }
                            throw new TimeOutException();
                        }
                        else
                        {
                            throw new UnknownContentException();
                        }
                    }
                    else
                    {
                        throw new BadResponseException();
                    }
                }
            
        /*
        }
            catch (Exception e)
            {
                //возвращаем xml в случае любых ошибок  
                Console.WriteLine(e.Message);
                return fullPath;
            }
            */
        }

        private string GetMd5Hash(string input)
        {
            MD5 md5Hash = MD5.Create();
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }

        private string[] ParseXml(string fileNameNoExt)
        {
            string xmlFileName = fileNameNoExt + ".xml";
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(xmlFileName);
            XmlNodeList nList = xDoc.GetElementsByTagName("edcnt:ContainerDoc");
            int c = nList.Count;

            //если файл представляет собой контейнер, то разбиваем на отдельные документы
            if (c > 1)
            {
                List<string> filesList = new List<string>();
                string xmlTempFileName;
                for (int i = 0; i < c; i++)
                {
                    XmlDocument xTempDoc = new XmlDocument();
                    xTempDoc.Load(xmlFileName);
                    XmlNodeList nTempList = xTempDoc.GetElementsByTagName("edcnt:ContainerDoc");

                    for (int j = c - 1; j >= 0; j--)
                    {
                        if (j != i)
                        {
                            nTempList[j].ParentNode.RemoveChild(nTempList[j]);
                        }
                    }
                    xmlTempFileName = fileNameNoExt + (i + 1).ToString() + ".xml";
                    xTempDoc.Save(xmlTempFileName);
                    filesList.Add(xmlTempFileName);
                }

                string[] str = filesList.ToArray();
                return str;
            }
            else
            {
                string[] str = new string[] { xmlFileName };
                return str;
            }
        }

        public class JsonResponse
        {
            public string status { get; set; }
            public string text { get; set; }
            public string id { get; set; }
        }

        private void ClearFolder(string folderName)
        {
            DirectoryInfo dir = new DirectoryInfo(folderName);

            foreach (FileInfo fi in dir.GetFiles())
            {
                try
                {
                    fi.Delete();
                }
                catch (Exception) { }
            }
        }

        public class TimeOutException : Exception
        {
            public TimeOutException() 
            {
                Console.WriteLine("TimeOutException");
            }
        }
        public class BadResponseException : Exception
        {
            public BadResponseException()
            {
                Console.WriteLine("BadResponseException");
            }
        }
        public class UnknownContentException : Exception
        {
            public UnknownContentException() 
        {
                Console.WriteLine("UnknownContentException");
            }
        }
        public class JsonException : Exception
        {
            public JsonException() {
                Console.WriteLine("JsonException");
            }
        }

    }
}
