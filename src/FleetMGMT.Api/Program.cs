using FleetMGMT.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.AddConfiguration();
builder.AddServices();
builder.AddSwagger();

var app = builder.Build();
app.MapControllers();

app.UseSwagger();
app.UseSwaggerUI();

app.Run();