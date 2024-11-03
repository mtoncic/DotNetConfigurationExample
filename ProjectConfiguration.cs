namespace DotNetConfigurationExample;

internal class ProjectConfiguration
{
    public string Name { get; set; }

    public string Author { get; set; }

    public ProjectConfiguration(
        string name,
        string author)
    {
        Name = name;
        Author = author;
    }

    public ProjectConfiguration()
    {
    }
}