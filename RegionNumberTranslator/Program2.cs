/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegionNumberTranslator
{
    class Program2
    {
        static void Main(string[] args)
        {
            //load region file
            Tools.GetRegions("NYC_862.txt");

            //return region numbers, 0 to 861
            int[] region = Tools.GetRegionNumber(40.723072551168954, -74.01107507264338);
            Console.WriteLine(region[0]);
        }
    }
}
*/