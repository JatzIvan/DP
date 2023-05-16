using ConsoleApp1.Api;
using CoreLibrary;
using CoreLibrary.RoadSectionHandling;
using CoreLibrary.RoadSectionHandling.Data;
using CoreLibrary.RoadSectionHandling.Model;
using CoreLibrary.RoadSectionHandling.RoadSimplificators;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using NetTopologySuite.Index.KdTree;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RoadVisualisation
{
    public partial class Form1 : Form
    {

        static float tolerance;
        static int regionSizeVal;
        static Dictionary<Tuple<int, int>, double> speedBasedOnPoint = new Dictionary<Tuple<int, int>, double>();

        private static long numberOfPoints = 0;

        private static GMapOverlay polyOverlay;

        public Form1()
        {

            InitializeComponent();
            ApplicationConfigurationHandler.LoadConfiguration();

            ApiHelper.InitializeClient();
            SimplificationMethod.DataSource = (SectionSimplificationFactory.GetInstance()).GetLoadedTypes().Select(_ => _.Name).ToList();
            CurvatureCalcMethod.DataSource = (RoadCurvitureCalculatorFactory.GetInstance()).GetLoadedTypes().Select(_ => _.Name).ToList();
            polyOverlay = new GMapOverlay("polygons");
            map.MapProvider = GMapProviders.OpenStreetMap;
            map.Overlays.Add(polyOverlay);
            map.Position = new GMap.NET.PointLatLng(48.138736, 17.099710);
            map.MaxZoom = 100;
            map.MinZoom = 5;
            map.Zoom = 15;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void DrawLineMap(LocationPoint point1, LocationPoint point2, double radius, GMapOverlay polyOverlay)
        {

            List<PointLatLng> points = new List<PointLatLng>();
            points.Add(new PointLatLng(point1.Latitude, point1.Longitude));
            points.Add(new PointLatLng(point2.Latitude, point2.Longitude));

            GMapPolygon polygon = new GMapPolygon(points, "mypolygon");
            /*polygon.Fill = new SolidBrush(Color.FromArgb(50, Color.Red));*/
            polygon.Stroke = radius < float.Parse(CurveTolerance.Text, CultureInfo.InvariantCulture) ? new Pen(Color.Green, 3) : new Pen(Color.Red, 3);
            polyOverlay.Polygons.Add(polygon);
        }

        private async void button1_Click(object sender, EventArgs e)
        {




            double latitude1 = double.Parse(Longitude.Text.Split(",")[0], CultureInfo.InvariantCulture);
            double longitude1 = double.Parse(Longitude.Text.Split(",")[1], CultureInfo.InvariantCulture);

            double latitude2 = double.Parse(Latitude.Text.Split(",")[0], CultureInfo.InvariantCulture);
            double longitude2 = double.Parse(Latitude.Text.Split(",")[1], CultureInfo.InvariantCulture);

            ApplicationConfigurationHandler.Longitude1 = longitude1;
            ApplicationConfigurationHandler.Latitude1 = latitude1;
            ApplicationConfigurationHandler.Longitude2 = longitude2;
            ApplicationConfigurationHandler.Latitude2 = latitude2;
            ApplicationConfigurationHandler.CustomRoadParameters = "ref=" + Ref.Text;
            ApplicationConfigurationHandler.TestRoadQuery = "?ref=" + Ref.Text + "&" +
                    $"long1={longitude1.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)}" +
                    $"&lat1={latitude1.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)}" +
                    $"&long2={longitude2.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)}" +
                    $"&lat2={latitude2.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)}";


            RoadDataFetcher.ClearInstance();

            map.Position = new GMap.NET.PointLatLng(latitude1, longitude1);

            tolerance = float.Parse(ToleranceValue.Text, CultureInfo.InvariantCulture);
            regionSizeVal = int.Parse(LangRange.Text, CultureInfo.InvariantCulture);
            /*            x_cent = Canvas.Width / 2;
                        y_cent = Canvas.Height / 2;*/
            numberOfPoints = 0;
            /*Canvas.Refresh();*/

            ApiHelper.InitializeClient();
            RoadDataHandler roadHandler = new RoadDataHandler("503", Ref.Text);
            speedBasedOnPoint = new Dictionary<Tuple<int, int>, double>();
            string model = (string)SimplificationMethod.SelectedItem;
            string curv = (string)CurvatureCalcMethod.SelectedItem;

            ApplicationConfigurationHandler.DPTolerance = tolerance;

            if (SimplificationMethod.SelectedItem.Equals("LangRoadSectionSimplification"))
            {
                ApplicationConfigurationHandler.LangRegionSize = regionSizeVal;
                //model = new LangConfig(tolerance, regionSizeVal);
            }

            Task task = Task.Run(() =>
            {
                roadHandler.GetParsedRoadData(new HandlerSetupConfig(model, curv));
            }
            );

            await Task.WhenAll(task);
            polyOverlay.Clear();

            foreach (AbstractRoadModel entry in roadHandler.GetParsedRoadDataList(new HandlerSetupConfig(model, curv)))
            {
                numberOfPoints++;
                //AddPoint(entry.CurrentLocation, entry.MaxSpeed);
                if (entry.Next != null)
                {
                    DrawLineMap(entry.CurrentLocation, entry.Next.Point.CurrentLocation, entry.Next.RadiusOfCurvature, polyOverlay);
                }
            }
            numOfPoints.Text = numberOfPoints + "";

            map.Zoom = 15;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SimplificationMethod.SelectedItem.Equals("LangRoadSectionSimplification"))
            {
                LangRange.Visible = true;
                LangRangeLabel.Visible = true;
            }
            else
            {
                LangRange.Visible = false;
                LangRangeLabel.Visible = false;
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void CurveTolerance_TextChanged(object sender, EventArgs e)
        {

        }

        private void ToleranceValue_TextChanged(object sender, EventArgs e)
        {


        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void LangRange_TextChanged(object sender, EventArgs e)
        {

        }

        private void Canvas_Paint(object sender, PaintEventArgs e)
        {
        }

        private void gMapControl1_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void label2_Click_1(object sender, EventArgs e)
        {

        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            map.Manager.CancelTileCaching();
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void CurvatureCalcMethod_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
