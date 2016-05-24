# Signature Pad

[![AppVeyor][ci-img]][ci-link] [![NuGet][signature-page-img]][signature-page-link]

Signature Pad makes capturing, saving, exporting, and displaying
signatures extremely simple on Xamarin.iOS, Xamarin.Android and WIndows Phone.

![Screenshot](/component/signature-ios.jpg)


### Displaying a Signature Pad on iOS

On iOS you can display a signature by adding a `SignaturePadView` to your view like this:

```csharp
using SignaturePad;
...

public override void ViewDidLoad ()
{
	...
	var signature = new SignaturePadView (View.Frame) {
		StrokeWidth = 3f
	};
	View.AddSubview (signature);
}
```

### Displaying a Signature Pad on Android
On Android, displaying a signature is done by adding a `SignaturePadView` to your Activity like the example below:

```csharp
using SignaturePad;
...

protected override void OnCreate (Bundle bundle)
{
	base.OnCreate (bundle);

	var signature = new SignaturePadView (this) {
		StrokeWidth = 3f
	};
	AddContentView (signature,
		new ViewGroup.LayoutParams (ViewGroup.LayoutParams.FillParent, ViewGroup.LayoutParams.FillParent));
}
```

### Displaying a Signature Pad on Windows Phone

On Windows Phone, it is easiest to add your `SignaturePad` control directly in your Page's `.xaml` file.  To do this, be sure you register the namespace in the `<phone:PhoneApplicationPage ... />` tag.  Here is an example:
```xml
<phone:PhoneApplicationPage 
	<!-- Other properties -->
    xmlns:component="clr-namespace:Xamarin.Controls;assembly=SignaturePad.WP7">

	<!-- Other controls -->
	<component:SignaturePad Margin="10,10,10,78" Name="signatureView" />
    
</phone:PhoneApplicationPage>
```

### Getting the signature as an image
You can get the signature drawn on the canvas as an image (the type will be the native platform's image class type):

```csharp
// iOS:
UIImage image = signature.GetImage ();

// Android:
Bitmap image = signature.GetImage ();

// Windows Phone:
WriteableBitmap bitmap = signatureView.GetImage ();
```

### Saving / Loading a Signature

While it's possible to get the signature as a bitmap on each platform, a bitmap is not a good format to restore signature data from.  If you would like to save the signature in a way it can be loaded back into the view, you will need to save the `PointF[]` array of points from the view:

```csharp
// Discontinuous lines are separated by PointF.Empty
PointF[] points = signature.Points;
```

To restore a previously saved `PointF[]` array of points, you can load them into the view like this:
```csharp
signature.LoadPoints (points);
```



Customization
-------------

You can change some of the positioning, colors, fonts and the background image of the SignaturePad
using a few interfaces that the control provides and standard techniques provided by the platform.

### SignaturePad customization interface

The class for both iOS and Android expose some of its internal elements to allow text, font, color and positioning manipulation from your code:

 - `StrokeColor` Sets the color of the signature input.
 - `StrokeWidth` Sets the width of the signature input.
 - `BackgroundColor` Sets the color for the whole signature pad.
 - `SignatureLineColor` The color of the horizontal line.
 - `SignaturePrompt` The text label containing the symbol or text that goes under the horizontal line (Default "X").
 - `Caption` The text label that goes under the horizontal line.
 - `SignatureLine` The view that is used to render the horizontal line.
 - `ClearLabel` The view that when clicked clears the pad.
 - `BackgroundImageView` An optional image rendered below the input strokes that can be used as a texture, logo or watermark.

### iOS customization tips

Check the sample for ideas on how to manipulate the layout to get the desired effects and color.

You can alter the subviews Frames or if you are targeting above iOS 6, use Auto-layout constraints to reposition elments within the pad. For coloring, reassign properties such as BackgroundColor (including `UIColor.Clear` for a transparent view).

`BackgroundImageView` cannot be set, but its `Image` member can, so you can assign a bitmap pulled from a resource or wherever you may get its data. Change the Alpha to make it semi-transparent to get a watermark effect or create a texture using a bitmap with the same dimensions as the pad.

If you don't want the SignaturePrompt, the Caption or the SignatureLine to appear inside your pad, just assign
their respective `Hidden` property to `true`.

`SignaturePad.Layer` can be manipulated to generate or remove the shadow from the control or alter its thickness or roundness.

### Android customization tips

Check the sample for ideas on how to manipulate the layout to get the desired effects and color.

Under Android, the control inherits from `RelativeLayout`, which provides a good amount of flexibility for repositioning of the child views within the pad. Assign for the children the `LayoutParameters` property with new `RelativeLayout.LayoutParams` to move the elements around or resize them using relative positioning policies. All of the elements within the pad have Ids already set so you can establish relative positions between them easily.

`BackgroundImageView` cannot be set, but you can assign it new data using the `SetImage*` methods and then alter it with SetAlpha to make it semi-transparent and get a watermark effect or create a texture effect (remember to resize it to the full extent of its parent, the SignaturePad).

If you don't want the SignaturePrompt, the Caption or the SignatureLine to appear inside your pad, just assign
their respective `Visibility` property to `ViewStates.Invisible`.



------------

## License

The Apache License 2.0 applies to all samples in this repository.

   Copyright 2016 Xamarin Inc

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.


[ci-img]: https://img.shields.io/appveyor/ci/mattleibow/SignaturePad/add-cake.svg?maxAge=2592000
[ci-link]: https://ci.appveyor.com/project/mattleibow/SignaturePad

[signature-page-img]: https://img.shields.io/nuget/v/Xamarin.Controls.SignaturePad.svg?maxAge=2592000
[signature-page-link]: https://www.nuget.org/packages/Xamarin.Controls.SignaturePad
