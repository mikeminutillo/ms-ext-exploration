using Microsoft.Extensions.FileSystemGlobbing;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

static class Utils
{
    public static string FindSolutionFile([CallerFilePath] string? root = null) => Path.GetDirectoryName(root) switch
    {
        null => throw new Exception("Could not find solution file"),
        var parentDirectory => Directory.EnumerateFiles(parentDirectory)
            .FirstOrDefault(file => Path.GetExtension(file) is ".slnx")
            ?? FindSolutionFile(parentDirectory)
    };

    public static string[] GetSeenProjects(string solutionFolder)
        => Path.Combine(solutionFolder, "Seen.txt") switch
        {
            var seenFilePath => File.Exists(seenFilePath)
                ? File.ReadAllLines(seenFilePath)
                : []
        };

    public static string[] GetIgnoredProjects(string solutionFolder)
        => Path.Combine(solutionFolder, "Ignored.txt") switch
        {
            var ignoredFilePath => File.Exists(ignoredFilePath)
                ? File.ReadAllLines(ignoredFilePath)
                : []
        };

    public static Project[] GetProjects(string repoRootPath, string[] ignoredProjects)
    {
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

        foreach(var ignored in ignoredProjects)
        {
            matcher.AddExclude($"**/{ignored.Trim()}.csproj");
        }

        return
        [
            ..
            from repoPath in Directory.EnumerateDirectories(repoRootPath).AsParallel()
            let repo = Path.GetFileName(repoPath)
            from file in matcher.GetResultsInFullPath(repoPath).AsParallel()
            let project = new Project(file, repo)
            orderby project.Name
            select project
        ];
    }

    public static string ToNodeId(string package) => $"node{string.Join(null, package.Split('.').Skip(2))}";

    public static string[] GetDependencies(string projectPath) => XDocument.Load(projectPath) switch
    {
        var doc => [
            .. Includes(doc, "Reference").Select(Path.GetFileName)!,
            .. Includes(doc, "PackageReference").Select(Path.GetFileName)!,
            .. Includes(doc, "ProjectReference").Select(Path.GetFileNameWithoutExtension)!
        ]
    };

    private static IEnumerable<string> Includes(XDocument doc, string elementName)
        => doc.Descendants(elementName)
            .Attributes("Include")
            .Select(x => x.Value)
            .Select(x => x.Replace("$", ""));
}