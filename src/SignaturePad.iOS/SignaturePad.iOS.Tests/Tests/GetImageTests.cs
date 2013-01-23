using System;
using NUnit.Framework;
using Xamarin.Controls;
using MonoTouch.UIKit;
using System.Drawing;

namespace SignaturePadTests {
	[TestFixture]
	public class GetImageTests {
		[Test]
		public void UnboundSignaturePadReturnsNullImage ()
		{
			SignaturePad signature = new SignaturePad ();
			UIImage image = signature.GetImage ();
			Assert.That (image == null);
		}

		[Test]
		public void BoundSignaturePadReturnsImage ()
		{
			SignaturePad signature = new SignaturePad (new RectangleF (0, 0, 50, 100));
			UIImage image = signature.GetImage ();
			Assert.That (image != null);
		}

		[Test]
		public void NegativeScaleReturnsNullImage ()
		{
			SignaturePad signature = new SignaturePad (new RectangleF (0, 0, 50, 100));
			UIImage image = signature.GetImage (-2f);
			Assert.That (image == null);
		}

		[Test]
		public void ZeroScaleReturnsNullImage ()
		{
			SignaturePad signature = new SignaturePad (new RectangleF (0, 0, 50, 100));
			UIImage image = signature.GetImage (0f);
			Assert.That (image == null);
		}

		[Test]
		public void ZeroWidthReturnsNullImage ()
		{
			SignaturePad signature = new SignaturePad (new RectangleF (0, 0, 50, 100));
			UIImage image = signature.GetImage (new SizeF (0, 50));
			Assert.That (image == null);
		}

		[Test]
		public void ZeroHeightReturnsNullImage ()
		{
			SignaturePad signature = new SignaturePad (new RectangleF (0, 0, 50, 100));
			UIImage image = signature.GetImage (new SizeF (25, 0));
			Assert.That (image == null);
		}

		[Test]
		public void NegativeWidthReturnsNullImage ()
		{
			SignaturePad signature = new SignaturePad (new RectangleF (0, 0, 50, 100));
			UIImage image = signature.GetImage (new SizeF (-25, 50));
			Assert.That (image == null);
		}

		[Test]
		public void NegativeHeightReturnsNullImage ()
		{
			SignaturePad signature = new SignaturePad (new RectangleF (0, 0, 50, 100));
			UIImage image = signature.GetImage (new SizeF (25, -50));
			Assert.That (image == null);
		}

		[Test]
		public void PositiveSizeReturnsImage ()
		{
			SignaturePad signature = new SignaturePad (new RectangleF (0, 0, 50, 100));
			UIImage image = signature.GetImage (new SizeF (25, 50));
			Assert.That (image != null);
		}

		[Test]
		public void PositiveScaleReturnsImage ()
		{
			SignaturePad signature = new SignaturePad (new RectangleF (0, 0, 50, 100));
			UIImage image = signature.GetImage (2f);
			Assert.That (image != null);
		}

		[Test]
		public void NullStrokeColorReturnsNullImage ()
		{
			SignaturePad signature = new SignaturePad (new RectangleF (0, 0, 50, 100));
			UIImage image = signature.GetImage (null);
			Assert.That (image == null);
		}

		[Test]
		public void ValidStrokeColorReturnsImage ()
		{
			SignaturePad signature = new SignaturePad (new RectangleF (0, 0, 50, 100));
			UIImage image = signature.GetImage (UIColor.Black);
			Assert.That (image != null);
		}

		[Test]
		public void NullFillColorReturnsNullImage ()
		{
			SignaturePad signature = new SignaturePad (new RectangleF (0, 0, 50, 100));
			UIImage image = signature.GetImage (UIColor.Black, null);
			Assert.That (image == null);
		}

		[Test]
		public void ValidFillColorReturnsImage ()
		{
			SignaturePad signature = new SignaturePad (new RectangleF (0, 0, 50, 100));
			UIImage image = signature.GetImage (UIColor.Black, UIColor.White);
			Assert.That (image != null);
		}
	}
}
