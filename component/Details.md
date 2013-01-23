`SignaturePad` makes capturing, saving, exporting, and displaying
signatures extremely simple.

Adding a `SignaturePad` to your iOS app:

```csharp
using SignaturePad;
...

public override void ViewDidLoad ()
{
	...
	var signature = new SignaturePadView (View.Frame);
	View.AddSubview (signature);
}
```

Adding a `SignaturePad` to your Android app:

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

To capture the user's signature as an image:

```csharp
var image = signature.GetImage ();
```
