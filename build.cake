#tool nuget:?package=XamarinComponent&version=1.1.0.60

#addin nuget:?package=Cake.Xamarin&version=1.3.0.15
#addin nuget:?package=Cake.Xamarin.Build&version=2.0.22

var TARGET = Argument ("t", Argument ("target", Argument ("Target", "Default")));
var VERBOSITY = (Verbosity) Enum.Parse (typeof(Verbosity), Argument ("v", Argument ("verbosity", Argument ("Verbosity", "Verbose"))), true);

class SolutionBuilder : DefaultSolutionBuilder 
{
	public override void RunBuild (FilePath solution)
	{
		CakeContext.MSBuild (solution, c => {
			if (CakeContext.GetOperatingSystem() == PlatformFamily.OSX)
				c.ToolPath = "/Library/Frameworks/Mono.framework/Versions/Current/Commands/msbuild";
			if (Verbosity.HasValue)
				c.Verbosity = Verbosity.Value;
			c.Configuration = "Release";
			if (!string.IsNullOrEmpty (Platform))
				c.Properties["Platform"] = new[] { Platform };
			c.MSBuildPlatform = MSBuildPlatform.x86;
		});
	}
}

var buildSpec = new BuildSpec () {
	Libs = new ISolutionBuilder [] {
		new SolutionBuilder {
			SolutionPath = "src/SignaturePad.Mac.sln",
			BuildsOn = BuildPlatforms.Mac,
			Verbosity = VERBOSITY,
			OutputFiles = new [] {
				new OutputFileCopy {
					FromFile = "./src/SignaturePad.Android/bin/Release/SignaturePad.dll",
					ToDirectory = "output/android",
				},
				new OutputFileCopy {
					FromFile = "./src/SignaturePad.iOS/bin/unified/Release/SignaturePad.dll",
					ToDirectory = "output/ios-unified",
				},
				new OutputFileCopy {
					FromFile = "./src/SignaturePad.Forms/bin/Release/SignaturePad.Forms.dll",
					ToDirectory = "output/pcl",
				},
				new OutputFileCopy {
					FromFile = "./src/SignaturePad.Forms.NetStandard/bin/Release/SignaturePad.Forms.dll",
					ToDirectory = "output/netstandard",
				},
				new OutputFileCopy {
					FromFile = "./src/SignaturePad.Forms.Droid/bin/Release/SignaturePad.Forms.dll",
					ToDirectory = "output/android",
				},
				new OutputFileCopy {
					FromFile = "./src/SignaturePad.Forms.iOS/bin/Release/SignaturePad.Forms.dll",
					ToDirectory = "output/ios-unified",
				},
			}
		},
		new SolutionBuilder {
			SolutionPath = "src/SignaturePad.VS2015.sln",
			BuildsOn = BuildPlatforms.Windows,
			Verbosity = VERBOSITY,
			OutputFiles = new [] {
				new OutputFileCopy {
					FromFile = "./src/SignaturePad.WP8/bin/Release/SignaturePad.dll",
					ToDirectory = "output/wp8",
				},
				new OutputFileCopy {
					FromFile = "./src/SignaturePad.Forms.WindowsPhone/bin/Release/SignaturePad.Forms.dll",
					ToDirectory = "output/wp8",
				},
				new OutputFileCopy {
					FromFile = "./src/SignaturePad.Windows81/bin/Release/SignaturePad.dll",
					ToDirectory = "output/win",
				},
				new OutputFileCopy {
					FromFile = "./src/SignaturePad.Forms.Windows81/bin/Release/SignaturePad.Forms.dll",
					ToDirectory = "output/win",
				},
				new OutputFileCopy {
					FromFile = "./src/SignaturePad.WindowsPhone81/bin/Release/SignaturePad.dll",
					ToDirectory = "output/wpa",
				},
				new OutputFileCopy {
					FromFile = "./src/SignaturePad.Forms.WindowsPhone81/bin/Release/SignaturePad.Forms.dll",
					ToDirectory = "output/wpa",
				},
				new OutputFileCopy {
					FromFile = "./src/SignaturePad.WindowsRuntime81/bin/Release/SignaturePad.dll",
					ToDirectory = "output/winrt",
				},
			}
		},
		new SolutionBuilder {
			SolutionPath = "src/SignaturePad.sln",
			BuildsOn = BuildPlatforms.Windows,
			Verbosity = VERBOSITY,
			OutputFiles = new [] {
				new OutputFileCopy {
					FromFile = "./src/SignaturePad.Android/bin/Release/SignaturePad.dll",
					ToDirectory = "output/android",
				},
				new OutputFileCopy {
					FromFile = "./src/SignaturePad.iOS/bin/unified/Release/SignaturePad.dll",
					ToDirectory = "output/ios-unified",
				},
				new OutputFileCopy {
					FromFile = "./src/SignaturePad.Forms/bin/Release/SignaturePad.Forms.dll",
					ToDirectory = "output/pcl",
				},
				new OutputFileCopy {
					FromFile = "./src/SignaturePad.Forms.NetStandard/bin/Release/SignaturePad.Forms.dll",
					ToDirectory = "output/netstandard",
				},
				new OutputFileCopy {
					FromFile = "./src/SignaturePad.Forms.Droid/bin/Release/SignaturePad.Forms.dll",
					ToDirectory = "output/android",
				},
				new OutputFileCopy {
					FromFile = "./src/SignaturePad.Forms.iOS/bin/Release/SignaturePad.Forms.dll",
					ToDirectory = "output/ios-unified",
				},
				new OutputFileCopy {
					FromFile = "./src/SignaturePad.UWP/bin/Release/SignaturePad.dll",
					ToDirectory = "output/uwp",
				},
				new OutputFileCopy {
					FromFile = "./src/SignaturePad.Forms.UWP/bin/Release/SignaturePad.Forms.dll",
					ToDirectory = "output/uwp",
				},
			}
		}
	},

	Samples = new ISolutionBuilder [] {
		new SolutionBuilder {
			SolutionPath = "./samples/Sample.Android/Sample.Android.sln",
			BuildsOn = BuildPlatforms.Mac | BuildPlatforms.Windows,
			Verbosity = VERBOSITY
		},
		new SolutionBuilder {
			SolutionPath = "./samples/Sample.iOS/Sample.iOS.sln",
			Platform = "iPhone",
			BuildsOn = BuildPlatforms.Mac,
			Verbosity = VERBOSITY
		},
		new SolutionBuilder {
			SolutionPath = "./samples/Sample.Forms/Sample.Forms.Mac.sln",
			BuildsOn = BuildPlatforms.Mac,
			Verbosity = VERBOSITY
		},
		new SolutionBuilder {
			SolutionPath = "./samples/Sample.WP8/Sample.WP8.sln",
			BuildsOn = BuildPlatforms.Windows,
			Verbosity = VERBOSITY
		},
		new SolutionBuilder {
			SolutionPath = "./samples/Sample.UWP/Sample.UWP.sln",
			BuildsOn = BuildPlatforms.Windows,
			Verbosity = VERBOSITY
		},
		new SolutionBuilder {
			SolutionPath = "./samples/Sample.Windows81/Sample.Windows81.sln",
			Platform = "ARM",
			BuildsOn = BuildPlatforms.Windows,
			Verbosity = VERBOSITY
		},
		new SolutionBuilder {
			SolutionPath = "./samples/Sample.WindowsPhone81/Sample.WindowsPhone81.sln",
			Platform = "ARM",
			BuildsOn = BuildPlatforms.Windows,
			Verbosity = VERBOSITY
		},
		new SolutionBuilder {
			SolutionPath = "./samples/Sample.WindowsRuntime81/Sample.WindowsRuntime81.sln",
			Platform = "ARM",
			BuildsOn = BuildPlatforms.Windows,
			Verbosity = VERBOSITY
		},
		new SolutionBuilder {
			SolutionPath = "./samples/Sample.Forms/Sample.Forms.Win.sln",
			BuildsOn = BuildPlatforms.Windows,
			Verbosity = VERBOSITY
		},
		new SolutionBuilder {
			SolutionPath = "./samples/Sample.Forms/Sample.Forms.Win.VS2015.sln",
			Platform = "ARM",
			BuildsOn = BuildPlatforms.Windows,
			Verbosity = VERBOSITY
		},
	},

	NuGets = new [] {
		new NuGetInfo {
			NuSpec = "./nuget/Xamarin.Controls.SignaturePad.nuspec",
			BuildsOn = BuildPlatforms.Mac | BuildPlatforms.Windows,
		},
		new NuGetInfo {
			NuSpec = "./nuget/Xamarin.Controls.SignaturePad.Forms.nuspec",
			BuildsOn = BuildPlatforms.Mac | BuildPlatforms.Windows,
		},
	},

	Components = new [] {
		new Component {
			ManifestDirectory = "./component",
			BuildsOn = BuildPlatforms.Mac | BuildPlatforms.Windows,
		},
	},
};

Task ("nuget")
	.IsDependentOn ("nuget-base")
	.Does (() =>
{
	DeleteFiles("./component/*.xam");
	DeleteFiles("./output/*.xam");
});

SetupXamarinBuildTasks (buildSpec, Tasks, Task);

Task ("CI")
	.IsDependentOn ("libs")
	.IsDependentOn ("nuget")
	.IsDependentOn ("component")
	.IsDependentOn ("samples");

RunTarget (TARGET);
