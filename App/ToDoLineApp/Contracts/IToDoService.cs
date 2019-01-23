using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using ToDoLine.Dto;

namespace ToDoLineApp.Contracts
{
    public interface IToDoService
    {
        ObservableCollection<ToDoGroupDto> ToDoGroups { get; set; }
        ObservableCollection<ToDoItemDto> ToDoItems { get; set; }

        List<ToDoItemDto> MyDayToDoItems { get; }
        List<ToDoItemDto> ImportantToDoItems { get; }
        List<ToDoItemDto> PlannedToDoItems { get; }
        List<ToDoItemDto> ToDoItemsWithoutToDoGroup { get; }

        UserDto User { get; set; }

        int MyDayToDoItemsCount { get; }
        int ImportantToDoItemsCount { get; }
        int PlannedToDoItemsCount { get; }
        int ToDoItemsWithoutToDoGroupCount { get; }

        bool AnyOverdueTask { get; }

        Task LoadData(CancellationToken cancellationToken);

        Task<ToDoGroupDto> AddNewGroup(string newGroupTitle, CancellationToken cancellationToken);
        Task DeleteGroup(ToDoGroupDto group, CancellationToken cancellationToken);
        Task UpdateGroup(ToDoGroupDto group, CancellationToken cancellationToken);
    }
}
