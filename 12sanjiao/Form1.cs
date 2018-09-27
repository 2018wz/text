using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace _12sanjiao
{
    struct coor
    {
        public double x, y, z;
    }
    struct TinLine
    {
        public int id1, id2;
        public coor Begin, End;
    }
    struct TinLine1
    {
        public PointF Begin, End;
    }
    struct tagTri
    {
        public int id1, id2, id3;
    }
    public partial class Form1 : Form
    {
        List<coor> zbArray = new List<coor>();
        List<TinLine> linesArray = new List<TinLine>();
        List<tagTri> VertexArray = new List<tagTri>();
        List<double> VolumeArray = new List<double>();

        public Form1()
        {
            InitializeComponent();
        }

        private void 打开ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Txt数据(*.txt)|*.txt";
            if (open.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            StreamReader sr = new StreamReader(open.FileName, Encoding.Default);
            coor zb = new coor();
            while (true)
            {
                string strLine = sr.ReadLine();
                if (strLine != null)
                {
                    string[] H = strLine.Split(' ');
                    zb.x = Convert.ToDouble(H[1]);
                    zb.y = Convert.ToDouble(H[2]);
                    zb.z = Convert.ToDouble(H[3]);
                    zbArray.Add(zb);
                }
                else break;
            }
            for (int i = 0; i < zbArray.Count; i++)
            {
                dataGridView1.Rows.Add();
                dataGridView1.Rows[i].Cells[0].Value = zbArray[i].x;
                dataGridView1.Rows[i].Cells[1].Value = zbArray[i].y;
                dataGridView1.Rows[i].Cells[2].Value = zbArray[i].z;
            }
        }
        bool isSameLine(TinLine line1, TinLine line2)
        {
            if (((line1.id1 == line2.id1) && (line1.id2 == line2.id2)) || ((line1.id1 == line2.id2) && (line1.id2 == line2.id1)))
                return true;
            return false;
        }
        bool isTwice(TinLine _line)
        {
            int lineCount = 0;
            for (int ii = 0; ii < linesArray.Count; ii++)
            {
                TinLine pTempLine = linesArray[ii];
                if (isSameLine(pTempLine, _line))
                    lineCount++;
            }
            if (lineCount == 2)
                return true;
            return false;
        }
        double Distance(coor a, coor b)
        {
            return Math.Sqrt((b.x - a.x) * (b.x - a.x) + (b.y - a.y) * (b.y - a.y));
        }
        int ZuoYou(coor a, coor b, coor c)
        {
            int youbian;
            double S;
            S = (a.x - c.x) * (b.y - c.y) - (a.y - c.y) * (b.x - c.x);
            if (S > 0)
            {
                youbian = 1;
            }
            else if (S < 0)
            {
                youbian = -1;
            }
            else
            {
                youbian = 0;
            }
            return youbian;
        }
        double Angle(coor a, coor b, coor c)
        {
            double ang;
            double l1 = Math.Sqrt((b.x - c.x) * (b.x - c.x) + (b.y - c.y) * (b.y - c.y));
            double l2 = Math.Sqrt((a.x - c.x) * (a.x - c.x) + (a.y - c.y) * (a.y - c.y));
            double l3 = Math.Sqrt((b.x - a.x) * (b.x - a.x) + (b.y - a.y) * (b.y - a.y));
            ang = Math.Acos((l1 * l1 + l2 * l2 - l3 * l3) / (2 * l1 * l2));
            return ang;
        }
        double Volume(int id1, int id2, int id3, double Height)
        {
            //coor zb = new coor();
            coor a = zbArray[id1];
            coor b = zbArray[id2];
            coor c = zbArray[id3];
            //double h1 = a.z;
            //double h2 = b.z;
            //double h3 = c.z;
            //double minHeight = a.z;
            //double maxHeight = a.z;
            #region
            //double maxHeight = (a.z > b.z ? a.z : b.z) > c.z ? (a.z > b.z ? a.z : b.z) : c.z;
            //double minHeight = (a.z < b.z ? a.z : b.z) < c.z ? (a.z < b.z ? a.z : b.z) : c.z;
            //double midHeight;            
            //if (maxHeight==a.z)
            //{                
            //}
            //if (a.z<b.z&&b.z<c.z||c.z<b.z&&b.z<a.z)
            //{
            //    midHeight = b.z;
            //}
            //else if (b.z<a.z&&a.z<c.z||c.z<a.z&&a.z<b.z)
            //{
            //    midHeight = a.z;
            //}
            //else
            //{
            //    midHeight = c.z;
            //}
            //double S = ((a.x * b.y - b.x * a.y) + (b.x * c.y - c.x * b.y) + (c.x * a.y - a.x * c.y)) / 2;
            //double V1 = (S * maxHeight) / 3;
            #endregion
            double ab = Distance(a, b);
            double ac = Distance(a, c);
            double bc = Distance(b, c);
            double p = (ab + ac + bc) / 2;
            double S = Math.Sqrt(p * (p - ab) * (p - ac) * (p - bc));
            double V1 = (S * (a.z + b.z + c.z) / 3) - Height * S;
            return V1;
        }
        private void 画图ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //定义与第一点最近的点
            double minDist = Distance(zbArray[0], zbArray[1]);
            double dis;
            int nearPtId = 1;
            for (int i = 0; i < zbArray.Count; i++)
            {
                dis = Distance(zbArray[0], zbArray[i]);
                if (dis < minDist)
                {
                    minDist = dis;
                    nearPtId = i;
                }
            }
            //if (nearPtId == -1)
            //    return;
            //将第一条边反向已进行三角形扩展
            TinLine tl = new TinLine();
            tl.id1 = 0;
            tl.id2 = nearPtId;
            tl.Begin = zbArray[0];
            tl.End = zbArray[nearPtId];
            linesArray.Add(tl);
            for (int j = 0; j < linesArray.Count; j++)
            {
                //TinLine line = linesArray[j];
                //if (isTwice(line))
                //    continue;
                double ang;
                double maxAngs = double.MinValue;
                int maxAngPtId = -1;
                bool ok = false;
                for (int i = 0; i < zbArray.Count; i++)
                {
                    int youbian;
                    //判断第三点与前两点的位置关系
                    youbian = ZuoYou(linesArray[j].Begin, linesArray[j].End, zbArray[i]);
                    //youbian = ZuoYou(zbArray[linesArray[j].id1], zbArray[linesArray[j].id2], zbArray[i]);
                    if (youbian == 1)
                    {
                        //获取角度最大点
                        ang = Angle(linesArray[j].Begin, linesArray[j].End, zbArray[i]);
                        //ang = Angle(zbArray[linesArray[j].id1], zbArray[linesArray[j].id2], zbArray[i]);
                        if (ang > maxAngs)
                        {
                            maxAngs = ang;
                            maxAngPtId = i;
                        }
                        ok = true;
                    }
                }
                if (ok == true)
                {
                    TinLine tline1 = new TinLine();
                    TinLine tline2 = new TinLine();
                    //将新生成两条边添加入集合中
                    int t1 = 0;
                    int t2 = 0;
                    tline1.id1 = linesArray[j].id1;
                    tline1.id2 = maxAngPtId;
                    tline2.id1 = linesArray[j].id2;
                    tline2.id2 = maxAngPtId;

                    tline1.Begin = linesArray[j].Begin;
                    tline1.End = zbArray[maxAngPtId];
                    tline2.Begin = zbArray[maxAngPtId];
                    tline2.End = linesArray[j].End;
                    //将三角顶点的编号添加顶点集合中
                    tagTri Vertex = new tagTri();
                    Vertex.id1 = linesArray[j].id1;
                    Vertex.id2 = linesArray[j].id2;
                    Vertex.id3 = maxAngPtId;
                    for (int i = 0; i < linesArray.Count; i++)
                    {
                        if ((tline1.Begin.x == linesArray[i].Begin.x && tline1.End.x == linesArray[i].End.x) ||
                          (tline1.Begin.x == linesArray[i].End.x && tline1.End.x == linesArray[i].Begin.x))
                        {
                            t1 = 1;
                        }
                        if ((tline2.Begin.x == linesArray[i].Begin.x && tline2.End.x == linesArray[i].End.x) ||
                            (tline2.Begin.x == linesArray[i].End.x && tline2.End.x == linesArray[i].Begin.x))
                        {
                            t2 = 1;
                        }

                        //if ((tline1.id1 == linesArray[i].id1 && tline1.id2 == linesArray[i].id2) ||
                        //  (tline1.id1 == linesArray[i].id2 && tline1.id2 == linesArray[i].id1))
                        //{
                        //    t1 = 1;
                        //}
                        //if ((tline2.id1 == linesArray[i].id1 && tline2.id2 == linesArray[i].id2) ||
                        //  (tline2.id1 == linesArray[i].id2 && tline2.id2 == linesArray[i].id1))
                        //{
                        //    t2 = 1;
                        //}

                    }
                    if (t1 == 0)
                    {
                        linesArray.Add(tline1);
                        VertexArray.Add(Vertex);
                    }
                    if (t2 == 0)
                    {
                        linesArray.Add(tline2);
                        VertexArray.Add(Vertex);
                    }
                    if (t1 == 0 && t2 == 0)
                    {
                        VertexArray.Remove(Vertex);
                    }
                }
            }
            #region
            //MessageBox.Show("Array length=" + linesArray.Count);
            //MessageBox.Show("VertexArray length=" + VertexArray.Count);
            //double minX = Double.MaxValue;//zbArray[0].x;
            //double minY = Double.MaxValue;// zbArray[0].y;
            //double maxX = Double.MinValue;//szbArray[0].x;
            //double maxY = Double.MinValue;// zbArray[0].y;
            //for (int i = 0; i < zbArray.Count; i++)
            //{
            //    double x0 = zbArray[i].x;
            //    double y0 = zbArray[i].y;
            //    if (x0 > maxX)
            //    {
            //        maxX = x0;
            //    }
            //    if (x0 < minX)
            //    {
            //        minX = x0;
            //    }
            //    if (y0 > maxY)
            //    {
            //        maxY = y0;
            //    }
            //    if (y0 < minY)
            //    {
            //        minY = y0;
            //    }
            //}
            //double MapX = maxX - minX;
            //double MapY = maxY - minY;
            //double WinX = panel1.Width - 10;
            //double WinY = panel1.Height - 10;
            //double ScaleX = MapX / WinX;
            //double ScaleY = MapY / WinY;

            //TinLine1 tl1 = new TinLine1();
            //Graphics g = panel1.CreateGraphics();

            //panel1.Refresh();
            //Pen pen = new Pen(Color.Red, 1);
            //for (int i = 0; i < linesArray.Count; i++)
            //{
            //    //tl1.Begin.X = (float)((zbArray[linesArray[i].id1].x - minX) / ScaleX);
            //    //tl1.Begin.Y = (float)(WinY - (zbArray[linesArray[i].id1].y - minY) / ScaleY);
            //    //tl1.End.X = (float)((zbArray[linesArray[i].id2].x - minX) / ScaleX);
            //    //tl1.End.Y = (float)(WinY - (zbArray[linesArray[i].id2].y - minY) / ScaleY);

            //    tl1.Begin.X = (float)((linesArray[i].Begin.x - minX) / ScaleX);
            //    tl1.Begin.Y = (float)(WinY - (linesArray[i].Begin.y - minY) / ScaleY);
            //    tl1.End.X = (float)((linesArray[i].End.x - minX) / ScaleX);
            //    tl1.End.Y = (float)(WinY - (linesArray[i].End.y - minY) / ScaleY);
            //    g.DrawLine(pen, tl1.Begin, tl1.End);
            //    if (i<zbArray.Count)
            //    {
            //        float x = (float)((zbArray[i].x - minX) / ScaleX);
            //        float y = (float)(WinY - (zbArray[i].y - minY) / ScaleY);
            //        g.FillEllipse(Brushes.Black, x-3 ,y-3 , 6, 6);
            //    }
            //} 
            #endregion
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            double minX = Double.MaxValue;//zbArray[0].x;
            double minY = Double.MaxValue;// zbArray[0].y;
            double maxX = Double.MinValue;//szbArray[0].x;
            double maxY = Double.MinValue;// zbArray[0].y;
            for (int i = 0; i < zbArray.Count; i++)
            {
                double x0 = zbArray[i].x;
                double y0 = zbArray[i].y;
                if (x0 > maxX)
                {
                    maxX = x0;
                }
                if (x0 < minX)
                {
                    minX = x0;
                }
                if (y0 > maxY)
                {
                    maxY = y0;
                }
                if (y0 < minY)
                {
                    minY = y0;
                }
            }
            double MapX = maxX - minX;
            double MapY = maxY - minY;
            double WinX = panel1.Width - 10;
            double WinY = panel1.Height - 10;
            double ScaleX = MapX / WinX;
            double ScaleY = MapY / WinY;
            TinLine1 tl1 = new TinLine1();
            Graphics g = panel1.CreateGraphics();
            panel1.Refresh();
            Pen pen = new Pen(Color.Red, 1);
            for (int i = 0; i < linesArray.Count; i++)
            {
                tl1.Begin.X = (float)((linesArray[i].Begin.x - minX) / ScaleX);
                tl1.Begin.Y = (float)(WinY - (linesArray[i].Begin.y - minY) / ScaleY);
                tl1.End.X = (float)((linesArray[i].End.x - minX) / ScaleX);
                tl1.End.Y = (float)(WinY - (linesArray[i].End.y - minY) / ScaleY);
                g.DrawLine(pen, tl1.Begin, tl1.End);
                if (i < zbArray.Count)
                {
                    float x = (float)((zbArray[i].x - minX) / ScaleX);
                    float y = (float)(WinY - (zbArray[i].y - minY) / ScaleY);
                    g.FillEllipse(Brushes.Black, x - 3, y - 3, 6, 6);
                }
            }
        }
        private void 计算体积ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Add(VertexArray.Count - zbArray.Count);
            //tagTri vertex = new tagTri();
            try
            {
                for (int i = 0; i < VertexArray.Count; i++)
                {

                    int id1 = VertexArray[i].id1;
                    int id2 = VertexArray[i].id2;
                    int id3 = VertexArray[i].id3;
                    double height = Convert.ToDouble(textBox1.Text);
                    double V = Volume(id1, id2, id3, height);
                    V = Math.Round(V, 3);
                    dataGridView1.Rows[i].Cells[3].Value = id1 + "_" + id2 + "_" + id3;
                    dataGridView1.Rows[i].Cells[4].Value = V;
                    VolumeArray.Add(V);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("请输入起算高程");
                return;
            }

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// 保存为文本格式或者表格格式
        /// </summary>
        /// <param name="VertexArray">三角形顶点点号</param>
        /// <param name="VolumeArray">对应三角形的体积</param>
        void ExportDataTOtxtOrExcell(List<tagTri> VertexArray, List<double> VolumeArray, string FilePath)
        {
            FileStream fs = new FileStream(FilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            for (int i = 0; i < VolumeArray.Count; i++)
            {
                string S = VertexArray[i].id1 + "-" + VertexArray[i].id2 + "-" + VertexArray[i].id3 + "\t" + VolumeArray[i];
                sw.WriteLine(S);
            }
            sw.Close();
            fs.Close();
        }
        /// <summary>
        /// dxf输出起始函数
        /// </summary>
        /// <param name="sw">以一定编码定义的数据流</param>
        public void WriterHeader(StreamWriter sw)
        {
            sw.WriteLine("  0");
            sw.WriteLine("SECTION");
            sw.WriteLine("  2");
            sw.WriteLine("ENTITIES");
        }
        public void WriterPoint(StreamWriter sw, double x, double y, double z)
        {
            sw.WriteLine("  0");
            sw.WriteLine("POINT");
            sw.WriteLine("  8");
            sw.WriteLine("0");
            sw.WriteLine(" 10");
            sw.WriteLine(x.ToString());
            sw.WriteLine(" 20");
            sw.WriteLine(y.ToString());
            sw.WriteLine(" 30");
            sw.WriteLine(z.ToString());
        }
        //public void WriterCircle(StreamWriter sw, double x, double y, double z)
        //{
        //    sw.WriteLine("  0");
        //    sw.WriteLine("CIRCLE");
        //    sw.WriteLine("  8");
        //    sw.WriteLine("0");
        //    sw.WriteLine(" 10");
        //    sw.WriteLine(x.ToString());
        //    sw.WriteLine(" 20");
        //    sw.WriteLine(y.ToString());
        //    sw.WriteLine(" 30");
        //    sw.WriteLine(z.ToString());
        //    sw.WriteLine(" 40");
        //    sw.WriteLine("1");
        //}
        public void WriterLine(StreamWriter sw, double x, double y, double z, double x1, double y1, double z1)
        {
            sw.WriteLine("  0");
            sw.WriteLine("LINE");
            sw.WriteLine("  8");
            sw.WriteLine("0");
            sw.WriteLine(" 10");
            sw.WriteLine(x.ToString());
            sw.WriteLine(" 20");
            sw.WriteLine(y.ToString());
            sw.WriteLine(" 30");
            sw.WriteLine(z.ToString());
            sw.WriteLine(" 11");
            sw.WriteLine(x1.ToString());
            sw.WriteLine(" 21");
            sw.WriteLine(y1.ToString());
            sw.WriteLine(" 31");
            sw.WriteLine(z1.ToString());
        }
        //public void WriterText(StreamWriter sw, double x, double y, double z, string Buffer)
        //{
        //    sw.WriteLine("  0");
        //    sw.WriteLine("TEXT");
        //    sw.WriteLine("  8");
        //    sw.WriteLine("0");
        //    sw.WriteLine(" 10");
        //    sw.WriteLine(x.ToString());
        //    sw.WriteLine(" 20");
        //    sw.WriteLine(y.ToString());
        //    sw.WriteLine(" 30");
        //    sw.WriteLine(z.ToString());
        //    sw.WriteLine(" 40");
        //    sw.WriteLine("2");
        //    sw.WriteLine(" 1");
        //    sw.WriteLine(Buffer);
        //    sw.WriteLine(" 50");
        //    sw.WriteLine("45");
        //    sw.WriteLine(" 41");
        //    sw.WriteLine("1");
        //    sw.WriteLine("  7");
        //    sw.WriteLine("Standard");
        //}
        public void WriterEof(StreamWriter sw)
        {
            sw.WriteLine("  0");
            sw.WriteLine("ENDSEC");
            sw.WriteLine("  0");
            sw.WriteLine("EOF");
        }
        void ExportToDxf(List<coor> zbArray, List<TinLine> linesArray, string FilePath)
        {
            FileStream fs = new FileStream(FilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            WriterHeader(sw);
            for (int i = 0; i < linesArray.Count; i++)
            {
                if (i < zbArray.Count)
                {
                    WriterPoint(sw, zbArray[i].x, zbArray[i].y, zbArray[i].z);
                    //WriterCircle(sw, zbArray[i].x, zbArray[i].y, zbArray[i].z);
                    // WriterText(sw, zbArray[i].x, zbArray[i].y, zbArray[i].z, Convert.ToString(i));
                }
                WriterLine(sw, linesArray[i].Begin.x, linesArray[i].Begin.y, linesArray[i].Begin.z, linesArray[i].End.x, linesArray[i].End.y, linesArray[i].End.z);
            }
            WriterEof(sw);
            sw.Flush();
            sw.Close();
            fs.Close();
        }
        private void 保存ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "txt文件|*.txt|xls文件|*.xls|dxf文件|*.dxf";
            if (sfd.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            string path = sfd.FileName;
            string pathEit = path.Substring(path.LastIndexOf(".") + 1);
            if (pathEit == "txt" || pathEit == "xls")
            {
                ExportDataTOtxtOrExcell(VertexArray, VolumeArray, path);
            }
            else
            {
                ExportToDxf(zbArray, linesArray, path);
            }
        }
    }
}
