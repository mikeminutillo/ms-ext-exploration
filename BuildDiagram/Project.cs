class Project
{
    public Project(string file, string repo)
    {
        File = file;
        Name = Path.GetFileNameWithoutExtension(file);
        Subsystem = string.Join(".", Name.Split('.').Take(3));
        ShortName = Name.Replace($"{Subsystem}.", "");
        if (Name == ShortName)
        {
            ShortName = ShortName.Split('.').Last();
        }
        Repo = repo;
        NodeId = Utils.ToNodeId(Name);
        Dependencies = Utils.GetDependencies(File);
    }

    public string Name { get; }
    public string Subsystem { get; }
    public string ShortName { get; }
    public string NodeId { get; }
    public string Repo { get; }
    public string File { get; }
    public string[] Dependencies { get; }
}