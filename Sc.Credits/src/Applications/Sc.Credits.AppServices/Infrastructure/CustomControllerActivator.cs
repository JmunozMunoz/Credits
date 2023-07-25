using credinet.exception.middleware.enums;
using credinet.exception.middleware.models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace Sc.Credits.Applications.AppServices.Infrastructure
{
    /// <summary>
    /// Custom controller activator is an implementation of <see cref="IControllerActivator"/>
    /// </summary>
    public class CustomControllerActivator : IControllerActivator
    {
        private readonly IServiceCollection _services;
        private readonly Type[] _types;

        /// <summary>
        /// Creates a new instance of <see cref="CustomControllerActivator"/>
        /// </summary>
        /// <param name="services"></param>
        /// <param name="types"></param>
        public CustomControllerActivator(IServiceCollection services, params Type[] types)
        {
            _services = services;
            _types = types;
        }

        /// <summary>
        /// Create
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public object Create(ControllerContext context)
        {
            Type type = context.ActionDescriptor.ControllerTypeInfo.AsType();

            if (!_types.Any(item => type == item))
            {
                throw new BusinessException("Controller access disabled", (int)BusinessResponse.NotControlledException);
            }

            return _services.BuildServiceProvider().GetService(type);
        }

        /// <summary>
        /// Release
        /// </summary>
        /// <param name="context"></param>
        /// <param name="controller"></param>
        public void Release(ControllerContext context, object controller)
        {
            if (context == null)
            {
                throw new BusinessException("Controller invalid", (int)BusinessResponse.NotControlledException);
            }
        }
    }
}