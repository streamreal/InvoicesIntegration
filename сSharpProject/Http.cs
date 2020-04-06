using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace сSharpProject
{
    class Http
    {
        private string login = "sa10552";
        private string password = "2VsW5aMn";

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
        public void ProcessXml()
        {
            string result;
            string[] xmlFiles = ParseXml(@"C:\Users\andreydruzhinin\Desktop\message");

            System.Threading.Thread.Sleep(1000);

            for (int i = 0; i < xmlFiles.Length; i++)
            {
                System.Threading.Thread.Sleep(1000);
                result = GetPdf(xmlFiles[i]);
            }
            Console.ReadLine();
        }

        public string GetPdf(string fullPath)
        {
            byte[] xml = File.ReadAllBytes(fullPath);
            string secret = GetMd5Hash(xml.Length.ToString() + ":" + login + ":" + GetMd5Hash(password));
            string url = @"https://www.alta.ru/xml-preview/api/?login=" + login + "&secret=" + secret;

            using (WebClient wc = new WebClient())
            {                
                byte[] response = wc.UploadData(url, xml);
                return Encoding.UTF8.GetString(response);
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
    }

}
