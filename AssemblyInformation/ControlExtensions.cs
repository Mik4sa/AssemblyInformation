using System;
using System.Windows.Forms;

namespace AssemblyInformation
{
	internal static class ControlExtensions
	{
		public static void InvokeIfRequired(this Control control, Action action)
		{
			if (control.InvokeRequired)
			{
				control.Invoke(action);
			}
			else
			{
				action();
			}
		}
	}
}
