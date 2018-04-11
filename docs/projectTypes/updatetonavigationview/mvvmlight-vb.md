# Update from HamburgerMenu to NavigationView in MVVM Light
If you have an UWP project created with WTS with project type **NavigationPane** and framework **MVVM Light**  please follow these steps to update to NavigationView:

## 1. Update min target version in project properties
NavigationView is a Fall Creators Update control, to start using it in your project is neccessary that you set FCU as min version.
![](../../resources/project-types/fcu-min-version.png)

## 2. Update ShellPage.xaml
The updated ShellPage will include the NavigationView and add the MenuItems directly in Xaml. The NavigationViewItems include an extension property that contains the target page type to navigate in the frame.

### XAML code you will have to remove:
 - **xmln namespaces** for fcu, cu, controls and vm (viewmodels).
 - DataTemplate **NavigationMenuItemDataTemplate** in Page resources.
 - **HamburgerMenu** control.

### XAML code you will have to add:
 - **namespaces**: xmlns:helpers="using:myAppNamespace.Helpers"
 - **NavigationView** control.
 - **MenuItems** inside of the NavigationView.
 - **HeaderTemplate** inside of the NavigationView.

 The resulting code should look like this:

```xml
<Page
    x:Class="SampleApp.Views.ShellPage"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
    xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
    DataContext="{Binding ShellViewModel, Source={StaticResource Locator}}"
    xmlns:helpers="using:SampleApp.Helpers"
    xmlns:ic="using:Microsoft.Xaml.Interactions.Core"
    xmlns:i="using:Microsoft.Xaml.Interactivity"
    mc:Ignorable="d">

    <NavigationView
        x:Name="navigationView"
        SelectedItem="{x:Bind ViewModel.Selected, Mode=OneWay}"
        Header="{Binding Selected.Title}"
        IsSettingsVisible="False"
        Background="{ThemeResource SystemControlBackgroundAltHighBrush}">
        <NavigationView.MenuItems>
            <!--
            TODO WTS: Change the symbols for each item as appropriate for your app
            More on Segoe UI Symbol icons: https://docs.microsoft.com/windows/uwp/style/segoe-ui-symbol-font
            Or to use an IconElement instead of a Symbol see https://github.com/Microsoft/WindowsTemplateStudio/blob/master/docs/projectTypes/navigationpane.md
            Edit String/en-US/Resources.resw: Add a menu item title for each page
            -->
            <NavigationViewItem x:Uid="Shell_Main" Icon="Document"  helpers:NavHelper.NavigateTo="SampleApp.ViewModels.MainViewModel" />
        </NavigationView.MenuItems>
        <NavigationView.HeaderTemplate>
            <DataTemplate>
                <TextBlock
                    Style="{StaticResource TitleTextBlockStyle}"
                    Margin="12,0,0,0"
                    VerticalAlignment="Center"
                    Text="{Binding Selected.Content}" />
            </DataTemplate>
        </NavigationView.HeaderTemplate>
        <i:Interaction.Behaviors>
            <ic:EventTriggerBehavior EventName="ItemInvoked">
                <ic:InvokeCommandAction Command="{x:Bind ViewModel.ItemInvokedCommand}" />
            </ic:EventTriggerBehavior>
        </i:Interaction.Behaviors>
        <Grid>
            <Frame x:Name="shellFrame" />
        </Grid>
    </NavigationView>
</Page>
```
## 3. Update ShellPage.xaml.cs
ShellViewModel will need the NavigationView instance (explained below), you will have to add it on initialization.

### VB code you will have to modify:
 - Add the navigationView control to ViewModel initialization.

The resulting code should look like this:
 ```vbnet
Public NotInheritable Partial Class ShellPage
    Inherits Page

    Private ReadOnly Property ViewModel As ShellViewModel
        Get
            Return TryCast(DataContext, ShellViewModel)
        End Get
    End Property

    Public Sub New()
        Me.InitializeComponent()
        DataContext = ViewModel
        ViewModel.Initialize(shellFrame, navigationView)
    End Sub
End Class
```

## 4. Add NavHelper.cs

Add this extension class in the **Helpers** folder to the project. This allows the NavigationViewItems to contain a Type property that is used for navigation.
```vbnet
Public Class NavHelper

    Public Shared Function GetNavigateTo(item As NavigationViewItem) As String
        Return TryCast(item.GetValue(NavigateToProperty), String)
    End Function

    Public Shared Sub SetNavigateTo(item As NavigationViewItem, value As String)
        item.SetValue(NavigateToProperty, value)
    End Sub

    Public Shared ReadOnly NavigateToProperty As DependencyProperty =
        DependencyProperty.RegisterAttached("NavigateTo", GetType(String), GetType(NavHelper), New PropertyMetadata(Nothing))
End Class
```

