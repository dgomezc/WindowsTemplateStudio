using EasyTablesPoc.Helpers;
using EasyTablesPoc.Models;

namespace EasyTablesPoc.Services
{
    public class FoodService : EasyTableService<Food>
    {
        private static FoodService _instance;
        public static FoodService Instance => _instance ?? (_instance = new FoodService());

        private FoodService()
        {
            _resolveConflictMode = ResolveConflictMode.UpdateOperation;
        }
    }
}
