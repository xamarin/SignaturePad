using System;
using System.Drawing;
using UIKit;
using NUnit.Framework;
using SignaturePad;

namespace SignaturePadTests {
	[TestFixture]
	public class GetImageTests {
		[Test]
		public void UnboundSignaturePadViewReturnsNullImage ()
		{
			SignaturePadView signature = new SignaturePadView ();
			UIImage image = signature.GetImage ();
			Assert.That (image == null);
		}

		[Test]
		public void BoundSignaturePadViewReturnsImage ()
		{
			SignaturePadView signature = new SignaturePadView (new RectangleF (0, 0, 50, 100));
			UIImage image = signature.GetImage ();
			Assert.That (image != null);
		}

		[Test]
		public void NegativeScaleReturnsNullImage ()
		{
			SignaturePadView signature = new SignaturePadView (new RectangleF (0, 0, 50, 100));
			UIImage image = signature.GetImage (-2f);
			Assert.That (image == null);
		}

		[Test]
		public void ZeroScaleReturnsNullImage ()
		{
			SignaturePadView signature = new SignaturePadView (new RectangleF (0, 0, 50, 100));
			UIImage image = signature.GetImage (0f);
			Assert.That (image == null);
		}

		[Test]
		public void ZeroWidthReturnsNullImage ()
		{
			SignaturePadView signature = new SignaturePadView (new RectangleF (0, 0, 50, 100));
			UIImage image = signature.GetImage (new SizeF (0, 50));
			Assert.That (image == null);
		}

		[Test]
		public void ZeroHeightReturnsNullImage ()
		{
			SignaturePadView signature = new SignaturePadView (new RectangleF (0, 0, 50, 100));
			UIImage image = signature.GetImage (new SizeF (25, 0));
			Assert.That (image == null);
		}

		[Test]
		public void NegativeWidthReturnsNullImage ()
		{
			SignaturePadView signature = new SignaturePadView (new RectangleF (0, 0, 50, 100));
			UIImage image = signature.GetImage (new SizeF (-25, 50));
			Assert.That (image == null);
		}

		[Test]
		public void NegativeHeightReturnsNullImage ()
		{
			SignaturePadView signature = new SignaturePadView (new RectangleF (0, 0, 50, 100));
			UIImage image = signature.GetImage (new SizeF (25, -50));
			Assert.That (image == null);
		}

		[Test]
		public void PositiveSizeReturnsImage ()
		{
			SignaturePadView signature = new SignaturePadView (new RectangleF (0, 0, 50, 100));
			UIImage image = signature.GetImage (new SizeF (25, 50));
			Assert.That (image != null);
		}

		[Test]
		public void PositiveScaleReturnsImage ()
		{
			SignaturePadView signature = new SignaturePadView (new RectangleF (0, 0, 50, 100));
			UIImage image = signature.GetImage (2f);
			Assert.That (image != null);
		}

		[Test]
		public void NullStrokeColorReturnsNullImage ()
		{
			SignaturePadView signature = new SignaturePadView (new RectangleF (0, 0, 50, 100));
			UIImage image = signature.GetImage (null);
			Assert.That (image == null);
		}

		[Test]
		public void ValidStrokeColorReturnsImage ()
		{
			SignaturePadView signature = new SignaturePadView (new RectangleF (0, 0, 50, 100));
			UIImage image = signature.GetImage (UIColor.Black);
			Assert.That (image != null);
		}

		[Test]
		public void NullFillColorReturnsNullImage ()
		{
			SignaturePadView signature = new SignaturePadView (new RectangleF (0, 0, 50, 100));
			UIImage image = signature.GetImage (UIColor.Black, null);
			Assert.That (image == null);
		}

		[Test]
		public void ValidFillColorReturnsImage ()
		{
			SignaturePadView signature = new SignaturePadView (new RectangleF (0, 0, 50, 100));
			UIImage image = signature.GetImage (UIColor.Black, UIColor.White);
			Assert.That (image != null);
		}
	}
}
