using ISD.Core.Interfaces.Databases;
using ISD.Infrastructure.Data;
using MasterData.Application.Commands;
using System.Reflection;

namespace MasterData.API.Configures
{
    public static class DIAssemblies
    {
        internal static Assembly[] AssembliesToScan = new Assembly[]
        {
            Assembly.GetExecutingAssembly(),
            Assembly.GetAssembly(typeof(EntityDataContext)),
            Assembly.GetAssembly(typeof(IUnitOfWork)),
            Assembly.GetAssembly(typeof(TestCommand)),
            Assembly.GetAssembly(typeof(Program)),
        };
    }
}
