using System;
using System.ComponentModel;
using System.Linq;
using Xamarin.Forms;
using SignaturePad.Forms;
using Color = Xamarin.Forms.Color;
using Point = Xamarin.Forms.Point;
#if WINDOWS_PHONE
using Xamarin.Forms.Platform.WinPhone;
using NativeSignaturePadCanvasView = Xamarin.Controls.SignaturePadCanvasView;
using NativePoint = System.Windows.Point;
#elif WINDOWS_UWP
using Xamarin.Forms.Platform.UWP;
using NativeSignaturePadCanvasView = Xamarin.Controls.SignaturePadCanvasView;
using NativePoint = Windows.Foundation.Point;
#elif WINDOWS_PHONE_APP || WINDOWS_APP
using Xamarin.Forms.Platform.WinRT;
using NativeSignaturePadCanvasView = Xamarin.Controls.SignaturePadCanvasView;
using NativePoint = Windows.Foundation.Point;
#elif __IOS__
using Xamarin.Forms.Platform.iOS;
using NativeSignaturePadCanvasView = Xamarin.Controls.SignaturePadCanvasView;
using NativePoint = CoreGraphics.CGPoint;
#elif __ANDROID__
using Xamarin.Forms.Platform.Android;
using NativeSignaturePadCanvasView = Xamarin.Controls.SignaturePadCanvasView;
using NativePoint = System.Drawing.PointF;
#endif

[assembly: ExportRenderer (typeof (SignaturePadCanvasView), typeof (SignaturePadCanvasRenderer))]

namespace SignaturePad.Forms
{
	public class SignaturePadCanvasRenderer : ViewRenderer<SignaturePadCanvasView, NativeSignaturePadCanvasView>
	{
#if __ANDROID__
		[Obsolete ("This constructor is obsolete as of version 2.5. Please use 'SignaturePadCanvasRenderer (Context)' instead.")]
#endif
		public SignaturePadCanvasRenderer ()
		{
		}

#if __ANDROID__
		public SignaturePadCanvasRenderer (Android.Content.Context context)
			: base (context)
		{
		}
#endif

		protected override void OnElementChanged (ElementChangedEventArgs<SignaturePadCanvasView> e)
		{
			base.OnElementChanged (e);

			if (Control == null && e.NewElement != null)
			{
				// Instantiate the native control and assign it to the Control property
#if __ANDROID__
				var native = new NativeSignaturePadCanvasView (Context);
#else
				var native = new NativeSignaturePadCanvasView ();
#endif

				native.StrokeCompleted += OnStrokeCompleted;
				native.Cleared += OnCleared;

				SetNativeControl (native);
			}

			if (e.OldElement != null)
			{
				// Unsubscribe from event handlers and cleanup any resources
				e.OldElement.ImageStreamRequested -= OnImageStreamRequested;
				e.OldElement.IsBlankRequested -= OnIsBlankRequested;
				e.OldElement.PointsRequested -= OnPointsRequested;
				e.OldElement.PointsSpecified -= OnPointsSpecified;
				e.OldElement.StrokesRequested -= OnStrokesRequested;
				e.OldElement.StrokesSpecified -= OnStrokesSpecified;
				e.OldElement.ClearRequested -= OnClearRequested;
			}

			if (e.NewElement != null)
			{
				// Configure the control and subscribe to event handlers
				e.NewElement.ImageStreamRequested += OnImageStreamRequested;
				e.NewElement.IsBlankRequested += OnIsBlankRequested;
				e.NewElement.PointsRequested += OnPointsRequested;
				e.NewElement.PointsSpecified += OnPointsSpecified;
				e.NewElement.StrokesRequested += OnStrokesRequested;
				e.NewElement.StrokesSpecified += OnStrokesSpecified;
				e.NewElement.ClearRequested += OnClearRequested;

				UpdateAll ();
			}
		}

		protected override void OnElementPropertyChanged (object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged (sender, e);

			Update (e.PropertyName);
		}

		private void OnStrokeCompleted (object sender, EventArgs e)
		{
			Element?.OnStrokeCompleted ();
		}

