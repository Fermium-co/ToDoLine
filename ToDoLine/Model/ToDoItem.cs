using Bit.Model.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ToDoLine.Model
{
    public class ToDoItem : IEntity
    {
        public virtual Guid Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public virtual string Title { get; set; }

        public virtual bool IsImportant { get; set; }

        public virtual bool IsCompleted { get; set; }

        public virtual string Notes { get; set; }

        public virtual DateTimeOffset? DueDate { get; set; }

        public virtual Guid? CompletedById { get; set; }

        public virtual User CompletedBy { get; set; }

        public virtual Guid CreatedById { get; set; }

        public virtual User CreatedBy { get; set; }

        public virtual DateTimeOffset CreatedOn { get; set; }

        public virtual DateTimeOffset ModifiedOn { get; set; }

        public virtual List<ToDoItemOptions> Options { get; set; }

        public virtual List<ToDoItemStep> Steps { get; set; }

        public ToDoGroup ToDoGroup { get; set; }

        public Guid ToDoGroupId { get; set; }
    }

    public class ToDoItemOptions : IEntity
    {
        public virtual Guid Id { get; set; }

        public virtual DateTimeOffset? RemindOn { get; set; }

        public virtual DateTimeOffset? ShowInMyDayOn { get; set; }

        public virtual Guid UserId { get; set; }

        public virtual User User { get; set; }

        public virtual Guid ToDoItemId { get; set; }

        public virtual ToDoItem ToDoItem { get; set; }
    }

    public class ToDoItemStep : IEntity
    {
        public virtual Guid Id { get; set; }

        public virtual string Text { get; set; }

        public virtual Guid ToDoItemId { get; set; }

        public virtual ToDoItem ToDoItem { get; set; }

        public virtual bool IsCompleted { get; set; }
    }
}
