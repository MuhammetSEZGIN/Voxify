using Ocelot.DependencyInjection;
using Ocelot.Middleware;
var builder = WebApplication.CreateBuilder(args);


builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Configuration.AddJsonFile("swagger.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot(builder.Configuration);
builder.Services.AddSwaggerForOcelot(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
   app.UseSwaggerForOcelotUI(options =>
   {
       options.PathToSwaggerGenerator = "/swagger/docs";
   });
}

await app.UseOcelot();
app.Run();