## 5. Update ShellViewModel.cs
ShellViewModel's complexity will be reduced significantly, these are the changes that you will have to implement on the class.

### VB code you will have to remove:
 - private **const properties** for Visual States (Panoramic, Wide, Narrow).
 - private field **_lastSelectedItem**
 - **IsPaneOpen** observable property.
 - **DisplayMode** observable property.
 - **ObservableCollections** properties for **PrimaryItems** and **SecondaryItems**.
 - **OpenPaneCommand**.
 - **ItemSelectedCommand** and handler method **ItemSelected**.
 - **StateChangedCommand** and handler method **GoToState**.
 - **PopulateNavItems** method and method call from Initialize.
 - **InitializeState** method and method call from Initialize.
 - **ChangeSelected** and **Navigate** method.

### VB code you will have to add _(implementation below)_:
 - **_navigationView** private property of type NavigationView.
 - **ItemInvokedCommand** and handler method **OnItemInvoked**.
  - **IsMenuItemForPageType** method.

### VB code you will have to update _(implementation below)_:
 - **Initialize** method.
 - **Frame_Navigated** method with the implementation below.

 The resulting code should look like this:
```vbnet
Public Class ShellViewModel
    Inherits ViewModelBase

    Private _navigationView As NavigationView

    Private _selected As Object

    Private _itemInvokedCommand As ICommand

    Public ReadOnly Property NavigationService As NavigationServiceEx
        Get
            Return CommonServiceLocator.ServiceLocator.Current.GetInstance(Of NavigationServiceEx)()
        End Get
    End Property

    Public Property Selected As Object
        Get
            Return _selected
        End Get

        Set(value As Object)
            [Set](_selected, value)
        End Set
    End Property

    Public ReadOnly Property ItemInvokedCommand As ICommand
        Get
            If _itemInvokedCommand Is Nothing Then
                _itemInvokedCommand = New RelayCommand(Of NavigationViewItemInvokedEventArgs)(AddressOf OnItemInvoked)
            End If

            Return _itemInvokedCommand
        End Get
    End Property

    Public Sub Initialize(frame As Frame, navigationView As NavigationView)
        _navigationView = navigationView
        NavigationService.Frame = frame
        AddHandler NavigationService.Navigated, AddressOf Frame_Navigated
    End Sub

    Private Sub OnItemInvoked(args As NavigationViewItemInvokedEventArgs)
        Dim item = _navigationView.MenuItems.OfType(Of NavigationViewItem)().First(Function(menuItem) CStr(menuItem.Content) = CStr(args.InvokedItem))
        Dim pageKey = TryCast(item.GetValue(NavHelper.NavigateToProperty), String)
        NavigationService.Navigate(pageKey)
    End Sub

    Private Sub Frame_Navigated(sender As Object, e As NavigationEventArgs)
        Dim selectedItem = _navigationView.MenuItems.OfType(Of NavigationViewItem)().FirstOrDefault(Function(menuItem) IsMenuItemForPageType(menuItem, e.SourcePageType))

        If selectedItem IsNot Nothing Then
            Selected = selectedItem
        End If
    End Sub

    Private Function IsMenuItemForPageType(menuItem As NavigationViewItem, sourcePageType As Type) As Boolean
        Dim navigatedPageKey = NavigationService.GetNameOfRegisteredPage(sourcePageType)
        Dim pageKey = TryCast(menuItem.GetValue(NavHelper.NavigateToProperty), String)
        Return pageKey = navigatedPageKey
    End Function
End Class
```

## 6. Remove ShellNavigationItem.cs
ShellNavigationItem is no longer used and you should remove it from the project.

## 7. Update XAML code for all pages
The pages do no longer need the TitlePage TextBlock and the Adaptive triggers, because the page title will be displayed on the NavigationView HeaderTemplate and the responsive behaviors will be added by NavigationView control.

### XAML code you will have to remove:
 - **xmln namespaces** for fcu and cu.
 - Textblock **TitlePage**
 - ContentArea Grid **RowDefinitions**
 - VisualStateManager **VisualStateGroups**.
 - **Grid.Row="1"** property  in the content Grid.

The resulting code should look like this:
```xml
<Page
    x:Class="SampleApp.Views.MainPage"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
    xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
    Style="{StaticResource PageStyle}"
    DataContext="{Binding MainViewModel, Source={StaticResource Locator}}"
    mc:Ignorable="d">
    <Grid
        x:Name="ContentArea"
        Margin="{StaticResource MediumLeftRightMargin}">
        <Grid
            Background="{ThemeResource SystemControlPageBackgroundChromeLowBrush}">
            <!--The SystemControlPageBackgroundChromeLowBrush background represents where you should place your content. 
                Place your content here.-->
        </Grid>
    </Grid>
</Page>
```