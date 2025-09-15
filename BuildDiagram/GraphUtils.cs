using OpenSoftware.DgmlTools;
using OpenSoftware.DgmlTools.Builders;
using OpenSoftware.DgmlTools.Model;

static class GraphUtils
{
    public static DirectedGraph BuildGraph(Project[] projects, string[] seenProjects)
    {
        Queue<Project> toProcess = [];
        toProcess.Enqueue(projects.Single(x => x.Name == "Microsoft.Extensions.ServiceDiscovery"));
        HashSet<Project> processed = [];

        //while (toProcess.Count > 0)
        //{
        //    var current = toProcess.Dequeue();
        //    System.Console.WriteLine($"Processing: {current.Name}");
        //    foreach (var dep in current.Dependencies)
        //    {
        //        System.Console.WriteLine($"\t{dep}");
        //        var dependency = projects.SingleOrDefault(x => x.Name == dep);
        //        if (dependency != null)
        //        {
        //            toProcess.Enqueue(dependency);
        //        }
        //    }
        //    processed.Add(current);
        //}

        //System.Console.WriteLine($"Project count: {processed.Count}");
        processed = [.. from p in projects where p.Repo is "runtime" select p];
        //processed = [.. projects];


        var builder = new DgmlBuilder(
            new InvestigationAnalysis(seenProjects),
            new RepoAnalysis()
        )
        {
            NodeBuilders = [
                new NodeBuilder<Project>(proj => new Node
                {
                    Label = proj.ShortName,
                    Id = proj.NodeId,
                    Properties = {
                        ["Repo"] = proj.Repo
                    }
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

        var graph = builder.Build([.. processed]);

        return graph;
    }
}
