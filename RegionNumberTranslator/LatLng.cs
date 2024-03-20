using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegionNumberTranslator
{
    public class LatLng
    {
        public double Latitude;
        public double Longitude;

        public LatLng(double lat, double lng)
        {
            this.Latitude = lat;
            this.Longitude = lng;
        }
    }

    public class LatLngBox
    {
        public LatLng LeftTop;
        public LatLng RightBottom;

        public LatLngBox(double leftTopLat, double leftTopLng, double rightBottomLat, double rightBottomLng)
        {
            LeftTop = new LatLng(leftTopLat, leftTopLng);
            RightBottom = new LatLng(rightBottomLat, rightBottomLng);
        }

        public int isInLatLngBox(LatLng ll)
        {
            LatLngBox box = this;
            if (ll.Latitude > box.RightBottom.Latitude && ll.Latitude < box.LeftTop.Latitude && ll.Longitude > box.LeftTop.Longitude && ll.Longitude < box.RightBottom.Longitude)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }
}
