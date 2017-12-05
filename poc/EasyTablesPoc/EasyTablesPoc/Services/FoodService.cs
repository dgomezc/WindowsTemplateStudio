using EasyTablesPoc.Models;

namespace EasyTablesPoc.Services
{
    public class FoodService : MobileBaseService<Food>
    {
        private static FoodService _instance;
        public static FoodService Instance => _instance ?? (_instance = new FoodService());

        private FoodService()
        {
        }
    }
}
