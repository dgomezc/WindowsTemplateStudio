using EasyTablesPoc.Helpers;
using EasyTablesPoc.Models;
using EasyTablesPoc.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Linq;

namespace EasyTablesPoc.ViewModels
{
    public class TodoListViewModel : Observable
    {
        private bool _isBusy;
        private string _statusText;

        private ObservableCollection<TodoItem> _todoItems;
        private TodoItem _editableTodoItem;
        private TodoItem _selectedTodoItem;

        private RelayCommand _loadTodoItemsCommand;
        private RelayCommand _newTodoItemCommand;
        private RelayCommand _saveTodoItemCommand;
        private RelayCommand _deleteTodoItemCommand;

        private readonly TodoItemService _service = TodoItemService.Instance;

        public TodoListViewModel()
        {
            CreateEmptyTodoItem();
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
