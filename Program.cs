using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Net;


    class Program
    {
        static void Main(string[] args)
        {
            
            String address = "";  
            WebRequest request = WebRequest.Create("http://checkip.dyndns.org/");  
            using (WebResponse response = request.GetResponse())  
            using (StreamReader stream = new StreamReader(response.GetResponseStream()))  
            {  
                address = stream.ReadToEnd();  
            }  
        
            int first = address.IndexOf("Address: ") + 9;  
            int last = address.LastIndexOf("</body>");  

            Console.WriteLine(address.Substring(first, last - first));
            CreateWebHostBuilder(args).Build().Run();
        }

        private static IWebHostBuilder CreateWebHostBuilder(string[] args)=>
            WebHost.CreateDefaultBuilder(args).UseStartup<Startup>();

    }
    public class Startup
    {

        public Startup (IConfiguration configuration){
            Configuration = configuration;
        }
        public IConfiguration Configuration{get;}

        public void ConfigureServices(IServiceCollection services){
            services.AddSignalR();
        }
        public void Configure(IApplicationBuilder app){
            app.UseRouting();

            app.UseEndpoints(endpoints => {
                endpoints.MapHub<ChatHub>("/chatHub");
            });
        }
    }