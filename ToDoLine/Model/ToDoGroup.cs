using Bit.Model.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ToDoLine.Enum;

namespace ToDoLine.Model
{
    public class ToDoGroup : IEntity
    {
        public virtual Guid Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public virtual string Title { get; set; }

        public virtual bool IsDefault { get; set; }

        public virtual Guid CreatedById { get; set; }

        public virtual User CreatedBy { get; set; }

        public virtual DateTimeOffset CreatedOn { get; set; }

        public virtual DateTimeOffset ModifiedOn { get; set; }

        public virtual List<ToDoGroupOptions> Options { get; set; }

        public virtual List<ToDoItem> Items { get; set; }
    }

    public class ToDoGroupOptions : IEntity
    {
        public virtual Guid Id { get; set; }

        public virtual string Theme { get; set; }

        public virtual SortBy SortedBy { get; set; }

        public virtual bool HideCompletedToDoItems { get; set; }

        public virtual Guid UserId { get; set; }

        public virtual User User { get; set; }

        public virtual Guid ToDoGroupId { get; set; }

        public virtual ToDoGroup ToDoGroup { get; set; }
    }
}
