using FluentValidation;

namespace Todo.Api.Features.Todos.UpdateTodo;

public class UpdateTodoCommandValidator
    : AbstractValidator<UpdateTodoCommand>
{
    public UpdateTodoCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);
    }
}