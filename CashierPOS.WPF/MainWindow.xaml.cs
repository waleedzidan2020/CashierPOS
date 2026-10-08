using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
namespace CashierPOS.WPF;
public partial class MainWindow:Window {
 private readonly MainViewModel vm;private readonly DispatcherTimer timer=new(){Interval=TimeSpan.FromSeconds(15)};private DateTime lastInput=DateTime.UtcNow;private bool locked;
 public bool AllowClose{get;set;}
 public MainWindow(MainViewModel viewModel){InitializeComponent();vm=viewModel;DataContext=vm;vm.LockRequested+=Lock;timer.Tick+=async (_,_)=>{await vm.TryAutoBackup();if(!vm.Busy&&!locked&&DateTime.UtcNow-lastInput>TimeSpan.FromMinutes(vm.AutoLockMinutes))Lock();};timer.Start();Closed+=(_,_)=>timer.Stop();}
 private void WindowLoaded(object sender,RoutedEventArgs e){if(vm.IsPos)BarcodeInput.Focus();}
 private void TouchActivity(object sender,InputEventArgs e)=>lastInput=DateTime.UtcNow;
 private async void BarcodeKey(object sender,KeyEventArgs e){if(e.Key==Key.Enter){e.Handled=true;await vm.Scan();BarcodeInput.Focus();}}
 private void ProductDoubleClick(object sender,MouseButtonEventArgs e)=>vm.AddProductCommand.Execute(null);
 private void Shortcut(object sender,KeyEventArgs e){lastInput=DateTime.UtcNow;if(vm.Busy||locked)return;switch(e.Key){case Key.F2:BarcodeInput.Focus();e.Handled=true;break;case Key.F4:if(vm.IsPos)vm.DiscountDialogCommand.Execute(null);e.Handled=true;break;case Key.F5:vm.RefreshCommand.Execute(null);e.Handled=true;break;case Key.F6:if(vm.IsPos)vm.SuspendCommand.Execute(null);e.Handled=true;break;case Key.F7:if(vm.IsPos)vm.ResumeCommand.Execute(null);e.Handled=true;break;case Key.F8:CashInput.Focus();e.Handled=true;break;case Key.F9:if(vm.IsPos)vm.CompleteCommand.Execute(null);e.Handled=true;break;}}
 private void GenerateColumn(object sender,DataGridAutoGeneratingColumnEventArgs e){if(e.PropertyName=="Cost"&&!vm.Session.IsManager&&e.PropertyType==typeof(decimal?)){var rows=vm.Rows.Cast<object>().OfType<ProductRow>();if(rows.All(x=>x.Cost==null)){e.Cancel=true;return;}}e.Column.Header=vm.L[e.PropertyName];if(e.Column is DataGridTextColumn column&&column.Binding is System.Windows.Data.Binding binding&&(e.PropertyType==typeof(decimal)||e.PropertyType==typeof(decimal?)))binding.StringFormat="N2";}
 private void Lock(){if(locked||vm.Busy)return;locked=true;Root.IsEnabled=false;try{while(!vm.Unlock(this)){if(MessageBox.Show(this,"Stay locked and try again? No closes the application. / إعادة المحاولة؟ لا يغلق البرنامج.","Session locked",MessageBoxButton.YesNo)==MessageBoxResult.No){AllowClose=true;System.Windows.Application.Current.Shutdown();return;}}lastInput=DateTime.UtcNow;}finally{Root.IsEnabled=true;locked=false;}}
 private void WindowClosing(object? sender,CancelEventArgs e){if(AllowClose)return;if(vm.Busy){e.Cancel=true;return;}if(vm.Cart.Count>0&&MessageBox.Show(this,"Close and discard the unsuspended cart? / إغلاق ومسح السلة غير المعلقة؟","CashierPOS",MessageBoxButton.YesNo)!=MessageBoxResult.Yes)e.Cancel=true;}
}
