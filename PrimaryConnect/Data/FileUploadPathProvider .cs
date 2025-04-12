
    using System;
    using System.IO;

    namespace PrimaryConnect
    {
        // Define the interface for file upload path provider
        public interface IFileUploadPathProvider
        {
            string GetUploadPath();
        }

        // Implement the IFileUploadPathProvider interface
        public class FileUploadPathProvider : IFileUploadPathProvider
        {
       
        private readonly string _webRootPath;

            // Constructor that takes the web root path as a parameter
            public FileUploadPathProvider(string webRootPath)
            {
                _webRootPath = webRootPath ?? throw new ArgumentNullException(nameof(webRootPath));
            }

            // This method will return the path to the 'uploads' directory under wwwroot
            public string GetUploadPath()
            {
                // Combine the web root path with 'uploads' to define where the files will be stored
                return Path.Combine(_webRootPath, "uploads");
            }
        }
    }
