using System;
using System.Globalization;
using System.Windows.Forms;

namespace AssemblyInformation
{
	internal static class StartUpClass
	{
		[STAThread]
		public static void Main(string[] args)
		{
			CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;

			Application.SetHighDpiMode(HighDpiMode.SystemAware);
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			string assemblyPath = args != null ? string.Join(" ", args) : null;

			using (MainForm mainForm = new(assemblyPath))
			{
				Application.Run(mainForm);
			}
		}
	}
}
