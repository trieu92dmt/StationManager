using IntegrationNS.Application.Commands.Vendors;
using Core.Interfaces.Databases;
using Infrastructure.Data;
using System.Reflection;

namespace IntegrationNS.API.Configures
{
    public static class DIAssemblies
    {
        internal static Assembly[] AssembliesToScan = new Assembly[]
        {
            Assembly.GetExecutingAssembly(),
            Assembly.GetAssembly(typeof(EntityDataContext)),
            Assembly.GetAssembly(typeof(IUnitOfWork)),
            Assembly.GetAssembly(typeof(VendorIntegrationCommand)),
            Assembly.GetAssembly(typeof(Startup)),
        };
    }
}
