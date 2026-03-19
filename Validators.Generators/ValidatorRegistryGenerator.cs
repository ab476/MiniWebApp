using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace Validators.Generators;

[Generator]
public class ValidatorRegistryGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // 1. INJECT MARKER ATTRIBUTES
        // Give the user the attributes they need to mark their classes
        context.RegisterPostInitializationOutput(InjectMarkerAttributes);
        

        // 2. PIPELINE A: Find the user's partial class marked with [ValidatorRegistry]
        var targetClassProvider = context.SyntaxProvider.ForAttributeWithMetadataName(
            "Validators.Validation.ValidatorRegistryAttribute",
            predicate: static (node, _) => node is ClassDeclarationSyntax classDecl && classDecl.Modifiers.Any(m => m.ValueText == "partial"),
            transform: static (ctx, _) =>
            {
                var symbol = (INamedTypeSymbol)ctx.TargetSymbol;
                var ignoredTypes = new List<string>();

                // NEW: Find [ValidatorRegistryIgnore(typeof(T))] on the Registry class
                foreach (var attr in symbol.GetAttributes())
                {
                    if (attr.AttributeClass?.Name == "ValidatorRegistryIgnoreAttribute" &&
                        attr.ConstructorArguments.Length == 1 &&
                        attr.ConstructorArguments[0].Value is ITypeSymbol ignoredType)
                    {
                        ignoredTypes.Add(ignoredType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
                    }
                }

                return new RegistryTargetInfo(
                    Namespace: symbol.ContainingNamespace.ToDisplayString(),
                    ClassName: symbol.Name,
                    // Grab accessibility (e.g., "public", "internal")
                    Modifier: symbol.DeclaredAccessibility.ToString().ToLower(),
                    IgnoredTypes: ignoredTypes.ToImmutableArray()

                );
            });

        // 3. PIPELINE B: Find the STJ Context marked with [ValidatorRegistrySource] and extract [JsonSerializable] types
        var sourceTypesProvider = context.SyntaxProvider.ForAttributeWithMetadataName(
            "Validators.Validation.ValidatorRegistrySourceAttribute",
            predicate: static (node, _) => node is ClassDeclarationSyntax,
            transform: static (ctx, _) =>
            {
                var symbol = (INamedTypeSymbol)ctx.TargetSymbol;
                var extractedTypes = new List<string>();

                // Look through all attributes on this STJ context class
                foreach (var attr in symbol.GetAttributes())
                {
                    // Check if it's the [JsonSerializable] attribute
                    if (attr.AttributeClass?.Name == "JsonSerializableAttribute" &&
                        attr.ConstructorArguments.Length > 0 &&
                        attr.ConstructorArguments[0].Value is ITypeSymbol typeSymbol &&
                        !IsExcludedType(typeSymbol))
                    {
                        // Extract the exact type passed into typeof(...)
                        extractedTypes.Add(typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
                    }
                }
                return extractedTypes;
            })
            .SelectMany(static (types, _) => types) // Flatten the lists
            .Collect(); // Gather all types into an ImmutableArray

        // 4. COMBINE & GENERATE: Pair the target class with the list of model types
        var generationInput = targetClassProvider.Combine(sourceTypesProvider);

        context.RegisterSourceOutput(generationInput, static (spc, source) =>
        {
            var targetInfo = source.Left;
            var modelTypes = source.Right;
            GenerateRegistryClass(spc, targetInfo, modelTypes);
        });
    }
    /// <summary>
    /// Injects the required marker attributes into the consuming project during the early initialization phase.
    /// <br/><br/>
    /// <strong>Why do we do this?</strong><br/>
    /// By generating these attributes automatically, the consumer of this Source Generator does not need 
    /// to install a separate "Attributes" NuGet package or write the classes themselves. The attributes 
    /// simply "exist" in their project as soon as they reference the generator.
    /// <br/><br/>
    /// <strong>Injected Attributes:</strong>
    /// <list type="number">
    /// <item>
    /// <description><strong>[ValidatorRegistry]</strong>: Placed on a 'partial' class by the user. This dictates WHERE the <c>GetValidator(object arg)</c> switch expression will be generated.</description>
    /// </item>
    /// <item>
    /// <description><strong>[ValidatorRegistrySource]</strong>: Placed on a <c>JsonSerializerContext</c> by the user. This dictates WHERE the generator should look for <c>[JsonSerializable(typeof(T))]</c> attributes to build the registry.</description>
    /// </item>
    /// </list>
    /// </summary>
    /// <param name="ctx">The initialization context used to register early source text.</param>
    private static void InjectMarkerAttributes(IncrementalGeneratorPostInitializationContext ctx)
    {
        var builder = new CSharpCodeBuilder()
            .AppendLine("// <auto-generated/>")
            .AppendLine("using System;")
            .AppendLine()
            .BeginBlock("namespace Validators.Validation")

                // Generate the ValidatorRegistryAttribute
                .AppendLine("/// <summary>")
                .AppendLine("/// Marks this partial class as the target for the generated Validator Registry.")
                .AppendLine("/// </summary>")
                .AppendLine("[AttributeUsage(AttributeTargets.Class)]")
                .AppendLine("public class ValidatorRegistryAttribute : Attribute { }")
                .AppendLine()

                // Generate the ValidatorRegistrySourceAttribute
                .AppendLine("/// <summary>")
                .AppendLine("/// Marks this JsonSerializerContext as the source of types to include in the Validator Registry.")
                .AppendLine("/// </summary>")
                .AppendLine("[AttributeUsage(AttributeTargets.Class)]")
                .AppendLine("public class ValidatorRegistrySourceAttribute : Attribute { }")

                // NEW: Generate the ValidatorRegistryIgnoreAttribute with a Type constructor
                .AppendLine("/// <summary>")
                .AppendLine("/// Marks a model to be explicitly ignored and excluded from the generated Validator Registry.")
                .AppendLine("/// Can be applied directly to a model, or to the registry class by passing the type.")
                .AppendLine("/// </summary>")
                .AppendLine("[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]")
                .AppendLine("public class ValidatorRegistryIgnoreAttribute : Attribute")
                .BeginBlock()
                    .AppendLine("public ValidatorRegistryIgnoreAttribute() { }")
                    .AppendLine("public ValidatorRegistryIgnoreAttribute(Type typeToIgnore) { }")
                .EndBlock()

            .EndBlock(); // Closes the namespace block

        // Add the generated attributes to the compilation pipeline
        ctx.AddSource("ValidatorRegistryAttributes.g.cs", SourceText.From(builder.Build(), Encoding.UTF8));
    }
    private static bool IsExcludedType(ITypeSymbol typeSymbol)
    {
        // Look through all attributes applied directly to the model type
        foreach (var attr in typeSymbol.GetAttributes())
        {
            // Check if it's our specific ignore attribute
            if (attr.AttributeClass?.ToDisplayString() == "Validators.Validation.ValidatorRegistryIgnoreAttribute")
            {
                return true;
            }
        }

        return false;
    }
    private static void GenerateRegistryClass(SourceProductionContext spc, RegistryTargetInfo target, ImmutableArray<string> modelTypes)
    {
        // NEW: Filter out any types that match the IgnoredTypes list from the registry class
        var uniqueTypes = modelTypes
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Where(t => !target.IgnoredTypes.Contains(t))
            .Distinct()
            .ToList();

        if (uniqueTypes.Count == 0) return;

        var builder = new CSharpCodeBuilder()
            .AppendLine("// <auto-generated/>")
            .AppendLine("using System;")
            .AppendLine("using Microsoft.Extensions.DependencyInjection;")
            .AppendLine("using FluentValidation;")
            .AppendLine()
            .BeginBlock($"namespace {target.Namespace}")
                // Generates e.g., "public partial class MyValidatorRegistry"
                .BeginBlock($"{target.Modifier} partial class {target.ClassName}")
                    .BeginBlock("public static IValidator? GetValidator(object arg, IServiceProvider sp)")

                        .BeginBlock("return arg switch")
                            .AppendLines(uniqueTypes, typeName => $"{typeName} => sp.GetService<IValidator<{typeName}>>(),")
                            .AppendLine("_ => null")
                        .EndBlock(";") // Closes the switch

                    .EndBlock()
                .EndBlock()
            .EndBlock();

        spc.AddSource($"{target.ClassName}.g.cs", SourceText.From(builder.Build(), Encoding.UTF8));
    }
}

// Struct to hold info about the user's partial class.
// A record struct automatically implements IEquatable, which is crucial for Incremental Generator caching.
public record struct RegistryTargetInfo(string Namespace, string ClassName, string Modifier, ImmutableArray<string> IgnoredTypes);