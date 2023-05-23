using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MagicBox.Extensions;
using Nada.Collections;
using TheUnnamedProject.Core;
using Path = System.IO.Path;

namespace TheUnnamedProject.WpfUi
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly string _path = "c:\\Workspace\\_UnnamedTestEnsure";
        private readonly TheRepository _repo;
        private readonly List<Document> _documents = new();

        public MainWindow()
        {
            InitializeComponent();

            // todo in production let user open folder
            _repo = new TheRepository(_path);
            _repo.EnsureStore();
            Loaded += WindowLoaded;
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            var filemap = _repo.GetFilemaps();
            filemap.ForEach(x => Filemap.Items.Add(x));

            _documents.AddRange(_repo.GetDocuments());
        }

        private void Filemap_OnDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;

            var files = (e.Data.GetData(DataFormats.FileDrop, false)) as string[] ?? Array.Empty<string>();

            var item = ((TreeViewItem)sender);
            var src = item.DataContext as TreeItem<Filemap>;
            var filemap = src.Item;
            if (filemap == null) return;

            // here comes the fine code
            var docType = _repo.GetDocumentTypes().Where(x => x.Name.ToLower() == filemap.DocTypes.Split(",")[0].Trim().ToLower()).Single();

            foreach (var file in files)
            {
                var fi = new FileInfo(file);
                var relativePath = filemap.StorePattern + Path.DirectorySeparatorChar + docType.TitlePattern + fi.Extension;
                var docParams = new Dictionary<string, string>();

                // todo fill properties them!
                docParams.Add("Date", "2003-04-30");
                docParams.Add("Title", "April 2023");

                var d = new Document()
                {
                    DocumentType = docType.Name,
                    Filemap = filemap.Name,
                    OriginalTitle = fi.Name,
                    RelativeFileName = relativePath.ReplaceWith(docParams),
                    Title = docType.TitlePattern.ReplaceWith(docParams),
                    Properties = docParams,
                };

                fi.CopyTo(Path.Combine(_path, d.RelativeFileName));

                _documents.Add(d);
            }
        }

        private void Filemap_Selected(object sender, RoutedEventArgs e)
        {
            var item = ((TreeViewItem)sender);
            var src = item.DataContext as TreeItem<Filemap>;
            var filemap = src?.Item;
            if (filemap == null) return;

            var files = _documents.Where(x => x.Filemap == filemap.Name);

            Documents.Items.Clear();
            files.ForEach(x => Documents.Items.Add(x));
        }

        private void Save_OnClick(object sender, RoutedEventArgs e)
        {
            _repo.SetDocuments(_documents.ToArray());
        }
    }

    public static class StringExtensions
    {
        public static string ReplaceWith(this string source, Dictionary<string, string> parameters)
        {
            var result = source;
            foreach (var parameter in parameters)
                result = result.Replace("{" + parameter.Key.ToLower() + "}", parameter.Value);
            return result;
        }
    }
}
