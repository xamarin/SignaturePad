# Signature Pad


[![Build Status](https://jenkins.mono-project.com/buildStatus/icon?job=Components-SignaturePad-Windows)](https://jenkins.mono-project.com/view/Components/job/Components-SignaturePad-Windows/)  [![Build Status](https://jenkins.mono-project.com/buildStatus/icon?job=Components-SignaturePad)](https://jenkins.mono-project.com/view/Components/job/Components-SignaturePad/)

[![SignaturePad NuGet](https://img.shields.io/nuget/v/Xamarin.Controls.SignaturePad.svg?label=SignaturePad%20NuGet)](https://www.nuget.org/packages/Xamarin.Controls.SignaturePad)  [![SignaturePad Xamairn.Forms NuGet](https://img.shields.io/nuget/v/Xamarin.Controls.SignaturePad.Forms.svg?label=SignaturePad.Forms%20NuGet)](https://www.nuget.org/packages/Xamarin.Controls.SignaturePad.Forms)

Signature Pad makes capturing, saving, exporting, and displaying signatures extremely simple on
Xamarin.iOS, Xamarin.Android and Windows.

Not only is Signature Pad available for native apps, but also available in Xamarin.Forms apps.

![Screenshot](component/signature-ios.jpg)

## Using Signature Pad

Signature Pad can be installed from [NuGet.org][nuget-link] for native Xamarin and Windows app:

```
nuget install Xamarin.Controls.SignaturePad
```

And also for Xamarin.Forms apps:

```
nuget install Xamarin.Controls.SignaturePad.Forms
```

### Using Signature Pad on iOS

```csharp
using SignaturePad;

var signature = new SignaturePadView (View.Frame) {
	StrokeWidth = 3f,
	StrokeColor = UIColor.Black,
	BackgroundColor = UIColor.White,
};
```

### Using Signature Pad on Android

```csharp
using SignaturePad;

var signature = new SignaturePadView (this) {
	StrokeWidth = 3f,
	StrokeColor = Color.White,
	BackgroundColor = Color.Black
};
```

### Using Signature Pad on Windows

```xml
<phone:PhoneApplicationPage 
	xmlns:component="clr-namespace:Xamarin.Controls;assembly=SignaturePad">

	<component:SignaturePad 
		Name="signature"
		StrokeWidth="3" 
		StrokeColor="White" 
		BackgroundColor="Black" />

</phone:PhoneApplicationPage>
```

### Obtaining a Signature Image

The signature that was drawn on the canvas can be obtained as a image using the `GetImage(...)`
method overloads. The resulting image will be in the native platform image type:

```csharp
// iOS
UIImage image = signature.GetImage ();

// Android
Bitmap image = signature.GetImage ();

// Windows
WriteableBitmap bitmap = signatureView.GetImage ();
```

### Obtaining the Signature Points

In addition to retrieving the signature as an image, the signature can also be retrieved as
as an array of points. Each line is separated with a `{ 0, 0 }` point:

```csharp
// iOS
CGPoint[] points = signature.Points;

// Android
PointF[] points = signature.Points;

// Windows
Point[] points = signature.Points;
```

These points can be used to save and restore a signature:

```csharp
// save points
var points = signature.Points

// restore points (native)
signature.LoadPoints (points);

// restore points (Xamarin.Forms)
signature.Points = points;
```

## Customization

You can change some of the positioning, colors, fonts and the background image of the Signature Pad
using a few properties:

 - `StrokeColor` Sets the color of the signature input.
 - `StrokeWidth` Sets the width of the signature input.
 - `BackgroundColor` Sets the color for the whole signature pad.
 - `SignatureLine` The view that is used to render the horizontal line.
 - `SignatureLineColor` The color of the horizontal line.
 - `SignaturePrompt` The text label containing the symbol or text that represents the prompt (Default "X").
 - `SignaturePromptText` The text that represents the prompt (Default "X").
 - `Caption` The text label that goes under the horizontal line.
 - `CaptionText` The text that goes under the horizontal line.
 - `ClearLabel` The view that when clicked clears the pad.
 - `ClearLabelText` The text that represents the clear label.
 - `BackgroundImageView` An optional image rendered below the input strokes that can be used as a texture, logo or watermark.

------------

## License

The license for this repository is specified in [LICENSE](LICENSE).


## .NET Foundation
This project is part of the [.NET Foundation](http://www.dotnetfoundation.org/projects).

[comp-store-link]: https://components.xamarin.com/view/signature-pad
[nuget-link]: https://www.nuget.org/packages/Xamarin.Controls.SignaturePad
