using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.API.Constant
{
    public interface IConfigConstantManager
    {
        string DomainUrl { get; }
        string APIDomainUrl { get; }
    }
}
