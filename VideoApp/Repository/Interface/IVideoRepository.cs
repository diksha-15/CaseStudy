using System;
using System.Collections.Generic;
using Video_Streaming.DbEntity;



namespace Video_Streaming.Repository.Interface
{
    public interface IVideoRepository
    {
        List<VideoInformation> GetAll();
        int Get();
        int Create(VideoInformation videoInfo);
        int Update(VideoInformation videoInfo);
        int Delete(int fileId);
    }
}
