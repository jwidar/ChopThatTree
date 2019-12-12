using Gma.System.MouseKeyHook;
using Loamen.KeyMouseHook;
using System;
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

		private IKeyboardMouseEvents m_GlobalHook;
		private Timer timer;
		private bool chopThatStuff = false;
		private bool isDown = false;
		private InputSimulator inputsim;
		public Form1()
		{
			this.Text = "ChopThatTree";
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			// Note: for the application hook, use the Hook.AppEvents() instead
			this.m_GlobalHook = Hook.GlobalEvents();

			this.m_GlobalHook.MouseDownExt += this.GlobalHookMouseDownExt;

			this.timer = new Timer
			{
				Interval = 60,
				Enabled = true
			};
			this.timer.Tick += this.Timer_Tick;

			this.inputsim = new InputSimulator();
		}

		private void Timer_Tick(object sender, EventArgs e)
		{
			if (!this.InGame())
			{
				this.chopThatStuff = false;
				this.UpdateBackColor();
			}

			if (this.chopThatStuff)
			{
				//this.inputsim.Mouse.LeftButtonClick();
				if (this.isDown)
				{
					this.inputsim.Mouse.LeftButtonUp();
				}
				else
				{
					this.inputsim.Mouse.LeftButtonDown();
				}
				this.isDown = !this.isDown;
			}
		}

		private bool InGame()
		{
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

		private void GlobalHookMouseDownExt(object sender, MouseEventExtArgs e)
		{
			if (!this.InGame())
			{
				return;
			}

			if (e.IsMouseButtonDown && e.Button == MouseButtons.Middle)
			{
				this.chopThatStuff = !this.chopThatStuff;
				this.inputsim.Mouse.LeftButtonUp();

				this.UpdateBackColor();
			}
		}

		private void UpdateBackColor()
		{
			this.BackColor = this.chopThatStuff ? System.Drawing.Color.Red : System.Drawing.Color.White;
		}

		protected override void Dispose(bool disposing)
		{
			this.timer.Stop();

			if (disposing)
			{
				this.m_GlobalHook.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}