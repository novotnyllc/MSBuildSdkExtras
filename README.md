# MSBuild.Sdk.Extras

This package contains a few extra extensions to the SDK-style projects that are currently not available
in the main SDK. That feature is tracked in [dotnet/sdk#491](https://github.com/dotnet/sdk/issues/491)

The primary goal is to enable multi-targeting without you having to enter in tons of properties within your
`csproj`, `vbproj`, `fsproj`, thus keeping it nice and clean.

See my [blog article](https://oren.codes/2017/01/04/multi-targeting-the-world-a-single-project-to-rule-them-all/) for some background on how to get started. Installing
this package, `MSBuild.Sdk.Extras` adds the missing properties so that you can use any TFM you want.

In short: Create a new .NET Standard class library in VS 2017. Then you can edit the `TargetFramework` to a different TFM (after installing this package), or you can rename `TargetFramework` to `TargetFrameworks` and specify multiple TFM's with a `;` separator.

After building, you can use the `Pack` target to easily create a NuGet package: `msbuild /t:Pack ...`

Few notes:

- This will only work in VS 2017, Visual Studio for Mac. It's possible it will work in Code, but only as an editor as this requires full `msbuild` to build.
- To compile, you'll need the desktop build engine -- `msbuild`. Most of the platforms rely on tasks and utilities that are not yet cross platform
- You must install the tools of the platforms you intend to build. For Xamarin, that means the Xamarin Workload; for UWP install those tools as well.

NuGet: `MSBuild.Sdk.Extras`

[![MSBuild.Sdk.Extras](https://img.shields.io/nuget/v/MSBuild.Sdk.Extras.svg)](https://nuget.org/packages/MSBuild.Sdk.Extras)

MyGet CI feed: `https://myget.org/F/msbuildsdkextras/api/v3/index.json`

[![MSBuild.Sdk.Extras](https://img.shields.io/myget/msbuildsdkextras/v/MSBuild.Sdk.Extras.svg)](https://myget.org/gallery/msbuildsdkextras)

## Using the package (VS 2017 15.6+)

Visual Studio 2017 Update 6 includes support for SDK's resolved from NuGet. That makes using these extras much easier. This approach is recommended and support for the "old" way will be removed in a future release of the Extras.

After creating a new .NET Standard class library, replace:
`<Project Sdk="Microsoft.NET.Sdk">` with `<Project Sdk="MSBuild.Sdk.Extras/1.2.2">`. That's it. You do not need to specify the UWP or Tizen meta-packages as they'll be automatically included.

For solutions with more than one project using the Extras, it's recommended to put the version in your `global.json` next to your solution like this:

```json
{
    "msbuild-sdks": {
        "MSBuild.Sdk.Extras": "1.2.2"
    }
}
```
Then, in your project file, just use `<Project Sdk="MSBuild.Sdk.Extras">`, and it'll use the version from the `global.json`.

More information on how SDK's are resolved can be found [here](https://docs.microsoft.com/en-us/visualstudio/msbuild/how-to-use-project-sdk#how-project-sdks-are-resolved).


## Using the Package (pre VS 2017 15.6)

**IMPORTANT** This approach should be considered deprecated with the upcoming release of 15.6. Update your projects to use the mechanism above.

To use this package, add a `PackageReference` to your project file like this (specify whatever version of the package or wildcard):

``` xml
<PackageReference Include="MSBuild.Sdk.Extras" Version="1.1.0" PrivateAssets="All" />
```

Setting `PrivateAssets="All"` means that this build-time dependency won't be added as a dependency to any packages you create by
using the Pack targets (`msbuild /t:Pack` or `dotnet pack`).

Then, at the end of your project file, either `.csproj`, `.vbproj` or `.fsproj`, add the following `Import` just before the closing tag

``` xml
<Import Project="$(MSBuildSDKExtrasTargets)" Condition="Exists('$(MSBuildSDKExtrasTargets)')" />
```

This last step is required until [Microsoft/msbuild#1045](https://github.com/Microsoft/msbuild/issues/1045) is resolved.

## Targeting UWP

If you plan to target UWP, then you must include the UWP meta-package in your project as well, something like this:

``` xml
<ItemGroup Condition=" '$(TargetFramework)' == 'uap10.0' ">
  <PackageReference Include="Microsoft.NETCore.UniversalWindowsPlatform" Version="5.4.0" />
</ItemGroup>
```

Starting with VS 2017 15.4, you can specify the `TargetPlatformMinVersion` with the TFM. The exact value depends on your installed Windows SDK; it may be something like `uap10.0.16278`. You can even multi-target to support older versions too, so have a `uap10.0` and `uap10.0.16278` target with different capabilities.

## Targeting Tizen

If you plan to target Tizen, then you should include the following meta-package:

```xml
<ItemGroup Condition=" '$(TargetFramework)' == 'tizen30' ">
  <PackageReference Include="Tizen.NET" Version="3.0.0" />
</ItemGroup>
```

## Targeting UWP, Windows 8.x, Windows Phone 8.1, etc. using the 1.0 SDK tooling

**This workaround is needed when using the SDK 1.x tooling. Recommendation is to use the 2.0+ SDK even if targeting 1.x**

If you're targeting a WinRT platform and you use the `Pack` target, there's an important workaround needed to ensure
that the `.pri` files are included in the package correctly. When you call `Pack`, you also must override `NuGetBuildTasksPackTargets` on the command-line
to ensure the fixed targets get applied. The value you specify must not be a real file.

You also need to add a `PackageReference` to the `NuGet.Build.Tasks.Pack` v4.3.0 to your project., something like this:

`<PackageReference Include="NuGet.Build.Tasks.Pack" Version="4.3.0" PrivateAssets="All" />`

On the command line, you need to invoke something like: `msbuild MyProject.csproj /t:Pack /p:NuGetBuildTasksPackTargets="workaround"`

[NuGet/Home#4136](https://github.com/NuGet/Home/issues/4136) is tracking this.

## Single or multi-targeting

Once this package is configured, you can now use any supported TFM in your `TargetFramework` or `TargetFrameworks` element. The supported TFM families are:

- `netstandard` (.NET Standard)
- `net` (.NET Framework - with support for WPF)
- `net35-client`/`net40-client` (.NET Framework Client profile)
- `netcoreapp` (.NET Core App)
- `wpa` (Windows Phone App 8.1)
- `win` (Windows 8 / 8.1)
- `uap` (UWP)
- `wp` (Windows Phone Silverlight, WP7+)
- `sl` (Silverlight 4+)
- `tizen` (Tizen 3.0+)
- `monoandroid`/`Xamarin.Android`
- `xamarinios` / `Xamarin.iOS`
- `xamarinmac` / `Xamarin.Mac`
- `xamarinwatchos` / `Xamarin.WatchOS`
- `xamarintvos` / `Xamarin.TVOS`
- `portable-`/`portableNN-` (legacy PCL profiles like `portable-net45+win8+wpa81+wp8`)

 For legacy PCL profiles, the order of the TFM's in the list does not matter but the profile must be an exact match
 to one of the known profiles. If it's not, you'll get a compile error saying it's unknown. You can see the full
 list of known profiles here: [Portable Library Profiles by Stephen Cleary](https://portablelibraryprofiles.stephencleary.com/).

 If you try to use a framework that you don't have tools installed for, you'll get an error as well saying to check the tools. In some cases
 this might mean installing an older version of VS (like 2015) to ensure that the necessary targets are installed on the machine.
