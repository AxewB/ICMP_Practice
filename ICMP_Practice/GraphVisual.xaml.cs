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
    public partial class GraphVisual : Window
    {
        Bitmap image;   // Сохраняем bitmap, для сохранения картинки в формате png
        public GraphVisual(List<Router> routers)
        {
            InitializeComponent();
            List<Vertex> vertices = new List<Vertex>(); // Список вершин
            List<Edge> edges = new List<Edge>();        // Список ребер

           

            int devicesCount = 0;   // Считаем количество устройств для расчета размера картинки
            int R = 15;             // Задаем радиус эллипса

            foreach (Router rout in routers)
            {
                foreach (Subnet subnet in rout.subnets)
                {
                    foreach (Device device in subnet.devices)
                    {
                        if (device.pinged)
                        {
                            // Если устройство существует, то увеличиваем счетчик
                            devicesCount++; 
                        }
                    }
                }
            }

            int j = 1, i = 1, k = 1;    // Переменные-множители для расположения вершин на
                                        // определенном расстоянии друг от друга
            Vertex prev_vert = null;    // Переменная предыдущей вершины, чтобы
                                        // соединить роутеры между собой


            int bitmapWidth = R * 16 * devicesCount;    // определение ширины картинки
            int bitmapHeight = R * 4 * 4;               // определение высоты картинки
            float main_w = bitmapWidth / routers.Count; // делим картинку на количество роутеров

            foreach (Router router in routers)
            {
                // Создаем вершину-роутер в центре соответствующей части
                Vertex rout_vert = new Vertex((int)((main_w * j) - (main_w / 2)), 30, router.name);

                // делим часть роутера на количество подсетей
                float router_w = main_w / router.subnets.Count; 

                // Если это второй или больше роутер, то соединяем их между собой
                if (prev_vert != null)
                {
                    Edge edge = new Edge(rout_vert, prev_vert);
                    edges.Add(edge);
                }

                // Просматриваем все подсети роутера
                foreach (Subnet subnet in router.subnets)
                {
                    // Создаем вершину-подсеть с центром в своей части 
                    Vertex subnet_vert = new Vertex((int)((router_w*i)-(router_w/2)), 120, subnet.ip);

                    // Создание ребра между подсетью и ротуером
                    Edge edge = new Edge(rout_vert, subnet_vert);
                   
                    // Считаем количество существующих устройств для подсети
                    int count = 0;
                    foreach (Device device in subnet.devices)
                    {
                        if (device.pinged)
                        {
                            count++;
                        }
                    }

                    // Делим часть подсетей на количество существующих устройств
                    float sub_w = router_w / count;
                    
                    // Если устройство существует, то добавляем вершину и ребро 
                    // между подсетью и устройством
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

                    // Добавляем вершину и ребро в список
                    vertices.Add(subnet_vert);
                    edges.Add(edge);
                    i++;

                    // определяем нынешнюю вершину как предыдущую и переходим к следующей
                    prev_vert = rout_vert; 
                }
                vertices.Add(rout_vert);
                j++;
            }

            // Инициализируем картинку
            DrawGraph drawGraph = new DrawGraph(bitmapWidth, bitmapHeight);

            // Отрисовываем на картинке граф на основе вершин и ребер
            drawGraph.drawALLGraph(vertices, edges);

            // Сохраняем картинку в переменную, и располагаем ее в контроллере Image 
            image = drawGraph.GetBitmap();
            sheet.Source = BitmapToImageSource(drawGraph.GetBitmap());
        }

        // Функция конвертации bitmap в источник для контроллера Image
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

        // Функция сохранения графа в формате png при нажатии на кнопку
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
