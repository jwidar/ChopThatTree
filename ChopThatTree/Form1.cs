using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace ChopThatTree
{

	internal class Form1 : Form
	{
		[DllImport("user32.dll")]
		private static extern IntPtr GetForegroundWindow();

		[DllImport("user32.dll")]
		private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);
		[DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
		public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);
		
		//Mouse actions
		private const int MOUSEEVENTF_LEFTDOWN = 0x02;
		private const int MOUSEEVENTF_LEFTUP = 0x04;

		private Timer timer;
		private bool chopThatStuff = false;
		private bool isDown = false;
		private int X;
		private int Y;

		public Form1()
		{
			this.Text = "ChopThatTree";
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			// Note: for the application hook, use the Hook.AppEvents() instead
			this.timer = new Timer
			{
				Interval = 60,
				Enabled = false
			};
			this.timer.Tick += this.Timer_Tick;
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			this.timer.Stop();

			base.OnClosing(e);
		}

		private void Timer_Tick(object sender, EventArgs e)
		{
			Debug.WriteLine(DateTime.Now.ToString());
			if (!this.InGame())
			{
				this.chopThatStuff = false;
				this.timer.Enabled = false;
				this.UpdateBackColor();
			}

			if (this.chopThatStuff)
			{
				if (this.isDown)
				{
					this.LeftButtonUp();
				}
				else
				{
					this.LeftButtonDown();
				}
				this.isDown = !this.isDown;
			}
		}

		internal void MiddleMouseButtonPressed(int x, int y)
		{
			if (!this.InGame())
			{
				return;
			}

			this.X = x;
			this.Y = y;

			this.chopThatStuff = !this.chopThatStuff;
			this.timer.Enabled = this.chopThatStuff;
			this.LeftButtonUp();
			this.isDown = false;

			this.UpdateBackColor();
		}

		private void LeftButtonDown()
		{
			mouse_event(MOUSEEVENTF_LEFTDOWN, (uint)this.X, (uint)this.Y, 0, 0);
		}
		private void LeftButtonUp()
		{
			mouse_event(MOUSEEVENTF_LEFTUP, (uint)this.X, (uint)this.Y, 0, 0);
		}

		private bool InGame()
		{
			//return true;
			return this.GetCurrentWindowTitle() == "Roblox";
		}

		private string GetCurrentWindowTitle()
		{
			var handle = GetForegroundWindow();
			var chars = 256;
			var sb = new StringBuilder(chars);

			if (GetWindowText(handle, sb, chars) > 0)
			{
				Debug.WriteLine(sb.ToString());
				return sb.ToString();
			}
			return "";
		}

		private void UpdateBackColor()
		{
			this.BackColor = this.chopThatStuff ? System.Drawing.Color.Red : System.Drawing.Color.White;
		}
	}
}