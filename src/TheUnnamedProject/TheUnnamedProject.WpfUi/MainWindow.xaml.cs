﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Nada.Core.Collections;
using Nada.Core.Extensions;
using Nada.Core.Replacer;
using Nada.NZazu.Contracts;
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

                // todo fill properties them!
                var dlg = new PropertyWindow();
                dlg.DocumentType = docType;
                dlg.File = fi.FullName;

                var dlgRes = dlg.ShowDialog();
                if (!dlgRes.HasValue || dlgRes.Value == false) continue;

                var docParams = dlg.Properties;
                var d = new Document()
                {
                    DocumentType = docType.Name,
                    Filemap = filemap.Name,
                    OriginalTitle = fi.Name,
                    RelativeFileName = _replacer.Parse(relativePath, docParams),
                    Title = _replacer.Parse(docType.TitlePattern, docParams),
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
