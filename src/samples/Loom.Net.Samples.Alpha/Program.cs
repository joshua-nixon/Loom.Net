using Loom.Net.AspNetCore.Configuration;
using Loom.Net.AspNetCore.Extensions;
using Serilog;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services
    .AddSerilog((_, config) =>
        config
            .ReadFrom.Configuration(builder.Configuration)
            .Enrich.FromLogContext()
    );

builder.Services.AddLoom(options => options
    .LoadFromConfiguration(builder.Configuration)
);

var app = builder.Build();

app.UseLoom();

app.MapGet("helloworld", () => "Hello World!");

app.MapGet("echo", (string message) => message);

app.MapGet("user/{userId:int}", (int userId) => $"User {userId}");


app.Run();
