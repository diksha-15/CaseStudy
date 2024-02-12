using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Ninject.Modules;
using Ninject.Web.Common;
using Video_Streaming.Repository.Interface;
using Video_Streaming.Repository.Imp;
using Video_Streaming.Services.Imp;
using Video_Streaming.Services.Interface;

namespace Video_Streaming.App_Start
{
    public class BindingModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IDbConnection>().To<SqlConnection>().InTransientScope()
             .WithConstructorArgument("connectionString",
                          ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);



            Bind<IVideoRepository>()
            .To<VideoRepository>()
            .InRequestScope();

            Bind<IVideoService>()
             .To<VideoService>()
             .InTransientScope();

        }
    }
}