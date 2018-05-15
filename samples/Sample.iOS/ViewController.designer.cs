// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace Sample.iOS
{
    [Register ("ViewController")]
    partial class ViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnLoad { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnSave { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnSaveImage { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        Xamarin.Controls.SignaturePadView signatureView { get; set; }

        [Action ("LoadVectorClicked:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void LoadVectorClicked (UIKit.UIButton sender);

        [Action ("SaveImageClicked:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void SaveImageClicked (UIKit.UIButton sender);

        [Action ("SaveVectorClicked:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void SaveVectorClicked (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (btnLoad != null) {
                btnLoad.Dispose ();
                btnLoad = null;
            }

            if (btnSave != null) {
                btnSave.Dispose ();
                btnSave = null;
            }

            if (btnSaveImage != null) {
                btnSaveImage.Dispose ();
                btnSaveImage = null;
            }

            if (signatureView != null) {
                signatureView.Dispose ();
                signatureView = null;
            }
        }
    }
}