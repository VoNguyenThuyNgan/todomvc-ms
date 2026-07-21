using FluentValidation;

namespace Todo.Api.Features.Todos
{
    public class CreateTodoRequestValidator : AbstractValidator<CreateTodoRequest>
    {
        public CreateTodoRequestValidator() 
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .MaximumLength(200);
        }
    }

    public class UpdateTodoRequestValidator : AbstractValidator<UpdateTodoRequest>
    {
        public UpdateTodoRequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .MaximumLength(200);
        }
    }
}
