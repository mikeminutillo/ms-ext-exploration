using OpenSoftware.DgmlTools.Model;

var solutionFile = Utils.FindSolutionFile();
var solutionFolder = Path.GetDirectoryName(solutionFile)!;
var repoRootPath = Path.Combine(solutionFolder, "ms-repos");

var seenProjects = Utils.GetSeenProjects(solutionFolder);
var ignoredProjects = Utils.GetIgnoredProjects(solutionFolder);

var allProjects = Utils.GetProjects(repoRootPath, ignoredProjects);

var graph = GraphUtils.BuildGraph(allProjects, seenProjects);

graph.WriteToFile(Path.Combine(solutionFolder, @"Exploration.dgml"));
