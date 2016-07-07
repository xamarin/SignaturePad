using System;
using UIKit;
using NUnit.Framework;
using SignaturePad;

namespace SignaturePadTests {
	[TestFixture]
	public class StrokeColorTests {
		[Test]
		public void StrokeColorChanges ()
		{
			SignaturePadView signature = new SignaturePadView ();
			signature.StrokeColor = UIColor.Green;
			Assert.That (signature.StrokeColor == UIColor.Green);
		}

		[Test]
		public void NullStrokeColorDoesntChange ()
		{
			SignaturePadView signature = new SignaturePadView ();
			signature.StrokeColor = UIColor.Green;
			signature.StrokeColor = null;
			Assert.That (signature.StrokeColor == UIColor.Green);
		}
	}
}
