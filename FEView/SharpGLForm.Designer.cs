namespace FEView
{
	partial class SharpGLForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.openGLControl = new SharpGL.OpenGLControl();
			this._btnCalculate = new System.Windows.Forms.Button();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			((System.ComponentModel.ISupportInitialize)(this.openGLControl)).BeginInit();
			this.tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// openGLControl
			// 
			this.openGLControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.openGLControl.DrawFPS = true;
			this.openGLControl.Location = new System.Drawing.Point(3, 3);
			this.openGLControl.Name = "openGLControl";
			this.openGLControl.OpenGLVersion = SharpGL.Version.OpenGLVersion.OpenGL2_1;
			this.openGLControl.RenderContextType = SharpGL.RenderContextType.FBO;
			this.openGLControl.RenderTrigger = SharpGL.RenderTrigger.TimerBased;
			this.openGLControl.Size = new System.Drawing.Size(734, 471);
			this.openGLControl.TabIndex = 0;
			this.openGLControl.OpenGLInitialized += new System.EventHandler(this.OpenGlControlOpenGlInitialized);
			this.openGLControl.OpenGLDraw += new SharpGL.RenderEventHandler(this.OpenGlControlOpenGlDraw);
			this.openGLControl.Resized += new System.EventHandler(this.openGLControl_Resized);
			this.openGLControl.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.OpenGlControlKeyPress);
			// 
			// _btnCalculate
			// 
			this._btnCalculate.Location = new System.Drawing.Point(743, 3);
			this._btnCalculate.Name = "_btnCalculate";
			this._btnCalculate.Size = new System.Drawing.Size(61, 23);
			this._btnCalculate.TabIndex = 1;
			this._btnCalculate.Text = "Calculate";
			this._btnCalculate.UseVisualStyleBackColor = true;
			this._btnCalculate.Click += new System.EventHandler(this.BtnCalculateClick);
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 67F));
			this.tableLayoutPanel1.Controls.Add(this.openGLControl, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this._btnCalculate, 1, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 1;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(807, 477);
			this.tableLayoutPanel1.TabIndex = 2;
			// 
			// SharpGLForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(807, 477);
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "SharpGLForm";
			this.Text = "SharpGL Form";
			this.Load += new System.EventHandler(this.SharpGLForm_Load);
			((System.ComponentModel.ISupportInitialize)(this.openGLControl)).EndInit();
			this.tableLayoutPanel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private SharpGL.OpenGLControl openGLControl;
		private System.Windows.Forms.Button _btnCalculate;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
	}
}

