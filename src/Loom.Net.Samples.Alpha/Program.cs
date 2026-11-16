using Loom.Net.AspNetCore.Configuration;
using Loom.Net.AspNetCore.Extensions;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.AddLoom(options => options
    .LoadFromConfiguration(builder.Configuration)
);

var app = builder.Build();

app.UseLoom();

app.MapGet("helloworld", () => "Hello World!");

app.Run();
