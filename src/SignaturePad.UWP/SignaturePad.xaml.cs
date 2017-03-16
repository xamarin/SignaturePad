using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using Microsoft.Graphics.Canvas;

namespace SignaturePad.UWP
{
    public sealed partial class SignaturePad
    {
        #region properties

        /// <summary>
        /// Property to check if its blank.
        /// </summary>
        public bool IsBlank =>
            (this.InkCanvas == null) ||
            (!this.InkCanvas.InkPresenter.StrokeContainer.GetStrokes().Any());

        CoreInkIndependentInputSource coreIIIS;

        public Point[] Points
        {
            get
            {
                if (IsBlank)
                    return new Point[0];

                var points = InkCanvas?.InkPresenter?.StrokeContainer?.GetStrokes()
                    .SelectMany(str => str.GetInkPoints().Concat(new [] {new InkPoint(new Point(0,0),1f)}))
                    .Select(ip => ip.Position)
                    .ToArray();

                return points ?? new Point[0];
            }
        }

        #region UI

        Color strokeColor;
        public Color StrokeColor
        {
            get { return strokeColor; }
            set
            {
                strokeColor = value;

                var drawingAttributes = this.InkCanvas.InkPresenter.CopyDefaultDrawingAttributes();
                drawingAttributes.Color = strokeColor;
                this.InkCanvas.InkPresenter.UpdateDefaultDrawingAttributes(drawingAttributes);
            }
        }

        Color backgroundColor;
        public Color BackgroundColor
        {
            get { return backgroundColor; }
            set
            {
                backgroundColor = value;
                LayoutRoot.Background = new SolidColorBrush(value);
            }
        }

        float lineWidth;
        public float StrokeWidth
        {
            get { return lineWidth; }
            set
            {
                lineWidth = value;

                var drawingAttributes = this.InkCanvas.InkPresenter.CopyDefaultDrawingAttributes();
                drawingAttributes.Size = new Size(lineWidth, lineWidth);
                this.InkCanvas.InkPresenter.UpdateDefaultDrawingAttributes(drawingAttributes);
            }
        }

        public TextBlock Caption
        {
            get { return captionLabel; }
        }

        public string CaptionText
        {
            get { return captionLabel.Text; }
            set { captionLabel.Text = value; }
        }

        public TextBlock ClearLabel
        {
            get { return btnClear; }
        }

        public string ClearLabelText
        {
            get { return btnClear.Text; }
            set { btnClear.Text = value; }
        }

        public TextBlock SignaturePrompt
        {
            get { return textBlock1; }
        }

        public string SignaturePromptText
        {
            get { return textBlock1.Text; }
            set { textBlock1.Text = value; }
        }

        public Border SignatureLine
        {
            get { return border1; }
        }

        public Brush SignatureLineBrush
        {
            get { return border1.Background; }
            set { border1.Background = value; }
        }

        #endregion

        #endregion

        #region ctor

        public SignaturePad()
        {
            this.InitializeComponent();

            // Set supported inking device types.
            this.InkCanvas.InkPresenter.InputDeviceTypes = CoreInputDeviceTypes.Mouse | CoreInputDeviceTypes.Pen;

            // set manipulation mode to allow manipulation event's fire
            //this.InkCanvas.ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY;
            InkCanvas.InkPresenter.InputDeviceTypes = CoreInputDeviceTypes.Mouse | CoreInputDeviceTypes.Touch;
            InkCanvas.InkPresenter.StrokesCollected += InkPresenter_StrokesCollected;

            //this.InkCanvas.ManipulationStarted += InkCanvasOnManipulationStarted;

            coreIIIS = CoreInkIndependentInputSource.Create(InkCanvas.InkPresenter);
            coreIIIS.PointerReleasing += Core_PointerReleasing;

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
            this.InkCanvas.InkPresenter.UpdateDefaultDrawingAttributes(drawingAttributes);

            SetRectangleGeometry(this.InkCanvas.ActualWidth, this.InkCanvas.ActualHeight);
            this.InkCanvas.SizeChanged += InkCanvas_SizeChanged;
        }

