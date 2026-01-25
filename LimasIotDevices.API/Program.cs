using LimasIotDevices.Application;
using LimasIotDevices.Infrastructure;
using LimasIoTDevices.API;
using LimasIoTDevices.Shared;

SetupInfrastructure.SetupEnvironmentVariables();

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddShared();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddAplication();
builder.Services.AddApi(builder.Configuration);

var app = builder.Build();
app.UseInfrastructureSettings(builder.Configuration);
app.UseApi();

app.Run();
