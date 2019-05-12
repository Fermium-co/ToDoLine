using Bit.Data.Contracts;
using Bit.Model.Contracts;
using Bit.OData.ODataControllers;
using Bit.Owin.Exceptions;
using Newtonsoft.Json.Converters;
using Swashbuckle.Examples;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using ToDoLine.Dto;
using ToDoLine.Model;

namespace ToDoLine.Controller
{
    public class ToDoItemStepsController : DtoController<ToDoItemStepDto>
    {
        public virtual IRepository<ToDoItemStep> ToDoItemStepsRepository { get; set; }

        public virtual IDtoEntityMapper<ToDoItemStepDto, ToDoItemStep> ToDoItemStepMapper { get; set; }


        [Function]
        public virtual IQueryable<ToDoItemStepDto> GetToDoItemSteps(Guid toDoItemId)
        {
            return ToDoItemStepMapper.FromEntityQueryToDtoQuery(ToDoItemStepsRepository.GetAll().Where(tdis => tdis.ToDoItemId == toDoItemId));
        }

        public class ToDoItemStepDtoCreateExamplesProvider : IExamplesProvider
        {
            public object GetExamples()
            {
                return new { Text = "Title", IsCompleted = false, ToDoItemId = Guid.Empty };
            }
        }

        [Create]
        [SwaggerRequestExample(typeof(ToDoItemStepDto), typeof(ToDoItemStepDtoCreateExamplesProvider), jsonConverter: typeof(StringEnumConverter))]
        public virtual async Task<SingleResult<ToDoItemStepDto>> CreateToDoItemSteps(ToDoItemStepDto toDoItemStep, CancellationToken cancellationToken)
        {
            ToDoItemStep addedToDoItemStep = await ToDoItemStepsRepository.AddAsync(ToDoItemStepMapper.FromDtoToEntity(toDoItemStep), cancellationToken);

            return SingleResult(ToDoItemStepMapper.FromEntityToDto(addedToDoItemStep));
        }


        public class ToDoItemStepDtoUpdateExamplesProvider : IExamplesProvider
        {
            public object GetExamples()
            {
                return new { Text = "Title", IsCompleted = false };
            }
        }

        [PartialUpdate]
        [SwaggerRequestExample(typeof(ToDoItemStepDto), typeof(ToDoItemStepDtoUpdateExamplesProvider), jsonConverter: typeof(StringEnumConverter))]
        public virtual async Task<SingleResult<ToDoItemStepDto>> UpdateToDoItemSteps(Guid key, ToDoItemStepDto toDoItemStep, CancellationToken cancellationToken)
        {
            ToDoItemStep updatedToDoItemSteps = await ToDoItemStepsRepository.GetByIdAsync(cancellationToken, key);

            if (updatedToDoItemSteps == null)
                throw new BadRequestException("ToDoItemStepMayBeNull");

            updatedToDoItemSteps.Text = toDoItemStep.Text;
            updatedToDoItemSteps.IsCompleted = toDoItemStep.IsCompleted;

            await ToDoItemStepsRepository.UpdateAsync(updatedToDoItemSteps, cancellationToken);

            return SingleResult(ToDoItemStepMapper.FromEntityToDto(updatedToDoItemSteps));
        }

        [Delete]
        public virtual async Task DeleteToDoItemSteps(Guid key, CancellationToken cancellationToken)
        {
            ToDoItemStep toDoItemStepToBeDeleted = await ToDoItemStepsRepository.GetByIdAsync(cancellationToken, key);

            if (toDoItemStepToBeDeleted == null)
                throw new BadRequestException("ToDoItemStepCountNotBeFound");

            await ToDoItemStepsRepository.DeleteAsync(toDoItemStepToBeDeleted, cancellationToken);
        }
    }
}