        private void InkPresenter_StrokesCollected(InkPresenter sender, InkStrokesCollectedEventArgs args)
        {
            if (args.Strokes.Count == sender.StrokeContainer.GetStrokes().Count)
            {
                OnIsBlankChanged(false);
            }
        }

        private async void Core_PointerReleasing(CoreInkIndependentInputSource sender, PointerEventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                btnClear.Visibility = Visibility.Visible;
            });
        }

        #endregion

        #region events

        public event EventHandler<bool> IsBlankChanged;

        private void InkCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetRectangleGeometry(e.NewSize.Width, e.NewSize.Height);
        }

        private void OnClearClick(object sender, TappedRoutedEventArgs e)
        {
            ClearStrokes();
            btnClear.Visibility = Visibility.Collapsed;
        }

        #endregion

        #region methods

        private void SetRectangleGeometry(double width, double height)
        {
            this.InkCanvas.Clip = new RectangleGeometry { Rect = new Rect(0, 0, width, height) };
        }

        private void ClearStrokes()
        {
            var wasBlank = IsBlank;
            this.InkCanvas.InkPresenter.StrokeContainer.Clear();

            if(wasBlank != IsBlank)
                OnIsBlankChanged(IsBlank);
        }

        public void UnsubscribeFromEvents()
        {
            if (this.btnClear != null)
                this.btnClear.Tapped -= OnClearClick;

            if (this.coreIIIS != null)
                this.coreIIIS.PointerReleasing -= Core_PointerReleasing;

            if (this.InkCanvas != null)
                this.InkCanvas.SizeChanged -= InkCanvas_SizeChanged;
        }


        private void OnIsBlankChanged(bool isblank)
        {
            IsBlankChanged?.Invoke(this, isblank);
        }

        #region save image

        public async Task<Stream> GetImageStreamAsync(CanvasBitmapFileFormat type)
        {
            // do not put in using{} structure - it will close the shared device.
            var device = CanvasDevice.GetSharedDevice();
            var width = (float)this.InkCanvas.ActualWidth;
            var height = (float)this.InkCanvas.ActualHeight;
            var currentDpi = DisplayInformation.GetForCurrentView().LogicalDpi;

            var fileStream = new InMemoryRandomAccessStream();
            using (var offscreen = new CanvasRenderTarget(device, width, height, currentDpi))
            {
                using (var session = offscreen.CreateDrawingSession())
                {
                    session.Clear(Colors.Transparent);
                    session.DrawInk(this.InkCanvas.InkPresenter.StrokeContainer.GetStrokes());
                }

                await offscreen.SaveAsync(fileStream, type);

                var stream = fileStream.AsStream();
                stream.Position = 0;
                return stream;
            }
        }

        public async Task<string> ConvertToBase64Async(CanvasBitmapFileFormat type)
        {
            var stream = await GetImageStreamAsync(type);
            var bytes = StreamToBytes(stream);
            var base64String = BytesToBase64String(bytes);

            return base64String;
        }

        private byte[] StreamToBytes(Stream input)
        {
            var memStream = StreamToMemoryStream(input);
            var bytes = memStream.ToArray();
            memStream.Dispose();

            return bytes;
        }

        /// <summary>
        /// Need to dispose stream after using
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private MemoryStream StreamToMemoryStream(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            var ms = new MemoryStream();
            int read;

            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                ms.Write(buffer, 0, read);
            }

            return ms;
        }

        private string BytesToBase64String(byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }

        #endregion

        public void LoadPoints(Point[] loadedPoints)
        {
            if (loadedPoints == null || loadedPoints.Count() == 0)
                return;

            var wasBlank = IsBlank;

            // unfortunately UWP does not allow to create InkStrokes from code..... crap...

            //Display the clear button.
            if(!IsBlank)
                btnClear.Visibility = Visibility.Visible;

            if (wasBlank != IsBlank)
                OnIsBlankChanged(IsBlank);
        }

        #endregion
    }
}
