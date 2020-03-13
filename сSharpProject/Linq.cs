using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Xml.Linq;

namespace сSharpProject
{
    class Linq
    {
        public void Go()
        {
            SqlConnectionStringBuilder connBuilder = new SqlConnectionStringBuilder
            {
                DataSource = "10.10.0.28",
                UserID = "phpuser",
                Password = "gnQCUElU"
            };

            string query_contents = @"SELECT * FROM [automation].[dbo].[request_contents] WHERE [guid] = '" + "8595FE9A-901A-4511-89BB-9402F50A951A" + "' ORDER BY [item_num] ASC, [group_num] ASC";
            string query_header = @"SELECT * FROM [automation].[dbo].[request_header] WHERE [guid] = '" + "8595FE9A-901A-4511-89BB-9402F50A951A" + "'";
            string query_documents = @"SELECT * FROM [automation].[dbo].[request_documents] WHERE [guid] = '" + "8595FE9A-901A-4511-89BB-9402F50A951A" + "' ORDER BY [G441] ASC";

            DataTable src_contents = new DataTable();
            DataTable src_header = new DataTable();
            DataTable src_documents = new DataTable();

            using (SqlDataAdapter sda = new SqlDataAdapter(query_contents, connBuilder.ConnectionString))
            {
                sda.Fill(src_contents);
            }

            using (SqlDataAdapter sda = new SqlDataAdapter(query_header, connBuilder.ConnectionString))
            {
                sda.Fill(src_header);
            }

            using (SqlDataAdapter sda = new SqlDataAdapter(query_documents, connBuilder.ConnectionString))
            {
                sda.Fill(src_documents);
            }

            XElement block = new XElement("TEMP_ROOT", src_contents.AsEnumerable().GroupBy(g1 => g1.Field<int>("item_num")).Select(g1a => new object[]
                        {
                            new XElement("BLOCK", new object[]
                            {
                                new XElement("G_31", new object[]
                                {
                                    new XElement("NAME", new XAttribute("Pref", "1-"), new XText(g1a.First().Field<string>("decl_common_des"))),
                                    new XElement("PL", new XAttribute("Pref", "\r\n" + "2-"), ""),
                                    new XElement("PLACE", g1a.First().Field<int>("item_packages")),
                                    new XElement("PLACE2", ", " + g1a.First().Field<string>("packing_code") + "-" +g1a.First().Field<int>("item_packages"))
                                }),
                                new XElement("G_32_1", g1a.Key),
                                new XElement("G_33_1", g1a.First().Field<string>("decl_hs_code")),
                                new XElement("G_34_1", g1a.First().Field<string>("country_code_let")),
                                new XElement("G_35_1", g1a.First().Field<decimal>("gross")),
                                new XElement("G_37_1", "4000000"),
                                new XElement("G_38_1", g1a.First().Field<decimal>("net")),
                                new XElement("G_42_1", g1a.First().Field<decimal>("price")),
                                new XElement("G_43_1", "1"),
                                new XElement("PARENT_POS", g1a.Key),
                                new XElement("PARENT_GNM", "1"),

                                new XElement("TEMP_TOVG", src_contents.AsEnumerable().Where(g2 => g2.Field<int>("item_num") == g1a.Key).Select(g2a => new object[]
                                {
                                    new XElement("TOVG", new object[]
                                    {
                                        new XElement("G32G", g2a.Field<int>("group_num")),
                                        new XElement("G31_1", g2a.Field<string>("decl_tovg_des")),
                                        new XElement("G31_11", g2a.Field<string>("manufacturer")),
                                        new XElement("G31_12", g2a.Field<string>("trade_mark")),
                                        new XElement("G31_15_MOD", g2a.Field<string>("model")),
                                        new XElement("G31_15", g2a.Field<string>("article")),
                                        new XElement("KOLVO", g2a.Field<decimal>("qty")),
                                        new XElement("CODE_EDI", g2a.Field<string>("unit_code_num")),
                                        new XElement("NAME_EDI", g2a.Field<string>("unit_measurement")),
                                        new XElement("G31_35", g2a.Field<decimal>("gross")),
                                        new XElement("G31_38", g2a.Field<decimal>("net")),
                                        new XElement("G31_42", g2a.Field<decimal>("price")),
                                        new XElement("INVOICCOST", g2a.Field<decimal>("price")),
                                    })
                                })),

                                new XElement("TEMP_G44", src_documents.AsEnumerable().Where(g3 => g3.Field<int>("item_num") == g1a.Key).Select(g3a => new object[]
                                {
                                    new XElement("G44", new object[]
                                    {
                                        new XElement("G4403", g3a.Field<string>("G4403")),
                                        new XElement("G441", g3a.Field<string>("G441")),
                                        new XElement("G442", g3a.Field<string>("G442")),
                                        new XElement("G442R", g3a.Field<string>("G442R")),
                                        new XElement("G443", g3a.Field<DateTime?>("G443_FROM")),
                                        new XElement("G447", g3a.Field<DateTime?>("G443_TO")),
                                        new XElement("G444", g3a.Field<string>("G444")),
                                        new XElement("BACK", "0"),
                                        new XElement("ED_TYP", ""),
                                        new XElement("ED_ID", g3a.Field<string>("ED_ID")),
                                        new XElement("ED_STAT", ""),
                                        new XElement("FACE", "false"),
                                        new XElement("DOCTEXT", g3a.Field<string>("conc"))
                                    })
                                }))
        })}));

            //удаление вспомогательных узлов            
            foreach (var x in block.Descendants("TEMP_TOVG").Reverse())
            {
                x.AddAfterSelf(x.Nodes());
                x.Remove();
            }
            foreach (var x in block.Descendants("TEMP_G44").Reverse())
            {
                x.AddAfterSelf(x.Nodes());
                x.Remove();
            }

            XDeclaration xdec = new XDeclaration("1.0", "windows-1251", null);
            XDocument declaration = new XDocument(xdec, new XElement("AltaGTD", new object[]
            {
                    new XElement("G_1_1", "ИМ"),
                    new XElement("G_1_2", "40"),
                    new XElement("G_1_31", "ЭД"),

                    new XElement("G_2_NAM", src_header.Rows[0].Field<string>("G022")),
                    new XElement("G_2_7", src_header.Rows[0].Field<string>("G0231")),
                    new XElement("G_2_POS", src_header.Rows[0].Field<string>("G023POST")),
                    new XElement("G_2_SUB", src_header.Rows[0].Field<string>("G023SUBD")),
                    new XElement("G_2_CIT", src_header.Rows[0].Field<string>("G023CITY")),
                    new XElement("G_2_STR", src_header.Rows[0].Field<string>("G023STREET")),
                    new XElement("G_2_BLD", src_header.Rows[0].Field<string>("G023BUILD")),

                    new XElement("G_3_1", "1"),
                    new XElement("G_3_2", src_header.Rows[0].Field<decimal>("forms_3")),
                    new XElement("G_5_1", src_header.Rows[0].Field<int>("tov_qty_5")),
                    new XElement("G_6_1", src_header.Rows[0].Field<int>("pack_qty_6")),

                    new XElement("G_7_1", new object[]
                    {
                        new XElement("POST", src_header.Rows[0].Field<string>("num_customs_A")),
                        new XElement("DATE", new XAttribute("Pref", "/"), new XText(src_header.Rows[0].Field<string>("num_date_A"))),
                        new XElement("NUM", new XAttribute("Pref", "/"), new XText(src_header.Rows[0].Field<string>("num_counter_A")))
                    }),

                    new XElement("G_8_1", src_header.Rows[0].Field<string>("G140")),
                    new XElement("G_8_50", src_header.Rows[0].Field<string>("G1431N")),
                    new XElement("G_8_6", src_header.Rows[0].Field<string>("G141") + "/" + src_header.Rows[0].Field<string>("G147")),
                    new XElement("G_8_7", src_header.Rows[0].Field<string>("G1431")),
                    new XElement("G_8_NAM", src_header.Rows[0].Field<string>("G142")),
                    new XElement("G_8_POS", src_header.Rows[0].Field<string>("G143POST")),
                    new XElement("G_8_SUB", src_header.Rows[0].Field<string>("G143SUBD")),
                    new XElement("G_8_CIT", src_header.Rows[0].Field<string>("G143CITY")),
                    new XElement("G_8_STR", src_header.Rows[0].Field<string>("G143STREET")),
                    new XElement("G_8_BLD", src_header.Rows[0].Field<string>("G143BUILD")),
                    new XElement("G_8_SM14", "true"),

                    new XElement("G_9_1", src_header.Rows[0].Field<string>("G140")),
                    new XElement("G_9_CN", src_header.Rows[0].Field<string>("G1431N")),
                    new XElement("G_9_4", src_header.Rows[0].Field<string>("G141") + "/" + src_header.Rows[0].Field<string>("G147")),
                    new XElement("G_9_CC", src_header.Rows[0].Field<string>("G1431")),
                    new XElement("G_9_NAM", src_header.Rows[0].Field<string>("G142")),
                    new XElement("G_9_POS", src_header.Rows[0].Field<string>("G143POST")),
                    new XElement("G_9_SUB", src_header.Rows[0].Field<string>("G143SUBD")),
                    new XElement("G_9_CIT", src_header.Rows[0].Field<string>("G143CITY")),
                    new XElement("G_9_STR", src_header.Rows[0].Field<string>("G143STREET")),
                    new XElement("G_9_BLD", src_header.Rows[0].Field<string>("G143BUILD")),
                    new XElement("G_9_SM14", "true"),
                    new XElement("G_9_7", src_header.Rows[0].Field<string>("G1431")),

                    new XElement("G_11_1", src_header.Rows[0].Field<string>("G11")),

                    new XElement("G_14_1", src_header.Rows[0].Field<string>("G140")),
                    new XElement("G_14_CN", src_header.Rows[0].Field<string>("G1431N")),
                    new XElement("G_14_4", src_header.Rows[0].Field<string>("G141") + "/" + src_header.Rows[0].Field<string>("G147")),
                    new XElement("G_14_CC", src_header.Rows[0].Field<string>("G1431")),
                    new XElement("G_14_NAM", src_header.Rows[0].Field<string>("G142")),
                    new XElement("G_14_POS", src_header.Rows[0].Field<string>("G143POST")),
                    new XElement("G_14_SUB", src_header.Rows[0].Field<string>("G143SUBD")),
                    new XElement("G_14_CIT", src_header.Rows[0].Field<string>("G143CITY")),
                    new XElement("G_14_STR", src_header.Rows[0].Field<string>("G143STREET")),
                    new XElement("G_14_BLD", src_header.Rows[0].Field<string>("G143BUILD")),

                    new XElement("G_15_1", src_header.Rows[0].Field<string>("G15")),
                    new XElement("G_15A_1", src_header.Rows[0].Field<string>("G15A")),

                    new XElement("G_16_1", src_header.Rows[0].Field<string>("country_name_16")),
                    new XElement("G_16_2", src_header.Rows[0].Field<string>("country_code_16")),

                    new XElement("G_17_1", "РОССИЯ"),
                    new XElement("G_17A_1", "RU"),

                    new XElement("G_18_0", src_header.Rows[0].Field<int>("count_transport_18")),
                    new XElement("G_18", src_header.Rows[0].Field<string>("man_TrainTransportDesc")),
                    new XElement("G_20_20", src_header.Rows[0].Field<string>("G202")),
                    new XElement("G_20_21", src_header.Rows[0].Field<string>("G2021")),
                    new XElement("G_21_0", "1"),
                    new XElement("G_22_1", src_header.Rows[0].Field<string>("currency_code")),
                    new XElement("G_22_2", src_header.Rows[0].Field<decimal>("price_sum_22")),
                    new XElement("G_22_3", src_header.Rows[0].Field<string>("currency_22")),

                    new XElement("G_25_1", src_header.Rows[0].Field<string>("transport_code_25")),
                    new XElement("G_26_1", src_header.Rows[0].Field<string>("transport_code_26")),
                    new XElement("G_28_12", src_header.Rows[0].Field<string>("G28212")),
                    new XElement("G_28_21", src_header.Rows[0].Field<string>("G28221")),

                    new XElement("G_30_0", "99"),
                    new XElement("G_30_CC", src_header.Rows[0].Field<string>("term_G30A")),
                    new XElement("G_30_SUB", src_header.Rows[0].Field<string>("term_G30SUBD")),
                    new XElement("G_30_CIT", src_header.Rows[0].Field<string>("term_G30CITY")),
                    new XElement("G_30_STR", src_header.Rows[0].Field<string>("term_G30STREET")),
                    new XElement("G_30_BLD", src_header.Rows[0].Field<string>("term_G30BUILD")),
                    new XElement("G_30_12", src_header.Rows[0].Field<string>("term_G3012")),
                    new XElement("G_30_DOP", "false"),

                    block.Elements("BLOCK"),

                    new XElement("G_54_1", "Г. ЯРЦЕВО"),
                    new XElement("G_54_21", "8(905)502-00-15, 8(929)681-92-75, 8(926)539-89-18"),
                    new XElement("G_54_3", "ПЕТРОВА"),
                    new XElement("G_54_3NM", "ОЛЬГА"),
                    new XElement("G_54_3MD", "АЛЕКСАНДРОВНА"),
                    new XElement("G_54_4", "ДОВЕРЕННОСТЬ"),
                    new XElement("G_54_5", "31"),
                    new XElement("G_54_60", "2020-02-11"),
                    new XElement("G_54_61", "2020-06-17"),
                    new XElement("G_54_7", "МЛАДШИЙ СПЕЦИАЛИСТ ПО ОФОРМЛ. ТАМ. ДЕКЛАРАЦИЙ"),
                    new XElement("G_54_8", "RU01001"),
                    new XElement("G_54_9", "ПАСПОРТ"),
                    new XElement("G_54_100", "506346"),
                    new XElement("G_54_101", "2009-11-23"),
                    new XElement("G_54_0", "false"),
                    new XElement("G_54_110", src_header.Rows[0].Field<string>("G5411")),
                    new XElement("G_54_111", src_header.Rows[0].Field<DateTime?>("G5411D")),
                    new XElement("G_54_12", "66 09"),
                    new XElement("G_54_13", "ОТДЕЛОМ ВНУТРЕННИХ ДЕЛ ЯРЦЕВСКОГО РАЙОНА СМОЛЕНСКОЙ ОБЛАСТИ"),
                    new XElement("G_54_150", "0035"),
                    new XElement("G_54_15", "7728219369/772701001"),

                    new XElement("___PRIM", new XAttribute("Name", "&&PRIM"), new XText(src_header.Rows[0].Field<string>("Delivery")))

                    }));


            declaration.Save(@"\\10.10.0.28\alta\Robot\declaration.xml");
            //File.Create(@"C:\Users\andreydruzhinin\Desktop\IN\eoj.txt");
            Console.WriteLine("ДТ СОЗДАНА");
            Console.ReadLine();
        }

