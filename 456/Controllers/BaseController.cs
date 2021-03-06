﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using bst.Model;
using Microsoft.AspNetCore.Mvc.Filters;

namespace bst.Controllers
{
    public class BaseController : Controller
    {
        public UserDB context;
        public User user;
        public Session session;
        public Protocol protocol;
        public History history;
        /// <summary>
        /// this happens before any controller call
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            //set the dbcontext
            this.context = new UserDB();
            HttpContext.Items["context"] = this;
            base.OnActionExecuting(context);
        }

    }
}