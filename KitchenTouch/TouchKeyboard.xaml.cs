using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KitchenTouch
{
	/// <summary>
	/// Interaction logic for TouchKeyboard.xaml
	/// </summary>
	public partial class TouchKeyboard : UserControl
	{
		private bool bShiftPressed = false;
		private SolidColorBrush colorBtnSelected = new SolidColorBrush(Color.FromArgb(255, (byte)106, (byte)90, (byte)205));
		private SolidColorBrush colorBtnDefault = new SolidColorBrush(Color.FromArgb(255, (byte)245, (byte)245, (byte)245));
		private string txtCurrentValue = "";

		public TouchKeyboard()
		{
			InitializeComponent();
			this.TouchKeyboard_SetText("");
		}

		public TouchKeyboard(string txt)
		{
			InitializeComponent();
			this.TouchKeyboard_SetText(txt);
		}
		
		public void TouchKeyboard_SetText(string txt) 
		{
			this.txtCurrentValue = txt;
			this.lblTextEntryField.Text = this.txtCurrentValue;
		}

		public string TouchKeyboard_GetText() 
		{
			return this.txtCurrentValue;
		}

		public void TouchKeyboard_Show()
		{
			this.Visibility = Visibility.Visible;
		}
		
		public void TouchKeyboard_Hide()
		{
			this.Visibility = Visibility.Collapsed;
		}

		public void TouchKeyboard_Destroy()
		{
			//this.Close();
		}
		
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
				switch (btn.Name)
				{
					case "btnEnter":
						this.TouchKeyboard_SetText(lblTextEntryField.Text);
						this.TouchKeyboard_Hide();
						break;
					case "btnCancel":
						this.TouchKeyboard_Hide();
						break;
					case "btnBackspace":
						this.lblTextEntryField.Text = this.lblTextEntryField.Text.Substring(0, this.lblTextEntryField.Text.Length - 1);
						break;
					case "btnLShift":
						this.fnKbdToggleShift();
						break;
					case "btnRShift":
						this.fnKbdToggleShift();
						break;
					case "btnTilda":
						this.lblTextEntryField.Text += this.bShiftPressed ? "~" : "`";
						if (this.bShiftPressed) this.fnKbdToggleShift();
						break;
					case "btn1":
						this.lblTextEntryField.Text += this.bShiftPressed ? "!" : "1";
						if (this.bShiftPressed) this.fnKbdToggleShift();
						break;
					case "btn2":
						this.lblTextEntryField.Text += this.bShiftPressed ? "@" : "2";
						if (this.bShiftPressed) this.fnKbdToggleShift();
						break;
					case "btn3":
						this.lblTextEntryField.Text += this.bShiftPressed ? "#" : "3";
						if (this.bShiftPressed) this.fnKbdToggleShift();
						break;
					case "btn4":
						this.lblTextEntryField.Text += this.bShiftPressed ? "$" : "4";
						if (this.bShiftPressed) this.fnKbdToggleShift();
						break;
					case "btn5":
						this.lblTextEntryField.Text += this.bShiftPressed ? "%" : "5";
						if (this.bShiftPressed) this.fnKbdToggleShift();
						break;
					case "btn6":
						this.lblTextEntryField.Text += this.bShiftPressed ? "^" : "6";
						if (this.bShiftPressed) this.fnKbdToggleShift();
						break;
					case "btn7":
						this.lblTextEntryField.Text += this.bShiftPressed ? "&" : "7";
						if (this.bShiftPressed) this.fnKbdToggleShift();
						break;
					case "btn8":
						this.lblTextEntryField.Text += this.bShiftPressed ? "*" : "8";
						if (this.bShiftPressed) this.fnKbdToggleShift();
						break;
					case "btn9":
						this.lblTextEntryField.Text += this.bShiftPressed ? "(" : "9";
						if (this.bShiftPressed) this.fnKbdToggleShift();
						break;
					case "btn0":
						this.lblTextEntryField.Text += this.bShiftPressed ? ")" : "0";
						if (this.bShiftPressed) this.fnKbdToggleShift();
						break;
					case "btnMinus":
						this.lblTextEntryField.Text += this.bShiftPressed ? "_" : "-";
						if (this.bShiftPressed) this.fnKbdToggleShift();
						break;
					case "btnPlus":
						this.lblTextEntryField.Text += this.bShiftPressed ? "+" : "=";
						if (this.bShiftPressed) this.fnKbdToggleShift();
						break;
					case "btnOBrace":
						this.lblTextEntryField.Text += this.bShiftPressed ? "{" : "[";
						if (this.bShiftPressed) this.fnKbdToggleShift();
						break;
					case "btnCBrace":
						this.lblTextEntryField.Text += this.bShiftPressed ? "}" : "]";
						if (this.bShiftPressed) this.fnKbdToggleShift();
						break;
					case "btnPipe":
						this.lblTextEntryField.Text += "|";
						if (this.bShiftPressed) this.fnKbdToggleShift();
						break;
					case "btnColon":
						this.lblTextEntryField.Text += this.bShiftPressed ? ":" : ";";
						if (this.bShiftPressed) this.fnKbdToggleShift();
						break;
					case "btnQuote":
						this.lblTextEntryField.Text += "'";
						if (this.bShiftPressed) this.fnKbdToggleShift();
						break;
					case "btnLess":
						this.lblTextEntryField.Text += this.bShiftPressed ? "<" : ",";
						if (this.bShiftPressed) this.fnKbdToggleShift();
						break;
					case "btnMore":
						this.lblTextEntryField.Text += this.bShiftPressed ? ">" : ".";
						if (this.bShiftPressed) this.fnKbdToggleShift();
						break;
					case "btnQuestion":
						this.lblTextEntryField.Text += this.bShiftPressed ? "?" : "/";
						if (this.bShiftPressed) this.fnKbdToggleShift();
						break;
					case "btnSpace":
						this.lblTextEntryField.Text += " ";
						if (this.bShiftPressed) this.fnKbdToggleShift();
						break;
					default:
						string str = (string)btn.Content;
						this.lblTextEntryField.Text += (this.bShiftPressed ? str.ToUpper() : str.ToLower());
						if (this.bShiftPressed) this.fnKbdToggleShift();
						break;
				}
			}
			catch
			{
				//fnDebugWrite("Error in fnKbdBtn_Click: " + ex.Message);
			}
		}
	}
}
