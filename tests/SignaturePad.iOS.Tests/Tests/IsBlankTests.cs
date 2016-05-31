using System;
using NUnit.Framework;
using System.Drawing;

using SignaturePad;

#if __UNIFIED__
using CoreGraphics;
#else
using MonoTouch.CoreGraphics;
using CGPoint = global::System.Drawing.PointF;
#endif

namespace SignaturePadTests {
	[TestFixture]
	public class IsBlankTests {
		[Test]
		public void ReturnsTrueIfNoPoints ()
		{
			SignaturePadView signature = new SignaturePadView ();
			Assert.That (signature.IsBlank);
		}

		[Test]
		public void ReturnsFalseIfPointsExist ()
		{
			SignaturePadView signature = new SignaturePadView ();
			signature.LoadPoints (new CGPoint [] { new CGPoint (0, 30) });
			Assert.That (!signature.IsBlank);
		}
	}
}
