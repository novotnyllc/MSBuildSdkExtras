# Using the Extras Sdk

## Migrate from previous versions

If you're using v1.6+ of the Extras SDK, then you would know that few properties have been changed, and the help is provided as a warning to use the new property names.

If you're not using the new version, and/or wish to migrate your build system to support this version, here are the instructions to do just that:

0. BACK UP! BACK UP! (if you're not using version control)

1. First remove any workarounds (check the closed issues first) that you've applied using the previous Extras SDK version.

2. If you're using Visual Studio IDE v15.6+ (if you can update, then update) and use SDK style.

Your project diff:

```diff
- <Project Sdk="Microsoft.NET.Sdk">
+ <Project Sdk="MSBuild.Sdk.Extras">
  <!-- OTHER PROPERTIES -->
  <PropertyGroup>
    <TargetFrameworks>net46;uwp10.0;tizen40</TargetFrameworks>
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

More info: [ReadMe](../README.md)

3. Remove any UWP/Tizen package referneces as they are implicitly included if you're using SDK style.

4. Rename these properties to the new ones

### User facing properties

| OLD Property                                    | NEW Property/Behaviour                                             |
| ---                                             | ----                                                               |
| `ExtrasSkipLibraryLayout`                       | `SkipWindowsLibraryLayout`                                         |
| `_SdkSetAndroidResgenFile`                      | `IncludeAndroidResgenFile`                                         |
| `SuppressWarnIfOldSdkPack`                      | `IgnoreOldSdkWarning`                                              |
| `ExtrasUwpMetaPackageVersion`                   | `_ImplicitPlatformPackageVersion` + `TargetFramework` condition    |
| `ExtrasImplicitPlatformPackageDisabled`         | `DisableImplicitFrameworkReferences` + `TargetFramework` condition |
| `ExtrasImplicitPlatformPackageIsPrivate`        | Auto set for library like items                                    |
| `EmbeddedResourceGeneratorVisibilityIsInternal` | opposite of `EmbeddedResourceGeneratedCodeIsPublic`                |

5. If you're using it for WinForms/WPF, you can set `EnableWpfProjectSetup`/`EnableWinFormsProjectSetup` to `true` to include required references and default items.

6. That's it you're done! If you find any issues during build, check with the [Templates/TestProjects](../TestProjects) included and create an issue if that doesn't help.

## Customizing your Build setup

TBD