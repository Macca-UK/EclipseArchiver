using System.Reflection;
using RazorLight;

namespace DocumentArchiver.Rendering
{
    internal class RazorLightHtmlRenderer
    {
        private readonly RazorLightEngine engine;

        public RazorLightHtmlRenderer()
        {
            string templatePath = $"{Assembly.GetExecutingAssembly().GetName().Name}.Templates";

            Console.WriteLine($"Template path: {templatePath}");

            engine = new RazorLightEngineBuilder()
                            .UseEmbeddedResourcesProject(Assembly.GetExecutingAssembly(), templatePath)
                            .UseMemoryCachingProvider()
                            .Build();

            engine.Options.EnableDebugMode = true;
        }

        public string Render(string templateKey, object model)
        {
            var task = Task.Run(() => engine.CompileRenderAsync(templateKey, model));
            task.Wait();

            return task.Result;
        }
    }
}