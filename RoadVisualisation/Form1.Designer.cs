
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
            label3 = new Label();
            label2 = new Label();
            label7 = new Label();
            LangRangeLabel = new Label();
            label5 = new Label();
            label1 = new Label();
            Ref = new TextBox();
            Latitude = new TextBox();
            Longitude = new TextBox();
            CurveTolerance = new TextBox();
            CurvatureCalcMethod = new ComboBox();
            LangRange = new TextBox();
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
            Redraw.Anchor = AnchorStyles.None;
            Redraw.CausesValidation = false;
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
            ToleranceValue.Anchor = AnchorStyles.None;
            ToleranceValue.CausesValidation = false;
            ToleranceValue.Location = new Point(70, 13);
            ToleranceValue.Name = "ToleranceValue";
            ToleranceValue.Size = new Size(62, 23);
            ToleranceValue.TabIndex = 2;
            ToleranceValue.Text = "0.3";
            ToleranceValue.TextChanged += ToleranceValue_TextChanged;
            // 
            // panel2
            // 
            panel2.Controls.Add(label3);
            panel2.Controls.Add(label2);
            panel2.Controls.Add(label7);
            panel2.Controls.Add(LangRangeLabel);
            panel2.Controls.Add(label5);
            panel2.Controls.Add(label1);
            panel2.Controls.Add(Ref);
            panel2.Controls.Add(Latitude);
            panel2.Controls.Add(Longitude);
            panel2.Controls.Add(CurveTolerance);
            panel2.Controls.Add(CurvatureCalcMethod);
            panel2.Controls.Add(LangRange);
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
            // label3
            // 
            label3.Anchor = AnchorStyles.None;
            label3.AutoSize = true;
            label3.CausesValidation = false;
            label3.Location = new Point(1106, 17);
            label3.Name = "label3";
            label3.Size = new Size(24, 15);
            label3.TabIndex = 20;
            label3.Text = "Ref";
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.None;
            label2.AutoSize = true;
            label2.CausesValidation = false;
            label2.Location = new Point(882, 17);
            label2.Name = "label2";
            label2.Size = new Size(75, 15);
            label2.TabIndex = 19;
            label2.Text = "BottomRight";
            // 
            // label7
            // 
            label7.Anchor = AnchorStyles.None;
            label7.AutoSize = true;
            label7.CausesValidation = false;
            label7.Location = new Point(665, 16);
            label7.Name = "label7";
            label7.Size = new Size(46, 15);
            label7.TabIndex = 18;
            label7.Text = "TopLeft";
            // 
            // LangRangeLabel
            // 
            LangRangeLabel.Anchor = AnchorStyles.None;
            LangRangeLabel.AutoSize = true;
            LangRangeLabel.CausesValidation = false;
            LangRangeLabel.Location = new Point(293, 17);
            LangRangeLabel.Name = "LangRangeLabel";
            LangRangeLabel.Size = new Size(40, 15);
            LangRangeLabel.TabIndex = 17;
            LangRangeLabel.Text = "Range";
            // 
            // label5
            // 
            label5.Anchor = AnchorStyles.None;
            label5.AutoSize = true;
            label5.CausesValidation = false;
            label5.Location = new Point(138, 17);
            label5.Name = "label5";
            label5.Size = new Size(59, 15);
            label5.TabIndex = 16;
            label5.Text = "Curvature";
            label5.Click += label5_Click;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.None;
            label1.AutoSize = true;
            label1.CausesValidation = false;
            label1.Location = new Point(7, 17);
            label1.Name = "label1";
            label1.Size = new Size(57, 15);
            label1.TabIndex = 4;
            label1.Text = "Tolerance";
            label1.Click += label1_Click;
            // 
            // Ref
            // 
            Ref.Anchor = AnchorStyles.None;
            Ref.CausesValidation = false;
            Ref.Location = new Point(1136, 12);
            Ref.Name = "Ref";
            Ref.Size = new Size(43, 23);
            Ref.TabIndex = 12;
            Ref.Text = "503";
            // 
            // Latitude
            // 
            Latitude.Anchor = AnchorStyles.None;
            Latitude.CausesValidation = false;
            Latitude.Location = new Point(963, 12);
            Latitude.Name = "Latitude";
            Latitude.Size = new Size(137, 23);
            Latitude.TabIndex = 11;
            Latitude.Text = "48.306885,17.265176";
            // 
            // Longitude
            // 
            Longitude.Anchor = AnchorStyles.None;
            Longitude.CausesValidation = false;
            Longitude.Location = new Point(717, 12);
            Longitude.Name = "Longitude";
            Longitude.Size = new Size(159, 23);
            Longitude.TabIndex = 10;
            Longitude.Text = "48.370298,17.150139";
            Longitude.TextChanged += textBox1_TextChanged_1;
            // 
            // CurveTolerance
            // 
            CurveTolerance.Anchor = AnchorStyles.None;
            CurveTolerance.CausesValidation = false;
            CurveTolerance.Location = new Point(201, 13);
            CurveTolerance.Name = "CurveTolerance";
            CurveTolerance.Size = new Size(81, 23);
            CurveTolerance.TabIndex = 8;
            CurveTolerance.Text = "0.006";
            CurveTolerance.TextChanged += CurveTolerance_TextChanged;
            // 
            // CurvatureCalcMethod
            // 
            CurvatureCalcMethod.Anchor = AnchorStyles.None;
            CurvatureCalcMethod.CausesValidation = false;
            CurvatureCalcMethod.FormattingEnabled = true;
            CurvatureCalcMethod.Location = new Point(378, 12);
            CurvatureCalcMethod.Name = "CurvatureCalcMethod";
            CurvatureCalcMethod.Size = new Size(136, 23);
            CurvatureCalcMethod.TabIndex = 7;
            CurvatureCalcMethod.SelectedIndexChanged += CurvatureCalcMethod_SelectedIndexChanged;
            // 
            // LangRange
            // 
            LangRange.Anchor = AnchorStyles.None;
            LangRange.CausesValidation = false;
            LangRange.Location = new Point(339, 12);
            LangRange.Name = "LangRange";
            LangRange.Size = new Size(24, 23);
            LangRange.TabIndex = 5;
            LangRange.Text = "4";
            LangRange.Visible = false;
            LangRange.TextChanged += LangRange_TextChanged;
            // 
            // SimplificationMethod
            // 
            SimplificationMethod.Anchor = AnchorStyles.None;
            SimplificationMethod.CausesValidation = false;
            SimplificationMethod.FormattingEnabled = true;
            SimplificationMethod.Location = new Point(530, 12);
            SimplificationMethod.Name = "SimplificationMethod";
            SimplificationMethod.Size = new Size(129, 23);
            SimplificationMethod.TabIndex = 3;
            SimplificationMethod.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            // 
            // numOfPoints
            // 
            numOfPoints.Enabled = false;
            numOfPoints.Location = new Point(0, 54);
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
            Canvas.Controls.Add(numOfPoints);
            Canvas.Controls.Add(map);
            Canvas.Dock = DockStyle.Fill;
            Canvas.Location = new Point(0, 0);
            Canvas.Name = "Canvas";
            Canvas.Size = new Size(1276, 627);
            Canvas.TabIndex = 0;
            Canvas.Paint += Canvas_Paint;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1276, 627);
            Controls.Add(panel2);
            Controls.Add(Canvas);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
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
        private System.Windows.Forms.TextBox LangRange;
        private System.Windows.Forms.ComboBox CurvatureCalcMethod;
        private System.Windows.Forms.TextBox CurveTolerance;
        private TextBox Ref;
        private TextBox Latitude;
        private TextBox Longitude;
        private TextBox numOfPoints;
        private GMap.NET.WindowsForms.GMapControl map;
        private Panel Canvas;
        private Label label5;
        private Label label3;
        private Label label2;
        private Label label7;
        private Label LangRangeLabel;
    }
}

