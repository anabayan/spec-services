using Microsoft.Extensions.Configuration;

namespace BuildingBlocks.Core.Extensions;

public static class ConfigurationExtensions
{
    public static TOptions BindOptions<TOptions>(this IConfiguration configuration, string section)
        where TOptions : new()
    {
        var options = new TOptions();

        var optionsSection = configuration.GetSection(section);
        optionsSection.Bind(options);

        return options;
    }

    public static TOptions BindOptions<TOptions>(this IConfiguration configuration)
        where TOptions : new()
    {
        return BindOptions<TOptions>(configuration, typeof(TOptions).Name);
    }
}
