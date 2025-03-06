var builder = DistributedApplication.CreateBuilder(args);

var kvServer = builder.AddProject<Projects.KvServer>("kvserver");

builder.AddProject<Projects.WorkerService>("workerservice")
    .WithReference(kvServer);

builder.Build().Run();
