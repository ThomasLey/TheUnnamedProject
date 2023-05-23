using System.Reflection;
using Nada.Collections;
using Nada.JStore;

namespace TheUnnamedProject.Core
{
    public class TheRepository
    {
        private readonly JsonFileStoreContext _context;
        private readonly string _dataPath;

        public TheRepository(string path)
        {
            _dataPath = Path.Combine(path, ".data");
            var fileReader = new FileReader(_dataPath);
            _context = new JsonFileStoreContext(fileReader);
        }
        public IEnumerable<Document> GetDocuments()
        {
            return _context.Get<Document>();
        }

        public void SetDocuments(IEnumerable<Document> documents)
        {
            _context.Save(documents);
        }

        public IEnumerable<DocumentType> GetDocumentTypes()
        {
            return _context.Get<DocumentType>();
        }

        public IEnumerable<TreeItem<Filemap>> GetFilemaps()
        {
            var lst = _context.Get<Filemap>();
            return lst.GenerateTree(x => x.Name, x => x.Parent);
        }

        public void EnsureStore()
        {
            if (Directory.Exists(_dataPath)) return;

            Directory.CreateDirectory(_dataPath);
            _context.Save(new[]
            {
                new DocumentType()
                {
                    Name = "eBook", TitlePattern = "{title}_{author}_{publisher}", Fields = new[]
                    {
                        new FieldType() { Name = "Title", Type = "string" },
                        new FieldType() { Name = "Author", Type = "string" },
                        new FieldType() { Name = "Publisher", Type = "string" },
                        new FieldType() { Name = "Year", Type = "int" },
                    }
                },
                new DocumentType()
                {
                    Name = "DataSheet", TitlePattern = "{component}_{purpose}", Fields = new[]
                    {
                        new FieldType() { Name = "Component", Type = "string" },
                        new FieldType() { Name = "Purpose", Type = "string" },
                        new FieldType() { Name = "Source", Type = "string" },
                    }
                },
                new DocumentType()
                {
                    Name = "Letter", TitlePattern = "{date}_{from}_{to}_{title}", Fields = new[]
                    {
                        new FieldType() { Name = "Date", Type = "date" },
                        new FieldType() { Name = "From", Type = "string" },
                        new FieldType() { Name = "To", Type = "string" },
                        new FieldType() { Name = "Title", Type = "string" },
                    }
                },
                new DocumentType()
                {
                    Name = "BulletJournal", TitlePattern = "{date}_{title}", Fields = new[]
                    {
                        new FieldType() { Name = "Date", Type = "date" },
                        new FieldType() { Name = "Title", Type = "string" },
                    }
                },
            });

            // generate all these filemaps
            var fm = new[]
            {
                new Filemap() { Name = "eBook", StorePattern = "eBook", DocTypes = "ebook,datasheet" },
                new Filemap() { Name = "Bullet Journal", StorePattern = "journal", DocTypes = "bulletjournal" },
                new Filemap() { Name = "Letter", StorePattern = "letter" },
                new Filemap() { Name = "Sent Letter", Parent = "Letter", StorePattern = "letter", DocTypes = "letter" },
                new Filemap()
                {
                    Name = "Received Letter", Parent = "Letter", StorePattern = "letter\\received", DocTypes = "letter"
                },
            };
            _context.Save(fm);
            foreach (var filemap in fm)
            {
                Directory.CreateDirectory(Path.Combine(_dataPath, "..", filemap.StorePattern!));
            }

            _context.Save(Array.Empty<Document>());
        }
    }
}