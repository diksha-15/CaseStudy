using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Video_Streaming.DbEntity;

namespace Video_Streaming.Services.Interface
{
    public interface IVideoService
    { 
        IEnumerable<VideoInformation> GetAllVideoFiles();
        IEnumerable<VideoInformation> GetAllVideoFiles(string name);
        int GetNextId();
        int CreateVideoFile(VideoInformation entity);
        int UpdateVideoFile(VideoInformation entity);
        int DeleteVideoFile(int fileId);

    }
}
