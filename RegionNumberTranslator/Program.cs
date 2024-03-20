using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace RegionNumberTranslator
{
    public class Program
    {
        static void Main(string[] args)
        {
            //load region file
            Tools.GetRegions("NYC_862.txt");
            int i = 0;
            //string filePath = "C:\\Users\\fander\\Desktop\\graphhopper-master\\2014_yellow_10.csv_time33.txt"; // 替换为实际的文件路径
            string filePath = "C:\\Users\\fander\\Desktop\\graphhopper-master\\test.txt"; // 替换为实际的文件路径
            string filePath_route = "C:\\Users\\fander\\Desktop\\graphhopper-master\\test2.txt"; // 替换为实际的文件路径
            List<DataEntry> parsedData = ReadCsvFromFile(filePath);
            List<List<double>> route_list = ReadrouteFromFile(filePath_route);
            List<int> zone = new List<int>();
            int index = 0;
            foreach (var entry in parsedData)
            {   
                
                Console.WriteLine($"Start Time: {entry.StartTime}, End Time: {entry.EndTime}, Value: {entry.Value},Value2:{entry.Value2},Value3:{entry.Value3}");
                i += 1;
                List<double> route = route_list[index];
                //int length = route.length;
                for (int j=0;j<route.Count/2;j++)
                {
                    int[] region = Tools.GetRegionNumber(route[2*j], route[2*j+1]);
                    if( region!=null)
                        {
                            for
                            zone.Add(region[0]);
                        
                        Console.WriteLine(region[0]);
                    }
                    else
                    {
                        //double lat1 = route[2 * j];
                        //double lon2 = route[2 * j + 1];
                        int[] region1 = Tools.GetRegionNumber(route[2 * j]+0.00045, route[2 * j + 1]);
                        int[] region2 = Tools.GetRegionNumber(route[2 * j]-0.00045, route[2 * j + 1]);
                        int[] region3 = Tools.GetRegionNumber(route[2 * j], route[2 * j + 1]+0.00059);
                        int[] region4 = Tools.GetRegionNumber(route[2 * j], route[2 * j + 1]-0.00059);
                        if (region1 != null)
                        {
                            zone.Add(region1[0]);
                            Console.WriteLine(region1[0]);
                        }

                        if (region2 != null)
                        {
                            zone.Add(region2[0]);
                            Console.WriteLine(region2[0]);
                        }

                        if (region3 != null)
                        {
                            zone.Add(region3[0]);

                            Console.WriteLine(region3[0]);
                        }

                        if (region4 != null)
                        {
                            zone.Add(region4[0]);

                            Console.WriteLine(region4[0]);
                        }
                       // Console.WriteLine("++++++++++++++++++++++++++++++++");
                    }
                   
                }
                index+=1;
                //  int[] region = Tools.GetRegionNumber(entry.), double.Parse(args[1]));
                string numbersString = string.Join(", ", zone);

                // 打印连接后的字符串
                //Console.WriteLine(numbersString);
                Console.WriteLine(entry);
            }
            Console.WriteLine(i);
            //return region numbers, 0 to 861
            Console.WriteLine("hello123");
          //  Console.ReadKey();
        }
        static List<DataEntry> ReadCsvFromFile(string filePath)
        {
            int i = 0;
            List<DataEntry> entries = new List<DataEntry>();
            try
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] parts = line.Split(',');
                        
                        if (parts.Length == 5)
                        {
                            i += 1;
                            DateTime startTime = DateTime.ParseExact(parts[0], "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                            DateTime endTime = DateTime.ParseExact(parts[1], "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                            double value = double.Parse(parts[2], CultureInfo.InvariantCulture);
                            double value2 = double.Parse(parts[3], CultureInfo.InvariantCulture);
                            double value3 = double.Parse(parts[4], CultureInfo.InvariantCulture);

                            DataEntry entry = new DataEntry { StartTime = startTime, EndTime = endTime, Value = value,Value2=value2,Value3=value3 };
                            entries.Add(entry);
                        }

                    }
                }
            }
            catch (Exception ex)
            {Console.WriteLine($"Error reading file: {ex.Message}"); }
            Console.WriteLine(i);
            return entries;
        }
        static List<List<double>> ReadrouteFromFile(string filePath)
        {
            List<List<double>> route_list = new List<List<double>>();

            try
            {
                // 读取文件的每一行
                using (StreamReader sr = new StreamReader(filePath))
                {
                    string line;
                    // 遍历每一行并解析坐标数据
                    while ((line = sr.ReadLine()) != null)
                    {
                        List<double> route = new List<double>();
                        // 去除整行的括号
                      string  line2=line.Replace(")", "").Replace("(", "");

                        // 分割坐标数据
                        string[] parts = line2.Split(',');
                        int num = parts.Length / 2;
                        for (int i = 0; i < num; i++)
                        {
                            double latitude, longitude;
                            latitude = double.Parse(parts[2 * i]);
                            longitude = double.Parse(parts[2 * i + 1]);
                            route.Add(latitude);
                            route.Add(longitude);

                        }
                        route_list.Add(route);
                    }

                }
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Error reading file: {ex.Message}");
            }

            return route_list;
        }


    }
}

class DataEntry
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public double Value { get; set; }
    public double Value2 { get; set; }
    public double Value3 { get; set; }
}


/*class Program
{
    static void Main()
    {
        string filePath = "your_file_path.csv"; // 替换为实际的文件路径

        List<DataEntry> parsedData = ReadCsvFromFile(filePath);

        foreach (var entry in parsedData)
        {
            Console.WriteLine($"Start Time: {entry.StartTime}, End Time: {entry.EndTime}, Value: {entry.Value}");
        }

        Console.ReadLine();
    }

    static List<DataEntry> ReadCsvFromFile(string filePath)
    {
        List<DataEntry> entries = new List<DataEntry>();

        try
        {
            using (StreamReader sr = new StreamReader(filePath))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] parts = line.Split(',');

                    if (parts.Length == 3)
                    {
                        DateTime startTime = DateTime.ParseExact(parts[0], "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                        DateTime endTime = DateTime.ParseExact(parts[1], "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                        double value = double.Parse(parts[2], CultureInfo.InvariantCulture);

                        DataEntry entry = new DataEntry { StartTime = startTime, EndTime = endTime, Value = value };
                        entries.Add(entry);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading file: {ex.Message}");
        }

        return entries;
    }
}

    */