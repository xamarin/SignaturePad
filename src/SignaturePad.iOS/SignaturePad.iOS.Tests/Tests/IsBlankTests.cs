using System;
using Xamarin.Controls;
using NUnit.Framework;
using System.Drawing;

namespace SignaturePadTests {
	[TestFixture]
	public class IsBlankTests {
		[Test]
		public void ReturnsTrueIfNoPoints ()
		{
			SignaturePad signature = new SignaturePad ();
			Assert.That (signature.IsBlank);
		}

		[Test]
		public void ReturnsFalseIfPointsExist ()
		{
			SignaturePad signature = new SignaturePad ();
			signature.LoadPoints (new PointF [] { new PointF (0, 30) });
			Assert.That (!signature.IsBlank);
		}
	}
}
