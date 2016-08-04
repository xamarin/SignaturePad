using System;
using System.Drawing;
using System.Linq;
using CoreGraphics;
using UIKit;
using NUnit.Framework;
using SignaturePad;

namespace SignaturePadTests {
	[TestFixture]
	public class LoadPointsTests {
		[Test]
		public void ValidPointsArrayDoesGetLoaded ()
		{
			SignaturePadView signature = new SignaturePadView ();
			CGPoint [] points = new CGPoint [] { new CGPoint (0, 0), new CGPoint (10, 30), new CGPoint (50, 70) };
			signature.LoadPoints (points);
			Assert.That (signature.Points.Count () > 0);
		}

		[Test]
		public void NullPointsArrayDoesntChangeAnything ()
		{
			SignaturePadView signature = new SignaturePadView ();
			CGPoint [] points = new CGPoint [] { new CGPoint (0, 0), new CGPoint (10, 30), new CGPoint (50, 70) };
			signature.LoadPoints (points);
			signature.LoadPoints (null);
			Assert.That (signature.Points.Count () > 0);
		}

		[Test]
		public void EmptyPointsArrayDoesntChangeAnything ()
		{
			SignaturePadView signature = new SignaturePadView ();
			CGPoint [] points = new CGPoint [] { new CGPoint (0, 0), new CGPoint (10, 30), new CGPoint (50, 70) };
			signature.LoadPoints (points);
			signature.LoadPoints (new CGPoint [0]);
			Assert.That (signature.Points.Count () > 0);
		}
	}
}
