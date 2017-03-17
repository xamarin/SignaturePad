using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Graphics;

namespace SignaturePad {
	public class SignaturePadView : RelativeLayout {
		SignatureCanvasView canvasView;
		ClearingImageView imageView;

		Context context;
		Paint _paint;
		Paint paint { get { return _paint; } set { canvasView.Paint = _paint = value; } }

		Path _currentPath;
		Path currentPath { get { return _currentPath; } set { canvasView.Path = _currentPath = value; } }
		List<Path> paths;
		List<System.Drawing.PointF> currentPoints;
		List<System.Drawing.PointF []> points;

		float lastX;
		float lastY;

		//Used to determine rectangle that needs to be redrawn.
		RectF dirtyRect;

        public event EventHandler<bool> IsBlankChanged;

        //Create an array containing all of the points used to draw the signature.  Uses null
        //to indicate a new line.
        public System.Drawing.PointF[] Points {
			get { 
				if (points == null || points.Count () == 0)
					return new System.Drawing.PointF [0];

				IEnumerable<System.Drawing.PointF> pointsList = points[0];

				for (var i = 1; i < points.Count; i++) {
					pointsList = pointsList.Concat (new [] { System.Drawing.PointF.Empty });
					pointsList = pointsList.Concat (points [i]);
				}

				return pointsList.ToArray (); 
			}
		}

		public bool IsBlank {
			get { 
				return points == null || points.Count () == 0 || !(points.Where (p => p.Any ()).Any ());
			}
		}

		/// <summary>
		/// Gets or sets the color of the strokes for the signature.
		/// </summary>
		/// <value>The color of the stroke.</value>
		Color strokeColor;
		public Color StrokeColor {
			get { return strokeColor; }
			set {
				strokeColor = value;

				if (paint != null)
					paint.Color = strokeColor;

				if (!IsBlank)
					DrawStrokes ();
			}
		}

		Color backgroundColor;
		public Color BackgroundColor {
			get { return backgroundColor; }
			set {
				backgroundColor = value;
				SetBackgroundColor (backgroundColor);
			}
		}

		/// <summary>
		/// Gets or sets the width in pixels of the strokes for the signature.
		/// </summary>
		/// <value>The width of the line.</value>
		float strokeWidth;
		public float StrokeWidth {
			get { return strokeWidth; }
			set {
				strokeWidth = value;
				if (paint != null)
					paint.StrokeWidth = strokeWidth;

				if (!IsBlank)
					DrawStrokes ();
			}
		}

		/// <summary>
		/// The text for the prompt displayed at the beginning of the signature line.
		/// </summary>
		/// <remarks>
		/// Text value defaults to 'X'.
		/// </remarks>
		/// <value>The signature prompt.</value>
		public string SignaturePromptText {get { return SignaturePrompt.Text; }
			set { SignaturePrompt.Text = value; }
		}

		/// <summary>
		/// The text for the caption displayed under the signature line.
		/// </summary>
		/// <remarks>
		/// Text value defaults to 'Sign here.'
		/// </remarks>
		/// <value>The caption.</value>
		public string CaptionText {
			get { return Caption.Text; }
			set { Caption.Text = value; }
		}

		/// <summary>
		/// Gets the label that clears the pad when clicked.
		/// </summary>
		/// <value>The clear label.</value>
		public string ClearLabelText {
			get { return ClearLabel.Text; }
			set { ClearLabel.Text = value; }
		}

		/// <summary>
		/// The prompt displayed at the beginning of the signature line.
		/// </summary>
		/// <remarks>
		/// Text value defaults to 'X'.
		/// </remarks>
		/// <value>The signature prompt.</value>
		public TextView SignaturePrompt {
			get;
			private set;
		}

		/// <summary>
		/// The caption displayed under the signature line.
		/// </summary>
		/// <remarks>
		/// Text value defaults to 'Sign here.'
		/// </remarks>
		/// <value>The caption.</value>
		public TextView Caption {
			get;
			private set;
		}

		/// <summary>
		/// The color of the signature line.
		/// </summary>
		/// <value>The color of the signature line.</value>
		protected Color signatureLineColor;
		public Color SignatureLineColor {
			get { return signatureLineColor; }
			set { 
				signatureLineColor = value; 
				SignatureLine.SetBackgroundColor (value);
			}
		}

