var target = Argument("target", "PublishAll");
var configuration = Argument("configuration", "Release");

var apiPublishDir = "deploy/api/api-build";
var apiProject = "Landing.API/Landing.API.csproj";

var adminPublishDir = "deploy/admin/admin-build";
var adminProject = "AdminPanel/AdminPanel.csproj";

var previewPublishDir = "deploy/preview/preview-build";
var previewProject = "PreviewPanel/PreviewPanel.csproj";

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

Task("BuildAdmin")
   .IsDependentOn("RestoreSolution")
   .Does(() =>
{
   var settings = new DotNetCoreBuildSettings {
      Configuration = configuration
   };
   DotNetCoreBuild(adminProject, settings);
});

Task("PublishAdmin")
   .IsDependentOn("BuildAdmin")
   .Does(() =>
{
   var settings = new DotNetCorePublishSettings
   {
      Configuration = configuration,
      OutputDirectory = adminPublishDir
   };

   DotNetCorePublish(adminProject, settings);
});

Task("BuildPreview")
   .IsDependentOn("RestoreSolution")
   .Does(() =>
{
   var settings = new DotNetCoreBuildSettings {
      Configuration = configuration
   };
   DotNetCoreBuild(previewProject, settings);
});

Task("PublishPreview")
   .IsDependentOn("BuildPreview")
   .Does(() =>
{
   var settings = new DotNetCorePublishSettings
   {
      Configuration = configuration,
      OutputDirectory = previewPublishDir
   };

   DotNetCorePublish(previewProject, settings);
});

Task("PublishAll")
   .IsDependentOn("PublishApi")
   .IsDependentOn("PublishAdmin")
   .IsDependentOn("PublishPreview")
   .Does(() =>
{
});

RunTarget(target);