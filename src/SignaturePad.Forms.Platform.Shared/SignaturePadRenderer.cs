using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using Xamarin.Forms;
using SignaturePad.Forms;
using SignaturePad.Forms.Platform;
using Color = Xamarin.Forms.Color;
using Point = Xamarin.Forms.Point;

#if WINDOWS_PHONE
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ImageTools;
using ImageTools.IO.Png;
using Xamarin.Forms.Platform.WinPhone;
using SignaturePad.Forms.WindowsPhone;
using NativeSignaturePadView = Xamarin.Controls.SignaturePad;
using NativePoint = System.Windows.Point;
#elif __IOS__
using UIKit;
using Xamarin.Forms.Platform.iOS;
using SignaturePad.Forms.iOS;
using NativeSignaturePadView = SignaturePad.SignaturePadView;
using NativePoint = CoreGraphics.CGPoint;
using NativeColor = UIKit.UIColor;
#elif __ANDROID__
using Android.Graphics;
using Android.Widget;
using Xamarin.Forms.Platform.Android;
using SignaturePad.Forms.Droid;
using NativeSignaturePadView = SignaturePad.SignaturePadView;
using NativePoint = System.Drawing.PointF;
using NativeColor = Android.Graphics.Color;
#elif WINDOWS_UWP
using Xamarin.Forms.Platform.UWP;
using SignaturePad.Forms.UWP;
using NativeSignaturePadView = SignaturePad.UWP.SignaturePad;
using System;
using Windows.UI.Xaml.Media;
#endif

[assembly: ExportRenderer(typeof(SignaturePadView), typeof(SignaturePadRenderer))]

#if WINDOWS_PHONE
namespace SignaturePad.Forms.WindowsPhone
#elif __IOS__
namespace SignaturePad.Forms.iOS
#elif __ANDROID__
namespace SignaturePad.Forms.Droid
#elif WINDOWS_UWP
namespace SignaturePad.Forms.UWP
#endif
{
    public class SignaturePadRenderer : ViewRenderer<SignaturePadView, NativeSignaturePadView>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<SignaturePadView> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                // Unsubscribe from event handlers and cleanup any resources
                e.OldElement.ImageStreamRequested -= OnImageStreamRequested;
                e.OldElement.IsBlankRequested -= OnIsBlankRequested;
                e.OldElement.PointsRequested -= OnPointsRequested;
                e.OldElement.PointsSpecified -= OnPointsSpecified;
            }

#if WINDOWS_UWP
            if (Control == null && e.NewElement == null)
                return; // handle back button press
