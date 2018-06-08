# Signature Pad


[![Build Status](https://jenkins.mono-project.com/buildStatus/icon?job=Components-SignaturePad-Windows)](https://jenkins.mono-project.com/view/Components/job/Components-SignaturePad-Windows/)  [![Build Status](https://jenkins.mono-project.com/buildStatus/icon?job=Components-SignaturePad)](https://jenkins.mono-project.com/view/Components/job/Components-SignaturePad/)

[![SignaturePad NuGet](https://img.shields.io/nuget/vpre/Xamarin.Controls.SignaturePad.svg?label=SignaturePad%20NuGet)](https://www.nuget.org/packages/Xamarin.Controls.SignaturePad)  [![SignaturePad Xamairn.Forms NuGet](https://img.shields.io/nuget/vpre/Xamarin.Controls.SignaturePad.Forms.svg?label=SignaturePad.Forms%20NuGet)](https://www.nuget.org/packages/Xamarin.Controls.SignaturePad.Forms)

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

var signatureView = new SignaturePadView (View.Frame) {
	StrokeWidth = 3f,
	StrokeColor = UIColor.Black,
	BackgroundColor = UIColor.White,
};
```

### Using Signature Pad on Android

```csharp
using Xamarin.Controls;

var signatureView = new SignaturePadView (this) {
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
UIImage image = signatureView.GetImage ();

// Android
Bitmap image = signatureView.GetImage ();

// Windows
WriteableBitmap bitmap = signatureView.GetImage ();
```

For Xamarin.Forms, there is no "native" image format, but `GetImageStreamAsync` can be used instead
to retrieve an encoded (jpeg or png) image stream:

```csharp
Stream bitmap = await signatureView.GetImageStreamAsync (SignatureImageFormat.Png);
```

### Obtaining the Signature Points

In addition to retrieving the signature as an image, the signature can also be retrieved as
as an array of points:

```csharp
var strokes = signatureView.Strokes;
```

These strokes can be used to save and restore a signature:

```csharp
// restore strokes (iOS, Android, Windows)
signatureView.LoadStrokes (newStrokes);

// restore strokes (Xamarin.Forms)
signatureView.Strokes = newStrokes;
```

---

## License

The license for this repository is specified in [LICENSE](LICENSE).


## .NET Foundation
This project is part of the [.NET Foundation](http://www.dotnetfoundation.org/projects).

[nuget-link]: https://www.nuget.org/packages/Xamarin.Controls.SignaturePad
