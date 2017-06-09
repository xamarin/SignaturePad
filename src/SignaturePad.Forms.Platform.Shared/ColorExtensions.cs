using Color = Xamarin.Forms.Color;
#if WINDOWS_PHONE
using System.Windows.Controls;
using System.Windows.Media;
using NativeColor = System.Windows.Media.Color;
#elif WINDOWS_UWP || WINDOWS_PHONE_APP || WINDOWS_APP
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using NativeColor = Windows.UI.Color;
#elif __IOS__
using UIKit;
using Xamarin.Forms.Platform.iOS;
using NativeColor = UIKit.UIColor;
#elif __ANDROID__
using Android.Widget;
using Xamarin.Forms.Platform.Android;
using NativeColor = Android.Graphics.Color;
#endif

namespace SignaturePad.Forms
{
	public static class ColorExtensions
	{
#if WINDOWS_PHONE || WINDOWS_UWP || WINDOWS_PHONE_APP || WINDOWS_APP
		public static NativeColor ToWindows (this Color color)
		{
			return NativeColor.FromArgb (
				(byte)(color.A * 255),
				(byte)(color.R * 255),
				(byte)(color.G * 255),
				(byte)(color.B * 255));
		}
#endif

		public static NativeColor ToNative (this Color color)
		{
#if WINDOWS_PHONE || WINDOWS_UWP || WINDOWS_PHONE_APP || WINDOWS_APP
			return color.ToWindows ();
#elif __IOS__
			return color.ToUIColor ();
#elif __ANDROID__
			return color.ToAndroid ();
#endif
		}

#if WINDOWS_PHONE || WINDOWS_UWP || WINDOWS_PHONE_APP || WINDOWS_APP
		public static void SetTextColor (this TextBlock textBlock, Color color)
		{
			textBlock.Foreground = new SolidColorBrush (color.ToNative ());
		}
#elif __IOS__
		public static void SetTextColor (this UILabel label, Color color)
		{
			label.TextColor = color.ToNative ();
		}
		public static void SetTextColor (this UIButton button, Color color)
		{
			button.SetTitleColor (color.ToNative (), UIControlState.Normal);
		}
#elif __ANDROID__
		public static void SetTextColor (this TextView label, Color color)
		{
			label.SetTextColor (color.ToNative ());
		}
#endif
	}
}
