using System;
using System.Drawing;

namespace ComponentOwl.BetterListView {

	/// <summary>
	///   Represents image size limits.
	/// </summary>
	public struct BetterListViewImageSize
	{
		/// <summary>
		///   Represents an empty BetterListViewImageSize structure.
		/// </summary>
		public static readonly BetterListViewImageSize Empty = new BetterListViewImageSize(Size.Empty, Size.Empty);

		private Size minimumSize;

		private Size maximumSize;

		/// <summary>
		///   Minimum allowed image size.
		/// </summary>
		public Size MinimumSize => this.minimumSize;

		/// <summary>
		///   Maximum allowed image size.
		/// </summary>
		public Size MaximumSize => this.maximumSize;

		/// <summary>
		///   Minimum and maximum image sizes are the same.
		/// </summary>
		public bool IsFixed => this.minimumSize == this.maximumSize;

		/// <summary>
		///   This BetterListViewImageSize structure is empty.
		/// </summary>
		public bool IsEmpty => this.Equals(BetterListViewImageSize.Empty);

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewImageSize" /> struct.
		/// </summary>
		/// <param name="size">Image dimension (both horizontal and vertical).</param>
		public BetterListViewImageSize(int size) {
			Checks.CheckTrue(size >= 0, "size >= 0");
			this.minimumSize = new Size(size, size);
			this.maximumSize = new Size(size, size);
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewImageSize" /> struct.
		/// </summary>
		/// <param name="minimumSize">Minimum image dimension (both horizontal and vertical).</param>
		/// <param name="maximumSize">Maximum image dimension (both horizontal and vertical).</param>
		public BetterListViewImageSize(int minimumSize, int maximumSize) {
			Checks.CheckTrue(minimumSize >= 0, "minimumSize >= 0");
			Checks.CheckTrue(maximumSize >= minimumSize, "maximumSize >= minimumSize");
			this.minimumSize = new Size(minimumSize, minimumSize);
			this.maximumSize = new Size(maximumSize, maximumSize);
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewImageSize" /> struct.
		/// </summary>
		/// <param name="size">Image size (both minimum and maximum).</param>
		public BetterListViewImageSize(Size size) {
			Checks.CheckSize(size, "size");
			this.minimumSize = size;
			this.maximumSize = size;
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewImageSize" /> struct.
		/// </summary>
		/// <param name="minimumSize">Mimimum allowed image size.</param>
		/// <param name="maximumSize">Maximum allowed image size.</param>
		public BetterListViewImageSize(Size minimumSize, Size maximumSize) {
			Checks.CheckSize(minimumSize, "minimumSize");
			Checks.CheckTrue(maximumSize.Width >= minimumSize.Width && maximumSize.Height >= minimumSize.Height, "maximumSize.Width >= minimumSize.Width && maximumSize.Height >= minimumSize.Height");
			this.minimumSize = minimumSize;
			this.maximumSize = maximumSize;
		}

		/// <summary>
		///   Test wheter the two BetterListViewImageSize objects are identical.
		/// </summary>
		/// <param name="imageSizeA">first BetterListViewImageSize object</param>
		/// <param name="imageSizeB">second BetterListViewImageSize object</param>
		/// <returns>
		///   the two BetterListViewImageSize objects are identical
		/// </returns>
		public static bool operator ==(BetterListViewImageSize imageSizeA, BetterListViewImageSize imageSizeB) {
			return imageSizeA.Equals(imageSizeB);
		}

		/// <summary>
		///   Test wheter the two BetterListViewImageSize objects are different.
		/// </summary>
		/// <param name="imageSizeA">first BetterListViewImageSize object</param>
		/// <param name="imageSizeB">second BetterListViewImageSize object</param>
		/// <returns>
		///   the two BetterListViewImageSize objects are identical
		/// </returns>
		public static bool operator !=(BetterListViewImageSize imageSizeA, BetterListViewImageSize imageSizeB) {
			return !imageSizeA.Equals(imageSizeB);
		}

		/// <summary>
		///   Get sizes of image frame and image for the given image.
		/// </summary>
		/// <param name="image">Image to obtain sizes for.</param>
		/// <param name="sizeImageFrame">Resulting image frame size.</param>
		public void GetImageSize(Image image, out Size sizeImageFrame) {
			this.GetImageSize(image, out sizeImageFrame, out var _);
		}

		/// <summary>
		///   Get sizes of image frame and image for the given image.
		/// </summary>
		/// <param name="image">Image to obtain sizes for.</param>
		/// <param name="sizeImageFrame">Resulting image frame size.</param>
		/// <param name="sizeImage">Resulting image size.</param>
		public void GetImageSize(Image image, out Size sizeImageFrame, out Size sizeImage) {
			if (this.IsEmpty || image == null) {
				sizeImageFrame = this.minimumSize;
				sizeImage = Size.Empty;
				return;
			}
			sizeImageFrame = new Size(Math.Min(Math.Max(image.Width, this.minimumSize.Width), this.maximumSize.Width), Math.Min(Math.Max(image.Height, this.minimumSize.Height), this.maximumSize.Height));
			if (sizeImageFrame.Width > 0 && sizeImageFrame.Height > 0) {
				sizeImage = Painter.ToIsotropicSize(image.Size, sizeImageFrame, upscale: false, fromOutside: false);
				sizeImageFrame = new Size(Math.Max(Math.Min(sizeImageFrame.Width, sizeImage.Width), this.minimumSize.Width), Math.Max(Math.Min(sizeImageFrame.Height, sizeImage.Height), this.minimumSize.Height));
			}
			else {
				sizeImage = Size.Empty;
				sizeImageFrame = new Size(Math.Min(sizeImageFrame.Width, this.minimumSize.Width), Math.Min(sizeImageFrame.Height, this.minimumSize.Height));
			}
		}

		/// <summary>
		///   Get size of image frame and relative image boundaries within the frame.
		/// </summary>
		/// <param name="image">Image for which to get boundaries.</param>
		/// <param name="alignmentHorizontal">Horizontal image alignment.</param>
		/// <param name="alignmentVertical">Vertical image alignment.</param>
		/// <param name="sizeImageFrame">Resulting image frame size.</param>
		/// <param name="boundsImage">Resulting image boundaries.</param>
		public void GetImageBounds(Image image, BetterListViewImageAlignmentHorizontal alignmentHorizontal, BetterListViewImageAlignmentVertical alignmentVertical, out Size sizeImageFrame, out Rectangle boundsImage) {
			this.GetImageSize(image, out sizeImageFrame, out var sizeImage);
			if (sizeImageFrame.Width == 0 || sizeImageFrame.Height == 0) {
				boundsImage = Rectangle.Empty;
			}
			else {
				boundsImage = BetterListViewImageSize.GetImageBounds(new Rectangle(0, 0, sizeImageFrame.Width, sizeImageFrame.Height), sizeImage, alignmentHorizontal, alignmentVertical);
			}
		}

		/// <summary>
		///   Determines whether the specified <see cref="T:System.Object" /> is equal to this instance.
		/// </summary>
		/// <param name="obj">The <see cref="T:System.Object" /> to compare with this instance.</param>
		/// <returns>
		///   <c>true</c> if the specified <see cref="T:System.Object" /> is equal to this instance; otherwise, <c>false</c>.
		/// </returns>
		public override bool Equals(object obj) {
			if (!(obj is BetterListViewImageSize betterListViewImageSize)) {
				return false;
			}
			if (this.minimumSize == betterListViewImageSize.minimumSize) {
				return this.maximumSize == betterListViewImageSize.maximumSize;
			}
			return false;
		}

		/// <summary>
		///   Returns a hash code for this instance.
		/// </summary>
		/// <returns>
		///   A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
		/// </returns>
		public override int GetHashCode() {
			return this.minimumSize.GetHashCode() ^ this.maximumSize.GetHashCode();
		}

		/// <summary>
		///   Get image boundaries for the specified image frame size and image size.
		/// </summary>
		/// <param name="alignmentHorizontal">Horizontal image alignment.</param>
		/// <param name="alignmentVertical">Vertical image alignment.</param>
		/// <param name="imageFrameBounds">Image frame boundaries.</param>
		/// <param name="imageSize">Image size.</param>
		internal static Rectangle GetImageBounds(Rectangle imageFrameBounds, Size imageSize, BetterListViewImageAlignmentHorizontal alignmentHorizontal, BetterListViewImageAlignmentVertical alignmentVertical) {
			if (imageFrameBounds.Width == 0 || imageFrameBounds.Height == 0 || imageSize.Width == 0 || imageSize.Height == 0) {
				return Rectangle.Empty;
			}
			if (imageSize == imageFrameBounds.Size) {
				return imageFrameBounds;
			}
			Size size = Painter.ToIsotropicSize(imageSize, imageFrameBounds.Size, upscale: false, fromOutside: false);
			int num;
			switch (alignmentHorizontal) {
				case BetterListViewImageAlignmentHorizontal.BeforeTextLeft:
				case BetterListViewImageAlignmentHorizontal.AfterTextLeft:
					num = 0;
					break;
				case BetterListViewImageAlignmentHorizontal.BeforeTextRight:
				case BetterListViewImageAlignmentHorizontal.AfterTextRight:
					num = imageFrameBounds.Width - size.Width;
					break;
				default:
					num = imageFrameBounds.Width - size.Width >> 1;
					break;
			}
			//int num2 = alignmentVertical switch {
			//	BetterListViewImageAlignmentVertical.Top => 0,
			//	BetterListViewImageAlignmentVertical.Bottom => imageFrameBounds.Height - size.Height,
			//	_ => imageFrameBounds.Height - size.Height >> 1,
			//};
			int num2 = 0;
			switch (alignmentVertical) {
				case BetterListViewImageAlignmentVertical.Top:
					num2 = 0;
					break;
				case BetterListViewImageAlignmentVertical.Bottom:
					num2 = imageFrameBounds.Height - size.Height;
					break;
				default:
					num2=imageFrameBounds.Height - size.Height >> 1;
					break;
			}

			return new Rectangle(imageFrameBounds.Left + num, imageFrameBounds.Top + num2, size.Width, size.Height);
		}
	}
}