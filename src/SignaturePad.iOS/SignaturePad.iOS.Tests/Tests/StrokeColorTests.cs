using System;
using Xamarin.Controls;
using NUnit.Framework;
using MonoTouch.UIKit;

namespace SignaturePadTests {
	[TestFixture]
	public class StrokeColorTests {
		[Test]
		public void StrokeColorChanges ()
		{
			SignaturePad signature = new SignaturePad ();
			signature.StrokeColor = UIColor.Green;
			Assert.That (signature.StrokeColor == UIColor.Green);
		}

		[Test]
		public void NullStrokeColorDoesntChange ()
		{
			SignaturePad signature = new SignaturePad ();
			signature.StrokeColor = UIColor.Green;
			signature.StrokeColor = null;
			Assert.That (signature.StrokeColor == UIColor.Green);
		}
	}
}
