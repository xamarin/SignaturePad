using SignaturePad.Forms.iOS;
using UIKit;
using Xamarin.Forms;


[assembly: ExportRenderer(typeof(Samples.Xam.Forms.SignaturePad), typeof(SignaturePadRenderer))]

namespace Samples.Xam.Forms.iOS
{
    public class Application
    {
        // This is the main entry point of the application.
        static void Main(string[] args)
        {
            // if you want to use a different Application Delegate class from "AppDelegate"
            // you can specify it here.
            UIApplication.Main(args, null, "AppDelegate");
        }
    }
}
