using System;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Platform.GTK;

namespace FormsGtkLive.GTK
{
    class Program
    {
        static void Main(string[] args)
        {
            Gtk.Application.Init();
            Forms.Init();
            var app = new AppEditor();
            LivelXaml(app);
            var window = new FormsWindow();
            window.LoadApplication(app);
            window.SetApplicationTitle("Live XAML Editor");
            window.Show();
            Gtk.Application.Run();
        }

            public static void LivelXaml(Application application)
        {
            var directory = Directory.GetCurrentDirectory();

            var fw = new FileSystemWatcher(directory)
            {
                IncludeSubdirectories = true,
                EnableRaisingEvents = true,
                NotifyFilter = NotifyFilters.LastWrite
            };

            // Listens to XAML file changes
            fw.Changed += async (sender, eventArgs) =>
            {
                Console.WriteLine(string.Format("Waiting changes in XAML from {0}", eventArgs.FullPath));

                var extension = Path.GetExtension(eventArgs.FullPath);

                if (extension != ".xaml")
                    return;

                var path = eventArgs.FullPath;

                // Read XAML file content
                var xaml = string.Empty;
                using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var textReader = new StreamReader(fileStream))
                {
                    xaml = textReader.ReadToEnd();
                }

                Console.WriteLine(xaml);

                var match = Live.Regex.Match(xaml);
                if (!match.Success) return;
                var className = match.Groups[1].Value;

                // Get Page
                var page = Live.GetPage(application.MainPage, className);

                if (page == null)
                    return;

                try
                {
                    // User XAML Content
                    await Live.UpdatePageFromXamlAsync(page, xaml);
                }
                catch (Exception exception)
                {
                    // Error Page
                    var errorXaml = Live.GetXamlException(exception);
                    await Live.UpdatePageFromXamlAsync(page, errorXaml);
                    Console.WriteLine(exception.Message);
                }
            };
        }
    }
}