
namespace RoadVisualisation
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Redraw = new Button();
            ToleranceValue = new TextBox();
            panel2 = new Panel();
            label4 = new Label();
            label3 = new Label();
            label2 = new Label();
            Ref = new TextBox();
            Latitude = new TextBox();
            Longitude = new TextBox();
            Max_Speed_fld = new TextBox();
            CurveTolerance = new TextBox();
            CurvatureCalcMethod = new ComboBox();
            LangRangeLabel = new Label();
            LangRange = new TextBox();
            label1 = new Label();
            SimplificationMethod = new ComboBox();
            numOfPoints = new TextBox();
            map = new GMap.NET.WindowsForms.GMapControl();
            Canvas = new Panel();
            panel2.SuspendLayout();
            Canvas.SuspendLayout();
            SuspendLayout();
            // 
            // Redraw
            // 
            Redraw.Location = new Point(1189, 13);
            Redraw.Name = "Redraw";
            Redraw.Size = new Size(75, 23);
            Redraw.TabIndex = 1;
            Redraw.Text = "Redraw";
            Redraw.UseVisualStyleBackColor = true;
            Redraw.Click += button1_Click;
            // 
            // ToleranceValue
            // 
            ToleranceValue.Location = new Point(70, 13);
            ToleranceValue.Name = "ToleranceValue";
            ToleranceValue.Size = new Size(94, 23);
            ToleranceValue.TabIndex = 2;
            ToleranceValue.Text = "0.3";
            ToleranceValue.TextChanged += ToleranceValue_TextChanged;
            // 
            // panel2
            // 
            panel2.Controls.Add(label4);
            panel2.Controls.Add(label3);
            panel2.Controls.Add(label2);
            panel2.Controls.Add(Ref);
            panel2.Controls.Add(Latitude);
            panel2.Controls.Add(Longitude);
            panel2.Controls.Add(Max_Speed_fld);
            panel2.Controls.Add(CurveTolerance);
            panel2.Controls.Add(CurvatureCalcMethod);
            panel2.Controls.Add(LangRangeLabel);
            panel2.Controls.Add(LangRange);
            panel2.Controls.Add(label1);
            panel2.Controls.Add(SimplificationMethod);
            panel2.Controls.Add(ToleranceValue);
            panel2.Controls.Add(Redraw);
            panel2.Dock = DockStyle.Top;
            panel2.Location = new Point(0, 0);
            panel2.Name = "panel2";
            panel2.Size = new Size(1276, 48);
            panel2.TabIndex = 3;
            panel2.Paint += panel2_Paint;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(1055, 15);
            label4.Name = "label4";
            label4.Size = new Size(24, 15);
            label4.TabIndex = 15;
            label4.Text = "Ref";
            label4.Visible = false;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(882, 15);
            label3.Name = "label3";
            label3.Size = new Size(75, 15);
            label3.TabIndex = 14;
            label3.Text = "BottomRight";
            label3.Visible = false;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(749, 15);
            label2.Name = "label2";
            label2.Size = new Size(46, 15);
            label2.TabIndex = 13;
            label2.Text = "TopLeft";
            label2.Visible = false;
            label2.Click += label2_Click_1;
            // 
            // Ref
            // 
            Ref.Location = new Point(1085, 12);
            Ref.Name = "Ref";
            Ref.Size = new Size(94, 23);
            Ref.TabIndex = 12;
            Ref.Text = "503";
            // 
            // Latitude
            // 
            Latitude.Location = new Point(963, 12);
            Latitude.Name = "Latitude";
            Latitude.Size = new Size(70, 23);
            Latitude.TabIndex = 11;
            Latitude.Text = "48.306885,17.265176";
            // 
            // Longitude
            // 
            Longitude.Location = new Point(801, 12);
            Longitude.Name = "Longitude";
            Longitude.Size = new Size(75, 23);
            Longitude.TabIndex = 10;
            Longitude.Text = "48.370298,17.150139";
            Longitude.TextChanged += textBox1_TextChanged_1;
            // 
            // Max_Speed_fld
            // 
            Max_Speed_fld.Enabled = false;
            Max_Speed_fld.Location = new Point(420, 12);
            Max_Speed_fld.Name = "Max_Speed_fld";
            Max_Speed_fld.Size = new Size(79, 23);
            Max_Speed_fld.TabIndex = 9;
            // 
            // CurveTolerance
            // 
            CurveTolerance.Location = new Point(188, 13);
            CurveTolerance.Name = "CurveTolerance";
            CurveTolerance.Size = new Size(94, 23);
            CurveTolerance.TabIndex = 8;
            CurveTolerance.Text = "0.006";
            CurveTolerance.TextChanged += CurveTolerance_TextChanged;
            // 
            // CurvatureCalcMethod
            // 
            CurvatureCalcMethod.FormattingEnabled = true;
            CurvatureCalcMethod.Location = new Point(505, 13);
            CurvatureCalcMethod.Name = "CurvatureCalcMethod";
            CurvatureCalcMethod.Size = new Size(100, 23);
            CurvatureCalcMethod.TabIndex = 7;
            // 
            // LangRangeLabel
            // 
            LangRangeLabel.AutoSize = true;
            LangRangeLabel.Location = new Point(288, 15);
            LangRangeLabel.Name = "LangRangeLabel";
            LangRangeLabel.Size = new Size(40, 15);
            LangRangeLabel.TabIndex = 6;
            LangRangeLabel.Text = "Range";
            LangRangeLabel.Visible = false;
            LangRangeLabel.Click += label2_Click;
            // 
            // LangRange
            // 
            LangRange.Location = new Point(339, 12);
            LangRange.Name = "LangRange";
            LangRange.Size = new Size(64, 23);
            LangRange.TabIndex = 5;
            LangRange.Text = "4";
            LangRange.Visible = false;
            LangRange.TextChanged += LangRange_TextChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(7, 16);
            label1.Name = "label1";
            label1.Size = new Size(57, 15);
            label1.TabIndex = 4;
            label1.Text = "Tolerance";
            label1.Click += label1_Click;
            // 
            // SimplificationMethod
            // 
            SimplificationMethod.FormattingEnabled = true;
            SimplificationMethod.Location = new Point(611, 13);
            SimplificationMethod.Name = "SimplificationMethod";
            SimplificationMethod.Size = new Size(96, 23);
            SimplificationMethod.TabIndex = 3;
            SimplificationMethod.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            // 
            // numOfPoints
            // 
            numOfPoints.Enabled = false;
            numOfPoints.Location = new Point(709, 54);
            numOfPoints.Name = "numOfPoints";
            numOfPoints.Size = new Size(79, 23);
            numOfPoints.TabIndex = 10;
            numOfPoints.TextChanged += textBox1_TextChanged;
            // 
            // map
            // 
            map.AutoSize = true;
            map.Bearing = 0F;
            map.CanDragMap = true;
            map.Dock = DockStyle.Fill;
            map.EmptyTileColor = Color.Navy;
            map.GrayScaleMode = false;
            map.HelperLineOption = GMap.NET.WindowsForms.HelperLineOptions.DontShow;
            map.LevelsKeepInMemory = 5;
            map.Location = new Point(0, 0);
            map.MarkersEnabled = true;
            map.MaxZoom = 2;
            map.MinZoom = 2;
            map.MouseWheelZoomEnabled = true;
            map.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionAndCenter;
            map.Name = "map";
            map.NegativeMode = false;
            map.PolygonsEnabled = true;
            map.RetryLoadTile = 0;
            map.RoutesEnabled = true;
            map.ScaleMode = GMap.NET.WindowsForms.ScaleModes.Integer;
            map.SelectedAreaFillColor = Color.FromArgb(33, 65, 105, 225);
            map.ShowTileGridLines = false;
            map.Size = new Size(1276, 627);
            map.TabIndex = 11;
            map.Zoom = 0D;
            map.Load += gMapControl1_Load;
            // 
            // Canvas
            // 
            Canvas.BackColor = Color.White;
            Canvas.Controls.Add(map);
            Canvas.Controls.Add(numOfPoints);
            Canvas.Dock = DockStyle.Fill;
            Canvas.Location = new Point(0, 0);
            Canvas.Name = "Canvas";
            Canvas.Size = new Size(1276, 627);
            Canvas.TabIndex = 0;
            Canvas.Paint += Canvas_Paint;
            Canvas.MouseMove += Canvas_MouseMove;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1276, 627);
            Controls.Add(panel2);
            Controls.Add(Canvas);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            Canvas.ResumeLayout(false);
            Canvas.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private System.Windows.Forms.Button Redraw;
        private System.Windows.Forms.TextBox ToleranceValue;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ComboBox SimplificationMethod;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label LangRangeLabel;
        private System.Windows.Forms.TextBox LangRange;
        private System.Windows.Forms.ComboBox CurvatureCalcMethod;
        private System.Windows.Forms.TextBox CurveTolerance;
        private System.Windows.Forms.TextBox Max_Speed_fld;
        private TextBox Ref;
        private TextBox Latitude;
        private TextBox Longitude;
        private Label label2;
        private Label label3;
        private Label label4;
        private TextBox numOfPoints;
        private GMap.NET.WindowsForms.GMapControl map;
        private Panel Canvas;
    }
}

