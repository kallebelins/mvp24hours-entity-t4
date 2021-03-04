using Mvp24Hours.Entity.Core.Settings;
using Mvp24Hours.Entity.SqlServer;
using System.IO;

namespace GenerateFromObjectApp
{
    class Program
    {
        static void Main(string[] _)
        {
            System.Console.WriteLine("Loading database settings.");

            var mvpSettings = new Mvp24Settings()
            {
                Database = new DatabaseSettings()
                {
                    DataSource = "",
                    Catalog = "",
                    TrustedConnection = true
                },
                IdKeyName = true, // Use if your table columns have the name "ID"
                TableFilter = "", // Use to filter the tables you want to read
            };

            var dbEntity = new DbEntitySqlServer(mvpSettings);
            var settings = dbEntity.GetSettings();

            if (settings != null)
            {
                System.Console.WriteLine("Writing processed results in the \"Models\" folder..");

                string pathDir = GetDirectory();

                foreach (var entity in settings.Entities)
                {
                    System.Console.WriteLine($"Writing {entity.ClassName}.cs");
                    ITextTemplate template = new ModelTemplate(settings, entity);
                    File.WriteAllText($"{pathDir}{entity.ClassName}.cs", template.TransformText());
                }
            }

            System.Console.WriteLine("Press any key to exit.");
            System.Console.Read();
        }

        private static string GetDirectory()
        {
            string dir = $"{Directory.GetCurrentDirectory()}\\Models\\";
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            return dir;
        }
    }
}
