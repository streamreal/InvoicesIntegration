using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace сSharpProject
{
    class MainIntro
    {
        public static void Main()
        {
            //CurrencyRate.GetRates();
            
            DeclarationParser d = new DeclarationParser();
            d.Go();

            //Web_api w = new Web_api();
            //w.ProcessXml();

            //Linq l = new Linq();
            //l.Go_inv();
        }
    }
}
