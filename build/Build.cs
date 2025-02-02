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
using Nuke.Common.Tools.ReportGenerator;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using Serilog;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;

// okazało się że jest problem z tą konfiguracją
// i nie da się, (albo na moment tworzenia tego nie jest to jasno opisane),
// w łatwy sposób skonfigurowac rozszerzeń np do wyświetlenia raportu z testów.
// Dlatego pierwsza wersja zostało wygenerowana ta wtyczką, a następnie wyłączyłęm automatyczne generowanie i dopisałem kilka kroków ręcznie
[GitHubActions(
    "build-and-test", 
    GitHubActionsImage.WindowsLatest, 
    OnPushBranches = new []{"master"}, 
    ImportSecrets = new[] { nameof(ExampleSecret) },
    AutoGenerate = false)]
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
    
    static AbsolutePath TestResultFolder => RootDirectory / "TestResults";

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
                // .AddLoggers($"trx;LogFileName={TestResultFolder}/test-results.trx")
                .AddLoggers($"teamcity")
                // .SetProcessArgumentConfigurator(args => args
                //     .Add("/p:CollectCoverage=true")
                //     .Add("/p:CoverletOutputFormat=cobertura"))
                .EnableNoRestore()
                .EnableNoBuild());
        });
    
    // uruchomienie api i przeprowadzenie testów lokalnie
    // Target StartApi => _ => _
    //     .DependsOn(UnitTests)
    //     .Executes(() =>
    //     {
    //         var apiProject = Solution.AllProjects.First(x => x.Name == "MN_StaticCodeAnalysis.Api");
    //         ApiProcess = ProcessTasks.StartProcess("dotnet", "run", apiProject.Directory);
    //     });
    //
    // Target FunctionalTests => _ => _
    //     .DependsOn(StartApi)
    //     .Triggers(StopApi)
    //     .Executes(() =>
    //     {
    //         var functionalTestsProject = Solution.AllProjects.First(x => x.Name == "MN_StaticCodeAnalysis.FunctionalTests");
    //         DotNetTest(s => s
    //             .SetProjectFile(functionalTestsProject)
    //             .SetConfiguration(Configuration)
    //             .EnableNoRestore()
    //             .EnableNoBuild());
    //     });
    
    Target StopApi => _ => _
        .Executes(() =>
        {
            // BUG: test
            ApiProcess.Kill();
            // throw new NotImplementedException();
        });
    
    // Target RunTests => _ => _.DependsOn(FunctionalTests);
    Target RunTests => _ => _.DependsOn(UnitTests);
}
