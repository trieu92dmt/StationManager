﻿using ISD.API.Repositories.Infrastructure.Database;
using ISD.Infrastructure.Data;
using MES.Application.Commands;
using System.Reflection;

namespace MES.API.Configures
{
    public static class DIAssemblies
    {
        internal static Assembly[] AssembliesToScan = new Assembly[]
        {
            Assembly.GetExecutingAssembly(),
            Assembly.GetAssembly(typeof(EntityDataContext)),
            Assembly.GetAssembly(typeof(ISDUnitOfWork)),
            Assembly.GetAssembly(typeof(TestCommand)),
            Assembly.GetAssembly(typeof(Program)),
        };
    }
}
