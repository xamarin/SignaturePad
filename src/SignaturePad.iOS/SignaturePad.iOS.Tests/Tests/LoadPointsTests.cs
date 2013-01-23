using System;
using System.Linq;
using NUnit.Framework;
using MonoTouch.UIKit;
using System.Drawing;

using SignaturePad;

namespace SignaturePadTests {
	[TestFixture]
	public class LoadPointsTests {
		[Test]
		public void ValidPointsArrayDoesGetLoaded ()
		{
			SignaturePadView signature = new SignaturePadView ();
			PointF [] points = new PointF [] { new PointF (0, 0), new PointF (10, 30), new PointF (50, 70) };
			signature.LoadPoints (points);
			Assert.That (signature.Points.Count () > 0);
		}

		[Test]
		public void NullPointsArrayDoesntChangeAnything ()
		{
			SignaturePadView signature = new SignaturePadView ();
			PointF [] points = new PointF [] { new PointF (0, 0), new PointF (10, 30), new PointF (50, 70) };
			signature.LoadPoints (points);
			signature.LoadPoints (null);
			Assert.That (signature.Points.Count () > 0);
		}

		[Test]
		public void EmptyPointsArrayDoesntChangeAnything ()
		{
			SignaturePadView signature = new SignaturePadView ();
			PointF [] points = new PointF [] { new PointF (0, 0), new PointF (10, 30), new PointF (50, 70) };
			signature.LoadPoints (points);
			signature.LoadPoints (new PointF [0]);
			Assert.That (signature.Points.Count () > 0);
		}
	}
}
