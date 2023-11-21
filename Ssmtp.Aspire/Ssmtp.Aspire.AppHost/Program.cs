var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.SSMTP_API>("ssmtp.api");

builder.Build().Run();
