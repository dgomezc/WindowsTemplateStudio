using EasyTablesPoc.Helpers;
using EasyTablesPoc.Models;
using EasyTablesPoc.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EasyTablesPoc.ViewModels
{
    public class MainViewModel : Observable
    {        
        public ObservableCollection<Food> Items = new ObservableCollection<Food>();
        private readonly FoodService _service = FoodService.Instance;

        public MainViewModel()
        {
        }
        
        private ICommand _populateListCommand;

        public ICommand PopulateListCommand => _populateListCommand ?? (_populateListCommand = new RelayCommand(async () => await PopulateList()));

        private async Task PopulateList()
        {
            Items.Clear();
            
            foreach(var i in await _service.ReadAsync())
            {
                Items.Add(i);
            }
        }
    }
}
