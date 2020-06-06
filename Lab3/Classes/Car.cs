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
using GMap.NET.MapProviders;
using System.Threading;
using System.Windows;

namespace Lab3.Classes
{
    class Car : MapObject
    {
        PointLatLng point = new PointLatLng();

        private List<Human> passengers = new List<Human>();
        public MapRoute route { get; private set; }
        GMapMarker marker;

        private Human human;

        public event EventHandler Arrived;
        public event EventHandler Follow;
        

        public Car(string name, PointLatLng Point) : base(name)
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
                    Source = new BitmapImage(new Uri("pack://application:,,,/Resources/car.png"))
                }
            };
            return marker;
        }


        public MapRoute MoveTo(PointLatLng endpoint)
        {
            RoutingProvider routingProvider = GMapProviders.OpenStreetMap;
            route = routingProvider.GetRoute(
                point,
                endpoint,
                false,
                false,
                15);

            Thread ridingCar = new Thread(MoveByRoute);
            ridingCar.Start();
            return route;
        }

        private void MoveByRoute()
        {
            try
            {
                foreach (var point in route.Points)
                {
                    Application.Current.Dispatcher.Invoke(delegate
                    {
                        this.point = point;
                        marker.Position = point;

                        if (human != null) // human = null
                        {
                            human.marker.Position = point;
                            Follow?.Invoke(this, null);
                        }
                    });

                    Thread.Sleep(700);
                }



                if (human == null)
                    Arrived?.Invoke(this, null);
                else
                {
                    MessageBox.Show("get destination");
                    human = null;
                    Arrived?.Invoke(this, null);
                }
            }
            catch
            {

            }
        }

        public void getintocar(object sender, EventArgs args)
        {

            human = (Human)sender;
            MoveTo(human.destinationPoint);
            human.point = point;
            (sender as Human).seated -= getintocar;
        }



    }
}