		/// <summary>
		/// Gets the background image view.
		/// </summary>
		/// <value>The background image view.</value>
		public ImageView BackgroundImageView { get; private set; }

		/// <summary>
		/// Gets the label that clears the pad when clicked.
		/// </summary>
		/// <value>The clear label.</value>
		public TextView ClearLabel {
			get;
			private set;
		}

		/// <summary>
		/// Gets the horizontal line that goes in the lower part of the pad.
		/// </summary>
		/// <value>The signature line.</value>
		public View SignatureLine {
			get;
			private set;
		}

		public SignaturePadView (Context context) : base (context)
		{
			this.context = context;
			Initialize ();
		}

		public SignaturePadView (Context context, IAttributeSet attrs) : base (context, attrs)
		{
			this.context = context;
			Initialize ();
		}

		public SignaturePadView (Context context, IAttributeSet attrs, int defStyle) :
			base (context, attrs, defStyle)
		{
			this.context = context;
			Initialize ();
		}

		static Random rndId = new Random ();
		protected int generateId ()
		{
			int id;
			for (;;) {
				id = rndId.Next (1, 0x00FFFFFF);
				if (FindViewById<View> (id) != null) {
					continue;
				}
				return id;
			}
		}

		void Initialize ()
		{
			BackgroundColor = Color.Black;
			strokeColor = Color.White;
			StrokeWidth = 2f;

			canvasView = new SignatureCanvasView (this.context);
			canvasView.LayoutParameters = new RelativeLayout.LayoutParams (RelativeLayout.LayoutParams.FillParent, RelativeLayout.LayoutParams.FillParent);

			//Set the attributes for painting the lines on the screen.
			paint = new Paint ();
			paint.Color = strokeColor;
			paint.StrokeWidth = StrokeWidth;
			paint.SetStyle (Paint.Style.Stroke);
			paint.StrokeJoin = Paint.Join.Round;
			paint.StrokeCap = Paint.Cap.Round;
			paint.AntiAlias = true;

			#region Add Subviews
			RelativeLayout.LayoutParams layout;

			BackgroundImageView = new ImageView (this.context);
			BackgroundImageView.Id = generateId ();
			AddView (BackgroundImageView);

			//Add an image that covers the entire signature view, used to display already drawn
			//elements instead of having to redraw them every time the user touches the screen.
			imageView = new ClearingImageView (context);
			imageView.SetBackgroundColor (Color.Transparent);
			imageView.LayoutParameters = new RelativeLayout.LayoutParams (RelativeLayout.LayoutParams.FillParent, RelativeLayout.LayoutParams.FillParent);
			AddView (imageView);

			Caption = new TextView (context);
			Caption.Id = generateId ();
			Caption.SetIncludeFontPadding (true);
			Caption.Text = "Sign Here";
			layout = new RelativeLayout.LayoutParams (RelativeLayout.LayoutParams.WrapContent, RelativeLayout.LayoutParams.WrapContent);
			layout.AlignWithParent = true;
			layout.BottomMargin = 6;
			layout.AddRule (LayoutRules.AlignBottom);
			layout.AddRule (LayoutRules.CenterHorizontal);
			Caption.LayoutParameters = layout;
			Caption.SetPadding (0, 0, 0, 6);
			AddView (Caption);

			//Display the base line for the user to sign on.
			SignatureLine = new View (context);
			SignatureLine.Id = generateId ();
			SignatureLine.SetBackgroundColor (Color.Gray);
			layout = new RelativeLayout.LayoutParams (RelativeLayout.LayoutParams.MatchParent, 1);
			layout.SetMargins (10, 0, 10, 5);
			layout.AddRule (LayoutRules.Above, Caption.Id);
            SignatureLine.LayoutParameters = layout;
			AddView (SignatureLine);

			//Display the X on the left hand side of the line where the user signs.
			SignaturePrompt = new TextView (context);
			SignaturePrompt.Id = generateId ();
			SignaturePrompt.Text = "X";
			SignaturePrompt.SetTypeface (null, TypefaceStyle.Bold);
			layout = new RelativeLayout.LayoutParams (RelativeLayout.LayoutParams.WrapContent, RelativeLayout.LayoutParams.WrapContent);
			layout.LeftMargin = 11;
			layout.AddRule (LayoutRules.Above, SignatureLine.Id);
			SignaturePrompt.LayoutParameters = layout;
			AddView (SignaturePrompt);

			AddView (canvasView);

			ClearLabel = new TextView (context);
			ClearLabel.Id = generateId ();
			ClearLabel.Text = "Clear";
			layout = new RelativeLayout.LayoutParams (RelativeLayout.LayoutParams.WrapContent, RelativeLayout.LayoutParams.WrapContent);
			layout.SetMargins (0, 10, 22, 0);
			layout.AlignWithParent = true;
			layout.AddRule (LayoutRules.AlignRight);
			layout.AddRule (LayoutRules.AlignTop);
			ClearLabel.LayoutParameters = layout;
			ClearLabel.Visibility = ViewStates.Invisible;
			ClearLabel.Click += (object sender, EventArgs e) => {
				Clear ();
			};
			AddView (ClearLabel);
			#endregion

			paths = new List<Path> ();
			points = new List<System.Drawing.PointF[]> ();
			currentPoints = new List<System.Drawing.PointF> ();

			dirtyRect = new RectF ();
		}

