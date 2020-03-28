#tool nuget:?package=vswhere
#addin nuget:?package=Cake.Boots

///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var CURRENT_PACKAGE_VERSION = "3.2.0";

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var packageVersion = Argument("packageVersion", CURRENT_PACKAGE_VERSION);
var majorVersion = $"{packageVersion.Substring(0, packageVersion.IndexOf("."))}.0.0.0";
var buildVersion = Argument("buildVersion", EnvironmentVariable("APPVEYOR_BUILD_NUMBER") ?? "");
if (!string.IsNullOrEmpty(buildVersion)) {
    buildVersion = $"-{buildVersion}";
}


MSBuildSettings CreateSettings()
{
    var ANDROID_HOME = EnvironmentVariable ("ANDROID_HOME") ?? 
                        (IsRunningOnWindows () ?
                                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + "/Android/android-sdk/" :
                                "/usr/local/share/android-sdk");

    Information($"ANDROID_HOME: {ANDROID_HOME}");

    var settings = new MSBuildSettings 
    {
        Configuration = configuration,
        MSBuildPlatform = MSBuildPlatform.x86,
        Properties = {
            { "AssemblyVersion", new [] { majorVersion } },
            { "Version", new [] { packageVersion } },
        },
        ArgumentCustomization = args => {
            return args.Append("/restore")
                    .Append($"/p:AndroidSdkDirectory=\"{ANDROID_HOME}\"")
                    .Append($"/p:AndroidNdkDirectory=\"{ANDROID_HOME}\\ndk-bundle\"");
            }
    };

    if (IsRunningOnWindows())
    {
        DirectoryPath vsLatest = VSWhereLatest(new VSWhereLatestSettings{IncludePrerelease = true});
        FilePath msBuildPath = vsLatest?.CombineWithFilePath("./MSBuild/Current/Bin/MSBuild.exe");       

        if (!FileExists(msBuildPath))
        {
            throw new Exception($"Failed to find MSBuild: {msBuildPath}");
        }

        Information("Building using MSBuild at " + msBuildPath);
        settings.ToolPath = msBuildPath;
    }
    else
    {
        settings.ToolPath = Context.Tools.Resolve("msbuild");
    }

    return settings;
}

///////////////////////////////////////////////////////////////////////////////
// TASKS
///////////////////////////////////////////////////////////////////////////////

Task("InstallSoftware")
    .WithCriteria(Argument("InstallSoftware", false) == true)
    .Does(async () =>
    {
        await Boots (Product.Mono, ReleaseChannel.Preview);
        await Boots (Product.XamariniOS, ReleaseChannel.Preview);            
        await Boots (Product.XamarinMac, ReleaseChannel.Preview);
        await Boots (Product.XamarinAndroid, ReleaseChannel.Preview);
    });

