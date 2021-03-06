using System.IO;
using System;
using System.Collections.Generic;
using System.Windows;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace ICMP_Practice
{
    public partial class MainWindow : Window
    {
        List<Router> routers = new List<Router>();
        SaveClass save_class = new SaveClass();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LaunchButton_Click(object sender, RoutedEventArgs e)
        {
            // Получаем диапазон IP
            string remoteHostsStr = IPRangeTextBox.Text.ToString();

            // Проверка, что строка не пустая
            if (remoteHostsStr == null)
            {
                MessageBox.Show("No hosts");
                return;
            }

            // Разделяем диапазон на 2 (начальный-конечный IP)
            string[] remoteHosts = remoteHostsStr.Split('-');
            if (remoteHosts.Length != 2)
            {
                MessageBox.Show("Not found separator");
                return;
            }

            // Задаем переменные начального и последнего IP
            string startAddr = remoteHosts[0];
            string endAddr = remoteHosts[1];

            // Разделяем начальный IP на октеты
            string[] octets = startAddr.Split('.');
            if (octets.Length != 4)
            {
                MessageBox.Show("Wrong start IP address");
                return;
            }

            // Проверяем октеты, чтобы они не превышали 255 и были не меньше 0
            int[] startAddrOctets = new int[octets.Length];
            for (int i = 0; i < 4; i++)
            {
                try
                {
                    startAddrOctets[i] = Convert.ToInt32(octets[i]);
                    if (startAddrOctets[i] > 255 && startAddrOctets[i] < 0)
                    {
                        MessageBox.Show("Wrong start IP address");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    return;
                }
            }

            // Разделяем на октеты конечный IP
            octets = endAddr.Split('.');
            if (octets.Length != 4)
            {
                MessageBox.Show("Wrong end IP address");
                return;
            }

            // Проверяем октеты, чтобы они не превышали 255 и были не меньше 0
            int[] endAddrOctets = new int[octets.Length];
            for (int i = 0; i < 4; i++)
            {
                try
                {
                    endAddrOctets[i] = Convert.ToInt32(octets[i]);
                    if (endAddrOctets[i] > 255 && endAddrOctets[i] < 0)
                    {
                        MessageBox.Show("Wrong end IP address");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
            }

            // Создаем списки устройств и роутеров
            List<Device> devices = new List<Device>();
            List<Router> routers = new List<Router>();

            // Расчет максимально допустомого количетсва устройств
            int devicesCount =
                (endAddrOctets[0] - startAddrOctets[0]) * 255 * 255 * 255 +
                (endAddrOctets[1] - startAddrOctets[1]) * 255 * 255 +
                (endAddrOctets[2] - startAddrOctets[2]) * 255 +
                (endAddrOctets[3] - startAddrOctets[3]);

            int[] lastAddrOctets = startAddrOctets;

            // Добавляем первое устройство с начальным IP
            devices.Add(new Device(String.Format("{0}.{1}.{2}.{3}", startAddrOctets[0], startAddrOctets[1], startAddrOctets[2], startAddrOctets[3])));

            // Циклично добавляем возможные устройства в соответствии с диапазоном
            for (int i = 0; i < devicesCount; i++)
            {
                lastAddrOctets[3]++;
                if (lastAddrOctets[3] > 255)
                {
                    lastAddrOctets[3] = 1;
                    lastAddrOctets[2]++;
                    if (lastAddrOctets[2] > 255)
                    {
                        lastAddrOctets[2] = 0;
                        lastAddrOctets[1]++;
                        if (lastAddrOctets[1] > 255)
                        {
                            lastAddrOctets[1] = 0;
                            lastAddrOctets[0]++;
                            if (lastAddrOctets[0] > 255)
                            {
                                MessageBox.Show("Wrong number (must be in [0,255])");
                                continue;
                            }
                        }
                    }
                }
                string newIp = String.Format("{0}.{1}.{2}.{3}", lastAddrOctets[0], lastAddrOctets[1], lastAddrOctets[2], lastAddrOctets[3]);
                devices.Add(new Device(newIp));
            }

            // Получаем строку с роутерами
            string routersStr = RouterIPsTextBox.Text;




            // Разделяем роутеры и распределяем подсети к роутерам и устройства к своим возможным подсетям
            try
            {
                // Получение данных, разделяя по фигурным скобкам "{данные}"
                Regex regular = new Regex("{(.*?)}", RegexOptions.Compiled);
                string[] routersD = regular.Split(routersStr); // получаем ip-адреса роутеров


                foreach (string router in routersD)
                {
                    if (router != "")
                    {
                        Router routerClass = new Router();
                        routerClass.name = router;
                        string[] subnetList = router.Split(',');

                        foreach (string subnet in subnetList)
                        {
                            Subnet subnetClass = new Subnet(subnet);
                            routerClass.subnets.Add(subnetClass);
                            Device removeDevice = null;
                            foreach (Device device in devices)
                            {
                                if (device.ip == router)
                                {
                                    removeDevice = device;
                                    continue;
                                }
                                int[] devStartOctets = device.getStartOctets();
                                int[] routerStartOctets = subnetClass.getStartOctets();
                                subnetClass.ip = routerStartOctets[0] + "." + routerStartOctets[1] + "." + routerStartOctets[2] + "." + "0";
                                if (devStartOctets[0] == routerStartOctets[0] &&
                                    devStartOctets[1] == routerStartOctets[1] &&
                                    devStartOctets[2] == routerStartOctets[2])
                                {
                                    subnetClass.devices.Add(device);
                                }
                            }
                            devices.Remove(removeDevice);
                        }
                        routers.Add(routerClass);
                    }
                }
                // Сохраняем введенные данные в файл
                SaveData();

                // Посылаем запросы для каждого роутера
                foreach (Router r in routers)
                {
                    foreach (Subnet item in r.subnets)
                    {
                        item.StartPing();
                    }
                }
                PingedDevicesListView.Items.Clear();

                // Заполняем таблицу опрошенных устройств
                foreach (Router r in routers)
                {
                    foreach (Subnet subnet in r.subnets)
                    {
                        foreach (Device device in subnet.devices)
                        {
                            IPInfo info = new IPInfo(device.ip, device.pinged);
                            PingedDevicesListView.Items.Add(info);
                        }
                    }
                }
                // Рисуем граф сети
                GraphVisual graphVisual = new GraphVisual(routers);
                graphVisual.Show();
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // Функция сохранения введенных данных
        void SaveData()
        {
            // Если строки диапазона и списка роутеров не пустые, то будет сохранять
            if (IPRangeTextBox.Text.Length == 0 && RouterIPsTextBox.Text.Length == 0)
            {
                return;
            }
            // Создается вспомогательная переменная класса SaveClass
            SaveClass save = new SaveClass();
            save.subnets = RouterIPsTextBox.Text;
            save.ip_range = IPRangeTextBox.Text;

            // С помощью библиотеки Newtonsoft Json сериализуем объект и записывам в файл
            // Файл будет находиться в папке, где расположена программа
            try
            {
                string json = JsonConvert.SerializeObject(save, Formatting.Indented);
                File.WriteAllText("Data.json", json);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        // Функция загрузки данных
        void LoadData()
        {
            // Создаем переменную для приема данных
            string json = "";
            try
            {
                // Читаем данные из файла
                json = File.ReadAllText("Data.json");
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR:\n" + ex.Message);
                return;
            }
            // Если данные есть, то пытаемся их загрузить
            if (json.Length == 0)
            {
                return;
            }

            try
            {
                save_class = JsonConvert.DeserializeObject<SaveClass>(json);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            IPRangeTextBox.Text = save_class.ip_range;
            RouterIPsTextBox.Text = save_class.subnets;
        }
        private void RemoveIPButton_Click(object sender, RoutedEventArgs e)
        {

        }

        // Функция, отвечающая за отображение примера с двумя роутерами
        // Все данные заполнены вручную
        private void LaunchExmapleButton_Click(object sender, RoutedEventArgs e)
        {
            // Очистка массива роутеров
            routers.Clear();

            // Инициализация роутера 1 и добавление в него подсетей и устройств
            Router router1 = new Router();
            router1.name = "192.168.40.1,192.168.41.1,192.168.42.1";
            router1.subnets = new List<Subnet>();

            {
                Subnet subnet = new Subnet("192.168.40.0");
                subnet.devices = new List<Device>();

                Device device1 = new Device("192.168.40.12");
                Device device2 = new Device("192.168.40.24");
                Device device3 = new Device("192.168.40.54");
                device1.pinged = true;
                device2.pinged = true;
                device3.pinged = true;
                subnet.devices.Add(device1);
                subnet.devices.Add(device2);
                subnet.devices.Add(device3);

                router1.subnets.Add(subnet);
            }
            {
                Subnet subnet = new Subnet("192.168.41.0");
                subnet.devices = new List<Device>();

                Device device1 = new Device("192.168.41.14");
                Device device2 = new Device("192.168.41.62");
                device1.pinged = true;
                device2.pinged = true;

                subnet.devices.Add(device1);
                subnet.devices.Add(device2);

                router1.subnets.Add(subnet);
            }
            {
                Subnet subnet = new Subnet("192.168.42.0");
                subnet.devices = new List<Device>();

                Device device1 = new Device("192.168.42.123");
                Device device2 = new Device("192.168.42.86");
                device1.pinged = true;
                device2.pinged = true;

                subnet.devices.Add(device1);
                subnet.devices.Add(device2);

                router1.subnets.Add(subnet);
            }

            // Инициализация роутера 2 и добавление в него подсетей и устройств
            Router router2 = new Router();
            router2.name = "192.168.45.1,192.168.46.1,192.168.47.1";
            router2.subnets = new List<Subnet>();

            {
                Subnet subnet = new Subnet("192.168.45.0");
                subnet.devices = new List<Device>();

                Device device2 = new Device("192.168.45.24");
                Device device3 = new Device("192.168.45.54");
                device2.pinged = true;
                device3.pinged = true;

                subnet.devices.Add(device2);
                subnet.devices.Add(device3);

                router2.subnets.Add(subnet);
            }
            {
                Subnet subnet = new Subnet("192.168.46.0");
                subnet.devices = new List<Device>();

                Device device1 = new Device("192.168.46.14");
                subnet.devices.Add(device1);

                router2.subnets.Add(subnet);
            }
            {
                Subnet subnet = new Subnet("192.168.47.0");
                subnet.devices = new List<Device>();

                Device device2 = new Device("192.168.47.86");
                subnet.devices.Add(device2);

                router2.subnets.Add(subnet);
            }

            // Добавление роутеров
            routers.Add(router1);
            routers.Add(router2);

            // Отрисовка графа в новом окне
            GraphVisual graphVisual = new GraphVisual(routers);
            graphVisual.Show();
        }

        // Загрузка данных из файла по нажатию кнопки 
        private void LoadDataButton_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }
    }
}
