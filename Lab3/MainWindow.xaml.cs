using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;
using System.Device.Location;
using Lab3.Classes;
using System.Threading;

namespace Lab3
{
    public partial class MainWindow : Window
    {
        List<MapObject> mapObjects = new List<MapObject>();
        List<MapObject> secondList = new List<MapObject>();
        PointLatLng point = new PointLatLng();
        List<PointLatLng> areapoints = new List<PointLatLng>();
        List<PointLatLng> routepoints = new List<PointLatLng>();
        bool creationmode = false;
        bool secondact = false;

        RoutingProvider routingProvider = GMapProviders.OpenStreetMap;
        static PointLatLng startOfRoute;
        static PointLatLng endOfRoute;
        List<PointLatLng> nearestPointPosition = new List<PointLatLng>();
        List<MapObject> nearestObjects = new List<MapObject>();

        public MainWindow()
        {
            InitializeComponent();
            MapLoad();
            createrb.IsChecked = true;
        }

        private void MapLoad()
        {
            GMaps.Instance.Mode = AccessMode.ServerAndCache;
            Map.MapProvider = GMapProviders.GoogleMap;
            Map.MinZoom = 2;
            Map.MaxZoom = 17;
            Map.Zoom = 15;
            Map.Position = new PointLatLng(55.012823, 82.950359);
            Map.MouseWheelZoomType = MouseWheelZoomType.MousePositionAndCenter;
            Map.CanDragMap = true;
            Map.DragButton = MouseButton.Left;
        }


        private void Map_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void MapLoaded(object sender, RoutedEventArgs e)
        {

        }

        private void Map_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            PointLatLng point = Map.FromLocalToLatLng((int)e.GetPosition(Map).X, (int)e.GetPosition(Map).Y);
            if (creationmode == true)
            {
                areapoints.Add(point);
            }

            else
            {
                OList.Items.Clear();
                // OList.Items.Add(null);
                secondList = mapObjects.OrderBy(mobject => mobject.getDistance(point)).ToList();
                foreach (MapObject obj in secondList)
                {
                    string mapObjectAndDistanceString = new StringBuilder()

                        .Append(" - ")
                        .Append(obj.getTitle())
                        .Append(" - ")
                        .Append(obj.getDistance(point).ToString("0.##"))
                        .Append(" м.").ToString();
                    OList.Items.Add(mapObjectAndDistanceString);
                }
                secondact = true;

            }

        }


        private void createMarker(List<PointLatLng> points, int index)
        {
            MapObject mapObject = null;
            switch (index)
            {
                case 4:
                    {
                        if (points.Count < 3)
                        {
                            MessageBox.Show("Выберите точки");
                            return;
                        }
                        mapObject = new Area(OName.Text, points);
                        break;
                    }
                case 0:
                    {
                        if (points.Count < 1)
                        {
                            MessageBox.Show("Выберите точки");
                            return;
                        }
                        mapObject = new Location_c(OName.Text, points.Last());
                        break;
                    }
                case 1:
                    {
                        if (points.Count < 1)
                        {
                            MessageBox.Show("Выберите точки");
                            return;
                        }

                        mapObject = new Car(OName.Text, points.Last());
                        break;
                    }
                case 2:
                    {
                        if (points.Count < 1)
                        {
                            MessageBox.Show("Выберите точки");
                            return;
                        }
                        mapObject = new Human(OName.Text, points.Last());
                        break;
                    }
                case 3:
                    {
                        if (points.Count < 2)
                        {
                            MessageBox.Show("Выберите точки");
                            return;
                        }
                        mapObject = new Route_c(OName.Text, points);
                        break;
                    }

            }
            if (mapObject != null)
            {
                mapObjects.Add(mapObject);
                Map.Markers.Add(mapObject.GetMarker());
            }
        }



