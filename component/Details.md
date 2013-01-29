Signature Pad makes capturing, saving, exporting, and displaying
signatures extremely simple.

Adding a `SignaturePadView` to your iOS app:

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

Adding a `SignaturePadView` to your Android app:

```csharp
using SignaturePad;
...

protected override void OnCreate (Bundle bundle)
{
	base.OnCreate (bundle);

	var signature = new SignaturePadView (this);
	AddContentView (signature,
		new ViewGroup.LayoutParams (ViewGroup.LayoutParams.FillParent, ViewGroup.LayoutParams.FillParent));
}
```

To capture the user's signature as an image:

```csharp
var image = signature.GetImage ();
```

*Screenshot created with [PlaceIt](http://placeit.breezi.com).*
