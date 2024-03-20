using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegionNumberTranslator
{
    public class zone
    {
        static int[] zone_num(double lon,double lat)
        {
            //load region file
            Tools.GetRegions("NYC_862.txt");

            //return region numbers, 0 to 861
            int[] region = Tools.GetRegionNumber(lon, lat);
            int jiang = 1998;
            Console.WriteLine(region[0]);
            Console.WriteLine("hello123");
            Console.ReadKey();
            return region;
        }


    }
}
