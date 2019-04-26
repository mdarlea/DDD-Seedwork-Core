using MediatR;

namespace Swaksoft.Application.Seedwork
{
	public interface IQuery<out TResponse> : IRequest<TResponse>
	{
	}
}
