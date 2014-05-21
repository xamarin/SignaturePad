//
// SignaturePadView.cs: UIView subclass for MonoTouch to allow users to draw their signature on the device
// 		     to be captured as an image or vector.
//
// Author:
//   Timothy Risi (timothy.risi@gmail.com)
//
// Copyright (C) 2012 Timothy Risi
//
using System;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using MonoTouch.CoreGraphics;
using MonoTouch.OpenGLES;
using MonoTouch.CoreImage;
using System.ComponentModel;

namespace SignaturePad {
	[Register("SignaturePadView")]
	public class SignaturePadView : UIView {
		#region UI Controls
		UIView signatureLine;
		UIButton btnClear;
		UIImageView imageView;
		#endregion

		UIBezierPath currentPath;
		List<UIBezierPath> paths;
		List<PointF> currentPoints;
		List<PointF[]> points;

		//Used to determine rectangle that needs to be redrawn.
		float minX, minY, maxX, maxY;

		//Create an array containing all of the points used to draw the signature.  Uses PointF.Empty
		//to indicate a new line.
		public PointF[] Points {
			get { 
				if (points == null || points.Count () == 0)
					return new PointF [0];

				List<PointF> pointsList = points [0].ToList ();

				for (var i = 1; i < points.Count; i++) {
					pointsList.Add (PointF.Empty);
					pointsList = pointsList.Concat (points [i]).ToList ();
				}

				return pointsList.ToArray (); 
			}
		}

		public bool IsBlank {
			get { return Points.Count () == 0; }
		}

		UIColor strokeColor;
		[Export, Browsable(true)]
		public UIColor StrokeColor {
			get { return strokeColor; }
			set {
				strokeColor = value ?? strokeColor;
				if (!IsBlank)
					imageView.Image = GetImage (false);
			}
		}

		float strokeWidth;
		[Export, Browsable(true)]
		public float StrokeWidth {
			get { return strokeWidth; }
			set {
				strokeWidth = value;
				if (!IsBlank)
					imageView.Image = GetImage (false);
			}
		}

		/// <summary>
		/// The prompt displayed at the beginning of the signature line.
		/// </summary>
		/// <remarks>
		/// Text value defaults to 'X'.
		/// </remarks>
		/// <value>The signature prompt.</value>
		[Export, Browsable(true)]
		public string SignaturePrompt {
			get { return SignaturePromptLabel.Text; }
			set { SignaturePromptLabel.Text = value; }
		}

		/// <summary>
		/// The native label for the SignaturePrompt.
		/// </summary>
		/// <remarks>
		/// Text value defaults to 'X'.
		/// </remarks>
		/// <value>The signature prompt label.</value>
		public UILabel SignaturePromptLabel { get; set; }

		/// <summary>
		/// The caption displayed under the signature line.
		/// </summary>
		/// <remarks>
		/// Text value defaults to 'Sign here.'
		/// </remarks>
		/// <value>The caption.</value>
		[Export, Browsable(true)]
		public string Caption { 
			get { return CaptionLabel.Text; }
			set { CaptionLabel.Text = value; }
		}

		/// <summary>
		/// The native label for the caption.
		/// </summary>
		/// <remarks>
		/// Text value defaults to 'Sign here.'
		/// </remarks>
		/// <value>The caption label.</value>
		public UILabel CaptionLabel { get; set; }

		/// <summary>
		/// The color of the signature line.
		/// </summary>
		/// <value>The color of the signature line.</value>
		[Export, Browsable(true)]
		public UIColor SignatureLineColor {
			get { return signatureLine.BackgroundColor; }
			set { signatureLine.BackgroundColor = value; }
		}

		/// <summary>
		///  An image view that may be used as a watermark or as a texture
		///  for the signature pad.
		/// </summary>
		/// <value>The background image view.</value>
		public UIImageView BackgroundImageView { get; private set; }

