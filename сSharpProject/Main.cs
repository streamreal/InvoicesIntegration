using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace сSharpProject
{
    class MainIntro
    {
        public static string GetJsonResponse(int start)
        {
            Bitrix24 bx_logon = new Bitrix24();
            string jsonResponse = bx_logon.SendCommand("log.blogpost.get", "LOG_RIGHTS[0]=SG330&start=" + start.ToString());

            Regex regex = new Regex(@"\\[U][0-9A-Z]{4}", RegexOptions.IgnoreCase);
            MatchCollection m = regex.Matches(jsonResponse);
            IFormatProvider prov = new CultureInfo("ru-RU");

            foreach (Match match in m)
            {
                if (int.TryParse(match.Value.Substring(2), NumberStyles.HexNumber, prov, out int output) == true)
                {
                    char c = (char)output;
                    jsonResponse = jsonResponse.Replace(match.Value, c.ToString());
                }
            }

            Regex reg = new Regex("[T][I][T][L][E][\"][:][\"].*?[\"][,][\"][A][U][T][H][O][R][_][I][D]", RegexOptions.IgnoreCase);

            m = reg.Matches(jsonResponse);
            string temp;

            foreach (Match match in m)
            {
                temp = match.Value.Replace("\"", "'");
                temp = temp.Replace("TITLE':'", "TITLE\":\"");
                temp = temp.Replace("','AUTHOR_ID", "\",\"AUTHOR_ID");
                jsonResponse = jsonResponse.Replace(match.Value, temp);
            }

            reg = new Regex("[D][E][T][A][I][L][_][T][E][X][T][\"][:][\"].*?[\"][,][\"][D][A][T][E]", RegexOptions.IgnoreCase);

            m = reg.Matches(jsonResponse);

            foreach (Match match in m)
            {
                temp = match.Value.Replace("\"", "'");
                temp = temp.Replace("DETAIL_TEXT':'", "DETAIL_TEXT\":\"");
                temp = temp.Replace("','DATE", "\",\"DATE");
                jsonResponse = jsonResponse.Replace(match.Value, temp);
            }

            return jsonResponse;
        }
        public static void Main()
        {
            ExcelCharts.Process();

            //RefDeclarationParser.Run();
            //RefInvoiceParser r = new RefInvoiceParser();
            //r.Run();


            //Deviation dev = new Deviation(); ;
            //int start = 0;
            //try
            //{

            //do
            //{
            //   Console.WriteLine("current position " + start.ToString());
            //        string s = GetJsonResponse(start);
            //   start = dev.ProcessNewRecords(s);  
            //    } while (start > 0);


            //}
            //catch (Exception ex)
            //{            
            //    Console.WriteLine(ex.Message);
            //    Console.WriteLine(ex.StackTrace);
            //    File.WriteAllText(@"C:\Users\andreydruzhinin\Desktop\1.txt", GetJsonResponse(start));

            //}

            //Console.ReadLine();
            //other bitrix examples      
            //bx_logon.SendCommand(""task.item.list"", ""ORDER[]=&FILTER[GROUP_ID]=44&PARAMS[]=&SELECT[]="");
            //string TaskListByJSON = bx_logon.SendCommand(""task.commentitem.get"", ""TASKID=57337&ITEMID=379512"");
            //string jsonResponse = bx_logon.SendCommand(""disk.storage.uploadfile"", ""id=2233&data[NAME]=test.jpg"", ""fileContent[0]=test.jpg&fileContent[1]="" + file);

            //bitrix add message with files

            //string filename = @""C:\Users\andreydruzhinin\Desktop\1.xlsx"";
            //string contents = Convert.ToBase64String(File.ReadAllBytes(filename), Base64FormattingOptions.None);

            //Bitrix24 bx_logon = new Bitrix24();

            //  _ = bx_logon.SendCommand("log.blogcomment.add", "",
            //        "USER_ID=1716" +
            //        "&POST_ID=36952" +
            //        "&TEXT=тест"
            //        );

            //Bitrix24 bx_logon = new Bitrix24();
            //string jsonResponse = bx_logon.SendCommand("log.blogpost.add", "",
            //"USER_ID=1827" +
            //"&POST_TITLE=заголовок" +
            //"&POST_MESSAGE=текст сообщения" +
            //"&DEST[0]=SG521") ;
           
            /*
            +
           ""&FILES[0][0]=test.xlsx&FILES[0][1]="" + HttpUtility.UrlEncode(contents)  //+
           //""&FILES[1][0]=2.xlsx&FILES[1][1]="" + file +
           //""&FILES[2][0]=3.xlsx&FILES[2][1]="" + file     
           );             
           Console.WriteLine(jsonResponse);
           Console.ReadLine();
           */

            //CurrencyRate.GetRates();

            //DeclarationParser d = new DeclarationParser();
            //d.Go();

            //var w = new NewWebApi();
            //w.ProcessXml();

            //Linq l = new Linq();
            //l.Go_inv();

            //MailGetAttachments.Download();
            Console.WriteLine("END");
            Console.ReadLine();
        }
    }
}
