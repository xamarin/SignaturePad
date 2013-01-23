`SignaturePad` makes capturing, saving, exporting, and displaying
signatures extremely simple.

## Examples

### Displaying the `SignaturePad`

On iOS:

```csharp
using SignaturePad;
...

var signature = new SignaturePadView (new RectangleF (10, 10, Bounds.Width - 20, Bounds.Height - 60)) {
	// Default BackgroundColor is UIColor.White.
	BackgroundColor = UIColor.Black,

	// Default StrokeColor is UIColor.Black.
	StrokeColor = UIColor.White,

	// Default LineWidth is 2.
	LineWidth = 3f
};
AddSubview (signature);
```

On Android:

```csharp
using SignaturePad;
...

protected override void OnCreate (Bundle bundle)
{
	base.OnCreate (bundle);

	var signature = new SignaturePadView (context) {
		BackgroundColor = Color.White,
		StrokeColor = Color.Black,
		LineWidth = 3f
	};
	AddContentView (signature, new ViewGroup.LayoutParams (200, 200));
}
```

### Getting the signature as an image

```csharp
// on iOS:
UIImage image = signature.GetImage ();

// on Android:
Bitmap image = signature.GetImage ();
```

### Getting the signature as a point array

```csharp
// Discontinuous lines are separated by PointF.Empty
PointF[] points = signature.Points;
```

### Loading a signature from a point array

```csharp
signature.LoadPoints (points);
```
