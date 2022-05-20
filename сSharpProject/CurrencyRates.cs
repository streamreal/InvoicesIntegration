using System;
using System.Linq;
using System.Xml.Linq;
using System.Data;
using System.Data.SqlClient;

namespace сSharpProject
{
    class CurrencyRates
    {
        public static void Go()
        {
            string ConnectionString = @"Data Source=10.10.0.28;User ID=phpuser;Password=gnQCUElU";
            XDocument doc = XDocument.Load(@"https://www.cbr-xml-daily.ru/daily_utf8.xml");
            DateTime date = DateTime.Parse(doc.Element("ValCurs").Attribute("Date").Value);

            var query = doc.Descendants("Valute").Select(v => new
            {
                Date = date,
                NumCode = v.Element("NumCode").Value,
                CharCode = v.Element("CharCode").Value,
                Nominal = Int32.Parse(v.Element("Nominal").Value),
                Name = v.Element("Name").Value,
                Value = Decimal.Parse(v.Element("Value").Value)
            });

            DataTable toSql = new DataTable();
            DataRow row;
            DataColumn[] dataColumns = new DataColumn[]
            {
                new DataColumn("Date", typeof(DateTime)),
                new DataColumn("NumCode", typeof(string)),
                new DataColumn("CharCode", typeof(string)),
                new DataColumn("Nominal", typeof(int)),
                new DataColumn("Name", typeof(string)),
                new DataColumn("Value", typeof(decimal))
            };
            toSql.Columns.AddRange(dataColumns);

            foreach (var item in query)
            {
                if (ExistsInDatabase(item.Date, item.Name) == false)
                {
                    row = toSql.NewRow();
                    row["Date"] = item.Date;
                    row["NumCode"] = item.NumCode;
                    row["CharCode"] = item.CharCode;
                    row["Nominal"] = item.Nominal;
                    row["Name"] = item.Name;
                    row["Value"] = item.Value;
                    toSql.Rows.Add(row);
                }                
            }

            using (SqlConnection SqlConnectionObj = new SqlConnection(ConnectionString))
            {
                SqlConnectionObj.Open();
                SqlBulkCopy bulkCopy = new SqlBulkCopy(SqlConnectionObj, SqlBulkCopyOptions.Default | SqlBulkCopyOptions.FireTriggers | SqlBulkCopyOptions.UseInternalTransaction, null);
                bulkCopy.DestinationTableName = @"automation.dbo.currency_rates";
                bulkCopy.ColumnMappings.Add("Date", "Date");
                bulkCopy.ColumnMappings.Add("NumCode", "NumCode");
                bulkCopy.ColumnMappings.Add("CharCode", "CharCode");
                bulkCopy.ColumnMappings.Add("Nominal", "Nominal");
                bulkCopy.ColumnMappings.Add("Name", "Name");
                bulkCopy.ColumnMappings.Add("Value", "Value");
                bulkCopy.WriteToServer(toSql);
            }

        }
        public static bool ExistsInDatabase(DateTime date, string name)
        {
            string ConnectionString = @"Data Source=10.10.0.28;User ID=phpuser;Password=gnQCUElU";
            
            //котируются около 35 валют
            string query_contents = @"SELECT TOP (100) * FROM [automation].[dbo].[currency_rates] ORDER BY [Date] DESC";
            DataTable src_contents = new DataTable();

            using (SqlDataAdapter sda = new SqlDataAdapter(query_contents, ConnectionString))
            {
                sda.Fill(src_contents);
            }

            var query = from c in src_contents.AsEnumerable()
                        where c.Field<DateTime>("Date") == date && c.Field<string>("Name") == name
                        select c;

            return query.Any();
        }
    }
}