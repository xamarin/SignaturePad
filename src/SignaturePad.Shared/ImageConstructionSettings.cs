using System;

#if __ANDROID__
using NativeSize = System.Drawing.SizeF;
using NativeColor = Android.Graphics.Color;
using NativeNullableColor = System.Nullable<Android.Graphics.Color>;
#elif __IOS__
using NativeSize = CoreGraphics.CGSize;
using NativeColor = UIKit.UIColor;
using NativeNullableColor = UIKit.UIColor;
#elif WINDOWS_PHONE
using NativeSize = System.Windows.Size;
using NativeColor = System.Windows.Media.Color;
using NativeNullableColor = System.Nullable<System.Windows.Media.Color>;
#elif WINDOWS_UWP
using NativeSize = Windows.Foundation.Size;
using NativeColor = Windows.UI.Color;
using NativeNullableColor = System.Nullable<Windows.UI.Color>;
#endif

namespace Xamarin.Controls
{
	public enum SizeOrScaleType
	{
		Size,
		Scale
	}

	public struct SizeOrScale
	{
		public SizeOrScale (float xy, SizeOrScaleType type)
		{
			X = xy;
			Y = xy;
			Type = type;
		}

		public SizeOrScale (float x, float y, SizeOrScaleType type)
		{
			X = x;
			Y = y;
			Type = type;
		}

		public float X { get; set; }

		public float Y { get; set; }

		public SizeOrScaleType Type { get; set; }

		public bool IsValid => X > 0 && Y > 0;

		public float GetScale (float width, float height)
		{
			if (Type == SizeOrScaleType.Scale)
			{
				return Math.Min (X, Y);
			}
			else
			{
				return Math.Min (X / width, Y / height);
			}
		}

		public NativeSize GetSize (float width, float height)
		{
			if (Type == SizeOrScaleType.Scale)
			{
				return new NativeSize (width * X, height * Y);
			}
			else
			{
				return new NativeSize (X, Y);
			}
		}

		public static implicit operator SizeOrScale (float scale)
		{
			return new SizeOrScale (scale, SizeOrScaleType.Scale);
		}

		public static implicit operator SizeOrScale (NativeSize size)
		{
			return new SizeOrScale ((float)size.Width, (float)size.Height, SizeOrScaleType.Size);
		}
	}

	public struct ImageConstructionSettings
	{
#if __IOS__ || __ANDROID__
		private static readonly NativeColor Black = NativeColor.Black;
		private static readonly NativeColor Transparent = new NativeColor (0, 0, 0, 0);
#elif WINDOWS_PHONE || WINDOWS_UWP
		private static readonly NativeColor Black = NativeColor.FromArgb (255, 0, 0, 0);
		private static readonly NativeColor Transparent = NativeColor.FromArgb (0, 0, 0, 0);
#endif

		public bool? ShouldCrop { get; set; }

		public SizeOrScale? DesiredSizeOrScale { get; set; }

		public NativeNullableColor StrokeColor { get; set; }

		public NativeNullableColor BackgroundColor { get; set; }

		public float? StrokeWidth { get; set; }

		internal void ApplyDefaults ()
		{
			ApplyDefaults (2f, Black);
		}

		internal void ApplyDefaults (float strokeWidth, NativeColor strokeColor)
		{
			ShouldCrop = ShouldCrop ?? true;
			DesiredSizeOrScale = DesiredSizeOrScale ?? 1f;
			StrokeColor = StrokeColor ?? strokeColor;
			BackgroundColor = BackgroundColor ?? Transparent;
			StrokeWidth = StrokeWidth ?? strokeWidth;
		}
	}
}
