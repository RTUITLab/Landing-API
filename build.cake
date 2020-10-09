var target = Argument("target", "PublishApi");
var configuration = Argument("configuration", "Release");

var apiPublishDir = "deploy/api-build";
var apiProject = "Landing.API/Landing.API.csproj";

Setup(ctx =>
{
   CleanDirectory(apiPublishDir);
});


Task("RestoreSolution")
   .Does(() =>
{
   DotNetCoreRestore();
});

Task("BuildApi")
   .IsDependentOn("RestoreSolution")
   .Does(() =>
{
   var settings = new DotNetCoreBuildSettings {
      Configuration = configuration
   };
   DotNetCoreBuild(apiProject, settings);
});

Task("PublishApi")
   .IsDependentOn("BuildApi")
   .Does(() =>
{
   var settings = new DotNetCorePublishSettings
   {
      Configuration = configuration,
      OutputDirectory = apiPublishDir
   };

   DotNetCorePublish(apiProject, settings);
});


RunTarget(target);