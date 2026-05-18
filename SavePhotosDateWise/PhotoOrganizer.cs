using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace SavePhotosDateWise
{
    public partial class PhotoOrganizer : ServiceBase
    {
        private string sourceFolderPath = @"E:\friends pic"; // Change this to your source folder path
        private string destinationRootFolder = @"E:\OrganizePhoto DateWise"; // Change this to your destination root folder path
        public PhotoOrganizer()
        {
            InitializeComponent();

        }
        
        Timer timer;
        protected override void OnStart(string[] args)
        {
            // Start organizing photos when the service starts
            

            timer = new Timer();
            timer.Enabled = true;
            timer.Interval = 300000;//1min
            timer.Elapsed += OnEventExecution;
            timer.Start();
            
        }
        private void OnEventExecution(Object sender, ElapsedEventArgs eventArgs)
        {
            try
            {
                OrganizePhotos();
                timer.Enabled = false;
                timer.Stop();
                //timer.Dispose();
                timer.Enabled = true;
                timer.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }

        }
        protected override void OnStop()
        {
            // Add cleanup logic if needed
        }

        private void OrganizePhotos()
        {

          //  System.Diagnostics.Debugger.Launch();
            // Define the list of image file extensions
            string[] imageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp" }; // Add more extensions if needed

            // Get all image files in the source folder
            string[] imageFiles = Directory.GetFiles(sourceFolderPath, "*", SearchOption.AllDirectories);

            //string[] imageFiles = Directory.GetFiles(sourceFolderPath)
                                         //   .Where(file => imageExtensions.Contains(Path.GetExtension(file).ToLower()))
                                           // .ToArray();

            foreach (string imagePath in imageFiles)
            {
                try
                {
                    // Get the creation date of the image
                    DateTime creationDate = File.GetCreationTime(imagePath);

                    // Create a folder for the date if it doesn't exist
                    string destinationFolder = Path.Combine(destinationRootFolder, creationDate.ToString("yyyy-MM-dd"));
                    if (!Directory.Exists(destinationFolder))
                    {
                        Directory.CreateDirectory(destinationFolder);
                    }

                    // Move the image to the date-wise folder
                    string fileName = Path.GetFileName(imagePath);
                    string destinationFilePath = Path.Combine(destinationFolder, fileName);
                    File.Move(imagePath, destinationFilePath);

                    Console.WriteLine($"Moved {fileName} to {destinationFolder}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing {imagePath}: {ex.Message}");
                }
            }

            Console.WriteLine("Process completed.");
        }

    }
}
