using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MYWatcher
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
           
                await Task.Delay(1000, stoppingToken);


                FileSystemWatcher watcher = new FileSystemWatcher();
                watcher.Path = @"C:\Users\stali\Desktop\MyWatcher";

                watcher.IncludeSubdirectories = false;

                watcher.NotifyFilter = NotifyFilters.Attributes | NotifyFilters.CreationTime 
                | NotifyFilters.FileName| NotifyFilters.Size;

                watcher.Filter = "*.*";

                //Register Event Handler

                watcher.Changed += new FileSystemEventHandler(onChanged);
                watcher.Created += new FileSystemEventHandler(onChanged);
                watcher.Deleted += new FileSystemEventHandler(onChanged);
                watcher.Renamed += new RenamedEventHandler(onRenamed);

                //Start Monitoring

                watcher.EnableRaisingEvents = true;


           
        }

        public void onChanged(object source, FileSystemEventArgs e){


            Console.WriteLine(e.Name + " is " + e.ChangeType);

            if(e.ChangeType == WatcherChangeTypes.Created)
            {
                string targetPath = "";
                if(Path.GetExtension(e.FullPath.ToString())==".txt") 
                    targetPath = @"C:\Users\stali\Desktop\Organiser\Text";    
                else if(Path.GetExtension(e.FullPath.ToString())==".xlsx") 
                    targetPath = @"C:\Users\stali\Desktop\Organiser\Excel"; 
                else if(Path.GetExtension(e.FullPath.ToString())==".json") 
                    targetPath = @"C:\Users\stali\Desktop\Organiser\JSON"; 

                targetPath = Path.Combine(targetPath,Path.GetFileName(e.FullPath.ToString()));
                File.Copy(e.FullPath.ToString(), targetPath,true);

                if(File.Exists(targetPath))
                    File.Delete(e.FullPath.ToString());

                Console.WriteLine(e.Name +" File moved successfully to organizer folder");

            }

            
        }

        public void onRenamed(object source, RenamedEventArgs e){
            Console.WriteLine(e.OldName + " is changed as "+ e.Name);
        }
    }
}
