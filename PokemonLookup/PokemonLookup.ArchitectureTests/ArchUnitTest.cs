using ArchUnitNET.Domain;
using ArchUnitNET.Loader;
using ArchUnitNET.xUnit;
using Microsoft.AspNetCore.Mvc;
using static ArchUnitNET.Fluent.ArchRuleDefinition;
using Assembly = System.Reflection.Assembly;

namespace PokemonLookup.ArchitectureTests;

/// <summary>
/// Ensures that all requirements for a Clean Architecture are met and types are consistent in their namespaces.
/// </summary>
public class ArchUnitTest
{
    private const string CoreAssemblyName = "PokemonLookup.Core";
    private const string InfrastructureAssemblyName = "PokemonLookup.Infrastructure";
    private const string WebAssemblyName = "PokemonLookup.Web";

    private static readonly Assembly CoreAssembly = Assembly.Load(CoreAssemblyName);
    private static readonly Assembly InfrastructureAssembly = Assembly.Load(InfrastructureAssemblyName);
    private static readonly Assembly WebAssembly = Assembly.Load(WebAssemblyName);

    // Architecture is loaded once for improved performance
    private static readonly Architecture Architecture = new ArchLoader()
        .LoadAssemblies(CoreAssembly, InfrastructureAssembly, WebAssembly)
        .Build();

    // The 3 layers of the project
    private readonly IObjectProvider<IType> _coreLayer = Types()
        .That()
        .ResideInAssembly(CoreAssembly)
        .As("Core Layer");

    private readonly IObjectProvider<IType> _infrastructureLayer = Types()
        .That()
        .ResideInAssembly(InfrastructureAssembly)
        .As("Infrastructure Layer");

    private readonly IObjectProvider<IType> _webLayer = Types()
        .That()
        .ResideInAssembly(WebAssembly)
        .As("Web Layer");

    /// <summary>
    /// Ensures that the dependencies between the projects conform to Clean Architecture
    /// </summary>
    [Fact]
    public void CheckDependencies()
    {
        // Core does not depend on Infrastructure
        var coreToInfrastructureRule = Types()
            .That()
            .Are(_coreLayer)
            .Should()
            .NotDependOnAny(_infrastructureLayer);

        // Core does not depend on Web
        var coreToWebRule = Types()
            .That()
            .Are(_coreLayer)
            .Should()
            .NotDependOnAny(_webLayer);

        // Core does only depend on Core
        var coreToCore = Types()
            .That()
            .Are(_coreLayer)
            .Should()
            .OnlyDependOn(_coreLayer);

        // Infrastructure does not depend on Web
        var infrastructureToWebRule = Types()
            .That()
            .Are(_infrastructureLayer)
            .Should()
            .NotDependOnAny(_webLayer);

        // Check all rules
        coreToInfrastructureRule.Check(Architecture);
        coreToWebRule.Check(Architecture);
        coreToCore.Check(Architecture);
        infrastructureToWebRule.Check(Architecture);
    }

    /// <summary>
    /// Ensures that the types of each project have the right namespace.
    /// E.g.: Core types have to be in the namespace "PokemonLookup.Core.*"
    /// </summary>
    [Fact]
    public void CheckNamespaces()
    {
        // Core classes should be in Core namespace
        var coreRule = Types()
            .That()
            .Are(_coreLayer)
            .Should()
            .ResideInNamespace($"{CoreAssemblyName}.*", true);

        // Infrastructure classes should be in Infrastructure namespace
        var infrastructureRule = Types()
            .That()
            .Are(_infrastructureLayer)
            .Should()
            .ResideInNamespace($"{InfrastructureAssemblyName}.*", true);

        // Web classes should be in Web namespace
        var webRule = Types()
            .That()
            .Are(_webLayer)
            .Should()
            .ResideInNamespace($"{WebAssemblyName}.*", true)
            .OrShould()
            .ResideInNamespace("AspNetCoreGeneratedDocument") // ASP.NET Views
            .OrShould()
            .ResideInNamespace(""); // top-level implementation of Program.cs

        // Check all rules
        coreRule.Check(Architecture);
        infrastructureRule.Check(Architecture);
        webRule.Check(Architecture);
    }

    /// <summary>
    /// Checks the names of all types.
    /// </summary>
    [Fact]
    public void CheckNamingConventions()
    {
        // Interfaces should start with "I", as usual
        var interfacesRule = Interfaces()
            .Should()
            .HaveNameStartingWith("I");

        // ASP.NET Controllers have the suffix "Controller"
        var controllerRule = Classes()
            .That()
            .ResideInNamespace(WebAssemblyName + ".Controllers")
            .Should()
            .HaveNameEndingWith("Controller");

        // Check all rules
        interfacesRule.Check(Architecture);
        controllerRule.Check(Architecture);
    }

    /// <summary>
    /// Checks if the functionality of types matches their namespace.
    /// </summary>
    [Fact]
    public void CheckClassTypes()
    {
        // All interfaces have to be defined in Core
        var interfaceRule = Interfaces()
            .Should()
            .Be(_coreLayer);

        // Exceptions may be defined in Core.Exceptions
        var exceptionsRule = Classes()
            .That()
            .ResideInNamespace(CoreAssemblyName + ".Exceptions")
            .Should()
            .BeAssignableTo(typeof(Exception))
            .AndShould()
            .HaveNameEndingWith("Exception");

        // ASP.NET Controllers have to inherit from ControllerBase (for API Endpoints) or Controller (for Razor Pages)
        var controllerRule = Classes()
            .That()
            .ResideInNamespace(WebAssemblyName + ".Controllers")
            .Should()
            .BeAssignableTo(typeof(ControllerBase))
            .OrShould()
            .BeAssignableTo(typeof(Controller));

        // Check all rules
        interfaceRule.Check(Architecture);
        exceptionsRule.Check(Architecture);
        controllerRule.Check(Architecture);
    }
}
