using Nada.NZazu.Contracts;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using TheUnnamedProject.Core;

namespace TheUnnamedProject.WpfUi
{
    /// <summary>
    /// Interaction logic for PropertyWindow.xaml
    /// </summary>
    public partial class PropertyWindow : Window
    {
        private string _file;

        public PropertyWindow()
        {
            InitializeComponent();
        }

        public DocumentType DocumentType
        {
            set
            {
                View.FormDefinition = new FormDefinition { Fields = value.ToNZazu() };
            }
        }

        public string File
        {
            set
            {
                _file = value;
                //var uri = new Uri("file://" + value, UriKind.Absolute);
                //Browser.Navigate(uri.AbsoluteUri);
            }
        }

        public Dictionary<string, string> Properties
        {
            get
            {
                return View.FormData;
            }
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            Close();
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            Close();
        }

        private void Open_OnClick(object sender, RoutedEventArgs e)
        {
            var pi = new ProcessStartInfo()
            {
                UseShellExecute = true,
                FileName = _file,
                Verb = "OPEN"
            };
            Process.Start(pi);
        }
    }
}
