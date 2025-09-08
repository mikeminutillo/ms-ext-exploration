using OpenSoftware.DgmlTools;
using OpenSoftware.DgmlTools.Analyses;
using OpenSoftware.DgmlTools.Builders;
using OpenSoftware.DgmlTools.Model;

class InvestigationAnalysis(string[] packages) : IGraphAnalysis
{
    public void Execute(DirectedGraph graph)
    {
        var seen = new HashSet<string>(packages.Select(Utils.ToNodeId), StringComparer.OrdinalIgnoreCase);
        var lookup = graph.Links.Where(x => x.IsContainment == false)
            .ToLookup(x => x.Source, x => x.Target);

        var containerLookup = graph.Links.Where(x => x.IsContainment)
            .ToLookup(x => x.Source, x => x.Target);

        var next = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var horizon = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var node in graph.Nodes)
        {
            if (containerLookup[node.Id].Any())
            {
                continue;
            }

            if (seen.Contains(node.Id))
            {
                node.Properties.Add("InvestigationStatus", "Seen");
            }
            else if (lookup[node.Id].Except(seen).Any() is false)
            {
                node.Properties.Add("InvestigationStatus", "Next");
                next.Add(node.Id);
            }
        }

        foreach (var node in graph.Nodes)
        {
            if (containerLookup[node.Id].Any() || node.Properties.ContainsKey("InvestigationStatus"))
            {
                continue;
            }
            if (lookup[node.Id].Except(seen).Except(next).Any() is false)
            {
                node.Properties.Add("InvestigationStatus", "Horizon");
                horizon.Add(node.Id);
            }
        }

        foreach (var node in graph.Nodes)
        {
            var contained = containerLookup[node.Id].ToArray();
            if (!contained.Any())
            {
                continue;
            }

            if (contained.Except(seen).Any() is false)
            {
                node.Properties.Add("InvestigationStatus", "Seen");
            }
            else if (contained.Intersect(next).Any())
            {
                node.Properties.Add("InvestigationStatus", "Next");
            }
            else if (contained.Intersect(horizon).Any())
            {
                node.Properties.Add("InvestigationStatus", "Horizon");
            }
        }
    }

    public IEnumerable<Property> GetProperties(DirectedGraph graph)
    {
        yield return new Property
        {
            Id = "InvestigationStatus",
            DataType = "System.String",
            Label = "Investigation status",
            Description = "Has the node been seen? If not, have we seen all it's dependencies?"
        };
    }

    public IEnumerable<Style> GetStyles(DirectedGraph graph)
    {
        yield return CreateStyle("Seen", "Green");
        yield return CreateStyle("Next", "Yellow");
        yield return CreateStyle("Horizon", "Orange");

        static Style CreateStyle(string status, string color)
        => new()
        {
            TargetType = "Node",
            GroupLabel = status,
            Condition = [
                new()
                {
                    Expression = $"InvestigationStatus='{status}'"
                }
            ],
            Setter = [
                new()
                {
                    Property = "Background",
                    Value = color
                }
            ]
        };
    }
}