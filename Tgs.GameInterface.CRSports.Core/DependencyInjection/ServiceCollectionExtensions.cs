using System.Reflection;
using Microsoft.AspNetCore.Authentication;
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
		/*.ConfigurePrimaryHttpMessageHandler(sp =>
		{
			var handler = new HttpClientHandler
			{
				SslProtocols = SslProtocols.Ssl3 | SslProtocols.Ssl2 | SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls,
				ClientCertificateOptions = ClientCertificateOption.Manual,
				ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
			};
			return handler;
		})*/
		.Services
		.AddHttpClient<IAGLoginApiClient, AGLoginApiClient>()
		/*.ConfigurePrimaryHttpMessageHandler(sp =>
		{
			var handler = new HttpClientHandler
			{
				SslProtocols = SslProtocols.Ssl3 | SslProtocols.Ssl2 | SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls,
				ClientCertificateOptions = ClientCertificateOption.Manual,
				ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
			};
			return handler;
		})*/
		.Services
		.AddSingleton<ISystemClock, SystemClock>()
		.AddSingleton<DateTimeConverter>()
		.AddSingleton<ICryptographyUtensil, CryptographyUtensil>()
		.AddSingleton<ITokenUtensil>((sp) => ActivatorUtilities.CreateInstance<TokenUtensil>(sp, redisDatabaseResolver(sp), TimeSpan.FromMinutes(10)));
}
