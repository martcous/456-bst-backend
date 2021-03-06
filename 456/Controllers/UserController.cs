﻿#pragma warning disable CS1701 // Assuming assembly reference matches identity

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using bst.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace bst.Controllers
{
    [Route("user")]
    public class UserController : BaseController
    {
        
        [HttpGet,Route("")]
        public object Index()
        {
            return "success";
        }


        [HttpPost,Route("login")]
        [ProducesResponseType(typeof(LoginOut),200)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<object> Login([FromBody]LoginIn data)
        {
            var user = await context.Users.Where(x => x.Email == data.Email && x.Password == data.Password).FirstOrDefaultAsync();
            if (user==null)
            {
                HttpContext.Response.StatusCode = 401;
                return "login failed";
            }
            var session = AuthFilter.AddSession(user.Id, data.Deviceid,user.Email);
            await context.SaveChangesAsync();
            return new LoginOut
            {
                Sessionid = session
            };
        }

        [HttpPost,Route("createuser")]
        [ProducesResponseType(typeof(CreateUserOut),200)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<object> CreateUser([FromBody]CreateUserIn user)
        {
            if (!ModelState.IsValid||user==null)
            {
                if (user==null)
                {
                    return BadRequest("received no package,, recheck frontend");
                }
                else
                {
                    return BadRequest(ModelState);
                }
            }

            var u = new User
            {
                Id = Guid.NewGuid(),
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Password = user.Password
            };
            
            context.Users.Add(u);
            await context.SaveChangesAsync();
            var sessionid = AuthFilter.AddSession(u.Id, user.Deviceid,user.Email);

            return new CreateUserOut
            {
                Sessionid = sessionid,
                Firstname = u.FirstName,
                Lastname = u.LastName,
                Email = u.Email
            };
        }

        [ProducesResponseType(typeof(bool),200)]
        [HttpPost, Route("checksession")]
        public object CheckSession([FromBody]CheckSessionIn input)
        {
            var session = AuthFilter.sessions
                .FirstOrDefault(s => s.Deviceid.Equals(input.Deviceid) && s.Sessionid.Equals(input.Sessionid));
            if (session == null||(session.LastActive.AddMinutes(AuthFilter.EXPIRETIME))<System.DateTime.Now) return Ok(false);
            else return Ok(true);
        }

        [ProducesResponseType(200),AuthFilter]
        [ProducesResponseType(StatusCodes.Status400BadRequest), ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPost, Route("logout")]       
        public object Logout()
        {
            AuthFilter.sessions.Remove(session);
            return Ok();
        }

        [HttpPost, Route("listprotocols"), ProducesResponseType(typeof(IEnumerable<ProtocolData>), 200),AuthFilter]
        public IEnumerable<ProtocolData> ListProjects([FromBody]ListCount data)
        {
            var groups = user.GroupUsers.Select(x => x.Group).ToList();
            var protocols = new List<ProtocolData>();
            foreach(var group in groups)
            {
                protocols.AddRange(group.GroupProtocols.Select(p => new ProtocolData(p.Protocol, p.GroupPrivilege+1)));
            }
            protocols.AddRange(user.ProtocolUsers.Select(x => new ProtocolData(x.Protocol, x.Privilege)).ToList());
            var groupbyID = protocols.GroupBy(p => p.Id).ToList();
            var result = new List<ProtocolData>();
            foreach (var samegroup in groupbyID)
            {
                var highestpermission = samegroup.OrderByDescending(x => x.Privilege).Take(1);
                result.AddRange(highestpermission);
            }
            return result.Skip(data.Start).Take(data.Count);
           
        }

        [HttpPost, Route("listgroups"), ProducesResponseType(typeof(IEnumerable<GroupPreview>), 200),AuthFilter]
        public async Task<object> ListGroup([FromBody]ListCount data)
        {
            
            var result = user.GroupUsers.Select(r => new GroupPreview(r.Group));
            if (data.Order == 0)
                result = result.OrderBy(r => r.Name);
            else if (data.Order == 1)
                result = result.OrderByDescending(r => r.Name);
            result = result.Skip(data.Start).Take(data.Count);
            return result;
        }

        
    }
}
