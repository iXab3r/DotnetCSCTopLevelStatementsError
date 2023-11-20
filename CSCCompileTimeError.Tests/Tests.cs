using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis;
using NUnit.Framework;
using Shouldly;

namespace CSCCompileTimeError.Tests;

[TestFixture]
public class Tests
{
    [Test]
    public async Task ShouldNotThrowOnCompilation() // throws Unable to cast object of type 'Microsoft.CodeAnalysis.CSharp.Syntax.CompilationUnitSyntax' to type 'Microsoft.CodeAnalysis.CSharp.Syntax.BaseTypeDeclarationSyntax'
    {
        // Given
        var (workspace, projectId) = PrepareWorkspace();

        workspace.AddDocument(projectId, "Program.cs", SourceText.From("PrintLine();"));
        workspace.AddDocument(projectId, "ProgramBase.cs", SourceText.From("""
                                                                           public class ProgramBase
                                                                           {
                                                                               public static void PrintLine()
                                                                               {
                                                                               }
                                                                           }
                                                                           
                                                                           partial class Program : ProgramBase
                                                                           {
                                                                           }
                                                                           """));

        var project = workspace.CurrentSolution.GetProject(projectId)!;
        var compilation = await project.GetCompilationAsync();
        
        // When
        var diagnostics = compilation!.GetDiagnostics();
        
        // Then
        diagnostics.ShouldBeEmpty();
    }
    
    [Test]
    public async Task ShouldNotThrowOnCompilationFromSingleFile() // does not throw
    {
        // Given
        var (workspace, projectId) = PrepareWorkspace();

        workspace.AddDocument(projectId, "Program.cs", SourceText.From("""
                                                                           PrintLine();
                                                                           
                                                                           public class ProgramBase
                                                                           {
                                                                               public static void PrintLine()
                                                                               {
                                                                               }
                                                                           }

                                                                           partial class Program : ProgramBase
                                                                           {
                                                                           }
                                                                           """));

        var project = workspace.CurrentSolution.GetProject(projectId)!;
        var compilation = await project.GetCompilationAsync();
        
        // When
        var diagnostics = compilation!.GetDiagnostics();
        
        // Then
        diagnostics.ShouldBeEmpty();
    }
    
    [Test]
    public async Task ShouldNotThrowOnCompilationWithoutInheritance() // does not throw
    {
        // Given
        var (workspace, projectId) = PrepareWorkspace();

        workspace.AddDocument(projectId, "Program.cs", SourceText.From("PrintLine();"));
        workspace.AddDocument(projectId, "ProgramBase.cs", SourceText.From("""
                                                                           partial class Program
                                                                           {
                                                                               public static void PrintLine()
                                                                               {
                                                                               }
                                                                           }
                                                                           """));

        var project = workspace.CurrentSolution.GetProject(projectId)!;
        var compilation = await project.GetCompilationAsync();
        
        // When
        var diagnostics = compilation!.GetDiagnostics();
        
        // Then
        diagnostics.ShouldBeEmpty();
    }

    private static (AdhocWorkspace workspace, ProjectId projectId)  PrepareWorkspace()
    {
        var projectId = ProjectId.CreateNewId();
        var projectInfo = ProjectInfo.Create(
            projectId, 
            VersionStamp.Default, 
            "NewProject", 
            "NewProject", 
            LanguageNames.CSharp,
            metadataReferences: new MetadataReference[]{ MetadataReference.CreateFromFile(typeof(object).Assembly.Location) });
        
        var workspace = new AdhocWorkspace();
        workspace.AddProject(projectInfo);
        return (workspace, projectId);
    }
}