using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace сSharpProject
{
    //24.03.20 загружены декларации приблизительно по конец ноября 2019 года.
    //Остальные есть в текущей базе
    class DeclarationParser
    {
        public void Go()
        {
            string path = @"\\10.0.0.33\Finished_Declarations\";
            int fCount = Directory.GetFiles(path, "*.xml", SearchOption.AllDirectories).Length;

            Regex r = new Regex("[_][0-9]{4}(17|18|19)[_]");

            DateTime start = DateTime.Now;

            DirectoryInfo dir = new DirectoryInfo(path);
            int counter = 0;

            foreach (var file in dir.GetFiles("*.xml", SearchOption.AllDirectories))
            {
                MatchCollection matches = r.Matches(file.FullName);

                if (matches.Count > 0)
                {
                    ParseDeclarations(file.FullName);
                }
            }

            DateTime end = DateTime.Now;

            Console.WriteLine("Начало: " + start.ToString() + "; Окончание: " + end.ToString());
            Console.WriteLine("Press Enter to exit...");
            Console.ReadLine();
        }


        public void ParseDeclarations(string filename)
        {
            try
            {
                XDocument doc = XDocument.Load(filename);

            string regNum = doc.Root.Element("REGNUM") != null ? doc.Root.Element("REGNUM").Value : null;

            string vat = doc.Root.Element("G_14_4") != null ? doc.Root.Element("G_14_4").Value : null;
            vat = vat.Contains(@"/") ? vat.Substring(0, vat.IndexOf(@"/") - 1) : vat;

            string contractNum = String.Empty;
            string contractDate = null;

            var contract = doc.Descendants("G44").Where(d => d.Element("G441").Value == "03011").Select(d1 => new
            {
                num = d1.Element("G442") != null ? d1.Element("G442").Value : null,
                dt = d1.Element("G443") != null ? d1.Element("G443").Value : null
            });

            if (contract.Count() > 0)
            {
                contractNum = contract.First().num;
                contractDate = contract.First().dt;
            }

            string releaseDate = doc.Root.Element("D_DATE") != null ? doc.Root.Element("D_DATE").Value : null;
            string releaseTime = doc.Root.Element("D_TIME") != null ? doc.Root.Element("D_TIME").Value : null;

            var tov =
            from block in doc.Descendants("BLOCK")
            join tovg in doc.Descendants("TOVG") on block.Element("G_32_1").Value equals tovg.Parent.Element("G_32_1").Value
            select new
            {
                blocknum = block.Element("G_32_1") != null ? block.Element("G_32_1").Value : String.Empty,
                commonDes = block.Element("G_31").Element("NAME") != null ? block.Element("G_31").Element("NAME").Value : String.Empty,
                hsCode = block.Element("G_33_1") != null ? block.Element("G_33_1").Value : String.Empty,
                addCode = block.Element("G_33_2") != null ? block.Element("G_33_2").Value : String.Empty,
                country = block.Element("G_34_1") != null ? block.Element("G_34_1").Value : String.Empty,
                manufacturer = tovg.Element("G31_11") != null ? tovg.Element("G31_11").Value : String.Empty,
                tradeMark = tovg.Element("G31_12") != null ? tovg.Element("G31_12").Value : String.Empty,
                tovgDes = tovg.Element("G31_1") != null ? tovg.Element("G31_1").Value : String.Empty,
                article = tovg.Element("G31_15") != null ? tovg.Element("G31_15").Value : String.Empty,
                model = tovg.Element("G31_15_MOD") != null ? tovg.Element("G31_15_MOD").Value : String.Empty
            };

            var docs = doc.Descendants("BLOCK").Join(doc.Descendants("G44"),
                b => b.Element("G_32_1").Value,
                g => g.Parent.Element("G_32_1").Value,
                (block, documents) => new
                {
                    blocknum = block.Element("G_32_1") != null ? block.Element("G_32_1").Value : String.Empty,
                    code = documents.Element("G441") != null ? documents.Element("G441").Value : String.Empty,
                    num = documents.Element("G442") != null ? documents.Element("G442").Value : String.Empty,
                    dateBegin = documents.Element("G443") != null ? documents.Element("G443").Value : String.Empty,
                    dateEnd = documents.Element("G447") != null ? documents.Element("G447").Value : String.Empty
                });

            var groupDocsSert = docs.Where(d => d.code == "01191").GroupBy(
                d1 => d1.blocknum,
                (key, document) => new
                {
                    document.First().blocknum,
                    nums = document.Aggregate(document.First().num, (total, next) => total.Contains(next.num) ? total : total + " ИЛИ " + next.num),
                    datesBegin = document.Aggregate(document.First().num, (total, next) => total.Contains(next.num) ? total : total + " ИЛИ " + next.num).Contains(" ИЛИ ") ? String.Empty : document.First().dateBegin,
                    datesEnd = document.Aggregate(document.First().num, (total, next) => total.Contains(next.num) ? total : total + " ИЛИ " + next.num).Contains(" ИЛИ ") ? String.Empty : document.First().dateEnd
                });

            var groupDocsSgr = docs.Where(d => d.code == "01206").GroupBy(
                d1 => d1.blocknum,
                (key, document) => new
                {
                    document.First().blocknum,
                    nums = document.Aggregate(document.First().num, (total, next) => total.Contains(next.num) ? total : total + " ИЛИ " + next.num),
                    datesBegin = document.Aggregate(document.First().num, (total, next) => total.Contains(next.num) ? total : total + " ИЛИ " + next.num).Contains(" ИЛИ ") ? String.Empty : document.First().dateBegin,
                    datesEnd = document.Aggregate(document.First().num, (total, next) => total.Contains(next.num) ? total : total + " ИЛИ " + next.num).Contains(" ИЛИ ") ? String.Empty : document.First().dateEnd
                });

            DataTable toSql = new DataTable();
            DataRow row;
            DataColumn[] dataColumns = new DataColumn[]
            {
                new DataColumn("regnum", typeof(string)),
                new DataColumn("vat", typeof(string)),
                new DataColumn("contract_num", typeof(string)),
                new DataColumn("contract_date", typeof(string)),
                new DataColumn("G_32_1", typeof(string)),
                new DataColumn("G_31", typeof(string)),
                new DataColumn("G_33_1", typeof(string)),
                new DataColumn("G_33_2", typeof(string)),
                new DataColumn("G_34_1", typeof(string)),
                new DataColumn("G31_11", typeof(string)),
                new DataColumn("G31_12", typeof(string)),
                new DataColumn("G31_1", typeof(string)),
                new DataColumn("G31_15", typeof(string)),
                new DataColumn("G31_15_MOD", typeof(string)),
                new DataColumn("sert_num", typeof(string)),
                new DataColumn("sert_begin", typeof(string)),
                new DataColumn("sert_end", typeof(string)),
                new DataColumn("sgr_num", typeof(string)),
                new DataColumn("sgr_begin", typeof(string)),
                new DataColumn("sgr_end", typeof(string)),
                new DataColumn("release_date", typeof(string)),
                new DataColumn("release_time", typeof(string))
            };
            toSql.Columns.AddRange(dataColumns);

            foreach (var tovar in tov)
            {
                row = toSql.NewRow();
                row["regnum"] = regNum;
                row["vat"] = vat;
                row["contract_num"] = contractNum;
                row["contract_date"] = CheckValidDate(contractDate, CheckDateType.Date);
                row["G_32_1"] = tovar.blocknum;
                row["G_31"] = tovar.commonDes;
                row["G_33_1"] = tovar.hsCode;
                row["G_33_2"] = tovar.addCode;
                row["G_34_1"] = tovar.country;
                row["G31_11"] = tovar.manufacturer;
                row["G31_12"] = tovar.tradeMark;
                row["G31_1"] = tovar.tovgDes;
                row["G31_15"] = tovar.article;
                row["G31_15_MOD"] = tovar.model;
                row["sert_num"] = groupDocsSert.Where(g => g.blocknum == tovar.blocknum).Select(g1 => g1.nums).Any() == true ? groupDocsSert.Where(g => g.blocknum == tovar.blocknum).Select(g1 => g1.nums).First().ToString() : String.Empty;
                row["sert_begin"] = CheckValidDate(groupDocsSert.Where(g => g.blocknum == tovar.blocknum).Select(g1 => g1.nums).Any() == true ? groupDocsSert.Where(g => g.blocknum == tovar.blocknum).Select(g1 => g1.datesBegin).First().ToString() : null, CheckDateType.Date);
                row["sert_end"] = CheckValidDate(groupDocsSert.Where(g => g.blocknum == tovar.blocknum).Select(g1 => g1.nums).Any() == true ? groupDocsSert.Where(g => g.blocknum == tovar.blocknum).Select(g1 => g1.datesEnd).First().ToString() : null, CheckDateType.Date);
                row["sgr_num"] = groupDocsSgr.Where(g => g.blocknum == tovar.blocknum).Select(g1 => g1.nums).Any() == true ? groupDocsSgr.Where(g => g.blocknum == tovar.blocknum).Select(g1 => g1.nums).First().ToString() : String.Empty;
                row["sgr_begin"] = CheckValidDate(groupDocsSgr.Where(g => g.blocknum == tovar.blocknum).Select(g1 => g1.nums).Any() == true ? groupDocsSgr.Where(g => g.blocknum == tovar.blocknum).Select(g1 => g1.datesBegin).First().ToString() : null, CheckDateType.Date);
                row["sgr_end"] = CheckValidDate(groupDocsSgr.Where(g => g.blocknum == tovar.blocknum).Select(g1 => g1.nums).Any() == true ? groupDocsSgr.Where(g => g.blocknum == tovar.blocknum).Select(g1 => g1.datesEnd).First().ToString() : null, CheckDateType.Date);
                row["release_date"] = CheckValidDate(releaseDate, CheckDateType.Date);
                row["release_time"] = CheckValidDate(releaseTime.Trim(), CheckDateType.Time);

                toSql.Rows.Add(row);
            }

            SqlConnectionStringBuilder connBuilder = new SqlConnectionStringBuilder
            {
                DataSource = "10.10.0.28",
                UserID = "phpuser",
                Password = "gnQCUElU"
            };

            using (SqlConnection SqlConnectionObj = new SqlConnection(connBuilder.ConnectionString))
            {
                SqlConnectionObj.Open();
                SqlBulkCopy bulkCopy = new SqlBulkCopy(SqlConnectionObj, SqlBulkCopyOptions.Default | SqlBulkCopyOptions.FireTriggers | SqlBulkCopyOptions.UseInternalTransaction, null);
                bulkCopy.DestinationTableName = @"automation.dbo.new_ref";
                bulkCopy.ColumnMappings.Add("regnum", "regnum");
                bulkCopy.ColumnMappings.Add("vat", "vat");
                bulkCopy.ColumnMappings.Add("contract_num", "contract_num");
                bulkCopy.ColumnMappings.Add("contract_date", "contract_date");
                bulkCopy.ColumnMappings.Add("G_32_1", "G_32_1");
                bulkCopy.ColumnMappings.Add("G_31", "G_31");
                bulkCopy.ColumnMappings.Add("G_33_1", "G_33_1");
                bulkCopy.ColumnMappings.Add("G_33_2", "G_33_2");
                bulkCopy.ColumnMappings.Add("G_34_1", "G_34_1");
                bulkCopy.ColumnMappings.Add("G31_11", "G31_11");
                bulkCopy.ColumnMappings.Add("G31_12", "G31_12");
                bulkCopy.ColumnMappings.Add("G31_1", "G31_1");
                bulkCopy.ColumnMappings.Add("G31_15", "G31_15");
                bulkCopy.ColumnMappings.Add("G31_15_MOD", "G31_15_MOD");
                bulkCopy.ColumnMappings.Add("sert_num", "sert_num");
                bulkCopy.ColumnMappings.Add("sert_begin", "sert_begin");
                bulkCopy.ColumnMappings.Add("sert_end", "sert_end");
                bulkCopy.ColumnMappings.Add("sgr_num", "sgr_num");
                bulkCopy.ColumnMappings.Add("sgr_begin", "sgr_begin");
                bulkCopy.ColumnMappings.Add("sgr_end", "sgr_end");
                bulkCopy.ColumnMappings.Add("release_date", "release_date");
                bulkCopy.ColumnMappings.Add("release_time", "release_time");

                bulkCopy.WriteToServer(toSql);
            }
            }
            catch (Exception e)
            {
                using (StreamWriter file = File.AppendText(@"C:\Users\andreydruzhinin\Desktop\errorlog.txt"))
                {
                    file.WriteLine("Ошибка при обработке файла " + filename + " " + e.Message);
                    File.Copy(filename, @"C:\Users\andreydruzhinin\Desktop\error\" + filename.Substring(filename.LastIndexOf(@"\") + 1));

                }
            }
        }
        private string CheckValidDate(string input, CheckDateType type)
        {
            DateTime temp;
            string format;
            CultureInfo enUS = new CultureInfo("en-US");

            if (type == CheckDateType.Date)
            {
                format = "yyyy-MM-dd";
            }
            else if (type == CheckDateType.Time)
            {
                format = "HH:mm:ss";
            }
            else
            {
                format = String.Empty;
            }

            if (DateTime.TryParseExact(input, format, enUS, DateTimeStyles.None, out temp))
            {
                return input;
            }
            else
            {
                return null;
            }
        }
        private enum CheckDateType
        {
            Date = 1,
            Time = 2
        }
    }
}