#endif

            if (Control == null)
            {
                // Instantiate the native control and assign it to the Control property
#if __ANDROID__
                var native = new NativeSignaturePadView(Xamarin.Forms.Forms.Context);
#else
                var native = new NativeSignaturePadView();
#endif
                native.IsBlankChanged += Native_IsBlankChanged;
                SetNativeControl(native);
            }

            if (e.NewElement != null)
            {
                // Configure the control and subscribe to event handlers
                e.NewElement.ImageStreamRequested += OnImageStreamRequested;
                e.NewElement.IsBlankRequested += OnIsBlankRequested;
                e.NewElement.PointsRequested += OnPointsRequested;
                e.NewElement.PointsSpecified += OnPointsSpecified;

                UpdateAll();
            }
        }

        private void Native_IsBlankChanged(object sender, bool isBlank)
        {
            var native = (NativeSignaturePadView) sender;
            if (Element != null)
                Element.IsBlank = isBlank;
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            Update(e.PropertyName);
        }

        private void OnImageStreamRequested(object sender, SignaturePadView.ImageStreamRequestedEventArgs e)
        {
            var ctrl = Control;
            if (ctrl != null)
            {
#if WINDOWS_PHONE
                var image = ctrl.GetImage();
                ExtendedImage img = null;
                if (e.ImageFormat == SignatureImageFormat.Png)
                {
                    img = image.ToImage();
                }
                var stream = new MemoryStream();
                e.ImageStreamTask = Task.Run<Stream>(() =>
                {
                    if (e.ImageFormat == SignatureImageFormat.Png)
                    {
                        var encoder = new PngEncoder();
                        encoder.Encode(img, stream);
                        return stream;
                    }
                    if (e.ImageFormat == SignatureImageFormat.Jpg)
                    {
                        image.SaveJpeg(stream, image.PixelWidth, image.PixelHeight, 0, 100);
                        return stream;
                    }
                    return null;
                });
#elif __IOS__
                var image = ctrl.GetImage();
                e.ImageStreamTask = Task.Run(() =>
                {
                    if (e.ImageFormat == SignatureImageFormat.Png)
                    {
                        return image.AsPNG().AsStream();
                    }
                    if (e.ImageFormat == SignatureImageFormat.Jpg)
                    {
                        return image.AsJPEG().AsStream();
                    }
                    return null;
                });
#elif __ANDROID__
                var image = ctrl.GetImage();
                var stream = new MemoryStream();
                var format = e.ImageFormat == SignatureImageFormat.Png ? Bitmap.CompressFormat.Png : Bitmap.CompressFormat.Jpeg;
                e.ImageStreamTask = image
                    .CompressAsync(format, 100, stream)
                    .ContinueWith(task =>
                    {
                        image.Recycle();
                        image.Dispose();
                        image = null;
                        if (task.Result)
                        {
                            return (Stream)stream;
                        }
                        else
                        {
                            return null;
                        }
                    });
#elif WINDOWS_UWP
                var imgFormat = e.ImageFormat == SignatureImageFormat.Png
                    ? Microsoft.Graphics.Canvas.CanvasBitmapFileFormat.Png
                    : Microsoft.Graphics.Canvas.CanvasBitmapFileFormat.Jpeg;
                e.ImageStreamTask = ctrl.GetImageStreamAsync(imgFormat);
#endif
            }
        }

        private void OnIsBlankRequested(object sender, SignaturePadView.IsBlankRequestedEventArgs e)
        {
            var ctrl = Control;
            if (ctrl != null)
            {
                e.IsBlank = ctrl.IsBlank;
            }
        }

        private void OnPointsRequested(object sender, SignaturePadView.PointsEventArgs e)
        {
#if WINDOWS_UWP
            throw new NotImplementedException();
#else
            var ctrl = Control;
            if (ctrl != null)
            {
                e.Points = ctrl.Points.Select(p => new Point(p.X, p.Y));
            }
#endif
        }

        private void OnPointsSpecified(object sender, SignaturePadView.PointsEventArgs e)
        {
#if WINDOWS_UWP
            throw new NotImplementedException();
#else
            var ctrl = Control;
            if (ctrl != null)
            {
                ctrl.LoadPoints(e.Points.Select(p => new NativePoint((float)p.X, (float)p.Y)).ToArray());
            }
#endif
        }

        /// <summary>
        /// Update all the properties on the native view.
        /// </summary>
        private void UpdateAll()
        {
            if (Control == null || Element == null)
            {
                return;
            }

            if (Element.BackgroundColor != Color.Default)
            {
                Control.BackgroundColor = Element.BackgroundColor.ToNative();
            }
            if (!string.IsNullOrEmpty(Element.CaptionText))
            {
                Control.CaptionText = Element.CaptionText;
            }
            if (Element.CaptionTextColor != Color.Default)
            {
                Control.Caption.SetTextColor(Element.CaptionTextColor);
            }
            if (!string.IsNullOrEmpty(Element.ClearText))
            {
                Control.ClearLabelText = Element.ClearText;
            }
            if (Element.ClearTextColor != Color.Default)
            {
                Control.ClearLabel.SetTextColor(Element.ClearTextColor);
            }
            if (!string.IsNullOrEmpty(Element.PromptText))
            {
                Control.SignaturePromptText = Element.PromptText;
            }
            if (Element.PromptTextColor != Color.Default)
            {
                Control.SignaturePrompt.SetTextColor(Element.PromptTextColor);
            }
            if (Element.SignatureLineColor != Color.Default)
            {
                var color = Element.SignatureLineColor.ToNative();
#if WINDOWS_PHONE || WINDOWS_UWP
                Control.SignatureLineBrush = new SolidColorBrush(color);
#else
                Control.SignatureLineColor = color;
#endif
            }
            if (Element.StrokeColor != Color.Default)
            {
                Control.StrokeColor = Element.StrokeColor.ToNative();
            }
            if (Element.StrokeWidth > 0)
            {
                Control.StrokeWidth = Element.StrokeWidth;
            }
        }

        /// <summary>
        /// Update a specific property on the native view.
        /// </summary>
        private void Update(string property)
        {
            if (Control == null || Element == null)
            {
                return;
            }

            if (property == SignaturePadView.BackgroundColorProperty.PropertyName)
            {
                Control.BackgroundColor = Element.BackgroundColor.ToNative();
            }
            else if (property == SignaturePadView.CaptionTextProperty.PropertyName)
            {
                Control.CaptionText = Element.CaptionText;
            }
            else if (property == SignaturePadView.CaptionTextColorProperty.PropertyName)
            {
                Control.Caption.SetTextColor(Element.CaptionTextColor);
            }
            else if (property == SignaturePadView.ClearTextProperty.PropertyName)
            {
                Control.ClearLabelText = Element.ClearText;
            }
            else if (property == SignaturePadView.ClearTextColorProperty.PropertyName)
            {
                Control.ClearLabel.SetTextColor(Element.ClearTextColor);
            }
            else if (property == SignaturePadView.PromptTextProperty.PropertyName)
            {
                Control.SignaturePromptText = Element.PromptText;
            }
            else if (property == SignaturePadView.PromptTextColorProperty.PropertyName)
            {
                Control.SignaturePrompt.SetTextColor(Element.PromptTextColor);
            }
            else if (property == SignaturePadView.SignatureLineColorProperty.PropertyName)
            {
                var color = Element.SignatureLineColor.ToNative();
#if WINDOWS_PHONE || WINDOWS_UWP
                Control.SignatureLineBrush = new SolidColorBrush(color);
#else
                Control.SignatureLineColor = color;
#endif
            }
            else if (property == SignaturePadView.StrokeColorProperty.PropertyName)
            {
                Control.StrokeColor = Element.StrokeColor.ToNative();
            }
            else if (property == SignaturePadView.StrokeWidthProperty.PropertyName)
            {
                Control.StrokeWidth = Element.StrokeWidth;
            }
        }

#if WINDOWS_UWP
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                Control?.UnsubscribeFromEvents();

            base.Dispose(disposing);
        }
#endif
    }
}
