using System;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace Windows_OEM_Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly RegistryHelpers _regHelper = new RegistryHelpers();
        public MainWindow()
        {
            MessageBox.Show("I am not reliable for any damages that come to you if you use this software wrong" + Environment.NewLine + Environment.NewLine + "" +
                            "This software comes as is without any warrenty at all you are on your own", "Notice!");
            InitializeComponent();

            Loaded += (sender, args) =>
            {
                foreach (var tb in Helpers.FindVisualChildren<TextBox>(this))
                {
                    tb.Text = _regHelper.GetValue(tb.Tag.ToString(), string.Empty);
                    if (!tb.Tag.ToString().Equals("Logo", StringComparison.CurrentCultureIgnoreCase) ||
                        tb.Text == string.Empty) continue;
                    tb.ToolTip = tb.Text;
                    tb.Text = Path.GetFileName(tb.Text);
                }

                foreach (var image in Helpers.FindVisualChildren<Image>(this))
                {
                    var url = _regHelper.GetValue(image.Tag.ToString(), string.Empty);

                    if(url == string.Empty) continue;

                    var f = Path.GetTempFileName();
                    File.Copy(url, f, true);
                    image.Source = new BitmapImage(new Uri(f));
                }
            };
        }

        private void BrowseForImage_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                Filter = "Image Files|*.png;*.jpg;*.jpeg;*.bmp;*.tif;*.tiff",
                Title = "Select a Image",
                Multiselect = false
            };

            var dialog = ofd.ShowDialog();
            if (dialog != null && !(bool) dialog) return;

            TextBox imageName =
                Helpers.FindVisualChildren<TextBox>(this).SingleOrDefault(o => o.Tag.ToString().Equals("Logo"));


            Image imagePreview =
                Helpers.FindVisualChildren<Image>(this).SingleOrDefault(o => o.Tag.ToString().Equals("Logo"));

            if (imageName == null) return;
            if (imagePreview == null) return;

            imageName.Text = ofd.SafeFileName;
            imageName.ToolTip = ofd.FileName;

            imagePreview.Source = Helpers.ResizeImage(System.Drawing.Image.FromFile(ofd.FileName), new System.Drawing.Size(150, 150)).ToBitmapSource();
        }

        private void RemoveOEMValues_Click(object sender, RoutedEventArgs e)
        {
            foreach (var tb in Helpers.FindVisualChildren<TextBox>(this))
            {
                tb.Text = string.Empty;
                if (!tb.Tag.ToString().Equals("Logo", StringComparison.CurrentCultureIgnoreCase) ||
                    tb.Text == string.Empty) continue;
                tb.ToolTip = null;
                tb.Text = string.Empty;
            }

            foreach (var image in Helpers.FindVisualChildren<Image>(this))
            {
                image.Source = null;
            }
        }

        private void ApplyOEMValues_Click(object sender, RoutedEventArgs e)
        {
            foreach (var tb in Helpers.FindVisualChildren<TextBox>(this))
            {
                if (tb.Tag.ToString().Equals("Logo", StringComparison.CurrentCultureIgnoreCase))
                {
                    if (tb.ToolTip == null || tb.Text == string.Empty) _regHelper.SetValue(tb.Tag.ToString(), string.Empty);
                    else
                    {
                        var image_ = System.Drawing.Image.FromFile(tb.ToolTip.ToString());
                        var f =
                            Path.Combine(Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.System)),
                                "oemImage.bmp");

                        if (tb.ToolTip.ToString() == f) continue;

                        if(File.Exists(f)) File.Delete(f);

                        image_.Save(f, ImageFormat.Bmp);

                        _regHelper.SetValue(tb.Tag.ToString(), f);
                    }

                    continue;
                }

                _regHelper.SetValue(tb.Tag.ToString(), tb.Text);
            }

            MessageBox.Show("OEM Information has been udpated!", "Updated ORM Information");
        }
    }
}
