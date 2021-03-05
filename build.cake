var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var artifactsDirectory = MakeAbsolute(Directory("./artifacts"));

Task("Default")
    .IsDependentOn("ReportGenerator")
    .Does(() => 
    { 
        if (IsRunningOnWindows())
        {
            var reportFilePath = ".\\GeneratedReports\\ReportGeneratorOutput\\index.htm";
          
            StartProcess("explorer", reportFilePath);
        }
    });



#tool "nuget:?package=ReportGenerator"
 
Task("ReportGenerator")
    .IsDependentOn("OpenCover")
    .Does(() => 
    {
        var reportGeneratorSettings = new ReportGeneratorSettings()
        {
            HistoryDirectory = new DirectoryPath("./GeneratedReports/ReportsHistory")
        };

        var outputFile = new FilePath("./GeneratedReports/CP.Api.UsuarioReport.xml");

        ReportGenerator(outputFile, "./GeneratedReports/ReportGeneratorOutput", reportGeneratorSettings );

});
	
    


    

#tool "nuget:?package=OpenCover"
#tool "nuget:?package=NUnit.ConsoleRunner"
 
Task("OpenCover")
    .IsDependentOn("BuildTest")
    .Does(() => 
    {
        var openCoverSettings = new OpenCoverSettings()
        {
            Register = "user",
            SkipAutoProps = true,
            ArgumentCustomization = args => args.Append("-coverbytest:*.CP.API.Usuario.TesteIntegrado.dll").Append("-mergebyhash")
        };
 
        var outputFile = new FilePath("./GeneratedReports/CP.Api.UsuarioReport.xml");
 
        OpenCover(tool => {
                var testAssemblies = GetFiles("./CP.API.Usuario.TesteIntegrado/bin/Debug/netcoreapp3.1/CP.API.Usuario.TesteIntegrado.dll");
                tool.NUnit3(testAssemblies);
            },
            outputFile,
            openCoverSettings
                .WithFilter("+[CP.API.Usuario*]*")
                .WithFilter("-[CP.API.Usuario.TesteIntegrado]*")
        );
    });


Task("BuildTest")
    .Does(() => 
    {
        MSBuild("./CP.API.Usuario.TesteIntegrado/CP.API.Usuario.TesteIntegrado.csproj", 
            new MSBuildSettings {
                Verbosity = Verbosity.Minimal,
                Configuration = "Debug"
            }
        );
    });

RunTarget(target);