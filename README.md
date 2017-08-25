# MSBuild.Sdk.Extras
This package contains a few extra extensions to the SDK-style projects that are currently not available
in the main SDK. That feature is tracked here: https://github.com/dotnet/sdk/issues/491

The primary goal is to enable multi-targeting without you having to enter in tons of properties within your
`csproj`, thus keeping it nice and clean.

See this [blog article](https://oren.codes/2017/01/04/multi-targeting-the-world-a-single-project-to-rule-them-all/) for some background on how to get started. Installing
this package, `MSBuild.Sdk.Extras` adds the missing properties so that you can use any TFM you want.

In short: Create a new .NET Standard class library in VS 2017. Then you can edit the `TargetFramework` to a different TFM (after installing this package), or you can rename `TargetFramework` to `TargetFrameworks` and specify multiple TFM's with a `;` separator. 
After building, you can use the `pack` target to easily create a NuGet package: `msbuild /t:pack ...` 

Few notes: 

 - This will only work in VS 2017, Visual Studio for Mac. It's possible it will work in Code, but only as an editor.
 - To compile, you'll need the desktop build engine -- `msbuild.exe`. Most of the platforms rely on tasks and utilities that are not yet cross platform
 - You must install the tools of the platforms you intend to build. For Xamarin, that means the Xamarin Workload; for UWP install those tools as well.
  
NuGet: `MSBuild.Sdk.Extras`<br />
[![MSBuild.Sdk.Extras](https://img.shields.io/nuget/v/MSBuild.Sdk.Extras.svg)](https://www.nuget.org/packages/MSBuild.Sdk.Extras)

CI feed is on MyGet:
`https://www.myget.org/F/msbuildsdkextras/api/v3/index.json` <br />
![MSBuild.Sdk.Extras](https://img.shields.io/myget/msbuildsdkextras/v/MSBuild.Sdk.Extras.svg)

# Using the Package
To use this package, add a `PackageReference` to your project file like this (specify whatever version of the package or wildcard):

``` xml
<PackageReference Include="MSBuild.Sdk.Extras" Version="1.0.5" PrivateAssets="all" />
```

Setting `PrivateAssets="all"` means that this build-time dependency won't be added as a dependency to any packages you create by
using the Pack targets (`msbuild /t:pack` or `dotnet pack`).

Then, at the end of your project file, either `.csproj` or `.vbproj`, add the following `Import` just before the closing tag

``` xml
<Import Project="$(MSBuildSDKExtrasTargets)" Condition="Exists('$(MSBuildSDKExtrasTargets)')" />
```

This last step is required until https://github.com/Microsoft/msbuild/issues/1045 is resolved.

## Targeting UWP
If you plan to target UWP, then you must include the UWP meta-package in your project as well, something like this:

``` xml
<ItemGroup Condition=" '$(TargetFramework)' == 'uap10.0' "> 
  <PackageReference Include="Microsoft.NETCore.UniversalWindowsPlatform " Version="5.2.3" /> 
</ItemGroup> 
```

Starting with VS 2017 15.4, you can specify the `TargetPlatformMinVersion` with the TFM. This lets you use NETStandard 2.0. The exact value depends on your installed Windows SDK; it may be something like `uap10.0.16267`. You can multi-target to support older versions too, so have a `uap10.0` and `uap10.0.16267`target with different capabilities. 

## Targeting Tizen
If you plan to target Tizen, then you should include the following meta-package:

```xml
<ItemGroup Condition=" '$(TargetFramework)' == 'tizen30' ">  
  <PackageReference Include="Tizen.NET" Version="3.0.0" />
</ItemGroup>  
```

## Targeting UWP, Windows 8, Windows Phone, etc using the 1.0 SDK tooling
If you're targeting a WinRT platform and you use the `pack` target, there's an important workaround needed to ensure
that the `.pri` files are included in the package correctly. When you call `pack`, you also must override `NuGetBuildTasksPackTargets` on the command-line
to ensure the fixed targets get applied. The value you specify must not be a real file.

For example: `msbuild my.csproj /t:pack /p:NuGetBuildTasksPackTargets="workaround"`

GitHub issue https://github.com/NuGet/Home/issues/4136 is tracking this. This is fixed in the 2.0 SDK tooling and is no longer needed there.

## Single or multi-targeting
Once this package is configured, you can now use any supported TFM in your `TargetFramework` or `TargetFrameworks` element. The supported TFM families are:
 - `netstandard` (.NET Standard)
 - `net` (.NET Framework)
 - `netcoreapp` (.NET Core App)
 - `wpa` (Windows Phone App 8.1)
 - `win` (Windows 8 / 8.1)
 - `uap` (UWP)
 - `wp` (Windows Phone Silverlight, WP7+)
 - `sl` (Silverlight 4+)
 - `tizen` (Tizen 3.0+)
 - `xamarinios` / `Xamarin.iOS`
 - `monoandroid`
 - `xamarinmac` / `Xamarin.Mac`
 - `xamarinwatchos` / `Xamarin.WatchOS`
 - `xamarintvos` / `Xamarin.TVOS`
 - `portable-` (legacy PCL profiles like `portable-net45+win8+wpa81+wp8`)

 For legacy PCL profiles, the order of the TFM's in the list does not matter but the profile must be an exact match 
 to one of the known profiles. If it's not, you'll get a compile error saying it's unknown. You can see the full
 list of known profiles here: https://portablelibraryprofiles.stephencleary.com/.

 If you try to use a framework that you don't have tools installed for, you'll get an error as well saying to check the tools. In some cases
 this might mean installing an older version of VS (like 2015) to ensure that the necessary targets are installed on the machine.
