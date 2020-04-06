using System;
using System.Data;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;

namespace сSharpProject
{
    class Deviation
    {
        public int ProcessNewRecords(string json)
        {
            SqlConnectionStringBuilder connBuilder = new SqlConnectionStringBuilder
            {
                DataSource = "10.10.0.28",
                UserID = "phpuser",
                Password = "gnQCUElU"
            };

            IDbConnection connection;
            connection = new SqlConnection(connBuilder.ConnectionString);

            DataContext db = new DataContext(connection);
            Table<messages> msg = db.GetTable<messages>();

            DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof(RootObject));

            var result = (RootObject)deserializer.ReadObject(new MemoryStream(Encoding.Unicode.GetBytes(json)));

            Console.WriteLine(result.next + " next");
            Console.WriteLine(result.total + " total");
            foreach (var item in result.result)
            {
                Regex reg = new Regex("[D][0-9]{6}");
                Match match = reg.Match(item.TITLE + item.DETAIL_TEXT);
                int count = msg.Where(m => m.bitrix_message_id.ToString() == item.ID && m.author_id.ToString() == item.AUTHOR_ID).Select(m => m).Count();

                if (match.Success == true && count == 0)
                {
                    //создание нового объекта для вставки
                    messages toInsert = new messages
                    {
                        bitrix_message_id = Convert.ToInt32(item.ID),
                        date_publish = item.DATE_PUBLISH,
                        delivery_id = match.Value,
                        message = item.TITLE,
                        detail = item.DETAIL_TEXT,
                        message_url = @"https://bitrix.eltransplus.ru/company/personal/user/" + item.AUTHOR_ID + @"/blog/" + item.ID + @"/",
                        blog_id = Convert.ToInt32(item.BLOG_ID),
                        author_id = Convert.ToInt32(item.AUTHOR_ID)
                    };

                    msg.InsertOnSubmit(toInsert);
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

        [Table(Name = "aberration.dbo.messages_330")]
        public class messages
        {
            [Column(IsDbGenerated = true, IsPrimaryKey = true)]
            public int id { get; set; }
            [Column]
            public int bitrix_message_id { get; set; }
            [Column]
            public string date_publish { get; set; }
            [Column]
            public string delivery_id { get; set; }
            [Column]
            public string message { get; set; }
            [Column]
            public string detail { get; set; }
            [Column]
            public string message_url { get; set; }
            [Column]
            public int blog_id { get; set; }
            [Column]
            public int author_id { get; set; }
        }

        [DataContract]
        public class RootObject
        {
            [DataMember]
            public BitrixMessage[] result { get; set; }
            [DataMember]
            public string next { get; set; }
            [DataMember]
            public string total { get; set; }
        }

        [DataContract]
        public class BitrixMessage
        {
            [DataMember]
            public string ID { get; set; }
            [DataMember]
            public string BLOG_ID { get; set; }
            [DataMember]
            public string TITLE { get; set; }
            [DataMember]
            public string AUTHOR_ID { get; set; }
            [DataMember]
            public string DETAIL_TEXT { get; set; }
            [DataMember]
            public string DATE_PUBLISH { get; set; }
        }
    }
}