		private void OnCleared (object sender, EventArgs e)
		{
			Element?.OnCleared ();
		}

		private void OnImageStreamRequested (object sender, SignaturePadCanvasView.ImageStreamRequestedEventArgs e)
		{
			var ctrl = Control;
			if (ctrl != null)
			{
				var format = e.ImageFormat == SignatureImageFormat.Png ? Xamarin.Controls.SignatureImageFormat.Png : Xamarin.Controls.SignatureImageFormat.Jpeg;

				var settings = new Xamarin.Controls.ImageConstructionSettings ();
				if (e.Settings.BackgroundColor.HasValue)
				{
					settings.BackgroundColor = e.Settings.BackgroundColor.Value.ToNative ();
				}
				if (e.Settings.DesiredSizeOrScale.HasValue)
				{
					var val = e.Settings.DesiredSizeOrScale.Value;
					settings.DesiredSizeOrScale = new Xamarin.Controls.SizeOrScale (val.X, val.Y, (Xamarin.Controls.SizeOrScaleType)(int)val.Type, val.KeepAspectRatio);
				}
				settings.ShouldCrop = e.Settings.ShouldCrop;
				if (e.Settings.StrokeColor.HasValue)
				{
					settings.StrokeColor = e.Settings.StrokeColor.Value.ToNative ();
				}
				settings.StrokeWidth = e.Settings.StrokeWidth;
				settings.Padding = e.Settings.Padding;

				e.ImageStreamTask = ctrl.GetImageStreamAsync (format, settings);
			}
		}

		private void OnIsBlankRequested (object sender, SignaturePadCanvasView.IsBlankRequestedEventArgs e)
		{
			var ctrl = Control;
			if (ctrl != null)
			{
				e.IsBlank = ctrl.IsBlank;
			}
		}

		private void OnPointsRequested (object sender, SignaturePadCanvasView.PointsEventArgs e)
		{
			var ctrl = Control;
			if (ctrl != null)
			{
				e.Points = ctrl.Points.Select (p => new Point (p.X, p.Y));
			}
		}

		private void OnPointsSpecified (object sender, SignaturePadCanvasView.PointsEventArgs e)
		{
			var ctrl = Control;
			if (ctrl != null)
			{
				ctrl.LoadPoints (e.Points.Select (p => new NativePoint ((float)p.X, (float)p.Y)).ToArray ());
			}
		}

		private void OnStrokesRequested (object sender, SignaturePadCanvasView.StrokesEventArgs e)
		{
			var ctrl = Control;
			if (ctrl != null)
			{
				e.Strokes = ctrl.Strokes.Select (s => s.Select (p => new Point (p.X, p.Y)));
			}
		}

		private void OnStrokesSpecified (object sender, SignaturePadCanvasView.StrokesEventArgs e)
		{
			var ctrl = Control;
			if (ctrl != null)
			{
				ctrl.LoadStrokes (e.Strokes.Select (s => s.Select (p => new NativePoint ((float)p.X, (float)p.Y)).ToArray ()).ToArray ());
			}
		}

		private void OnClearRequested (object sender, EventArgs e)
		{
			var ctrl = Control;
			if (ctrl != null)
			{
				ctrl.Clear ();
			}
		}

		/// <summary>
		/// Update all the properties on the native view.
		/// </summary>
		private void UpdateAll ()
		{
			if (Control == null || Element == null)
			{
				return;
			}

			if (Element.StrokeColor != Color.Default)
			{
				Control.StrokeColor = Element.StrokeColor.ToNative ();
			}
			if (Element.StrokeWidth > 0)
			{
				Control.StrokeWidth = Element.StrokeWidth;
			}
		}

		/// <summary>
		/// Update a specific property on the native view.
		/// </summary>
		private void Update (string property)
		{
			if (Control == null || Element == null)
			{
				return;
			}

			if (property == SignaturePadCanvasView.StrokeColorProperty.PropertyName)
			{
				Control.StrokeColor = Element.StrokeColor.ToNative ();
			}
			else if (property == SignaturePadCanvasView.StrokeWidthProperty.PropertyName)
			{
				Control.StrokeWidth = Element.StrokeWidth;
			}
		}
	}
}
