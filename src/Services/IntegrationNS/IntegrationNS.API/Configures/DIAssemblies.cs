using IntegrationNS.Application.Commands;
using ISD.Core.Interfaces.Databases;
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
            Assembly.GetAssembly(typeof(IUnitOfWork)),
            Assembly.GetAssembly(typeof(VendorIntegrationCommand)),
            Assembly.GetAssembly(typeof(Startup)),
        };
    }
}
