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
    class NewWebApi
    {
        private readonly string login = "sa10552";
        private readonly string password = "2VsW5aMn";

        public void ProcessXml()
        {
            string[] xmlFiles = ParseXml(@"C:\Users\andreydruzhinin\Desktop\message");
            string result;
            for (int i = 0; i < xmlFiles.Length; i++)
            {
                System.Threading.Thread.Sleep(1000);
                result = GetPdf(xmlFiles[i]);
            }
            Console.WriteLine("Finished");
            Console.ReadLine();
        }
        private string GetPdf(string fullPath)
        {
            int counter = 0;

            //возврат PDF или ID очереди
            string postResult = PostResponse(fullPath);
            Console.WriteLine(postResult);

            if (postResult.EndsWith(".pdf") == true)
            {
                return postResult;
            }
            else
            {
                string id = postResult;

                while (1 == 1)
                {
                    //if (counter++ == 12)
                    //{
                    //    //возврат XML 
                    //    return fullPath;
                    //}

                    System.Threading.Thread.Sleep(10000);

                    string getResult = GetResponse(id, fullPath);
                    Console.WriteLine(getResult);

                    if (getResult.EndsWith(".pdf") == true)
                    {
                        return getResult;
                    }
                }
            }

        }

        private string GetMd5Hash(string input)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder sBuilder = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }
                return sBuilder.ToString();
            }
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

        //Запускается один раз
        public string PostResponse(string fullPath)
        {
            byte[] xml = File.ReadAllBytes(fullPath);
            string secret = GetMd5Hash(xml.Length.ToString() + ":" + login + ":" + GetMd5Hash(password));
            string url = @"https://www.alta.ru/xml-preview/api/?login=" + login + "&secret=" + secret;

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.169 Safari/537.36";
            req.ContentType = "application/xml";
            req.ContentLength = xml.Length;
            req.Method = "POST";
            using (Stream s = req.GetRequestStream())
            {
                s.Write(xml, 0, xml.Length);
            }

            using (HttpWebResponse resp = (HttpWebResponse)req.GetResponse())
            {
                if (resp.StatusCode == HttpStatusCode.OK)
                {
                    if (resp.ContentType.StartsWith("application/pdf"))
                    {
                        return SavePdfFromResponse(resp, fullPath);
                    }
                    else if (resp.ContentType.StartsWith("application/json"))
                    {
                        return ReadJsonId(resp);
                    }
                }
                return String.Empty;
            }
        }

        //Загружается повторно
        public string GetResponse(string id, string fullPath)
        {
            string secret = GetMd5Hash(id + ":" + login + ":" + GetMd5Hash(password));
            string url = @"https://www.alta.ru/xml-preview/api/?login=" + login + "&secret=" + secret + "&id=" + id;

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.UserAgent = "Mozilla/5.0 (Windows NT 6.1; W…) Gecko/20100101 Firefox/74.0";
            req.Method = "GET";

            using (HttpWebResponse resp = (HttpWebResponse)req.GetResponse())
            {
                Console.WriteLine(resp.ContentType);

                if (resp.ContentType.StartsWith("application/pdf") == true)
                {
                    return SavePdfFromResponse(resp, fullPath);
                }
                return String.Empty;
            }
        }

        public string SavePdfFromResponse(HttpWebResponse resp, string fullPath)
        {
            MemoryStream ms = new MemoryStream();
            resp.GetResponseStream().CopyTo(ms);
            byte[] pdf = ms.ToArray();
            ms.Close();
            string result = fullPath.Remove(fullPath.LastIndexOf(".")) + ".pdf";
            File.WriteAllBytes(result, pdf);
            return result;
        }

        public string ReadJsonId(HttpWebResponse resp)
        {
            Encoding enc = Encoding.UTF8;
            string json = String.Empty;
            using (Stream st = resp.GetResponseStream())
            using (StreamReader sr = new StreamReader(st, enc))
            {
                json = sr.ReadToEnd();
            }
            JsonResponse jr = (JsonResponse)JsonConvert.DeserializeObject(json, typeof(JsonResponse));
            return jr.Id;
        }

        public class JsonResponse
        {
            public string Status { get; set; }
            public string Text { get; set; }
            public string Id { get; set; }
        }
    }
}
