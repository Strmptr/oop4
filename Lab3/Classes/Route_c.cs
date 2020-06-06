using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;
using System.Device.Location;
using System.Windows.Shapes;
using System.Windows.Media;

namespace Lab3.Classes
{
    class Route_c : MapObject
    {
        List<PointLatLng> points = new List<PointLatLng>();
        public Route_c(string name, List<PointLatLng> Points) : base(name)
        {
            points = Points;
        }

        
        public override double getDistance(PointLatLng pointtwo)
        {
            GeoCoordinate geo1 = new GeoCoordinate(pointtwo.Lat, pointtwo.Lng);
            GeoCoordinate geo2 = new GeoCoordinate(points[0].Lat, points[0].Lng);
            double distance = geo1.GetDistanceTo(geo2);
            for (int i = 0; i < points.Count; i++)
            {
                geo2 = new GeoCoordinate(points[i].Lat, points[i].Lng);
                if (geo1.GetDistanceTo(geo2) < distance)
                    distance = geo1.GetDistanceTo(geo2);
            }
            return distance;
        }

        public override PointLatLng getFocus()
        {
            return points.Last();
        }

        public override GMapMarker GetMarker()
        {
            GMapMarker marker = new GMapRoute(points)
            {
                Shape = new Path()
                {
                    Stroke = Brushes.DarkBlue,
                    Fill = Brushes.DarkBlue,
                    StrokeThickness = 4,
                    ToolTip = getTitle()
                }
            };
            return marker;
        }

    }
}
