using HotelMateWeb.Dal.DataCore;
using HotelMateWeb.Services.Core;
using HotelMateWeb.Services.ServiceApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BarAndRestaurantMate.Controllers
{
    [HandleError(View = "CustomErrorView")]//[SessionExpireFilter()][LocsAuthorize()]
    public class FileUploadGalleryController : Controller
    {

        public FileUploadGalleryController()
        {
           
        }

        [HttpGet]
        public ActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            ISchoolPictureService schoolPictureService = new SchoolPictureService();
           
            // TODO: Check for whether file is required?
            if (Request.Files.Count > 0)
            {
                for (var i = 0; i < Request.Files.Count; i++)
                {
                    var letterAttachment = Request.Files[i];

                    if (letterAttachment != null && letterAttachment.ContentLength > 0)
                    {
                        var sp = new SchoolPicture();
                        var binaryFileData = new byte[letterAttachment.ContentLength];
                        letterAttachment.InputStream.Read(binaryFileData, 0, letterAttachment.ContentLength);
                        var contentType = letterAttachment.ContentType;
                        var filename = letterAttachment.FileName;
                        sp.Filename = filename;
                        //sp.Binary = binaryFileData;
                        sp.ContentType = contentType;
                        sp.CreatedBy = "GUEST";
                        sp.CreatedDate = DateTime.Now;
                        var newPicture = schoolPictureService.Create(sp);
                        var fileName = newPicture.SchoolPicturesId.ToString() + "_" + Path.GetFileName(letterAttachment.FileName);
                        var path = Path.Combine(Server.MapPath("~/Products/Gallery"), fileName);
                        file.SaveAs(path);
                        var existingSP = schoolPictureService.GetById(newPicture.SchoolPicturesId);
                        existingSP.Filename = fileName;
                        schoolPictureService.Update(existingSP);
                    }
                }
            }

            return RedirectToAction("GalleryIndex","Guest");
        }
    }
}