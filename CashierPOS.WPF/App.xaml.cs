using System.Windows;
using Velopack;
using CashierPOS.Application;
using CashierPOS.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
namespace CashierPOS.WPF;
public partial class App : System.Windows.Application {
 [STAThread]
 private static void Main(string[] args) {
  VelopackApp.Build().Run();
  var app = new App();
  app.InitializeComponent();
  app.Run();
 }
 private Mutex? instance;private ServiceProvider? provider;
 public static string DataDirectory=>System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"CashierPOS");
 protected override async void OnStartup(StartupEventArgs e){base.OnStartup(e);instance=new Mutex(true,"Local\\CashierPOS.SingleDrawer",out var created);if(!created){MessageBox.Show("CashierPOS is already running.");Shutdown();return;}Directory.CreateDirectory(DataDirectory);
  var services=new ServiceCollection();services.AddSingleton(new SqliteFactory(System.IO.Path.Combine(DataDirectory,"pos.db")));services.AddSingleton<IDbFactory>(sp=>sp.GetRequiredService<SqliteFactory>());services.AddSingleton<IPasswordHasher,PasswordHasher>();services.AddSingleton<IClock,SystemClock>();services.AddSingleton<SecurityService>();services.AddSingleton<PosService>();services.AddSingleton<ExcelService>();services.AddSingleton<BackupService>();services.AddSingleton<DialogService>();services.AddSingleton<PrintService>();services.AddLogging(b=>b.AddProvider(new FileLoggerProvider(System.IO.Path.Combine(DataDirectory,"Logs","app.log"))));provider=services.BuildServiceProvider();
  DispatcherUnhandledException+=(s,args)=>{provider.GetRequiredService<ILogger<App>>().LogError(args.Exception,"Unhandled UI failure");MessageBox.Show("Unexpected error. Your committed data is preserved. See the application log.");args.Handled=true;};
  try{await Task.Run(()=>provider.GetRequiredService<SqliteFactory>().Initialize());ShowLogin();}catch(Exception ex){provider.GetRequiredService<ILogger<App>>().LogError(ex,"Startup failed");MessageBox.Show("Could not open application database: "+ex.Message);Shutdown();}
 }
 private void ShowLogin(){var security=provider!.GetRequiredService<SecurityService>();var vm=new LoginViewModel(security,security.NeedsSetup());var login=new LoginWindow(vm);if(login.ShowDialog()!=true||vm.Authenticated==null){Shutdown();return;}var mainVm=ActivatorUtilities.CreateInstance<MainViewModel>(provider!,vm.Authenticated);var window=new MainWindow(mainVm);MainWindow=window;mainVm.EndSession+=()=>{window.AllowClose=true;window.Close();security.Logout(mainVm.Session);ShowLogin();};var updater=new AutoUpdateService(window,()=>!mainVm.Busy&&mainVm.Cart.Count==0);window.Closed+=(_,_)=>{updater.Dispose();if(!window.AllowClose)Shutdown();};window.Show();}
 protected override void OnExit(ExitEventArgs e){provider?.Dispose();instance?.Dispose();base.OnExit(e);}
}
public sealed class FileLoggerProvider(string path):ILoggerProvider {
 private readonly object gate=new();private readonly string logPath=path;public ILogger CreateLogger(string categoryName)=>new FileLogger(this,categoryName);public void Dispose(){}
 private sealed class FileLogger(FileLoggerProvider owner,string category):ILogger {
  public IDisposable? BeginScope<TState>(TState state) where TState:notnull=>null;public bool IsEnabled(LogLevel level)=>level>=LogLevel.Warning;
  public void Log<TState>(LogLevel level,EventId id,TState state,Exception? exception,Func<TState,Exception?,string> formatter){if(!IsEnabled(level))return;lock(owner.gate){try{Directory.CreateDirectory(System.IO.Path.GetDirectoryName(owner.logPath)!);File.AppendAllText(owner.logPath,$"{DateTime.UtcNow:O} {level} {category}: {formatter(state,exception)} {exception?.ToString()}\n");}catch(IOException){}}}
 }
}
