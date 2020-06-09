using System;
using System.IO;
using System.Web;

namespace BitrixLists
{
    class In
    {
        public void Intro()
        {
            Bitrix24 bx_logon = new Bitrix24();
            string jsonResponse = string.Empty;
            int start = 0;
            int temp = 0;
            do
            {
                Console.WriteLine(start);
                jsonResponse = bx_logon.SendCommand("lists.element.get", "start=" + start.ToString(),
                                             "IBLOCK_TYPE_ID=lists_socnet" +
                                             "&SOCNET_GROUP_ID=330" +
                                             "&IBLOCK_ID=184"
                                             );
            
                jsonResponse = ResponseProcessor.FixJson(jsonResponse);
                start = ResponseProcessor.ProcessNewRecords(jsonResponse);
             

            } while (start > 0);           

            Console.ReadLine();
        }
    }
}