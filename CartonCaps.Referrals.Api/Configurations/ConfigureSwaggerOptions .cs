using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CartonCaps.Referrals.ApiMock.Configurations
{
    public class ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider _provider = provider;

        public void Configure(SwaggerGenOptions options)
        {
            foreach (var desc in _provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(desc.GroupName, new OpenApiInfo
                {
                    Title = "CartonCaps Referrals API",
                    Version = desc.ApiVersion.ToString(),
                    Description = desc.IsDeprecated ? "This version is deprecated." : null
                });
            }
        }
    }
}
