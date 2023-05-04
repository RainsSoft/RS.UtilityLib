using System;
using System.Drawing;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Represents a cached element image.
	/// </summary>
	public sealed class BetterListViewCachedImage : IDisposable
	{
		private readonly Image image;

		private readonly Size imageSize;

		private readonly byte opacity;

		private readonly ImageBorderType borderType;

		private readonly int borderThickness;

		private readonly bool enabled;

		/// <summary>
		///   cached image
		/// </summary>
		public Image Image => this.image;

		/// <summary>
		///   displayed image size
		/// </summary>
		public Size ImageSize => this.imageSize;

		/// <summary>
		///   displayed image opacity
		/// </summary>
		public byte Opacity => this.opacity;

		/// <summary>
		///   displayed image border type
		/// </summary>
		public ImageBorderType BorderType => this.borderType;

		/// <summary>
		///   displayed image border thickness
		/// </summary>
		public int BorderThickness => this.borderThickness;

		/// <summary>
		///   displayed image enabled state
		/// </summary>
		public bool Enabled => this.enabled;

		public BetterListViewCachedImage(Image image, Size imageSize, byte opacity, ImageBorderType borderType, int borderThickness, bool enabled) {
			Checks.CheckNotNull(image, "image");
			Checks.CheckSize(imageSize, "imageSize");
			Checks.CheckNotEqual(borderType, ImageBorderType.Undefined, "borderType", "ImageBorderType.Undefined");
			Checks.CheckBounds(borderThickness, 0, 16, "borderThickness");
			this.image = image;
			this.imageSize = imageSize;
			this.opacity = opacity;
			this.borderType = borderType;
			this.borderThickness = borderThickness;
			this.enabled = enabled;
		}

		/// <summary>
		///   Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose() {
			this.image.Dispose();
		}
	}
}