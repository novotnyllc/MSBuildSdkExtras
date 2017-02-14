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
  
Package will be released to NuGet shortly. CI feed is on myget:
`https://www.myget.org/F/msbuildsdkextras/api/v3/index.json`

# Using the Package
To use this package, add a `PackageReference` to your project file like this (specify whatever version of the package or wildcard):

``` xml
<PackageReference Include="MSBuild.Sdk.Extras" Version="1.0.0-rc4.*">
    <PrivateAssets>All</PrivateAssets>
</PackageReference>
```

Setting `<PrivateAssets>All</PrivateAssets>` means that this build-time dependency won't be added as a dependency to any packages you create by
using the Pack targets (`msbuild /t:pack` or `dotnet pack`).

Then, at the end of your project file, either `.csproj` or `.vbproj`, add the following `Import` just before the closing tag

``` xml
<Import Project="$(MSBuildSDKExtrasTargets)" Condition="Exists('$(MSBuildSDKExtrasTargets)')" />
```

This last step is required until https://github.com/Microsoft/msbuild/issues/1045 is resolved.