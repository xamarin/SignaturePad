//
// Main.cs
//
// Author:
//   Timothy Risi (timothy.risi@gmail.com)
//
// Copyright (C) 2016 Xamarin Inc.
//

using UIKit;

namespace Sample.iOS
{
    public class Application
	{
		// This is the main entry point of the application.
		static void Main (string[] args)
		{
			// if you want to use a different Application Delegate class from "AppDelegate"
			// you can specify it here.
			UIApplication.Main (args, null, "AppDelegate");
		}
	}
}
