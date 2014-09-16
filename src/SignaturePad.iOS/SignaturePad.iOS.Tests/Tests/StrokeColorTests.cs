using System;
using NUnit.Framework;

#if __UNIFIED__
using UIKit;
#else
using MonoTouch.UIKit;
#endif

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
