using MediatR;
using SharedKernel.Common;

namespace Catalog.Application.Features.Commands.DeleteCategory;

public record DeleteCategoryCommand(Guid Id) : IRequest<Result>;