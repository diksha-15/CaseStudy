using log4net.Config;
using Serilog.Events;
using Serilog;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Video_Streaming.App_Start;
using Serilog.Sinks.MSSqlServer;
using System.Configuration;

namespace Video_Streaming
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            XmlConfigurator.Configure();

            ConfigureLogging();
        }

        private void ConfigureLogging()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString.ToString();
            Log.Logger = new LoggerConfiguration()
                .WriteTo.MSSqlServer(
                    connectionString: connectionString,
                    sinkOptions: new MSSqlServerSinkOptions { TableName = "RunLog", AutoCreateSqlTable = true },
                    restrictedToMinimumLevel: LogEventLevel.Information)
                .CreateLogger();
        }

    }
}
