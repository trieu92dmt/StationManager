using ISD.Core.Interfaces.Databases;
using ISD.Infrastructure.Data;
using MES.Application.Commands.MES;
using System.Reflection;

namespace MES.API.Configures
{
    public static class DIAssemblies
    {
        internal static Assembly[] AssembliesToScan = new Assembly[]
        {
            Assembly.GetExecutingAssembly(),
            Assembly.GetAssembly(typeof(EntityDataContext)),
            Assembly.GetAssembly(typeof(IUnitOfWork)),
            Assembly.GetAssembly(typeof(GetNKMHCommand)),
            Assembly.GetAssembly(typeof(Program)),
        };
    }
}
