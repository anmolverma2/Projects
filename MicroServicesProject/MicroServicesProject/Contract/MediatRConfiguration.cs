using MediatR;
using MicroServicesProject.Commands;
using Microsoft.Extensions.DependencyInjection;
namespace MicroServicesProject.Contract
{
    public static class MediatRConfiguration
    {
        public static void AddMediatorServices(this IServiceCollection services)
        {
            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssemblyContaining<GetAllDetailsQuery>();
                config.RegisterServicesFromAssemblyContaining<GetStudentsQuery>();
            });
        }
    }
}
