using ISD.API.EntityModels.Data;
using ISD.API.Repositories;
using ISD.API.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;

namespace ISD.API.Core
{
    public class ControllerBaseAPI : ControllerBase
    {
        //Entity
        public EntityDataContext _context;
        private static IHttpContextAccessor httpContextAccessor;

        protected ControllerBaseAPI()
        {
            _context = new EntityDataContext();
        }
        public static void SetHttpContextAccessor(IHttpContextAccessor accessor)
        {
            httpContextAccessor = accessor;
        }

        public AppUserPrincipal CurrentUser
        {
            get
            {
                return new AppUserPrincipal(this.User as ClaimsPrincipal);
            }
        }
    }
}
