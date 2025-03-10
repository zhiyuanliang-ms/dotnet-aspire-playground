using Azure.Provisioning.AppConfiguration;

var builder = DistributedApplication.CreateBuilder(args);

var appConfig = builder.AddAzureAppConfiguration("aspire-appconfig");
    //.ConfigureInfrastructure(infra =>
    //{
    //    var resource = infra.GetProvisionableResources()
    //        .OfType<AppConfigurationStore>()
    //        .Single();
    //    resource.SkuName = "free";
    //    resource.Tags.Add("TestKey", "Some value");
    //});

var keyVault = builder.AddAzureKeyVault("aspire-keyvault");

builder.AddProject<Projects.WorkerService>("workerservice")
    .WithReference(appConfig)
    .WithReference(keyVault);


builder.Build().Run();
