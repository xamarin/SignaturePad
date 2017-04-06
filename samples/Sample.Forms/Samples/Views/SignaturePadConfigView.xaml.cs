using Xamarin.Forms;

using Samples.Models;

namespace Samples.Views
{
	public partial class SignaturePadConfigView : ContentPage
	{
		public SignaturePadConfigView ()
		{
			InitializeComponent ();

			SetColors (backgroundColorPicker);
			SetColors (captionTextColorPicker);
			SetColors (clearTextColorPicker);
			SetColors (promptTextColorPicker);
			SetColors (signatureLineColorPicker);
			SetColors (strokeColorPicker);
		}

		private void SetColors (Picker picker)
		{
			foreach (var color in Configurations.ColorNames)
			{
				picker.Items.Add (color);
			}
		}
	}
}
