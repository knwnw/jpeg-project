using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FDCT_11_Multiplications
{
    class Program
    {
        static double[] FDCT(double[] x)
        {
            var c = new double[x.Length];
            int N = 8;

            // Промежуточные массивы, в которые будут записываться результаты stage1, 2 и 3
            var stage1 = new double[x.Length];
            var stage2 = new double[x.Length];
            var stage3 = new double[x.Length];
            var stage4 = new double[x.Length];

            // STAGE 1
            stage1[0] = x[0] + x[7];
            stage1[1] = x[1] + x[6];
            stage1[2] = x[2] + x[5];
            stage1[3] = x[3] + x[4];
            stage1[4] = x[3] - x[4];
            stage1[5] = x[2] - x[5];
            stage1[6] = x[1] - x[6];
            stage1[7] = x[0] - x[7];

            // STAGE 2
            stage2[0] = stage1[0] + stage1[3];
            stage2[1] = stage1[1] + stage1[2];
            stage2[2] = stage1[1] - stage1[2];
            stage2[3] = stage1[0] - stage1[3];
            stage2[4] = stage1[4] * Math.Cos(3 * Math.PI / (2 * N)) + stage1[7] * Math.Sin(3 * Math.PI / (2 * N));
            stage2[5] = stage1[5] * Math.Cos(Math.PI / (2 * N)) + stage1[6] * Math.Sin(Math.PI / (2 * N));
            stage2[6] = -stage1[5] * Math.Sin(Math.PI / (2 * N)) + stage1[6] * Math.Cos(Math.PI / (2 * N));
            stage2[7] = -stage1[4] * Math.Sin(3 * Math.PI / (2 * N)) + stage1[7] * Math.Cos(3 * Math.PI / (2 * N));

            // STAGE 3
            stage3[0] = stage2[0] + stage2[1];
            stage3[1] = stage2[0] - stage2[1];
            stage3[2] = stage2[2] * Math.Sqrt(2) * Math.Cos(6 * Math.PI / (2 * N)) + stage2[3] * Math.Sqrt(2) * Math.Sin(6 * Math.PI / (2 * N));
            stage3[3] = -stage2[2] * Math.Sqrt(2) * Math.Sin(6 * Math.PI / (2 * N)) + stage2[3] * Math.Sqrt(2) * Math.Cos(6 * Math.PI / (2 * N));
            stage3[4] = stage2[4] + stage2[6];
            stage3[5] = -stage2[5] + stage2[7];
            stage3[6] = stage2[4] - stage2[6];
            stage3[7] = stage2[5] + stage2[7];

            // STAGE 4
            stage4[0] = stage3[0];
            stage4[1] = stage3[1];
            stage4[2] = stage3[2];
            stage4[3] = stage3[3];
            stage4[4] =- stage3[4] + stage3[7];
            stage4[5] = stage3[5] * Math.Sqrt(2);
            stage4[6] = stage3[6] * Math.Sqrt(2);
            stage4[7] = stage3[4] + stage3[7];

            // FINAL
            c[0] = stage4[0]/N; //+
            c[1] = stage4[7]*Math.Sqrt(2)/N; //+
            c[2] = stage4[2] * Math.Sqrt(2) / N; //+
            c[3] = stage4[5] * Math.Sqrt(2) / N; //+
            c[4] = stage4[1] * Math.Sqrt(2) / N; //+
            c[5] = stage4[6] * Math.Sqrt(2) / N; //+
            c[6] = stage4[3] * Math.Sqrt(2) / N; //+
            c[7] = stage4[4] * Math.Sqrt(2) / N; //+

            return c;
        }
        static double[] IFDCT(double[] y)
        {
            var x = new double[y.Length];
            int N = 8;

            // Промежуточные массивы, в которые будут записываться результаты stage1, 2, 3 и 4
            var stage1 = new double[y.Length];
            var stage2 = new double[y.Length];
            var stage3 = new double[y.Length];
            var stage4 = new double[y.Length];

            // FINAL
            stage4[0] = y[0] * N;
            stage4[7] = y[1] * N / Math.Sqrt(2);
            stage4[2] = y[2] * N / Math.Sqrt(2);
            stage4[5] = y[3] * N / Math.Sqrt(2);
            stage4[1] = y[4] * N / Math.Sqrt(2);
            stage4[6] = y[5] * N / Math.Sqrt(2);
            stage4[3] = y[6] * N / Math.Sqrt(2);
            stage4[4] = y[7] * N / Math.Sqrt(2);
            // STAGE 4
            stage3[0] = stage4[0];
            stage3[1] = stage4[1];
            stage3[2] = stage4[2];
            stage3[3] = stage4[3];
            stage3[4] = 0.5 * (stage4[7] - stage4[4]);
            stage3[5] = stage4[5] / Math.Sqrt(2);
            stage3[6] = stage4[6] / Math.Sqrt(2);
            stage3[7] = 0.5 * (stage4[7] + stage4[4]);
            // STAGE 3
            stage2[0] = 0.5 * (stage3[0] + stage3[1]);
            stage2[1] = 0.5 * (stage3[0] - stage3[1]);
            stage2[2] = 0.5 * (stage3[2] * Math.Sqrt(2) * Math.Cos(6 * Math.PI / (2 * N)) - stage3[3] * Math.Sqrt(2) * Math.Sin(6 * Math.PI / (2 * N)));
            stage2[3] = 0.5 * (stage3[2] * Math.Sqrt(2) * Math.Sin(6 * Math.PI / (2 * N)) + stage3[3] * Math.Sqrt(2) * Math.Cos(6 * Math.PI / (2 * N)));
            stage2[4] = 0.5 * (stage3[4] + stage3[6]);
            stage2[5] = 0.5 * (stage3[7] - stage3[5]);
            stage2[6] = 0.5 * (stage3[4] - stage3[6]);
            stage2[7] = 0.5 * (stage3[7] + stage3[5]);
            // STAGE 2
            stage1[0] = 0.5 * (stage2[0] + stage2[3]);
            stage1[1] = 0.5 * (stage2[1] + stage2[2]);
            stage1[2] = 0.5 * (stage2[1] - stage2[2]);
            stage1[3] = 0.5 * (stage2[0] - stage2[3]);
            stage1[4] = stage2[4] * Math.Cos(3 * Math.PI / (2 * N)) - stage2[7] * Math.Sin(3 * Math.PI / (2 * N));
            stage1[5] = stage2[5] * Math.Cos(Math.PI / (2 * N)) - stage2[6] * Math.Sin(Math.PI / (2 * N));
            stage1[6] = stage2[5] * Math.Sin(Math.PI / (2 * N)) + stage2[6] * Math.Cos(Math.PI / (2 * N));
            stage1[7] = stage2[4] * Math.Sin(3 * Math.PI / (2 * N)) + stage2[7] * Math.Cos(3 * Math.PI / (2 * N));
            // STAGE 1
            x[0] = 0.5 * (stage1[0] + stage1[7]);
            x[1] = 0.5 * (stage1[1] + stage1[6]);
            x[2] = 0.5 * (stage1[2] + stage1[5]);
            x[3] = 0.5 * (stage1[3] + stage1[4]);
            x[4] = 0.5 * (stage1[3] - stage1[4]);
            x[5] = 0.5 * (stage1[2] - stage1[5]);
            x[6] = 0.5 * (stage1[1] - stage1[6]);
            x[7] = 0.5 * (stage1[0] - stage1[7]);

            return x;
        }

        static void Main(string[] args)
        {
            var massiv = new double[] { 0, 1, 2, 3, 4, 5, 6, 7 };

            var fdct = FDCT(massiv);

            foreach (var item in fdct)
            {
                Console.WriteLine(item);
            }

            Console.WriteLine("------------------------------------"); 

            var idfct = IFDCT(fdct);

            foreach (var item in idfct)
            {
                Console.WriteLine(item);
            }

            Console.ReadLine();
        }
    }
}
