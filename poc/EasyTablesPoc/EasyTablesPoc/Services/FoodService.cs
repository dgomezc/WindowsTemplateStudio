using EasyTablesPoc.Models;

namespace EasyTablesPoc.Services
{
    public class FoodService : EasyTableService<Food>
    {
        private static FoodService _instance;
        public static FoodService Instance => _instance ?? (_instance = new FoodService());

        private FoodService()
        {
        }

        protected override bool ItemsAreEquals(Food serverItem, Food localItem)
        {
            return base.ItemsAreEquals(serverItem, localItem)
                && serverItem.Name == localItem.Name;
        }
    }
}
