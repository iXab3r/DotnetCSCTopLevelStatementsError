using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Emit;
using System.Reflection;
using System.IO;
using Microsoft.CodeAnalysis.Host.Mef;
using Shouldly;

namespace CSCCompileTimeError.Tests;

[TestFixture]
public class Tests
{
    [Test]
    public async Task ShouldNotThrowOnCompilation() // throws
    {
        // Given
        var projectId = ProjectId.CreateNewId();
        var projectInfo = ProjectInfo.Create(projectId, VersionStamp.Default, "NewProject", "NewProject", LanguageNames.CSharp);
        
        var workspace = new AdhocWorkspace();
        workspace.AddProject(projectInfo);

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
        var action = () => compilation?.GetDiagnostics();
        
        // Then
        action.ShouldNotThrow();
    }
    
    [Test]
    public async Task ShouldNotThrowOnCompilationFromSingleFile() // does not throw
    {
        // Given
        var projectId = ProjectId.CreateNewId();
        var projectInfo = ProjectInfo.Create(projectId, VersionStamp.Default, "NewProject", "NewProject", LanguageNames.CSharp);
        
        var workspace = new AdhocWorkspace();
        workspace.AddProject(projectInfo);

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
        var action = () => compilation?.GetDiagnostics();
        
        // Then
        action.ShouldNotThrow();
    }
}