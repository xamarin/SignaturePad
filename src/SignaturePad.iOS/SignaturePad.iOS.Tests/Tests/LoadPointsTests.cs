using System;
using System.Linq;
using Xamarin.Controls;
using NUnit.Framework;
using MonoTouch.UIKit;
using System.Drawing;

namespace SignaturePadTests {
	[TestFixture]
	public class LoadPointsTests {
		[Test]
		public void ValidPointsArrayDoesGetLoaded ()
		{
			SignaturePad signature = new SignaturePad ();
			PointF [] points = new PointF [] { new PointF (0, 0), new PointF (10, 30), new PointF (50, 70) };
			signature.LoadPoints (points);
			Assert.That (signature.Points.Count () > 0);
		}

		[Test]
		public void NullPointsArrayDoesntChangeAnything ()
		{
			SignaturePad signature = new SignaturePad ();
			PointF [] points = new PointF [] { new PointF (0, 0), new PointF (10, 30), new PointF (50, 70) };
			signature.LoadPoints (points);
			signature.LoadPoints (null);
			Assert.That (signature.Points.Count () > 0);
		}

		[Test]
		public void EmptyPointsArrayDoesntChangeAnything ()
		{
			SignaturePad signature = new SignaturePad ();
			PointF [] points = new PointF [] { new PointF (0, 0), new PointF (10, 30), new PointF (50, 70) };
			signature.LoadPoints (points);
			signature.LoadPoints (new PointF [0]);
			Assert.That (signature.Points.Count () > 0);
		}
	}
}
