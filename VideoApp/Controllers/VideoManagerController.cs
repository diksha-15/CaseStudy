using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Video_Streaming.Services.Interface;
using Video_Streaming.DbEntity;
using Newtonsoft.Json;
using System.Web.Http;
using HttpGetAttribute = System.Web.Http.HttpGetAttribute;
using ActionNameAttribute = System.Web.Http.ActionNameAttribute;
using HttpPostAttribute = System.Web.Http.HttpPostAttribute;
using System.Configuration;
using Serilog;
using static System.Net.WebRequestMethods;
using System.Globalization;


namespace Video_Streaming.Controllers
{
    public class VideoManagerController : ApiController
    {

        private readonly IVideoService _videoService;
        string _staticPath = "";
        public VideoManagerController(IVideoService videoService)
        {
            _videoService = videoService ?? throw new ArgumentNullException("videoService");
            _staticPath = ConfigurationManager.AppSettings["StaticFilePath"]; ;
        }

        [HttpGet()]
        [ActionName("getAllVideoFiles")]
        public async Task<IHttpActionResult> GetAllVideoFiles()
        {
            IEnumerable<VideoInformation> videoList = new List<VideoInformation>();
            try
            {
                Log.Information("Getting all video files");
                videoList = await Task.FromResult(_videoService.GetAllVideoFiles());

                return Ok(videoList);
            }
            catch (Exception ex)
            {
                Log.Error("An internal server error occurred while fetching all video files data {0}", ex);
                return InternalServerError(new Exception("An internal server error occurred while fetching all video files data. Please see logs for more details"));
            }
        }

        [HttpPost()]
        [ActionName("uploadVideoFile")]
        public async Task<IHttpActionResult> UploadVideoFile()
        {
            int result = -1;
            string savedFile = "";
            try
            {
                Log.Information("Uploading video file");

                if (!Request.Content.IsMimeMultipartContent())
                {
                    Log.Error("Unsupported Media Type while uploading file");
                    return BadRequest("Unsupported Media Type while uploading file");
                }

                var provider = new MultipartMemoryStreamProvider();
                await Request.Content.ReadAsMultipartAsync(provider);

                string formInfo = HttpContext.Current.Request.Form.Get("fileInfo");

                if (formInfo == string.Empty)
                {
                    Log.Error("Missing video file information while uploading file data");
                    return BadRequest("Missing video file information");
                }

                var fileInfo = JsonConvert.DeserializeObject<VideoInformation>(formInfo);

                if (provider.Contents.Count > 0)
                {
                    var fileName = provider.Contents[0].Headers.ContentDisposition.FileName.Trim('\"');
                    var fileSize = provider.Contents[0].Headers.ContentLength;

                    if (string.IsNullOrEmpty(fileName))
                    {
                        Log.Error("Error occurred Invalid filename found while uploading");
                        return BadRequest("Invalid file name");
                    }

                    var buffer = await provider.Contents[0].ReadAsByteArrayAsync();
                    var folderId = _videoService.GetNextId();

                     var fileLocation = Path.Combine(_staticPath, folderId.ToString());

                    if (Directory.Exists(fileLocation) == false)
                    {
                      
                        Directory.CreateDirectory(fileLocation);
                        Log.Information("File directory created successfully: {0}", fileLocation);
                    }

                     savedFile = Path.Combine(fileLocation, fileName);

                    System.IO.File.WriteAllBytes(savedFile, buffer);

                    fileInfo.FilePath = fileLocation;
                    fileInfo.FileSize = (double)fileSize / (1024 * 1024);

                    result = await Task.FromResult(_videoService.CreateVideoFile(fileInfo));
                   
                }
                if (result > 0)
                {
                    return Ok(result);
                }
                else
                {
                    System.IO.File.Delete(savedFile);
                    Log.Error("Error occurred while inserting video file data {0}", fileInfo.ToString());
                    return BadRequest("Error occurred while inserting video file data");
                }
            }
            catch (Exception ex)
            {
                Log.Error("An internal server error occurred while uploading video file data {0}", ex);
                return InternalServerError(new Exception("An internal server error occurred while uploading video file data. Please see logs for more details"));
            }
        }

