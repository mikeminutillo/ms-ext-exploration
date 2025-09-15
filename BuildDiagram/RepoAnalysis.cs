using OpenSoftware.DgmlTools.Analyses;
using OpenSoftware.DgmlTools.Model;

class RepoAnalysis : IGraphAnalysis
{
    public void Execute(DirectedGraph graph)
    {
    }

    public IEnumerable<Property> GetProperties(DirectedGraph graph)
    {
        yield return new Property
        {
            Id = "Repo",
            DataType = "System.String",
            Label = "Repository",
            Description = "Which of the repositories is this code in?"
        };
    }

    public IEnumerable<Style> GetStyles(DirectedGraph graph)
    {
        yield return CreateStyle("aspire");
        yield return CreateStyle("aspnetcore");
        yield return CreateStyle("extensions");
        yield return CreateStyle("runtime");

        static Style CreateStyle(string repo)
            => new()
            {
                TargetType = "Node",
                GroupLabel = "Repository",
                Condition = [
                    new()
                    {
                        Expression = $"Repo='{repo}'"
                    }
                ],
                ValueLabel = repo
            };
    }
}
