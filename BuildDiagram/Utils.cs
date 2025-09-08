using Microsoft.Extensions.FileSystemGlobbing;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

static class Utils
{
    public static string FindSolutionFile([CallerFilePath] string? root = null) => Path.GetDirectoryName(root) switch
    {
        null => throw new Exception("Could nt find solution file"),
        var parentDirectory => Directory.EnumerateFiles(parentDirectory)
            .FirstOrDefault(file => Path.GetExtension(file) is ".slnx")
            ?? FindSolutionFile(parentDirectory)
    };

    public static string[] GetSeenProjects(string solutionFolder)
    {
        var seenFilePath = Path.Combine(solutionFolder, "Seen.txt");
        if (File.Exists(seenFilePath))
        {
            return [..File.ReadLines(seenFilePath)];
        }
        return [];
    }

    public static Project[] GetProjects(string solutionFolder)
    {
        var repoRootPath = Path.Combine(solutionFolder, "ms-repos");

        var repos = Directory.EnumerateDirectories(repoRootPath);

        var matcher = new Matcher(StringComparison.OrdinalIgnoreCase)
            .AddInclude("**/Microsoft.Extensions.*.csproj")
            .AddExclude("**/test*/**")
            .AddExclude("**/gen/**")
            .AddExclude("**/ref/**")
            .AddExclude("**/tools/**")
            .AddExclude("**/perf/**")
            .AddExclude("**/bench/**")
            .AddExclude("**/*Test*.csproj")
            ;

        var ignoreFilePath = Path.Combine(solutionFolder, "Ignored.txt");
        if (File.Exists(ignoreFilePath))
        {
            foreach (var ignored in File.ReadLines(ignoreFilePath))
            {
                System.Console.WriteLine($"Ignoring {ignored}");
                matcher.AddExclude($"**/{ignored.Trim()}.csproj");
            }
        }

        return
        [
            ..
            from repoPath in repos.AsParallel()
            let repo = Path.GetFileName(repoPath)
            from file in matcher.GetResultsInFullPath(repoPath).AsParallel()
            let project = new Project(file, repo)
            orderby project.Name
            select project
        ];
    }

    public static string ToNodeId(string package) => $"node{string.Join(null, package.Split('.').Skip(2))}";

    public static string[] GetDependencies(string projectPath) => [
        ..
            from line in File.ReadLines(projectPath)
            let isProjectReference = line.Contains("ProjectReference")
            let isPackageReference = line.Contains("PackageReference") || line.Contains("Reference")
            where isPackageReference || isPackageReference
            let match = Regex.Match(line, @"Include=\""([^""]*)\""")
            where match.Success
            let depPath = match.Result("$1").Replace("$", "")
            let depName = isProjectReference
                ? Path.GetFileNameWithoutExtension(depPath)
                : Path.GetFileName(depPath)
            select depName
    ];
}