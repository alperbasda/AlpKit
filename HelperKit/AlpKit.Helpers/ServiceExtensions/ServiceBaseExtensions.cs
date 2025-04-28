using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace AlpKit.Helpers.ServiceExtensions;

public static class ServiceBaseExtensions
{
    /// <summary>
    /// T ile verilen generic sınııf ile aynı isimde appsettings te düğüm arar ve modeli DI a singleton olarak ekler
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static T AddSettings<T>(this IServiceCollection services, IConfiguration configuration)
where T : class, new()
    {
        var section = configuration.GetSection(typeof(T).Name);

        if (!section.Exists())
        {
            throw new InvalidOperationException($"Configuration section '{typeof(T).Name}' not found.");
        }

        var settings = section.Get<T>() ?? throw new InvalidOperationException($"Failed to bind configuration section '{typeof(T).Name}' to '{typeof(T)}'.");
        services.Configure<T>(section);
        services.AddSingleton(sp => sp.GetRequiredService<IOptions<T>>().Value);

        return settings;
    }


    /// <summary>
    /// Verilen tipten kalıtılan tüm sınıfları DI'a Scoped olarak ekler. addWithLifeCycle a değer geçilirse farklı tiplerde de ekleyebilir.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="assembly"></param>
    /// <param name="type"></param>
    /// <param name="addWithLifeCycle"></param>
    /// <returns></returns>
    public static IServiceCollection AddSubClassesOfType(
   this IServiceCollection services,
   Assembly assembly,
   Type type,
   Func<IServiceCollection, Type, IServiceCollection>? addWithLifeCycle = null
)
    {
        var types = assembly.GetTypes().Where(t => type.IsAssignableFrom(t) && type != t).ToList();
        foreach (var item in types)
        {
            if (addWithLifeCycle == null)
                services.AddScoped(item);

            else
                addWithLifeCycle(services, type);
        }

        return services;
    }
}
