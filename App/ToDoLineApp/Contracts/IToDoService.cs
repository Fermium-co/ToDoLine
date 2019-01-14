using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ToDoLine.Dto;

namespace ToDoLineApp.Contracts
{
    public interface IToDoService
    {
        List<ToDoGroupDto> ToDoGroups { get; set; }
        List<ToDoItemDto> ToDoItems { get; set; }

        List<ToDoItemDto> MyDayToDoItems { get; }
        List<ToDoItemDto> ImportantToDoItems { get; }
        List<ToDoItemDto> PlannedToDoItems { get; }
        List<ToDoItemDto> ToDoItemsWithoutToDoGroup { get; }

        int MyDayToDoItemsCount { get; }
        int ImportantToDoItemsCount { get; }
        int PlannedToDoItemsCount { get; }
        int ToDoItemsWithoutToDoGroupCount { get; }

        bool AnyOverdueTask { get; }

        Task LoadDataAsync(CancellationToken cancellationToken);
    }
}
