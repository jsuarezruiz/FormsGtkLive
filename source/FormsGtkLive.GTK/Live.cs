using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xamarin.Forms;

namespace FormsGtkLive.GTK
{
    public static class Live
    {
        static readonly Regex Regex = new Regex("x:Class=\"([^\"]+)\"");

        public static void Init(Application application)
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

                var match = Regex.Match(xaml);
                if (!match.Success) return;
                var className = match.Groups[1].Value;

                // Get Page
                var page = GetPage(application.MainPage, className);

                if (page == null)
                    return;

                try
                {
                    // User XAML Content
                    await UpdatePageFromXamlAsync(page, xaml);
                }
                catch (Exception exception)
                {
                    // Error Page
                    var errorXaml = GetXamlException(exception);
                    await UpdatePageFromXamlAsync(page, errorXaml);
                    Console.WriteLine(exception.Message);
                }
            };
        }

        private static Page GetPage(Page page, string fullTypeName)
        {
            if (page == null)
                return null;

            if (page.GetType().FullName == fullTypeName)
                return page;

            return null;
        }

        private static Task UpdatePageFromXamlAsync(Page page, string xaml)
        {
            var taskCompletionSource = new TaskCompletionSource<Page>();

            Device.BeginInvokeOnMainThread(() =>
            {
                var bindingContext = page.BindingContext;
                try
                {
                    Console.WriteLine("Loading XAML...");
                    LoadXaml(page, xaml);
                    page.ForceLayout(); // Update
                    taskCompletionSource.SetResult(page);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message);
                    taskCompletionSource.SetException(exception);
                }
                finally
                {
                    page.BindingContext = bindingContext;
                    Console.WriteLine("XAML Loaded!");
                }
            });

            return taskCompletionSource.Task;
        }

        private static string GetXamlException(Exception exception)
        {
            XNamespace xmlns = "http://xamarin.com/schemas/2014/forms";

            var errorPage = new XDocument(
                new XElement(xmlns + "ContentPage",
                new XElement(xmlns + "ScrollView",
                new XElement(xmlns + "StackLayout",
                new XAttribute("Margin", "12, 0"),
                new XElement(xmlns + "Label",
                    new XAttribute("Text", "Oops!"),
                    new XAttribute("TextColor", "Red"),
                    new XAttribute("FontSize", "Large")
                ),
                new XElement(xmlns + "Label",
                    new XAttribute("Text", exception.Message),
                    new XAttribute("TextColor", "Red"),
                    new XAttribute("LineBreakMode", "CharacterWrap"),
                    new XAttribute("FontSize", "Small")
                ))))).ToString();

            return errorPage;
        }

        private static void LoadXaml(BindableObject view, string xaml)
        {
            var xamlAssembly = Assembly.Load(new AssemblyName("Xamarin.Forms.Xaml"));
            var xamlLoader = xamlAssembly.GetType("Xamarin.Forms.Xaml.XamlLoader");
            var load = xamlLoader.GetRuntimeMethod("Load", new[] { typeof(BindableObject), typeof(string) });

            try
            {
                load.Invoke(null, new object[] { view, xaml });
            }
            catch (TargetInvocationException exception)
            {
                throw exception.InnerException; // To show to the user in the ErrorPage!
            }
        }
    }
}