		//Delete the current signature.
		public void Clear ()
		{
		    var wasBlank = IsBlank;
			paths = new List<Path> ();
			points = new List<System.Drawing.PointF[]> ();
			currentPoints = new List<System.Drawing.PointF> ();
			currentPath = new Path ();
			imageView.SetImageBitmap (null);
			ClearLabel.Visibility = ViewStates.Invisible;
			GC.Collect ();

			canvasView.Invalidate ();
			Invalidate ();

            if(wasBlank != IsBlank)
                OnIsBlankChanged(IsBlank);
        }

		//Create a UIImage of the currently drawn signature.
		public Bitmap GetImage (bool shouldCrop = true, bool keepAspectRatio = true)
		{
			return GetImage (strokeColor, Color.Transparent, new System.Drawing.SizeF (Width, Height), 1,
			                 shouldCrop, keepAspectRatio);
		}

		public Bitmap GetImage (System.Drawing.SizeF size, bool shouldCrop = true, bool keepAspectRatio = true)
		{
			return GetImage (strokeColor, Color.Transparent, size, 
			                 getScaleFromSize (size, Width, Height), shouldCrop, keepAspectRatio);
		}

		public Bitmap GetImage (float scale, bool shouldCrop = true, bool keepAspectRatio = true)
		{
			return GetImage (strokeColor, Color.Transparent, getSizeFromScale (scale, Width, Height), 
			                 scale, shouldCrop, keepAspectRatio);
		}

		//Create a UIImage of the currently drawn signature with the specified Stroke color.
		public Bitmap GetImage (Color strokeColor, bool shouldCrop = true, bool keepAspectRatio = true)
		{
			return GetImage (strokeColor, Color.Transparent, new System.Drawing.SizeF (Width, Height), 1,
			                 shouldCrop, keepAspectRatio);
		}

		public Bitmap GetImage (Color strokeColor, System.Drawing.SizeF size, bool shouldCrop = true, bool keepAspectRatio = true)
		{
			return GetImage (strokeColor, Color.Transparent, size, getScaleFromSize (size, Width, Height), 
			                 shouldCrop, keepAspectRatio);
		}

		public Bitmap GetImage (Color strokeColor, float scale, bool shouldCrop = true, bool keepAspectRatio = true)
		{
			return GetImage (strokeColor, Color.Transparent, getSizeFromScale (scale, Width, Height), 
			                 scale, shouldCrop, keepAspectRatio);
		}

		//Create a UIImage of the currently drawn signature with the specified Stroke and Fill colors.
		public Bitmap GetImage (Color strokeColor, Color fillColor, bool shouldCrop = true, bool keepAspectRatio = true)
		{
			return GetImage (strokeColor, fillColor, new System.Drawing.SizeF (Width, Height), 1, 
			                 shouldCrop, keepAspectRatio);
		}

		public Bitmap GetImage (Color strokeColor, Color fillColor, System.Drawing.SizeF size, 
		                        bool shouldCrop = true, bool keepAspectRatio = true)
		{
			return GetImage (strokeColor, fillColor, size, getScaleFromSize (size, Width, Height), 
			                 shouldCrop, keepAspectRatio);
		}

		public Bitmap GetImage (Color strokeColor, Color fillColor, float scale, bool shouldCrop = true, bool keepAspectRatio = true)
		{
			return GetImage (strokeColor, fillColor, getSizeFromScale (scale, Width, Height), 
			                 scale, shouldCrop, keepAspectRatio);
		}

