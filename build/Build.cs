using System;
using System.IO;
using System.Linq;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using Serilog;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;

[GitHubActions("build-and-test", GitHubActionsImage.WindowsLatest, OnPushBranches = new []{"master"}, ImportSecrets = new[] { nameof(ExampleSecret) })]
class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main () => Execute<Build>(x => x.RunTests);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;
    
    [Parameter] [Secret] readonly string ExampleSecret;

    [Solution] readonly Solution Solution;
    [PathVariable] readonly Tool Git;
    IProcess ApiProcess;

    Target Clean => _ => _
        .Executes(() =>
        {
            Log.Information(ExampleSecret);
            // Log.Information(RootDirectory);
            // DotNetClean();
            // Git("status");
            // DotNetPack()
            // DotNetPublish()
        });

    Target Restore => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            DotNetRestore(s => s.SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .EnableNoRestore());
        });

    Target UnitTests => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            var testProject = Solution.AllProjects.First(x => x.Name == "MN_StaticCodeAnalysis.Tests");
            DotNetTest(s => s
                .SetProjectFile(testProject)
                .SetConfiguration(Configuration)
                .EnableNoRestore()
                .EnableNoBuild());
        });
    
    Target StartApi => _ => _
        .DependsOn(UnitTests)
        .Executes(() =>
        {
            var apiProject = Solution.AllProjects.First(x => x.Name == "MN_StaticCodeAnalysis.Api");
            ApiProcess = ProcessTasks.StartProcess("dotnet", "run", apiProject.Directory);
        });
    
    Target FunctionalTests => _ => _
        .DependsOn(StartApi)
        .Triggers(StopApi)
        .Executes(() =>
        {
            var functionalTestsProject = Solution.AllProjects.First(x => x.Name == "MN_StaticCodeAnalysis.FunctionalTests");
            DotNetTest(s => s
                .SetProjectFile(functionalTestsProject)
                .SetConfiguration(Configuration)
                .EnableNoRestore()
                .EnableNoBuild());
        });
    
    Target StopApi => _ => _
        .Executes(() =>
        {
            ApiProcess.Kill();
        });
    
    Target RunTests => _ => _.DependsOn(FunctionalTests);
}
