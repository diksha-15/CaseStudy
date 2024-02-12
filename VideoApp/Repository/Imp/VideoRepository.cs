using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Video_Streaming.DbEntity;
using Video_Streaming.Repository.Interface;



namespace Video_Streaming.Repository.Imp
{
    public class VideoRepository : IVideoRepository
    {
        private readonly SqlRepository _repository;

        public VideoRepository(SqlRepository sqlRepository)
        {
            _repository = sqlRepository ?? throw new ArgumentNullException("sqlRepository");
        }

        const string INSERT_QUERY = @"INSERT INTO dbo.VideoInfo
                                      (Title, Description, FilePath, FileSize)
                                      OUTPUT INSERTED.Id
                                      VALUES (@Title, @Description, @FilePath, @FileSize)";

        const string UPDATE_QUERY = @"UPDATE dbo.VideoInfo
                                        SET Title = @Title, Description = @Description, Version = Version + 1, ModifiedDate=GetDate()
                                        WHERE Id = @Id";

        const string DELETE_QUERY = @"UPDATE dbo.VideoInfo
                                        SET IsDeleted = 1,  ModifiedDate=GetDate()  WHERE Id = @Id";

        public List<VideoInformation> GetAll()
        {
            string query = "SELECT * FROM dbo.VideoInfo WHERE IsDeleted<>1";
            DataTable dataTable = _repository.ExecuteQuery(query);

            List<VideoInformation> listOfVideos = SqlRepository.MapDataTableToClass<VideoInformation>(dataTable);
            return listOfVideos;

        }

        public int Get()
        {
            string query = "SELECT COALESCE((SELECT MAX(Id) FROM dbo.VideoInfo), 0)";
            DataTable dataTable = _repository.ExecuteQuery(query);

            int maxId = Convert.ToInt32(dataTable.Rows[0][0]);
            return maxId;
        }

        public int Create(VideoInformation videoInfo)
        {
            int response = -1;
            string query = $"{INSERT_QUERY}";

            SqlCommand command = new SqlCommand();

            command.Parameters.Add(new SqlParameter("@Title", videoInfo.Title));
            command.Parameters.Add(new SqlParameter("@Description", videoInfo.Description));
            command.Parameters.Add(new SqlParameter("@FileSize", videoInfo.FileSize));
            command.Parameters.Add(new SqlParameter("@FilePath", videoInfo.FilePath));

             response = _repository.ExecuteQuery(query, command.Parameters);
            return response ;
        }

        public int Update(VideoInformation videoInfo)
        {
            int response = -1;
            string query = $"{UPDATE_QUERY}";

            SqlCommand command = new SqlCommand();

            command.Parameters.Add(new SqlParameter("@Title", videoInfo.Title));
            command.Parameters.Add(new SqlParameter("@Description", videoInfo.Description));
            command.Parameters.Add(new SqlParameter("@Id", videoInfo.Id));

            response = _repository.ExecuteQuery(query, command.Parameters);
            return response;
        }

        public int Delete(int fileId)
        {
            int response = -1;
            string query = $"{DELETE_QUERY}";

            SqlCommand command = new SqlCommand();

            command.Parameters.Add(new SqlParameter("@Id", fileId));

            response = _repository.ExecuteQuery(query, command.Parameters);
            return response ;
        }
    }
}