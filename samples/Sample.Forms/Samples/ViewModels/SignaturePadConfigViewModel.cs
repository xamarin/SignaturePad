using System.Windows.Input;
using Xamarin.Forms;

using Samples.Models;

namespace Samples.ViewModels
{
	public class SignaturePadConfigViewModel : BaseViewModel
	{
		private readonly ICommand isConfiguringCommand;
		private bool isConfiguring;

		private string captionText;
		private string clearText;
		private string promptText;
		private int strokeWidth;

		private Color captionTextColor;
		private Color clearTextColor;
		private Color promptTextColor;
		private Color signaturePadBackground;
		private Color signatureLineColor;
		private Color strokeColor;

		public SignaturePadConfigViewModel ()
		{
			isConfiguringCommand = new Command (() => IsConfiguring = !IsConfiguring);
		}

		public ICommand ConfigureCommand => isConfiguringCommand;

		public bool IsConfiguring
		{
			get { return isConfiguring; }
			set { Set (ref isConfiguring, value); }
		}

		public string CaptionText
		{
			get { return captionText; }
			set { Set (ref captionText, value); }
		}

		public string ClearText
		{
			get { return clearText; }
			set { Set (ref clearText, value); }
		}

		public string PromptText
		{
			get { return promptText; }
			set { Set (ref promptText, value); }
		}

		public Color CaptionTextColor
		{
			get { return captionTextColor; }
			set { if (Set (ref captionTextColor, value)) Refresh (nameof (CaptionTextColorIndex)); }
		}

		public Color ClearTextColor
		{
			get { return clearTextColor; }
			set { if (Set (ref clearTextColor, value)) Refresh (nameof (ClearTextColorIndex)); }
		}

		public Color PromptTextColor
		{
			get { return promptTextColor; }
			set { if (Set (ref promptTextColor, value)) Refresh (nameof (PromptTextColorIndex)); }
		}

		public Color SignaturePadBackground
		{
			get { return signaturePadBackground; }
			set { if (Set (ref signaturePadBackground, value)) Refresh (nameof (SignaturePadBackgroundIndex)); }
		}

		public Color SignatureLineColor
		{
			get { return signatureLineColor; }
			set { if (Set (ref signatureLineColor, value)) Refresh (nameof (SignatureLineColorIndex)); }
		}

		public Color StrokeColor
		{
			get { return strokeColor; }
			set { if (Set (ref strokeColor, value)) Refresh (nameof (StrokeColorIndex)); }
		}

		public int StrokeWidth
		{
			get { return strokeWidth; }
			set { Set (ref strokeWidth, value); }
		}

		public int CaptionTextColorIndex
		{
			get { return Configurations.Colors.IndexOf (CaptionTextColor); }
			set { CaptionTextColor = Configurations.Colors[value]; }
		}

		public int ClearTextColorIndex
		{
			get { return Configurations.Colors.IndexOf (ClearTextColor); }
			set { ClearTextColor = Configurations.Colors[value]; }
		}

		public int PromptTextColorIndex
		{
			get { return Configurations.Colors.IndexOf (PromptTextColor); }
			set { PromptTextColor = Configurations.Colors[value]; }
		}

		public int SignaturePadBackgroundIndex
		{
			get { return Configurations.Colors.IndexOf (SignaturePadBackground); }
			set { SignaturePadBackground = Configurations.Colors[value]; }
		}

		public int SignatureLineColorIndex
		{
			get { return Configurations.Colors.IndexOf (SignatureLineColor); }
			set { SignatureLineColor = Configurations.Colors[value]; }
		}

		public int StrokeColorIndex
		{
			get { return Configurations.Colors.IndexOf (StrokeColor); }
			set { StrokeColor = Configurations.Colors[value]; }
		}

		public override void OnAppearing ()
		{
			base.OnAppearing ();

			IsConfiguring = true;

			CaptionText = "sign here";
			ClearText = "clear";
			PromptText = ">";
			StrokeWidth = 2;

			CaptionTextColor = Color.Gray;
			ClearTextColor = Color.Gray;
			PromptTextColor = Color.Gray;
			SignaturePadBackground = Color.Yellow;
			SignatureLineColor = Color.Black;
			StrokeColor = Color.Black;
		}
	}
}
