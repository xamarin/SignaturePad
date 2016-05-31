using System;
using System.Drawing;
using CoreGraphics;
using NUnit.Framework;
using SignaturePad;

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
