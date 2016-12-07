//
// SignaturePad.cs: User Control subclass for Windows Phone to allow users to draw their signature on 
//				   the device to be captured as an image or vector.
//
// Author:
//   Timothy Risi (timothy.risi@gmail.com)
//
// Copyright (C) 2012 Timothy Risi
//
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;

namespace Xamarin.Controls
{
    public partial class SignaturePad : UserControl
    {
        Point previousPosition = default(Point);
        List<List<Point>> points = new List<List<Point>>();
        bool pressed = false;
        BitmapInfo bitmapInfo = new BitmapInfo();

        //Create an array containing all of the points used to draw the signature.  Uses (0, 0)
        //to indicate a new line.
        public Point[] Points
        {
            get
            {
                if (points == null || points.Count() == 0)
                    return new Point[0];

                IEnumerable<Point> pointsList = points[0];

                for (var i = 1; i < points.Count; i++)
                {
                    pointsList = pointsList.Concat(new[] { new Point(0, 0) });
                    pointsList = pointsList.Concat(points[i]);
                }

                return pointsList.ToArray();
            }
        }

        public bool IsBlank
        {
            get { return points == null || points.Count() == 0 || !(points.Where(p => p.Any()).Any()); }
        }

        /// <summary>
		/// Gets or sets the color of the strokes for the signature.
		/// </summary>
		/// <value>The color of the stroke.</value>
        public SolidColorBrush Stroke
        {
            get; set;
        }

