using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Input.Inking;
using Windows.UI.Input.Inking.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Microsoft.Graphics.Canvas;

namespace Xamarin.Controls
{
    public sealed partial class SignaturePad : UserControl
    {
        private const float UWP_DEFAULT_PRESSURE = 0.5f;

        #region properties

        /// <summary>
        /// Property to check if its blank.
        /// </summary>
        public bool IsBlank =>
            (inkCanvas == null) ||
            (!inkCanvas.InkPresenter.StrokeContainer.GetStrokes().Any());

        private CoreInkIndependentInputSource coreIIIS;

        public Point[] Points
        {
            get
            {
                if (IsBlank)
                    return new Point[0];

                var points = inkCanvas?.InkPresenter?.StrokeContainer?.GetStrokes()
                    .SelectMany(str => str.GetInkPoints().Concat(new[] { new InkPoint(new Point(0, 0), UWP_DEFAULT_PRESSURE) }))
                    .Select(ip => ip.Position)
                    .ToArray();

                return points ?? new Point[0];
            }
        }

        #region UI

        private Color strokeColor;
        public Color StrokeColor
        {
            get { return strokeColor; }
            set
            {
                strokeColor = value;

                var drawingAttributes = inkCanvas.InkPresenter.CopyDefaultDrawingAttributes();
                drawingAttributes.Color = strokeColor;
                inkCanvas.InkPresenter.UpdateDefaultDrawingAttributes(drawingAttributes);
            }
        }

        private Color backgroundColor;
        public Color BackgroundColor
        {
            get { return backgroundColor; }
            set
            {
                backgroundColor = value;
                LayoutRoot.Background = new SolidColorBrush(value);
            }
        }

        private float lineWidth;
        public float StrokeWidth
        {
            get { return lineWidth; }
            set
            {
                lineWidth = value;

                var drawingAttributes = inkCanvas.InkPresenter.CopyDefaultDrawingAttributes();
                drawingAttributes.Size = new Size(lineWidth, lineWidth);
                inkCanvas.InkPresenter.UpdateDefaultDrawingAttributes(drawingAttributes);
            }
        }

        public TextBlock Caption => captionLabel;

        public string CaptionText
        {
            get { return captionLabel.Text; }
            set { captionLabel.Text = value; }
        }

        public TextBlock ClearLabel => btnClear;

        public string ClearLabelText
        {
            get { return btnClear.Text; }
            set { btnClear.Text = value; }
        }

        public TextBlock SignaturePrompt => promptLabel;

        public string SignaturePromptText
        {
            get { return promptLabel.Text; }
            set { promptLabel.Text = value; }
        }

        public Border SignatureLine => signatureBorder;

        public Brush SignatureLineBrush
        {
            get { return signatureBorder.Background; }
            set { signatureBorder.Background = value; }
        }

        #endregion

        #endregion

        #region ctor

        public SignaturePad()
        {
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            // Set supported inking device types.
            inkCanvas.InkPresenter.InputDeviceTypes = CoreInputDeviceTypes.Touch | CoreInputDeviceTypes.Pen | CoreInputDeviceTypes.Mouse;

            strokeColor = Colors.Black;
            backgroundColor = Colors.White;
            LayoutRoot.Background = new SolidColorBrush(backgroundColor);
            lineWidth = 3f;

            // Set initial ink stroke attributes.
            var drawingAttributes = new InkDrawingAttributes
            {
                Color = StrokeColor,
                Size = new Size(lineWidth, lineWidth),
                IgnorePressure = false,
                FitToCurve = true
            };
            inkCanvas.InkPresenter.UpdateDefaultDrawingAttributes(drawingAttributes);

            SetRectangleGeometry(inkCanvas.ActualWidth, inkCanvas.ActualHeight);
            inkCanvas.SizeChanged += OnInkCanvasSizeChanged;

            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            coreIIIS = CoreInkIndependentInputSource.Create(inkCanvas.InkPresenter);
            coreIIIS.PointerReleasing += OnPointerReleasing;
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            coreIIIS.PointerReleasing -= OnPointerReleasing;
            coreIIIS = null;
        }

