using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AssemblyInformation
{
	internal partial class MainForm : Form
	{
		private readonly ToolTip pictureBoxRecursiveReferencesToolTip;

		private bool disposed;
		private string assemblyPath;
		private AssemblyInformationLoader assemblyInformationLoader;
		private AssemblyInformation assemblyInformation;
		private ErrorDetailsForm errorDetailsForm;
		private HashSet<AssemblyName> recursiveReferences;
		private SortedSet<string> errors;

		public MainForm(string assemblyPath)
		{
			this.assemblyPath = assemblyPath;

			this.InitializeComponent();

			this.Text += Environment.Is64BitProcess ? " (x64)" : " (x86)";

			this.openToolStripMenuItem.Click += this.OpenToolStripMenuItem_Click;
			this.exitToolStripMenuItem.Click += this.ExitToolStripMenuItem_Click;
			this.optionsToolStripMenuItem.Click += this.OptionsToolStripMenuItem_Click;

			using (Bitmap bitmap = SystemIcons.Warning.ToBitmap())
			{
				Size imageSize = this.pictureBoxRecursiveReferences.Size;
				Bitmap resizedBitmap = Utilities.ResizeImage(bitmap, imageSize.Width, imageSize.Height);
				this.pictureBoxRecursiveReferences.Image = resizedBitmap;
			}

			this.pictureBoxRecursiveReferencesToolTip = new ToolTip();
			this.pictureBoxRecursiveReferencesToolTip.SetToolTip(this.pictureBoxRecursiveReferences, "The reference list is incomplete, not all could be found/loaded. Click for details.");

			this.textBoxJITTracking.SelectionLength = 0;
		}

		/// <summary>
		///  Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (this.disposed == false)
			{
				if (disposing)
				{
					this.components?.Dispose();
					this.pictureBoxRecursiveReferencesToolTip?.Dispose();
					this.errorDetailsForm?.Dispose();
				}

				this.disposed = true;

				base.Dispose(disposing);
			}
		}

		private void MainForm_Shown(object sender, EventArgs e)
		{
			this.LoadAssembly();
		}

		private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog openFileDialog = new())
			{
				if (openFileDialog.ShowDialog() == DialogResult.OK && File.Exists(openFileDialog.FileName))
				{
					bool is64BitAssembly = Utilities.Is64BitAssembly(openFileDialog.FileName);

					if (Environment.Is64BitProcess != is64BitAssembly)
					{
						Utilities.StartAssemblyInformation(is64BitAssembly, openFileDialog.FileName);
						Application.Exit();
					}
					else
					{
						this.assemblyPath = openFileDialog.FileName;
						this.LoadAssembly();
					}
				}
			}
		}

		private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void OptionsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.SetAssemblyFullName();
		}

		private void TabControlReferences_Selected(object sender, TabControlEventArgs e)
		{
			if (e.TabPageIndex == 1 && this.recursiveReferences == null)
			{
				SortedSet<string> errors = new();
				this.recursiveReferences = this.GetAssemblyNames(ref errors);

				this.errors = errors;
				this.pictureBoxRecursiveReferences.Visible = this.errors.Count > 0;

				this.SetAssemblyFullName();
			}
		}

		private void TreeViewReferences_BeforeExpand(object sender, TreeViewCancelEventArgs e)
		{
			if (e.Node.Tag is AssemblyName assemblyName)
			{
				try
				{
					this.Cursor = Cursors.WaitCursor;
					this.treeViewReferences.SuspendLayout();

					e.Node.Nodes.Clear();
					this.treeViewReferences.PerformLayout();

					AssemblyName[] assemblyReferences = this.assemblyInformationLoader.GetReferencedAssemblies(assemblyName);
					this.AddAssemblyReferenceNodes(e.Node.Nodes, assemblyReferences);

					e.Node.Tag = null;
				}
				catch (FileNotFoundException ex)
				{
					e.Node.Nodes.Add(ex.Message);
				}
				finally
				{
					this.treeViewReferences.ResumeLayout();
					this.Cursor = Cursors.Default;
				}
			}
		}

		private void PictureBoxRecursiveReferencesToolTip_Click(object sender, EventArgs e)
		{
			if (this.errorDetailsForm == null)
			{
				this.errorDetailsForm = new(this.errors);
			}

			this.errorDetailsForm.Show();
		}

		private void LoadAssembly()
		{
			if (string.IsNullOrWhiteSpace(this.assemblyPath) == false)
			{
				if (File.Exists(this.assemblyPath) == false)
				{
					MessageBox.Show($"The assembly info couldn't be loaded, the path \"{ Path.GetFullPath(this.assemblyPath) }\" doesn't exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
				else
				{
					this.ClearForm();

					this.assemblyInformationLoader = new AssemblyInformationLoader(this.assemblyPath);
					this.assemblyInformation = this.assemblyInformationLoader.GetAssemblyInformation(this.assemblyPath);
					this.SetAssemblyInformation(this.assemblyInformation);

					Task.Run(() =>
					{
						try
						{
							this.InvokeIfRequired(() => this.Cursor = Cursors.WaitCursor);
							this.SuspendLayout();

							this.AddAssemblyReferenceNodes(this.treeViewReferences.Nodes, this.assemblyInformation.ReferencedAssemblies);
						}
						finally
						{
							this.ResumeLayout();
							this.InvokeIfRequired(() => this.Cursor = Cursors.Default);
						}
					});
				}
			}
		}

		private void ClearForm()
		{
			this.textBoxAssemblyKind.Clear();
			this.textBoxEditAndContinue.Clear();
			this.textBoxFullName.Clear();
			this.textBoxJITTracking.Clear();
			this.textBoxOptimized.Clear();
			this.textBoxSequencing.Clear();
			this.textBoxTargetFramework.Clear();
			this.textBoxTargetProcessor.Clear();

			this.treeViewReferences.Nodes.Clear();
			this.listBoxRecursiveReferences.Items.Clear();

			this.Update();
		}

		private void SetAssemblyInformation(AssemblyInformation assemblyInformation)
		{
			this.textBoxJITTracking.Text = assemblyInformation.Debuggable?.IsJITTrackingEnabled == true ? "Debug" : "Release";
			this.textBoxOptimized.Text = assemblyInformation.Debuggable?.IsJITOptimizerDisabled == true ? "Not Optimized" : "Optimized";
			this.textBoxSequencing.Text = assemblyInformation.Debuggable?.DebuggingFlags.HasFlag(DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints) == true ?
				"MSIL Sequencing" :
				"PDB Sequencing";
			this.textBoxEditAndContinue.Text = assemblyInformation.Debuggable?.DebuggingFlags.HasFlag(DebuggableAttribute.DebuggingModes.EnableEditAndContinue) == true ?
				"Edit and Continue Enabled" :
				"Edit and Continue Disabled";

			if (assemblyInformation.TargetFramework != null)
			{
				this.textBoxTargetFramework.Text = string.IsNullOrWhiteSpace(assemblyInformation.TargetFramework.FrameworkDisplayName) == false ?
					assemblyInformation.TargetFramework.FrameworkDisplayName :
					assemblyInformation.TargetFramework.FrameworkName;
			}

			IEnumerable<PortableExecutableKinds> kinds = Enum.GetValues(typeof(PortableExecutableKinds)).Cast<PortableExecutableKinds>()
				.Where(kind => kind != PortableExecutableKinds.NotAPortableExecutableImage && assemblyInformation.PortableExecutableKinds.HasFlag(kind));

			this.textBoxAssemblyKind.Text = string.Join(", ", kinds);

			this.textBoxTargetProcessor.Text = assemblyInformation.ImageFileMachine == ImageFileMachine.I386 && assemblyInformation.PortableExecutableKinds == PortableExecutableKinds.ILOnly ?
				"Any CPU" :
				assemblyInformation.ImageFileMachine.ToString();

			this.textBoxFullName.Text = assemblyInformation.FullName;
		}

		private void AddAssemblyReferenceNodes(TreeNodeCollection treeNodes, IEnumerable<AssemblyName> assemblyReferences)
		{
			foreach (AssemblyName assemblyReference in assemblyReferences.OrderBy(assemblyName => assemblyName.FullName))
			{
				this.InvokeIfRequired(() =>
				{
					TreeNode treeNode = treeNodes.Add(assemblyReference.FullName);
					treeNode.Tag = assemblyReference;

					treeNode.Nodes.Add("<Dummy>");
				});
			}
		}

		private void SetAssemblyFullName()
		{
			if (this.recursiveReferences != null)
			{
				Func<AssemblyName, string> selector = this.optionsToolStripMenuItem.Checked ? assemblyName => assemblyName.FullName : assemblyName => assemblyName.Name;

				string[] range = this.recursiveReferences
						.Select(selector)
						.Distinct()
						.OrderBy(assemblyName => assemblyName)
						.ToArray();

				this.listBoxRecursiveReferences.Items.Clear();
				this.listBoxRecursiveReferences.Items.AddRange(range);
			}
		}

		private HashSet<AssemblyName> GetAssemblyNames(ref SortedSet<string> errors)
		{
			AssemblyNameEqualityComparer comparer = new();
			HashSet<AssemblyName> hashSet = new(comparer);

			this.FillAssemblyNames(hashSet, this.assemblyInformation.ReferencedAssemblies, ref errors);

			return hashSet;
		}

		private void FillAssemblyNames(HashSet<AssemblyName> hashSet, IEnumerable<AssemblyName> assemblyNames, ref SortedSet<string> errors)
		{
			foreach (AssemblyName assemblyName in assemblyNames.OrderBy(assemblyName => assemblyName.FullName))
			{
				if (hashSet.Add(assemblyName))
				{
					AssemblyName[] referencedAssemblies;

					try
					{
						referencedAssemblies = this.assemblyInformationLoader.GetReferencedAssemblies(assemblyName);
					}
					catch (FileNotFoundException)
					{
						errors?.Add(assemblyName.FullName);
						continue;
					}

					this.FillAssemblyNames(hashSet, referencedAssemblies, ref errors);
				}
			}
		}
	}
}
