using EasyTablesPoc.Helpers;
using EasyTablesPoc.Models;
using EasyTablesPoc.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Linq;

namespace EasyTablesPoc.ViewModels
{
    public class MainViewModel : Observable
    {
        private string _statusText;
        public string StatusText
        {
            get => _statusText;
            set => Set(ref _statusText, value);
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                Set(ref _isBusy, value);
                LoadItemsCommand.OnCanExecuteChanged();
                NewItemCommand.OnCanExecuteChanged();
                SaveItemCommand.OnCanExecuteChanged();
                DeleteItemCommand.OnCanExecuteChanged();
            }
        }

        private ObservableCollection<Food> _items;
        public ObservableCollection<Food> Items
        {
            get => _items;
            set => Set(ref _items, value);
        }
        
        private Food _selectedItem;
        public Food SelectedItem
        {
            get => _selectedItem;
            set
            {
                Set(ref _selectedItem, value);
                if(value != null)
                {
                    EditableItem = value;
                }
            }
        }

        private Food _editableItem;
        public Food EditableItem
        {
            get => _editableItem;
            set
            {
                Set(ref _editableItem, value);
                DeleteItemCommand.OnCanExecuteChanged();
            }
        }

        private RelayCommand _loadItemsCommand;
        public RelayCommand LoadItemsCommand => _loadItemsCommand ?? (_loadItemsCommand = new RelayCommand(async () => await LoadItemsAsync(), () => !IsBusy));

        private RelayCommand _newItemCommand;
        public RelayCommand NewItemCommand => _newItemCommand ?? (_newItemCommand = new RelayCommand(CreateEmptyItem, () => !IsBusy));

        private RelayCommand _saveItemCommand;
        public RelayCommand SaveItemCommand => _saveItemCommand ?? (_saveItemCommand = new RelayCommand(async () => await SaveItem(), () => !IsBusy));

        private RelayCommand _deleteItemCommand;
        public RelayCommand DeleteItemCommand => _deleteItemCommand ?? (_deleteItemCommand = new RelayCommand(async () => await DeleteItem(), () =>!IsBusy && CanDeleteItem()));
        
        private readonly FoodService _service = FoodService.Instance;

        public MainViewModel()
        {
            CreateEmptyItem();
        }

        private async Task RefreshItems()
        {
            StatusText = "Loading items from Azure Easy Tables...";

            var items = await _service.ReadAsync();
            Items = new ObservableCollection<Food>(items);

            SelectedItem = Items.FirstOrDefault(i => i.Id == EditableItem?.Id);            
        }

        private async Task LoadItemsAsync()
        {
            if (!IsBusy)
            {
                IsBusy = true;
                await RefreshItems();
                IsBusy = false;

                StatusText = "Finished load items!.";
            }
        }

        private void CreateEmptyItem()
        {
            EditableItem = new Food ();
        }

        private async Task SaveItem()
        {
            if (!IsBusy)
            {
                IsBusy = true;
                StatusText = "Save item...";

                await _service.AddOrUpdateAsync(EditableItem);
                await RefreshItems();

                IsBusy = false;
                StatusText = "Item saved completed!.";
            }
        }

        private async Task DeleteItem()
        {
            if (!IsBusy)
            {
                IsBusy = true;
                StatusText = "Remove item...";

                await _service.DeleteAsync(EditableItem);
                EditableItem = new Food();
                await RefreshItems();

                IsBusy = false;
                StatusText = "Remove item completed!.";
            }
        }

        private bool CanDeleteItem()
        {
            return !string.IsNullOrEmpty(EditableItem?.Id);
        }
    }
}
