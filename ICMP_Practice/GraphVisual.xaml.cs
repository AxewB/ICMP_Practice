using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ICMP_Practice
{
    /// <summary>
    /// Логика взаимодействия для GraphVisual.xaml
    /// </summary>
    public partial class GraphVisual : Window
    {
        Bitmap image;
        public GraphVisual(List<Router> routers)
        {
            InitializeComponent();
            List<Vertex> vertices = new List<Vertex>();
            List<Edge> edges = new List<Edge>();

           

            int devicesCount = 0;
            int R = 15;

            foreach (Router rout in routers)
            {
                foreach (Subnet subnet in rout.subnets)
                {
                    foreach (Device device in subnet.devices)
                    {
                        if (device.pinged)
                        {
                            devicesCount++;
                        }
                    }
                }
            }

            int j = 1, i = 1, k = 1;
            Vertex prev_vert = null;

            int bitmapWidth = R * 16 * devicesCount;
            int bitmapHeight = R * 4 * 4;
            float main_w = bitmapWidth / routers.Count;

            foreach (Router router in routers)
            {

                Vertex rout_vert = new Vertex((int)((main_w * j) - (main_w / 2)), 30, router.name);
                float router_w = main_w / router.subnets.Count;

                if (prev_vert != null)
                {
                    Edge edge = new Edge(rout_vert, prev_vert);
                    edges.Add(edge);
                }

                foreach (Subnet subnet in router.subnets)
                {
                    Vertex subnet_vert = new Vertex((int)((router_w*i)-(router_w/2)), 120, subnet.ip);
                    Edge edge = new Edge(rout_vert, subnet_vert);
                   
                    int count = 0;
                    foreach (Device device in subnet.devices)
                    {
                        if (device.pinged)
                        {
                            count++;
                        }
                    }
                    float sub_w = router_w / count;
                    foreach (Device device in subnet.devices)
                    {
                        if (device.pinged)
                        {
                            Vertex dev_vert = new Vertex((int)((sub_w * k) - (sub_w / 2)), 210, device.ip);
                            Edge dev_edge = new Edge(subnet_vert, dev_vert);
                            vertices.Add(dev_vert);
                            edges.Add(dev_edge);
                            k++;

                        }
                    }
                    vertices.Add(subnet_vert);
                    edges.Add(edge);
                    i++;
                    prev_vert = rout_vert;
                }
                vertices.Add(rout_vert);
                j++;
            }

            DrawGraph drawGraph = new DrawGraph(bitmapWidth, bitmapHeight);
            drawGraph.drawALLGraph(vertices, edges);
            image = drawGraph.GetBitmap();
            sheet.Source = BitmapToImageSource(drawGraph.GetBitmap());
            
        }

        BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }

        }

        private void SavePictureButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                image.Save("Graph.png", ImageFormat.Png);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            MessageBox.Show("Картинка сохранена");
        }
    }
}
