using IntegrationNS.Application.Commands;
using ISD.API.Repositories.Infrastructure.Database;
using ISD.Infrastructure.Data;
using System.Reflection;

namespace IntegrationNS.API.Configures
{
    public static class DIAssemblies
    {
        internal static Assembly[] AssembliesToScan = new Assembly[]
        {
            Assembly.GetExecutingAssembly(),
            Assembly.GetAssembly(typeof(EntityDataContext)),
            Assembly.GetAssembly(typeof(ISDUnitOfWork)),
            Assembly.GetAssembly(typeof(VendorIntegrationCommand)),
            Assembly.GetAssembly(typeof(Startup)),
        };
    }
}
