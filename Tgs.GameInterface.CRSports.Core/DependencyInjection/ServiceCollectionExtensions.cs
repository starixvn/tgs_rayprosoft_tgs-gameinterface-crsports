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
		/*.ConfigurePrimaryHttpMessageHandler(x => new HttpClientHandler()
		{
			SslProtocols = SslProtocols.Tls13 | SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls,
		})
		.UseSocketsHttpHandler(x => new CipherSuitesPolicy(
		[
			TlsCipherSuite.TLS_RSA_WITH_AES_128_CBC_SHA,
			TlsCipherSuite.TLS_RSA_WITH_AES_256_CBC_SHA,
			TlsCipherSuite.TLS_RSA_WITH_3DES_EDE_CBC_SHA,
			TlsCipherSuite.TLS_RSA_WITH_AES_128_CBC_SHA256 ,
			TlsCipherSuite.TLS_RSA_WITH_AES_256_CBC_SHA256  ,
		]))*/
		.Services
		.AddSingleton(TimeProvider.System)
		.AddSingleton<DateTimeConverter>()
		.AddSingleton<ICryptographyUtensil, CryptographyUtensil>()
		.AddSingleton<ITokenUtensil>((sp) => ActivatorUtilities.CreateInstance<TokenUtensil>(sp, redisDatabaseResolver(sp), TimeSpan.FromMinutes(10)));
}
