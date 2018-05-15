///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var packageVersion = Argument("packageVersion", "1.0.0");
var majorVersion = packageVersion.Substring(0, packageVersion.IndexOf("."));

///////////////////////////////////////////////////////////////////////////////
// TASKS
///////////////////////////////////////////////////////////////////////////////

Task("libs")
    .Does(() =>
{
    var sln = IsRunningOnWindows() ? "./src/SignaturePad.sln" : "./src/SignaturePad.Mac.sln";

    MSBuild(sln, new MSBuildSettings {
        Verbosity = Verbosity.Minimal,
        Configuration = configuration,
        PlatformTarget = PlatformTarget.MSIL,
        MSBuildPlatform = MSBuildPlatform.x86,
        ArgumentCustomization = args => args.Append("/restore"),
        Properties = {
            { "AssemblyVersion", new [] { $"{majorVersion}.0.0.0" } },
            { "Version", new [] { packageVersion } },
        },
    });

    EnsureDirectoryExists("./output/android/");
    EnsureDirectoryExists("./output/ios/");
    EnsureDirectoryExists("./output/uwp/");
    EnsureDirectoryExists("./output/uwp/Themes");
    EnsureDirectoryExists("./output/netstandard/");

    CopyFiles($"./src/SignaturePad.Android/bin/{configuration}/SignaturePad.*", "./output/android/");
    CopyFiles($"./src/SignaturePad.iOS/bin/{configuration}/SignaturePad.*", "./output/ios/");
    CopyFiles($"./src/SignaturePad.UWP/bin/{configuration}/SignaturePad.*", "./output/uwp/");
    CopyFiles($"./src/SignaturePad.UWP/bin/{configuration}/Themes/*", "./output/uwp/Themes");

    CopyFiles($"./src/SignaturePad.Forms.Android/bin/{configuration}/SignaturePad.Forms.*", "./output/android/");
    CopyFiles($"./src/SignaturePad.Forms.iOS/bin/{configuration}/SignaturePad.Forms.*", "./output/ios/");
    CopyFiles($"./src/SignaturePad.Forms.UWP/bin/{configuration}/SignaturePad.Forms.*", "./output/uwp/");
    CopyFiles($"./src/SignaturePad.Forms.UWP/bin/{configuration}/Themes/*", "./output/uwp/Themes");
    CopyFiles($"./src/SignaturePad.Forms/bin/{configuration}/SignaturePad.Forms.*", "./output/netstandard/");
});

Task("nuget")
    .IsDependentOn("libs")
    .WithCriteria(IsRunningOnWindows())
    .Does(() =>
{
    var nuget = Context.Tools.Resolve("nuget.exe");

    EnsureDirectoryExists("./output");
    NuGetPack(GetFiles("./nuget/*.nuspec"), new NuGetPackSettings {
        BasePath = ".",
        OutputDirectory = "./output",
        Properties = new Dictionary<string, string> {
            { "configuration", configuration },
            { "version", packageVersion },
        },
    });
});

Task("samples")
    .IsDependentOn("libs")
    .Does(() =>
{
    var settings = new MSBuildSettings {
        Verbosity = Verbosity.Minimal,
        Configuration = configuration,
        PlatformTarget = PlatformTarget.MSIL,
        MSBuildPlatform = MSBuildPlatform.x86,
        ArgumentCustomization = args => args.Append("/restore"),
    };

    if (!IsRunningOnWindows()) {
        MSBuild("./samples/Sample.Android/Sample.Android.sln", settings);
        MSBuild("./samples/Sample.iOS/Sample.iOS.sln", settings);
        MSBuild("./samples/Sample.UWP/Sample.UWP.sln", settings);
        MSBuild("./samples/Sample.Forms/Sample.Forms.sln", settings);
    } else {
        MSBuild("./samples/Sample.Android/Sample.Android.sln", settings);
        MSBuild("./samples/Sample.iOS/Sample.iOS.sln", settings);
        MSBuild("./samples/Sample.Forms/Sample.Forms.Mac.sln", settings);
    }

});

Task("Default")
    .IsDependentOn("libs")
    .IsDependentOn("nuget")
    .IsDependentOn("samples");

Task("CI")
    .IsDependentOn("Default");

RunTarget(target);
