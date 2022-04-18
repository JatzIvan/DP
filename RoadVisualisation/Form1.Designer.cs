
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
            this.Redraw.Location = new System.Drawing.Point(456, 13);
            this.Redraw.Name = "Redraw";
            this.Redraw.Size = new System.Drawing.Size(75, 23);
            this.Redraw.TabIndex = 1;
            this.Redraw.Text = "Redraw";
            this.Redraw.UseVisualStyleBackColor = true;
            this.Redraw.Click += new System.EventHandler(this.button1_Click);
            // 
            // ToleranceValue
            // 
            this.ToleranceValue.Location = new System.Drawing.Point(215, 13);
            this.ToleranceValue.Name = "ToleranceValue";
            this.ToleranceValue.Size = new System.Drawing.Size(191, 23);
            this.ToleranceValue.TabIndex = 2;
            this.ToleranceValue.Text = "0.05";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.ToleranceValue);
            this.panel2.Controls.Add(this.Redraw);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(800, 48);
            this.panel2.TabIndex = 3;
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
    }
}

