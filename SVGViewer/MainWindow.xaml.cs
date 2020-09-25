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

        private string[] files;
        private Uri curr;
        private int pos;

        public MainWindow()
        {
            InitializeComponent();

            //"Open with" from the file explorer passes the file as a command line argument
            //So grab that if it's there
            string[] args = Environment.GetCommandLineArgs();
            if(args.Length > 1)
            {
                Uri uri = new Uri(args[1]);
                LoadImage(uri);
                LoadDir(uri);
            }
        }

        //Bound to Ctrl-O
        private void CommandBinding_Open(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            if (openDialog.ShowDialog() == true)
            {
                Uri chosenFile = new Uri(openDialog.FileName);
                LoadImage(chosenFile);
                LoadDir(chosenFile);
            }
        }

        //Reload current image on F5
        private void CommandBinding_Reload(object sender, ExecutedRoutedEventArgs e)
        {
            LoadImage(curr);
            LoadDir(curr);
        }

        private void LoadImage(Uri uri)
        {
            string path = uri.AbsolutePath;

            curr = uri;
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

        private void LoadDir(Uri uri)
        {
            string path = Path.GetDirectoryName(uri.AbsolutePath);

            files = Directory.GetFiles(path, "*.svg");

            //Figure out where in the directory our current file is
            for(int i = 0; i < files.Length; i++)
            {
                //Make sure the path is in a normalized format so it matches
                if (Path.GetFullPath(curr.AbsolutePath).Equals(files[i]))
                {
                    pos = i;
                    return;
                }
            }
            pos = -1;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Left)
            {
                PreviousImage();
            }else if(e.Key == Key.Right)
            {
                NextImage();
            }
        }

        private void NextImage()
        {
            //Error handling boilerplate
            //Just no-op if anything is wrong, the user can still fix it by opening something from a different directory
            if(files == null || pos < 0)
            {
                return;
            }

            pos = (pos + 1) % files.Length; //Wrap around
            LoadImage(new Uri(files[pos]));
        }

        private void PreviousImage()
        {
            if (files == null || pos < 0)
            {
                return;
            }

            int temp = pos - 1;
            pos = temp < 0 ? files.Length - 1 : temp;
            LoadImage(new Uri(files[pos]));
        }


    }
}
