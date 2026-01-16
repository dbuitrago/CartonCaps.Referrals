using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using CartonCaps.Referrals.ApiMock.Configurations;
using CartonCaps.Referrals.ApiMock.Middlewares;
using CartonCaps.Referrals.Business.Mocks;
using CartonCaps.Referrals.Business.Services;
using CartonCaps.Referrals.Common.Services;
using CartonCaps.Referrals.Common.Vendor;
using CartonCaps.Referrals.Contracts.Data.Repositories;
using CartonCaps.Referrals.Contracts.Services;
using CartonCaps.Referrals.Data;
using CartonCaps.Referrals.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
})
.AddMvc();

builder.Services.AddApiVersioning().AddApiExplorer(options => {
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
});

builder.Services.AddDbContext<ReferralDbContext>(options =>
    options.UseSqlite("Data Source=referrals.db"));

RegisterDI(builder);

builder.Services.AddControllers();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ReferralDbContext>();
    db.Database.EnsureCreated();
}

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

        foreach (var desc in provider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint($"/swagger/{desc.GroupName}/swagger.json", desc.GroupName.ToUpperInvariant());
        }
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

static void RegisterDI(WebApplicationBuilder builder)
{
    builder.Services.AddScoped<IReferralLinkRepository, ReferralLinkRepository>();
    builder.Services.AddScoped<IReferralRepository, ReferralRepository>();

    builder.Services.AddScoped<IReferralLinkService, ReferralLinkService>();
    builder.Services.AddScoped<IReferralResolveService, ReferralLinkService>();
    builder.Services.AddScoped<IReferralService, ReferralService>();

    builder.Services.AddScoped<ILinkVendorClient, LinkVendorClientMock>();
}