# MS Extensions Exploration

This repo contains a tool to help you explore the packages in the Microsoft Extensions namespace.

When you sit down to explore, run the tool and and open the generated diagram. Pick one of the Yellow (Next) packages and locate it's source code. Have a read. Write some spike code. Once you are happy that you understand the basics of the project, mark it as Seen and repeat the process.

## Setup

There are submodules in the `ms-repos` folder that point to the Microsoft repositories which contain the source code of these packages. Each of these will need to be initialized before running the tool.

## Running

Execute the code:

```shell
dotnet run --project BuildDiagram
```

This will generate a files called `Exploration.dgml` next to the solution file. This file can be opened in Visual Studio and shows the relationships of all Microsoft Extensions packages to each other.

The nodes are color coded:

- Green: Has been seen (see below)
- Yellow: Has no unseen dependencies
- Orange: Has only Green and Yellow dependencies

### Configuration

The tool can be controlled by two text files in the same folder as the solution file.

#### `Ignored.txt`

Place one project name on each line. The tool will ignore these projects. They will not appear in the diagram or be treated as dependencies.

Here is mine:
```
Microsoft.Extensions.AuditReports
Microsoft.Extensions.DependencyModel
Microsoft.Extensions.HostFactoryResolver.Sources
Microsoft.Extensions.Options.Contextual
Microsoft.Extensions.StaticAnalysis
```

#### `Seen.txt`

Place one project name on each line. These projects will be marked as `Seen` (Green in the diagram).