        Color strokeColor;
        public Color StrokeColor
        {
            get { return strokeColor; }
            set
            {
                strokeColor = value;
                if (Stroke != null)
                    Stroke.Color = strokeColor;
                if (!IsBlank)
                    image.Source = GetImage(false);
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

        /// <summary>
        /// Gets or sets the width in pixels of the strokes for the signature.
        /// </summary>
        /// <value>The width of the line.</value>
        public float StrokeWidth
        {
            get { return (float)GetValue(StrokeWidthProperty); }
            set { SetValue(StrokeWidthProperty, (int)value); }
        }

        public static readonly DependencyProperty StrokeWidthProperty =
            DependencyProperty.Register("StrokeWidth", typeof(int), typeof(SignaturePad), new PropertyMetadata(2));
        
        /// <summary>
        /// The caption displayed under the signature line.
        /// </summary>
        /// <remarks>
        /// Text value defaults to 'Sign here.'
        /// </remarks>
        /// <value>The caption.</value>
        public TextBlock Caption
        {
            get { return caption; }
        }

        public string CaptionText
        {
            get { return caption.Text; }
            set { caption.Text = value; }
        }

        public TextBlock ClearLabel
        {
            get { return clearText; }
        }

        public string ClearLabelText
        {
            get { return clearText.Text; }
            set { clearText.Text = value; }
        }

        /// <summary>
        /// The prompt displayed at the beginning of the signature line.
        /// </summary>
        /// <remarks>
        /// Text value defaults to 'X'.
        /// </remarks>
        /// <value>The signature prompt.</value>
        public TextBlock SignaturePrompt
        {
            get { return signaturePrompt; }
        }

        public string SignaturePromptText
        {
            get { return signaturePrompt.Text; }
            set { signaturePrompt.Text = value; }
        }

        /// <summary>
        /// The color of the signature line.
        /// </summary>
        /// <value>The color of the signature line.</value>
        protected Color signatureLineColor;

        public Color SignatureLineColor
        {
            get { return signatureLineColor; }
            set
            {
                signatureLineColor = value;
                signatureLine.BorderBrush = new SolidColorBrush(value);
            }
        }

        public SignaturePad()
        {
            InitializeComponent();
            Initialize();
        }

        /// <summary>
        /// Fires when SignatureBox has been loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void inkPresenter_Loaded(object sender, RoutedEventArgs e)
        {
            var background = base.Background ?? new SolidColorBrush(Colors.White);
            this.Background = background;
        }

        void Initialize()
        {
            points = new List<List<Point>>();
            Stroke = new SolidColorBrush(Colors.White);
            backgroundColor = Colors.Black;
            LayoutRoot.Background = new SolidColorBrush(backgroundColor);
            StrokeWidth = 3;
            SizeChanged += SignaturePad_SizeChanged;
        }

        private void SignaturePad_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            image.Source = GetImage(false);
        }

        //Delete the current signature
        public void Clear()
        {
            var lines = new List<Line>();

            foreach (var child in this.inkPresenter.Children)
            {
                if (child.GetType() == typeof(Line))
                {
                    lines.Add(child as Line);
                }
            }

            foreach (var line in lines)
                inkPresenter.Children.Remove(line);

            points = new List<List<Point>>(); //Reset Points

            clearText.Visibility = Visibility.Collapsed;
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            Clear();
        }



        #region GetImage
        //Create a WriteableBitmap of the currently drawn signature with default colors.
        public WriteableBitmap GetImage(bool shouldCrop = true, bool keepAspectRatio = true)
        {
            return GetImage(Stroke.Color, Colors.Transparent, RenderSize, 1, shouldCrop, keepAspectRatio);
        }

        public WriteableBitmap GetImage(Size size, bool shouldCrop = true, bool keepAspectRatio = true)
        {
            return GetImage(Stroke.Color, Colors.Transparent, size, getScaleFromSize(size, RenderSize), shouldCrop, keepAspectRatio);
        }

        public WriteableBitmap GetImage(float scale, bool shouldCrop = true, bool keepAspectRatio = true)
        {
            return GetImage(Stroke.Color, Colors.Transparent, getSizeFromScale(scale, RenderSize), scale, shouldCrop, keepAspectRatio);
        }

        //Create a WriteableBitmap of the currently drawn signature with the specified Stroke color.
        public WriteableBitmap GetImage(Color strokeColor, bool shouldCrop = true, bool keepAspectRatio = true)
        {
            return GetImage(strokeColor, Colors.Transparent, RenderSize, 1, shouldCrop, keepAspectRatio);
        }

        public WriteableBitmap GetImage(Color strokeColor, Size size, bool shouldCrop = true, bool keepAspectRatio = true)
        {
            return GetImage(strokeColor, Colors.Transparent, size, getScaleFromSize(size, RenderSize), shouldCrop, keepAspectRatio);
        }

        public WriteableBitmap GetImage(Color strokeColor, float scale, bool shouldCrop = true, bool keepAspectRatio = true)
        {
            return GetImage(strokeColor, Colors.Transparent, getSizeFromScale(scale, RenderSize), scale, shouldCrop, keepAspectRatio);
        }

        //Create a WriteableBitmap of the currently drawn signature with the specified Stroke and Fill colors.
        public WriteableBitmap GetImage(Color strokeColor, Color fillColor, bool shouldCrop = true, bool keepAspectRatio = true)
        {
            return GetImage(strokeColor, fillColor, RenderSize, 1, shouldCrop, keepAspectRatio);
        }

        public WriteableBitmap GetImage(Color strokeColor, Color fillColor, Size size, bool shouldCrop = true, bool keepAspectRatio = true)
        {
            return GetImage(strokeColor, fillColor, size, getScaleFromSize(size, RenderSize), shouldCrop, keepAspectRatio);
        }

        public WriteableBitmap GetImage(Color strokeColor, Color fillColor, float scale, bool shouldCrop = true, bool keepAspectRatio = true)
        {
            return GetImage(strokeColor, fillColor, getSizeFromScale(scale, RenderSize), scale, shouldCrop, keepAspectRatio);
        }

        WriteableBitmap GetImage(Color strokeColor, Color fillColor, Size size, float scale, bool shouldCrop = true, bool keepAspectRatio = true)
        {
            if (size.Width == 0 || size.Height == 0 || scale <= 0 || strokeColor == null || fillColor == null)
                return null;

            var imageTask = Task.Run(async () =>
            {
                Rect croppedRectangle = new Rect();

                Point[] cachedPoints;

                if (shouldCrop && (cachedPoints = Points).Any())
                {
                    croppedRectangle = getCroppedRectangle(cachedPoints);

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
                }

                //Create new Random Access Tream
                var stream = new InMemoryRandomAccessStream();

                //Create bitmap encoder
                var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);

                //Set the pixel data and flush it
                encoder.SetPixelData(
                            BitmapPixelFormat.Bgra8,
                            BitmapAlphaMode.Straight,
                            (uint)bitmapInfo.PixelWidth,
                            (uint)bitmapInfo.PixelHeight, 96d, 96d,
                            bitmapInfo.BitmapBuffer.ToArray());
                await encoder.FlushAsync();
                stream.Seek(0);

                var bitmap = new WriteableBitmap((int)bitmapInfo.PixelWidth, (int)bitmapInfo.PixelHeight);

                if (shouldCrop)
                {
                    // cropping on UWP: https://social.msdn.microsoft.com/Forums/en-US/00779641-d6d2-42af-b368-8b32f2c702cc/writeablebitmap-resizing?forum=winappswithcsharp
                    // At this point we have an encoded image in inMemoryRandomStream
                    // We apply the transform and decode
                    var width = (uint) croppedRectangle.Width;
                    var height = (uint) croppedRectangle.Height;
                    var transform = new BitmapTransform
                    {
                        ScaledWidth = width,
                        ScaledHeight = height,
                    };
                    var decoder = await BitmapDecoder.CreateAsync(stream);
                    var pixelData = await decoder.GetPixelDataAsync(
                                    BitmapPixelFormat.Rgba8,
                                    BitmapAlphaMode.Straight,
                                    transform,
                                    ExifOrientationMode.IgnoreExifOrientation,
                                    ColorManagementMode.DoNotColorManage);
                    //An array containing the decoded image data
                    var sourceDecodedPixels = pixelData.DetachPixelData();

                    // Approach 1 : Encoding the image buffer again:
                    //Encoding data
                    var inMemoryRandomStream2 = new InMemoryRandomAccessStream();
                    var encoder2 = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, inMemoryRandomStream2);
                    encoder2.SetPixelData(BitmapPixelFormat.Rgba8, BitmapAlphaMode.Ignore, width, height, 96, 96, sourceDecodedPixels);
                    await encoder2.FlushAsync();
                    inMemoryRandomStream2.Seek(0);

                    // finally the resized writablebitmap
                    bitmap = new WriteableBitmap((int)width, (int)height);
                    await bitmap.SetSourceAsync(inMemoryRandomStream2);
                    return bitmap;
                }

                // finally the resized writablebitmap
                await bitmap.SetSourceAsync(stream);
                return bitmap;
            });
            imageTask.Wait();
            return imageTask.Result;
        }
        #endregion

