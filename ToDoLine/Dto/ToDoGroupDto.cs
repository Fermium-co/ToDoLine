using Bit.Model.Contracts;
using System;
using System.ComponentModel.DataAnnotations;
using ToDoLine.Enum;

namespace ToDoLine.Dto
{
    public class ToDoGroupDto : IDto
    {
        public virtual Guid Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public virtual string Title { get; set; }

        public virtual bool IsDefault { get; set; }

        public virtual string CreatedBy { get; set; }

        public virtual DateTimeOffset CreatedOn { get; set; }

        public virtual DateTimeOffset ModifiedOn { get; set; }

        public virtual string Theme { get; set; }

        public virtual SortBy SortedBy { get; set; }

        public virtual bool HideCompletedToDoItems { get; set; }

        public virtual int SharedByCount { get; set; }
    }
}
