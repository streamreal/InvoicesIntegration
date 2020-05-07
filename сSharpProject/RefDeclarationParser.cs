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
    public static class RefDeclarationParser
    {
        public static void Run()
        {
            DirectoryInfo dir = new DirectoryInfo(@"\\10.0.0.33\Finished_Declarations\");
            Regex r = new Regex("[_][0-9]{4}(17|18|19|20)[_]");
            DateTime start = DateTime.Now;

            foreach (var file in dir.GetFiles("*.xml", SearchOption.AllDirectories))
            {
                MatchCollection matches = r.Matches(file.FullName);

                if (matches.Count > 0)
                {
                    Console.WriteLine($"Обработка файла { file.FullName }");
                    ParseDeclarations(file.FullName);
                }
            }

            DateTime end = DateTime.Now;

            Console.WriteLine("Начало: " + start.ToString() + "; Окончание: " + end.ToString());
            Console.WriteLine("Press Enter to exit...");
            Console.ReadLine();
        }

        private static void ParseDeclarations(string filename)
        {
            try
            {
                XDocument doc = XDocument.Load(filename);

                //общие данные по ДТ
                string regNum = doc.Root.Element("REGNUM") != null ? doc.Root.Element("REGNUM").Value : String.Empty;
                string regDate = String.Empty;

                Regex reg = new Regex("[/][0-9]{6}[/]");
                Match match = reg.Match(regNum);

                if (match.Success == true)
                {
                    //формат даты регистрации ммддгг
                    try
                    {
                        regDate = match.Value.Replace("/", "");
                        regDate = "20" + regDate.Substring(4) + "-" + regDate.Substring(2, 2) + "-" + regDate.Substring(0, 2);
                    }
                    catch (Exception)
                    {

                    }
                }

                string g14 = doc.Root.Element("G_14_4") != null ? doc.Root.Element("G_14_4").Value : String.Empty;
                string vat = g14.Contains(@"/") ? g14.Substring(0, g14.IndexOf(@"/") - 1) : g14;
                string kpp = g14.Contains(@"/") ? g14.Substring(g14.IndexOf(@"/") + 1) : g14;

                string name14 = doc.Root.Element("G_14_NAM") != null ? doc.Root.Element("G_14_NAM").Value : String.Empty;

                string releaseDate = doc.Root.Element("D_DATE") != null ? doc.Root.Element("D_DATE").Value : String.Empty;
                string releaseTime = doc.Root.Element("D_TIME") != null ? doc.Root.Element("D_TIME").Value : String.Empty;

                string comment = doc.Root.Element("___PRIM") != null ? doc.Root.Element("___PRIM").Value : String.Empty;

                if (comment.Length > 100)
                {
                    comment = comment.Substring(0, 100);
                }

                string parent_id = doc.Root.Element("PARENT_ID") != null ? doc.Root.Element("PARENT_ID").Value : String.Empty;
                string parent_doc = doc.Root.Element("PARENT_DOC") != null ? doc.Root.Element("PARENT_DOC").Value : String.Empty;


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
                    pref = block.Element("G_36_2") != null ? block.Element("G_36_2").Value : String.Empty,
                    procedure = block.Element("G_37_1") != null ? block.Element("G_37_1").Value : String.Empty,
                    method = block.Element("G_43_1") != null ? block.Element("G_43_1").Value : String.Empty,
                    method_decision = block.Element("G_43_2") != null ? block.Element("G_43_2").Value : String.Empty,
                    parent_pos = block.Element("PARENT_POS") != null ? block.Element("PARENT_POS").Value : String.Empty,
                    parent_gnm = block.Element("PARENT_GNM") != null ? block.Element("PARENT_GNM").Value : String.Empty,
                    free_sert = block.Element("G_33_4") != null ? block.Element("G_33_4").Value : String.Empty,
                    intellect = block.Element("G_33_5") != null ? block.Element("G_33_5").Value : String.Empty,
                    manufacturer = tovg.Element("G31_11") != null ? tovg.Element("G31_11").Value : String.Empty,
                    tradeMark = tovg.Element("G31_12") != null ? tovg.Element("G31_12").Value : String.Empty,
                    mark = tovg.Element("G31_14") != null ? tovg.Element("G31_14").Value : String.Empty,
                    tovgDes = tovg.Element("G31_1") != null ? tovg.Element("G31_1").Value : String.Empty,
                    article = tovg.Element("G31_15") != null ? tovg.Element("G31_15").Value : String.Empty,
                    model = tovg.Element("G31_15_MOD") != null ? tovg.Element("G31_15_MOD").Value : String.Empty,
                    groups = tovg.Element("G32G") != null ? tovg.Element("G32G").Value : String.Empty,
                    brutto = tovg.Element("G31_35") != null ? tovg.Element("G31_35").Value : String.Empty,
                    netto = tovg.Element("G31_38") != null ? tovg.Element("G31_38").Value : String.Empty,
                    sum = tovg.Element("G31_42") != null ? tovg.Element("G31_42").Value : String.Empty

                };

                DataTable toSql = new DataTable();
                DataRow row;
                DataColumn[] dataColumns = new DataColumn[]
                {
                new DataColumn("regnum", typeof(string)),
                new DataColumn("regdate", typeof(string)),
                new DataColumn("vat", typeof(string)),
                new DataColumn("kpp", typeof(string)),
                new DataColumn("decl_name", typeof(string)),
                new DataColumn("comment", typeof(string)),
                new DataColumn("parent_id", typeof(string)),
                new DataColumn("parent_doc", typeof(string)),
                new DataColumn("release_date", typeof(string)),
                new DataColumn("release_time", typeof(string)),
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
                new DataColumn("G_36_2", typeof(string)),
                new DataColumn("G_37_1", typeof(string)),
                new DataColumn("G_43_1", typeof(string)),
                new DataColumn("G_43_2", typeof(string)),
                new DataColumn("PARENT_POS", typeof(string)),
                new DataColumn("PARENT_GNM", typeof(string)),
                new DataColumn("G_33_4", typeof(string)),
                new DataColumn("G_33_5", typeof(string)),
                new DataColumn("G31_14", typeof(string)),
                new DataColumn("G32G", typeof(string)),
                new DataColumn("G31_35", typeof(string)),
                new DataColumn("G31_38", typeof(string)),
                new DataColumn("G31_42", typeof(string))
                };
                toSql.Columns.AddRange(dataColumns);

                foreach (var tovar in tov)
                {
                    row = toSql.NewRow();
                    row["regnum"] = regNum;
                    row["regdate"] = CheckValidDate(regDate, CheckDateType.Date);
                    row["vat"] = vat;
                    row["kpp"] = kpp;
                    row["decl_name"] = name14;
                    row["comment"] = comment;
                    row["parent_id"] = parent_id;
                    row["parent_doc"] = parent_doc;
                    row["release_date"] = CheckValidDate(releaseDate, CheckDateType.Date);
                    row["release_time"] = CheckValidDate(releaseTime.Trim(), CheckDateType.Time);
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
                    row["G_36_2"] = tovar.pref;
                    row["G_37_1"] = tovar.procedure;
                    row["G_43_1"] = tovar.method;
                    row["G_43_2"] = tovar.method_decision;
                    row["PARENT_POS"] = tovar.parent_pos;
                    row["PARENT_GNM"] = tovar.parent_gnm;
                    row["G_33_4"] = tovar.free_sert;
                    row["G_33_5"] = tovar.intellect;
                    row["G31_14"] = tovar.mark;
                    row["G32G"] = tovar.groups;
                    row["G31_35"] = tovar.brutto;
                    row["G31_38"] = tovar.netto;
                    row["G31_42"] = tovar.sum;

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
                    bulkCopy.DestinationTableName = @"automation.dbo.web_ref";

                    bulkCopy.ColumnMappings.Add("regnum", "regnum");
                    bulkCopy.ColumnMappings.Add("vat", "vat");
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
                    bulkCopy.ColumnMappings.Add("release_date", "release_date");
                    bulkCopy.ColumnMappings.Add("release_time", "release_time");
                    bulkCopy.ColumnMappings.Add("G_36_2", "G_36_2");
                    bulkCopy.ColumnMappings.Add("G_37_1", "G_37_1");
                    bulkCopy.ColumnMappings.Add("G_43_1", "G_43_1");
                    bulkCopy.ColumnMappings.Add("G_43_2", "G_43_2");
                    bulkCopy.ColumnMappings.Add("PARENT_POS", "PARENT_POS");
                    bulkCopy.ColumnMappings.Add("PARENT_GNM", "PARENT_GNM");
                    bulkCopy.ColumnMappings.Add("G_33_4", "G_33_4");
                    bulkCopy.ColumnMappings.Add("G_33_5", "G_33_5");
                    bulkCopy.ColumnMappings.Add("G31_14", "G31_14");
                    bulkCopy.ColumnMappings.Add("G32G", "G32G");
                    bulkCopy.ColumnMappings.Add("G31_35", "G31_35");
                    bulkCopy.ColumnMappings.Add("G31_38", "G31_38");
                    bulkCopy.ColumnMappings.Add("G31_42", "G31_42");
                    bulkCopy.ColumnMappings.Add("regdate", "regdate");
                    bulkCopy.ColumnMappings.Add("kpp", "kpp");
                    bulkCopy.ColumnMappings.Add("decl_name", "decl_name");
                    bulkCopy.ColumnMappings.Add("comment", "comment");
                    bulkCopy.ColumnMappings.Add("parent_id", "parent_id");
                    bulkCopy.ColumnMappings.Add("parent_doc", "parent_doc");

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
        private static string CheckValidDate(string input, CheckDateType type)
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
