using System;
using Microsoft.Extensions.Logging;
using Swaksoft.Core.Dto;

namespace Swaksoft.Application.Seedwork.Services
{
    public abstract class AppServiceBase<T> : IDisposable
    {
		private readonly ILogger<T> logger;		

		protected AppServiceBase(ILogger<T> logger) {
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}
		

        protected TResult Failed<TResult>(string errorMessage, params object[] args)
           where TResult : ActionResult, new()
        {
            return ErrorResult<TResult>(ActionResultCode.Failed, errorMessage, args);
        }

        protected TResult Error<TResult>(string errorMessage, params object[] args)
           where TResult : ActionResult, new()
        {
            return ErrorResult<TResult>(ActionResultCode.Errored, errorMessage, args);
        }

        protected TResult ErrorResult<TResult>(ActionResultCode resultCode, string errorMessage, params object[] args)
            where TResult : ActionResult, new()
        {
            var actionResult = new TResult
            {
                Status = resultCode,
                Message = string.Format(errorMessage, args)
            };
            logger.LogError(actionResult.Message);
            return actionResult;
        }


        #region dispose
        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
        }
        #endregion dispose
    }
}
