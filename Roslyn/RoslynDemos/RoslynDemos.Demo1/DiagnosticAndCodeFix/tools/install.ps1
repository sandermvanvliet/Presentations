param($installPath, $toolsPath, $package, $project)

$analyzerPath = join-path $toolsPath "analyzers"
$analyzerFilePath = join-path $analyzerPath "RoslynDemos.Demo1.dll"

$project.Object.AnalyzerReferences.Add("$analyzerFilePath")