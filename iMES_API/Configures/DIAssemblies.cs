using ISD.API.Applications.Commands.IntegrationNS;
using ISD.API.EntityModels.Data;
using ISD.API.Repositories.Infrastructure.Database;
using MP_CRM_API;
using System.Reflection;

namespace ITP_MES_API.Configures
{
    public static class DIAssemblies
    {
        internal static Assembly[] AssembliesToScan = new Assembly[]
        {
            Assembly.GetExecutingAssembly(),
            Assembly.GetAssembly(typeof(EntityDataContext)),
            Assembly.GetAssembly(typeof(ISDUnitOfWork)),
            Assembly.GetAssembly(typeof(OrderTypeIntegrationCommand)),
            Assembly.GetAssembly(typeof(Startup)),
        };
    }
}
