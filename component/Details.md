`SignaturePad` makes capturing, exporting, storing, and displaying
signatures extremely simple.

Adding a `SignaturePad` to your iOS app:

```csharp
using Xamarin.Controls;
...

public override void ViewDidLoad ()
{
	...
	var signature = new SignaturePad (View.Frame);
	View.AddSubview (signature);
}
```

Adding a `SignaturePad` to your Android app:

```csharp
using Xamarin.Controls;
...

var signature = new SignaturePad (context);
signature.Layout (10, 10, Width - 10, Height - 50);
```

To capture the user's signature as an image:

```csharp
var image = signature.GetImage ();
```
