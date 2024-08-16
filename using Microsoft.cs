using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.IO;

namespace ProfilePictureOverlay
{
    public partial class MainWindow : Window
    {
        private BitmapImage profileImage;
        private BitmapImage overlayImage;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ChooseFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png";

            if (openFileDialog.ShowDialog() == true)
            {
                profileImage = new BitmapImage(new Uri(openFileDialog.FileName));
                ProfileImage.Source = profileImage;
            }
        }

        private void Overlay_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Image clickedOverlay = sender as Image;
            overlayImage = clickedOverlay.Source as BitmapImage;
            ApplyOverlay();
        }

        private void ApplyOverlay()
        {
            if (profileImage == null || overlayImage == null)
                return;

            // Combine the profile image and overlay
            int width = Math.Max(profileImage.PixelWidth, overlayImage.PixelWidth);
            int height = Math.Max(profileImage.PixelHeight, overlayImage.PixelHeight);

            RenderTargetBitmap renderTarget = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);

            DrawingVisual drawingVisual = new DrawingVisual();
            using (DrawingContext context = drawingVisual.RenderOpen())
            {
                context.DrawImage(profileImage, new Rect(0, 0, profileImage.PixelWidth, profileImage.PixelHeight));
                context.DrawImage(overlayImage, new Rect(0, 0, profileImage.PixelWidth, profileImage.PixelHeight));
            }

            renderTarget.Render(drawingVisual);
            ProfileImage.Source = renderTarget;

            // Optional: Save the result to a file
            SaveOverlay(renderTarget);
        }

        private void SaveOverlay(RenderTargetBitmap renderTarget)
        {
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(renderTarget));

            using (FileStream fileStream = new FileStream("output.png", FileMode.Create))
            {
                encoder.Save(fileStream);
            }
        }
    }
}
