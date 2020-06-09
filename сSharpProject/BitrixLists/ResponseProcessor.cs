using System;
using System.Data;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;

namespace BitrixLists
{
    public static class ResponseProcessor
    {
        //метод переводит Unicode-символы из ответа в читаемый вид
        //исправляет лишние двойные кавычки в элементах
        public static string FixJson(string jsonResponse)
        {
            Regex regex = new Regex(@"\\[U][0-9A-Z]{4}", RegexOptions.IgnoreCase);
            MatchCollection m = regex.Matches(jsonResponse);
            IFormatProvider prov = new CultureInfo("ru-RU");
            string temp = string.Empty;

            foreach (Match match in m)
            {
                if (int.TryParse(match.Value.Substring(2), NumberStyles.HexNumber, prov, out int output) == true)
                {
                    char c = (char)output;
                    jsonResponse = jsonResponse.Replace(match.Value, c.ToString());
                }
            }

            regex = new Regex("{\"TYPE\":\"HTML\",\"TEXT\":\".*?\"}", RegexOptions.IgnoreCase);
            m = regex.Matches(jsonResponse);           

            foreach (Match match in m)
            {
                temp = match.Value.Replace("\"", "'");

                
                temp = temp.Replace("{'TYPE':'HTML','TEXT':'", "{\"TYPE\":\"HTML\",\"TEXT\":\"");
                temp = temp.Replace("'}", "\"}");
                jsonResponse = jsonResponse.Replace(match.Value, temp);
            }

            regex = new Regex("\"PREVIEW_TEXT\":\".*?\",\"TIMESTAMP", RegexOptions.IgnoreCase);
            m = regex.Matches(jsonResponse);

            foreach (Match match in m)
            {
                temp = match.Value.Replace("\"", "'");
                temp = temp.Replace("'PREVIEW_TEXT':'", "\"PREVIEW_TEXT\":\"");
                temp = temp.Replace("','TIMESTAMP", "\",\"TIMESTAMP");
                jsonResponse = jsonResponse.Replace(match.Value, temp);
            }

            regex = new Regex("\"DETAIL_TEXT\":\".*?\",\"PREVIEW", RegexOptions.IgnoreCase);
            m = regex.Matches(jsonResponse);

            foreach (Match match in m)
            {
                temp = match.Value.Replace("\"", "'");
                temp = temp.Replace("'DETAIL_TEXT':'", "\"DETAIL_TEXT\":\"");
                temp = temp.Replace("','PREVIEW", "\",\"PREVIEW");
                jsonResponse = jsonResponse.Replace(match.Value, temp);
            }

            regex = new Regex("\"PROPERTY_[0-9]{4}\":{\".*?\":\".*?\"}", RegexOptions.IgnoreCase);
            m = regex.Matches(jsonResponse);

            foreach (Match match in m)
            {
                temp = match.Value.Replace("\"", "'");
                temp = temp.Replace("'PROPERTY_", "\"PROPERTY_");
                temp = temp.Replace("':{'", "\":{\"");
                temp = temp.Replace("':'", "\":\"");
                temp = temp.Replace("'}", "\"}");
                temp = temp.Replace("','", "\",\""); 
                jsonResponse = jsonResponse.Replace(match.Value, temp);
            }
                
            return jsonResponse;
        }

        //метод десериализует JSON, проверяет наличие записи в базе и добавляет новые записи
        //возвращаемое значение соответствует указателю для повторного запроса следующих записей или 0 если достигнут конец
        public static int ProcessNewRecords(string json)
        {
            SqlConnectionStringBuilder connBuilder = new SqlConnectionStringBuilder
            {
                DataSource = "10.10.0.28",
                UserID = "phpuser",
                Password = "gnQCUElU"
            };

            IDbConnection connection = new SqlConnection(connBuilder.ConnectionString);

            DataContext db = new DataContext(connection);
            Table<messages> msg = db.GetTable<messages>();

            DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof(RootObject), new DataContractJsonSerializerSettings
            {
                UseSimpleDictionaryFormat = true
            });

            using (MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(json)))
            {
                var result = (RootObject)deserializer.ReadObject(ms);

                foreach (var item in result.result)
                {
                    Regex reg = new Regex("[D][0-9]{6}");
                    MatchCollection match = reg.Matches(item.NAME);
                    int count = msg.Where(m => m.element_id.ToString() == item.ID).Select(m => m).Count();

                    if (match.Count > 0 && count == 0)
                    {
                        foreach (Match m in match)
                        {                         
                            msg.InsertOnSubmit(new messages
                            {
                                element_id = Convert.ToInt32(item.ID),
                                date_create = item.DATE_CREATE,
                                delivery_id = m.Value,
                                message = item.DETAIL_TEXT,
                                message_url = @"https://bitrix.eltransplus.ru/workgroups/group/330/lists/184/element/0/" + item.ID.ToString() + @"/?list_section_id=",
                                author = item.CREATED_USER_NAME,
                                task_url = @"https://bitrix.eltransplus.ru/workgroups/group/330/tasks/task/view/" + item.PROPERTY_1292.First().Value.ToString() + @"/"
                            });  
                        }                     
                    }
                }

                db.SubmitChanges();

                if (String.IsNullOrWhiteSpace(result.next) == false)
                {
                    return Convert.ToInt32(result.next);
                }
                else
                {
                    return 0;
                }
            }
        }
    }
}