        Rect getCroppedRectangle(Point[] cachedPoints)
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

        float getScaleFromSize(Size size, Size original)
        {
            double scaleX = size.Width / original.Width;
            double scaleY = size.Height / original.Height;

            return (float)Math.Min(scaleX, scaleY);
        }

        Size getSizeFromScale(float scale, Size original)
        {
            double width = original.Width * scale;
            double height = original.Height * scale;

            return new Size(width, height);
        }


        #region Touch Events
        /// <summary>
        /// Handles the pointer pressed event. Sets the previousPosition to the 
        /// current position of the pointer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void inkPresenter_OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            // Get information about the pointer location. 
            previousPosition = e.GetCurrentPoint(this).Position;
            pressed = true;
        }

        /// <summary>
        /// Handles the pointer moved event. If the pointer is pressed and has moved it will 
        /// draw the line on the canvas and add the points drawn to the points list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void inkPresenter_OnPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (!pressed) return;

            var positions = e.GetIntermediatePoints(this).Select(ppt => ppt.Position);
            var currentPosition = e.GetCurrentPoint(this).Position;

            //Only add points if previous and current positions are different
            if (positions.Any() &&
                currentPosition.X != previousPosition.X &&
                currentPosition.Y != previousPosition.Y)
            {
                points.Add(new List<Point>());
                points.Last().Add(previousPosition);

                foreach (Point pt in positions)
                {
                    this.inkPresenter.Children.Add(
                      new Line()
                      {
                          X1 = previousPosition.X,
                          Y1 = previousPosition.Y,
                          X2 = pt.X,
                          Y2 = pt.Y,
                          Stroke = this.Stroke,
                          StrokeThickness = this.StrokeWidth
                      }
                    );
                    previousPosition = pt;
                }
                points.Last().AddRange(positions);
            }
        }

        /// <summary>
        /// Handles the PointerReleased Event. Toggled the Visibility of the clearText TextBlock
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void inkPresenter_OnPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (pressed)
                pressed = false;

            if (inkPresenter.Children.Any())
            {
                clearText.Visibility = Visibility.Visible;
            }

            UpdateBitmapBuffer(); //Update the BitMapBuffer model with the current state of the canvas
        }
        #endregion

        //Allow the user to import an array of points to be used to draw a signature in the view, with new
        //lines indicated by a PointF.Empty in the array.
        public void LoadPoints(Point[] loadedPoints)
        {
            if (!loadedPoints.Any())
            {
                return;
            }

            //Clear any existing paths or points.
            points = new List<List<Point>>();

            foreach (Point pt in loadedPoints)
            {
                this.inkPresenter.Children.Add(
                  new Line()
                  {
                      X1 = previousPosition.X,
                      Y1 = previousPosition.Y,
                      X2 = pt.X,
                      Y2 = pt.Y,
                      Stroke = this.Stroke,
                      StrokeThickness = this.StrokeWidth
                  }
                );
                previousPosition = pt;
            }

            points.Last().AddRange(loadedPoints);
            ////Obtain the image for the imported signature and display it in the image view.
            //image.Source = GetImage (false);
            ////Display the clear button.
            clearText.Visibility = Visibility.Visible;

            UpdateBitmapBuffer(); //Update the BitmapBuffer
        }
        
        /// <summary>
        /// Updates the BitmapBuffer object with the current state of the canvas
        /// </summary>
        public void UpdateBitmapBuffer()
        {
            Task.Run(() =>
            {
                var task = this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                {
                    Size canvasSize = this.inkPresenter.RenderSize;
                    Point defaultPoint = this.inkPresenter.RenderTransformOrigin;

                    this.inkPresenter.Measure(canvasSize);
                    this.inkPresenter.UpdateLayout();
                    this.inkPresenter.Arrange(new Rect(defaultPoint, canvasSize));

                    // Convert canvas to bmp.  
                    var renderTargetBitmap = new RenderTargetBitmap();
                    await renderTargetBitmap.RenderAsync(this.inkPresenter);

                    //Fetch the Pixel Buffer fromt the bitmap
                    bitmapInfo.BitmapBuffer = await renderTargetBitmap.GetPixelsAsync();
                    bitmapInfo.PixelWidth = renderTargetBitmap.PixelWidth;
                    bitmapInfo.PixelHeight = renderTargetBitmap.PixelHeight;
                }).AsTask();
                task.Wait();
            });
        }
        
        /// <summary>
        /// Class to contain the Current State of the Canvas Bitmap
        /// </summary>
        private class BitmapInfo
        {
            public IBuffer BitmapBuffer
            {
                get; set;
            }
            public int PixelWidth
            {
                get; set;
            }
            public int PixelHeight
            {
                get; set;
            }
        }
    }
}