        public void Go_inv()
        {
            SqlConnectionStringBuilder connBuilder = new SqlConnectionStringBuilder
            {
                DataSource = "10.10.0.28",
                UserID = "phpuser",
                Password = "gnQCUElU"
            };

            string query_contents = @"SELECT * FROM [automation].[dbo].[request_contents] WHERE [guid] = '" + "8595FE9A-901A-4511-89BB-9402F50A951A" + "' ORDER BY [row_id] ASC";
            string query_header = @"SELECT * FROM [automation].[dbo].[request_header] WHERE [guid] = '" + "8595FE9A-901A-4511-89BB-9402F50A951A" + "'";
            
            DataTable src_contents = new DataTable();
            DataTable src_header = new DataTable();

            using (SqlDataAdapter sda = new SqlDataAdapter(query_contents, connBuilder.ConnectionString))
            {
                sda.Fill(src_contents);
            }

            using (SqlDataAdapter sda = new SqlDataAdapter(query_header, connBuilder.ConnectionString))
            {
                sda.Fill(src_header);
            }
            
            XElement block = new XElement("TEMP_ROOT", src_contents.AsEnumerable().Select(g1a => new object[]
                        {
                            new XElement("BLOCK", new object[]
                            {
                                new XElement("G_7_NUM", "1"),
                                new XElement("G_32_NUM", g1a.Field<int>("item_num")),
                                new XElement("G_31_DESCRIPT", g1a.Field<string>("decl_tovg_des")),
                                new XElement("G_31_FIRMA", g1a.Field<string>("manufacturer") != null ? g1a.Field<string>("manufacturer").Trim() : null),
                                new XElement("G_31_TM", g1a.Field<string>("trade_mark") != null ? g1a.Field<string>("trade_mark").Trim() : null),
                                new XElement("G_31_MODEL", g1a.Field<string>("model") != null ? g1a.Field<string>("model").Trim() : null),
                                new XElement("G_31_ARTICUL", g1a.Field<string>("article") != null ? g1a.Field<string>("article").Trim() : null),
                                new XElement("G_32_1", g1a.Field<int>("row_id")),
                                new XElement("G_33_1", g1a.Field<string>("decl_hs_code")),
                                new XElement("G_33_2", g1a.Field<string>("decl_hs_code_add")),
                                new XElement("G_34_1", g1a.Field<string>("country_code_num")),
                                new XElement("G_34_ALFA", g1a.Field<string>("country_code_let")),
                                new XElement("G_34_TEXT", g1a.Field<string>("country")),
                                new XElement("G_31_QUNT", g1a.Field<decimal>("qty") == Decimal.Round(g1a.Field<decimal>("qty"), 0) ? Decimal.Round(g1a.Field<decimal>("qty"), 0) : Decimal.Round(g1a.Field<decimal>("qty"), 4)),
                                new XElement("G_31_TEXT", g1a.Field<string>("unit_measurement")),
                                new XElement("G_31_EDCODE", g1a.Field<string>("unit_code_num")),
                                new XElement("G_35_1", g1a.Field<decimal>("gross").ToString("0.000")),
                                new XElement("G_35_SHTUKA", (g1a.Field<decimal>("gross") / g1a.Field<decimal>("qty")).ToString("0.0000")),
                                new XElement("G_38_1", g1a.Field<decimal>("net").ToString("0.000")),
                                new XElement("G_38_SHTUKA", (g1a.Field<decimal>("net") / g1a.Field<decimal>("qty")).ToString("0.0000")),
                                new XElement("G_42_ZASUM", g1a.Field<decimal>("price").ToString("0.00")),
                                new XElement("G_42_ZAEDINIC", (g1a.Field<decimal>("price") / g1a.Field<decimal>("qty")).ToString("0.0000")),

                                new XElement("G_31_TARASUM", g1a.Field<int>("item_packages")),
                                new XElement("G_31_TARALFA", g1a.Field<string>("packing_code")),
                                new XElement("PEXTALFA", g1a.Field<string>("packing_code")),

                                
                                new XElement("G441_4", "04021"),
                                new XElement("G442_4", new object[]
                                {
                                    new XElement("NUM", g1a.Field<string>("inv_num"))
                                }),
                                new XElement("G443_4", g1a.Field<DateTime?>("inv_date") != null ? g1a.Field<DateTime?>("inv_date").Value.ToString("yyyy-MM-dd") : ""),
                                
                                
                                new XElement("G441_10", "03011"),
                                new XElement("G442_10", new object[]
                                {
                                    new XElement("NUM", g1a.Field<string>("contract_num"))
                                }),
                                new XElement("G443_10", g1a.Field<DateTime?>("contract_date") != null ? g1a.Field<DateTime?>("contract_date").Value.ToString("yyyy-MM-dd") : ""),

                                new XElement("G441_6", g1a.Field<string>("decl_sgr_num") == "" ? "" : "01206"),
                                new XElement("G442_6", new object[]
                                {
                                    new XElement("NUM", g1a.Field<string>("decl_sgr_num"))
                                }),
                                new XElement("G443_6", g1a.Field<DateTime?>("decl_sgr_from") != null ? g1a.Field<DateTime?>("decl_sgr_from").Value.ToString("yyyy-MM-dd") : ""),
                                new XElement("G446_6", g1a.Field<DateTime?>("decl_sgr_from") != null ? g1a.Field<DateTime?>("decl_sgr_from").Value.ToString("yyyy-MM-dd") : ""),
                                new XElement("G447_6", g1a.Field<DateTime?>("decl_sgr_to") != null ? g1a.Field<DateTime?>("decl_sgr_to").Value.ToString("yyyy-MM-dd") : ""),

                                new XElement("G441_7", g1a.Field<string>("decl_sert_num") == "" ? "" : "01191"),
                                new XElement("G442_7", new object[]
                                {
                                    new XElement("NUM", g1a.Field<string>("decl_sert_num"))
                                }),
                                new XElement("G443_7", g1a.Field<DateTime?>("decl_sert_from") != null ? g1a.Field<DateTime?>("decl_sert_from").Value.ToString("yyyy-MM-dd") : ""),
                                new XElement("G446_7", g1a.Field<DateTime?>("decl_sert_from") != null ? g1a.Field<DateTime?>("decl_sert_from").Value.ToString("yyyy-MM-dd") : ""),
                                new XElement("G447_7", g1a.Field<DateTime?>("decl_sert_to") != null ? g1a.Field<DateTime?>("decl_sert_to").Value.ToString("yyyy-MM-dd") : "")
                               
        })}));
            
            XDeclaration xdec = new XDeclaration("1.0", "windows-1251", null);
            XDocument invoice = new XDocument(xdec, new XElement("AltaINV", new object[]
            {
                new XElement("INV_NUM", src_header.Rows[0].Field<string>("num_counter_A")),
                new XElement("INV_DATE", DateTime.Now.ToString("yyyy-MM-dd")),
                new XElement("___PRIM", new XAttribute("Name", "&&PRIM"), new XText(src_header.Rows[0].Field<string>("Delivery"))),
                
                new XElement("G_2_NAME", src_header.Rows[0].Field<string>("G022")),
                new XElement("G_2_KODSTR", src_header.Rows[0].Field<string>("G0231")),
                new XElement("G_2_POS", src_header.Rows[0].Field<string>("G023POST")),
                new XElement("G_2_SUB", src_header.Rows[0].Field<string>("G023SUBD")),
                new XElement("G_2_CIT", src_header.Rows[0].Field<string>("G023CITY")),
                new XElement("G_2_STREET", src_header.Rows[0].Field<string>("G023STREET")),
                new XElement("G_2_BLD", src_header.Rows[0].Field<string>("G023BUILD")),

                new XElement("G_8_1", src_header.Rows[0].Field<string>("G140")),                
                new XElement("G_8_STR", src_header.Rows[0].Field<string>("G1431N")),              
                new XElement("G_8_INN", src_header.Rows[0].Field<string>("G141")),
                new XElement("G_8_KPP", src_header.Rows[0].Field<string>("G147")),
                new XElement("G_8_KODSTR", src_header.Rows[0].Field<string>("G1431")),                
                new XElement("G_8_NAME", src_header.Rows[0].Field<string>("G142")),
                new XElement("G_8_POS", src_header.Rows[0].Field<string>("G143POST")),
                new XElement("G_8_SUB", src_header.Rows[0].Field<string>("G143SUBD")),
                new XElement("G_8_CIT", src_header.Rows[0].Field<string>("G143CITY")),
                new XElement("G_8_STREET", src_header.Rows[0].Field<string>("G143STREET")),
                new XElement("G_8_BLD", src_header.Rows[0].Field<string>("G143BUILD")),

                new XElement("G_9_1", src_header.Rows[0].Field<string>("G140")),
                new XElement("G_9_STR", src_header.Rows[0].Field<string>("G1431N")),
                new XElement("G_9_INN", src_header.Rows[0].Field<string>("G141")),
                new XElement("G_9_KPP", src_header.Rows[0].Field<string>("G147")),
                new XElement("G_9_KODSTR", src_header.Rows[0].Field<string>("G1431")),
                new XElement("G_9_NAME", src_header.Rows[0].Field<string>("G142")),
                new XElement("G_9_POS", src_header.Rows[0].Field<string>("G143POST")),
                new XElement("G_9_SUB", src_header.Rows[0].Field<string>("G143SUBD")),
                new XElement("G_9_CIT", src_header.Rows[0].Field<string>("G143CITY")),
                new XElement("G_9_STREET", src_header.Rows[0].Field<string>("G143STREET")),
                new XElement("G_9_BLD", src_header.Rows[0].Field<string>("G143BUILD")),

                new XElement("G_14_1", src_header.Rows[0].Field<string>("G140")),
                new XElement("G_14_STR", src_header.Rows[0].Field<string>("G1431N")),
                new XElement("G_14_INN", src_header.Rows[0].Field<string>("G141")),
                new XElement("G_14_KPP", src_header.Rows[0].Field<string>("G147")),
                new XElement("G_14_KODSTR", src_header.Rows[0].Field<string>("G1431")),
                new XElement("G_14_NAME", src_header.Rows[0].Field<string>("G142")),
                new XElement("G_14_POS", src_header.Rows[0].Field<string>("G143POST")),
                new XElement("G_14_SUB", src_header.Rows[0].Field<string>("G143SUBD")),
                new XElement("G_14_CIT", src_header.Rows[0].Field<string>("G143CITY")),
                new XElement("G_14_STREET", src_header.Rows[0].Field<string>("G143STREET")),
                new XElement("G_14_BLD", src_header.Rows[0].Field<string>("G143BUILD")),

                new XElement("BUY_OGRN", src_header.Rows[0].Field<string>("G140")),
                new XElement("BUY_STR", src_header.Rows[0].Field<string>("G1431N")),
                new XElement("BUY_INN", src_header.Rows[0].Field<string>("G141")),
                new XElement("BUY_KPP", src_header.Rows[0].Field<string>("G147")),
                new XElement("BUY_KODSTR", src_header.Rows[0].Field<string>("G1431")),
                new XElement("BUY_NAME", src_header.Rows[0].Field<string>("G142")),
                new XElement("BUY_POS", src_header.Rows[0].Field<string>("G143POST")),
                new XElement("BUY_SUB", src_header.Rows[0].Field<string>("G143SUBD")),
                new XElement("BUY_CIT", src_header.Rows[0].Field<string>("G143CITY")),
                new XElement("BUY_STREET", src_header.Rows[0].Field<string>("G143STREET")),
                new XElement("BUY_BLD", src_header.Rows[0].Field<string>("G143BUILD") ),
                
                new XElement("DEPART", src_header.Rows[0].Field<string>("G15")),
                new XElement("DEPART_ALFA", src_header.Rows[0].Field<string>("G15A")),
                new XElement("FINAL", "РОССИЯ"),
                new XElement("FINAL_ALFA", "RU"),
                new XElement("ORIGIN", src_header.Rows[0].Field<string>("country_name_16")),
                new XElement("ORIGIN_ALFA", src_header.Rows[0].Field<string>("country_code_16")),
                new XElement("TRADER_ALFA", src_header.Rows[0].Field<string>("G11")),
                new XElement("G_1_1", "ИМ"),
                new XElement("G_1_2", "40"),
                new XElement("G_1_3", src_header.Rows[0].Field<string>("ptd_sign_7")),
                new XElement("G_1_31", "true"),
                new XElement("G_22_1", src_header.Rows[0].Field<string>("currency_code")),                    
                new XElement("G_22_3", src_header.Rows[0].Field<string>("currency_22")),                
                new XElement("G_24_1", src_header.Rows[0].Field<string>("G28212")),
                new XElement("G_24_2", src_header.Rows[0].Field<string>("G28221")),
                new XElement("METHOD", "1"),
                new XElement("G_43_1", "1"),
                new XElement("G_37_1", "4000000"),
                new XElement("VIDT_1", src_header.Rows[0].Field<string>("transport_code_26")),
                new XElement("NUMT", src_header.Rows[0].Field<string>("man_TrainTransportDesc")),
                new XElement("QUANTT", src_header.Rows[0].Field<int>("count_transport_18")),
                new XElement("USL", src_header.Rows[0].Field<string>("G202")),
                new XElement("G_20_21", src_header.Rows[0].Field<string>("G2021")),
                
                block.Elements("BLOCK")

                    }));

            invoice.Save(@"\\10.10.0.28\alta\Robot\Alta_import\" + "inv_" + src_header.Rows[0].Field<string>("num_counter_A").Trim() + ".xml");
            //File.Create(@"C:\Users\andreydruzhinin\Desktop\IN\eoj.txt");
            Console.WriteLine("ИНВОЙС СОЗДАН");
            Console.ReadLine();
        }
    }
}
