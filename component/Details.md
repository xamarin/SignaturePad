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

var signature = new SignaturePadView (context);
signature.Layout (10, 10, Width - 10, Height - 50);
```

To capture the user's signature as an image:

```csharp
var image = signature.GetImage ();
```
