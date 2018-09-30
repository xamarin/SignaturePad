// <copyright file="UiService.cs" company="Vladislav Antonyuk">
//     Vladislav Antonyuk. All rights reserved.
// </copyright>
// <author>Vladislav Antonyuk</author>

namespace Samples.MacOS
{
    using AppKit;
    using CoreGraphics;

    internal static class UiService
    {
        static UiService()
        {
            var mainWindowStyle = NSWindowStyle.Closable | NSWindowStyle.Miniaturizable | NSWindowStyle.Resizable |
                                  NSWindowStyle.Titled;
            var mainWindowSize = new CGSize(550, 330);
            var mainWindowRect = new CGRect(
                NSScreen.MainScreen.Frame.Width / 2 - mainWindowSize.Width / 2,
                NSScreen.MainScreen.Frame.Height / 2 - mainWindowSize.Height / 2,
                mainWindowSize.Width,
                mainWindowSize.Height);
            MainWindow = new NSWindow(mainWindowRect, mainWindowStyle, NSBackingStore.Buffered, false)
            {
                TitleVisibility = NSWindowTitleVisibility.Visible
            };
        }

        internal static NSWindow MainWindow { get; }
    }
}
