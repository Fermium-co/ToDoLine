using Bit.Model.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ToDoLine.Dto
{
    public class ToDoItemDto : IDto
    {
        public virtual Guid Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public virtual string Title { get; set; }

        public virtual bool IsImportant { get; set; }

        public virtual bool IsCompleted { get; set; }

        public virtual string Notes { get; set; }

        public virtual DateTimeOffset? DueDate { get; set; }

        public virtual string CreatedBy { get; set; }

        public virtual string CompletedBy { get; set; }

        public virtual DateTimeOffset CreatedOn { get; set; }

        public virtual DateTimeOffset ModifiedOn { get; set; }

        public virtual List<ToDoItemStepDto> Steps { get; set; }

        public virtual DateTimeOffset? RemindOn { get; set; }

        public virtual bool ShowInMyDay { get; set; }

        public virtual Guid ToDoGroupId { get; set; }
    }

    public class ToDoItemStepDto : IDto
    {
        public virtual Guid Id { get; set; }

        public virtual string Text { get; set; }

        public virtual Guid ToDoItemId { get; set; }

        public virtual ToDoItemDto ToDoItem { get; set; }

        public virtual bool IsCompleted { get; set; }
    }
}
