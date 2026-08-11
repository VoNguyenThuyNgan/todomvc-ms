using FluentValidation;

namespace Todo.Api.Features.Todos.CreateTodo;

public class CreateTodoCommandValidator
    : AbstractValidator<CreateTodoCommand>
{
    public CreateTodoCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);
    }
}