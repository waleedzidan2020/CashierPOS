using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Win32;
using CashierPOS.Application;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
namespace CashierPOS.WPF;
public sealed record Field(string Key,string Label,string Value="",bool Secret=false,string[]? Choices=null,bool Check=false);
public sealed class DialogService {
 public Dictionary<string,string>? Form(string title,params Field[] fields){var win=new Window{Title=title,Width=460,MaxHeight=690,SizeToContent=SizeToContent.Height,WindowStartupLocation=WindowStartupLocation.CenterOwner,Owner=System.Windows.Application.Current.MainWindow};var root=new StackPanel{Margin=new Thickness(24)};var editors=new Dictionary<string,FrameworkElement>();
  foreach(var f in fields){root.Children.Add(new TextBlock{Text=f.Label,TextWrapping=TextWrapping.Wrap,Margin=new Thickness(0,8,0,0)});FrameworkElement editor;
   if(f.Secret)editor=new PasswordBox();else if(f.Check)editor=new CheckBox{IsChecked=f.Value=="true",Content="Enabled / مفعّل",Margin=new Thickness(0,6,0,6)};else if(f.Choices!=null)editor=new ComboBox{ItemsSource=f.Choices,SelectedItem=f.Choices.Contains(f.Value)?f.Value:f.Choices.FirstOrDefault()};else editor=new TextBox{Text=f.Value};editors[f.Key]=editor;root.Children.Add(editor);}
  var buttons=new StackPanel{Orientation=Orientation.Horizontal,HorizontalAlignment=HorizontalAlignment.Right};var ok=new Button{Content="Save / حفظ",IsDefault=true};ok.Click+=(_,_)=>win.DialogResult=true;var cancel=new Button{Content="Cancel / إلغاء",IsCancel=true};buttons.Children.Add(cancel);buttons.Children.Add(ok);root.Children.Add(buttons);win.Content=new ScrollViewer{Content=root,VerticalScrollBarVisibility=ScrollBarVisibility.Auto};if(win.ShowDialog()!=true)return null;return editors.ToDictionary(x=>x.Key,x=>x.Value switch{PasswordBox p=>p.Password,CheckBox c=>c.IsChecked==true?"true":"false",ComboBox c=>Convert.ToString(c.SelectedItem)??"",TextBox t=>t.Text,_=>""});
 }
 public bool Confirm(string message)=>MessageBox.Show(System.Windows.Application.Current.MainWindow,message,"CashierPOS",MessageBoxButton.YesNo,MessageBoxImage.Question)==MessageBoxResult.Yes;
 public string? Open(string filter){var d=new OpenFileDialog{Filter=filter};return d.ShowDialog()==true?d.FileName:null;}
 public string? Save(string filter,string name){var d=new SaveFileDialog{Filter=filter,FileName=name};return d.ShowDialog()==true?d.FileName:null;}
}
public partial class LoginViewModel(SecurityService security,bool setup,string? lockedUsername=null):ObservableObject {
 [ObservableProperty] private string username=lockedUsername??ReadRemembered();[ObservableProperty] private string fullName="";[ObservableProperty] private string password="";[ObservableProperty] private string message="";[ObservableProperty] private bool busy;[ObservableProperty] private bool remember=true;
 public bool IsSetup=>setup;public bool UsernameEditable=>lockedUsername==null;public string Title=>setup?"Create your Manager account / إنشاء حساب المدير":"Welcome back / تسجيل الدخول";
 public Session? Authenticated{get;private set;}public event Action? Success;
 [RelayCommand] private async Task SignIn(){if(Busy)return;Busy=true;Message="";try{Authenticated=await Task.Run(()=>setup?security.Setup(Username,FullName,Password):security.Login(Username,Password));if(Remember&&lockedUsername==null)File.WriteAllText(System.IO.Path.Combine(App.DataDirectory,"username.txt"),Username);else if(lockedUsername==null)File.Delete(System.IO.Path.Combine(App.DataDirectory,"username.txt"));Password="";Success?.Invoke();}catch(Exception e){Message=e is DomainException?e.Message:"Could not sign in. Check the database and application log.";}finally{Busy=false;}}
 private static string ReadRemembered(){var p=System.IO.Path.Combine(App.DataDirectory,"username.txt");return File.Exists(p)?File.ReadAllText(p):"";}
}
