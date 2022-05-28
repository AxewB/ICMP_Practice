using System;
using System.Collections.Generic;
using System.Drawing;
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
        public GraphVisual(List<Router> routers)
        {
            InitializeComponent();
            List<Vertex> vertices = new List<Vertex>();
            List<Edge> edges = new List<Edge>();

            float main_w = (float)sheet.Width / routers.Count;

            int j = 1, i = 1, k = 1;
            Vertex prev_vert = null;

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

            DrawGraph drawGraph = new DrawGraph((int)sheet.Width, (int)sheet.Height);
            drawGraph.drawALLGraph(vertices, edges);
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

    }
}