		Bitmap GetImage (Color strokeColor, Color fillColor, System.Drawing.SizeF size, float scale, 
		                 bool shouldCrop = true, bool keepAspectRatio = true)
		{
			if (size.Width == 0 || size.Height == 0 || scale <= 0)
				return null;

			float uncroppedScale;
			RectF croppedRectangle = new RectF ();

			System.Drawing.PointF [] cachedPoints;

			if (shouldCrop && (cachedPoints = Points).Any ()) {
				croppedRectangle = getCroppedRectangle (cachedPoints);

				if (croppedRectangle.Left >= 5)
					croppedRectangle.Left -= 5;
				if (croppedRectangle.Right <= size.Width - 5)
					croppedRectangle.Right += 5;
				if (croppedRectangle.Top >= 5)
					croppedRectangle.Top -= 5;
				if (croppedRectangle.Bottom <= size.Height - 5)
					croppedRectangle.Bottom += 5;

				float scaleX = (croppedRectangle.Right - croppedRectangle.Left) / Width;
				float scaleY = (croppedRectangle.Bottom - croppedRectangle.Top) / Height;
				uncroppedScale = 1 / Math.Max (scaleX, scaleY);
			} else {
				uncroppedScale = scale;
			}

			Bitmap image;
			if (keepAspectRatio)
				image = Bitmap.CreateBitmap ((int)size.Width, (int)size.Height, 
					Bitmap.Config.Argb8888);
			else
				image = Bitmap.CreateBitmap ((int)(croppedRectangle.Width () * uncroppedScale), (int)(croppedRectangle.Height () * uncroppedScale),
					Bitmap.Config.Argb8888);
			Canvas canvas = new Canvas (image);
			canvas.Scale (uncroppedScale, uncroppedScale);

			DrawStrokesOnCanvas (canvas, strokeColor, fillColor, shouldCrop, croppedRectangle);

			return image;
		}

		private void DrawStrokesOnCanvas (Canvas canvas, Color strokeColor, Color fillColor, bool shouldCrop, RectF croppedRectangle = null) {
			canvas.DrawColor (fillColor);

			paint.Color = strokeColor;
			foreach (var path in paths) {
				var tempPath = path;

				if (shouldCrop) {
					tempPath = new Path (path);

					var translate = new Matrix ();
					translate.SetTranslate (-croppedRectangle.Left, -croppedRectangle.Top);
					tempPath.Transform (translate);
				}
				canvas.DrawPath (tempPath, paint);

				tempPath = null;
			}
			paint.Color = this.strokeColor;
		}

		// Bitmap buffer off by default since memory is a limited resource.
		private bool _useBitmapBuffer = false;
		public bool UseBitmapBuffer {
			get { return _useBitmapBuffer; }
			set {
				_useBitmapBuffer = value;
				if (_useBitmapBuffer) {
					DrawStrokes ();
				} else {
					imageView.SetImageBitmap (null);
				}
			}
		}

		private void DrawStrokes ()
		{
			if (UseBitmapBuffer) {
				//Get an image of the current signature and display it so that the entire set of paths
				//doesn't have to be redrawn every time.
				this.imageView.SetImageBitmap (this.GetImage (false));
			} else {
				Invalidate ();
			}
		}

		public override void Draw (Canvas canvas)
		{
			base.Draw (canvas);

			if (!UseBitmapBuffer) {
				//Bitmap not in use: redraw all of the paths.
				DrawStrokesOnCanvas (canvas, this.strokeColor, Color.Transparent, false);
			}
		}

		RectF getCroppedRectangle(System.Drawing.PointF [] cachedPoints)
		{
			var xMin = cachedPoints.Where (point => !point.IsEmpty).Min (point => point.X) - strokeWidth / 2;
			var xMax = cachedPoints.Where (point => !point.IsEmpty).Max (point => point.X) + strokeWidth / 2;
			var yMin = cachedPoints.Where (point => !point.IsEmpty).Min (point => point.Y) - strokeWidth / 2;
			var yMax = cachedPoints.Where (point => !point.IsEmpty).Max (point => point.Y) + strokeWidth / 2;

			xMin = Math.Max (xMin, 0);
			xMax = Math.Min (xMax, Width);
			yMin = Math.Max (yMin, 0);
			yMax = Math.Min (yMax, Height);

			return new RectF (xMin, yMin, xMax, yMax);
		}

