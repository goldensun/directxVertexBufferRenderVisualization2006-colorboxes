using System;
using System.Windows.Forms;

namespace Game
{
	internal static class Program
	{
		[STAThread]
		private static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Form1 form = new Form1();
			form.Show();
			form.GameLoop();
		}
	}
}
