using EasyTablesPoc.Helpers;
using EasyTablesPoc.Models;
using EasyTablesPoc.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace EasyTablesPoc.ViewModels
{
    public class TodoListViewModel : Observable
    {
        private bool _isBusy;
        private string _statusText;
        private string _errorText;

        private ObservableCollection<TodoItem> _todoItems;
        private TodoItem _editableTodoItem;
        private TodoItem _selectedTodoItem;
        private bool _isInternet;

        private RelayCommand _loadTodoItemsCommand;
        private RelayCommand _newTodoItemCommand;
        private RelayCommand _saveTodoItemCommand;
        private RelayCommand _deleteTodoItemCommand;

        private readonly TodoItemService _service = TodoItemService.Instance;

        public TodoListViewModel()
        {
            CreateEmptyTodoItem();

            IsInternet = InternetConnection.Instance.IsInternetAvailable;

            InternetConnection.Instance.OnInternetAvailabilityChange += async (isInternet) =>
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => IsInternet = isInternet);

            _service.OnResolveConflict += async (serverItem, localItem) => await _service_OnResolveConflict(serverItem, localItem);
        }

        private async Task _service_OnResolveConflict(TodoItem serverItem, TodoItem localItem)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                ErrorText += $"Changes to item {localItem.Text} are revert since there is a new version on the server.\n";
            });
        }

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                Set(ref _isBusy, value);
                LoadTodoItemsCommand.OnCanExecuteChanged();
                NewTodoItemCommand.OnCanExecuteChanged();
                SaveTodoItemCommand.OnCanExecuteChanged();
                DeleteTodoItemCommand.OnCanExecuteChanged();
            }
        }

        public string StatusText
        {
            get => _statusText;
            set => Set(ref _statusText, value);
        }

        public string ErrorText
        {
            get => _errorText;
            set => Set(ref _errorText, value);
        }

        public ObservableCollection<TodoItem> TodoItems
        {
            get => _todoItems;
            set => Set(ref _todoItems, value);
        }

        public TodoItem EditableTodoItem
        {
            get => _editableTodoItem;
            set
            {
                Set(ref _editableTodoItem, value);
                DeleteTodoItemCommand.OnCanExecuteChanged();
            }
        }

        public TodoItem SelectedTodoItem
        {
            get => _selectedTodoItem;
            set
            {
                Set(ref _selectedTodoItem, value);
                if (value != null)
                {
                    EditableTodoItem = value;
                }
            }
        }

        public bool IsInternet
        {
            get => _isInternet;
            set
            {
                Set(ref _isInternet, value);
                OnPropertyChanged(nameof(NoInternet));
            }
        }
        public bool NoInternet => !IsInternet;

        public RelayCommand LoadTodoItemsCommand => _loadTodoItemsCommand ?? (_loadTodoItemsCommand = new RelayCommand(async () => await LoadTodoItemsAsync(), () => !IsBusy));

        public RelayCommand NewTodoItemCommand => _newTodoItemCommand ?? (_newTodoItemCommand = new RelayCommand(CreateEmptyTodoItem, () => !IsBusy));

        public RelayCommand SaveTodoItemCommand => _saveTodoItemCommand ?? (_saveTodoItemCommand = new RelayCommand(async () => await SaveTodoItemAsync(), () => !IsBusy));

        public RelayCommand DeleteTodoItemCommand => _deleteTodoItemCommand ?? (_deleteTodoItemCommand = new RelayCommand(async () => await DeleteTodoItemAsync(), () => !IsBusy && CanDeleteTodoItem()));

        private async Task RefreshTodoItemsAsync()
        {
            StatusText = "Loading todoItems from Azure Easy Tables...";

            var todoItems = await _service.ReadAsync();
            TodoItems = new ObservableCollection<TodoItem>(todoItems);

            SelectedTodoItem = TodoItems.FirstOrDefault(i => i.Id == EditableTodoItem?.Id);
        }

        private async Task LoadTodoItemsAsync()
        {
            ErrorText = string.Empty;

            if (!IsBusy)
            {
                IsBusy = true;
                await RefreshTodoItemsAsync();
                IsBusy = false;

                StatusText = "Finished load todoItems!.";
            }
        }

        private async Task SaveTodoItemAsync()
        {
            ErrorText = string.Empty;

            if (!IsBusy)
            {
                IsBusy = true;
                StatusText = "Save todoItem...";

                await _service.AddOrUpdateAsync(EditableTodoItem);
                await RefreshTodoItemsAsync();

                IsBusy = false;
                StatusText = "TodoItem saved completed!.";
            }
        }

        private async Task DeleteTodoItemAsync()
        {
            ErrorText = string.Empty;

            if (!IsBusy)
            {
                IsBusy = true;
                StatusText = "Remove todoItem...";

                await _service.DeleteAsync(EditableTodoItem);
                EditableTodoItem = new TodoItem();
                await RefreshTodoItemsAsync();

                IsBusy = false;
                StatusText = "Remove todoItem completed!.";
            }
        }

        private void CreateEmptyTodoItem() => EditableTodoItem = new TodoItem();

        private bool CanDeleteTodoItem() => !string.IsNullOrEmpty(EditableTodoItem?.Id);
    }
}
