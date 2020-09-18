using Microsoft.Win32;
using Svg2Xaml;
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace SVGViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        //Bound to Ctrl-O
        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            if (openDialog.ShowDialog() == true)
            {
                Uri chosenFile = new Uri(openDialog.FileName);
                LoadImage(chosenFile);
            }
        }

        private void LoadImage(Uri uri)
        {
            string path = uri.AbsolutePath;
            try
            {
                if (path.EndsWith(".svg"))
                {
                    using (FileStream stream = File.OpenRead(path))
                    {
                        //Convert svg to xaml so Image knows how to display it
                        //Only takes one line--now that's my kind of library
                        var img = SvgReader.Load(stream);
                        display.Source = img;
                    }
                }
                else
                {
                    //Raster images aren't the point of this, but if we get one for some reason, we might as well do something useful
                    display.Source = new BitmapImage(uri);
                }

                Title = $"SVG Viewer - {Path.GetFileName(path)}";
            }
            catch (Exception)
            {
                //YOLO
                //But seriously, the only error handling it's worth it for us to bother with is keeping the program from crashing
                //so the user can try another file.
                Title = $"Could not show: {Path.GetFileName(path)}";
            }
        }
    }
}
