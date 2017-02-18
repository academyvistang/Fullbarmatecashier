//using BarAndRestaurantMate.Helpers;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net.Http;
//using System.Web;
//using System.Web.Http;
//using System.Web.Mvc;

//namespace BarAndRestaurantMate.Controllers
//{

//public class VideosController : ApiController
//{
//   public HttpResponseMessage Get(string filename, string ext)
//   {
//      var video = new VideoStream(filename, ext);
 
//      var response = Request.CreateResponse();
//      response.Content = new PushStreamContent(video.WriteToStream, new MediaTypeHeaderValue("video/"+ext));
 
//      return response;
//   }
//}
//}