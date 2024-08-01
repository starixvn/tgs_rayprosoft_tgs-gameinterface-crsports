using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Security.Authentication;
using StackExchange.Redis;
using Tgs.GameInterface.CRSports.Core;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddGameInterfaceCore(
		this IServiceCollection services,
		Func<IServiceProvider, IDatabase> redisDatabaseResolver)
		=> services
		.RegisterAllServiceUtilityTypes(
			Assembly.GetExecutingAssembly(),
			ServiceLifetime.Transient)
		.AddHttpClient<IGameApiClient, GameApiClient>()

		.ConfigurePrimaryHttpMessageHandler(sp =>
		{
			var handler = new HttpClientHandler();
			handler.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13 | SslProtocols.Tls11 | SslProtocols.Tls | SslProtocols.Ssl3 | SslProtocols.Ssl2;
			handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
			return handler;
		})
		.Services
		.AddSingleton(TimeProvider.System)
		.AddSingleton<DateTimeConverter>()
		.AddSingleton<ICryptographyUtensil, CryptographyUtensil>()
		.AddSingleton<ITokenUtensil>((sp) => ActivatorUtilities.CreateInstance<TokenUtensil>(sp, redisDatabaseResolver(sp), TimeSpan.FromMinutes(10)));
}
