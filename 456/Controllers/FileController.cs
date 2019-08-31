﻿using System;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using bst.Model;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using bst.Logic;

namespace bst.Controllers
{
    [Route("file")]
    [ApiController, AuthFilter]
    public class FileController : BaseController
    {

        private IHostingEnvironment env;

        public FileController(IHostingEnvironment env)
        {
            this.env = env;
        }

        [HttpPost("upload/channel")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadChannel()//[FromBody]UploadChannelIn input)
        {
            if (HttpContext.Request.Form.Files.Count < 1) return BadRequest("You must upload a file.");
            var file = HttpContext.Request.Form.Files[0];
            /*
            var study = await context.Studies.FindAsync(input.FileInfo.StudyId);
            if (study == null) return NotFound("Study doesn't exist.");
            var user = (User)HttpContext.Items["user"];
            var participation = user.Protocols.FirstOrDefault(x => x.Protocol.Id.Equals(study.Protocol.Id));
            if (participation == null) return BadRequest("You don't have access to this study.");
            if (participation.Privilege > 2) return BadRequest("You don't have write access to this protocol.");
            */

            //var filePath = Path.Combine(env.ContentRootPath, "db", study.Protocol.Name, "data", study.Name, input.FileInfo.FileName);
            var rootlocation = "D:\\";
            var filePath = Path.Combine(rootlocation, "channelfiletest");
            // If file already exists, do nothing
            if (System.IO.File.Exists(filePath))
                return BadRequest("You can't upload the same file twice.");

            Directory.CreateDirectory(filePath);
            /*
            //update database
            FunctionalFile functionalFile = ConfigureData.ToFunctionalFile(input.FileInfo, FunctionalFileType.Channel, study);
            context.FunctionalFiles.Add(functionalFile);
            Channel channel = ConfigureData.ToChannel(input.Metadata, functionalFile);
            context.Channels.Add(channel);
            await context.SaveChangesAsync();
            */

            // Create a new file in the home-bst directory 
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                //copy the contents of the received file to the newly created local file 
                await file.CopyToAsync(stream);
            }
            // return the file name for the locally stored file
            return Ok(filePath);

        }







        // GET file/upload
        /// <summary>
        /// Receive form data containing a file, save file on server, and return the file path
        /// </summary>
        /// <param name="file">Received IFormFile file</param>
        /// <param name="fileTransferIn"></param>
        /// <returns></returns>
        /*
        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Upload()
        {
            if (HttpContext.Request.Form.Files.Count < 1)
            {
                return BadRequest();
            }

            var file = HttpContext.Request.Form.Files[0];

            var user = await context.Users.FindAsync(HttpContext.Items["user"]);
            var protocol = await context.Protocols.FindAsync(fileTransferIn.Protocolid);
            if (protocol == null) return new NotFoundResult();
            var role = user.Roles.FirstOrDefault(r => r.Group.Id.Equals(protocol.Group.Id));
            if (role == null) return new NotFoundResult();
            if (role.Privilege > 2) throw new UnauthorizedAccessException("User doesn't have write access to this protocol.");

            // Verify the home-bst directory exists, and combine the home-bst directory with the new file name
            Directory.CreateDirectory(bstHomePath);

            var filePath = Path.Combine(bstHomePath, protocol.Group.Name, protocol.Name, fileTransferIn.Filelocation);



            // If exists old version, delete
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            // Create a new file in the home-bst directory 
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                //copy the contents of the received file to the newly created local file 
                await file.CopyToAsync(stream);
            }
            // return the file name for the locally stored file
            return Ok(filePath);
        }
        */

        /*
        // GET file/downlaod
        /// <summary>
        /// Return a file stored on server based on file info provided
        /// </summary>
        /// <param name="fileTransferIn"></param>
        /// <returns></returns>
        [HttpGet("download")]
        public async Task<IActionResult> Download([FromBody] FileTransferIn fileTransferIn)
        {
            var user = await context.Users.FindAsync(HttpContext.Items["user"]);
            var protocol = await context.Protocols.FindAsync(fileTransferIn.Protocolid);
            if (protocol == null) return new NotFoundResult();
            var role = user.Roles.FirstOrDefault(r => r.Group.Id.Equals(protocol.Group.Id));
            if (role == null) return new NotFoundResult();
            if (role.Privilege > 3) throw new UnauthorizedAccessException("User doesn't have access to this protocol.");
            var path = Path.Combine(bstHomePath, protocol.Group.Name, protocol.Name, fileTransferIn.Filelocation);

            if (System.IO.File.Exists(path))
            {

                // Get all bytes of the file and return the file with the specified file contents 
                byte[] b = await System.IO.File.ReadAllBytesAsync(path);
                return File(b, "application/octet-stream");
            }

            else
            {
                // return error if file not found
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

        }*/
    }
}
