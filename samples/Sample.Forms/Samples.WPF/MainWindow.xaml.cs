// <copyright file="MainWindow.xaml.cs" company="Vladislav Antonyuk">
//     Vladislav Antonyuk. All rights reserved.
// </copyright>
// <author>Vladislav Antonyuk</author>

namespace Samples.WPF
{
	using System;
	using System.IO;
	using System.Threading.Tasks;
	using SignaturePad.Forms;
	using Xamarin.Forms;
	using Xamarin.Forms.Platform.WPF;

	/// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : FormsApplicationPage
    {
        public MainWindow()
        {
            InitializeComponent();
            Forms.Init();
	        SignaturePadCanvasRenderer.Init ();
            LoadApplication(new Samples.App(OnSaveSignature));
        }

	    private async Task<bool> OnSaveSignature (Stream bitmap, string filename)
	    {
		    var storageFolder = Environment.SpecialFolder.CommonPictures.ToString();
		    var file = System.IO.File.Create (Path.Combine(storageFolder, filename));

		    using (var dest = file)
		    {
			    await bitmap.CopyToAsync (dest);
		    }

		    return true;
	    }
	}
}
