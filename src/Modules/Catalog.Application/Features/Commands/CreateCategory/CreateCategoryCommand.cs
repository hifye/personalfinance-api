using MediatR;
using SharedKernel.Common;

namespace Catalog.Application.Features.Commands.CreateCategory;

public record CreateCategoryCommand(
    string Name,
    string Type) : IRequest<Result<Guid>>;
