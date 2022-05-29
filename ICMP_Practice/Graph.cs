using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ICMP_Practice
{
    // Класс вершин
    class Vertex
    {
        public string name; // Название вершины
        public int x, y;    // Координаты вершины

        // Конструктор для вершины
        public Vertex(int x, int y, string name)
        {
            this.x = x;
            this.y = y;
            this.name = name;
        }
    }

    // Класс ребер
    class Edge
    {
        // Вершины, которые соединяет ребро
        public Vertex v1, v2;

        // Конструктор ребра
        public Edge(Vertex v1, Vertex v2)
        {
            this.v1 = v1;
            this.v2 = v2;
        }
    }

    class DrawGraph
    {
        // Инициализация переменных для отрисовки картинки графа
        Bitmap bitmap;      // Класс массива битов картинки
        Pen blackPen;       // "Ручка", отвечающая за черный цвет
        Pen redPen;         // "Ручка", отвечающая за красный цвет
        Pen darkGoldPen;    // "Ручка", отвечающая за желтый цвет
        Graphics gr;        // Класс, отвечающий за рисование графа
        Font fo;            // Шрифт
        Brush br;           // Кисть
        PointF point;       // Центр рисовки 
        public int R = 14;  // Радиус окружности вершины

        // Функция инициализации bitmap
        public DrawGraph(int width, int height)
        {
            bitmap = new Bitmap(width, height);
            gr = Graphics.FromImage(bitmap);
            clearSheet();
            blackPen = new Pen(Color.Black);
            blackPen.Width = 2;
            redPen = new Pen(Color.Red);
            redPen.Width = 2;
            darkGoldPen = new Pen(Color.DarkGoldenrod);
            darkGoldPen.Width = 2;
            fo = new Font("Comic Sans MS", 9);
            br = Brushes.Black;
        }

        // Функция для получения bitmap
        public Bitmap GetBitmap()
        {
            return bitmap;
        }

        // Функция очистки bitmap
        public void clearSheet()
        {
            gr.Clear(Color.White);

        }
        
        // Функция отрисовки вершин
        public void drawVertex(int x, int y, string number)
        {
            StringFormat sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Center;
            
            // Рисуем эллипс с соответсвующими размерами и координатами
            gr.FillEllipse(Brushes.White, (x - R*4), (y - R), 8 * R, 2 * R);
            gr.DrawEllipse(blackPen, (x - R*4), (y - R), 8 * R, 2 * R);
            
            // Если длина строки больше 15, то это роутер, его название 
            // отрисовываем иначе
            if (number.Length > 15)
            {
                point = new PointF(x, y - 20);
            }
            else point = new PointF(x, y); // в ином случае это подсеть или устройство
            gr.DrawString(number, fo, br, point, sf);
        }

        
        public void drawALLGraph(List<Vertex> V, List<Edge> E)
        {
            //рисуем ребра
            for (int i = 0; i < E.Count; i++)
            {
                if (E[i].v1 == E[i].v2)
                {
                    gr.DrawArc(darkGoldPen, (E[i].v1.x - 2 * R), (E[i].v1.y - 2 * R), 2 * R, 2 * R, 90, 270);
                    point = new PointF(E[i].v1.x - (int)(2.75 * R), E[i].v1.y - (int)(2.75 * R));
                }
                else
                {
                    gr.DrawLine(darkGoldPen, E[i].v1.x, E[i].v1.y, E[i].v2.x, E[i].v2.y);
                    point = new PointF((E[i].v1.x + E[i].v2.x) / 2, (E[i].v1.y + E[i].v2.y) / 2);
                }
            }
            //рисуем вершины
            for (int i = 0; i < V.Count; i++)
            {
                drawVertex(V[i].x, V[i].y, V[i].name);
            }
        }
    }
}
