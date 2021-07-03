using System;
using System.Linq;
using FluentValidation;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.DependencyInjection;

namespace Hive.Shared.Common
{
    public sealed class ResponseValidationInterceptor : Interceptor
    {
        /// <summary>
        /// Name of an empty request in gRPC, by convention.
        /// </summary>
        private const string EmptyRequestTypeName = "EmptyRequest";

        private readonly IServiceProvider _provider;

        public ResponseValidationInterceptor(IServiceProvider provider)
        {
            _provider = provider;
        }

        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            var response = base.AsyncUnaryCall(request, context, continuation);
            // Todo: #16570 Check whether response awaiting is needed in Hive common response validation
            ValidateRequestAsync(response.GetAwaiter().GetResult());
            return response;
        }

        private void ValidateRequestAsync<TResponse>(TResponse response)
        {
            // Don't bother to validate an empty request, because there is nothing to validate.
            if (typeof(TResponse).Name == EmptyRequestTypeName)
            {
                return;
            }

            var validator = _provider.GetService<IValidator<TResponse>>();
            if (validator == null)
            {
                throw new RpcException(new Status(StatusCode.FailedPrecondition, $"No validator was found for {typeof(TResponse).Name}"));
            }

            var validationResult = validator.Validate(response);
            if (!validationResult.IsValid)
            {
                var details = string.Join(",", validationResult.Errors.Select(x => x.ErrorMessage));

                throw new RpcException(new Status(StatusCode.InvalidArgument, details));
            }
        }
    }
}