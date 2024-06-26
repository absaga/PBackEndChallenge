using Api.Configuration;
using Api.Data;
using Api.Models;
using Api.Repositories;
using Api.Rules.SalaryDeduction;
using Api.Services;
using Api.Services.Mapping;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSingleton<IDataProvider, ProvidedDataProvider>();
builder.Services.AddScoped<IRepository<Employee>, EmployeeRepository>();
builder.Services.AddScoped<IDependentRepository, DependentRepository>();
builder.Services.AddScoped<IMapperService, MapperService>();
builder.Services.AddScoped<IDependentService, DependentService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();

builder.Services.Configure<SalarySettings>(builder.Configuration.GetSection("SalarySettings"));
builder.Services.Configure<SalaryDeductionSettings>(builder.Configuration.GetSection("SalaryDeductionSettings"));

/* 
 * I'm registering the salary deduction rules here individually, but on a real app I would do it through other means
 *
 */
builder.Services.AddScoped<ISalaryDeductionRule, BaseBenefitCostRule>();
builder.Services.AddScoped<ISalaryDeductionRule, BaseDependentCostRule>();
builder.Services.AddScoped<ISalaryDeductionRule, OverFiftyDependentFeeRule>();
builder.Services.AddScoped<ISalaryDeductionRule, HighSalaryFeeRule>();

builder.Services.AddScoped<ISalaryDeductionCalculatorService, SalaryDeductionCalculatorService>();
builder.Services.AddScoped<IPaycheckCalculatorService, PaycheckCalculatorService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Employee Benefit Cost Calculation Api",
        Description = "Api to support employee benefit cost calculations"
    });
});

var allowLocalhost = "allow localhost";
builder.Services.AddCors(options =>
{
    options.AddPolicy(allowLocalhost,
        policy => { policy.WithOrigins("http://localhost:3000", "http://localhost"); });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(allowLocalhost);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
