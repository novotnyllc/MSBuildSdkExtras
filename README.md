# MSBuild.Sdk.Extras

## Summary

This package contains a few extra extensions to the SDK-style projects that are currently not available in `Microsoft.NET.Sdk` SDK. This feature is tracked in [dotnet/sdk#491](https://github.com/dotnet/sdk/issues/491)

The primary goal of this project is to enable multi-targeting without you having to enter in tons of properties within your `csproj`, `vbproj`, `fsproj`, thus keeping it nice and clean.

See the [blog post](https://oren.codes/2017/01/04/multi-targeting-the-world-a-single-project-to-rule-them-all) for more information.

### Package Name: `MSBuild.Sdk.Extras`

[![MSBuild.Sdk.Extras](https://img.shields.io/nuget/v/MSBuild.Sdk.Extras.svg)](https://nuget.org/packages/MSBuild.Sdk.Extras)
[![MSBuild.Sdk.Extras](https://img.shields.io/myget/msbuildsdkextras/v/MSBuild.Sdk.Extras.svg)](https://myget.org/feed/msbuildsdkextras/package/nuget/MSBuild.Sdk.Extras)
[![MSBuildSDKExtras](https://img.shields.io/badge/MSBuildSDKExtras-myget-brightgreen.svg)](https://myget.org/gallery/msbuildsdkextras)

### Getting started (VS 15.6+)

Visual Studio 2017 Update 6 (aka _v15.6_) includes support for SDK's resolved from NuGet. That makes using the custom Sdks much easier.

#### Using the SDK

1. Create a new project
    - .NET Core console app or .NET Standard class library.
    - With your exisiting SDK-style project.
    - With the templates in the repo's [TestProjects](/TestProjects) folder.

2. Replace `Microsoft.NET.Sdk` with `MSBuild.Sdk.Extras` to the project's top-level `Sdk` attribute.

3. You have to tell MSBuild that the `Sdk` should resolve from NuGet by
    - Adding a `global.json` containing the Sdk name and version.
    - Appending a version info to the `Sdk` attribute value.

4. Then you can edit the `TargetFramework` to a different TFM, or you can rename `TargetFramework` to `TargetFrameworks` and specify multiple TFM's with a `;` separator.

The final project should look like this:

```xml
<Project Sdk="MSBuild.Sdk.Extras">
  <PropertyGroup>
    <TargetFrameworks>net46;uap10.0.16299;tizen40</TargetFrameworks>
  </PropertyGroup>
</Project>
```

You can put the `global.json` file next to your solution (use the latest available version of the extras):

```json
{
    "msbuild-sdks": {
        "MSBuild.Sdk.Extras": "1.6.0"
    }
}
```

Then, all of your project files, from that directory forward, uses the version from the `global.json` file.
This would be a preferred solution for all the projects in your solution.

Then again, you might want to override the version for just one project _OR_ if you have only one project in your solution (without adding `global.json`), you can do so like this:

```xml
<Project Sdk="MSBuild.Sdk.Extras/1.6.0">
  <PropertyGroup>
    <TargetFrameworks>net46;uap10.0.16299;tizen40</TargetFrameworks>
  </PropertyGroup>
</Project>
```

That's it. You do not need to specify the UWP or Tizen meta-packages as they'll be automatically included.
After that, you can use the `Restore`, `Build`, `Pack` targets to restore packages, build the project and create NuGet packages. E.g.: `msbuild /t:Pack ...`

#### Important to Note

- It will only work with Visual Studio IDE (Windows/Mac) as it requires the desktop `msbuild` and the target Platform SDKs which are not cross platform.
- It might work in Visual Studio Code, but you have to configure build tasks in `launch.json` to use desktop `msbuild` to build.
- You must install the tools of the platforms you intend to build. For Xamarin, that means the Xamarin Workload; for UWP install those tools as well.

More information on how SDK's are resolved can be found [here](https://docs.microsoft.com/en-us/visualstudio/msbuild/how-to-use-project-sdk#how-project-sdks-are-resolved).

### Migrate from the old way (VS pre-15.6)

For those who are using in a `PackageReference` style, you can't do that with v2.0+ of this package. So update VS to 15.6+ and manually upgrade your projects as shown below:

1. The same as above, replace the Sdk attribute's value.
2. Remove the workaround import specified with the old way. The import property should be `MSBuildSdkExtrasTargets`.
3. Do a trial build and then compare your project with the templates in the repo's [TestProjects](/TestProjects) folder to troubleshoot any issues if you encounter them.
4. Please file a issue. 

Your project diff:

```diff
- <Project Sdk="Microsoft.NET.Sdk">
+ <Project Sdk="MSBuild.Sdk.Extras">
  <!-- OTHER PROPERTIES -->
  <PropertyGroup>
    <TargetFrameworks>net46;uap10.0.16299;tizen40</TargetFrameworks>
  </PropertyGroup>

  <ItemGroup>
-    <PackageReference Include="MSBuild.Sdk.Extras" Version="1.6.0" PrivateAssets="All"/>
  <!-- OTHER PACKAGES/INCLUDES -->
  </ItemGroup>

-  <Import Project="$(MSBuildSdkExtrasTargets)" Condition="Exists('$(MSBuildSdkExtrasTargets)')"/>
  <!-- OTHER IMPORTS -->
</Project>
```

```diff
- PackageReference style
+ SDK style
```

**Note**: If you're using Visual Studio for Mac, currently there's no support for resolving SDKs from NuGet. Until VS for Mac supports it, you can use `PackageReference` style. Also you have to include any UWP or Tizen meta-package manually. If you are already using the package, just update it to get the new fixes.

## Release Notes

### 1.6.0

 - A few properties have been changed, and the help is provided as a warning to use the new property names.

   | Old Property                                    | New Property/Behaviour                                             |
   | ---                                             | ----                                                               
   | `SuppressWarnIfOldSdkPack`                      | `ExtrasIgnoreOldSdkWarning`                                              |
   | `ExtrasUwpMetaPackageVersion`                   | `ExtrasImplicitPlatformPackageVersion` + `TargetFramework` condition    |
   | `ExtrasImplicitPlatformPackageDisabled`         | `DisableImplicitFrameworkReferences` + `TargetFramework` condition |
   | `EmbeddedResourceGeneratorVisibilityIsInternal` | opposite of `ExtrasExtrasEmbeddedResourceGeneratedCodeIsPublic`                |

 - Support for WPF and Windows Forms requires an opt-in property to enable:
Set `ExtrasEnableWpfProjectSetup`/`ExtrasEnableWinFormsProjectSetup` to `true` to include required references and default items.

## Single or multi-targeting

Once this package is configured, you can now use any supported TFM in your `TargetFramework` or `TargetFrameworks` element. The supported TFM families are:

- `netstandard` (.NET Standard)
- `netcoreapp` (.NET Core App)
- `net` (.NET Framework)
- `net35-client`/`net40-client` (.NET Framework legacy Client profile)
- `wpa` (Windows Phone App 8.1)
- `win` (Windows 8 / 8.1)
- `uap` (Windows 10 / UWP)
- `wp` (Windows Phone Silverlight, WP7+)
- `sl` (Silverlight 4+)
- `tizen` (Tizen 3+)
- `xamarin.android`
- `xamarin.ios`
- `xamarin.mac`
- `xamarin.watchos`
- `xamarin.tvos`
- `portableNN-`/`portable-` (legacy PCL profiles like `portable-net45+win8+wpa81+wp8`)

 For legacy PCL profiles, the order of the TFM's in the list does not matter but the profile must be an exact match to one of the known profiles. If it's not, you'll get a compile error saying it's unknown. You can see the full list of known profiles here: [Portable Library Profiles by Stephen Cleary](https://portablelibraryprofiles.stephencleary.com/).

 If you try to use a framework that you don't have tools installed for, you'll get an error as well saying to check the tools. In some cases this might mean installing an older version of Visual Studio IDE (_like 2015_) to ensure that the necessary targets are installed on the machine.
