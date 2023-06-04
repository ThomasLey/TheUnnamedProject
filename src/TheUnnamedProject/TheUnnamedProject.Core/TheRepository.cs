using Nada.Core.Collections;
using Nada.Core.JStore;
using TheUnnamedProject.Core.Model;

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
            return lst.GenerateTree(x => x.Id, x => x.Parent);
        }

        public void EnsureStore()
        {
            if (Directory.Exists(_dataPath)) return;

            Directory.CreateDirectory(_dataPath);
            _context.Save(new[]
            {
                new DocumentType()
                {
                    Id = Guid.NewGuid(), Name = "eBook", TitlePattern = "{string|title}_{string|author}_{string|publisher}", Fields = new[]
                    {
                        new FieldType() { Id = Guid.Parse("969a5a68-9786-485b-bb7b-42c2ce9a932c"), Name = "title", Type = "string" },
                        new FieldType() { Id = Guid.Parse("d0131551-d46c-41c0-800a-d5aae66dc8f1"), Name = "author", Type = "string" },
                        new FieldType() { Id = Guid.Parse("67da51c6-9477-41d2-bdae-56fc70951da6"), Name = "publisher", Type = "string" },
                        new FieldType() { Id = Guid.Parse("d7e9cae5-84ab-492b-b155-a8bcfd2b54ae"), Name = "year", Type = "int" },
                    }
                },
                new DocumentType()
                {
                    Id = Guid.NewGuid(), Name = "DataSheet", TitlePattern = "{string|component}_{string|purpose}", Fields = new[]
                    {
                        new FieldType() { Id = Guid.Parse("b587ea96-7963-4ff8-8b9a-e5b7ccafee84"), Name = "component", Type = "string" },
                        new FieldType() { Id = Guid.Parse("f3585724-6550-4212-9ab2-02775176d194"), Name = "purpose", Type = "string" },
                        new FieldType() { Id = Guid.Parse("2068b1e7-85e8-4e1d-b4a3-1386fa93d0cb"), Name = "source", Type = "string" },
                    }
                },
                new DocumentType()
                {
                    Id = Guid.NewGuid(), Name = "Letter", TitlePattern = "{datetime|date|yyyyMMdd}_{string|from}_{string|to}_{string|title}", Fields = new[]
                    {
                        new FieldType() { Id = Guid.Parse("75757416-9fff-4267-9824-7285f9bb0e2e"), Name = "date", Type = "dateonly" },
                        new FieldType() { Id = Guid.Parse("0fb87ed8-f5e5-4380-bc64-1adeaeadc7ef"), Name = "from", Type = "string" },
                        new FieldType() { Id = Guid.Parse("c9858f23-f067-4c7b-98d2-98726e397ff8"), Name = "to", Type = "string" },
                        new FieldType() { Id = Guid.Parse("969a5a68-9786-485b-bb7b-42c2ce9a932c"), Name = "title", Type = "string" },
                    }
                },
                new DocumentType()
                {
                    Id = Guid.NewGuid(), Name = "BulletJournal", TitlePattern = "{datetime|date|yyyyMMdd}_{string|title}", Fields = new[]
                    {
                        new FieldType() { Id = Guid.Parse("75757416-9fff-4267-9824-7285f9bb0e2e"), Name = "date", Type = "dateonly" },
                        new FieldType() { Id = Guid.Parse("969a5a68-9786-485b-bb7b-42c2ce9a932c"), Name = "title", Type = "string" },
                    }
                },
            });

            // generate all these filemaps
            var fm = new[]
            {
                new Filemap() { Id = Guid.Parse("b5c3a25d-efed-4dd0-b855-04883e9bf0fc"), Name = "eBook", StorePattern = "eBook", DocTypes = "eBook,DataSheet" },
                new Filemap() { Id = Guid.Parse("004e3b8e-1b2c-4d28-b9b2-55b855fce725"), Name = "Bullet Journal", StorePattern = "Bullet Journal", DocTypes = "BulletJournal" },
                new Filemap() { Id = Guid.Parse("4de5aa8e-2809-4128-988e-57c3f56fde26"), Name = "Letter", StorePattern = "Schriftverkehr" },
                new Filemap() { Id = Guid.Parse("854c4d48-a9a9-4cbe-ad92-6573f1fff2ff"), Name = "Sent Letter", Parent = Guid.Parse("4de5aa8e-2809-4128-988e-57c3f56fde26"), StorePattern = "Schriftverkehr", DocTypes = "Letter" },
                new Filemap() { Id = Guid.Parse("a6e7d279-0050-496c-b84c-34fe30ab83e6"), Name = "Received Letter", Parent = Guid.Parse("4de5aa8e-2809-4128-988e-57c3f56fde26"), StorePattern = "Schriftverkehr\\Eingang", DocTypes = "Letter" },
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