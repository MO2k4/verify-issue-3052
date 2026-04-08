var builder = DistributedApplication.CreateBuilder(args);

builder.AddAzureFunctionsProject<Projects.VerifyIssue3052_FuncApp>("funcapp");

builder.Build().Run();
