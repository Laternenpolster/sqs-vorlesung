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
    private const string DomainAssemblyName = "PokemonLookup.Domain";
    private const string ApplicationAssemblyName = "PokemonLookup.Application";
    private const string InfrastructureAssemblyName = "PokemonLookup.Infrastructure";
    private const string WebAssemblyName = "PokemonLookup.Web";

    private static readonly Assembly DomainAssembly = Assembly.Load(DomainAssemblyName);
    private static readonly Assembly ApplicationAssembly = Assembly.Load(ApplicationAssemblyName);
    private static readonly Assembly InfrastructureAssembly = Assembly.Load(InfrastructureAssemblyName);
    private static readonly Assembly WebAssembly = Assembly.Load(WebAssemblyName);

    // Architecture is loaded once for improved performance
    private static readonly Architecture Architecture = new ArchLoader()
        .LoadAssemblies(DomainAssembly, ApplicationAssembly, InfrastructureAssembly, WebAssembly)
        .Build();

    // The 4 layers of the project
    private readonly IObjectProvider<IType> _domainLayer = Types()
        .That()
        .ResideInAssembly(DomainAssembly)
        .As("Domain Layer");

    private readonly IObjectProvider<IType> _applicationLayer = Types()
        .That()
        .ResideInAssembly(ApplicationAssembly)
        .As("Application Layer");

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
        // Domain does not depend on Infrastructure
        var domainToApplicationRule = Types()
            .That()
            .Are(_domainLayer)
            .Should()
            .NotDependOnAny(_applicationLayer);

        // Domain does not depend on Infrastructure
        var domainToInfrastructureRule = Types()
            .That()
            .Are(_domainLayer)
            .Should()
            .NotDependOnAny(_infrastructureLayer);

        // Domain does not depend on Web
        var domainToWebRule = Types()
            .That()
            .Are(_domainLayer)
            .Should()
            .NotDependOnAny(_webLayer);

        // Domain does only depend on Domain
        var domainToDomain = Types()
            .That()
            .Are(_domainLayer)
            .Should()
            .OnlyDependOn(_domainLayer);

        // Application does not depend on Infrastructure
        var applicationToInfrastructureRule = Types()
            .That()
            .Are(_applicationLayer)
            .Should()
            .NotDependOnAny(_infrastructureLayer);

        // Application does not depend on Web
        var applicationToWebRule = Types()
            .That()
            .Are(_applicationLayer)
            .Should()
            .NotDependOnAny(_webLayer);

        // Infrastructure does not depend on Web
        var infrastructureToWebRule = Types()
            .That()
            .Are(_infrastructureLayer)
            .Should()
            .NotDependOnAny(_webLayer);

        // Check all rules
        domainToApplicationRule.Check(Architecture);
        domainToInfrastructureRule.Check(Architecture);
        domainToWebRule.Check(Architecture);
        domainToDomain.Check(Architecture);

        applicationToInfrastructureRule.Check(Architecture);
        applicationToWebRule.Check(Architecture);
        infrastructureToWebRule.Check(Architecture);
    }

    /// <summary>
    /// Ensures that the types of each project have the right namespace.
    /// E.g.: Domain types have to be in the namespace "PokemonLookup.Domain.*"
    /// </summary>
    [Fact]
    public void CheckNamespaces()
    {
        // Domain classes should be in Domain namespace
        var domainRule = Types()
            .That()
            .Are(_domainLayer)
            .Should()
            .ResideInNamespace($"{DomainAssemblyName}.*", true);

        // Application classes should be in Application namespace
        var applicationRule = Types()
            .That()
            .Are(_applicationLayer)
            .Should()
            .ResideInNamespace($"{ApplicationAssemblyName}.*", true);

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
        domainRule.Check(Architecture);
        applicationRule.Check(Architecture);
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
        // All interfaces have to be defined in Application
        var interfaceRule = Interfaces()
            .Should()
            .Be(_applicationLayer);

        // Exceptions may be defined in Application.Exceptions
        var exceptionsRule = Classes()
            .That()
            .ResideInNamespace(ApplicationAssemblyName + ".Exceptions")
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
