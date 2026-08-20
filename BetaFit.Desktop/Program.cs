using BetaFit.Desktop.Forms;
using BetaFit.Desktop.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace BetaFit.Desktop;
internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        var configuration=new ConfigurationBuilder().SetBasePath(AppContext.BaseDirectory).AddJsonFile("appsettings.json",false,false).AddEnvironmentVariables("BETAFIT_").Build();
        using var provider=new ServiceCollection().AddSingleton<IConfiguration>(configuration).AddSingleton<Session>().AddTransient<JwtHandler>().AddHttpClient<IBetaFitApiService,BetaFitApiService>((_,client)=>{var baseUrl=configuration["BetaFitApi:BaseUrl"]??"https://localhost:5001/";client.BaseAddress=new Uri(baseUrl.EndsWith('/')?baseUrl:baseUrl+"/");client.Timeout=TimeSpan.FromSeconds(15);}).AddHttpMessageHandler<JwtHandler>().Services.BuildServiceProvider();
        var api=provider.GetRequiredService<IBetaFitApiService>();var session=provider.GetRequiredService<Session>();
        var online=api.IsApiOnlineAsync().GetAwaiter().GetResult();
        if(!online){Application.Run(new FrmMain(api,session));return;}
        using var login=new FrmLogin(api,session);if(login.ShowDialog()!=DialogResult.OK)return;Application.Run(new FrmMain(api,session));
    }
}