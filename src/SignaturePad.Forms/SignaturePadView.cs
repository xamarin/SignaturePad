using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SignaturePad.Forms
{
    public class SignaturePadView : View
    {
        public static readonly BindableProperty CaptionTextProperty = BindableProperty.Create(nameof(CaptionText), typeof(string), typeof(SignaturePadView), null);
        public static readonly BindableProperty CaptionTextColorProperty = BindableProperty.Create(nameof(CaptionTextColor), typeof(Color), typeof(SignaturePadView), Color.Default);
        public static readonly BindableProperty ClearTextProperty = BindableProperty.Create(nameof(ClearText), typeof(string), typeof(SignaturePadView), null);
        public static readonly BindableProperty ClearTextColorProperty = BindableProperty.Create(nameof(ClearTextColor), typeof(Color), typeof(SignaturePadView), Color.Default);
        public static readonly BindableProperty PromptTextProperty = BindableProperty.Create(nameof(PromptText), typeof(string), typeof(SignaturePadView), null);
        public static readonly BindableProperty PromptTextColorProperty = BindableProperty.Create(nameof(PromptTextColor), typeof(Color), typeof(SignaturePadView), Color.Default);
        public static readonly BindableProperty SignatureLineColorProperty = BindableProperty.Create(nameof(SignatureLineColor), typeof(Color), typeof(SignaturePadView), Color.Default);
        public static readonly BindableProperty StrokeColorProperty = BindableProperty.Create(nameof(StrokeColor), typeof(Color), typeof(SignaturePadView), Color.Default);
        public static readonly BindableProperty StrokeWidthProperty = BindableProperty.Create(nameof(StrokeWidth), typeof(float), typeof(SignaturePadView), (float)0);

        public bool IsBlank
        {
            get { return RequestIsBlank(); }
        }

        public string CaptionText
        {
            get { return (string)GetValue(CaptionTextProperty); }
            set { SetValue(CaptionTextProperty, value); }
        }

        public Color CaptionTextColor
        {
            get { return (Color)GetValue(CaptionTextColorProperty); }
            set { SetValue(CaptionTextColorProperty, value); }
        }

        public string ClearText
        {
            get { return (string)GetValue(ClearTextProperty); }
            set { SetValue(ClearTextProperty, value); }
        }

        public Color ClearTextColor
        {
            get { return (Color)GetValue(ClearTextColorProperty); }
            set { SetValue(ClearTextColorProperty, value); }
        }

        public string PromptText
        {
            get { return (string)GetValue(PromptTextProperty); }
            set { SetValue(PromptTextProperty, value); }
        }

        public Color PromptTextColor
        {
            get { return (Color)GetValue(PromptTextColorProperty); }
            set { SetValue(PromptTextColorProperty, value); }
        }

        public Color SignatureLineColor
        {
            get { return (Color)GetValue(SignatureLineColorProperty); }
            set { SetValue(SignatureLineColorProperty, value); }
        }

        public float StrokeWidth
        {
            get { return (float)GetValue(StrokeWidthProperty); }
            set { SetValue(StrokeWidthProperty, value); }
        }

        public Color StrokeColor
        {
            get { return (Color)GetValue(StrokeColorProperty); }
            set { SetValue(StrokeColorProperty, value); }
        }

        public IEnumerable<Point> Points
        {
            get { return GetSignaturePoints(); }
            set { SetSignaturePoints(value); }
        }

        /// <summary>
        /// Returns an image data stream for the current signature.
        /// (The caller must dispose the stream)
        /// </summary>
        /// <param name="imageFormat">The format/encoding of the image that is desired.</param>
        /// <returns>Returns the image data stream.</returns>
        public Task<Stream> GetImageStreamAsync(SignatureImageFormat imageFormat)
        {
            var args = new ImageStreamRequestedEventArgs(imageFormat);
            ImageStreamRequested?.Invoke(this, args);
            return args.ImageStreamTask;
        }

        public void Clear ()
        {
            ClearRequested?.Invoke (this, null);
        }

        private IEnumerable<Point> GetSignaturePoints()
        {
            var args = new PointsEventArgs();
            PointsRequested?.Invoke(this, args);
            return args.Points;
        }

        private void SetSignaturePoints(IEnumerable<Point> points)
        {
            PointsSpecified?.Invoke(this, new PointsEventArgs { Points = points });
        }

        private bool RequestIsBlank()
        {
            var args = new IsBlankRequestedEventArgs();
            IsBlankRequested?.Invoke(this, args);
            return args.IsBlank;
        }

        internal event EventHandler<ImageStreamRequestedEventArgs> ImageStreamRequested;
        internal event EventHandler<IsBlankRequestedEventArgs> IsBlankRequested;
        internal event EventHandler<PointsEventArgs> PointsRequested;
        internal event EventHandler<PointsEventArgs> PointsSpecified;
        internal event EventHandler<EventArgs> ClearRequested;

        internal class ImageStreamRequestedEventArgs : EventArgs
        {
            public ImageStreamRequestedEventArgs(SignatureImageFormat imageFormat)
            {
                ImageFormat = imageFormat;
            }

            public SignatureImageFormat ImageFormat { get; private set; }

            public Task<Stream> ImageStreamTask { get; set; } = Task.FromResult<Stream>(null);
        }

        internal class IsBlankRequestedEventArgs : EventArgs
        {
            public bool IsBlank { get; set; } = true;
        }

        internal class PointsEventArgs : EventArgs
        {
            public IEnumerable<Point> Points { get; set; } = new Point[0];
        }
    }
}
