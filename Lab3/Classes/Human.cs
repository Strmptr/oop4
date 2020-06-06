using GMap.NET;
using GMap.NET.WindowsPresentation;
using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Device.Location;
using Lab3.Classes;

namespace Lab3
{
    class Human : MapObject
    {

        public PointLatLng point { get; set; }
        public PointLatLng destinationPoint { get; set; }
        public GMapMarker marker { get; private set; }

        public event EventHandler seated;


        public Human(string name, PointLatLng Point) : base(name)
        {
            this.point = Point;
        }

        public override double getDistance(PointLatLng pointtwo)
        {
            GeoCoordinate geo1 = new GeoCoordinate(point.Lat, point.Lng);
            GeoCoordinate geo2 = new GeoCoordinate(pointtwo.Lat, pointtwo.Lng);
            return geo1.GetDistanceTo(geo2);
        }

        public override PointLatLng getFocus()
        {
            return point;
        }

        public override GMapMarker GetMarker()
        {
            marker = new GMapMarker(point)
            {
                Shape = new Image
                {
                    Width = 32,
                    Height = 32,
                    ToolTip = getTitle(),
                    Source = new BitmapImage(new Uri("pack://application:,,,/Resources/men.png"))
                }
            };
            return marker;
        }

        public void CarArrived(object sender, EventArgs args)
        {

            System.Windows.MessageBox.Show("car is arrived ");
            seated?.Invoke(this, EventArgs.Empty);
            (sender as Car).Arrived -= CarArrived;


        }


    }
}
