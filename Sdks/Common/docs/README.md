# Sugar.Accelerators.Sdks.Common

`Sugar.Accelerators.Sdks.Common` is an MSBuildSdk package that provides common functionality for Sugar.Accelerators projects. This package should be used as a development-time dependency.

## Installation

To install `Sugar.Accelerators.Sdks.Common`, run the following command in the Package Manager Console:
```dotnet add package Sugar.Accelerators.Sdk.Common```


## Usage and Version Resolution

To use `Sugar.Accelerators.Sdk.Common`, add the following to your project file:

```xml
<Project Sdk="Sugar.Accelerators.Sdk.Common">

</Project>
```

To change the version of the SDK used by your project, you can modify the `global.json` file in the root directory of your project. Here is an example `global.json` file that specifies the version `1.0.44-beta-ga07cb66f8d` of `Sugar.Accelerators.Sdk.Common`:

```json
{
  "msbuild-sdks": {
    "Sugar.Accelerators.Sdk.Common": "1.0.44-beta-ga07cb66f8d"
  }
}
```
For more information on how to configure the global.json file, please refer to the [documentation](https://learn.microsoft.com/en-us/dotnet/core/tools/global-json).
## Features

`Sugar.Accelerators.Sdk.Common` includes the following features:

### Autodocumentation

This package includes the `DefaultDocumentation` NuGet package, which outputs reference docs into a `.docs` folder.

### Root Namespace

The root namespace is set by default as the `$(MSBuildProjectName)`.

### Common Properties

The following common properties are defined:

```xml
<PropertyGroup Label="Common Properties">
  <ImplicitUsings>enable</ImplicitUsings>
  <Nullable>enable</Nullable>
  <CodeAnalysisTreatWarningsAsErrors>false</CodeAnalysisTreatWarningsAsErrors>
  <EnforceCodeStyleInBuild>true</EnforceCodeStyleInBuild>
</PropertyGroup>
```

### InternalsVisibleTo

`InternalsVisibleTo` is applied to assemblies of the name `$(MSBuildProjectName).Tests`.

### Default TargetFramework

If no TargetFramework is supplied, the default TargetFramework will be set to `net7.0`.

