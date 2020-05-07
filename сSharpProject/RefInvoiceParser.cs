using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace сSharpProject
{
    public class RefInvoiceParser
    {

        SqlConnectionStringBuilder connBuilderNav = new SqlConnectionStringBuilder
        {
            DataSource = "10.0.0.33",
            UserID = "phpuser",
            Password = "gnQCUElU"
        };

        SqlConnectionStringBuilder connBuilderAlta = new SqlConnectionStringBuilder
        {
            DataSource = "10.10.0.28",
            UserID = "phpuser",
            Password = "gnQCUElU"
        };
        public RefInvoiceParser()
        {
        }

        public void Run()
        {
            DirectoryInfo dir = new DirectoryInfo(@"C:\11\INV\");
            DateTime start = DateTime.Now;
            int filesCount = dir.GetFiles("*.xml", SearchOption.AllDirectories).Count();
            int counter = 0;

            foreach (var file in dir.GetFiles("*.xml", SearchOption.AllDirectories))
            {
                Console.WriteLine($"{ DateTime.Now.ToString() } Обработка файла { file.FullName } № { ++counter } из { filesCount }");
                ParseInvoice(file.FullName);
            }

            DateTime end = DateTime.Now;

            Console.WriteLine("Начало: " + start.ToString() + "; Окончание: " + end.ToString());
            Console.WriteLine("Press Enter to exit...");
            Console.ReadLine();
        }

        private void ParseInvoice(string filename)
        {
            try
            {
                XDocument doc = XDocument.Load(filename);
                SqlDataReader reader;
                int count = 0;

                string ID = doc.Root.Element("ID") != null ? doc.Root.Element("ID").Value : String.Empty;

                if (String.IsNullOrEmpty(ID) == false)
                {
                    using (SqlConnection conn = new SqlConnection(connBuilderAlta.ConnectionString))
                    {
                        conn.Open();
                        SqlCommand command = new SqlCommand($"SELECT CASE WHEN EXISTS(SELECT 0 FROM[automation].[dbo].[web_ref] WHERE parent_id = '{ ID }') THEN 1 ELSE 0 END", conn);
                        command.CommandType = CommandType.Text;
                        reader = command.ExecuteReader();

                        if (reader.Read() == true)
                        {
                            count = reader.GetInt32(0);
                        }
                    }  
                }

                if (count > 0)
                {
                    string INV_NUM = doc.Root.Element("INV_NUM") != null ? doc.Root.Element("INV_NUM").Value : String.Empty;
                    string INV_DATE = doc.Root.Element("INV_DATE") != null ? doc.Root.Element("INV_DATE").Value : String.Empty;
                    string G_14_NAME = doc.Root.Element("G_14_NAME") != null ? doc.Root.Element("G_14_NAME").Value : String.Empty;
                    string G_14_KPP = doc.Root.Element("G_14_KPP") != null ? doc.Root.Element("G_14_KPP").Value : String.Empty;
                    string G_14_INN = doc.Root.Element("G_14_INN") != null ? doc.Root.Element("G_14_INN").Value : String.Empty;

                    var tov = from block in doc.Descendants("BLOCK")
                              select new
                              {
                                  G_32_1 = block.Element("G_32_1") != null ? block.Element("G_32_1").Value : String.Empty,
                                  G_33_1 = block.Element("G_33_1") != null ? block.Element("G_33_1").Value : String.Empty,
                                  G_33_2 = block.Element("G_33_2") != null ? block.Element("G_33_2").Value : String.Empty,
                                  G_33_4 = block.Element("G_33_4") != null ? block.Element("G_33_4").Value : String.Empty,
                                  G_33_5 = block.Element("G_33_5") != null ? block.Element("G_33_5").Value : String.Empty,
                                  G_34_1 = block.Element("G_34_1") != null ? block.Element("G_34_1").Value : String.Empty,
                                  G_34_ALFA = block.Element("G_34_ALFA") != null ? block.Element("G_34_ALFA").Value : String.Empty,
                                  G_34_TEXT = block.Element("G_34_TEXT") != null ? block.Element("G_34_TEXT").Value : String.Empty,
                                  G_31_ARTICUL = block.Element("G_31_ARTICUL") != null ? block.Element("G_31_ARTICUL").Value : String.Empty,
                                  G_31_DESCRIPT = block.Element("G_31_DESCRIPT") != null ? block.Element("G_31_DESCRIPT").Value : String.Empty,
                                  G_31_DESCRENG = block.Element("G_31_DESCRENG") != null ? block.Element("G_31_DESCRENG").Value : String.Empty,
                                  G_31_COMENT = block.Element("G_31_COMENT") != null ? block.Element("G_31_COMENT").Value : String.Empty,
                                  G_31_SIZE = block.Element("G_31_SIZE") != null ? block.Element("G_31_SIZE").Value : String.Empty,
                                  G_31_COLOR = block.Element("G_31_COLOR") != null ? block.Element("G_31_COLOR").Value : String.Empty,
                                  G_31_SN = block.Element("G_31_SN") != null ? block.Element("G_31_SN").Value : String.Empty,
                                  G_31_BRAND = block.Element("G_31_BRAND") != null ? block.Element("G_31_BRAND").Value : String.Empty,
                                  G_31_MODEL = block.Element("G_31_MODEL") != null ? block.Element("G_31_MODEL").Value : String.Empty,
                                  G_31_GOST = block.Element("G_31_GOST") != null ? block.Element("G_31_GOST").Value : String.Empty,
                                  G_31_OKP = block.Element("G_31_OKP") != null ? block.Element("G_31_OKP").Value : String.Empty,
                                  G_31_CAS = block.Element("G_31_CAS") != null ? block.Element("G_31_CAS").Value : String.Empty,
                                  G_31_SORT = block.Element("G_31_SORT") != null ? block.Element("G_31_SORT").Value : String.Empty,
                                  G_31_PORODA = block.Element("G_31_PORODA") != null ? block.Element("G_31_PORODA").Value : String.Empty,
                                  G_31_LES = block.Element("G_31_LES") != null ? block.Element("G_31_LES").Value : String.Empty,
                                  G_31_HEIGHT = block.Element("G_31_HEIGHT") != null ? block.Element("G_31_HEIGHT").Value : String.Empty,
                                  G_31_WIDTH = block.Element("G_31_WIDTH") != null ? block.Element("G_31_WIDTH").Value : String.Empty,
                                  G_31_LENGTH = block.Element("G_31_LENGTH") != null ? block.Element("G_31_LENGTH").Value : String.Empty,
                                  G_31_DIAMETR = block.Element("G_31_DIAMETR") != null ? block.Element("G_31_DIAMETR").Value : String.Empty,
                                  G_31_FIRMA = block.Element("G_31_FIRMA") != null ? block.Element("G_31_FIRMA").Value : String.Empty,
                                  G_31_TM = block.Element("G_31_TM") != null ? block.Element("G_31_TM").Value : String.Empty,
                                  G_35_1 = block.Element("G_35_1") != null ? block.Element("G_35_1").Value : String.Empty,
                                  G_35_SHTUKA = block.Element("G_35_SHTUKA") != null ? block.Element("G_35_SHTUKA").Value : String.Empty,
                                  G_38_1 = block.Element("G_38_1") != null ? block.Element("G_38_1").Value : String.Empty,
                                  G_38_SHTUKA = block.Element("G_38_SHTUKA") != null ? block.Element("G_38_SHTUKA").Value : String.Empty,
                                  G_38_2 = block.Element("G_38_2") != null ? block.Element("G_38_2").Value : String.Empty,
                                  G_38_PURE = block.Element("G_38_PURE") != null ? block.Element("G_38_PURE").Value : String.Empty,
                                  G_42_ZAEDINIC = block.Element("G_42_ZAEDINIC") != null ? block.Element("G_42_ZAEDINIC").Value : String.Empty,
                                  G_42_ZASUM = block.Element("G_42_ZASUM") != null ? block.Element("G_42_ZASUM").Value : String.Empty,
                                  G_42_REDUCE = block.Element("G_42_REDUCE") != null ? block.Element("G_42_REDUCE").Value : String.Empty,
                                  G_42_VAL = block.Element("G_42_VAL") != null ? block.Element("G_42_VAL").Value : String.Empty,
                                  PRICES = block.Element("PRICES") != null ? block.Element("PRICES").Value : String.Empty,
                                  PRICETAM = block.Element("PRICETAM") != null ? block.Element("PRICETAM").Value : String.Empty,
                                  G_41_1 = block.Element("G_41_1") != null ? block.Element("G_41_1").Value : String.Empty,
                                  G_41_TEXT = block.Element("G_41_TEXT") != null ? block.Element("G_41_TEXT").Value : String.Empty,
                                  G_41_QUNT = block.Element("G_41_QUNT") != null ? block.Element("G_41_QUNT").Value : String.Empty,
                                  G_31_EDCODE = block.Element("G_31_EDCODE") != null ? block.Element("G_31_EDCODE").Value : String.Empty,
                                  G_31_TEXT = block.Element("G_31_TEXT") != null ? block.Element("G_31_TEXT").Value : String.Empty,
                                  G_31_QUNT = block.Element("G_31_QUNT") != null ? block.Element("G_31_QUNT").Value : String.Empty,
                                  CONT = block.Element("CONT") != null ? block.Element("CONT").Value : String.Empty,
                                  CONTALFA = block.Element("CONTALFA") != null ? block.Element("CONTALFA").Value : String.Empty,
                                  G_31_MEST = block.Element("G_31_MEST") != null ? block.Element("G_31_MEST").Value : String.Empty,
                                  G_31_TARA = block.Element("G_31_TARA") != null ? block.Element("G_31_TARA").Value : String.Empty,
                                  G_31_TARASUM = block.Element("G_31_TARASUM") != null ? block.Element("G_31_TARASUM").Value : String.Empty,
                                  G_31_TARALFA = block.Element("G_31_TARALFA") != null ? block.Element("G_31_TARALFA").Value : String.Empty,
                                  G_31_PKGNUM = block.Element("G_31_PKGNUM") != null ? block.Element("G_31_PKGNUM").Value : String.Empty,
                                  G_31_TARAPAL = block.Element("G_31_TARAPAL") != null ? block.Element("G_31_TARAPAL").Value : String.Empty,
                                  PEXTALFA = block.Element("PEXTALFA") != null ? block.Element("PEXTALFA").Value : String.Empty,
                                  PEXTNAME = block.Element("PEXTNAME") != null ? block.Element("PEXTNAME").Value : String.Empty,
                                  PEXTCNT = block.Element("PEXTCNT") != null ? block.Element("PEXTCNT").Value : String.Empty,
                                  PINDALFA = block.Element("PINDALFA") != null ? block.Element("PINDALFA").Value : String.Empty,
                                  PINDNAME = block.Element("PINDNAME") != null ? block.Element("PINDNAME").Value : String.Empty,
                                  PINDVAL = block.Element("PINDVAL") != null ? block.Element("PINDVAL").Value : String.Empty,
                                  PINDCNT = block.Element("PINDCNT") != null ? block.Element("PINDCNT").Value : String.Empty,
                                  PINDNO = block.Element("PINDNO") != null ? block.Element("PINDNO").Value : String.Empty,
                                  G_31_20_20 = block.Element("G_31_20_20") != null ? block.Element("G_31_20_20").Value : String.Empty,
                                  G_31_20_21 = block.Element("G_31_20_21") != null ? block.Element("G_31_20_21").Value : String.Empty,
                                  G_31_USER1 = block.Element("G_31_USER1") != null ? block.Element("G_31_USER1").Value : String.Empty,
                                  G_31_USER2 = block.Element("G_31_USER2") != null ? block.Element("G_31_USER2").Value : String.Empty,
                                  G_31_USER3 = block.Element("G_31_USER3") != null ? block.Element("G_31_USER3").Value : String.Empty,
                                  G_31_USER4 = block.Element("G_31_USER4") != null ? block.Element("G_31_USER4").Value : String.Empty,
                                  G_31_USER5 = block.Element("G_31_USER5") != null ? block.Element("G_31_USER5").Value : String.Empty,
                                  G_31_USER6 = block.Element("G_31_USER6") != null ? block.Element("G_31_USER6").Value : String.Empty,
                                  G_31_USER7 = block.Element("G_31_USER7") != null ? block.Element("G_31_USER7").Value : String.Empty,
                                  G_31_USER8 = block.Element("G_31_USER8") != null ? block.Element("G_31_USER8").Value : String.Empty,
                                  G_31_USER9 = block.Element("G_31_USER9") != null ? block.Element("G_31_USER9").Value : String.Empty,
                                  G_31_USER10 = block.Element("G_31_USER10") != null ? block.Element("G_31_USER10").Value : String.Empty,
                                  G441_1 = block.Element("G441_1") != null ? block.Element("G441_1").Value : String.Empty,
                                  G442_1 = block.Element("G442_1") != null ? block.Element("G442_1").Value : String.Empty,
                                  G443_1 = block.Element("G443_1") != null ? block.Element("G443_1").Value : String.Empty,
                                  G446_1 = block.Element("G446_1") != null ? block.Element("G446_1").Value : String.Empty,
                                  G447_1 = block.Element("G447_1") != null ? block.Element("G447_1").Value : String.Empty,
                                  G4401_1 = block.Element("G4401_1") != null ? block.Element("G4401_1").Value : String.Empty,
                                  G4402_1 = block.Element("G4402_1") != null ? block.Element("G4402_1").Value : String.Empty,
                                  G441_2 = block.Element("G441_2") != null ? block.Element("G441_2").Value : String.Empty,
                                  G442_2 = block.Element("G442_2") != null ? block.Element("G442_2").Value : String.Empty,
                                  G443_2 = block.Element("G443_2") != null ? block.Element("G443_2").Value : String.Empty,
                                  G446_2 = block.Element("G446_2") != null ? block.Element("G446_2").Value : String.Empty,
                                  G447_2 = block.Element("G447_2") != null ? block.Element("G447_2").Value : String.Empty,
                                  G4401_2 = block.Element("G4401_2") != null ? block.Element("G4401_2").Value : String.Empty,
                                  G4402_2 = block.Element("G4402_2") != null ? block.Element("G4402_2").Value : String.Empty,
                                  G441_3 = block.Element("G441_3") != null ? block.Element("G441_3").Value : String.Empty,
                                  G442_3 = block.Element("G442_3") != null ? block.Element("G442_3").Value : String.Empty,
                                  G443_3 = block.Element("G443_3") != null ? block.Element("G443_3").Value : String.Empty,
                                  G446_3 = block.Element("G446_3") != null ? block.Element("G446_3").Value : String.Empty,
                                  G447_3 = block.Element("G447_3") != null ? block.Element("G447_3").Value : String.Empty,
                                  G4401_3 = block.Element("G4401_3") != null ? block.Element("G4401_3").Value : String.Empty,
                                  G4402_3 = block.Element("G4402_3") != null ? block.Element("G4402_3").Value : String.Empty,
                                  G441_4 = block.Element("G441_4") != null ? block.Element("G441_4").Value : String.Empty,
                                  G442_4 = block.Element("G442_4") != null ? block.Element("G442_4").Value : String.Empty,
                                  G443_4 = block.Element("G443_4") != null ? block.Element("G443_4").Value : String.Empty,
                                  G446_4 = block.Element("G446_4") != null ? block.Element("G446_4").Value : String.Empty,
                                  G447_4 = block.Element("G447_4") != null ? block.Element("G447_4").Value : String.Empty,
                                  G4401_4 = block.Element("G4401_4") != null ? block.Element("G4401_4").Value : String.Empty,
                                  G4402_4 = block.Element("G4402_4") != null ? block.Element("G4402_4").Value : String.Empty,
                                  G441_5 = block.Element("G441_5") != null ? block.Element("G441_5").Value : String.Empty,
                                  G442_5 = block.Element("G442_5") != null ? block.Element("G442_5").Value : String.Empty,
                                  G443_5 = block.Element("G443_5") != null ? block.Element("G443_5").Value : String.Empty,
                                  G446_5 = block.Element("G446_5") != null ? block.Element("G446_5").Value : String.Empty,
                                  G447_5 = block.Element("G447_5") != null ? block.Element("G447_5").Value : String.Empty,
                                  G4401_5 = block.Element("G4401_5") != null ? block.Element("G4401_5").Value : String.Empty,
                                  G4402_5 = block.Element("G4402_5") != null ? block.Element("G4402_5").Value : String.Empty,
                                  G441_6 = block.Element("G441_6") != null ? block.Element("G441_6").Value : String.Empty,
                                  G442_6 = block.Element("G442_6") != null ? block.Element("G442_6").Value : String.Empty,
                                  G443_6 = block.Element("G443_6") != null ? block.Element("G443_6").Value : String.Empty,
                                  G446_6 = block.Element("G446_6") != null ? block.Element("G446_6").Value : String.Empty,
                                  G447_6 = block.Element("G447_6") != null ? block.Element("G447_6").Value : String.Empty,
                                  G4401_6 = block.Element("G4401_6") != null ? block.Element("G4401_6").Value : String.Empty,
                                  G4402_6 = block.Element("G4402_6") != null ? block.Element("G4402_6").Value : String.Empty,
                                  G441_7 = block.Element("G441_7") != null ? block.Element("G441_7").Value : String.Empty,
                                  G442_7 = block.Element("G442_7") != null ? block.Element("G442_7").Value : String.Empty,
                                  G443_7 = block.Element("G443_7") != null ? block.Element("G443_7").Value : String.Empty,
                                  G446_7 = block.Element("G446_7") != null ? block.Element("G446_7").Value : String.Empty,
                                  G447_7 = block.Element("G447_7") != null ? block.Element("G447_7").Value : String.Empty,
                                  G4401_7 = block.Element("G4401_7") != null ? block.Element("G4401_7").Value : String.Empty,
                                  G4402_7 = block.Element("G4402_7") != null ? block.Element("G4402_7").Value : String.Empty,
                                  G441_8 = block.Element("G441_8") != null ? block.Element("G441_8").Value : String.Empty,
                                  G442_8 = block.Element("G442_8") != null ? block.Element("G442_8").Value : String.Empty,
                                  G443_8 = block.Element("G443_8") != null ? block.Element("G443_8").Value : String.Empty,
                                  G446_8 = block.Element("G446_8") != null ? block.Element("G446_8").Value : String.Empty,
                                  G447_8 = block.Element("G447_8") != null ? block.Element("G447_8").Value : String.Empty,
                                  G4401_8 = block.Element("G4401_8") != null ? block.Element("G4401_8").Value : String.Empty,
                                  G4402_8 = block.Element("G4402_8") != null ? block.Element("G4402_8").Value : String.Empty,
                                  G441_9 = block.Element("G441_9") != null ? block.Element("G441_9").Value : String.Empty,
                                  G442_9 = block.Element("G442_9") != null ? block.Element("G442_9").Value : String.Empty,
                                  G443_9 = block.Element("G443_9") != null ? block.Element("G443_9").Value : String.Empty,
                                  G446_9 = block.Element("G446_9") != null ? block.Element("G446_9").Value : String.Empty,
                                  G447_9 = block.Element("G447_9") != null ? block.Element("G447_9").Value : String.Empty,
                                  G4401_9 = block.Element("G4401_9") != null ? block.Element("G4401_9").Value : String.Empty,
                                  G4402_9 = block.Element("G4402_9") != null ? block.Element("G4402_9").Value : String.Empty,
                                  G441_10 = block.Element("G441_10") != null ? block.Element("G441_10").Value : String.Empty,
                                  G442_10 = block.Element("G442_10") != null ? block.Element("G442_10").Value : String.Empty,
                                  G443_10 = block.Element("G443_10") != null ? block.Element("G443_10").Value : String.Empty,
                                  G446_10 = block.Element("G446_10") != null ? block.Element("G446_10").Value : String.Empty,
                                  G447_10 = block.Element("G447_10") != null ? block.Element("G447_10").Value : String.Empty,
                                  G4401_10 = block.Element("G4401_10") != null ? block.Element("G4401_10").Value : String.Empty,
                                  G4402_10 = block.Element("G4402_10") != null ? block.Element("G4402_10").Value : String.Empty,
                                  G441_11 = block.Element("G441_11") != null ? block.Element("G441_11").Value : String.Empty,
                                  G442_11 = block.Element("G442_11") != null ? block.Element("G442_11").Value : String.Empty,
                                  G443_11 = block.Element("G443_11") != null ? block.Element("G443_11").Value : String.Empty,
                                  G446_11 = block.Element("G446_11") != null ? block.Element("G446_11").Value : String.Empty,
                                  G447_11 = block.Element("G447_11") != null ? block.Element("G447_11").Value : String.Empty,
                                  G4401_11 = block.Element("G4401_11") != null ? block.Element("G4401_11").Value : String.Empty,
                                  G4402_11 = block.Element("G4402_11") != null ? block.Element("G4402_11").Value : String.Empty,
                                  G441_12 = block.Element("G441_12") != null ? block.Element("G441_12").Value : String.Empty,
                                  G442_12 = block.Element("G442_12") != null ? block.Element("G442_12").Value : String.Empty,
                                  G443_12 = block.Element("G443_12") != null ? block.Element("G443_12").Value : String.Empty,
                                  G446_12 = block.Element("G446_12") != null ? block.Element("G446_12").Value : String.Empty,
                                  G447_12 = block.Element("G447_12") != null ? block.Element("G447_12").Value : String.Empty,
                                  G4401_12 = block.Element("G4401_12") != null ? block.Element("G4401_12").Value : String.Empty,
                                  G4402_12 = block.Element("G4402_12") != null ? block.Element("G4402_12").Value : String.Empty,
                                  G441_13 = block.Element("G441_13") != null ? block.Element("G441_13").Value : String.Empty,
                                  G442_13 = block.Element("G442_13") != null ? block.Element("G442_13").Value : String.Empty,
                                  G443_13 = block.Element("G443_13") != null ? block.Element("G443_13").Value : String.Empty,
                                  G446_13 = block.Element("G446_13") != null ? block.Element("G446_13").Value : String.Empty,
                                  G447_13 = block.Element("G447_13") != null ? block.Element("G447_13").Value : String.Empty,
                                  G4401_13 = block.Element("G4401_13") != null ? block.Element("G4401_13").Value : String.Empty,
                                  G4402_13 = block.Element("G4402_13") != null ? block.Element("G4402_13").Value : String.Empty,
                                  G441_14 = block.Element("G441_14") != null ? block.Element("G441_14").Value : String.Empty,
                                  G442_14 = block.Element("G442_14") != null ? block.Element("G442_14").Value : String.Empty,
                                  G443_14 = block.Element("G443_14") != null ? block.Element("G443_14").Value : String.Empty,
                                  G446_14 = block.Element("G446_14") != null ? block.Element("G446_14").Value : String.Empty,
                                  G447_14 = block.Element("G447_14") != null ? block.Element("G447_14").Value : String.Empty,
                                  G4401_14 = block.Element("G4401_14") != null ? block.Element("G4401_14").Value : String.Empty,
                                  G4402_14 = block.Element("G4402_14") != null ? block.Element("G4402_14").Value : String.Empty,
                                  G441_15 = block.Element("G441_15") != null ? block.Element("G441_15").Value : String.Empty,
                                  G442_15 = block.Element("G442_15") != null ? block.Element("G442_15").Value : String.Empty,
                                  G443_15 = block.Element("G443_15") != null ? block.Element("G443_15").Value : String.Empty,
                                  G446_15 = block.Element("G446_15") != null ? block.Element("G446_15").Value : String.Empty,
                                  G447_15 = block.Element("G447_15") != null ? block.Element("G447_15").Value : String.Empty,
                                  G4401_15 = block.Element("G4401_15") != null ? block.Element("G4401_15").Value : String.Empty,
                                  G4402_15 = block.Element("G4402_15") != null ? block.Element("G4402_15").Value : String.Empty,
                                  D_DATE = block.Element("D_DATE") != null ? block.Element("D_DATE").Value : String.Empty,
                                  D_TIME = block.Element("D_TIME") != null ? block.Element("D_TIME").Value : String.Empty,
                                  G_47_POSHL = block.Element("G_47_POSHL") != null ? block.Element("G_47_POSHL").Value : String.Empty,
                                  G_47_POSHLST = block.Element("G_47_POSHLST") != null ? block.Element("G_47_POSHLST").Value : String.Empty,
                                  G_47_POSHLK = block.Element("G_47_POSHLK") != null ? block.Element("G_47_POSHLK").Value : String.Empty,
                                  G_47_POSHLS = block.Element("G_47_POSHLS") != null ? block.Element("G_47_POSHLS").Value : String.Empty,
                                  G_47_POSP = block.Element("G_47_POSP") != null ? block.Element("G_47_POSP").Value : String.Empty,
                                  G_47_POSPST = block.Element("G_47_POSPST") != null ? block.Element("G_47_POSPST").Value : String.Empty,
                                  G_47_POSPK = block.Element("G_47_POSPK") != null ? block.Element("G_47_POSPK").Value : String.Empty,
                                  G_47_POSPS = block.Element("G_47_POSPS") != null ? block.Element("G_47_POSPS").Value : String.Empty,
                                  G_47_POAD = block.Element("G_47_POAD") != null ? block.Element("G_47_POAD").Value : String.Empty,
                                  G_47_POADST = block.Element("G_47_POADST") != null ? block.Element("G_47_POADST").Value : String.Empty,
                                  G_47_POADK = block.Element("G_47_POADK") != null ? block.Element("G_47_POADK").Value : String.Empty,
                                  G_47_POADS = block.Element("G_47_POADS") != null ? block.Element("G_47_POADS").Value : String.Empty,
                                  G_47_AKCIZ = block.Element("G_47_AKCIZ") != null ? block.Element("G_47_AKCIZ").Value : String.Empty,
                                  G_47_AKCIZST = block.Element("G_47_AKCIZST") != null ? block.Element("G_47_AKCIZST").Value : String.Empty,
                                  G_47_AKCIZK = block.Element("G_47_AKCIZK") != null ? block.Element("G_47_AKCIZK").Value : String.Empty,
                                  G_47_AKCIZS = block.Element("G_47_AKCIZS") != null ? block.Element("G_47_AKCIZS").Value : String.Empty,
                                  G_47_NDS = block.Element("G_47_NDS") != null ? block.Element("G_47_NDS").Value : String.Empty,
                                  G_47_NDSST = block.Element("G_47_NDSST") != null ? block.Element("G_47_NDSST").Value : String.Empty,
                                  G_47_NDSK = block.Element("G_47_NDSK") != null ? block.Element("G_47_NDSK").Value : String.Empty,
                                  G_47_NDSS = block.Element("G_47_NDSS") != null ? block.Element("G_47_NDSS").Value : String.Empty,
                                  G_47_TAM = block.Element("G_47_TAM") != null ? block.Element("G_47_TAM").Value : String.Empty,
                                  G_47_TAMST = block.Element("G_47_TAMST") != null ? block.Element("G_47_TAMST").Value : String.Empty,
                                  G_47_TAMK = block.Element("G_47_TAMK") != null ? block.Element("G_47_TAMK").Value : String.Empty,
                                  G_47_TAMS = block.Element("G_47_TAMS") != null ? block.Element("G_47_TAMS").Value : String.Empty,
                                  G_47_OTHER = block.Element("G_47_OTHER") != null ? block.Element("G_47_OTHER").Value : String.Empty,
                                  G_47_OTHERST = block.Element("G_47_OTHERST") != null ? block.Element("G_47_OTHERST").Value : String.Empty,
                                  G_47_OTHERK = block.Element("G_47_OTHERK") != null ? block.Element("G_47_OTHERK").Value : String.Empty,
                                  GTD_REGDT = block.Element("GTD_REGDT") != null ? block.Element("GTD_REGDT").Value : String.Empty,
                                  GTD_KURS = block.Element("GTD_KURS") != null ? block.Element("GTD_KURS").Value : String.Empty,
                                  GTD_22_1 = block.Element("GTD_22_1") != null ? block.Element("GTD_22_1").Value : String.Empty,
                                  GTD_22_3 = block.Element("GTD_22_3") != null ? block.Element("GTD_22_3").Value : String.Empty,
                                  G_42_DOPS = block.Element("G_42_DOPS") != null ? block.Element("G_42_DOPS").Value : String.Empty,
                                  G_45_DOPS = block.Element("G_45_DOPS") != null ? block.Element("G_45_DOPS").Value : String.Empty,
                                  G_45_1 = block.Element("G_45_1") != null ? block.Element("G_45_1").Value : String.Empty,
                                  KTS45_1 = block.Element("KTS45_1") != null ? block.Element("KTS45_1").Value : String.Empty,
                                  G_32_REALNUM = block.Element("G_32_REALNUM") != null ? block.Element("G_32_REALNUM").Value : String.Empty,
                                  G_32_TOVNUM = block.Element("G_32_TOVNUM") != null ? block.Element("G_32_TOVNUM").Value : String.Empty,
                                  GTD_5 = block.Element("GTD_5") != null ? block.Element("GTD_5").Value : String.Empty,
                                  G_33_REAL = block.Element("G_33_REAL") != null ? block.Element("G_33_REAL").Value : String.Empty,
                                  D_G_CODE = block.Element("D_G_CODE") != null ? block.Element("D_G_CODE").Value : String.Empty,
                                  D_G_DATE = block.Element("D_G_DATE") != null ? block.Element("D_G_DATE").Value : String.Empty,
                                  D_G_TIME = block.Element("D_G_TIME") != null ? block.Element("D_G_TIME").Value : String.Empty,
                                  D_G_LNP = block.Element("D_G_LNP") != null ? block.Element("D_G_LNP").Value : String.Empty,
                                  D_G_FOUNDATION = block.Element("D_G_FOUNDATION") != null ? block.Element("D_G_FOUNDATION").Value : String.Empty,
                                  D_G_RESOLUTIONDESCR = block.Element("D_G_RESOLUTIONDESCR") != null ? block.Element("D_G_RESOLUTIONDESCR").Value : String.Empty,
                                  D_G_PERSON = block.Element("D_G_PERSON") != null ? block.Element("D_G_PERSON").Value : String.Empty

                              };

                    DataTable toSql = new DataTable();
                    DataRow row;
                    DataColumn[] dataColumns = new DataColumn[]
                    {
                    new DataColumn("G_32_1", typeof(string)),
                    new DataColumn("G_33_1", typeof(string)),
                    new DataColumn("G_33_2", typeof(string)),
                    new DataColumn("G_33_4", typeof(string)),
                    new DataColumn("G_33_5", typeof(string)),
                    new DataColumn("G_34_1", typeof(string)),
                    new DataColumn("G_34_ALFA", typeof(string)),
                    new DataColumn("G_34_TEXT", typeof(string)),
                    new DataColumn("G_31_ARTICUL", typeof(string)),
                    new DataColumn("G_31_DESCRIPT", typeof(string)),
                    new DataColumn("G_31_DESCRENG", typeof(string)),
                    new DataColumn("G_31_COMENT", typeof(string)),
                    new DataColumn("G_31_SIZE", typeof(string)),
                    new DataColumn("G_31_COLOR", typeof(string)),
                    new DataColumn("G_31_SN", typeof(string)),
                    new DataColumn("G_31_BRAND", typeof(string)),
                    new DataColumn("G_31_MODEL", typeof(string)),
                    new DataColumn("G_31_GOST", typeof(string)),
                    new DataColumn("G_31_OKP", typeof(string)),
                    new DataColumn("G_31_CAS", typeof(string)),
                    new DataColumn("G_31_SORT", typeof(string)),
                    new DataColumn("G_31_PORODA", typeof(string)),
                    new DataColumn("G_31_LES", typeof(string)),
                    new DataColumn("G_31_HEIGHT", typeof(string)),
                    new DataColumn("G_31_WIDTH", typeof(string)),
                    new DataColumn("G_31_LENGTH", typeof(string)),
                    new DataColumn("G_31_DIAMETR", typeof(string)),
                    new DataColumn("G_31_FIRMA", typeof(string)),
                    new DataColumn("G_31_TM", typeof(string)),
                    new DataColumn("G_35_1", typeof(string)),
                    new DataColumn("G_35_SHTUKA", typeof(string)),
                    new DataColumn("G_38_1", typeof(string)),
                    new DataColumn("G_38_SHTUKA", typeof(string)),
                    new DataColumn("G_38_2", typeof(string)),
                    new DataColumn("G_38_PURE", typeof(string)),
                    new DataColumn("G_42_ZAEDINIC", typeof(string)),
                    new DataColumn("G_42_ZASUM", typeof(string)),
                    new DataColumn("G_42_REDUCE", typeof(string)),
                    new DataColumn("G_42_VAL", typeof(string)),
                    new DataColumn("PRICES", typeof(string)),
                    new DataColumn("PRICETAM", typeof(string)),
                    new DataColumn("G_41_1", typeof(string)),
                    new DataColumn("G_41_TEXT", typeof(string)),
                    new DataColumn("G_41_QUNT", typeof(string)),
                    new DataColumn("G_31_EDCODE", typeof(string)),
                    new DataColumn("G_31_TEXT", typeof(string)),
                    new DataColumn("G_31_QUNT", typeof(string)),
                    new DataColumn("CONT", typeof(string)),
                    new DataColumn("CONTALFA", typeof(string)),
                    new DataColumn("G_31_MEST", typeof(string)),
                    new DataColumn("G_31_TARA", typeof(string)),
                    new DataColumn("G_31_TARASUM", typeof(string)),
                    new DataColumn("G_31_TARALFA", typeof(string)),
                    new DataColumn("G_31_PKGNUM", typeof(string)),
                    new DataColumn("G_31_TARAPAL", typeof(string)),
                    new DataColumn("PEXTALFA", typeof(string)),
                    new DataColumn("PEXTNAME", typeof(string)),
                    new DataColumn("PEXTCNT", typeof(string)),
                    new DataColumn("PINDALFA", typeof(string)),
                    new DataColumn("PINDNAME", typeof(string)),
                    new DataColumn("PINDVAL", typeof(string)),
                    new DataColumn("PINDCNT", typeof(string)),
                    new DataColumn("PINDNO", typeof(string)),
                    new DataColumn("G_31_20_20", typeof(string)),
                    new DataColumn("G_31_20_21", typeof(string)),
                    new DataColumn("G_31_USER1", typeof(string)),
                    new DataColumn("G_31_USER2", typeof(string)),
                    new DataColumn("G_31_USER3", typeof(string)),
                    new DataColumn("G_31_USER4", typeof(string)),
                    new DataColumn("G_31_USER5", typeof(string)),
                    new DataColumn("G_31_USER6", typeof(string)),
                    new DataColumn("G_31_USER7", typeof(string)),
                    new DataColumn("G_31_USER8", typeof(string)),
                    new DataColumn("G_31_USER9", typeof(string)),
                    new DataColumn("G_31_USER10", typeof(string)),
                    new DataColumn("G_31_MEMO1", typeof(string)),
                    new DataColumn("G_31_MEMO2", typeof(string)),
                    new DataColumn("G_31_MEMO3", typeof(string)),
                    new DataColumn("G_31_MEMO4", typeof(string)),
                    new DataColumn("G_31_MEMO5", typeof(string)),
                    new DataColumn("G_31_MEMO6", typeof(string)),
                    new DataColumn("G441_1", typeof(string)),
                    new DataColumn("G442_1", typeof(string)),
                    new DataColumn("G443_1", typeof(string)),
                    new DataColumn("G446_1", typeof(string)),
                    new DataColumn("G447_1", typeof(string)),
                    new DataColumn("G4401_1", typeof(string)),
                    new DataColumn("G4402_1", typeof(string)),
                    new DataColumn("G441_2", typeof(string)),
                    new DataColumn("G442_2", typeof(string)),
                    new DataColumn("G443_2", typeof(string)),
                    new DataColumn("G446_2", typeof(string)),
                    new DataColumn("G447_2", typeof(string)),
                    new DataColumn("G4401_2", typeof(string)),
                    new DataColumn("G4402_2", typeof(string)),
                    new DataColumn("G441_3", typeof(string)),
                    new DataColumn("G442_3", typeof(string)),
                    new DataColumn("G443_3", typeof(string)),
                    new DataColumn("G446_3", typeof(string)),
                    new DataColumn("G447_3", typeof(string)),
                    new DataColumn("G4401_3", typeof(string)),
                    new DataColumn("G4402_3", typeof(string)),
                    new DataColumn("G441_4", typeof(string)),
                    new DataColumn("G442_4", typeof(string)),
                    new DataColumn("G443_4", typeof(string)),
                    new DataColumn("G446_4", typeof(string)),
                    new DataColumn("G447_4", typeof(string)),
                    new DataColumn("G4401_4", typeof(string)),
                    new DataColumn("G4402_4", typeof(string)),
                    new DataColumn("G441_5", typeof(string)),
                    new DataColumn("G442_5", typeof(string)),
                    new DataColumn("G443_5", typeof(string)),
                    new DataColumn("G446_5", typeof(string)),
                    new DataColumn("G447_5", typeof(string)),
                    new DataColumn("G4401_5", typeof(string)),
                    new DataColumn("G4402_5", typeof(string)),
                    new DataColumn("G441_6", typeof(string)),
                    new DataColumn("G442_6", typeof(string)),
                    new DataColumn("G443_6", typeof(string)),
                    new DataColumn("G446_6", typeof(string)),
                    new DataColumn("G447_6", typeof(string)),
                    new DataColumn("G4401_6", typeof(string)),
                    new DataColumn("G4402_6", typeof(string)),
                    new DataColumn("G441_7", typeof(string)),
                    new DataColumn("G442_7", typeof(string)),
                    new DataColumn("G443_7", typeof(string)),
                    new DataColumn("G446_7", typeof(string)),
                    new DataColumn("G447_7", typeof(string)),
                    new DataColumn("G4401_7", typeof(string)),
                    new DataColumn("G4402_7", typeof(string)),
                    new DataColumn("G441_8", typeof(string)),
                    new DataColumn("G442_8", typeof(string)),
                    new DataColumn("G443_8", typeof(string)),
                    new DataColumn("G446_8", typeof(string)),
                    new DataColumn("G447_8", typeof(string)),
                    new DataColumn("G4401_8", typeof(string)),
                    new DataColumn("G4402_8", typeof(string)),
                    new DataColumn("G441_9", typeof(string)),
                    new DataColumn("G442_9", typeof(string)),
                    new DataColumn("G443_9", typeof(string)),
                    new DataColumn("G446_9", typeof(string)),
                    new DataColumn("G447_9", typeof(string)),
                    new DataColumn("G4401_9", typeof(string)),
                    new DataColumn("G4402_9", typeof(string)),
                    new DataColumn("G441_10", typeof(string)),
                    new DataColumn("G442_10", typeof(string)),
                    new DataColumn("G443_10", typeof(string)),
                    new DataColumn("G446_10", typeof(string)),
                    new DataColumn("G447_10", typeof(string)),
                    new DataColumn("G4401_10", typeof(string)),
                    new DataColumn("G4402_10", typeof(string)),
                    new DataColumn("G441_11", typeof(string)),
                    new DataColumn("G442_11", typeof(string)),
                    new DataColumn("G443_11", typeof(string)),
                    new DataColumn("G446_11", typeof(string)),
                    new DataColumn("G447_11", typeof(string)),
                    new DataColumn("G4401_11", typeof(string)),
                    new DataColumn("G4402_11", typeof(string)),
                    new DataColumn("G441_12", typeof(string)),
                    new DataColumn("G442_12", typeof(string)),
                    new DataColumn("G443_12", typeof(string)),
                    new DataColumn("G446_12", typeof(string)),
                    new DataColumn("G447_12", typeof(string)),
                    new DataColumn("G4401_12", typeof(string)),
                    new DataColumn("G4402_12", typeof(string)),
                    new DataColumn("G441_13", typeof(string)),
                    new DataColumn("G442_13", typeof(string)),
                    new DataColumn("G443_13", typeof(string)),
                    new DataColumn("G446_13", typeof(string)),
                    new DataColumn("G447_13", typeof(string)),
                    new DataColumn("G4401_13", typeof(string)),
                    new DataColumn("G4402_13", typeof(string)),
                    new DataColumn("G441_14", typeof(string)),
                    new DataColumn("G442_14", typeof(string)),
                    new DataColumn("G443_14", typeof(string)),
                    new DataColumn("G446_14", typeof(string)),
                    new DataColumn("G447_14", typeof(string)),
                    new DataColumn("G4401_14", typeof(string)),
                    new DataColumn("G4402_14", typeof(string)),
                    new DataColumn("G441_15", typeof(string)),
                    new DataColumn("G442_15", typeof(string)),
                    new DataColumn("G443_15", typeof(string)),
                    new DataColumn("G446_15", typeof(string)),
                    new DataColumn("G447_15", typeof(string)),
                    new DataColumn("G4401_15", typeof(string)),
                    new DataColumn("G4402_15", typeof(string)),
                    new DataColumn("D_DATE", typeof(string)),
                    new DataColumn("D_TIME", typeof(string)),
                    new DataColumn("G_47_POSHL", typeof(string)),
                    new DataColumn("G_47_POSHLST", typeof(string)),
                    new DataColumn("G_47_POSHLK", typeof(string)),
                    new DataColumn("G_47_POSHLS", typeof(string)),
                    new DataColumn("G_47_POSP", typeof(string)),
                    new DataColumn("G_47_POSPST", typeof(string)),
                    new DataColumn("G_47_POSPK", typeof(string)),
                    new DataColumn("G_47_POSPS", typeof(string)),
                    new DataColumn("G_47_POAD", typeof(string)),
                    new DataColumn("G_47_POADST", typeof(string)),
                    new DataColumn("G_47_POADK", typeof(string)),
                    new DataColumn("G_47_POADS", typeof(string)),
                    new DataColumn("G_47_AKCIZ", typeof(string)),
                    new DataColumn("G_47_AKCIZST", typeof(string)),
                    new DataColumn("G_47_AKCIZK", typeof(string)),
                    new DataColumn("G_47_AKCIZS", typeof(string)),
                    new DataColumn("G_47_NDS", typeof(string)),
                    new DataColumn("G_47_NDSST", typeof(string)),
                    new DataColumn("G_47_NDSK", typeof(string)),
                    new DataColumn("G_47_NDSS", typeof(string)),
                    new DataColumn("G_47_TAM", typeof(string)),
                    new DataColumn("G_47_TAMST", typeof(string)),
                    new DataColumn("G_47_TAMK", typeof(string)),
                    new DataColumn("G_47_TAMS", typeof(string)),
                    new DataColumn("G_47_OTHER", typeof(string)),
                    new DataColumn("G_47_OTHERST", typeof(string)),
                    new DataColumn("G_47_OTHERK", typeof(string)),
                    new DataColumn("GTD_REGDT", typeof(string)),
                    new DataColumn("GTD_KURS", typeof(string)),
                    new DataColumn("GTD_22_1", typeof(string)),
                    new DataColumn("GTD_22_3", typeof(string)),
                    new DataColumn("G_42_DOPS", typeof(string)),
                    new DataColumn("G_45_DOPS", typeof(string)),
                    new DataColumn("G_45_1", typeof(string)),
                    new DataColumn("KTS45_1", typeof(string)),
                    new DataColumn("G_32_REALNUM", typeof(string)),
                    new DataColumn("G_32_TOVNUM", typeof(string)),
                    new DataColumn("GTD_5", typeof(string)),
                    new DataColumn("G_33_REAL", typeof(string)),
                    new DataColumn("D_G_CODE", typeof(string)),
                    new DataColumn("D_G_DATE", typeof(string)),
                    new DataColumn("D_G_TIME", typeof(string)),
                    new DataColumn("D_G_LNP", typeof(string)),
                    new DataColumn("D_G_FOUNDATION", typeof(string)),
                    new DataColumn("D_G_RESOLUTIONDESCR", typeof(string)),
                    new DataColumn("D_G_PERSON", typeof(string)),
                    new DataColumn("ID", typeof(string)),
                    new DataColumn("INV_NUM", typeof(string)),
                    new DataColumn("INV_DATE", typeof(string)),
                    new DataColumn("G_14_NAME", typeof(string)),
                    new DataColumn("G_14_KPP", typeof(string)),
                    new DataColumn("G_14_INN", typeof(string))

                };
                    toSql.Columns.AddRange(dataColumns);

                    foreach (var tovar in tov)
                    {
                        row = toSql.NewRow();
                        row["G_32_1"] = tovar.G_32_1;
                        row["G_33_1"] = tovar.G_33_1;
                        row["G_33_2"] = tovar.G_33_2;
                        row["G_33_4"] = tovar.G_33_4;
                        row["G_33_5"] = tovar.G_33_5;
                        row["G_34_1"] = tovar.G_34_1;
                        row["G_34_ALFA"] = tovar.G_34_ALFA;
                        row["G_34_TEXT"] = tovar.G_34_TEXT;
                        row["G_31_ARTICUL"] = tovar.G_31_ARTICUL;
                        row["G_31_DESCRIPT"] = tovar.G_31_DESCRIPT;
                        row["G_31_DESCRENG"] = tovar.G_31_DESCRENG;
                        row["G_31_COMENT"] = tovar.G_31_COMENT;
                        row["G_31_SIZE"] = tovar.G_31_SIZE;
                        row["G_31_COLOR"] = tovar.G_31_COLOR;
                        row["G_31_SN"] = tovar.G_31_SN;
                        row["G_31_BRAND"] = tovar.G_31_BRAND;
                        row["G_31_MODEL"] = tovar.G_31_MODEL;
                        row["G_31_GOST"] = tovar.G_31_GOST;
                        row["G_31_OKP"] = tovar.G_31_OKP;
                        row["G_31_CAS"] = tovar.G_31_CAS;
                        row["G_31_SORT"] = tovar.G_31_SORT;
                        row["G_31_PORODA"] = tovar.G_31_PORODA;
                        row["G_31_LES"] = tovar.G_31_LES;
                        row["G_31_HEIGHT"] = tovar.G_31_HEIGHT;
                        row["G_31_WIDTH"] = tovar.G_31_WIDTH;
                        row["G_31_LENGTH"] = tovar.G_31_LENGTH;
                        row["G_31_DIAMETR"] = tovar.G_31_DIAMETR;
                        row["G_31_FIRMA"] = tovar.G_31_FIRMA;
                        row["G_31_TM"] = tovar.G_31_TM;
                        row["G_35_1"] = tovar.G_35_1;
                        row["G_35_SHTUKA"] = tovar.G_35_SHTUKA;
                        row["G_38_1"] = tovar.G_38_1;
                        row["G_38_SHTUKA"] = tovar.G_38_SHTUKA;
                        row["G_38_2"] = tovar.G_38_2;
                        row["G_38_PURE"] = tovar.G_38_PURE;
                        row["G_42_ZAEDINIC"] = tovar.G_42_ZAEDINIC;
                        row["G_42_ZASUM"] = tovar.G_42_ZASUM;
                        row["G_42_REDUCE"] = tovar.G_42_REDUCE;
                        row["G_42_VAL"] = tovar.G_42_VAL;
                        row["PRICES"] = tovar.PRICES;
                        row["PRICETAM"] = tovar.PRICETAM;
                        row["G_41_1"] = tovar.G_41_1;
                        row["G_41_TEXT"] = tovar.G_41_TEXT;
                        row["G_41_QUNT"] = tovar.G_41_QUNT;
                        row["G_31_EDCODE"] = tovar.G_31_EDCODE;
                        row["G_31_TEXT"] = tovar.G_31_TEXT;
                        row["G_31_QUNT"] = tovar.G_31_QUNT;
                        row["CONT"] = tovar.CONT;
                        row["CONTALFA"] = tovar.CONTALFA;
                        row["G_31_MEST"] = tovar.G_31_MEST;
                        row["G_31_TARA"] = tovar.G_31_TARA;
                        row["G_31_TARASUM"] = tovar.G_31_TARASUM;
                        row["G_31_TARALFA"] = tovar.G_31_TARALFA;
                        row["G_31_PKGNUM"] = tovar.G_31_PKGNUM;
                        row["G_31_TARAPAL"] = tovar.G_31_TARAPAL;
                        row["PEXTALFA"] = tovar.PEXTALFA;
                        row["PEXTNAME"] = tovar.PEXTNAME;
                        row["PEXTCNT"] = tovar.PEXTCNT;
                        row["PINDALFA"] = tovar.PINDALFA;
                        row["PINDNAME"] = tovar.PINDNAME;
                        row["PINDVAL"] = tovar.PINDVAL;
                        row["PINDCNT"] = tovar.PINDCNT;
                        row["PINDNO"] = tovar.PINDNO;
                        row["G_31_20_20"] = tovar.G_31_20_20;
                        row["G_31_20_21"] = tovar.G_31_20_21;
                        row["G_31_USER1"] = tovar.G_31_USER1;
                        row["G_31_USER2"] = tovar.G_31_USER2;
                        row["G_31_USER3"] = tovar.G_31_USER3;
                        row["G_31_USER4"] = tovar.G_31_USER4;
                        row["G_31_USER5"] = tovar.G_31_USER5;
                        row["G_31_USER6"] = tovar.G_31_USER6;
                        row["G_31_USER7"] = tovar.G_31_USER7;
                        row["G_31_USER8"] = tovar.G_31_USER8;
                        row["G_31_USER9"] = tovar.G_31_USER9;
                        row["G_31_USER10"] = tovar.G_31_USER10;
                        row["G441_1"] = tovar.G441_1;
                        row["G442_1"] = tovar.G442_1;
                        row["G443_1"] = CheckValidDate(tovar.G443_1, CheckDateType.Date);
                        row["G446_1"] = tovar.G446_1;
                        row["G447_1"] = CheckValidDate(tovar.G447_1, CheckDateType.Date);
                        row["G4401_1"] = tovar.G4401_1;
                        row["G4402_1"] = tovar.G4402_1;
                        row["G441_2"] = tovar.G441_2;
                        row["G442_2"] = tovar.G442_2;
                        row["G443_2"] = CheckValidDate(tovar.G443_2, CheckDateType.Date);
                        row["G446_2"] = tovar.G446_2;
                        row["G447_2"] = CheckValidDate(tovar.G447_2, CheckDateType.Date);
                        row["G4401_2"] = tovar.G4401_2;
                        row["G4402_2"] = tovar.G4402_2;
                        row["G441_3"] = tovar.G441_3;
                        row["G442_3"] = tovar.G442_3;
                        row["G443_3"] = CheckValidDate(tovar.G443_3, CheckDateType.Date);
                        row["G446_3"] = tovar.G446_3;
                        row["G447_3"] = CheckValidDate(tovar.G447_3, CheckDateType.Date);
                        row["G4401_3"] = tovar.G4401_3;
                        row["G4402_3"] = tovar.G4402_3;
                        row["G441_4"] = tovar.G441_4;
                        row["G442_4"] = tovar.G442_4;
                        row["G443_4"] = CheckValidDate(tovar.G443_4, CheckDateType.Date);
                        row["G446_4"] = tovar.G446_4;
                        row["G447_4"] = CheckValidDate(tovar.G447_4, CheckDateType.Date);
                        row["G4401_4"] = tovar.G4401_4;
                        row["G4402_4"] = tovar.G4402_4;
                        row["G441_5"] = tovar.G441_5;
                        row["G442_5"] = tovar.G442_5;
                        row["G443_5"] = CheckValidDate(tovar.G443_5, CheckDateType.Date);
                        row["G446_5"] = tovar.G446_5;
                        row["G447_5"] = CheckValidDate(tovar.G447_5, CheckDateType.Date);
                        row["G4401_5"] = tovar.G4401_5;
                        row["G4402_5"] = tovar.G4402_5;
                        row["G441_6"] = tovar.G441_6;
                        row["G442_6"] = tovar.G442_6;
                        row["G443_6"] = CheckValidDate(tovar.G443_6, CheckDateType.Date);
                        row["G446_6"] = tovar.G446_6;
                        row["G447_6"] = CheckValidDate(tovar.G447_6, CheckDateType.Date);
                        row["G4401_6"] = tovar.G4401_6;
                        row["G4402_6"] = tovar.G4402_6;
                        row["G441_7"] = tovar.G441_7;
                        row["G442_7"] = tovar.G442_7;
                        row["G443_7"] = CheckValidDate(tovar.G443_7, CheckDateType.Date);
                        row["G446_7"] = tovar.G446_7;
                        row["G447_7"] = CheckValidDate(tovar.G447_7, CheckDateType.Date);
                        row["G4401_7"] = tovar.G4401_7;
                        row["G4402_7"] = tovar.G4402_7;
                        row["G441_8"] = tovar.G441_8;
                        row["G442_8"] = tovar.G442_8;
                        row["G443_8"] = CheckValidDate(tovar.G443_8, CheckDateType.Date);
                        row["G446_8"] = tovar.G446_8;
                        row["G447_8"] = CheckValidDate(tovar.G447_8, CheckDateType.Date);
                        row["G4401_8"] = tovar.G4401_8;
                        row["G4402_8"] = tovar.G4402_8;
                        row["G441_9"] = tovar.G441_9;
                        row["G442_9"] = tovar.G442_9;
                        row["G443_9"] = CheckValidDate(tovar.G443_9, CheckDateType.Date);
                        row["G446_9"] = tovar.G446_9;
                        row["G447_9"] = CheckValidDate(tovar.G447_9, CheckDateType.Date);
                        row["G4401_9"] = tovar.G4401_9;
                        row["G4402_9"] = tovar.G4402_9;
                        row["G441_10"] = tovar.G441_10;
                        row["G442_10"] = tovar.G442_10;
                        row["G443_10"] = CheckValidDate(tovar.G443_10, CheckDateType.Date);
                        row["G446_10"] = tovar.G446_10;
                        row["G447_10"] = CheckValidDate(tovar.G447_10, CheckDateType.Date);
                        row["G4401_10"] = tovar.G4401_10;
                        row["G4402_10"] = tovar.G4402_10;
                        row["G441_11"] = tovar.G441_11;
                        row["G442_11"] = tovar.G442_11;
                        row["G443_11"] = CheckValidDate(tovar.G443_11, CheckDateType.Date);
                        row["G446_11"] = tovar.G446_11;
                        row["G447_11"] = CheckValidDate(tovar.G447_11, CheckDateType.Date);
                        row["G4401_11"] = tovar.G4401_11;
                        row["G4402_11"] = tovar.G4402_11;
                        row["G441_12"] = tovar.G441_12;
                        row["G442_12"] = tovar.G442_12;
                        row["G443_12"] = CheckValidDate(tovar.G443_12, CheckDateType.Date);
                        row["G446_12"] = tovar.G446_12;
                        row["G447_12"] = CheckValidDate(tovar.G447_12, CheckDateType.Date);
                        row["G4401_12"] = tovar.G4401_12;
                        row["G4402_12"] = tovar.G4402_12;
                        row["G441_13"] = tovar.G441_13;
                        row["G442_13"] = tovar.G442_13;
                        row["G443_13"] = CheckValidDate(tovar.G443_13, CheckDateType.Date);
                        row["G446_13"] = tovar.G446_13;
                        row["G447_13"] = CheckValidDate(tovar.G447_13, CheckDateType.Date);
                        row["G4401_13"] = tovar.G4401_13;
                        row["G4402_13"] = tovar.G4402_13;
                        row["G441_14"] = tovar.G441_14;
                        row["G442_14"] = tovar.G442_14;
                        row["G443_14"] = CheckValidDate(tovar.G443_14, CheckDateType.Date);
                        row["G446_14"] = tovar.G446_14;
                        row["G447_14"] = CheckValidDate(tovar.G447_14, CheckDateType.Date);
                        row["G4401_14"] = tovar.G4401_14;
                        row["G4402_14"] = tovar.G4402_14;
                        row["G441_15"] = tovar.G441_15;
                        row["G442_15"] = tovar.G442_15;
                        row["G443_15"] = CheckValidDate(tovar.G443_15, CheckDateType.Date);
                        row["G446_15"] = tovar.G446_15;
                        row["G447_15"] = CheckValidDate(tovar.G447_15, CheckDateType.Date);
                        row["G4401_15"] = tovar.G4401_15;
                        row["G4402_15"] = tovar.G4402_15;
                        row["D_DATE"] = tovar.D_DATE;
                        row["D_TIME"] = tovar.D_TIME;
                        row["G_47_POSHL"] = tovar.G_47_POSHL;
                        row["G_47_POSHLST"] = tovar.G_47_POSHLST;
                        row["G_47_POSHLK"] = tovar.G_47_POSHLK;
                        row["G_47_POSHLS"] = tovar.G_47_POSHLS;
                        row["G_47_POSP"] = tovar.G_47_POSP;
                        row["G_47_POSPST"] = tovar.G_47_POSPST;
                        row["G_47_POSPK"] = tovar.G_47_POSPK;
                        row["G_47_POSPS"] = tovar.G_47_POSPS;
                        row["G_47_POAD"] = tovar.G_47_POAD;
                        row["G_47_POADST"] = tovar.G_47_POADST;
                        row["G_47_POADK"] = tovar.G_47_POADK;
                        row["G_47_POADS"] = tovar.G_47_POADS;
                        row["G_47_AKCIZ"] = tovar.G_47_AKCIZ;
                        row["G_47_AKCIZST"] = tovar.G_47_AKCIZST;
                        row["G_47_AKCIZK"] = tovar.G_47_AKCIZK;
                        row["G_47_AKCIZS"] = tovar.G_47_AKCIZS;
                        row["G_47_NDS"] = tovar.G_47_NDS;
                        row["G_47_NDSST"] = tovar.G_47_NDSST;
                        row["G_47_NDSK"] = tovar.G_47_NDSK;
                        row["G_47_NDSS"] = tovar.G_47_NDSS;
                        row["G_47_TAM"] = tovar.G_47_TAM;
                        row["G_47_TAMST"] = tovar.G_47_TAMST;
                        row["G_47_TAMK"] = tovar.G_47_TAMK;
                        row["G_47_TAMS"] = tovar.G_47_TAMS;
                        row["G_47_OTHER"] = tovar.G_47_OTHER;
                        row["G_47_OTHERST"] = tovar.G_47_OTHERST;
                        row["G_47_OTHERK"] = tovar.G_47_OTHERK;
                        row["GTD_REGDT"] = tovar.GTD_REGDT;
                        row["GTD_KURS"] = tovar.GTD_KURS;
                        row["GTD_22_1"] = tovar.GTD_22_1;
                        row["GTD_22_3"] = tovar.GTD_22_3;
                        row["G_42_DOPS"] = tovar.G_42_DOPS;
                        row["G_45_DOPS"] = tovar.G_45_DOPS;
                        row["G_45_1"] = tovar.G_45_1;
                        row["KTS45_1"] = tovar.KTS45_1;
                        row["G_32_REALNUM"] = tovar.G_32_REALNUM;
                        row["G_32_TOVNUM"] = tovar.G_32_TOVNUM;
                        row["GTD_5"] = tovar.GTD_5;
                        row["G_33_REAL"] = tovar.G_33_REAL;
                        row["D_G_CODE"] = tovar.D_G_CODE;
                        row["D_G_DATE"] = tovar.D_G_DATE;
                        row["D_G_TIME"] = tovar.D_G_TIME;
                        row["D_G_LNP"] = tovar.D_G_LNP;
                        row["D_G_FOUNDATION"] = tovar.D_G_FOUNDATION;
                        row["D_G_RESOLUTIONDESCR"] = tovar.D_G_RESOLUTIONDESCR;
                        row["D_G_PERSON"] = tovar.D_G_PERSON;
                        row["ID"] = ID;
                        row["INV_NUM"] = INV_NUM;
                        row["INV_DATE"] = INV_DATE;
                        row["G_14_NAME"] = G_14_NAME;
                        row["G_14_KPP"] = G_14_KPP;
                        row["G_14_INN"] = G_14_INN;

                        toSql.Rows.Add(row);
                    }

                    using (SqlConnection SqlConnectionObj = new SqlConnection(connBuilderNav.ConnectionString))
                    {
                        SqlConnectionObj.Open();
                        SqlBulkCopy bulkCopy = new SqlBulkCopy(SqlConnectionObj, SqlBulkCopyOptions.Default | SqlBulkCopyOptions.FireTriggers | SqlBulkCopyOptions.UseInternalTransaction, null);
                        bulkCopy.DestinationTableName = @"webproject.dbo.web_invoices";

                        bulkCopy.ColumnMappings.Add("G_32_1", "G_32_1");
                        bulkCopy.ColumnMappings.Add("G_33_1", "G_33_1");
                        bulkCopy.ColumnMappings.Add("G_33_2", "G_33_2");
                        bulkCopy.ColumnMappings.Add("G_33_4", "G_33_4");
                        bulkCopy.ColumnMappings.Add("G_33_5", "G_33_5");
                        bulkCopy.ColumnMappings.Add("G_34_1", "G_34_1");
                        bulkCopy.ColumnMappings.Add("G_34_ALFA", "G_34_ALFA");
                        bulkCopy.ColumnMappings.Add("G_34_TEXT", "G_34_TEXT");
                        bulkCopy.ColumnMappings.Add("G_31_ARTICUL", "G_31_ARTICUL");
                        bulkCopy.ColumnMappings.Add("G_31_DESCRIPT", "G_31_DESCRIPT");
                        bulkCopy.ColumnMappings.Add("G_31_DESCRENG", "G_31_DESCRENG");
                        bulkCopy.ColumnMappings.Add("G_31_COMENT", "G_31_COMENT");
                        bulkCopy.ColumnMappings.Add("G_31_SIZE", "G_31_SIZE");
                        bulkCopy.ColumnMappings.Add("G_31_COLOR", "G_31_COLOR");
                        bulkCopy.ColumnMappings.Add("G_31_SN", "G_31_SN");
                        bulkCopy.ColumnMappings.Add("G_31_BRAND", "G_31_BRAND");
                        bulkCopy.ColumnMappings.Add("G_31_MODEL", "G_31_MODEL");
                        bulkCopy.ColumnMappings.Add("G_31_GOST", "G_31_GOST");
                        bulkCopy.ColumnMappings.Add("G_31_OKP", "G_31_OKP");
                        bulkCopy.ColumnMappings.Add("G_31_CAS", "G_31_CAS");
                        bulkCopy.ColumnMappings.Add("G_31_SORT", "G_31_SORT");
                        bulkCopy.ColumnMappings.Add("G_31_PORODA", "G_31_PORODA");
                        bulkCopy.ColumnMappings.Add("G_31_LES", "G_31_LES");
                        bulkCopy.ColumnMappings.Add("G_31_HEIGHT", "G_31_HEIGHT");
                        bulkCopy.ColumnMappings.Add("G_31_WIDTH", "G_31_WIDTH");
                        bulkCopy.ColumnMappings.Add("G_31_LENGTH", "G_31_LENGTH");
                        bulkCopy.ColumnMappings.Add("G_31_DIAMETR", "G_31_DIAMETR");
                        bulkCopy.ColumnMappings.Add("G_31_FIRMA", "G_31_FIRMA");
                        bulkCopy.ColumnMappings.Add("G_31_TM", "G_31_TM");
                        bulkCopy.ColumnMappings.Add("G_35_1", "G_35_1");
                        bulkCopy.ColumnMappings.Add("G_35_SHTUKA", "G_35_SHTUKA");
                        bulkCopy.ColumnMappings.Add("G_38_1", "G_38_1");
                        bulkCopy.ColumnMappings.Add("G_38_SHTUKA", "G_38_SHTUKA");
                        bulkCopy.ColumnMappings.Add("G_38_2", "G_38_2");
                        bulkCopy.ColumnMappings.Add("G_38_PURE", "G_38_PURE");
                        bulkCopy.ColumnMappings.Add("G_42_ZAEDINIC", "G_42_ZAEDINIC");
                        bulkCopy.ColumnMappings.Add("G_42_ZASUM", "G_42_ZASUM");
                        bulkCopy.ColumnMappings.Add("G_42_REDUCE", "G_42_REDUCE");
                        bulkCopy.ColumnMappings.Add("G_42_VAL", "G_42_VAL");
                        bulkCopy.ColumnMappings.Add("PRICES", "PRICES");
                        bulkCopy.ColumnMappings.Add("PRICETAM", "PRICETAM");
                        bulkCopy.ColumnMappings.Add("G_41_1", "G_41_1");
                        bulkCopy.ColumnMappings.Add("G_41_TEXT", "G_41_TEXT");
                        bulkCopy.ColumnMappings.Add("G_41_QUNT", "G_41_QUNT");
                        bulkCopy.ColumnMappings.Add("G_31_EDCODE", "G_31_EDCODE");
                        bulkCopy.ColumnMappings.Add("G_31_TEXT", "G_31_TEXT");
                        bulkCopy.ColumnMappings.Add("G_31_QUNT", "G_31_QUNT");
                        bulkCopy.ColumnMappings.Add("CONT", "CONT");
                        bulkCopy.ColumnMappings.Add("CONTALFA", "CONTALFA");
                        bulkCopy.ColumnMappings.Add("G_31_MEST", "G_31_MEST");
                        bulkCopy.ColumnMappings.Add("G_31_TARA", "G_31_TARA");
                        bulkCopy.ColumnMappings.Add("G_31_TARASUM", "G_31_TARASUM");
                        bulkCopy.ColumnMappings.Add("G_31_TARALFA", "G_31_TARALFA");
                        bulkCopy.ColumnMappings.Add("G_31_PKGNUM", "G_31_PKGNUM");
                        bulkCopy.ColumnMappings.Add("G_31_TARAPAL", "G_31_TARAPAL");
                        bulkCopy.ColumnMappings.Add("PEXTALFA", "PEXTALFA");
                        bulkCopy.ColumnMappings.Add("PEXTNAME", "PEXTNAME");
                        bulkCopy.ColumnMappings.Add("PEXTCNT", "PEXTCNT");
                        bulkCopy.ColumnMappings.Add("PINDALFA", "PINDALFA");
                        bulkCopy.ColumnMappings.Add("PINDNAME", "PINDNAME");
                        bulkCopy.ColumnMappings.Add("PINDVAL", "PINDVAL");
                        bulkCopy.ColumnMappings.Add("PINDCNT", "PINDCNT");
                        bulkCopy.ColumnMappings.Add("PINDNO", "PINDNO");
                        bulkCopy.ColumnMappings.Add("G_31_20_20", "G_31_20_20");
                        bulkCopy.ColumnMappings.Add("G_31_20_21", "G_31_20_21");
                        bulkCopy.ColumnMappings.Add("G_31_USER1", "G_31_USER1");
                        bulkCopy.ColumnMappings.Add("G_31_USER2", "G_31_USER2");
                        bulkCopy.ColumnMappings.Add("G_31_USER3", "G_31_USER3");
                        bulkCopy.ColumnMappings.Add("G_31_USER4", "G_31_USER4");
                        bulkCopy.ColumnMappings.Add("G_31_USER5", "G_31_USER5");
                        bulkCopy.ColumnMappings.Add("G_31_USER6", "G_31_USER6");
                        bulkCopy.ColumnMappings.Add("G_31_USER7", "G_31_USER7");
                        bulkCopy.ColumnMappings.Add("G_31_USER8", "G_31_USER8");
                        bulkCopy.ColumnMappings.Add("G_31_USER9", "G_31_USER9");
                        bulkCopy.ColumnMappings.Add("G_31_USER10", "G_31_USER10");
                        bulkCopy.ColumnMappings.Add("G441_1", "G441_1");
                        bulkCopy.ColumnMappings.Add("G442_1", "G442_1");
                        bulkCopy.ColumnMappings.Add("G443_1", "G443_1");
                        bulkCopy.ColumnMappings.Add("G446_1", "G446_1");
                        bulkCopy.ColumnMappings.Add("G447_1", "G447_1");
                        bulkCopy.ColumnMappings.Add("G4401_1", "G4401_1");
                        bulkCopy.ColumnMappings.Add("G4402_1", "G4402_1");
                        bulkCopy.ColumnMappings.Add("G441_2", "G441_2");
                        bulkCopy.ColumnMappings.Add("G442_2", "G442_2");
                        bulkCopy.ColumnMappings.Add("G443_2", "G443_2");
                        bulkCopy.ColumnMappings.Add("G446_2", "G446_2");
                        bulkCopy.ColumnMappings.Add("G447_2", "G447_2");
                        bulkCopy.ColumnMappings.Add("G4401_2", "G4401_2");
                        bulkCopy.ColumnMappings.Add("G4402_2", "G4402_2");
                        bulkCopy.ColumnMappings.Add("G441_3", "G441_3");
                        bulkCopy.ColumnMappings.Add("G442_3", "G442_3");
                        bulkCopy.ColumnMappings.Add("G443_3", "G443_3");
                        bulkCopy.ColumnMappings.Add("G446_3", "G446_3");
                        bulkCopy.ColumnMappings.Add("G447_3", "G447_3");
                        bulkCopy.ColumnMappings.Add("G4401_3", "G4401_3");
                        bulkCopy.ColumnMappings.Add("G4402_3", "G4402_3");
                        bulkCopy.ColumnMappings.Add("G441_4", "G441_4");
                        bulkCopy.ColumnMappings.Add("G442_4", "G442_4");
                        bulkCopy.ColumnMappings.Add("G443_4", "G443_4");
                        bulkCopy.ColumnMappings.Add("G446_4", "G446_4");
                        bulkCopy.ColumnMappings.Add("G447_4", "G447_4");
                        bulkCopy.ColumnMappings.Add("G4401_4", "G4401_4");
                        bulkCopy.ColumnMappings.Add("G4402_4", "G4402_4");
                        bulkCopy.ColumnMappings.Add("G441_5", "G441_5");
                        bulkCopy.ColumnMappings.Add("G442_5", "G442_5");
                        bulkCopy.ColumnMappings.Add("G443_5", "G443_5");
                        bulkCopy.ColumnMappings.Add("G446_5", "G446_5");
                        bulkCopy.ColumnMappings.Add("G447_5", "G447_5");
                        bulkCopy.ColumnMappings.Add("G4401_5", "G4401_5");
                        bulkCopy.ColumnMappings.Add("G4402_5", "G4402_5");
                        bulkCopy.ColumnMappings.Add("G441_6", "G441_6");
                        bulkCopy.ColumnMappings.Add("G442_6", "G442_6");
                        bulkCopy.ColumnMappings.Add("G443_6", "G443_6");
                        bulkCopy.ColumnMappings.Add("G446_6", "G446_6");
                        bulkCopy.ColumnMappings.Add("G447_6", "G447_6");
                        bulkCopy.ColumnMappings.Add("G4401_6", "G4401_6");
                        bulkCopy.ColumnMappings.Add("G4402_6", "G4402_6");
                        bulkCopy.ColumnMappings.Add("G441_7", "G441_7");
                        bulkCopy.ColumnMappings.Add("G442_7", "G442_7");
                        bulkCopy.ColumnMappings.Add("G443_7", "G443_7");
                        bulkCopy.ColumnMappings.Add("G446_7", "G446_7");
                        bulkCopy.ColumnMappings.Add("G447_7", "G447_7");
                        bulkCopy.ColumnMappings.Add("G4401_7", "G4401_7");
                        bulkCopy.ColumnMappings.Add("G4402_7", "G4402_7");
                        bulkCopy.ColumnMappings.Add("G441_8", "G441_8");
                        bulkCopy.ColumnMappings.Add("G442_8", "G442_8");
                        bulkCopy.ColumnMappings.Add("G443_8", "G443_8");
                        bulkCopy.ColumnMappings.Add("G446_8", "G446_8");
                        bulkCopy.ColumnMappings.Add("G447_8", "G447_8");
                        bulkCopy.ColumnMappings.Add("G4401_8", "G4401_8");
                        bulkCopy.ColumnMappings.Add("G4402_8", "G4402_8");
                        bulkCopy.ColumnMappings.Add("G441_9", "G441_9");
                        bulkCopy.ColumnMappings.Add("G442_9", "G442_9");
                        bulkCopy.ColumnMappings.Add("G443_9", "G443_9");
                        bulkCopy.ColumnMappings.Add("G446_9", "G446_9");
                        bulkCopy.ColumnMappings.Add("G447_9", "G447_9");
                        bulkCopy.ColumnMappings.Add("G4401_9", "G4401_9");
                        bulkCopy.ColumnMappings.Add("G4402_9", "G4402_9");
                        bulkCopy.ColumnMappings.Add("G441_10", "G441_10");
                        bulkCopy.ColumnMappings.Add("G442_10", "G442_10");
                        bulkCopy.ColumnMappings.Add("G443_10", "G443_10");
                        bulkCopy.ColumnMappings.Add("G446_10", "G446_10");
                        bulkCopy.ColumnMappings.Add("G447_10", "G447_10");
                        bulkCopy.ColumnMappings.Add("G4401_10", "G4401_10");
                        bulkCopy.ColumnMappings.Add("G4402_10", "G4402_10");
                        bulkCopy.ColumnMappings.Add("G441_11", "G441_11");
                        bulkCopy.ColumnMappings.Add("G442_11", "G442_11");
                        bulkCopy.ColumnMappings.Add("G443_11", "G443_11");
                        bulkCopy.ColumnMappings.Add("G446_11", "G446_11");
                        bulkCopy.ColumnMappings.Add("G447_11", "G447_11");
                        bulkCopy.ColumnMappings.Add("G4401_11", "G4401_11");
                        bulkCopy.ColumnMappings.Add("G4402_11", "G4402_11");
                        bulkCopy.ColumnMappings.Add("G441_12", "G441_12");
                        bulkCopy.ColumnMappings.Add("G442_12", "G442_12");
                        bulkCopy.ColumnMappings.Add("G443_12", "G443_12");
                        bulkCopy.ColumnMappings.Add("G446_12", "G446_12");
                        bulkCopy.ColumnMappings.Add("G447_12", "G447_12");
                        bulkCopy.ColumnMappings.Add("G4401_12", "G4401_12");
                        bulkCopy.ColumnMappings.Add("G4402_12", "G4402_12");
                        bulkCopy.ColumnMappings.Add("G441_13", "G441_13");
                        bulkCopy.ColumnMappings.Add("G442_13", "G442_13");
                        bulkCopy.ColumnMappings.Add("G443_13", "G443_13");
                        bulkCopy.ColumnMappings.Add("G446_13", "G446_13");
                        bulkCopy.ColumnMappings.Add("G447_13", "G447_13");
                        bulkCopy.ColumnMappings.Add("G4401_13", "G4401_13");
                        bulkCopy.ColumnMappings.Add("G4402_13", "G4402_13");
                        bulkCopy.ColumnMappings.Add("G441_14", "G441_14");
                        bulkCopy.ColumnMappings.Add("G442_14", "G442_14");
                        bulkCopy.ColumnMappings.Add("G443_14", "G443_14");
                        bulkCopy.ColumnMappings.Add("G446_14", "G446_14");
                        bulkCopy.ColumnMappings.Add("G447_14", "G447_14");
                        bulkCopy.ColumnMappings.Add("G4401_14", "G4401_14");
                        bulkCopy.ColumnMappings.Add("G4402_14", "G4402_14");
                        bulkCopy.ColumnMappings.Add("G441_15", "G441_15");
                        bulkCopy.ColumnMappings.Add("G442_15", "G442_15");
                        bulkCopy.ColumnMappings.Add("G443_15", "G443_15");
                        bulkCopy.ColumnMappings.Add("G446_15", "G446_15");
                        bulkCopy.ColumnMappings.Add("G447_15", "G447_15");
                        bulkCopy.ColumnMappings.Add("G4401_15", "G4401_15");
                        bulkCopy.ColumnMappings.Add("G4402_15", "G4402_15");
                        bulkCopy.ColumnMappings.Add("D_DATE", "D_DATE");
                        bulkCopy.ColumnMappings.Add("D_TIME", "D_TIME");
                        bulkCopy.ColumnMappings.Add("G_47_POSHL", "G_47_POSHL");
                        bulkCopy.ColumnMappings.Add("G_47_POSHLST", "G_47_POSHLST");
                        bulkCopy.ColumnMappings.Add("G_47_POSHLK", "G_47_POSHLK");
                        bulkCopy.ColumnMappings.Add("G_47_POSHLS", "G_47_POSHLS");
                        bulkCopy.ColumnMappings.Add("G_47_POSP", "G_47_POSP");
                        bulkCopy.ColumnMappings.Add("G_47_POSPST", "G_47_POSPST");
                        bulkCopy.ColumnMappings.Add("G_47_POSPK", "G_47_POSPK");
                        bulkCopy.ColumnMappings.Add("G_47_POSPS", "G_47_POSPS");
                        bulkCopy.ColumnMappings.Add("G_47_POAD", "G_47_POAD");
                        bulkCopy.ColumnMappings.Add("G_47_POADST", "G_47_POADST");
                        bulkCopy.ColumnMappings.Add("G_47_POADK", "G_47_POADK");
                        bulkCopy.ColumnMappings.Add("G_47_POADS", "G_47_POADS");
                        bulkCopy.ColumnMappings.Add("G_47_AKCIZ", "G_47_AKCIZ");
                        bulkCopy.ColumnMappings.Add("G_47_AKCIZST", "G_47_AKCIZST");
                        bulkCopy.ColumnMappings.Add("G_47_AKCIZK", "G_47_AKCIZK");
                        bulkCopy.ColumnMappings.Add("G_47_AKCIZS", "G_47_AKCIZS");
                        bulkCopy.ColumnMappings.Add("G_47_NDS", "G_47_NDS");
                        bulkCopy.ColumnMappings.Add("G_47_NDSST", "G_47_NDSST");
                        bulkCopy.ColumnMappings.Add("G_47_NDSK", "G_47_NDSK");
                        bulkCopy.ColumnMappings.Add("G_47_NDSS", "G_47_NDSS");
                        bulkCopy.ColumnMappings.Add("G_47_TAM", "G_47_TAM");
                        bulkCopy.ColumnMappings.Add("G_47_TAMST", "G_47_TAMST");
                        bulkCopy.ColumnMappings.Add("G_47_TAMK", "G_47_TAMK");
                        bulkCopy.ColumnMappings.Add("G_47_TAMS", "G_47_TAMS");
                        bulkCopy.ColumnMappings.Add("G_47_OTHER", "G_47_OTHER");
                        bulkCopy.ColumnMappings.Add("G_47_OTHERST", "G_47_OTHERST");
                        bulkCopy.ColumnMappings.Add("G_47_OTHERK", "G_47_OTHERK");
                        bulkCopy.ColumnMappings.Add("GTD_REGDT", "GTD_REGDT");
                        bulkCopy.ColumnMappings.Add("GTD_KURS", "GTD_KURS");
                        bulkCopy.ColumnMappings.Add("GTD_22_1", "GTD_22_1");
                        bulkCopy.ColumnMappings.Add("GTD_22_3", "GTD_22_3");
                        bulkCopy.ColumnMappings.Add("G_42_DOPS", "G_42_DOPS");
                        bulkCopy.ColumnMappings.Add("G_45_DOPS", "G_45_DOPS");
                        bulkCopy.ColumnMappings.Add("G_45_1", "G_45_1");
                        bulkCopy.ColumnMappings.Add("KTS45_1", "KTS45_1");
                        bulkCopy.ColumnMappings.Add("G_32_REALNUM", "G_32_REALNUM");
                        bulkCopy.ColumnMappings.Add("G_32_TOVNUM", "G_32_TOVNUM");
                        bulkCopy.ColumnMappings.Add("GTD_5", "GTD_5");
                        bulkCopy.ColumnMappings.Add("G_33_REAL", "G_33_REAL");
                        bulkCopy.ColumnMappings.Add("ID", "ID");
                        bulkCopy.ColumnMappings.Add("INV_NUM", "INV_NUM");
                        bulkCopy.ColumnMappings.Add("INV_DATE", "INV_DATE");
                        bulkCopy.ColumnMappings.Add("G_14_NAME", "G_14_NAME");
                        bulkCopy.ColumnMappings.Add("G_14_KPP", "G_14_KPP");
                        bulkCopy.ColumnMappings.Add("G_14_INN", "G_14_INN");

                        bulkCopy.WriteToServer(toSql);
                    }
                }
            }
            catch (Exception e)
            { 
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
