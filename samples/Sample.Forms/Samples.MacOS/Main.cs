// <copyright file="Main.cs" company="Vladislav Antonyuk">
//     Vladislav Antonyuk. All rights reserved.
// </copyright>
// <author>Vladislav Antonyuk</author>

namespace Samples.MacOS
{
    using AppKit;

    public class Application
    {
        // This is the main entry point of the application.
        private static void Main(string[] args)
        {
            NSApplication.Init();
            NSApplication.SharedApplication.Delegate = new AppDelegate();
            NSApplication.Main(args);
        }
    }
}
