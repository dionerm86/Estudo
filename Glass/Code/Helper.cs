using System;
using System.Web.UI.DataVisualization.Charting;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace Glass.UI.Web.Util
{
    public static class Helper
    {
        /// <summary>
        /// Salva a imagem de um gráfico MS CHART em um arquivo temporário e retorna o caminho da imagem
        /// </summary>
        /// <param name="chart">MS Chart</param>
        /// <param name="nome">Nome da imagem</param>
        /// <returns></returns>
        public static string SalvaGraficoTemp(Chart chart, string nome)
        {
            string pathImage = "";

            string tempFile = Path.GetTempFileName();

            using (FileStream fs = new FileStream(tempFile, FileMode.Create, FileAccess.Write))
            {
                chart.SaveImage(fs);
            }
            using (FileStream fs = new FileStream(tempFile, FileMode.Open, FileAccess.Read))
            {
                System.Drawing.Image img = Bitmap.FromStream(fs);
                pathImage = String.Concat(System.IO.Path.GetTempPath(), nome + ".png");
                img.Save(pathImage, ImageFormat.Png);
            }

            return pathImage;
        }

        public static byte[] ChartToByteArray(Chart chart)
        {
            byte[] buffer = null;

            using (MemoryStream ms = new MemoryStream())
            {
                chart.SaveImage(ms);
                ms.Position = 0;
                buffer = ms.ToArray();
            }

            return buffer;
        }

    }
}
