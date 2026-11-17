using System.Reflection;

namespace Fortaleza.Tests;

/// <summary>
/// Resource debug tests.
/// </summary>
public sealed class ResourceDebugTests(ITestOutputHelper output)
{
    /// <summary>
    /// List embedded resources.
    /// </summary>
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
