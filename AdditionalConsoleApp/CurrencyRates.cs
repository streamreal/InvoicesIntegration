using System;
using System.Linq;
using System.Xml.Linq;

namespace AdditionalConsoleApp
{
    class CurrencyRates
    {
        public static void GetRates()
        {
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

            foreach (var item in query)
            {
                Console.WriteLine(item);
            }
            Console.ReadLine();
        }
    }
}
