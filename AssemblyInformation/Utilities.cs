using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace AssemblyInformation
{
	internal static class Utilities
	{
		public static bool Is64BitAssembly(string assemblyPath)
		{
			AssemblyName assemblyName = AssemblyName.GetAssemblyName(assemblyPath);
			return assemblyName.ProcessorArchitecture == ProcessorArchitecture.Amd64;
		}

		public static void StartAssemblyInformation(bool use64Bit, string assemblyPath)
		{
			string executingAssemblyPath = Assembly.GetExecutingAssembly().Location;
			string executingAssemblyDirectory = Path.GetDirectoryName(executingAssemblyPath);
			string processorArchitectureDependentAssemblyPath = Path.Combine(executingAssemblyDirectory, $"AssemblyInformation.{ (use64Bit ? "x64" : "x86") }.exe");

			if (File.Exists(processorArchitectureDependentAssemblyPath) == false)
			{
				MessageBox.Show($"Executable \"{ Path.GetFullPath(processorArchitectureDependentAssemblyPath) }\" not found!", "AssemblyInformation", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			else
			{
				Process.Start(processorArchitectureDependentAssemblyPath, assemblyPath);
			}
		}

		/// <summary>
		/// Resize the image to the specified width and height.
		/// https://stackoverflow.com/a/24199315
		/// </summary>
		/// <param name="image">The image to resize.</param>
		/// <param name="width">The width to resize to.</param>
		/// <param name="height">The height to resize to.</param>
		/// <returns>The resized image.</returns>
		internal static Bitmap ResizeImage(Image image, int width, int height)
		{
			Rectangle destRect = new(0, 0, width, height);
			Bitmap destImage = new(width, height);

			destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

			using (Graphics graphics = Graphics.FromImage(destImage))
			{
				graphics.CompositingMode = CompositingMode.SourceCopy;
				graphics.CompositingQuality = CompositingQuality.HighQuality;
				graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
				graphics.SmoothingMode = SmoothingMode.HighQuality;
				graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

				using (ImageAttributes wrapMode = new())
				{
					wrapMode.SetWrapMode(WrapMode.TileFlipXY);
					graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
				}
			}

			return destImage;
		}
	}
}
