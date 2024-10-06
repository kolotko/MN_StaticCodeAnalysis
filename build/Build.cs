using System;
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

[GitHubActions("build-and-test", GitHubActionsImage.WindowsLatest, OnPushBranches = new []{"master"})]
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

    [Solution] readonly Solution Solution;
    [PathVariable] readonly Tool Git;
    IProcess ApiProcess;

    Target Clean => _ => _
        .Executes(() =>
        {
            Log.Information(RootDirectory);
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
            DotNetTest(s => s
                .SetProjectFile(RootDirectory / "MN_StaticCodeAnalysis" / "MN_StaticCodeAnalysis.Tests")
                .EnableNoRestore()
                .EnableNoBuild());
        });
    
    Target StartApi => _ => _
        .DependsOn(UnitTests)
        .Executes(() =>
        {
            ApiProcess = ProcessTasks.StartProcess("dotnet", "run", RootDirectory / "MN_StaticCodeAnalysis" / "MN_StaticCodeAnalysis.Api");
        });
    
    Target FunctionalTests => _ => _
        .DependsOn(StartApi)
        .Triggers(StopApi)
        .Executes(() =>
        {
            DotNetTest(s => s
                .SetProjectFile(RootDirectory / "MN_StaticCodeAnalysis" / "MN_StaticCodeAnalysis.FunctionalTests")
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