Task("libs")
    .Does(() =>
{
    var sln = IsRunningOnWindows() ? "./src/SignaturePad.sln" : "./src/SignaturePad.Mac.sln";
    MSBuild(sln, CreateSettings());

    EnsureDirectoryExists("./output/android/");
    EnsureDirectoryExists("./output/wpf/");
    EnsureDirectoryExists("./output/gtk/");
    EnsureDirectoryExists("./output/mac/");
    EnsureDirectoryExists("./output/ios/");
    EnsureDirectoryExists("./output/uwp/SignaturePad/Themes");    
    EnsureDirectoryExists("./output/netstandard/");

    CopyFiles($"./src/SignaturePad.Android/bin/{configuration}/SignaturePad.*", "./output/android/");
    CopyFiles($"./src/SignaturePad.iOS/bin/{configuration}/SignaturePad.*", "./output/ios/");
    CopyFiles($"./src/SignaturePad.MacOS/bin/{configuration}/SignaturePad.*", "./output/mac/");
    CopyFiles($"./src/SignaturePad.WPF/bin/{configuration}/SignaturePad.*", "./output/wpf/");
    CopyFiles($"./src/SignaturePad.GTK/bin/{configuration}/SignaturePad.*", "./output/gtk/");
    CopyFiles($"./src/SignaturePad.UWP/bin/{configuration}/SignaturePad.*", "./output/uwp/");
    CopyFiles($"./src/SignaturePad.UWP/bin/{configuration}/SignaturePad/*", "./output/uwp/SignaturePad");
    CopyFiles($"./src/SignaturePad.UWP/bin/{configuration}/SignaturePad/Themes/*", "./output/uwp/SignaturePad/Themes");

    CopyFiles($"./src/SignaturePad.Forms.Droid/bin/{configuration}/SignaturePad.Forms.*", "./output/android/");
    CopyFiles($"./src/SignaturePad.Forms.iOS/bin/{configuration}/SignaturePad.Forms.*", "./output/ios/");
    CopyFiles($"./src/SignaturePad.Forms.MacOS/bin/{configuration}/SignaturePad.Forms.*", "./output/mac/");
    CopyFiles($"./src/SignaturePad.Forms.WPF/bin/{configuration}/SignaturePad.Forms.*", "./output/wpf/");
    CopyFiles($"./src/SignaturePad.Forms.GTK/bin/{configuration}/SignaturePad.Forms.*", "./output/gtk/");
    CopyFiles($"./src/SignaturePad.Forms.UWP/bin/{configuration}/SignaturePad.Forms.*", "./output/uwp/");
    CopyFiles($"./src/SignaturePad.Forms.UWP/bin/{configuration}/SignaturePad/*", "./output/uwp/SignaturePad/");
    CopyFiles($"./src/SignaturePad.Forms.UWP/bin/{configuration}/SignaturePad/Themes/*", "./output/uwp/SignaturePad/Themes");
    CopyFiles($"./src/SignaturePad.Forms/bin/{configuration}/SignaturePad.Forms.*", "./output/netstandard/");
});

Task("nuget")
    .IsDependentOn("libs")
    .WithCriteria(IsRunningOnWindows())
    .Does(() =>
{
    var nuspecs = GetFiles("./nuget/*.nuspec");
    var settings = new NuGetPackSettings {
        BasePath = ".",
        OutputDirectory = "./output",
        Properties = new Dictionary<string, string> {
            { "configuration", configuration },
            { "version", packageVersion },
        },
    };

    EnsureDirectoryExists("./output");

    NuGetPack(nuspecs, settings);

    settings.Properties["version"] = $"{packageVersion}-preview{buildVersion}";
    NuGetPack(nuspecs, settings);
});

Task("samples")
    .IsDependentOn("libs")
    .Does(() =>
{
    var settings = CreateSettings();
    var settingsIos = CreateSettings();
    settingsIos.ArgumentCustomization = args => {
            return args.Append($"/p:Platform=\"iPhoneSimulator\"");
            };

    if (IsRunningOnWindows()) {
        NuGetRestore("./samples/Sample.Android/Sample.Android.sln");
        MSBuild("./samples/Sample.Android/Sample.Android.sln", settings);

        NuGetRestore("./samples/Sample.iOS/Sample.iOS.sln");
        MSBuild("./samples/Sample.iOS/Sample.iOS.sln", settingsIos);

        NuGetRestore("./samples/Sample.UWP/Sample.UWP.sln");
        MSBuild("./samples/Sample.UWP/Sample.UWP.sln", settings);

        NuGetRestore("./samples/Sample.Forms/Sample.Forms.sln");
        MSBuild("./samples/Sample.Forms/Sample.Forms.sln", settingsIos);
    } else {
        NuGetRestore("./samples/Sample.Android/Sample.Android.sln");
        MSBuild("./samples/Sample.Android/Sample.Android.sln", settings);

        NuGetRestore("./samples/Sample.iOS/Sample.iOS.sln");
        MSBuild("./samples/Sample.iOS/Sample.iOS.sln", settingsIos);

        NuGetRestore("./samples/Sample.Forms/Sample.Forms.Mac.sln");
        MSBuild("./samples/Sample.Forms/Sample.Forms.Mac.sln", settingsIos);
    }

});

Task("Default")
    .IsDependentOn("InstallSoftware")
    .IsDependentOn("libs")
    .IsDependentOn("nuget")
    .IsDependentOn("samples");

RunTarget(target);