		float getScaleFromSize (System.Drawing.SizeF size, float width, float height)
		{
			float scaleX = size.Width / width;
			float scaleY = size.Height / height;
			
			return Math.Min (scaleX, scaleY);
		}
		
		System.Drawing.SizeF getSizeFromScale (float scale, float inWidth, float inHeight)
		{
			float width = inWidth * scale;
			float height = inHeight * scale;
			
			return new System.Drawing.SizeF (width, height);
		}

		//Allow the user to import an array of points to be used to draw a signature in the view, with new
		//lines indicated by a System.Drawing.PointF.Empty in the array.
		public void LoadPoints (System.Drawing.PointF[] loadedPoints)
		{
			if (loadedPoints == null || loadedPoints.Count () == 0)
				return;
			
			var startIndex = 0;
			var emptyIndex = loadedPoints.ToList ().IndexOf (System.Drawing.PointF.Empty);
			
			if (emptyIndex == -1)
				emptyIndex = loadedPoints.Count ();
			
			//Clear any existing paths or points.
			paths = new List<Path> ();
			points = new List<System.Drawing.PointF[]> ();
			
			do {
				//Create a new path and set the line options
				currentPath = new Path ();
				currentPoints = new List<System.Drawing.PointF> ();
				
				//Move to the first point and add that point to the current_points array.
				currentPath.MoveTo (loadedPoints [startIndex].X, loadedPoints [startIndex].Y);
				currentPoints.Add (loadedPoints [startIndex]);
				
				//Iterate through the array until an empty point (or the end of the array) is reached,
				//adding each point to the current_path and to the current_points array.
				for (var i = startIndex + 1; i < emptyIndex; i++) {
					currentPath.LineTo (loadedPoints [i].X, loadedPoints [i].Y);
					currentPoints.Add (loadedPoints [i]);
				}
				
				//Add the current_path and current_points list to their respective Lists before
				//starting on the next line to be drawn.
				paths.Add (currentPath);
				points.Add (currentPoints.ToArray ());
				
				//Obtain the indices for the next line to be drawn.
				startIndex = emptyIndex + 1;
				if (startIndex < loadedPoints.Count () - 1) {
					emptyIndex = loadedPoints.ToList ().IndexOf (System.Drawing.PointF.Empty, 
					                                             startIndex);
					
					if (emptyIndex == -1)
						emptyIndex = loadedPoints.Count ();
				} else
					emptyIndex = startIndex;
			} while (startIndex < emptyIndex);
			
			DrawStrokes ();

			//Display the clear button.
			ClearLabel.Visibility = ViewStates.Visible; 
			Invalidate ();

            OnIsBlankChanged(IsBlank);
		}

		//Update the bounds for the rectangle to be redrawn if necessary for the given point.
		void updateBounds (float touchX, float touchY)
		{
			if (touchX < dirtyRect.Left)
				dirtyRect.Left = touchX;
			else if (touchX > dirtyRect.Right)
				dirtyRect.Right = touchX;
			
			if (touchY < dirtyRect.Top)
				dirtyRect.Top = touchY;
			else if (touchY > dirtyRect.Bottom)
				dirtyRect.Bottom = touchY;
		}

		//Set the bounds for the rectangle that will need to be redrawn to show the drawn path.
		void resetBounds (float touchX, float touchY)
		{
			if (touchX < lastX)
				dirtyRect.Left = touchX;
			if (touchX > lastX)
				dirtyRect.Right = touchX;
			if (touchY < lastY)
				dirtyRect.Top = touchY;
			if (touchY > lastY)
				dirtyRect.Bottom = touchY;
		}

