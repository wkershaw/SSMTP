var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.SSMTP_API>("ssmtp.api");

builder.AddProject<Projects.SSMTP_Server>("ssmtp.server");

builder.Build().Run();
