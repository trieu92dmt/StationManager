using Authentication.Application.Commands;
using Core.Interfaces.Databases;
using Infrastructure.Data;
using System.Reflection;

namespace Authentication.API.Configures
{
    public static class DIAssemblies
    {
        internal static Assembly[] AssembliesToScan = new Assembly[]
        {
            Assembly.GetExecutingAssembly(),
            Assembly.GetAssembly(typeof(EntityDataContext)),
            Assembly.GetAssembly(typeof(IUnitOfWork)),
            Assembly.GetAssembly(typeof(LoginCommand)),
            Assembly.GetAssembly(typeof(Program)),
        };
    }
}