		public override bool OnTouchEvent (MotionEvent e)
		{

			float touchX = e.GetX ();
			float touchY = e.GetY ();

			System.Drawing.PointF touch = new System.Drawing.PointF (touchX, touchY);

			switch (e.Action) {
			case MotionEventActions.Down:
				lastX = touchX;
				lastY = touchY;

				//Create a new path and move to the touched point.
				currentPath = new Path();
				currentPath.MoveTo (touchX, touchY);

				//Clear the list of points then add the touched point
				currentPoints.Clear ();
				currentPoints.Add (touch);

				//Display the clear button
				ClearLabel.Visibility = ViewStates.Visible;
				return true;
			case MotionEventActions.Move:
				handleTouch (e);
				canvasView.Invalidate(
					(int) (dirtyRect.Left - 1),
					(int) (dirtyRect.Top - 1),
					(int) (dirtyRect.Right + 1),
					(int) (dirtyRect.Bottom + 1));
				break;
			case MotionEventActions.Up:
			    var wasBlank = IsBlank;
				handleTouch (e);
				currentPath = smoothedPathWithGranularity (20, out currentPoints);

				//Add the current path and points to their respective lists.
				paths.Add (currentPath);
				points.Add (currentPoints.ToArray ());

				DrawStrokes ();
				canvasView.Invalidate ();

                if(wasBlank != IsBlank)
                    OnIsBlankChanged(IsBlank);

                break;
			default:
				return false;
			}
			
			lastX = touchX;
			lastY = touchY;
			
			return true;
		}

		//Iterate through the touch history since the last touch event and add them to the path and points list.
		void handleTouch (MotionEvent e)
		{
			float touchX = e.GetX ();
			float touchY = e.GetY ();

			System.Drawing.PointF touch = new System.Drawing.PointF (touchX, touchY);

			resetBounds (touchX, touchY);
			
			for (var i = 0; i < e.HistorySize; i++) {
				float historicalX = e.GetHistoricalX(i);
				float historicalY = e.GetHistoricalY(i);

				System.Drawing.PointF historical = new System.Drawing.PointF (historicalX, historicalY);

				updateBounds (historicalX, historicalY);

				currentPath.LineTo (historicalX, historicalY);
				currentPoints.Add (historical);
			}

			currentPath.LineTo (touchX, touchY);
			currentPoints.Add (touch);
		}

		Path smoothedPathWithGranularity (int granularity, out List<System.Drawing.PointF> smoothedPoints)
		{
			List<System.Drawing.PointF> pointsArray = currentPoints;
			smoothedPoints = new List<System.Drawing.PointF> ();

			//Not enough points to smooth effectively, so return the original path and points.
			if (pointsArray.Count < 4) {
				smoothedPoints = pointsArray;
				return currentPath;
			}

			//Create a new bezier path to hold the smoothed path.
			Path smoothedPath = new Path ();
	
			//Duplicate the first and last points as control points.
			pointsArray.Insert (0, pointsArray [0]);
			pointsArray.Add (pointsArray [pointsArray.Count - 1]);

			//Add the first point
			smoothedPath.MoveTo (pointsArray [0].X, pointsArray [0].Y);
			smoothedPoints.Add (pointsArray [0]);

			for (var index = 1; index < pointsArray.Count - 2; index++) {
				System.Drawing.PointF p0 = pointsArray [index - 1];
				System.Drawing.PointF p1 = pointsArray [index];
				System.Drawing.PointF p2 = pointsArray [index + 1];
				System.Drawing.PointF p3 = pointsArray [index + 2];

				//Add n points starting at p1 + dx/dy up until p2 using Catmull-Rom splines
				for (var i = 1; i < granularity; i++) {
					float t = (float)i * (1f / (float)granularity);
					float tt = t * t;
					float ttt = tt * t;

					//Intermediate point
					System.Drawing.PointF mid = new System.Drawing.PointF ();
					mid.X = 0.5f * (2f * p1.X + (p2.X - p0.X) * t + 
					                (2f * p0.X - 5f * p1.X + 4f * p2.X - p3.X) * tt + 
					                (3f * p1.X - p0.X - 3f * p2.X + p3.X) * ttt);
					mid.Y = 0.5f * (2 * p1.Y + (p2.Y - p0.Y) * t + 
					                (2 * p0.Y - 5 * p1.Y + 4 * p2.Y - p3.Y) * tt + 
					                (3 * p1.Y - p0.Y - 3 * p2.Y + p3.Y) * ttt);

					smoothedPath.LineTo (mid.X, mid.Y);
					smoothedPoints.Add (mid);
				}

				//Add p2
				smoothedPath.LineTo (p2.X, p2.Y);
				smoothedPoints.Add (p2);
			}

			//Add the last point
			System.Drawing.PointF last = pointsArray [pointsArray.Count - 1];
			smoothedPath.LineTo (last.X, last.Y);
			smoothedPoints.Add (last);

			return smoothedPath;
        }

        private void OnIsBlankChanged(bool isblank)
        {
            IsBlankChanged?.Invoke(this, isblank);
        }
    }
}

