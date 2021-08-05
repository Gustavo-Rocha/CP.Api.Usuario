var target = Argument("target", "Default");

#addin nuget:?package=Cake.Coverlet
#tool nuget:?package=ReportGenerator

/*  Specify the relative paths to your tests projects here. */
var testProjectsRelativePaths = new string[]
{
    "../CP.API.Usuario.TesteIntegrado/CP.API.Usuario.TesteIntegrado.csproj",
    "../CP.APi.Usuario.TesteUnitario/CP.APi.Usuario.TesteUnitario.csproj"
};


/*  Change the output artifacts and their configuration here. */
//pegar diretorio acima
var parentDirectory =  Directory("..");
//caminho da pasta
var pastaCodeCoverage = "code_coverage";

var coverageDirectory = parentDirectory + Directory(pastaCodeCoverage);//nome do arquivo de resultado
var cuberturaFileName = "results";
//extensão do arquivo com o resultado
var cuberturaFileExtension = ".cobertura.xml";
// nome do html com o resultado para exibição do report
var reportTypes = "CP.API.REPORTS"; // Use "Html" value locally for performance and files' size.
//caminho completo do arquivo de cobertura 
var coverageFilePath = coverageDirectory + File(cuberturaFileName + cuberturaFileExtension);
//caminho completo do arquivo de cobertura .json
var jsonFilePath = coverageDirectory + File(cuberturaFileName + ".json");

Task("Clean")
    .Does(() =>
{
    if (!DirectoryExists(coverageDirectory))
        CreateDirectory(coverageDirectory);
    else
        CleanDirectory(coverageDirectory);
});

Task("Test")
    .IsDependentOn("Clean")
    .Does(() =>
{
    var testSettings = new DotNetCoreTestSettings();

    var coverletSettings = new CoverletSettings
    {
        CollectCoverage = true,
        CoverletOutputDirectory = coverageDirectory,
        CoverletOutputName = cuberturaFileName
    };

    if (testProjectsRelativePaths.Length == 1)
    {
        coverletSettings.CoverletOutputFormat  = CoverletOutputFormat.cobertura;
        DotNetCoreTest(testProjectsRelativePaths[0], testSettings, coverletSettings);
    }
    else
    {
        DotNetCoreTest(testProjectsRelativePaths[0], testSettings, coverletSettings);

        coverletSettings.MergeWithFile = jsonFilePath;
        for (int i = 1; i < testProjectsRelativePaths.Length; i++)
        {
            if (i == testProjectsRelativePaths.Length - 1)
            {
                coverletSettings.CoverletOutputFormat  = CoverletOutputFormat.cobertura;
            }

            DotNetCoreTest(testProjectsRelativePaths[i], testSettings, coverletSettings);
        }
    }
});

Task("Report")
    .IsDependentOn("Test")
    .Does(() =>
{
    var reportSettings = new ReportGeneratorSettings();
    ReportGenerator(coverageFilePath, coverageDirectory, reportSettings);
});

Task("Default")
    .IsDependentOn("Report")
    .Does(() => 
    { 
        if (IsRunningOnWindows())
        {
            StartProcess("explorer", "..\\" + pastaCodeCoverage + "\\index.htm");
        }
    });


RunTarget(target);