        private async void OnPointerReleasing(CoreInkIndependentInputSource sender, PointerEventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                btnClear.Visibility = Visibility.Visible;
            });
        }

        #endregion

        #region events

        private void OnInkCanvasSizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetRectangleGeometry(e.NewSize.Width, e.NewSize.Height);
        }

        private void OnClearClick(object sender, TappedRoutedEventArgs e)
        {
            Clear();
        }

        #endregion

        #region methods

        private void SetRectangleGeometry(double width, double height)
        {
            inkCanvas.Clip = new RectangleGeometry { Rect = new Rect(0, 0, width, height) };
        }

        public void Clear()
        {
            btnClear.Visibility = Visibility.Collapsed;

            inkCanvas.InkPresenter.StrokeContainer.Clear();
        }

        #region GetImage

        //Create a WriteableBitmap of the currently drawn signature with default colors.
        public WriteableBitmap GetImage(bool shouldCrop = true, bool keepAspectRatio = true)
        {
            return GetImage(strokeColor, Colors.Transparent, RenderSize, 1, shouldCrop, keepAspectRatio);
        }

        public WriteableBitmap GetImage(Size size, bool shouldCrop = true, bool keepAspectRatio = true)
        {
            return GetImage(strokeColor, Colors.Transparent, size, GetScaleFromSize(size, RenderSize), shouldCrop, keepAspectRatio);
        }

        public WriteableBitmap GetImage(float scale, bool shouldCrop = true, bool keepAspectRatio = true)
        {
            return GetImage(strokeColor, Colors.Transparent, GetSizeFromScale(scale, RenderSize), scale, shouldCrop, keepAspectRatio);
        }

        //Create a WriteableBitmap of the currently drawn signature with the specified Stroke color.
        public WriteableBitmap GetImage(Color strokeColor, bool shouldCrop = true, bool keepAspectRatio = true)
        {
            return GetImage(strokeColor, Colors.Transparent, RenderSize, 1, shouldCrop, keepAspectRatio);
        }

        public WriteableBitmap GetImage(Color strokeColor, Size size, bool shouldCrop = true, bool keepAspectRatio = true)
        {
            return GetImage(strokeColor, Colors.Transparent, size, GetScaleFromSize(size, RenderSize), shouldCrop, keepAspectRatio);
        }

        public WriteableBitmap GetImage(Color strokeColor, float scale, bool shouldCrop = true, bool keepAspectRatio = true)
        {
            return GetImage(strokeColor, Colors.Transparent, GetSizeFromScale(scale, RenderSize), scale, shouldCrop, keepAspectRatio);
        }

        //Create a WriteableBitmap of the currently drawn signature with the specified Stroke and Fill colors.
        public WriteableBitmap GetImage(Color strokeColor, Color fillColor, bool shouldCrop = true, bool keepAspectRatio = true)
        {
            return GetImage(strokeColor, fillColor, RenderSize, 1, shouldCrop, keepAspectRatio);
        }

        public WriteableBitmap GetImage(Color strokeColor, Color fillColor, Size size, bool shouldCrop = true, bool keepAspectRatio = true)
        {
            return GetImage(strokeColor, fillColor, size, GetScaleFromSize(size, RenderSize), shouldCrop, keepAspectRatio);
        }

        public WriteableBitmap GetImage(Color strokeColor, Color fillColor, float scale, bool shouldCrop = true, bool keepAspectRatio = true)
        {
            return GetImage(strokeColor, fillColor, GetSizeFromScale(scale, RenderSize), scale, shouldCrop, keepAspectRatio);
        }

        private WriteableBitmap GetImage(Color strokeColor, Color fillColor, Size size, float scale, bool shouldCrop = true, bool keepAspectRatio = true)
        {
            using (var offscreen = GetRenderTarget(strokeColor, fillColor, size, scale, shouldCrop, keepAspectRatio))
            {
                var bitmap = new WriteableBitmap((int)offscreen.SizeInPixels.Width, (int)offscreen.SizeInPixels.Height);
                offscreen.GetPixelBytes(bitmap.PixelBuffer);
                return bitmap;
            }
        }

        //Create a WriteableBitmap of the currently drawn signature with default colors.
        public Task<Stream> GetImageStream(CanvasBitmapFileFormat type, bool shouldCrop = true, bool keepAspectRatio = true)
        {
            return GetImageStream(type, strokeColor, Colors.Transparent, RenderSize, 1, shouldCrop, keepAspectRatio);
        }

        public Task<Stream> GetImageStream(CanvasBitmapFileFormat type, Size size, bool shouldCrop = true, bool keepAspectRatio = true)
        {
            return GetImageStream(type, strokeColor, Colors.Transparent, size, GetScaleFromSize(size, RenderSize), shouldCrop, keepAspectRatio);
        }

        public Task<Stream> GetImageStream(CanvasBitmapFileFormat type, float scale, bool shouldCrop = true, bool keepAspectRatio = true)
        {
            return GetImageStream(type, strokeColor, Colors.Transparent, GetSizeFromScale(scale, RenderSize), scale, shouldCrop, keepAspectRatio);
        }

        //Create a WriteableBitmap of the currently drawn signature with the specified Stroke color.
        public Task<Stream> GetImageStream(CanvasBitmapFileFormat type, Color strokeColor, bool shouldCrop = true, bool keepAspectRatio = true)
        {
            return GetImageStream(type, strokeColor, Colors.Transparent, RenderSize, 1, shouldCrop, keepAspectRatio);
        }

        public Task<Stream> GetImageStream(CanvasBitmapFileFormat type, Color strokeColor, Size size, bool shouldCrop = true, bool keepAspectRatio = true)
        {
            return GetImageStream(type, strokeColor, Colors.Transparent, size, GetScaleFromSize(size, RenderSize), shouldCrop, keepAspectRatio);
        }

        public Task<Stream> GetImageStream(CanvasBitmapFileFormat type, Color strokeColor, float scale, bool shouldCrop = true, bool keepAspectRatio = true)
        {
            return GetImageStream(type, strokeColor, Colors.Transparent, GetSizeFromScale(scale, RenderSize), scale, shouldCrop, keepAspectRatio);
        }

        //Create a WriteableBitmap of the currently drawn signature with the specified Stroke and Fill colors.
        public Task<Stream> GetImageStream(CanvasBitmapFileFormat type, Color strokeColor, Color fillColor, bool shouldCrop = true, bool keepAspectRatio = true)
        {
            return GetImageStream(type, strokeColor, fillColor, RenderSize, 1, shouldCrop, keepAspectRatio);
        }

        public Task<Stream> GetImageStream(CanvasBitmapFileFormat type, Color strokeColor, Color fillColor, Size size, bool shouldCrop = true, bool keepAspectRatio = true)
        {
            return GetImageStream(type, strokeColor, fillColor, size, GetScaleFromSize(size, RenderSize), shouldCrop, keepAspectRatio);
        }

        public Task<Stream> GetImageStream(CanvasBitmapFileFormat type, Color strokeColor, Color fillColor, float scale, bool shouldCrop = true, bool keepAspectRatio = true)
        {
            return GetImageStream(type, strokeColor, fillColor, GetSizeFromScale(scale, RenderSize), scale, shouldCrop, keepAspectRatio);
        }

        private async Task<Stream> GetImageStream(CanvasBitmapFileFormat type, Color strokeColor, Color fillColor, Size size, float scale, bool shouldCrop = true, bool keepAspectRatio = true)
        {
            using (var offscreen = GetRenderTarget(strokeColor, fillColor, size, scale, shouldCrop, keepAspectRatio))
            {
                var fileStream = new InMemoryRandomAccessStream();
                await offscreen.SaveAsync(fileStream, type);

                var stream = fileStream.AsStream();
                stream.Position = 0;
                return stream;
            }
        }

        private CanvasRenderTarget GetRenderTarget(Color strokeColor, Color fillColor, Size size, float scale, bool shouldCrop, bool keepAspectRatio)
        {
            if (size.Width == 0 || size.Height == 0 || scale <= 0 || strokeColor == null || fillColor == null)
                return null;

            float uncroppedScale;
            Rect croppedRectangle = new Rect();

            Point[] cachedPoints;

            if (shouldCrop && (cachedPoints = Points).Any())
            {
                croppedRectangle = GetCroppedRectangle(cachedPoints);

                if (croppedRectangle.X >= 5)
                {
                    croppedRectangle.X -= 5;
                    croppedRectangle.Width += 5;
                }
                if (croppedRectangle.Y >= 5)
                {
                    croppedRectangle.Y -= 5;
                    croppedRectangle.Height += 5;
                }
                if (croppedRectangle.X + croppedRectangle.Width <= size.Width - 5)
                    croppedRectangle.Width += 5;
                if (croppedRectangle.Y + croppedRectangle.Height <= size.Height - 5)
                    croppedRectangle.Height += 5;

                double scaleX = croppedRectangle.Width / size.Width;
                double scaleY = croppedRectangle.Height / size.Height;
                uncroppedScale = (float)(1 / Math.Max(scaleX, scaleY));
            }
            else
            {
                uncroppedScale = scale;
            }

            double width;
            double height;
            if (shouldCrop)
            {
                width = keepAspectRatio ? size.Width / uncroppedScale : croppedRectangle.Width;
                height = keepAspectRatio ? size.Height / uncroppedScale : croppedRectangle.Height;
            }
            else
            {
                width = size.Width;
                height = size.Height;
            }

            var device = CanvasDevice.GetSharedDevice();
            var currentDpi = DisplayInformation.GetForCurrentView().LogicalDpi;

            var offscreen = new CanvasRenderTarget(device, (int)width, (int)height, currentDpi);

            using (var session = offscreen.CreateDrawingSession())
            {
                session.Clear(fillColor);
                if (shouldCrop)
                {
                    session.Transform = Matrix3x2.CreateTranslation((float)-croppedRectangle.X, (float)-croppedRectangle.Y);
                }
                IEnumerable<InkStroke> strokes = inkCanvas.InkPresenter.StrokeContainer.GetStrokes();

                // apply the specified colors/style
                strokes = strokes.Select(s =>
                {
                    // clone first, since this will change the UI if we don't
                    s = s.Clone();
                    var attr = s.DrawingAttributes;
                    attr.Color = strokeColor;
                    attr.Size = new Size(lineWidth, lineWidth);
                    s.DrawingAttributes = attr;
                    return s;
                });

                session.DrawInk(strokes);
            }

            return offscreen;
        }

        private Rect GetCroppedRectangle(Point[] cachedPoints)
        {
            var xMin = cachedPoints.Where(point => point != new Point(0, 0)).Min(point => point.X) - StrokeWidth / 2;
            var xMax = cachedPoints.Where(point => point != new Point(0, 0)).Max(point => point.X) + StrokeWidth / 2;
            var yMin = cachedPoints.Where(point => point != new Point(0, 0)).Min(point => point.Y) - StrokeWidth / 2;
            var yMax = cachedPoints.Where(point => point != new Point(0, 0)).Max(point => point.Y) + StrokeWidth / 2;

            xMin = Math.Max(xMin, 0);
            xMax = Math.Min(xMax, ActualWidth);
            yMin = Math.Max(yMin, 0);
            yMax = Math.Min(yMax, ActualHeight);

            return new Rect(xMin, yMin, xMax - xMin, yMax - yMin);
        }

        private float GetScaleFromSize(Size size, Size original)
        {
            double scaleX = size.Width / original.Width;
            double scaleY = size.Height / original.Height;

            return (float)Math.Min(scaleX, scaleY);
        }

        private Size GetSizeFromScale(float scale, Size original)
        {
            double width = original.Width * scale;
            double height = original.Height * scale;

            return new Size(width, height);
        }

        #endregion

        public void LoadPoints(Point[] loadedPoints)
        {
            if (loadedPoints == null || loadedPoints.Count() == 0)
                return;

            var strokeBuilder = new InkStrokeBuilder();
            var drawingAttributes = inkCanvas.InkPresenter.CopyDefaultDrawingAttributes();
            drawingAttributes.Color = strokeColor;
            drawingAttributes.Size = new Size(lineWidth, lineWidth);
            strokeBuilder.SetDefaultDrawingAttributes(drawingAttributes);

            var strokes = new List<InkStroke>();
            var currentPoints = new List<InkPoint>();
            foreach (var point in loadedPoints)
            {
                if (point == new Point(0, 0))
                {
                    var stroke = strokeBuilder.CreateStrokeFromInkPoints(currentPoints, Matrix3x2.Identity);
                    strokes.Add(stroke);
                    currentPoints = new List<InkPoint>();
                }
                else
                    currentPoints.Add(new InkPoint(point, UWP_DEFAULT_PRESSURE));
            }

            if (strokes.Count > 0)
            {
                inkCanvas.InkPresenter.StrokeContainer.AddStrokes(strokes);
            }

            //Display the clear button.
            if (!IsBlank)
                btnClear.Visibility = Visibility.Visible;
        }

        #endregion
    }
}
