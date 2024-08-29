using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PolygonAreaCalculator
{
    public partial class PolygonAreaCalculator : Form
    {
        private List<PointF> vertices = new List<PointF>();
        private List<List<PointF>> polygons = new List<List<PointF>>();
        private bool isDrawing = true;
        private Image loadedImage;

        public PolygonAreaCalculator()
        {
            this.Text = "导入图片并计算鼠标圈定图形的面积百分比";
            this.DoubleBuffered = true;
            this.MouseClick += new MouseEventHandler(OnMouseClick);
            this.Paint += new PaintEventHandler(OnPaint);
            this.KeyDown += new KeyEventHandler(OnKeyDown);

            // 添加一个菜单栏用于加载图片
            MenuStrip menuStrip = new MenuStrip();
            ToolStripMenuItem fileMenuItem = new ToolStripMenuItem("文件");
            ToolStripMenuItem openMenuItem = new ToolStripMenuItem("打开图片");
            openMenuItem.Click += new EventHandler(OnOpenImage);
            fileMenuItem.DropDownItems.Add(openMenuItem);
            menuStrip.Items.Add(fileMenuItem);
            this.MainMenuStrip = menuStrip;
            this.Controls.Add(menuStrip);
        }

        private void OnOpenImage(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "图片文件|*.jpg;*.jpeg;*.png;*.bmp"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                loadedImage = Image.FromFile(openFileDialog.FileName);
                this.Invalidate(); // 重新绘制窗口
            }
        }

        private void OnMouseClick(object sender, MouseEventArgs e)
        {
            if (isDrawing && loadedImage != null)
            {
                vertices.Add(new PointF(e.X, e.Y));
                this.Invalidate(); // 重新绘制窗口
            }
        }

        private void OnPaint(object sender, PaintEventArgs e)
        {
            // 绘制导入的图片
            if (loadedImage != null)
            {
                e.Graphics.DrawImage(loadedImage, 0, 0, this.ClientSize.Width, this.ClientSize.Height);
            }

            // 绘制当前正在圈定的多边形
            if (vertices.Count > 1)
            {
                e.Graphics.DrawPolygon(Pens.Black, vertices.ToArray());
            }

            // 绘制已完成的多边形
            foreach (var polygon in polygons)
            {
                e.Graphics.DrawPolygon(Pens.Red, polygon.ToArray());
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && isDrawing)
            {
                // 完成当前图形
                polygons.Add(new List<PointF>(vertices));
                vertices.Clear();
                this.Invalidate();

                // 如果已经圈定了两个多边形，计算面积百分比
                if (polygons.Count == 2)
                {
                    isDrawing = false;
                    double area1 = CalculatePolygonArea(polygons[0]);
                    double area2 = CalculatePolygonArea(polygons[1]);
                    double ratio = area1 / area2 * 100.0;

                    MessageBox.Show($"图形1的面积: {area1}\n图形2的面积: {area2}\n面积比: {ratio}%");
                }
            }
            else if (e.KeyCode == Keys.Escape)
            {
                // 重置
                vertices.Clear();
                polygons.Clear();
                isDrawing = true;
                this.Invalidate();
            }
        }

        private double CalculatePolygonArea(List<PointF> vertices)
        {
            int n = vertices.Count;
            double area = 0;

            for (int i = 0; i < n; i++)
            {
                PointF p1 = vertices[i];
                PointF p2 = vertices[(i + 1) % n]; // 使用模数循环处理最后一个顶点与第一个顶点的连接

                area += (p1.X * p2.Y - p2.X * p1.Y);
            }

            return Math.Abs(area) / 2.0;
        }




    }
}
