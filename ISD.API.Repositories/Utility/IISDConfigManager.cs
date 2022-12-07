using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.API.Repositories
{
    public interface IISDConfigManager
    {
        string GetConnectionString(string connectionName);
        string Username { get; }

        string Password { get; }
    }
}
