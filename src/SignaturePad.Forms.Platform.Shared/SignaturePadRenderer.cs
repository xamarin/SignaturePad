using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using Xamarin.Forms;
using SignaturePad.Forms;
using Color = Xamarin.Forms.Color;
using Point = Xamarin.Forms.Point;
#if WINDOWS_PHONE
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Xamarin.Forms.Platform.WinPhone;
using NativeSignaturePadView = Xamarin.Controls.SignaturePad;
using NativePoint = System.Windows.Point;
#elif WINDOWS_UWP
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Xamarin.Forms.Platform.UWP;
using Microsoft.Graphics.Canvas;
using NativeSignaturePadView = Xamarin.Controls.SignaturePad;
using NativePoint = Windows.Foundation.Point;
#elif __IOS__
using UIKit;
using Xamarin.Forms.Platform.iOS;
using NativeSignaturePadView = Xamarin.Controls.SignaturePadView;
using NativePoint = CoreGraphics.CGPoint;
using NativeColor = UIKit.UIColor;
#elif __ANDROID__
using Android.Graphics;
using Android.Widget;
using Xamarin.Forms.Platform.Android;
using NativeSignaturePadView = Xamarin.Controls.SignaturePadView;
using NativePoint = System.Drawing.PointF;
using NativeColor = Android.Graphics.Color;
#endif

[assembly: ExportRenderer (typeof (SignaturePadView), typeof (SignaturePadRenderer))]

namespace SignaturePad.Forms
{
	public class SignaturePadRenderer : ViewRenderer<SignaturePadView, NativeSignaturePadView>
	{
		protected override void OnElementChanged (ElementChangedEventArgs<SignaturePadView> e)
		{
			base.OnElementChanged (e);

			if (Control == null && e.NewElement != null)
			{
				// Instantiate the native control and assign it to the Control property
#if __ANDROID__
				var native = new NativeSignaturePadView (Xamarin.Forms.Forms.Context);
#else
				var native = new NativeSignaturePadView ();
#endif
				SetNativeControl (native);
			}

			if (e.OldElement != null)
			{
				// Unsubscribe from event handlers and cleanup any resources
				e.OldElement.ImageStreamRequested -= OnImageStreamRequested;
				e.OldElement.IsBlankRequested -= OnIsBlankRequested;
				e.OldElement.PointsRequested -= OnPointsRequested;
				e.OldElement.PointsSpecified -= OnPointsSpecified;
				e.OldElement.ClearRequested -= OnClearRequested;
			}

			if (e.NewElement != null)
			{
				// Configure the control and subscribe to event handlers
				e.NewElement.ImageStreamRequested += OnImageStreamRequested;
				e.NewElement.IsBlankRequested += OnIsBlankRequested;
				e.NewElement.PointsRequested += OnPointsRequested;
				e.NewElement.PointsSpecified += OnPointsSpecified;
				e.NewElement.ClearRequested += OnClearRequested;

				UpdateAll ();
			}
		}

		protected override void OnElementPropertyChanged (object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged (sender, e);

			Update (e.PropertyName);
		}

		private void OnImageStreamRequested (object sender, SignaturePadView.ImageStreamRequestedEventArgs e)
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

				e.ImageStreamTask = ctrl.GetImageStreamAsync (format, settings);
			}
		}

		private void OnIsBlankRequested (object sender, SignaturePadView.IsBlankRequestedEventArgs e)
		{
			var ctrl = Control;
			if (ctrl != null)
			{
				e.IsBlank = ctrl.IsBlank;
			}
		}

		private void OnPointsRequested (object sender, SignaturePadView.PointsEventArgs e)
		{
			var ctrl = Control;
			if (ctrl != null)
			{
				e.Points = ctrl.Points.Select (p => new Point (p.X, p.Y));
			}
		}

		private void OnPointsSpecified (object sender, SignaturePadView.PointsEventArgs e)
		{
			var ctrl = Control;
			if (ctrl != null)
			{
				ctrl.LoadPoints (e.Points.Select (p => new NativePoint ((float)p.X, (float)p.Y)).ToArray ());
			}
		}

		private void OnClearRequested (object sender, System.EventArgs e)
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

			if (Element.BackgroundColor != Color.Default)
			{
				Control.BackgroundColor = Element.BackgroundColor.ToNative ();
			}
			if (!string.IsNullOrEmpty (Element.CaptionText))
			{
				Control.CaptionText = Element.CaptionText;
			}
			if (Element.CaptionTextColor != Color.Default)
			{
				Control.Caption.SetTextColor (Element.CaptionTextColor);
			}
			if (!string.IsNullOrEmpty (Element.ClearText))
			{
				Control.ClearLabelText = Element.ClearText;
			}
			if (Element.ClearTextColor != Color.Default)
			{
				Control.ClearLabel.SetTextColor (Element.ClearTextColor);
			}
			if (!string.IsNullOrEmpty (Element.PromptText))
			{
				Control.SignaturePromptText = Element.PromptText;
			}
			if (Element.PromptTextColor != Color.Default)
			{
				Control.SignaturePrompt.SetTextColor (Element.PromptTextColor);
			}
			if (Element.SignatureLineColor != Color.Default)
			{
				Control.SignatureLineColor = Element.SignatureLineColor.ToNative ();
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

			if (property == SignaturePadView.BackgroundColorProperty.PropertyName)
			{
				Control.BackgroundColor = Element.BackgroundColor.ToNative ();
			}
			else if (property == SignaturePadView.CaptionTextProperty.PropertyName)
			{
				Control.CaptionText = Element.CaptionText;
			}
			else if (property == SignaturePadView.CaptionTextColorProperty.PropertyName)
			{
				Control.Caption.SetTextColor (Element.CaptionTextColor);
			}
			else if (property == SignaturePadView.ClearTextProperty.PropertyName)
			{
				Control.ClearLabelText = Element.ClearText;
			}
			else if (property == SignaturePadView.ClearTextColorProperty.PropertyName)
			{
				Control.ClearLabel.SetTextColor (Element.ClearTextColor);
			}
			else if (property == SignaturePadView.PromptTextProperty.PropertyName)
			{
				Control.SignaturePromptText = Element.PromptText;
			}
			else if (property == SignaturePadView.PromptTextColorProperty.PropertyName)
			{
				Control.SignaturePrompt.SetTextColor (Element.PromptTextColor);
			}
			else if (property == SignaturePadView.SignatureLineColorProperty.PropertyName)
			{
				Control.SignatureLineColor = Element.SignatureLineColor.ToNative ();
			}
			else if (property == SignaturePadView.StrokeColorProperty.PropertyName)
			{
				Control.StrokeColor = Element.StrokeColor.ToNative ();
			}
			else if (property == SignaturePadView.StrokeWidthProperty.PropertyName)
			{
				Control.StrokeWidth = Element.StrokeWidth;
			}
		}
	}
}
