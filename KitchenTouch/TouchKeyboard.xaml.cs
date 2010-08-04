/* 
 *  Project     : KitchenTouch::TouchKeyboard
 *  Source      : http://kitchentouch.codeplex.com/
 *  Author      : Ruslan Ulanov
 *  Description : As part of the KitchenTouch project this component 
 *                provides touch-based input for TextBox fields.
 * 
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System.Windows;
using System.Windows.Media;

namespace KitchenTouch
{
	/// <summary>
	/// Interaction logic for TouchKeyboard.xaml
	/// </summary>
	public partial class TouchKeyboard
	{
		private bool bShiftPressed = false;
		private SolidColorBrush colorBtnDefault  = new SolidColorBrush(Color.FromArgb(255, (byte)245, (byte)245, (byte)245));
		private SolidColorBrush colorBtnSelected = new SolidColorBrush(Color.FromArgb(255, (byte)106, (byte)90, (byte)205));
		private string _OriginalValue = "";
		private string _CurrentValue = "";

		public TouchKeyboard()
		{
			InitializeComponent();
			ResultText = "";
		}

		public TouchKeyboard(string txt)
		{
			InitializeComponent();
			_OriginalValue = ResultText = txt;
		}

		public string ResultText
		{
			get { 
				return _CurrentValue; 
			}
			private set {
				this.lblTextEntryField.Text = this._CurrentValue = value; 
			}
		}

		/*
		 public void TouchKeyboard_Close()
		{
			this.Close();
		}
		 */

		private void fnKbdToggleShift()
		{
			try
			{
				this.bShiftPressed = this.bShiftPressed ? false : true;
				this.btnLShift.Background = this.btnRShift.Background = this.bShiftPressed ? this.colorBtnSelected : this.colorBtnDefault;
			}
			catch
			{
			}
		}

		private void fnKbdBtn_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				System.Windows.Controls.Button btn = (System.Windows.Controls.Button)e.OriginalSource;
				//fnDebugWrite("Button [" + btn.Name + "] clicked");
				string txt = ResultText;
				switch (btn.Name)
				{
					case "btnEnter":
						this.Close();
						break;
					case "btnCancel":
						this.ResultText = _OriginalValue;
						this.Close();
						break;
					case "btnBackspace":
						txt = txt.Substring(0, txt.Length - 1);
						this.ResultText = txt;
						break;
					case "btnLShift":
						this.fnKbdToggleShift();
						break;
					case "btnRShift":
						this.fnKbdToggleShift();
						break;
					case "btnTilda":
						txt += this.bShiftPressed ? "~" : "`";
						this.ResultText = txt;
						if (this.bShiftPressed) this.fnKbdToggleShift();
						break;
					case "btn1":
						txt += this.bShiftPressed ? "!" : "1";
						this.ResultText = txt;
						if (this.bShiftPressed) this.fnKbdToggleShift();
						break;
					case "btn2":
						txt += this.bShiftPressed ? "@" : "2";
						this.ResultText = txt;
						if (this.bShiftPressed) this.fnKbdToggleShift();
						break;
					case "btn3":
						txt += this.bShiftPressed ? "#" : "3";
						this.ResultText = txt;
						if (this.bShiftPressed) this.fnKbdToggleShift();
						break;
					case "btn4":
						txt += this.bShiftPressed ? "$" : "4";
						this.ResultText = txt;
						if (this.bShiftPressed) this.fnKbdToggleShift();
						break;
					case "btn5":
						txt += this.bShiftPressed ? "%" : "5";
						this.ResultText = txt;
						if (this.bShiftPressed) this.fnKbdToggleShift();
						break;
					case "btn6":
						txt += this.bShiftPressed ? "^" : "6";
						this.ResultText = txt;
						if (this.bShiftPressed) this.fnKbdToggleShift();
						break;
					case "btn7":
						txt += this.bShiftPressed ? "&" : "7";
						this.ResultText = txt;
						if (this.bShiftPressed) this.fnKbdToggleShift();
						break;
					case "btn8":
						txt += this.bShiftPressed ? "*" : "8";
						this.ResultText = txt;
						if (this.bShiftPressed) this.fnKbdToggleShift();
						break;
					case "btn9":
						txt += this.bShiftPressed ? "(" : "9";
						this.ResultText = txt;
						if (this.bShiftPressed) this.fnKbdToggleShift();
						break;
					case "btn0":
						txt += this.bShiftPressed ? ")" : "0";
						this.ResultText = txt;
						if (this.bShiftPressed) this.fnKbdToggleShift();
						break;
					case "btnMinus":
						txt += this.bShiftPressed ? "_" : "-";
						this.ResultText = txt;
						if (this.bShiftPressed) this.fnKbdToggleShift();
						break;
					case "btnPlus":
						txt += this.bShiftPressed ? "+" : "=";
						this.ResultText = txt;
						if (this.bShiftPressed) this.fnKbdToggleShift();
						break;
					case "btnOBrace":
						txt += this.bShiftPressed ? "{" : "[";
						this.ResultText = txt;
						if (this.bShiftPressed) this.fnKbdToggleShift();
						break;
					case "btnCBrace":
						txt += this.bShiftPressed ? "}" : "]";
						this.ResultText = txt;
						if (this.bShiftPressed) this.fnKbdToggleShift();
						break;
					case "btnPipe":
						txt += "|";
						this.ResultText = txt;
						if (this.bShiftPressed) this.fnKbdToggleShift();
						break;
					case "btnColon":
						txt += this.bShiftPressed ? ":" : ";";
						this.ResultText = txt;
						if (this.bShiftPressed) this.fnKbdToggleShift();
						break;
					case "btnQuote":
						txt += "'";
						this.ResultText = txt;
						if (this.bShiftPressed) this.fnKbdToggleShift();
						break;
					case "btnLess":
						txt += this.bShiftPressed ? "<" : ",";
						this.ResultText = txt;
						if (this.bShiftPressed) this.fnKbdToggleShift();
						break;
					case "btnMore":
						txt += this.bShiftPressed ? ">" : ".";
						this.ResultText = txt;
						if (this.bShiftPressed) this.fnKbdToggleShift();
						break;
					case "btnQuestion":
						txt += this.bShiftPressed ? "?" : "/";
						this.ResultText = txt;
						if (this.bShiftPressed) this.fnKbdToggleShift();
						break;
					case "btnSpace":
						txt += " ";
						this.ResultText = txt;
						if (this.bShiftPressed) this.fnKbdToggleShift();
						break;
					default:
						string str = (string)btn.Content;
						txt += (this.bShiftPressed ? str.ToUpper() : str.ToLower());
						this.ResultText = txt;
						if (this.bShiftPressed) this.fnKbdToggleShift();
						break;
				}
			}
			catch
			{
				//fnDebugWrite("Error in fnKbdBtn_Click: " + ex.Message);
			}
		}
		/*
		private void TK_ContentRendered(object sender, System.EventArgs e)
		{
			int iKbdWidth = 732; 
			int iKbdHeight = 294;
			this.Top = (System.Windows.Forms.SystemInformation.WorkingArea.Height - iKbdHeight);
			//this.Left = (System.Windows.Forms.SystemInformation.WorkingArea.Width - (iKbdWidth / 2 ));
		}
		*/
	}
}
