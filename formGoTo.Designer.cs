/*
 * Created by SharpDevelop.
 * User: Gary
 * Date: 21/05/2009
 * Time: 12:20 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace OpenBve
{
	partial class formGoTo
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.lblGoTo = new System.Windows.Forms.Label();
			this.numPosition = new System.Windows.Forms.NumericUpDown();
			((System.ComponentModel.ISupportInitialize)(this.numPosition)).BeginInit();
			this.SuspendLayout();
			// 
			// btnOK
			// 
			this.btnOK.Location = new System.Drawing.Point(97, 35);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(75, 23);
			this.btnOK.TabIndex = 3;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.BtnOKClick);
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(12, 35);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 2;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// lblGoTo
			// 
			this.lblGoTo.Location = new System.Drawing.Point(12, 9);
			this.lblGoTo.Name = "lblGoTo";
			this.lblGoTo.Size = new System.Drawing.Size(39, 20);
			this.lblGoTo.TabIndex = 0;
			this.lblGoTo.Text = "Go to:";
			this.lblGoTo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// numPosition
			// 
			this.numPosition.DecimalPlaces = 2;
			this.numPosition.Location = new System.Drawing.Point(57, 9);
			this.numPosition.Maximum = new decimal(new int[] {
									1000000,
									0,
									0,
									0});
			this.numPosition.Name = "numPosition";
			this.numPosition.Size = new System.Drawing.Size(115, 20);
			this.numPosition.TabIndex = 1;
			// 
			// formGoTo
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(187, 70);
			this.Controls.Add(this.numPosition);
			this.Controls.Add(this.lblGoTo);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "formGoTo";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Go To Point";
			((System.ComponentModel.ISupportInitialize)(this.numPosition)).EndInit();
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.NumericUpDown numPosition;
		private System.Windows.Forms.Label lblGoTo;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOK;
	}
}
