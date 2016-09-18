using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TextCAPSToggler
{
	public class WindowsShell
	{
		//Code sourced from: http://www.pinvoke.net/default.aspx/user32/RegisterHotKey.html

		#region fields
		public static int MOD_ALT = 0x1;
		public static int MOD_CONTROL = 0x2;
		public static int MOD_SHIFT = 0x4;
		public static int MOD_WIN = 0x8;
		public static int WM_HOTKEY = 0x312;
		#endregion

		[DllImport("user32.dll")]
		private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);

		[DllImport("user32.dll")]
		private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

		private static int keyId;
		public static void RegisterHotKey(Form f, Keys key)
		{
			int modifiers = 0;

			if ((key & Keys.Alt) == Keys.Alt)
				modifiers = modifiers | WindowsShell.MOD_ALT;

			if ((key & Keys.Control) == Keys.Control)
				modifiers = modifiers | WindowsShell.MOD_CONTROL;

			if ((key & Keys.Shift) == Keys.Shift)
				modifiers = modifiers | WindowsShell.MOD_SHIFT;

			Keys k = key & ~Keys.Control & ~Keys.Shift & ~Keys.Alt;
			keyId = f.GetHashCode(); // this should be a key unique ID, modify this if you want more than one hotkey
			RegisterHotKey((IntPtr)f.Handle, keyId, modifiers, (int)k);
		}

		private delegate void Func();

		public static void UnregisterHotKey(Form f)
		{
			try
			{
				UnregisterHotKey(f.Handle, keyId); // modify this if you want more than one hotkey
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}
		}
	}

	public partial class Form1 : IDisposable
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

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			Keys k = Keys.CapsLock | Keys.Shift;
			WindowsShell.RegisterHotKey(this, k);

			hotkey.RegisterGlobalHotKey((int)Keys.CapsLock, GlobalHotkeys.MOD_SHIFT);
			hotkey.UnregisterGlobalHotKey();
		}

		public GlobalHotkeys hotkey = new GlobalHotkeys();

		protected override void WndProc(ref Message m)
		{
			const int WM_HOTKEY = 0x0312;
			
			switch (m.Msg)
			{
				case WM_HOTKEY:
					{
						MessageBox.Show("KeyCombo");
						break;
					}
				default:
					{
						base.WndProc(ref m);
						break;
					}
			}
		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			WindowsShell.UnregisterHotKey(this);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Text = "Form1";
		}

		#endregion
	}
}

