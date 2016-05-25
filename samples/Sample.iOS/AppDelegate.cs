//
// AppDelegate.cs
//
// Author:
//   Timothy Risi (timothy.risi@gmail.com)
//
// Copyright (C) 2016 Xamarin Inc.
//

using Foundation;
using UIKit;

namespace Sample.iOS
{
    [Register("AppDelegate")]
    public class AppDelegate : UIApplicationDelegate
    {
        public override UIWindow Window { get; set; }

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            return true;
        }
    }
}
