Signature Pad makes capturing, saving, exporting, and displaying signatures extremely simple.

There are controls for both native apps and Xamarin.Forms apps for most .NET platforms:

 - Xamarin.Android
 - Xamarin.iOS
 - Windows 10 UWP
 - Windows Store 8+
 - Windows Phone 8+

## Using Signature Pad

Signature Pad has many features that are easy to use.

_Signature Pad can be used with Xamarin.Forms, however this component does not contain those libraries. In order to use Signature Pad with Xamarin.Forms, you can install the ["Xamarin.Controls.SignaturePad.Forms" NuGet](https://www.nuget.org/packages/Xamarin.Controls.SignaturePad.Forms)._

### Capturing & Displaying Signatures

The Signature Pad control can be added to your app's view hierarchy using drag-and-drop with the designers or by writing simple code.

On iOS:

```csharp
var signature = new SignaturePadView () {
	StrokeWidth = 3f,
	StrokeColor = UIColor.Black
};
```

On Android:

```csharp
var signature = new SignaturePadView (this) {
	StrokeWidth = 3f,
	StrokeColor = Color.Black
};
```

On Windows:

```csharp
var signature = new SignaturePad () {
	StrokeWidth = 3f,
	StrokeColor = Colors.Black
};
```

### Exporting Signature Images

Once the user has written his/her signature, you can export that signature as a native bitmap.

```csharp
// iOS
UIImage image = signature.GetImage ();

// Android
Bitmap image = signature.GetImage ();

// Windows
WriteableBitmap image = signature.GetImage ();
```

### Exporting Signature Points

If you have to save and restore a user's signature, you can also capture the raw stoke points.

```csharp
// iOS
CGPoint[] points = signature.Points;
CGPoint[][] strokes = signature.Strokes;

// Android
PointF[] points = signature.Points;
PointF[][] strokes = signature.Strokes;

// Windows
Point[] points = signature.Points;
Point[][] strokes = signature.Strokes;
```

### Importing Signature Points

```csharp
signature.LoadPoints (points);
signature.LoadStrokes (strokes);
```
