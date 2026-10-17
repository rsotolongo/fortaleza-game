using System.Reflection;
using Xunit;
using Xunit.Abstractions;

namespace Fortaleza.Tests;

public class ResourceDebugTests(ITestOutputHelper output)
{
    [Fact]
    public void ListEmbeddedResources()
    {
        var assembly = Assembly.GetAssembly(typeof(Fortaleza.Localization.Localizer))!;
        foreach (var name in assembly.GetManifestResourceNames())
            output.WriteLine(name);
    }
}
