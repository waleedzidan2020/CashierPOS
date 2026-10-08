using System.Windows;
using System.Windows.Controls;
namespace CashierPOS.WPF;
public partial class LoginWindow:Window {
 private readonly LoginViewModel vm;public LoginWindow(LoginViewModel viewModel){InitializeComponent();vm=viewModel;DataContext=vm;vm.Success+=()=>DialogResult=true;}
 private void PasswordChanged(object sender,RoutedEventArgs e){if(DataContext is LoginViewModel m)m.Password=((PasswordBox)sender).Password;}
 private void ShowPassword(object sender,RoutedEventArgs e){VisiblePassword.Visibility=Visibility.Visible;Secret.Visibility=Visibility.Collapsed;}
 private void HidePassword(object sender,RoutedEventArgs e){if(DataContext is LoginViewModel m)Secret.Password=m.Password;VisiblePassword.Visibility=Visibility.Collapsed;Secret.Visibility=Visibility.Visible;}
}
