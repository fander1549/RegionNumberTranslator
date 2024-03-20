using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Threading;

namespace RegionNumberTranslator
{
    public class Tools
    {
        public static int ImageWidth = 2400;
        public static int ImageHeight = 2400;
        public static int RegionRange = 862; //default
        public static int[,] regions;
        public static double[,] RegionCenter = new double[Tools.RegionRange, 2]; // latitude and longitude of all region centers
        public static Dictionary<int, int> Region_Area_Map;
        public static LatLngBox NYC_BoundingBox = new LatLngBox(40.918, -74.259, 40.486, -73.7);

        #region get region
        public static void GetRegions(string regionsFile)
        {
            Console.WriteLine("Start Read Regions, " + DateTime.Now);
            Region_Area_Map = new Dictionary<int, int>();
            regions = new int[ImageWidth, ImageHeight];

            #region Get boundary
            string reader;
            StreamReader sr = new StreamReader(regionsFile);
            int numOfLines = 0;
            while ((reader = sr.ReadLine()) != null)
            {
                string[] items = reader.Split('\t');
                for (int i = 0; i < ImageHeight; i++)
                {
                    regions[numOfLines, i] = Convert.ToInt32(items[i]);
                    //为每片小区域分配所属的大区域index
                }
                numOfLines++;
            }
            sr.Close();
            #endregion

            #region get area of each region
            for (int i = 0; i < ImageWidth; i++)
            {
                for (int j = 0; j < ImageHeight; j++)
                {
                    if (regions[i, j] != 0)
                    {
                        int area;
                        if (Region_Area_Map.TryGetValue(regions[i, j], out area))
                        {
                            Region_Area_Map.Remove(regions[i, j]);
                            Region_Area_Map.Add(regions[i, j], ++area);
                        }
                        else
                        {
                            Region_Area_Map.Add(regions[i, j], 1);
                        }
                    }
                }
            }
            #endregion
            Console.WriteLine("Numner of regions with area is " + Region_Area_Map.Count);
            RegionRange = Region_Area_Map.Count;

            Console.WriteLine("Finish Read Regions, " + DateTime.Now);
        }

        public static int[] GetRegionNumber(double lat, double lng)
        {
            int[] cors;
            if ((cors = GetCoordinate(lat, lng)) == null)
            {
                return null;
            }

            List<int> rn = new List<int>();
            for (int x = Math.Max(cors[0] - 1, 0); x <= Math.Min(cors[0] + 1, ImageWidth - 1); x++)
            {
                for (int y = Math.Max(cors[1] - 1, 0); y <= Math.Min(cors[1] + 1, ImageHeight - 1); y++)
                {
                    int regionNum = regions[x, y];
                    if (!(regionNum == 0 || regionNum == -1))
                    {
                        if (!rn.Contains(regionNum - 1))
                        {
                            rn.Add(regionNum - 1);
                        }
                    }
                }
            }
            if (rn.Count == 0)
            {
                return null;
            }
            return rn.ToArray();
        }

        #endregion

        public static int[] GetCoordinate(double lat, double lng)
        {
            SetConstant(ImageWidth, ImageHeight);
            LatLng ll = new LatLng(lat, lng);
            float[] pixel = GetPix(ll);

            int[] Pix = new int[2];
            if (pixel[0] > 0 && pixel[1] > 0)
            {
                Pix[0] = (int)Math.Floor(pixel[0]);
                Pix[1] = (int)Math.Floor(pixel[1]);
                return Pix;
            }
            else
            {
                return null;
            }
        }

        public static float[] GetPix(LatLng latlng)
        {
            float[] pixel = new float[2];
            if (NYC_BoundingBox.isInLatLngBox(latlng) == 1)
            {
                VEPixel pix = VEPixel.LatLongToPixel(latlng, VEConstants.ZoomLevel);
                PointF ptf = new PointF(Convert.ToSingle(pix.X - VEConstants.LeftTop.X), Convert.ToSingle(pix.Y - VEConstants.LeftTop.Y));
                pixel[0] = ptf.Y;
                pixel[1] = ptf.X;
            }
            return pixel;
        }

        public static void SetConstant(float width, float height)
        {
            VEConstants.ZoomLevel = 18;

            double y1 = NYC_BoundingBox.LeftTop.Latitude;
            double y2 = NYC_BoundingBox.RightBottom.Latitude;
            double a_y1 = A(y1);
            double a_y2 = A(y2);
            VEConstants.TileSizeLat = height * VEConstants.EarthCircum / ((1 << VEConstants.ZoomLevel) * (a_y1 - a_y2));

            double x1 = NYC_BoundingBox.LeftTop.Longitude;
            double x2 = NYC_BoundingBox.RightBottom.Longitude;
            double d_x1 = D(x1);
            double d_x2 = D(x2);
            VEConstants.TileSizeLon = width * VEConstants.EarthCircum / ((1 << VEConstants.ZoomLevel) * (d_x2 - d_x1));

            VEConstants.LeftTop = VEPixel.LatLongToPixel(NYC_BoundingBox.LeftTop, VEConstants.ZoomLevel);
        }

        public static double A(double y)
        {
            y += VEConstants.LatitudeBias;
            double d = Math.Sin(VEPixel.DegToRad(y));
            double a = Math.Log((d + 1) / (1 - d)) * VEConstants.EarthRadius / 2;
            return a;
        }

        public static double D(double x)
        {
            x += VEConstants.LongitudeBias;
            double d = (VEPixel.DegToRad(x) * VEConstants.EarthRadius + VEConstants.EarthHalfCirc);
            return d;
        }

    }

    public static class VEConstants
    {
        public static VEPixel LeftTop;
        public static double TileSizeLat;
        public static double TileSizeLon;
        public static int ZoomLevel;
        public static double EarthRadius = 6378137;
        public static double EarthCircum = EarthRadius * 2.0 * Math.PI;
        public static double EarthHalfCirc = EarthCircum / 2;
        public static double LatitudeBias = 0;
        public static double LongitudeBias = 0;
    }

    public class VEPixel
    {
        public double X, Y;

        public VEPixel(double x, double y)
        {
            X = x;
            Y = y;
        }

        public static VEPixel LatLongToPixel(LatLng ve, int zoomLevel)
        {
            double x = LongitudeToXAtZoom(ve.Longitude, zoomLevel);
            double y = LatitudeToYAtZoom(ve.Latitude, zoomLevel);
            return new VEPixel(x, y);
        }

        public static double LatitudeToYAtZoom(double y, int zoom)
        {
            y += VEConstants.LatitudeBias;

            double arc = VEConstants.EarthCircum / ((1 << zoom) * VEConstants.TileSizeLat);
            double d = Math.Sin(DegToRad(y));
            double a = Math.Log((d + 1) / (1 - d)) * VEConstants.EarthRadius / 2;
            return (VEConstants.EarthHalfCirc - a) / arc;
        }

        public static double LongitudeToXAtZoom(double x, int zoom)
        {
            x += VEConstants.LongitudeBias;

            double arc = VEConstants.EarthCircum / ((1 << zoom) * VEConstants.TileSizeLon);
            double d = (DegToRad(x) * VEConstants.EarthRadius + VEConstants.EarthHalfCirc) / arc;
            return d;
        }

        public static double DegToRad(double d)
        {
            return d * Math.PI / 180.0;
        }
    }
}
