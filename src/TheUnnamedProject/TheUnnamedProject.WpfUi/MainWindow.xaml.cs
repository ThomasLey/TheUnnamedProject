using System;
using System.Linq;
using System.Security.Cryptography;
using Nada.Core.Extensions;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Nada.Core.Collections;
using Nada.Core.Replacer;
using Nada.NZazu.Contracts;
using TheUnnamedProject.Core;
using TheUnnamedProject.Core.Model;
using Path = System.IO.Path;
using System.Windows.Forms;
using DataFormats = System.Windows.DataFormats;
using DragEventArgs = System.Windows.DragEventArgs;

namespace TheUnnamedProject.WpfUi
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private string _path = "c:\\Workspace\\_UnnamedTestEnsure";
        private TheRepository _repo;
        private readonly List<Document> _documents = new();
        private readonly IPropertyParser _replacer;

        public MainWindow()
        {
            InitializeComponent();

            // todo in production let user open folder
            _repo = new TheRepository(_path);
            _repo.EnsureStore();
            _replacer = new PropertyParserFactory().Create(CultureInfo.CurrentCulture);
            Loaded += WindowLoaded;
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            LoadFilemapTree();
        }

        private void LoadFilemapTree()
        {
            Filemap.Items.Clear();
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
                var originalProperties = new Dictionary<string, string>();

                var fi = new FileInfo(file);
                var relativePath = filemap.StorePattern + Path.DirectorySeparatorChar + docType.TitlePattern + fi.Extension;

                // todo fill properties them!
                var dlg = new PropertyWindow();
                dlg.DocumentType = docType;
                dlg.File = fi.FullName;

                var dlgRes = dlg.ShowDialog();
                if (!dlgRes.HasValue || dlgRes.Value == false) continue;

                var docParams = dlg.Properties;
                originalProperties.Add("OriginalTitle", fi.Name);
                originalProperties.Add("Title", _replacer.Parse(docType.TitlePattern, docParams));

                var d = new Document()
                {
                    Id = Guid.NewGuid(),
                    Hash = fi.GetMD5Hash(),
                    DocumentType = docType.Name,
                    FilemapId = filemap.Id,
                    RelativeFileName = _replacer.Parse(relativePath, docParams),
                    UserProperties = docParams,
                    DocumentProperties = originalProperties
                };

                fi.CopyTo(Path.Combine(_path, d.RelativeFileName));

                _documents.Add(d);
            }

            e.Handled = true;
        }

        private void Filemap_Selected(object sender, RoutedEventArgs e)
        {
            var item = ((TreeViewItem)sender);
            var src = item.DataContext as TreeItem<Filemap>;
            var filemap = src?.Item;
            if (filemap == null) return;

            var files = _documents.Where(x => x.FilemapId == filemap.Id);

            Documents.Items.Clear();
            files.ForEach(x => Documents.Items.Add(x));
        }

        private void Save_OnClick(object sender, RoutedEventArgs e)
        {
            _repo.SetDocuments(_documents.ToArray());
        }

        private void OpenDirectory(object sender, RoutedEventArgs e)
        {
            using var fbd = new FolderBrowserDialog();
            var result = fbd.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
            {
                _path = fbd.SelectedPath;
                _repo = new TheRepository(fbd.SelectedPath);
                _repo.EnsureStore();
                LoadFilemapTree();
            }
        }
    }

    public static class FileInfoExtensions
    {
        public static string GetMD5Hash(this FileInfo source)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(source.FullName))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }
    }

    public static class DocumentTypeNZazuExtensions
    {
        public static FieldDefinition[] ToNZazu(this DocumentType source)
        {
            var result = new List<FieldDefinition>();

            foreach (var type in source.Fields)
            {
                var fd = new FieldDefinition()
                {
                    Key = type.Name,
                    Type = type.Type
                };

                result.Add(fd);
            }

            return result.ToArray();
        }
    }
}
