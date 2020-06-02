using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace сSharpProject
{
    class MainIntro
    {
        public static void Main()
        {
            BitrixLists.In i = new BitrixLists.In();
            i.Intro();
            
            //ExcelCharts.Process();

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
