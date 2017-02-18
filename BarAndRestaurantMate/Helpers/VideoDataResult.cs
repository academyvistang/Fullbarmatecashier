using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace BarAndRestaurantMate.Helpers
{
    public class VideoDataResult : ActionResult
    {
        private int? _Id = null;

        public VideoDataResult(int? id)
        {
            _Id = id;
        }
        /// <summary>
        /// The below method will respond with the Video file
        /// </summary>
        /// <param name="context"></param>
        public override void ExecuteResult(ControllerContext context)
        {
            var tt = context.RouteData.Values;

            var movieNamePath = @"~/videos/Movies/" + _Id.ToString() + ".mp4";
            var moviePathExtention = @"attachment; filename=" + _Id.ToString() + ".mp4";

            //var strVideoFilePath = HostingEnvironment.MapPath("~/Videos/Movies/10.mp4");
            var strVideoFilePath = HostingEnvironment.MapPath(movieNamePath);


            //context.HttpContext.Response.AddHeader("Content-Disposition", "attachment; filename=10.mp4");
            context.HttpContext.Response.AddHeader("Content-Disposition", moviePathExtention);


            var objFile = new FileInfo(strVideoFilePath);

            var stream = objFile.OpenRead();
            var objBytes = new byte[stream.Length];
            stream.Read(objBytes, 0, (int)objFile.Length);
            context.HttpContext.Response.BinaryWrite(objBytes);

        }
    }
}