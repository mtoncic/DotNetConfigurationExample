using Microsoft.Extensions.Options;

namespace DotNetConfigurationExample;

internal class Foo
{
    private readonly ProjectConfiguration projectConfiguration;

    public Foo(IOptions<ProjectConfiguration> projectConfigurationOptions)
    {
        this.projectConfiguration = projectConfigurationOptions.Value;
    }

    public string Describe() => $"Name: {projectConfiguration.Name}, Author: {projectConfiguration.Author}";
}
