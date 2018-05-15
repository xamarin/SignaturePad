using Foundation;
using UIKit;

namespace Sample.iOS
{
	[Register (nameof (AppDelegate))]
	public class AppDelegate : UIApplicationDelegate
	{
		public override UIWindow Window { get; set; }

		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			return true;
		}
	}
}
