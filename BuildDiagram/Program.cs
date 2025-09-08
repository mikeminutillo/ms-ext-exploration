using OpenSoftware.DgmlTools;
using OpenSoftware.DgmlTools.Analyses;
using OpenSoftware.DgmlTools.Builders;
using OpenSoftware.DgmlTools.Model;

var solutionFile = Utils.FindSolutionFile();
var solutionFolder = Path.GetDirectoryName(solutionFile);

var projects = Utils.GetProjects(solutionFolder!);

var seenProjects = Utils.GetSeenProjects(solutionFolder!);

var builder = new DgmlBuilder(
    new InvestigationAnalysis(seenProjects)
)
{
    NodeBuilders = [
        new NodeBuilder<Project>(proj => new Node
        {
            Category = proj.Subsystem,
            Label = proj.ShortName,
            Id = proj.NodeId,
            Properties = {
                ["Repo"] = proj.Repo
            }
        })
    ],
    CategoryBuilders = [
        new CategoryBuilder<Project>(proj => new Category
        {
            Id = proj.Subsystem,
            Label = proj.Subsystem
        })
    ],
    LinkBuilders = [
        new LinkBuilder<Project>(proj => new Link
        {
            Source = proj.Subsystem,
            Target = proj.NodeId,
            IsContainment = true
        }),
        new LinksBuilder<Project>(proj =>
            from dep in proj.Dependencies
            join project in projects on dep equals project.Name
            select new Link
            {
                Source = proj.NodeId,
                Target = project.NodeId
            })
    ]
};

var graph = builder.Build(projects);

graph.WriteToFile(Path.Combine(solutionFolder!, @"Exploration.dgml"));
