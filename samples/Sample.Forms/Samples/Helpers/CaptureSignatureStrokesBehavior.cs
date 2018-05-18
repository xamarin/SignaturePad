using System.Collections.Generic;
using Xamarin.Forms;

using SignaturePad.Forms;

namespace Samples
{
	public class CaptureSignatureStrokesBehavior : CaptureSignatureBehaviorBase
	{
		public static readonly BindableProperty StrokesProperty = BindableProperty.Create (
			nameof (Strokes),
			typeof (IEnumerable<IEnumerable<Point>>),
			typeof (CaptureSignatureStrokesBehavior),
			default (IEnumerable<IEnumerable<Point>>),
			BindingMode.TwoWay,
			propertyChanged: CreatePropertyChanged (StrokesProperty));

		public IEnumerable<IEnumerable<Point>> Strokes
		{
			get => (IEnumerable<IEnumerable<Point>>)GetValue (StrokesProperty);
			set => SetValue (StrokesProperty, value);
		}

		protected override void UpdateSignaturePad (SignaturePadView bindable, BindableProperty property, object oldValue, object newValue)
		{
			bindable.Strokes = newValue as IEnumerable<IEnumerable<Point>>;
		}

		protected override void UpdateBehavior (SignaturePadView signaturePad)
		{
			Strokes = signaturePad.Strokes;
		}
	}
}
