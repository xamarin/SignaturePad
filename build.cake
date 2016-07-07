#tool nuget:?package=XamarinComponent&version=1.1.0.29

#addin nuget:?package=Cake.Xamarin.Build&version=1.0.14.0
#addin nuget:?package=Cake.Xamarin

BuildSpec buildSpec = null;


var TARGET = Argument ("t", Argument ("target", "Default"));

buildSpec = new BuildSpec () {

	Libs = new ISolutionBuilder [] { 
		new DefaultSolutionBuilder {
			SolutionPath = "src/SignaturePad.Mac.sln",
			BuildsOn = BuildPlatforms.Mac,
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
					FromFile = "./src/SignaturePad.Forms.Droid/bin/Release/SignaturePad.Forms.Droid.dll",
					ToDirectory = "output/android",
				},
				new OutputFileCopy {
					FromFile = "./src/SignaturePad.Forms.iOS/bin/Release/SignaturePad.Forms.iOS.dll",
					ToDirectory = "output/ios-unified",
				},
			}
		},
		new DefaultSolutionBuilder {
			SolutionPath = "src/SignaturePad.sln",
			BuildsOn = BuildPlatforms.Windows,
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
					FromFile = "./src/SignaturePad.Forms.Droid/bin/Release/SignaturePad.Forms.Droid.dll",
					ToDirectory = "output/android",
				},
				new OutputFileCopy {
					FromFile = "./src/SignaturePad.Forms.iOS/bin/Release/SignaturePad.Forms.iOS.dll",
					ToDirectory = "output/ios-unified",
				},
				new OutputFileCopy {
					FromFile = "./src/SignaturePad.WP8/bin/Release/SignaturePad.dll",
					ToDirectory = "output/wp8",
				},
				new OutputFileCopy {
					FromFile = "./src/SignaturePad.Forms.WindowsPhone/bin/Release/SignaturePad.Forms.WindowsPhone.dll",
					ToDirectory = "output/wp8",
				},
			}
		}
	},

	Samples = new ISolutionBuilder [] {
		new DefaultSolutionBuilder { SolutionPath = "./samples/Sample.Android/Sample.Android.sln", BuildsOn = BuildPlatforms.Mac }, 
		new IOSSolutionBuilder { SolutionPath = "./samples/Sample.iOS/Sample.iOS.sln", BuildsOn = BuildPlatforms.Mac },
		new IOSSolutionBuilder { SolutionPath = "./samples/Sample.Forms/Sample.Forms.Mac.sln", BuildsOn = BuildPlatforms.Mac },
		new DefaultSolutionBuilder { SolutionPath = "./samples/Sample.WP8/Sample.WP8.sln", BuildsOn = BuildPlatforms.Windows }, 
		new DefaultSolutionBuilder { SolutionPath = "./samples/Sample.Forms/Sample.Forms.Win.sln", BuildsOn = BuildPlatforms.Windows }, 
	},

	NuGets = new [] {
		new NuGetInfo { NuSpec = "./nuget/Xamarin.Controls.SignaturePad.nuspec", BuildsOn = BuildPlatforms.Mac },
		new NuGetInfo { NuSpec = "./nuget/Xamarin.Controls.SignaturePad.Forms.nuspec", BuildsOn = BuildPlatforms.Mac },
	},

	Components = new [] {
		new Component {ManifestDirectory = "./component", BuildsOn = BuildPlatforms.Mac },
	},
};

SetupXamarinBuildTasks (buildSpec, Tasks, Task);

RunTarget (TARGET);