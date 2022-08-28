using Logic;
using Microsoft.Extensions.DependencyInjection;

public static class LogicModule
{
    public static IServiceCollection AddLogic(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton<IDataManager>(_ =>
        {
            return new DataManager(connectionString);
        });
        return services;
    }
}