using Infra.MercadoPago.Gateway;
using Infra.Transactions;
using InterfaceAdapters.MercadoPago.Gateway.Interfaces;
using InterfaceAdapters.Transactions.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infra;

public static class InfraBootstrapper
{
    public static void Register(IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IUnitOfWorks, UnitOfWorks>();
        services.AddTransient<IMercadoPagoGateway, MercadoPagoGateway>();

        services.AddNHibernate(configuration);

        services.AddHttpClient("MercadoPago.API", HttpClient =>
        {
            HttpClient.BaseAddress = new Uri(configuration["MercadoPago.API:UrlBase"]);
            HttpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            HttpClient.DefaultRequestHeaders.Add("User-Agent", AppDomain.CurrentDomain.FriendlyName);
        });
    }
}
