using Nada.Core.Collections;
using Nada.Core.JStore;

namespace TheUnnamedProject.Core
{
    public class TheRepository
    {
        private readonly JsonFileStoreContext _context;
        private readonly string _dataPath;

        public TheRepository(string path)
        {
            _dataPath = Path.Combine(path, ".data");
            var fileReader = new FileStore(_dataPath);
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
                    Name = "eBook", TitlePattern = "{string|title}_{string|author}_{string|publisher}", Fields = new[]
                    {
                        new FieldType() { Name = "title", Type = "string" },
                        new FieldType() { Name = "author", Type = "string" },
                        new FieldType() { Name = "publisher", Type = "string" },
                        new FieldType() { Name = "year", Type = "int" },
                    }
                },
                new DocumentType()
                {
                    Name = "DataSheet", TitlePattern = "{string|component}_{string|purpose}", Fields = new[]
                    {
                        new FieldType() { Name = "component", Type = "string" },
                        new FieldType() { Name = "purpose", Type = "string" },
                        new FieldType() { Name = "source", Type = "string" },
                    }
                },
                new DocumentType()
                {
                    Name = "Letter", TitlePattern = "{datetime|date|yyyyMMdd}_{string|from}_{string|to}_{string|title}", Fields = new[]
                    {
                        new FieldType() { Name = "date", Type = "date" },
                        new FieldType() { Name = "from", Type = "string" },
                        new FieldType() { Name = "to", Type = "string" },
                        new FieldType() { Name = "title", Type = "string" },
                    }
                },
                new DocumentType()
                {
                    Name = "BulletJournal", TitlePattern = "{datetime|date|yyyyMMdd}_{string|title}", Fields = new[]
                    {
                        new FieldType() { Name = "date", Type = "date" },
                        new FieldType() { Name = "title", Type = "string" },
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