
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
            this.Canvas = new System.Windows.Forms.Panel();
            this.Redraw = new System.Windows.Forms.Button();
            this.ToleranceValue = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.CurveTolerance = new System.Windows.Forms.TextBox();
            this.CurvatureCalcMethod = new System.Windows.Forms.ComboBox();
            this.LangRangeLabel = new System.Windows.Forms.Label();
            this.LangRange = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SimplificationMethod = new System.Windows.Forms.ComboBox();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // Canvas
            // 
            this.Canvas.BackColor = System.Drawing.Color.White;
            this.Canvas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Canvas.Location = new System.Drawing.Point(0, 0);
            this.Canvas.Name = "Canvas";
            this.Canvas.Size = new System.Drawing.Size(800, 446);
            this.Canvas.TabIndex = 0;
            this.Canvas.Paint += new System.Windows.Forms.PaintEventHandler(this.Canvas_Paint);
            // 
            // Redraw
            // 
            this.Redraw.Location = new System.Drawing.Point(713, 13);
            this.Redraw.Name = "Redraw";
            this.Redraw.Size = new System.Drawing.Size(75, 23);
            this.Redraw.TabIndex = 1;
            this.Redraw.Text = "Redraw";
            this.Redraw.UseVisualStyleBackColor = true;
            this.Redraw.Click += new System.EventHandler(this.button1_Click);
            // 
            // ToleranceValue
            // 
            this.ToleranceValue.Location = new System.Drawing.Point(70, 13);
            this.ToleranceValue.Name = "ToleranceValue";
            this.ToleranceValue.Size = new System.Drawing.Size(94, 23);
            this.ToleranceValue.TabIndex = 2;
            this.ToleranceValue.Text = "0.05";
            this.ToleranceValue.TextChanged += new System.EventHandler(this.ToleranceValue_TextChanged);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.CurveTolerance);
            this.panel2.Controls.Add(this.CurvatureCalcMethod);
            this.panel2.Controls.Add(this.LangRangeLabel);
            this.panel2.Controls.Add(this.LangRange);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.SimplificationMethod);
            this.panel2.Controls.Add(this.ToleranceValue);
            this.panel2.Controls.Add(this.Redraw);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(800, 48);
            this.panel2.TabIndex = 3;
            // 
            // CurveTolerance
            // 
            this.CurveTolerance.Location = new System.Drawing.Point(188, 13);
            this.CurveTolerance.Name = "CurveTolerance";
            this.CurveTolerance.Size = new System.Drawing.Size(94, 23);
            this.CurveTolerance.TabIndex = 8;
            this.CurveTolerance.Text = "2";
            this.CurveTolerance.TextChanged += new System.EventHandler(this.CurveTolerance_TextChanged);
            // 
            // CurvatureCalcMethod
            // 
            this.CurvatureCalcMethod.FormattingEnabled = true;
            this.CurvatureCalcMethod.Location = new System.Drawing.Point(505, 13);
            this.CurvatureCalcMethod.Name = "CurvatureCalcMethod";
            this.CurvatureCalcMethod.Size = new System.Drawing.Size(100, 23);
            this.CurvatureCalcMethod.TabIndex = 7;
            // 
            // LangRangeLabel
            // 
            this.LangRangeLabel.AutoSize = true;
            this.LangRangeLabel.Location = new System.Drawing.Point(288, 15);
            this.LangRangeLabel.Name = "LangRangeLabel";
            this.LangRangeLabel.Size = new System.Drawing.Size(40, 15);
            this.LangRangeLabel.TabIndex = 6;
            this.LangRangeLabel.Text = "Range";
            this.LangRangeLabel.Visible = false;
            this.LangRangeLabel.Click += new System.EventHandler(this.label2_Click);
            // 
            // LangRange
            // 
            this.LangRange.Location = new System.Drawing.Point(339, 12);
            this.LangRange.Name = "LangRange";
            this.LangRange.Size = new System.Drawing.Size(135, 23);
            this.LangRange.TabIndex = 5;
            this.LangRange.Text = "4";
            this.LangRange.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 15);
            this.label1.TabIndex = 4;
            this.label1.Text = "Tolerance";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // SimplificationMethod
            // 
            this.SimplificationMethod.FormattingEnabled = true;
            this.SimplificationMethod.Location = new System.Drawing.Point(611, 13);
            this.SimplificationMethod.Name = "SimplificationMethod";
            this.SimplificationMethod.Size = new System.Drawing.Size(96, 23);
            this.SimplificationMethod.TabIndex = 3;
            this.SimplificationMethod.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 446);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.Canvas);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel Canvas;
        private System.Windows.Forms.Button Redraw;
        private System.Windows.Forms.TextBox ToleranceValue;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ComboBox SimplificationMethod;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label LangRangeLabel;
        private System.Windows.Forms.TextBox LangRange;
        private System.Windows.Forms.ComboBox CurvatureCalcMethod;
        private System.Windows.Forms.TextBox CurveTolerance;
    }
}

