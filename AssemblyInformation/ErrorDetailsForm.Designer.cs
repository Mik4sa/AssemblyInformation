
namespace AssemblyInformation
{
	partial class ErrorDetailsForm
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ErrorDetailsForm));
			this.labelDescription = new System.Windows.Forms.Label();
			this.listBoxErrors = new System.Windows.Forms.ListBox();
			this.SuspendLayout();
			// 
			// labelDescription
			// 
			this.labelDescription.AutoSize = true;
			this.labelDescription.Location = new System.Drawing.Point(12, 9);
			this.labelDescription.Name = "labelDescription";
			this.labelDescription.Size = new System.Drawing.Size(382, 15);
			this.labelDescription.TabIndex = 1;
			this.labelDescription.Text = "The references for the following assembly names couldn\'t be loaded:";
			// 
			// listBoxErrors
			// 
			this.listBoxErrors.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listBoxErrors.ItemHeight = 15;
			this.listBoxErrors.Location = new System.Drawing.Point(12, 25);
			this.listBoxErrors.Name = "listBoxErrors";
			this.listBoxErrors.Size = new System.Drawing.Size(560, 124);
			this.listBoxErrors.TabIndex = 2;
			this.listBoxErrors.TabStop = false;
			// 
			// ErrorDetailsForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(584, 161);
			this.Controls.Add(this.listBoxErrors);
			this.Controls.Add(this.labelDescription);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "ErrorDetailsForm";
			this.ShowIcon = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Error Details";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.Label labelDescription;
		private System.Windows.Forms.ListBox listBoxErrors;
	}
}