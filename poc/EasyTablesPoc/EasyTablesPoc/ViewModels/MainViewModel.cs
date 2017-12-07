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
        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                Set(ref _isBusy, value);
                AddItemCommand.OnCanExecuteChanged();
                DeleteItemCommand.OnCanExecuteChanged();
                LoadItemsCommand.OnCanExecuteChanged();
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
                DeleteItemCommand.OnCanExecuteChanged();
            }
        }

        private RelayCommand _loadItemsCommand;
        public RelayCommand LoadItemsCommand => _loadItemsCommand ?? (_loadItemsCommand = new RelayCommand(async () => await LoadItems(), () => !IsBusy));

        private RelayCommand _addItemCommand;
        public RelayCommand AddItemCommand => _addItemCommand ?? (_addItemCommand = new RelayCommand(async () => await AddItem(), () => !IsBusy));

        private RelayCommand _deleteItemCommand;
        public RelayCommand DeleteItemCommand => _deleteItemCommand ?? (_deleteItemCommand = new RelayCommand(async () => await DeleteItem(SelectedItem), () => SelectedItem != null && !IsBusy));
        
        private readonly FoodService _service = FoodService.Instance;

        public MainViewModel()
        {
        }

        private async Task LoadItems()
        {
            if (!IsBusy)
            {
                IsBusy = true;

                var items = await _service.ReadAsync();
                Items = new ObservableCollection<Food>(items);

                IsBusy = false;
            }
        }

        private async Task AddItem()
        {
            if (!IsBusy)
            {
                IsBusy = true;

                await _service.AddOrUpdateAsync(new Food { Name = "Example Name", Category = "Example Category" });

                IsBusy = false;

                await LoadItems();
            }
        }

        private async Task DeleteItem(Food item)
        {
            if (!IsBusy)
            {
                IsBusy = true;

                await _service.DeleteAsync(item);

                IsBusy = false;

                await LoadItems();
            }
        }
    }
}
