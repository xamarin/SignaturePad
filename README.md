# Signature Pad


[![Build Status](https://jenkins.mono-project.com/buildStatus/icon?job=Components-SignaturePad-Windows)](https://jenkins.mono-project.com/view/Components/job/Components-SignaturePad-Windows/)  [![Build Status](https://jenkins.mono-project.com/buildStatus/icon?job=Components-SignaturePad)](https://jenkins.mono-project.com/view/Components/job/Components-SignaturePad/)

[![SignaturePad NuGet](https://img.shields.io/nuget/v/Xamarin.Controls.SignaturePad.svg?label=SignaturePad%20NuGet)](https://www.nuget.org/packages/Xamarin.Controls.SignaturePad)  [![SignaturePad Xamairn.Forms NuGet](https://img.shields.io/nuget/v/Xamarin.Controls.SignaturePad.Forms.svg?label=SignaturePad.Forms%20NuGet)](https://www.nuget.org/packages/Xamarin.Controls.SignaturePad.Forms)

Signature Pad makes capturing, saving, exporting, and displaying signatures extremely simple on
Xamarin.iOS, Xamarin.Android and Windows.

Not only is Signature Pad available for native apps, but also available in Xamarin.Forms apps.

![Screenshot](images/signature-ios.jpg)

---

## Using Signature Pad

Signature Pad can be installed from [NuGet.org][nuget-link] for native Xamarin and Windows apps:

```
nuget install Xamarin.Controls.SignaturePad
```

And also for Xamarin.Forms apps:

```
nuget install Xamarin.Controls.SignaturePad.Forms
```

### Using Signature Pad on iOS

```csharp
using Xamarin.Controls;

var signature = new SignaturePadView (View.Frame) {
	StrokeWidth = 3f,
	StrokeColor = UIColor.Black,
	BackgroundColor = UIColor.White,
};
```

### Using Signature Pad on Android

```csharp
using Xamarin.Controls;

var signature = new SignaturePadView (this) {
	StrokeWidth = 3f,
	StrokeColor = Color.White,
	BackgroundColor = Color.Black
};
```

### Using Signature Pad on Windows

```xml
<!-- xmlns:controls="using:Xamarin.Controls" -->

<controls:SignaturePad
	x:Name="signatureView"
	StrokeWidth="3"
	StrokeColor="White"
	Background="Black" />
```

### Using Signature Pad on Xamarin.Forms

```xml
<!-- xmlns:controls="clr-namespace:SignaturePad.Forms;assembly=SignaturePad.Forms" -->

<controls:SignaturePadView
	x:Name="signatureView"
	StrokeWidth="3"
	StrokeColor="BlackWhite"
	BackgroundColor="Black" />
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

// Xamarin.Forms
Stream bitmap = await signatureView.GetImageStreamAsync (SignatureImageFormat.Png);
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

// Xamarin.Forms
IEnumerable<Point> points = signature.Points;
```

These points can be used to save and restore a signature:

```csharp
// save points
var points = signature.Points;

// restore points (iOS, Android, Windows)
signature.LoadPoints (points);

// restore points (Xamarin.Forms)
signature.Points = points;
```

---

## License

The license for this repository is specified in [LICENSE](LICENSE).


## .NET Foundation
This project is part of the [.NET Foundation](http://www.dotnetfoundation.org/projects).

[nuget-link]: https://www.nuget.org/packages/Xamarin.Controls.SignaturePad
