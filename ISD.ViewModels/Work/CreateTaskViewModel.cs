using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels.Work
{
   public class CreateTaskViewModel
    {
        public HttpStatusCode Code;
        public bool Success;
        public List<string> LData;
        public string Data;
        public string redirectUrl;
        public Guid? Id;
    }
}
