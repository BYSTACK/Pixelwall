﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;

namespace Pixelwall
{
    /// <summary>
    /// Interaction logic for Result.xaml
    /// </summary>
    public partial class Result : Window
    {
        Pixelart art;
        Bitmap image;
        Data data;

        public Result(Pixelart result, Data data)
        {
            InitializeComponent();

            art = result;
            image = art.GetImage();
            this.data = data;
            GenerationResult.Source = Util.BitmapToBitmapImage(image);
            AddMaterials();
        }



        private void AddMaterials()
        {
            foreach (KeyValuePair<string, int> pair in art.blockUses)
            {
                System.Windows.Controls.Image image = new System.Windows.Controls.Image
                {
                    Source = Util.BitmapToBitmapImage(data.textures[pair.Key].texture),
                    Margin = new Thickness(2.0)
                };

                TextBlock number = new TextBlock
                {
                    Text = data.textures[pair.Key].displayName + " - " + pair.Value.ToString(),
                    Margin = new Thickness(2.0)
                };

                StackPanel panel = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(5.0)
                };

                Border border = new Border
                {
                    BorderBrush = System.Windows.Media.Brushes.Gray,
                    BorderThickness = new Thickness(1.0),
                    Margin = new Thickness(5.0)
                };

                panel.Children.Add(image);
                panel.Children.Add(number);
                border.Child = panel;
                MaterialList.Children.Add(border);
            }
        }

        private void OnSaveClick(object sender, RoutedEventArgs e)
        {
            var fileDialog = new Microsoft.Win32.SaveFileDialog();
            fileDialog.Filter = "PNG image|*.png";
            fileDialog.Title = "Save an Image";
            fileDialog.ShowDialog();

            if (!String.IsNullOrEmpty(fileDialog.FileName))
            {
                if (ShowChunkGrid.IsChecked.Value)
                {
                    DrawnChunkGrid(image).Save(fileDialog.FileName);
                }
                else
                {
                    image.Save(fileDialog.FileName);
                }
            }
        }

        private Bitmap DrawnChunkGrid(Bitmap image)
        {
            Bitmap newImage = new Bitmap(image);
            Graphics gr = Graphics.FromImage(newImage);

            for (int i = 0; i < image.Size.Width; i += data.TextureResolution * 16)
            {
                gr.DrawLine(Pens.Red, new PointF { X = i, Y = 0 }, new PointF { X = i, Y = image.Size.Height });
            }

            for (int i = 0; i < image.Size.Height; i += data.TextureResolution * 16)
            {
                gr.DrawLine(Pens.Red, new PointF { X = 0, Y = i }, new PointF { X = image.Size.Width, Y = i});
            }

            return newImage;
        }

        private void OnSaveTextClick(object sender, RoutedEventArgs e)
        {
            var fileDialog = new Microsoft.Win32.SaveFileDialog();
            fileDialog.Filter = "Txt file|*.txt";
            fileDialog.Title = "Save resources list";
            fileDialog.ShowDialog();

            if (!String.IsNullOrEmpty(fileDialog.FileName))
            {
                StreamWriter file = new StreamWriter(fileDialog.FileName);
                foreach (KeyValuePair<string, int> pair in art.blockUses)
                {
                    file.WriteLine(data.textures[pair.Key].displayName + ": " + pair.Value);
                }
                file.Close();
                file.Dispose();
            }
        }
    }
}
