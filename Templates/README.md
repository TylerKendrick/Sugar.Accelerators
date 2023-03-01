# Sugar Templates

Each of the templates in this directory are used to generate other Sugar projects.

## Template

The template project is used to generate "dotnet new" template projects. This allows developers to more easily generate the project and file structures to build a "dotnet new" template. 

install:
```cli
dotnet new install sugar-template
```

usage:
```cli
dotnet new sugar-template -n PROJECT_NAME
```

## Sdk

The sdk template sets up file structures and property groups to create new MSBuild Sdks. Because this feature isn't well documented by microsoft, the template project more easily allows developers to structure their build correctly to be registered by NuGet as an sdk package.

install:
```cli
dotnet new install sugar-sdk
```

usage:
```cli
dotnet new sugar-sdk -n PROJECT_NAME
```

## Core

The core template is used to generate class libraries with the Sugar MSBuild SDK.

install:
```cli
dotnet new install sugar-lib
```

usage:
```cli
dotnet new sugar-lib -n PROJECT_NAME
```

## Generator

The generator template is used to create new Roslyn Source Generators. Because of the limitations and specific configurations at the project level that are necessary for generators to register correctly, this template aims to handle all of the work around building a generator package.

install:
```cli
dotnet new install sugar-gen
```

usage:
```cli
dotnet new sugar-gen -n PROJECT_NAME
```

## Testing

The testing template aims to setup all dependencies used for testing and to disable typical build processes.

install:
```cli
dotnet new install sugar-test
```

usage:
```cli
dotnet new sugar-test -n PROJECT_NAME
```

## Framework Projects

The Sugar Framework uses an opinionated project/package structure to separate data, abstractions, application logic, and hosting concerns. Each of the project types are built to reference eachother and work together.

These projects required build tasks located in ``` Directory.Build.targets ```.

### Build tasks

#### refresh-sugar-abstractions

This task runs before ``` _GenerateRestoreProjectPathWalk ``` to rebuild the project dependencies for the abstractions project. It requires the ``` sugar-serialization ``` project template to be installed in order to build the project.

#### refresh-sugar-application

This task runs before ``` _GenerateRestoreProjectPathWalk ``` to rebuild the project dependencies for the application project. It requires the ``` sugar-abstractions ``` and ``` sugar-serialization ``` project templates to be installed in order to build the project.

### Projects

#### Serialization

install:
```cli
dotnet new install sugar-grpc
```

usage:
```cli
dotnet new sugar-grpc -n PROJECT_NAME
```

#### Abstractions

install:
```cli
dotnet new install sugar-abstractions
```

usage:
```cli
dotnet new sugar-abstractions -n PROJECT_NAME
```

#### Application

install:
```cli
dotnet new install sugar-application
```

usage:
```cli
dotnet new sugar-application -n PROJECT_NAME
```