        private void OList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (mapObjects.Count != 0)
            {
                if (OList.SelectedItem != null)
                {
                    foreach (MapObject obm in mapObjects)
                    {
                        if (obm.getTitle() == (string)OList.SelectedItem)
                        {
                            Map.Position = obm.getFocus();
                        }
                    }

                }
            }
        }

        Random rnd = new Random();

        //Получить очередное (в данном случае - первое) случайное число


        //Вывод полученного числа в консоль

        private void Createra_Click(object sender, RoutedEventArgs e)
        {
            bool exsist = false;
            if (OName.Text == "")
            {
                MessageBox.Show("Object name is null");
            }
            else
                foreach (MapObject obj in mapObjects)
                {
                    if (obj.getTitle() == OName.Text)
                    {
                        OName.Text += OName.Text + rnd.Next().ToString();
                    }


                }
            if (!exsist)
            {
                {
                    createMarker(areapoints, combox.SelectedIndex);
                    OName.Text = "";

                    locate.IsEnabled = true;
                    objfind.IsEnabled = true;
                    clearpoints.IsEnabled = false;
                    areapoints = new List<PointLatLng>();
                }
                OList.Items.Clear();
                for (int i = 0; i < mapObjects.Count; i++)
                {
                    if (i == 0)
                    {
                        // OList.Items.Add(null);
                        OList.Items.Add(mapObjects[i].getTitle());
                    }
                    else
                    {
                        OList.Items.Add(mapObjects[i].getTitle());
                    }
                }
                secondact = false;
            }
        }

        private void Combox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            createra.IsEnabled = true;
            clearpoints.IsEnabled = true;

            //  areapoints = new List<PointLatLng>();
        }

        private void Findrb_Checked(object sender, RoutedEventArgs e)
        {
            createrb.IsChecked = false;
            creationmode = false;
        }

        private void Createrb_Checked(object sender, RoutedEventArgs e)
        {
            findrb.IsChecked = false;
            creationmode = true;
        }

        private void Locate_Click(object sender, RoutedEventArgs e)
        {
            OList.Items.Clear();
            if (objfind.Text == "")
            {
                for (int i = 0; i < mapObjects.Count; i++)
                {
                    if (i == 0)
                    {
                        // OList.Items.Add(null);
                        OList.Items.Add(mapObjects[i].getTitle());
                    }
                    else
                    {
                        OList.Items.Add(mapObjects[i].getTitle());
                    }
                }
                secondact = false;
            }
            else
            {
                secondact = true;
                secondList.Clear();
                for (int i = 0; i < mapObjects.Count; i++)
                {

                    if (i == 0)
                    {
                        //  OList.Items.Add(null);
                        if (mapObjects[i].getTitle().Contains(objfind.Text))
                        {
                            OList.Items.Add(mapObjects[i].getTitle());
                            secondList.Add(mapObjects[i]);
                        }
                    }
                    else
                    {
                        if (mapObjects[i].getTitle().Contains(objfind.Text))
                        {
                            OList.Items.Add(mapObjects[i].getTitle());
                            secondList.Add(mapObjects[i]);
                        }
                    }
                }
            }
        }

        private void OList_MouseLeave(object sender, MouseEventArgs e)
        {
            // OList.SelectedIndex = 0;
        }

        private void Clearpoints_Click(object sender, RoutedEventArgs e)
        {

            areapoints = new List<PointLatLng>();
            routepoints = new List<PointLatLng>();
            clearpoints.IsEnabled = true;
            createra.IsEnabled = true;
        }

        private void OName_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Focus_Follow(object sender, EventArgs args)
        {
            Car c = (Car)sender;
            waybar.Maximum = c.route.Points.Count;
            Map.Position = c.getFocus();

            if (waybar.Value == waybar.Maximum)
                (sender as Car).Follow -= Focus_Follow;

            if (waybar.Value != waybar.Maximum)
                waybar.Value += 1;

            else
                waybar.Value = 0;

        }

        private void stuvk_Click(object sender, RoutedEventArgs e)
        {
            if (areapoints.Count == 0)
            {
                MessageBox.Show("точка назначения не выбрана");
            }
            else
            {
                waybar.Value = 0;
                foreach (MapObject obj in mapObjects)
                    if (obj is Human)
                        startOfRoute = (obj.getFocus());
                endOfRoute = areapoints.Last();
                var besidedObj = mapObjects.OrderBy(mapObject => mapObject.getDistance(startOfRoute));

                Car nearestCar = null;
                Human h = null;

                foreach (MapObject obj in mapObjects)
                {
                    if (obj is Human)
                    {
                        h = (Human)obj;
                        h.destinationPoint = endOfRoute;
                        break;
                    }
                }

                foreach (MapObject obj in besidedObj)
                {
                    if (obj is Car)
                    {
                        nearestCar = (Car)obj;
                        break;
                    }
                }

                if (nearestCar != null && h != null)
                {

                    var aaa = nearestCar.MoveTo(startOfRoute);
                    createMarker(aaa.Points, 3);

                    RoutingProvider routingProvider = GMapProviders.OpenStreetMap;
                    MapRoute route = routingProvider.GetRoute(
                        startOfRoute,
                        endOfRoute,
                        false,
                        false,
                        15);
                    createMarker(route.Points, 3);
                    nearestCar.Arrived += h.CarArrived;
                    h.seated += nearestCar.getintocar;
                    nearestCar.Follow += Focus_Follow;
                }

                //  nearestCar.Arrived -= h.CarArrived;
                //  h.seated -= nearestCar.getintocar;
            }
        }
    }



}
