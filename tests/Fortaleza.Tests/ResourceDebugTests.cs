using System.Reflection;
using Xunit.Abstractions;

namespace Fortaleza.Tests;

public sealed class ResourceDebugTests(ITestOutputHelper output)
{
    [Fact]
    public void ListEmbeddedResources()
    {
        Assembly assembly = Assembly.GetAssembly(typeof(Localization.Localizer))!;
        string[] names = assembly.GetManifestResourceNames();
        foreach (string name in names)
        {
            output.WriteLine(name);
        }

        Assert.NotEmpty(names);
    }
}
