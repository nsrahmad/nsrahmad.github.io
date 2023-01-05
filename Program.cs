await Bootstrapper
  .Factory
  .CreateWeb(args)
  .ConfigureFileSystem(fs => fs.OutputPath = "docs")
  .RunAsync();