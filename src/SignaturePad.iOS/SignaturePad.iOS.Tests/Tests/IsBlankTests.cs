using System;
using NUnit.Framework;
using System.Drawing;

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
			signature.LoadPoints (new PointF [] { new PointF (0, 30) });
			Assert.That (!signature.IsBlank);
		}
	}
}
