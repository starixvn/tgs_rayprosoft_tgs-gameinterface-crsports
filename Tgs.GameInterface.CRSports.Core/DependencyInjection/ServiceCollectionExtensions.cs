using System.Reflection;
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
		.Services
		.AddSingleton(TimeProvider.System)
		.AddSingleton<DateTimeConverter>()
		.AddSingleton<ICryptographyUtensil, CryptographyUtensil>()
		.AddSingleton<ITokenUtensil>((sp) => ActivatorUtilities.CreateInstance<TokenUtensil>(sp, redisDatabaseResolver(sp), TimeSpan.FromMinutes(10)));
}
