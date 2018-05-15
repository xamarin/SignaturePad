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
					FromFile = "./src/SignaturePad.Forms/bin/Release/SignaturePad.Forms.dll",
					ToDirectory = "output/netstandard",
				},
				new OutputFileCopy {
					FromFile = "./src/SignaturePad.Android/bin/Release/SignaturePad.dll",
					ToDirectory = "output/android",
				},
				new OutputFileCopy {
					FromFile = "./src/SignaturePad.Forms.Droid/bin/Release/SignaturePad.Forms.dll",
					ToDirectory = "output/android",
				},
				new OutputFileCopy {
					FromFile = "./src/SignaturePad.iOS/bin/unified/Release/SignaturePad.dll",
					ToDirectory = "output/ios",
				},
				new OutputFileCopy {
					FromFile = "./src/SignaturePad.Forms.iOS/bin/Release/SignaturePad.Forms.dll",
					ToDirectory = "output/ios",
				},
			}
		},
		new SolutionBuilder {
			SolutionPath = "src/SignaturePad.sln",
			BuildsOn = BuildPlatforms.Windows,
			Verbosity = VERBOSITY,
			OutputFiles = new [] {
				new OutputFileCopy {
					FromFile = "./src/SignaturePad.Forms/bin/Release/SignaturePad.Forms.dll",
					ToDirectory = "output/netstandard",
				},
				new OutputFileCopy {
					FromFile = "./src/SignaturePad.Android/bin/Release/SignaturePad.dll",
					ToDirectory = "output/android",
				},
				new OutputFileCopy {
					FromFile = "./src/SignaturePad.Forms.Droid/bin/Release/SignaturePad.Forms.dll",
					ToDirectory = "output/android",
				},
				new OutputFileCopy {
					FromFile = "./src/SignaturePad.iOS/bin/unified/Release/SignaturePad.dll",
					ToDirectory = "output/ios",
				},
				new OutputFileCopy {
					FromFile = "./src/SignaturePad.Forms.iOS/bin/Release/SignaturePad.Forms.dll",
					ToDirectory = "output/ios",
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
			SolutionPath = "./samples/Sample.UWP/Sample.UWP.sln",
			BuildsOn = BuildPlatforms.Windows,
			Verbosity = VERBOSITY
		},
		new SolutionBuilder {
			SolutionPath = "./samples/Sample.Forms/Sample.Forms.Mac.sln",
			BuildsOn = BuildPlatforms.Mac,
			Verbosity = VERBOSITY
		},
		new SolutionBuilder {
			SolutionPath = "./samples/Sample.Forms/Sample.Forms.Win.sln",
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
};

SetupXamarinBuildTasks (buildSpec, Tasks, Task);

Task ("CI")
	.IsDependentOn ("libs")
	.IsDependentOn ("nuget")
	.IsDependentOn ("samples");

RunTarget (TARGET);
