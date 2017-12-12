using EasyTablesPoc.Models;

namespace EasyTablesPoc.Services
{
    class TodoItemService : EasyTableService<TodoItem>
    {
        private static TodoItemService _instance;
        public static TodoItemService Instance => _instance ?? (_instance = new TodoItemService());

        private TodoItemService()
        {
        }

        protected override bool ItemsAreEquals(TodoItem serverItem, TodoItem localItem)
        {
            return base.ItemsAreEquals(serverItem, localItem)
                && serverItem.Text == localItem.Text;
        }
    }
}
