using ConsoleApp1.Api;
using ConsoleApp2;
using ConsoleApp2.RoadSectionHandling;
using ConsoleApp2.RoadSectionHandling.CircleCurvitureModel;
using ConsoleApp2.RoadSectionHandling.Data;
using ConsoleApp2.RoadSectionHandling.Model;
using ConsoleApp2.RoadSectionHandling.RoadSimplificators;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        Pen blackPen = new Pen(Color.Black);
        Pen orangePen = new Pen(Color.Orange);
        Pen redPen = new Pen(Color.Red);
        Graphics g = null;

        static int x_cent, y_cent;

        static float tolerance;
        static int regionSizeVal;
        private static LocationPoint topPoint = new LocationPoint(17.16, 48.38);
        private static LocationPoint bottomPoint = new LocationPoint(17.27, 48.31);

        private static Tuple<double, double> topPointXY;
        private static Tuple<double, double> bottomPointXY;

        private readonly float EarthRadius = 6371;      //Earth Radius in Km

        //## Now I can calculate the global X and Y for each reference point ##\\

        // This function converts lat and lng coordinates to GLOBAL X and Y positions
        private Tuple<double, double> latlngToGlobalXY(LocationPoint point)
        {
            //Calculates x based on cos of average of the latitudes
            double x = EarthRadius * point.Longitude * Math.Cos((topPoint.Latitude + bottomPoint.Latitude) / 2);
            //Calculates y based on latitude
            double y = EarthRadius * point.Latitude;
            return new Tuple<double, double>(x,y);
        }

        /*
        * This gives me the X and Y in relation to map for the 2 reference points.
        * Now we have the global AND screen areas and then we can relate both for the projection point.
        */

        // This function converts lat and lng coordinates to SCREEN X and Y positions
        private Tuple<double, double> latlngToScreenXY(LocationPoint point, Tuple<double, double> screen)
        {
            //Calculate global X and Y for projection point
            Tuple<double, double> pos = latlngToGlobalXY(point);
            //Calculate the percentage of Global X position in relation to total global width
            double perX = ((pos.Item1 - topPointXY.Item1) / (bottomPointXY.Item1 - topPointXY.Item1));
            //Calculate the percentage of Global Y position in relation to total global height
            double perY = ((pos.Item2 - topPointXY.Item2) / (bottomPointXY.Item2 - topPointXY.Item2));

            //Returns the screen position based on reference points
            
            return new Tuple<double, double>(0 + (screen.Item1 - 0) * perX,
                                      0 + (screen.Item2 - 0) * perY);
        }

        public Form1()
        {
            InitializeComponent();
            ApplicationConfigurationHandler.LoadConfiguration();

            ApiHelper.InitializeClient(ApplicationConfigurationHandler.DigitalMapConnection);
            topPointXY = latlngToGlobalXY(topPoint);
            bottomPointXY = latlngToGlobalXY(bottomPoint);
            SimplificationMethod.DataSource = Enum.GetValues(typeof(SimplMethods));
            CurvatureCalcMethod.DataSource = Enum.GetValues(typeof(CurvCalcMethods));
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void AddPoint(LocationPoint point)
        {

            Tuple<double, double> pointLoc = latlngToScreenXY(point, new Tuple<double, double>(Canvas.Width, Canvas.Height));

            g.FillEllipse(new SolidBrush(Color.Black), (int)pointLoc.Item1 - 5, (int)pointLoc.Item2 - 5 , 5, 5);
        }

        private void DrawLine(LocationPoint point1, LocationPoint point2, double radius)
        {
            Tuple<double, double> pointLoc1 = latlngToScreenXY(point1, new Tuple<double, double>(Canvas.Width, Canvas.Height));
            Tuple<double, double> pointLoc2 = latlngToScreenXY(point2, new Tuple<double, double>(Canvas.Width, Canvas.Height));

            g.DrawLine(radius < 2 ? Pens.Green : Pens.Red, new PointF((float)pointLoc1.Item1, (float)pointLoc1.Item2), new PointF((float)pointLoc2.Item1, (float)pointLoc2.Item2));
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            tolerance = float.Parse(ToleranceValue.Text, CultureInfo.InvariantCulture);
            regionSizeVal = int.Parse(LangRange.Text, CultureInfo.InvariantCulture);
            x_cent = Canvas.Width / 2;
            y_cent = Canvas.Height / 2;

            Canvas.Refresh();

            ApiHelper.InitializeClient();
            RoadDataHandler roadHandler = new RoadDataHandler("a", "a");

            AbstractSimplificationModel model;
            CurvCalcMethods curv = (CurvCalcMethods)CurvatureCalcMethod.SelectedItem;

            if (SimplificationMethod.SelectedItem.Equals(SimplMethods.LANG))
            {
                model = new LangConfig(tolerance, regionSizeVal);
            }
            else
            {
                model = new DouglasPeuckerConfig(tolerance);
            }

            Task task = Task.Run(() => {
                roadHandler.GetParsedRoadData(new HandlerSetupConfig(model, curv));
                }
            );

            await Task.WhenAll(task);
            g = Canvas.CreateGraphics();

            //PointF point1 = new PointF(200, 200);
            //PointF point2 = PointF.Add(point1, new Size(20, 20));
            //g.DrawLine(Pens.Black, point1, point2);

            foreach (KeyValuePair<LocationPoint, AbstractRoadModel> entry in roadHandler.GetParsedRoadData(new HandlerSetupConfig(model, curv)))
            {
                AddPoint(entry.Key);
                if(entry.Value.Next != null)
                {
                    DrawLine(entry.Value.CurrentLocation, entry.Value.Next.Point.CurrentLocation, 1/entry.Value.Next.RadiusOfCurvature);
                }
            }
            Console.WriteLine("HEREE");
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SimplificationMethod.SelectedItem.Equals(SimplMethods.LANG))
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

        private void Canvas_Paint(object sender, PaintEventArgs e)
        {
        }
    }
}
