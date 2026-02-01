using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.Security.Authentication;
using System.Data.SqlClient;
using System;
using System.IO;

namespace WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //Read Configuration from appSettings
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();


            // test bd connection 
            TestDatabaseConnection(config);
            //Initialize Logger
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(config)
                .CreateLogger();

            CreateHostBuilder(args).Build().Run();
        }
        private static  void TestDatabaseConnection(IConfiguration config)
        {
            var count = 0;
            try {
                count++;
                var connstr = config.GetConnectionString("DefaultConnection");
                using var conn = new SqlConnection(connstr);
                conn.Open();
                File.AppendAllText("C:\\temp\\db-log.txt", 
                    +count+
        ". SUCCESS: DB Connected\n");
               
            }
            catch (SqlException ex)
            {
                count++;
                Console.WriteLine("Database connection failed");
                File.AppendAllText("C:\\temp\\db-log.txt",
        "ERROR: " + count  +". "+ ex.Message +"\n");
                throw; // Rethrow or handle as needed
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(options =>
                        {
                            //options.Limits.MaxRequestBodySize = 536870912;
                            options.ConfigureHttpsDefaults(co => co.SslProtocols = SslProtocols.Tls12);
                        })
                        .UseStartup<Startup>();
                });
    }
}