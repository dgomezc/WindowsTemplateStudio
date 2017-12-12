using EasyTablesPoc.Helpers;
using EasyTablesPoc.Models;

namespace EasyTablesPoc.Services
{
    class TodoItemService : EasyTableService<TodoItem>
    {
        private static TodoItemService _instance;
        public static TodoItemService Instance => _instance ?? (_instance = new TodoItemService());

        private TodoItemService()
        {
            _resolveConflictMode = ResolveConflictMode.CancelAndUpdate;
        }
    }
}
