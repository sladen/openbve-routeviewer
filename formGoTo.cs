/*
 * Created by SharpDevelop.
 * User: Gary
 * Date: 21/05/2009
 * Time: 12:20 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Windows.Forms;

namespace OpenBve
{
	/// <summary>
	/// Description of formGoTo.
	/// </summary>
	public partial class formGoTo : Form
	{
		public double Position {
			get { return Convert.ToDouble(numPosition.Value); }
		}
		
		
		public formGoTo(double posCurrent, double posMin, double posMax)
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			numPosition.Value = Convert.ToDecimal(posCurrent);
			numPosition.Minimum = Convert.ToDecimal(posMin);
			numPosition.Maximum = Convert.ToDecimal(posMax);
			numPosition.Select(0, 10);
		}
		
		
		void BtnOKClick(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
		}
	}
}