        [HttpGet()]
        [ActionName("viewVideoFile")]
        public IHttpActionResult ViewVideoFile([FromUri] int fileId)
        {
            try
            {
                Log.Information("Fetching video file");

                string videoFilePath = Path.Combine(_staticPath, fileId.ToString());

                DirectoryInfo dirInfo = new DirectoryInfo(videoFilePath);

                FileInfo file = dirInfo.GetFiles("*.mp4").FirstOrDefault();

                if (file == null || !file.Exists)
                {
                    Log.Error("Error attempting retrieving video file, file doesn't exists ");
                    return BadRequest("Error attempting retrieving video file, file doesn't exists ");
                }

                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + file.Name);
                HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", "http://localhost:3000");
                HttpContext.Current.Response.ContentType = "application/octet-stream";
                HttpContext.Current.Response.WriteFile(file.FullName);
                HttpContext.Current.Response.End();
                HttpContext.Current.Response.Flush();

                return Ok();
            }
            catch (Exception ex)
            {
                Log.Error("An internal server error occurred while fetching video file to view {0}", ex);
                return InternalServerError(new Exception("An internal server error occurred while fetching video file to view data. Please see logs for more details"));
            }
        }


        [HttpPost()]
        [ActionName("editVideoFile")]
        public async Task<IHttpActionResult> EditVideoFile([FromBody] VideoInformation videodetails)
        {
            int result = -1;
            try
            {
                Log.Information("Updating video file");

                if (string.IsNullOrEmpty(videodetails.Title) || string.IsNullOrEmpty(videodetails.Description))
                {
                    Log.Error("Error occured while updating, Title or Description cannot be empty");
                    return BadRequest("Title or Description cannot be empty");

                }

                result = await Task.FromResult(_videoService.UpdateVideoFile(videodetails));

                if (result > 0)
                { return Ok(result); }

                else
                {
                    Log.Error("Error occurred while updating video file data {0}", videodetails.ToString());
                    return BadRequest("Error occurred while updating video file data");
                }
            }
            catch (Exception ex)
            {
                Log.Error("An internal server error occurred while updating video file details {0}", ex);
                return InternalServerError(new Exception("An internal server error occurred while updating video file details. Please see logs for more details"));
            }
        }

        [HttpPost()]
        [ActionName("deleteVideoFile")]
        public async Task<IHttpActionResult> DeleteVideoFile([FromUri] int fileId)
        {
            int result = -1;
            try
            {
                Log.Information("Deleting video file");

                result = await Task.FromResult(_videoService.DeleteVideoFile(fileId));

                if (result > 0)
                { return Ok(result); }

                else
                {
                    Log.Error("Error occurred while deleting video file data {0}", fileId);
                    return BadRequest("Error occurred while deleting video file data");
                }
            }
            catch (Exception ex)
            {
                Log.Error("An internal server error occurred while deleting video file details {0}", ex);
                return InternalServerError(new Exception("An internal server error occurred while deleting video file details. Please see logs for more details"));
            }
        }

        [HttpGet()]
        [ActionName("searchVideo")]
        public async Task<IHttpActionResult> SearchVideo([FromUri] string name=null)
        {
            IEnumerable<VideoInformation> videoList = new List<VideoInformation>();
            try
            {
                if(string.IsNullOrEmpty(name))
                    return BadRequest("name cannot be empty");  

                videoList = await Task.FromResult(_videoService.GetAllVideoFiles(name));

                return Ok(videoList);
            }
            catch (Exception ex)
            {
                Log.Error("An internal server error occurred while searching video file details {0}", ex);
                return InternalServerError(new Exception("An internal server error occurred while searching video file details. Please see logs for more details"));
            }
        }
    }
}