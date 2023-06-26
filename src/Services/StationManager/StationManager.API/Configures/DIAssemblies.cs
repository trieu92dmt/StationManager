using Core.Interfaces.Databases;
using Infrastructure.Data;
using StationManager.Application.Queries.Common;
using System.Reflection;

namespace StationManager.API.Configures
{
    public static class DIAssemblies
    {
        internal static Assembly[] AssembliesToScan = new Assembly[]
        {
            Assembly.GetExecutingAssembly(),
            Assembly.GetAssembly(typeof(EntityDataContext)),
            Assembly.GetAssembly(typeof(IUnitOfWork)),
            Assembly.GetAssembly(typeof(ICommonQuery)),
            Assembly.GetAssembly(typeof(Program)),
        };
    }
}
