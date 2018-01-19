using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Numerics;

namespace Ex_1
{
    public partial class Form1 : Form
    {
        public int[,,,] array;
        double[,][,] b;
        
        public Form1()
        {
            InitializeComponent();
        }
        static Complex[] RecursiveFFT(Complex[] array)
        {
            int n = array.Length;
            if (n == 1)
            {
                return array;
            }

            var omega_n = new Complex(Math.Cos(2 * Math.PI / n), Math.Sin(2 * Math.PI / n));
            var omega = new Complex(1, 0);

            var even = new Complex[n / 2];
            for (int i = 0; i < n / 2; i++)
            {
                even[i] = array[2 * i];
            }

            var odd = new Complex[n / 2];
            for (int i = 0; i < n / 2; i++)
            {
                odd[i] = array[2 * i + 1];
            }

            var y0 = new Complex[n / 2];
            y0 = RecursiveFFT(even);

            var y1 = new Complex[n / 2];
            y1 = RecursiveFFT(odd);

            var y = new Complex[n];
            for (int k = 0; k < n / 2; k++)
            {
                y[k] = y0[k] + omega * y1[k];
                y[k + n / 2] = y0[k] - omega * y1[k];
                omega = omega * omega_n;
            }
            return y;
        }
        static double[] AddMirrored(double[] array)
        {
            int n = array.Length;

            var result = new double[2 * n];
            for (int i = 0; i < n; i++)  // n = temp.Length/2
            {
                result[i] = array[i];
                result[n + i] = array[n - i - 1];
            }

            return result;
        }
        static double[] Zeroes(double[] array)
        {
            int n = array.Length;

            var result = new double[2 * n];
            for (int i = 0; i < n; i++)
            {
                result[2 * i] = 0;
                result[2 * i + 1] = array[i];
            }

            return result;
        }
        public static double[] DCT(double[] array)
        {
            int n = array.Length;
            var coeff = new double[n];
            var array_complex = Zeroes(AddMirrored(array)).Select(x => new Complex(x, 0)).ToArray();
            var coeff1 = RecursiveFFT(array_complex).Take(n).Select(x => x.Real).ToArray();
            coeff[0] = coeff1[0] / (2 * n);
            for (int i = 1; i < n; i++)
            {
                coeff[i] = coeff1[i] / n;
            }
            return coeff;
        }
        public static double[] IDCT(double[] coeff)
        {
            int n = coeff.Length;
            var c_4n = new Complex[4 * n]; // здесь храним 4n коэффициентов DCT
            for (int i = 0; i < n; i++)
            {
                c_4n[i] = coeff[i];
            }
            for (int i = 1; i < n; i++)
            {
                c_4n[n + i] = -coeff[n - i];
            }
            c_4n[n] = 0;
            for (int i = 0; i < 2 * n; i++)
            {
                c_4n[2*n + i] = -c_4n[i];
            }

            // портим коэффициенты, чтобы правильно восстановить исходный массив
            for (int i = 0; i < 4 * n; i++)
            {
                if (i == 0 || i == 4 * n / 2)
                {
                    c_4n[i] = c_4n[i] * 2 * n;
                }
                else
                {
                    c_4n[i] = c_4n[i] * n;
                }
            }
            var temp1 = RecursiveFFT(c_4n.Select(x => new Complex(x.Imaginary, x.Real)).ToArray());
            var aa = temp1.Select(y => new Complex(y.Imaginary, y.Real) / (4 * n)).ToArray();
            var result = new double[n];
            for (int i = 0; i < n; i++)
            {
                result[i] = aa[2 * i + 1].Real;
            }
            return result;
        }
        public static double[,] TwoDCT(double[,] massiv)
        {
            int n = massiv.GetLength(1);
            double[] row = new double[n];
            double[] column = new double[n];
            double[,] coeff = new double[n, n];
            double[,] twocoeff = new double[n, n];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    row[j] = massiv[i, j];
                }
                row = DCT(row);
                for (int j = 0; j < n; j++)
                {
                    coeff[i, j] = row[j];
                }
            }
            //массив коэфф
            for (int j = 0; j < n; j++)
            {
                for (int i = 0; i < n; i++)
                {
                    column[i] = coeff[i, j];
                }
                column = DCT(column);
                for (int i = 0; i < n; i++)
                {
                    twocoeff[i, j] = column[i];
                }
            }
            return twocoeff;
        }
        public static double[,] TwoIDCT(double[,] array)
        {
            int n = array.GetLength(1);
            double[] row = new double[n];
            double[] column = new double[n];
            double[,] orig = new double[n, n];
            double[,] original = new double[n, n];
            for (int j = 0; j < n; j++)
            {
                for (int i = 0; i < n; i++)
                {
                    column[i] = array[i, j];
                }
                column = IDCT(column);
                for (int i = 0; i < n; i++)
                {
                    orig[i, j] = column[i];
                }
            }
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    row[j] = orig[i, j];
                }
                row = IDCT(row);
                for (int j = 0; j < n; j++)
                {
                    original[i, j] = row[j];
                }
            }
            return original;
        }
        private sbyte[,] ConvertArrayFromDoubleToSByte(double[,] x)
        {

            sbyte[,] a = new sbyte[x.GetLength(0), x.GetLength(1)];

            for (int i = 0; i < x.GetLength(0); i++)
            {
                for (int j = 0; j < x.GetLength(1); j++)
                {
                    a[i, j] = Convert.ToSByte(Math.Min(127, Math.Max(-128, x[i, j] / 2)));
                }
            }
            return a;
        }
        private double[,] ConvertArrayFromSByteToDouble(sbyte[,] x)
        {
            double[,] a = new double[x.GetLength(0), x.GetLength(1)];

            for (int i = 0; i < x.GetLength(0); i++)
            {
                for (int j = 0; j < x.GetLength(1); j++)
                {
                    a[i, j] = Convert.ToDouble(x[i, j]) * 2;
                }
            }
            return a;
        }
        private sbyte[,] Compress(sbyte[,] x, int n)
        {
            sbyte[,] a = new sbyte[x.GetLength(0), x.GetLength(1)];

            for (int i = 0; i < a.GetLength(0); i++)
            {
                for (int j = 0; j < a.GetLength(1); j++)
                {
                    a[i, j] = 0;
                }
            }
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    a[i, j] = x[i, j];
                }
            }

            return a;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("fekhvgf");
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Open Image";
            ofd.Filter = "JPG Image|*.jpg";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Bitmap bmp = new Bitmap(ofd.FileName);
                pictureBox1.Image = bmp;
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            Bitmap myBitmap = new Bitmap(pictureBox1.Image);

            for (int Xcount = 0; Xcount < myBitmap.Width; Xcount++)
            {
                for (int Ycount = 0; Ycount < myBitmap.Height; Ycount++)
                {
                    if ((Xcount+Ycount)%2==0)
                    myBitmap.SetPixel(Xcount, Ycount, Color.Black);
                }
            }
           pictureBox1.Image = myBitmap;
            //Console.WriteLine(myBitmap);
        }
        public static sbyte[,] ff(sbyte[] a, int s)
        {
            //int n = a.Length;
            var result = new sbyte[s, s];
            for (int k = 0; k < a.Length; k++)
            {
                int i = k / s;
                int j = k % s;
                result[i, j] = a[k];
            }
            return result;
        }
        private void button4_Click(object sender, EventArgs e)
        {
            int N, Nwidth, Nheight, n = 8;
            List<sbyte> list = new List<sbyte>();
            Bitmap bmp = new Bitmap(pictureBox1.Image);

            if (bmp.Width % n != 0)
            {
                Nwidth = bmp.Width / n + 1;
            }
            else Nwidth = bmp.Width / n;

            if (bmp.Height % n != 0)
            {
                Nheight = bmp.Height / n + 1;
            }
            else Nheight = bmp.Height / n;

            Bitmap bmp2 = new Bitmap(n*Nwidth, n*Nheight);
            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    bmp2.SetPixel(i,j, bmp.GetPixel(i, j));
                }
            }

            for (int i = 0; i < bmp2.Width; i++)
            {
                for (int j = bmp.Height; j < bmp2.Height; j++)
                {
                    bmp2.SetPixel(i, j, Color.Green);
                }
            }

            for (int i = bmp.Width; i < bmp2.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    bmp2.SetPixel(i, j, Color.Green);
                }
            }
       
            N = Nwidth * Nheight;
            array = new int[3, n, n, N];
            for (int m = 0; m<N; m++)
                {
                for (int i = 0; i<n; i++)
                    {
                    for (int j = 0; j<n; j++)
                        {
                            array[0, i, j, (m / Nwidth) * Nwidth + (m % Nwidth)] = bmp2.GetPixel(i + n* (m % Nwidth), j + n* (m / Nwidth)).R;
                            array[1, i, j, (m / Nwidth) * Nwidth + (m % Nwidth)] = bmp2.GetPixel(i + n* (m % Nwidth), j + n* (m / Nwidth)).G;
                            array[2, i, j, (m / Nwidth) * Nwidth + (m % Nwidth)] = bmp2.GetPixel(i + n* (m % Nwidth), j + n* (m / Nwidth)).B;
                        }
                    }   
                }
            double[,][,] mass = new double[3, N][,];

            for (int color = 0; color < 3; color++)
            {
                for (int m = 0; m < N; m++)
                {
                    mass[color, m] = new double[n, n];
                }
            }

            for (int m = 0; m < N; m++)
            {
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        mass[0, m][i, j] = array[0, i, j, m];
                        mass[1, m][i, j] = array[1, i, j, m];
                        mass[2, m][i, j] = array[2, i, j, m];
                    }
                }
            }

            sbyte[,][,] by = new sbyte[3, N][,];
            for (int m = 0; m < N; m++)
            {
                by[0, m] = ConvertArrayFromDoubleToSByte(TwoDCT(mass[0, m]));
                by[1, m] = ConvertArrayFromDoubleToSByte(TwoDCT(mass[1, m]));
                by[2, m] = ConvertArrayFromDoubleToSByte(TwoDCT(mass[2, m]));
            }
            for (int i = 0; i < by.GetLength(0); i++)
            {
                for (int j = 0; j < by.GetLength(1); j++)
                {
                    list.Add(Convert.ToSByte(by[i, j].Length));

                    for (int k = 0; k < n; k++)
                    {
                        for (int l = 0; l < n; l++)
                        {
                            list.Add(by[i, j][k, l]);
                        }
                    }
                }
            }

            string path = @"c:\temp\myimage.iri";

            // Delete the file if it exists.
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            //Create the file.
            using (var fs = new BinaryWriter(File.Create(path)))
            {
                fs.Write(Nwidth);
                fs.Write(Nheight);
                fs.Write(N);
                fs.Write(n);
                fs.Write(list.Count);
                for (int i = 0; i < list.Count; i++)
                {
                    fs.Write(list[i]);
                }
            }

            button5.Enabled = true;
        }
        private void button5_Click(object sender, EventArgs e)
        {
            //Open the stream and read it back.
            var list = new List<sbyte>();
            int Nwidth = 0;
            int Nheight = 0;
            int N = 0, n;
            string path = @"c:\temp\myimage.iri";
            using (var fs = new BinaryReader(File.OpenRead(path)))
            {
                Nwidth = fs.ReadInt32();
                Nheight = fs.ReadInt32();
                N = fs.ReadInt32();
                n = fs.ReadInt32();
                int count = fs.ReadInt32();

                for (int i = 0; i < count; i++)
                {
                    list.Add(fs.ReadSByte());
                }
            }

            Bitmap bmp2 = new Bitmap(n * Nwidth, n * Nheight);

            List<sbyte> list2 = new List<sbyte>();
            for (int i = 0; i < list.Count / (n * n + 1); i++)
            {
                list2.AddRange(list.Skip((n * n + 1) * i + 1).Take(list[(n * n + 1) * i]));
            }
            sbyte[] a1 = new sbyte[n * n];
            sbyte[,] a2 = new sbyte[n, n];
            sbyte[,][,] a3 = new sbyte[3, list2.Count / (3 * n * n)][,];

            for (int i = 0; i < list2.Count / (3 * n * n); i++)
            {
                a1 = list2.Skip(n * n * i).Take(n * n).ToArray();
                a2 = ff(a1, n);
                a3[0, i] = a2;
            }

            for (int i = list2.Count / (3 * n * n); i < 2 * list2.Count / (3 * n * n); i++)
            {
                a1 = list2.Skip(n * n * i).Take(n * n).ToArray();
                a2 = ff(a1, n);
                a3[1, i - list2.Count / (3 * n * n)] = a2;
            }

            for (int i = 2 * list2.Count / (3 * n * n); i < list2.Count / (n * n); i++)
            {
                a1 = list2.Skip(n * n * i).Take(n * n).ToArray();
                a2 = ff(a1, n);
                a3[2, i - 2 * list2.Count / (3 * n * n)] = a2;
            }

            double[,][,] a = new double[3, N][,];
            //sbyte[,][,] bb = new sbyte[3, N][,];

            for (int m = 0; m < N; m++)
            {
                a[0, m] = TwoIDCT(ConvertArrayFromSByteToDouble(a3[0, m]));
                a[1, m] = TwoIDCT(ConvertArrayFromSByteToDouble(a3[1, m]));
                a[2, m] = TwoIDCT(ConvertArrayFromSByteToDouble(a3[2, m]));
            }
            for (int m = 0; m < N; m++)
            {
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        int r = Convert.ToInt32(a[0, m][i, j]);
                        int g = Convert.ToInt32(a[1, m][i, j]);
                        int b = Convert.ToInt32(a[2, m][i, j]);

                        r = Math.Min(255, Math.Max(0, r));
                        g = Math.Min(255, Math.Max(0, g));
                        b = Math.Min(255, Math.Max(0, b));

                        bmp2.SetPixel(i + n * (m % Nwidth), j + n * (m / Nwidth),
                           Color.FromArgb(r, g, b));
                    }
                }
            }
            pictureBox1.Image = bmp2;
            //Console.WriteLine(bmp2.Width);
            //Console.WriteLine(bmp2.Height);
           
        }
    }
}

