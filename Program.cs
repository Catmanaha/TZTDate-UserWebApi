using TZTDate_UserWebApi.Middlewares;
using TZTDate_UserWebApi.Extensions;
using System.Reflection;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.ResponseCompression;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers().AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);

builder.Services.AddResponseCompression(opts =>
{
  opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(["application/octet-stream"]);
});

builder.Services.Inject();
builder.Services.InitDbContext(builder.Configuration);
builder.Services.Configure(builder.Configuration);
builder.Services.AddMediatR(o => o.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
builder.Services.AddCors(options =>
{
  options.AddPolicy("BlazorWasmPolicy", corsBuilder =>
  {
    corsBuilder
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod();
  });
});
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.MapDefaultControllerRoute();
app.UseResponseCompression();
app.UseCors("BlazorWasmPolicy");

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.Run();
