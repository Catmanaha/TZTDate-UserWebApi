using TZTDate_UserWebApi.Middlewares;
using TZTDate_UserWebApi.Extensions;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.Inject();
builder.Services.InitDbContext(builder.Configuration);
builder.Services.Configure(builder.Configuration);
builder.Services.AddMediatR(o => o.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

var app = builder.Build();
    
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.MapDefaultControllerRoute();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.Run();