		/// <summary>
		/// Gets the label that clears the pad when clicked.
		/// </summary>
		/// <value>The clear label.</value>
		public UIButton ClearLabel {
			get { return btnClear; }
		}

		/// <summary>
		/// Gets the horizontal line that goes in the lower part of the pad.
		/// </summary>
		/// <value>The signature line.</value>
		public UIView SignatureLine {
			get { return signatureLine; }
		}

		public SignaturePadView ()
		{
			Initialize ();
		}

		public SignaturePadView (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		public SignaturePadView (IntPtr ptr) : base (ptr)
		{
			Initialize ();
		}

		public SignaturePadView (RectangleF frame)
		{
			Frame = frame;
			Initialize ();
		}

		void Initialize ()
		{
			BackgroundColor = UIColor.FromRGB(225, 225, 225);
			strokeColor = UIColor.Black;
			StrokeWidth = 2f;

			Layer.ShadowColor = UIColor.Black.CGColor;
			Layer.ShadowOffset = new SizeF (2, 2);
			Layer.ShadowOpacity = 1f;
			Layer.ShadowRadius = 2f;

			#region Add Subviews
			BackgroundImageView = new UIImageView ();
			AddSubview (BackgroundImageView);

			//Add an image that covers the entire signature view, used to display already drawn
			//elements instead of having to redraw them every time the user touches the screen.
			imageView = new UIImageView ();
			AddSubview (imageView);

			CaptionLabel = new UILabel ();
			CaptionLabel.Text = "Sign here.";
			CaptionLabel.Font = UIFont.BoldSystemFontOfSize (11f);
			CaptionLabel.BackgroundColor = UIColor.Clear;
			CaptionLabel.TextColor = UIColor.Gray;
			AddSubview (CaptionLabel);

			//Display the base line for the user to sign on.
			signatureLine = new UIView ();
			signatureLine.BackgroundColor = UIColor.Gray;
			AddSubview (signatureLine);

			//Display the X on the left hand side of the line where the user signs.
			SignaturePromptLabel = new UILabel ();
			SignaturePromptLabel.Text = "X";
			SignaturePromptLabel.Font = UIFont.BoldSystemFontOfSize (20f);
			SignaturePromptLabel.BackgroundColor = UIColor.Clear;
			SignaturePromptLabel.TextColor = UIColor.Gray;
			AddSubview (SignaturePromptLabel);

			btnClear = UIButton.FromType (UIButtonType.Custom);
			btnClear.SetTitle ("Clear", UIControlState.Normal);
			btnClear.Font = UIFont.BoldSystemFontOfSize (11f);
			btnClear.BackgroundColor = UIColor.Clear;
			btnClear.SetTitleColor (UIColor.Gray, UIControlState.Normal);
			//btn_clear.SetBackgroundImage (UIImage.FromFile ("Images/closebox.png"), UIControlState.Normal);
			//btn_clear.SetBackgroundImage (UIImage.FromFile ("Images/closebox_pressed.png"), 
			//                             UIControlState.Selected);
			btnClear.TouchUpInside += (sender, e) => {
				Clear ();
			};
			AddSubview (btnClear);
			btnClear.Hidden = true;
			#endregion

			paths = new List<UIBezierPath> ();
			points = new List<PointF[]> ();
			currentPoints = new List<PointF> ();
		}

		//Delete the current signature.
		public void Clear ()
		{
			paths = new List<UIBezierPath> ();
			currentPath = UIBezierPath.Create ();
			points = new List<PointF[]> ();
			currentPoints.Clear ();
			imageView.Image = null;
			btnClear.Hidden = true;

			SetNeedsDisplay ();
		}

		//Create a UIImage of the currently drawn signature with default colors.
		public UIImage GetImage (bool shouldCrop = true)
		{
			return GetImage (strokeColor, UIColor.Clear, 
			                 getSizeFromScale (UIScreen.MainScreen.Scale, Bounds), 
			                 UIScreen.MainScreen.Scale, shouldCrop);
		}
		
		public UIImage GetImage (SizeF size, bool shouldCrop = true)
		{
			return GetImage (strokeColor, UIColor.Clear, size, getScaleFromSize (size, Bounds), shouldCrop);
		}

		public UIImage GetImage (float scale, bool shouldCrop = true)
		{
			return GetImage (strokeColor, UIColor.Clear, getSizeFromScale(scale, Bounds), scale, shouldCrop);
		}

		//Create a UIImage of the currently drawn signature with the specified Stroke color.
		public UIImage GetImage (UIColor strokeColor, bool shouldCrop = true)
		{
			return GetImage (strokeColor, UIColor.Clear, 
			                 getSizeFromScale (UIScreen.MainScreen.Scale, Bounds), 
			                 UIScreen.MainScreen.Scale, shouldCrop);
		}
		
		public UIImage GetImage (UIColor strokeColor, SizeF size, bool shouldCrop = true)
		{
			return GetImage (strokeColor, UIColor.Clear, size, getScaleFromSize (size, Bounds), shouldCrop);
		}

		public UIImage GetImage (UIColor strokeColor, float scale, bool shouldCrop = true)
		{
			return GetImage (strokeColor, UIColor.Clear, getSizeFromScale(scale, Bounds), scale, shouldCrop);
		}

		//Create a UIImage of the currently drawn signature with the specified Stroke and Fill colors.
		public UIImage GetImage (UIColor strokeColor, UIColor fillColor, bool shouldCrop = true)
		{
			return GetImage (strokeColor, fillColor, 
			                 getSizeFromScale (UIScreen.MainScreen.Scale, Bounds),
			                 UIScreen.MainScreen.Scale, shouldCrop);
		}

		public UIImage GetImage (UIColor strokeColor, UIColor fillColor, SizeF size, bool shouldCrop = true)
		{
			return GetImage (strokeColor, fillColor, size, getScaleFromSize (size, Bounds), shouldCrop);
		}

		public UIImage GetImage (UIColor strokeColor, UIColor fillColor, float scale, bool shouldCrop = true)
		{
			return GetImage (strokeColor, fillColor, getSizeFromScale(scale, Bounds), scale, shouldCrop);
		}

		UIImage GetImage (UIColor strokeColor, UIColor fillColor, SizeF size, float scale, bool shouldCrop = true)
		{
			if (size.Width == 0 || size.Height == 0 || scale <= 0 || strokeColor == null ||
			    fillColor == null)
				return null;

			float uncroppedScale;
			SizeF uncroppedSize;
			RectangleF croppedRectangle;

			if (shouldCrop && Points.Count () > 0) {
				croppedRectangle = getCroppedRectangle ();
				float scaleX = croppedRectangle.Width / Bounds.Width;
				float scaleY = croppedRectangle.Height / Bounds.Height;
				uncroppedScale = 1 / Math.Max (scaleX, scaleY);
				//uncroppedScale = 1 / getScaleFromSize (croppedRectangle.Size, Bounds);
				uncroppedSize = getSizeFromScale (uncroppedScale, 
				                                  new RectangleF (new Point (0, 0), size));
			} else {
				uncroppedScale = scale;
				uncroppedSize = size;
			}

			//Make sure the image is scaled to the screen resolution in case of Retina display.
			UIGraphics.BeginImageContext (uncroppedSize);

			//Create context and set the desired options
			CGContext context = UIGraphics.GetCurrentContext ();
			context.SetFillColor (fillColor.CGColor);
			context.FillRect (new RectangleF (0, 0, uncroppedSize.Width, uncroppedSize.Height));
			context.SetStrokeColor (strokeColor.CGColor);
			context.SetLineWidth (StrokeWidth);
			context.SetLineCap (CGLineCap.Round);
			context.SetLineJoin (CGLineJoin.Round);
			context.ScaleCTM (uncroppedScale, uncroppedScale);

			//Obtain all drawn paths from the array
			foreach (var bezierPath in paths) {
				CGPath path = bezierPath.CGPath;
				context.AddPath (path);
			}
			context.StrokePath ();

			UIImage image = UIGraphics.GetImageFromCurrentImageContext ();

			if (shouldCrop && Points.Count () > 0) {
				croppedRectangle = getCroppedRectangle ();
				RectangleF scaledRectangle;
				scaledRectangle = new RectangleF (croppedRectangle.X * uncroppedScale, 
				                                  croppedRectangle.Y * uncroppedScale, 
				                                  size.Width, 
				                                  size.Height);
				if (scaledRectangle.X >= 5) {
					scaledRectangle.X -= 5;
					scaledRectangle.Width += 5;
				}
				if (scaledRectangle.Y >= 5) {
					scaledRectangle.Y -= 5;
					scaledRectangle.Height += 5;
				}
				if (scaledRectangle.X + scaledRectangle.Width <= uncroppedSize.Width - 5)
					scaledRectangle.Width += 5;
				if (scaledRectangle.Y + scaledRectangle.Height <= uncroppedSize.Height - 5)
					scaledRectangle.Height += 5;
				CGImage croppedImage = image.CGImage.WithImageInRect (scaledRectangle);

				image = new UIImage (croppedImage);
			}
			return image;
		}

		RectangleF getCroppedRectangle()
		{
			var xMin = Points.Where (point => !point.IsEmpty).Min (point => point.X) - strokeWidth / 2;
			var xMax = Points.Where (point => !point.IsEmpty).Max (point => point.X) + strokeWidth / 2;
			var yMin = Points.Where (point => !point.IsEmpty).Min (point => point.Y) - strokeWidth / 2;
			var yMax = Points.Where (point => !point.IsEmpty).Max (point => point.Y) + strokeWidth / 2;

			xMin = Math.Max (xMin, 0);
			xMax = Math.Min (xMax, Bounds.Width);
			yMin = Math.Max (yMin, 0);
			yMax = Math.Min (yMax, Bounds.Height);

			return new RectangleF (xMin, yMin, xMax - xMin, yMax - yMin);
		}

		float getScaleFromSize (SizeF size, RectangleF rectangle)
		{
			float scaleX = size.Width / rectangle.Width;
			float scaleY = size.Height / rectangle.Height;

			return Math.Min (scaleX, scaleY);
		}

		SizeF getSizeFromScale (float scale, RectangleF rectangle)
		{
			float width = rectangle.Width * scale;
			float height = rectangle.Height * scale;

			return new SizeF (width, height);
		}

		//Allow the user to import an array of points to be used to draw a signature in the view, with new
		//lines indicated by a PointF.Empty in the array.
		public void LoadPoints (PointF[] loadedPoints)
		{
			if (loadedPoints == null || loadedPoints.Count () == 0)
				return;

			var startIndex = 0;
			var emptyIndex = loadedPoints.ToList ().IndexOf (PointF.Empty);

			if (emptyIndex == -1)
				emptyIndex = loadedPoints.Count ();

			//Clear any existing paths or points.
			paths = new List<UIBezierPath> ();
			points = new List<PointF[]> ();

			do {
				//Create a new path and set the line options
				currentPath = UIBezierPath.Create ();
				currentPath.LineWidth = StrokeWidth;
				currentPath.LineJoinStyle = CGLineJoin.Round;

				currentPoints = new List<PointF> ();

				//Move to the first point and add that point to the current_points array.
				currentPath.MoveTo (loadedPoints [startIndex]);
				currentPoints.Add (loadedPoints [startIndex]);

				//Iterate through the array until an empty point (or the end of the array) is reached,
				//adding each point to the current_path and to the current_points array.
				for (var i = startIndex + 1; i < emptyIndex; i++) {
					currentPath.AddLineTo (loadedPoints [i]);
					currentPoints.Add (loadedPoints [i]);
				}

				//Add the current_path and current_points list to their respective Lists before
				//starting on the next line to be drawn.
				paths.Add (currentPath);
				points.Add (currentPoints.ToArray ());

				//Obtain the indices for the next line to be drawn.
				startIndex = emptyIndex + 1;
				if (startIndex < loadedPoints.Count () - 1) {
					emptyIndex = loadedPoints.ToList ().IndexOf (PointF.Empty, startIndex);

					if (emptyIndex == -1)
						emptyIndex = loadedPoints.Count ();
				} else
					emptyIndex = startIndex;
			} while (startIndex < emptyIndex);

			//Obtain the image for the imported signature and display it in the image view.
			imageView.Image = GetImage (false);
			//Display the clear button.
			btnClear.Hidden = false;
			SetNeedsDisplay ();
		}

		//Update the bounds for the rectangle to be redrawn if necessary for the given point.
		void updateBounds (PointF point)
		{
			if (point.X < minX + 1)
				minX = point.X - 1;
			if (point.X > maxX - 1)
				maxX = point.X + 1;
			if (point.Y < minY + 1)
				minY = point.Y - 1;
			if (point.Y > maxY - 1)
				maxY = point.Y + 1;
		}

		//Set the bounds for the rectangle that will need to be redrawn to show the drawn path.
		void resetBounds (PointF point)
		{
			minX = point.X - 1;
			maxX = point.X + 1;
			minY = point.Y - 1;
			maxY = point.Y + 1;
		}

		/*
		 *Obtain a smoothed path with the specified granularity from the current path using Catmull-Rom spline.  
		 *Implemented using a modified version of the code in the solution at 
		 *http://stackoverflow.com/questions/8702696/drawing-smooth-curves-methods-needed.
		 *Also outputs a List of the points corresponding to the smoothed path.
		 */
		UIBezierPath smoothedPathWithGranularity (int granularity, out List<PointF> smoothedPoints)
		{
			List<PointF> pointsArray = currentPoints;
			smoothedPoints = new List<PointF> ();

			//Not enough points to smooth effectively, so return the original path and points.
			if (pointsArray.Count < 4) {
				smoothedPoints = pointsArray;
				return currentPath;
			}

			//Create a new bezier path to hold the smoothed path.
			UIBezierPath smoothedPath = UIBezierPath.Create ();
			smoothedPath.LineWidth = StrokeWidth;
			smoothedPath.LineJoinStyle = CGLineJoin.Round;

			//Duplicate the first and last points as control points.
			pointsArray.Insert (0, pointsArray [0]);
			pointsArray.Add (pointsArray [pointsArray.Count - 1]);

			//Add the first point
			smoothedPath.MoveTo (pointsArray [0]);
			smoothedPoints.Add (pointsArray [0]);

			for (var index = 1; index < pointsArray.Count - 2; index++) {
				PointF p0 = pointsArray [index - 1];
				PointF p1 = pointsArray [index];
				PointF p2 = pointsArray [index + 1];
				PointF p3 = pointsArray [index + 2];

				//Add n points starting at p1 + dx/dy up until p2 using Catmull-Rom splines
				for (var i = 1; i < granularity; i++) {
					float t = (float)i * (1f / (float)granularity);
					float tt = t * t;
					float ttt = tt * t;

					//Intermediate point
					PointF mid = default(PointF);
					mid.X = 0.5f * (2f * p1.X + (p2.X - p0.X) * t + 
						(2f * p0.X - 5f * p1.X + 4f * p2.X - p3.X) * tt + 
						(3f * p1.X - p0.X - 3f * p2.X + p3.X) * ttt);
					mid.Y = 0.5f * (2 * p1.Y + (p2.Y - p0.Y) * t + 
						(2 * p0.Y - 5 * p1.Y + 4 * p2.Y - p3.Y) * tt + 
						(3 * p1.Y - p0.Y - 3 * p2.Y + p3.Y) * ttt);

					smoothedPath.AddLineTo (mid);
					smoothedPoints.Add (mid);
				}

				//Add p2
				smoothedPath.AddLineTo (p2);
				smoothedPoints.Add (p2);
			}

			//Add the last point
			smoothedPath.AddLineTo (pointsArray [pointsArray.Count - 1]);
			smoothedPoints.Add (pointsArray [pointsArray.Count - 1]);

			return smoothedPath;
		}
		
		#region Touch Events
		public override void TouchesBegan (MonoTouch.Foundation.NSSet touches, UIEvent evt)
		{
			//Create a new path and set the options.
			currentPath = UIBezierPath.Create ();
			currentPath.LineWidth = StrokeWidth;
			currentPath.LineJoinStyle = CGLineJoin.Round;
			
			currentPoints.Clear ();
			
			UITouch touch = touches.AnyObject as UITouch;
			
			//Obtain the location of the touch, move the path to that position and add it to the
			//current_points array.
			PointF touchLocation = touch.LocationInView (this);
			currentPath.MoveTo (touchLocation);
			currentPoints.Add (touchLocation);

			resetBounds (touchLocation);
			
			btnClear.Hidden = false;
		}
		
		public override void TouchesMoved (MonoTouch.Foundation.NSSet touches, UIEvent evt)
		{
			UITouch touch = touches.AnyObject as UITouch;
			
			//Obtain the location of the touch and add it to the current path and current_points array.
			PointF touchLocation = touch.LocationInView (this);
			currentPath.AddLineTo (touchLocation);
			currentPoints.Add (touchLocation);
			
			updateBounds (touchLocation);
			SetNeedsDisplayInRect (new RectangleF (minX, minY, 
			                                       Math.Abs (maxX - minX), 
			                                       Math.Abs (maxY - minY)));
		}
		
		public override void TouchesEnded (NSSet touches, UIEvent evt)
		{
			UITouch touch = touches.AnyObject as UITouch;
			
			//Obtain the location of the touch and add it to the current path and current_points array.
			PointF touchLocation = touch.LocationInView (this);
			currentPath.AddLineTo (touchLocation);
			currentPoints.Add (touchLocation);
			
			//Obtain the smoothed path and the points array for that path.
			currentPath = smoothedPathWithGranularity (40, out currentPoints);
			//Add the smoothed path and points array to their Lists.
			paths.Add (currentPath);
			points.Add (currentPoints.ToArray ());
			
			//Obtain the image for the imported signature and display it in the image view.
			imageView.Image = GetImage (false);
			updateBounds (touchLocation);
			SetNeedsDisplay ();
		}
		#endregion

		public override void Draw (RectangleF rect)
		{
			if (currentPath == null || currentPath.Empty)
				return;

			strokeColor.SetStroke ();
			currentPath.Stroke ();
		}

		public override void LayoutSubviews ()
		{
			CaptionLabel.SizeToFit ();
			SignaturePromptLabel.SizeToFit ();
			btnClear.SizeToFit ();
			var bottomOffset = Bounds.Height - Bounds.Height / 10;
			imageView.Frame = new RectangleF (0, 0, Bounds.Width, Bounds.Height);

			CaptionLabel.Frame = new RectangleF ((Bounds.Width / 2) - (CaptionLabel.Frame.Width / 2), bottomOffset - CaptionLabel.Frame.Height - 3, 
			                                CaptionLabel.Frame.Width, CaptionLabel.Frame.Height);

			signatureLine.Frame = new RectangleF (10, bottomOffset - signatureLine.Frame.Height - 5 - CaptionLabel.Frame.Height, Bounds.Width - 20, 1);

			SignaturePromptLabel.Frame = new RectangleF (10, bottomOffset - SignaturePromptLabel.Frame.Height - signatureLine.Frame.Height - 2 - CaptionLabel.Frame.Height, 
			                               SignaturePromptLabel.Frame.Width, SignaturePromptLabel.Frame.Height);
			btnClear.Frame = new RectangleF (Bounds.Width - 41 - CaptionLabel.Frame.Height, 10, 31, 14);
		}
	}
}

