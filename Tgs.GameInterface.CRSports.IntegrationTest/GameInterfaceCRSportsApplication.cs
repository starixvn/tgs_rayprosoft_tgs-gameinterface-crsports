using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NSubstitute;

using OpenTelemetry.Exporter;
using Tgs.GameInterface.CRSports.Core;

namespace Tgs.GameInterface.CRSports.IntegrationTest;

internal class GameInterfaceCRSportsApplication(Action<IWebHostBuilder>? webHostConfigure = null) : WebApplicationFactory<Program>
{
	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		_ = builder
			.ConfigureAppConfiguration(c => c.Sources.Clear())
			.ConfigureServices(services => _ = services.AddSingleton<IConfigureOptions<OtlpExporterOptions>>(
				sp => new ConfigureNamedOptions<OtlpExporterOptions>(
					Options.DefaultName,
					exporterOptions => exporterOptions.TimeoutMilliseconds = 0))
			.AddFakeNatsMessageQueue()
			.AddSingleton(Substitute.For<ITokenUtensil>())
			.AddSingleton(Substitute.For<IGameApiClient>())
			.AddSingleton(Substitute.For<ICryptographyUtensil>())
			/*
			.AddSingleton<IConfigureOptions<RedisOptions>>(
				sp => new ConfigureNamedOptions<RedisOptions>(
					Options.DefaultName,
					options => options.ConnectionString = "localhost"))
			.AddSingleton<IConfigureOptions<GameInterfaceSettingsOptions>>(
				sp => new ConfigureNamedOptions<GameInterfaceSettingsOptions>(
					Options.DefaultName,
					options => options.PlayerPassword = "TgsPassw0rd"))
			.AddSingleton<IConfigureOptions<PolicySettingsOptions>>(
				sp => new ConfigureNamedOptions<PolicySettingsOptions>(
					Options.DefaultName,
					options =>
					{
						options.GlobalRetryCount = 1;
						options.GlobalRetryWaitSec = 1;
					}))
			*/
			);
		webHostConfigure?.Invoke(builder);
	}
}
