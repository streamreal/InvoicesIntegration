using Microsoft.SqlServer.Server;
using System;
using System.Data.SqlClient;

public partial class Triggers
{
    // Введите существующую таблицу или представление для целевого объекта и раскомментируйте строку атрибута.
    [Microsoft.SqlServer.Server.SqlTrigger (Name="SendMessageBitrix", Target="[dbo].[trigger_test]", Event="FOR UPDATE")]
    public static void SendMessageBitrix()
    {
        SqlTriggerContext triggContext = SqlContext.TriggerContext;
        SqlCommand command;
        SqlDataReader reader;

        if (triggContext.TriggerAction == TriggerAction.Update)
        {
            using (SqlConnection connection = new SqlConnection(@"context connection=true"))
            {
                connection.Open();
                command = new SqlCommand(@"SELECT * FROM INSERTED;", connection);
                reader = command.ExecuteReader();
                reader.Read();
                bool goingToSendMessage = false;

                for (int i = 0; i < reader.FieldCount - 1; i++)
                {
                    if (reader.GetName(i) == "DocNum")
                    {
                        goingToSendMessage = true;
                        break;
                    }
                }

                reader.Close();

                if (goingToSendMessage)
                {
                    Bitrix24 bx_logon = new Bitrix24();
                    try
                    {
                        _ = bx_logon.SendCommand("log.blogpost.add", "",
                            "USER_ID=1767" +
                            "&POST_TITLE=тест триггер" +
                            "&POST_MESSAGE=тест триггер" +
                            "&DEST[0]=SG521"
                            );
                    }
                    catch (Exception)
                    {

                    }
                }
            }
        }
    }
}

//public void Main()
//{
//    Bitrix24 bx_logon = new Bitrix24();
//    string contents = string.Empty;
//    int i = 0;

//    DirectoryInfo di = new DirectoryInfo(@"\\10.10.0.28\alta\Robot\Data\TempReportsPirPM\");

//    foreach (var file in di.GetFiles())
//    {
//        string base64 = Convert.ToBase64String(File.ReadAllBytes(file.FullName), Base64FormattingOptions.None);
//        contents += $"&FILES[{i}][0]=" + file.Name + $"&FILES[{i}][1]=" + HttpUtility.UrlEncode(base64);
//        i++;
//    }
//    try
//    {
//        _ = bx_logon.SendCommand("log.blogpost.add", "",
//            "USER_ID=1767" +
//            "&POST_TITLE=Показатели персональных менеджеров на " + DateTime.Now.ToString("dd.MM.yyyy") +
//            "&POST_MESSAGE=." +
//            "&DEST[0]=SG402" + contents
//            );
//    }
//    catch (Exception ex)
//    {
//        Dts.Events.FireError(0, "", ex.Message, "", 0);
//    }

//    Dts.TaskResult = (int)ScriptResults.Success;

