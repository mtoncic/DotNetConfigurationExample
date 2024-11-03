using System;
using System.Linq;
using DotNetConfigurationExample;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

DotNetConfigurationStepByStep(args);
// ConfigurationWithApplicationBuilder(args);

static void DotNetConfigurationStepByStep(string[] args)
{
    var configurationBuilder = new ConfigurationBuilder();

    var configurationRoot = configurationBuilder.Build();

    // will show us no configuration providers registered
    Console.WriteLine("Configuration providers:");
    foreach (var configProvider in configurationRoot.Providers)
    {
        Console.WriteLine(configProvider);
    }
    Console.WriteLine("-----End----");

    // set env variable programmatically, otherwise it's already set through launch profile (launchSettings.json)
    //Environment.SetEnvironmentVariable(
    //    "MyOwnEnvVariable__Secret",
    //    "Environment Variable Secret",
    //    target: EnvironmentVariableTarget.Process); // lifetime of a variable connected to a lifetime of a process

    configurationBuilder
        .AddJsonFile("settings.json", optional: true, reloadOnChange: true)
        .AddEnvironmentVariables(prefix: "MyOwnEnvVariable__");

    if (args is { Length: > 0 })
    {
        configurationBuilder.AddCommandLine(args);
    }

    configurationRoot = configurationBuilder.Build();

    // Prints recently registered configuration providers
    // JsonConfigurationProvider for 'settings.json' (Optional)
    // EnvironmentVariablesConfigurationProvider Prefix: 'MyOwnEnvVariable__'
    // CommandLineConfigurationProvider
    Console.WriteLine("Configuration providers:");
    foreach (var configProvider in configurationRoot.Providers)
    {
        Console.WriteLine(configProvider);
    }
    Console.WriteLine("-----End----");

    // Prints configuration pairs (key:value)
    // Secret: Environment Variable Secret
    // Project:
    // Project: Name: ConsoleProjectName
    // Project:Author: ConsoleAuthor
    // ConnectionStrings:
    // ConnectionStrings: my_connection_string: my super secret db connection string
    Console.WriteLine("Configuration entries:");
    foreach (var config in configurationRoot.AsEnumerable())
    {
        Console.WriteLine($"{config.Key}:{config.Value}");
    }
    Console.WriteLine("-----End----");

    Console.WriteLine("Configuration values (Project Name&Author (Console override) and Secret (Env variable override)):");
    Console.WriteLine(configurationRoot["Project:Name"]); // ConsoleProjectName
    Console.WriteLine(configurationRoot["Project:Author"]); // ConsoleAuthor
    Console.WriteLine(configurationRoot["Secret"]); // Environment Variable Secret
    Console.WriteLine("-----End----");

    Console.WriteLine("Configuration values (Project Name, Author bind to a ProjectConfiguration object):");
    ProjectConfiguration? projectConfiguration =
        configurationRoot
        .GetSection("Project")
        .Get<ProjectConfiguration>();

    Console.WriteLine(projectConfiguration?.Name); // ConsoleProjectName
    Console.WriteLine(projectConfiguration?.Author); // ConsoleAuthor
    Console.WriteLine("-----End----");

    Console.WriteLine("Useful extension methods:");
    Console.WriteLine("Debug view:");
    Console.WriteLine(configurationRoot.GetDebugView());
    Console.WriteLine("----End-----");
    Console.WriteLine("Connection string config value:");
    Console.WriteLine(configurationRoot.GetConnectionString("my_connection_string")); // my super secret db connection string
    Console.WriteLine("-----End----");
    Console.WriteLine("Trying to get non-existing section using GetRequiredSection extension method");
    // Console.WriteLine(configurationRoot.GetRequiredSection("non_existing_key")); //throws System.InvalidOperationException
}

static void ConfigurationWithApplicationBuilder(string[] args)
{
    var hostApplicationBuilder = Host.CreateApplicationBuilder(args);

    // Console.WriteLine(hostApplicationBuilder.Configuration.GetDebugView());

    Console.WriteLine("Configuration sources predefined on HostApplicationBuilder:");
    foreach (var configSource in hostApplicationBuilder.Configuration.Sources)
    {
        Console.WriteLine(configSource);
    }
    Console.WriteLine("----End----");

    hostApplicationBuilder.Configuration.Sources.Clear();

    hostApplicationBuilder.Configuration
        .AddJsonFile("settings.json", optional: true, reloadOnChange: false);

    hostApplicationBuilder.Services.AddScoped<Foo>();
    hostApplicationBuilder.Services.AddScoped<ProjectConfiguration>();

    hostApplicationBuilder.Services
        .Configure<ProjectConfiguration>(
            hostApplicationBuilder.Configuration.GetSection("Project"));

    var app = hostApplicationBuilder.Build();

    var foo = app.Services.GetRequiredService<Foo>();

    Console.WriteLine("Configuration through dependency injection via IOptions pattern:");
    Console.WriteLine(foo.Describe());
    Console.WriteLine("----End----");
}