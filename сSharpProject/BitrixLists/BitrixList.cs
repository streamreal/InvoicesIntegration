using System;

namespace BitrixLists
{
    class In
    {
        public void Intro()
        {
            Bitrix24 bx_logon = new Bitrix24();

            int start = 0;

            //do
            //{
            
                string jsonResponse = bx_logon.SendCommand("lists.element.get", "",
                                                            "IBLOCK_TYPE_ID=lists_socnet" +
                                                            "&SOCNET_GROUP_ID=330" +
                                                            "&IBLOCK_ID=184" +
                                                            "&FILTER[ID][0]=15008&FILTER[ID][1]=15020" 

                                                            //"&ELEMENT_ORDER[ID]=DESC" 
                                                            //"&start=" + start.ToString()
                                                            );

                jsonResponse = ResponseProcessor.FixJson(jsonResponse);
                start = ResponseProcessor.ProcessNewRecords(jsonResponse);

            //} while (start > 0);
            Console.WriteLine(jsonResponse);

            Console.ReadLine();
        }
    }
}