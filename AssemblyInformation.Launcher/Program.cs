using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Windows.Forms;

namespace AssemblyInformation.Launcher
{
	internal static class Program
	{
		/// <summary>
		///  The main entry point for the application.
		/// </summary>
		[STAThread]
		[SuppressMessage("Design", "CA1031:Do not catch general exception types")]
		private static void Main(string[] args)
		{
			try
			{
				string assemblyPath = args != null ? string.Join(" ", args) : null;

				if (File.Exists(assemblyPath))
				{
					bool is64BitAssembly = Utilities.Is64BitAssembly(assemblyPath);
					Utilities.StartAssemblyInformation(is64BitAssembly, assemblyPath);
				}
				else
				{
					Utilities.StartAssemblyInformation(true, null);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Assembly Information", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}
