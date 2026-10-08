using System.Windows;
using System.Windows.Threading;
using Velopack;
using Velopack.Exceptions;
using Velopack.Sources;

namespace CashierPOS.WPF;

// Downloads updates in the background. Never interrupt a sale to restart the POS.
// If an update is downloaded but the operator declines to restart, Velopack applies
// it at the next launch, before the normal WPF startup.
public sealed class AutoUpdateService : IDisposable
{
    private const string Repository = "https://github.com/waleedzidan2020/CashierPOS";
    private readonly Window owner;
    private readonly Func<bool> safeToRestart;
    private readonly DispatcherTimer timer = new() { Interval = TimeSpan.FromMinutes(5) };
    private bool busy;
    private bool disposed;
    private bool prompted;

    public AutoUpdateService(Window owner, Func<bool> safeToRestart)
    {
        this.owner = owner;
        this.safeToRestart = safeToRestart;
        owner.Loaded += OnLoaded;
        timer.Tick += OnTimer;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        owner.Loaded -= OnLoaded;
        timer.Start();
        await CheckForUpdatesAsync();
    }

    private async void OnTimer(object? sender, EventArgs e) => await CheckForUpdatesAsync();

    private async Task CheckForUpdatesAsync()
    {
        if (disposed || busy) return;
        busy = true;
        try
        {
            var updater = new UpdateManager(new GithubSource(Repository, accessToken: null, prerelease: false));
            var update = await updater.CheckForUpdatesAsync();
            if (update is null) return;

            await updater.DownloadUpdatesAsync(update);
            if (disposed || !owner.IsVisible || prompted || !safeToRestart()) return;

            prompted = true;
            var choice = MessageBox.Show(owner,
                "A new CashierPOS update has been downloaded. Restart now to install it?\n" +
                "تم تنزيل تحديث جديد. هل تريد إعادة تشغيل البرنامج لتثبيته؟\n\n" +
                "If you choose No, it will be installed when you next open CashierPOS.",
                "CashierPOS update",
                MessageBoxButton.YesNo,
                MessageBoxImage.Information);

            if (choice == MessageBoxResult.Yes && safeToRestart())
                updater.ApplyUpdatesAndRestart(update);
        }
        catch (NotInstalledException)
        {
            // Developers running from bin/ or Portable do not have Velopack installed.
        }
        catch (Exception ex)
        {
            try
            {
                var folder = System.IO.Path.Combine(App.DataDirectory, "Logs");
                System.IO.Directory.CreateDirectory(folder);
                System.IO.File.AppendAllText(System.IO.Path.Combine(folder, "updates.log"),
                    $"{DateTime.UtcNow:O} {ex}\n");
            }
            catch (Exception) { /* Updates are optional; keep the POS operational. */ }
        }
        finally { busy = false; }
    }

    public void Dispose()
    {
        disposed = true;
        owner.Loaded -= OnLoaded;
        timer.Stop();
    }
}
