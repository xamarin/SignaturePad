Signature Pad makes capturing, saving, exporting, and displaying
signatures extremely simple.

![<SignaturePad Screenshot>](https://raw.githubusercontent.com/xamarin/SignaturePad/master/component/signature-ios.jpg)

## Examples

### Displaying a signature pad

On iOS:

```csharp
using SignaturePad;
...

public override void ViewDidLoad ()
{
	...
	var signature = new SignaturePadView (View.Frame) {
		LineWidth = 3f
	};
	View.AddSubview (signature);
}
```

On Android:

```csharp
using SignaturePad;
...

protected override void OnCreate (Bundle bundle)
{
	base.OnCreate (bundle);

	var signature = new SignaturePadView (this) {
		LineWidth = 3f
	};
	AddContentView (signature,
		new ViewGroup.LayoutParams (ViewGroup.LayoutParams.FillParent, ViewGroup.LayoutParams.FillParent));
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
------------

## License

The Apache License 2.0 applies to all samples in this repository.

   Copyright 2011 Xamarin Inc

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.