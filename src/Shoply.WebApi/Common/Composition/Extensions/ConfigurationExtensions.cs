namespace Shoply.WebApi.Common.Composition.Extensions;

internal static class ConfigurationExtensions
{
    extension(IConfiguration configuration)
    {
        public T GetOptions<T>(string? sectionName = null!) where T : class
        {
            sectionName ??= typeof(T).Name;

            var options = configuration.GetSection(sectionName).Get<T>();
            ArgumentNullException.ThrowIfNull(sectionName);

            return options!;
        }
    }
}