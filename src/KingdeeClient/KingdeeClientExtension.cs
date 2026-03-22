using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KingdeeClient
{
    public static class KingdeeClientExtension
    {
        public static IServiceCollection AddKingdeeClient(this IServiceCollection services, string configNodeName, IConfiguration configuration)
        {
            //services.Configure(configure);
            //services.AddSingleton<IQueuePolicy, QueuePolicy>();  //QueuePolicy以单例方式添加到容器中

            services.AddHttpClient(configNodeName, client =>
            {
                client.BaseAddress = new Uri(configNodeName);
                client.DefaultRequestHeaders.UserAgent.ParseAdd("HttpClient/8.0");
            });

            services.AddTransient<IKingdeeClient, DefaultKingdeeClient>();
            return services;
        }


        public static IServiceCollection AddMultiKingdeeClient(this IServiceCollection services, string configNodeName, IConfiguration configuration, List<string> keys)
        {

            services.AddHttpClient(configNodeName, client =>
            {
                client.BaseAddress = new Uri(configNodeName);
                client.DefaultRequestHeaders.UserAgent.ParseAdd("HttpClient/8.0");
            });

            foreach (var key in keys)
            {
                services.AddKeyedTransient<IKingdeeClient, DefaultKingdeeClient>(key);
            }
            return services;
        }
    }
}
