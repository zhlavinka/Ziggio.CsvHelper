using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Ziggio.Csv.Extensions;

public static class ServiceCollectionExtensions {
  public static IServiceCollection AddZiggioCsv(this IServiceCollection services) {
    services.TryAddScoped<ICsvConfiguration, CsvConfiguration>();
    services.TryAddScoped<ICsvParser, CsvParser>();
    services.TryAddScoped<ICsvReader, CsvReader>();

    return services;
  }
}
