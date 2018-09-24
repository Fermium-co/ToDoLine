using Bit.Model.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ToDoLine.Model
{
    public class User : IEntity
    {
        public virtual Guid Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public virtual string UserName { get; set; }

        [Required]
        [StringLength(250, MinimumLength = 3)]
        public virtual string Password { get; set; }

        public virtual List<ToDoGroup> OwnedToDoGroups { get; set; }

        public virtual List<ToDoItem> OwnedToDoItems { get; set; }

        public virtual List<ToDoItem> CompletedToDoItems { get; set; }

        public virtual List<ToDoGroupOptions> ToDoGroupOptionsList { get; set; }

        public virtual List<ToDoItemOptions> ToDoItemOptionsList { get; set; }
    }
}
