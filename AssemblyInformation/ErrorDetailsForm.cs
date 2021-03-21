using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace AssemblyInformation
{
	internal partial class ErrorDetailsForm : Form
	{
		public ErrorDetailsForm(SortedSet<string> errors)
		{
			this.InitializeComponent();

			this.listBoxErrors.Items.AddRange(errors.ToArray());
		}
	}
}
