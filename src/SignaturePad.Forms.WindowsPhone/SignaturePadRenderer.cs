using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Media;
using Xamarin.Forms;
using Xamarin.Forms.Platform.WinPhone;
using SignaturePad.Forms;
using SignaturePad.Forms.WindowsPhone;
using NativeSignaturePadView = Xamarin.Controls.SignaturePad;
using NativePoint = System.Windows.Point;
using NativeColor = System.Windows.Media.Color;
using Color = Xamarin.Forms.Color;
using Point = Xamarin.Forms.Point;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using ImageTools.IO.Png;
using ImageTools;

[assembly: ExportRenderer(typeof(SignaturePadView), typeof(SignaturePadRenderer))]

namespace SignaturePad.Forms.WindowsPhone
{
    public class SignaturePadRenderer : ViewRenderer<SignaturePadView, NativeSignaturePadView>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<SignaturePadView> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                // Instantiate the native control and assign it to the Control property
                var native = new NativeSignaturePadView();
                SetNativeControl(native);
            }

            if (e.OldElement != null)
            {
                // Unsubscribe from event handlers and cleanup any resources
                e.OldElement.ImageStreamRequested -= OnImageStreamRequested;
                e.OldElement.IsBlankRequested -= OnIsBlankRequested;
                e.OldElement.PointsRequested -= OnPointsRequested;
                e.OldElement.PointsSpecified -= OnPointsSpecified;
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
            var ctrl = Control;
            if (ctrl != null)
            {
                e.Points = ctrl.Points.Select(p => new Point(p.X, p.Y));
            }
        }

        private void OnPointsSpecified(object sender, SignaturePadView.PointsEventArgs e)
        {
            var ctrl = Control;
            if (ctrl != null)
            {
                ctrl.LoadPoints(e.Points.Select(p => new NativePoint((float)p.X, (float)p.Y)).ToArray());
            }
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
                Control.BackgroundColor = Element.BackgroundColor.ToWindows();
            }
            if (!string.IsNullOrEmpty(Element.CaptionText))
            {
                Control.CaptionText = Element.CaptionText;
            }
            if (Element.CaptionTextColor != Color.Default)
            {
                Control.Caption.Foreground = new SolidColorBrush(Element.CaptionTextColor.ToWindows());
            }
            if (!string.IsNullOrEmpty(Element.ClearText))
            {
                Control.ClearLabelText = Element.ClearText;
            }
            if (Element.ClearTextColor != Color.Default)
            {
                Control.ClearLabel.Foreground = new SolidColorBrush(Element.ClearTextColor.ToWindows());
            }
            if (!string.IsNullOrEmpty(Element.PromptText))
            {
                Control.SignaturePromptText = Element.PromptText;
            }
            if (Element.PromptTextColor != Color.Default)
            {
                Control.SignaturePrompt.Foreground = new SolidColorBrush(Element.PromptTextColor.ToWindows());
            }
            if (Element.SignatureLineColor != Color.Default)
            {
                Control.SignatureLineBrush = new SolidColorBrush(Element.SignatureLineColor.ToWindows());
            }
            if (Element.StrokeColor != Color.Default)
            {
                Control.StrokeColor = Element.StrokeColor.ToWindows();
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
                this.Control.BackgroundColor = Element.BackgroundColor.ToWindows();
            }
            else if (property == SignaturePadView.CaptionTextProperty.PropertyName)
            {
                this.Control.CaptionText = Element.CaptionText;
            }
            else if (property == SignaturePadView.CaptionTextColorProperty.PropertyName)
            {
                this.Control.Caption.Foreground = new SolidColorBrush(Element.CaptionTextColor.ToWindows());
            }
            else if (property == SignaturePadView.ClearTextProperty.PropertyName)
            {
                this.Control.ClearLabelText = Element.ClearText;
            }
            else if (property == SignaturePadView.ClearTextColorProperty.PropertyName)
            {
                this.Control.ClearLabel.Foreground = new SolidColorBrush(Element.ClearTextColor.ToWindows());
            }
            else if (property == SignaturePadView.PromptTextProperty.PropertyName)
            {
                this.Control.SignaturePromptText = Element.PromptText;
            }
            else if (property == SignaturePadView.PromptTextColorProperty.PropertyName)
            {
                this.Control.SignaturePrompt.Foreground = new SolidColorBrush(Element.PromptTextColor.ToWindows());
            }
            else if (property == SignaturePadView.SignatureLineColorProperty.PropertyName)
            {
                this.Control.SignatureLineBrush = new SolidColorBrush(Element.SignatureLineColor.ToWindows());
            }
            else if (property == SignaturePadView.StrokeColorProperty.PropertyName)
            {
                this.Control.StrokeColor = Element.StrokeColor.ToWindows();
            }
            else if (property == SignaturePadView.StrokeWidthProperty.PropertyName)
            {
                this.Control.StrokeWidth = Element.StrokeWidth;
            }
        }
    }

    public static class ColorExtensions
    {
        public static NativeColor ToWindows(this Color color)
        {
            return NativeColor.FromArgb(
                (byte)(color.A * 255),
                (byte)(color.R * 255),
                (byte)(color.G * 255),
                (byte)(color.B * 255));
        }
    }
}