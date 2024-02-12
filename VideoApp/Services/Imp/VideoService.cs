using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Video_Streaming.DbEntity;
using Video_Streaming.Repository.Interface;
using Video_Streaming.Services.Interface;

namespace Video_Streaming.Services.Imp
{

    public class VideoService  : IVideoService
    {
        private readonly IVideoRepository _repo;

        public VideoService(IVideoRepository repo)
        {
            _repo = repo ?? throw new ArgumentNullException("video repository");
        }


        public IEnumerable<VideoInformation> GetAllVideoFiles()
        {
            IEnumerable<VideoInformation> videoList ;
            var response = _repo.GetAll();
            videoList = response.Any() ? response : new List<VideoInformation>();

            return videoList;
        }

        public IEnumerable<VideoInformation> GetAllVideoFiles(string name)
        {
            IEnumerable<VideoInformation> videoList;
            var response = _repo.GetAll();
            videoList = response.Where(x => x.Description.Contains(name) || x.Title.Contains(name)).ToList();

            return videoList ?? new List<VideoInformation>();
        }

        public int GetNextId()
        {
            var response = _repo.Get()+1;

            return response ;
        }

        public int CreateVideoFile(VideoInformation entity)
        {
            var response = _repo.Create(entity);

            return response;
        }

        public int UpdateVideoFile(VideoInformation entity)
        {
            var response = _repo.Update(entity);

            return response;
        }

        public int DeleteVideoFile(int fileId)
        {
            var response = _repo.Delete(fileId);

            return response;
        }
    }
}