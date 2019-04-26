using MediatR;

namespace Swaksoft.Application.Seedwork
{
	public interface ICommand<out TResponse> : IRequest<TResponse>
	{
	}
}
