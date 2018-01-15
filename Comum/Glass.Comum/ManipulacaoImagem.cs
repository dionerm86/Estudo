using System;
using System.Drawing;

namespace Glass
{
    /// <summary>
    /// Classe de manipulação de imagens.
    /// </summary>
    public static class ManipulacaoImagem
    {
        #region Constants

        /// <summary>
        /// Altura máxima padrão.
        /// </summary>
        public const int AlturaMaxima = 1536;

        /// <summary>
        /// Largura máxima padrão.
        /// </summary>
        public const int LarguraMaxima = 1536;

        #endregion

        #region Muda o tamanho de uma imagem

        #region Métodos de suporte

        /// <summary>
        /// Recupera o tamanho da imagem.
        /// </summary>
        /// <param name="imagem"></param>
        /// <param name="alturaMaxima"></param>
        /// <param name="larguraMaxima"></param>
        /// <param name="percentual"></param>
        /// <returns></returns>
        private static Size ObtemTamanhoImagem(Image imagem, int alturaMaxima, int larguraMaxima, float percentual)
        {
            alturaMaxima = alturaMaxima > 0 ? alturaMaxima : imagem.Height;
            larguraMaxima = larguraMaxima > 0 ? larguraMaxima : imagem.Width;

            if (imagem.Width > imagem.Height)
            {
                if (imagem.Width <= larguraMaxima)
                {
                    larguraMaxima = (int)(imagem.Width * percentual);
                    alturaMaxima = (int)(imagem.Height * percentual);
                }
                else
                    alturaMaxima = (int)(((float)larguraMaxima / (float)imagem.Width) * imagem.Height);
            }
            else
            {
                if (imagem.Height <= alturaMaxima)
                {
                    alturaMaxima = (int)(imagem.Height * percentual);
                    larguraMaxima = (int)(imagem.Width * percentual);
                }
                else
                    larguraMaxima = (int)(((float)alturaMaxima / (float)imagem.Height) * imagem.Width);
            }

            return new Size(larguraMaxima, alturaMaxima);
        }

        #endregion

        /// <summary>
        /// Redimensiona a imagem.
        /// </summary>
        /// <param name="imagem">Imagem que será redimensioada.</param>
        /// <param name="alturaMaxima">Altura máxima.</param>
        /// <param name="larguraMaxima">Largura máxima.</param>
        /// <param name="percentual">Percentual.</param>
        /// <returns></returns>
        public static Image Redimensionar(this Image imagem, int alturaMaxima, int larguraMaxima, float percentual)
        {
            Size novoTamanho = ObtemTamanhoImagem(imagem, alturaMaxima, larguraMaxima, percentual);
            return new Bitmap(imagem, novoTamanho);
        }

        /// <summary>
        /// Redimensiona a imagem.
        /// </summary>
        /// <param name="imagem">Buffer da imagem.</param>
        /// <param name="alturaMaxima">Altura máxima.</param>
        /// <param name="larguraMaxima">Largura máxima.</param>
        /// <param name="percentual">Percentual.</param>
        /// <returns></returns>
        public static byte[] Redimensionar(byte[] imagem, int alturaMaxima, int larguraMaxima, float percentual)
        {
            byte[] retorno = null;
            using (var original = new System.IO.MemoryStream(imagem))
            using (Image i = Image.FromStream(original))
            {
                using (Image nova = Redimensionar(i, alturaMaxima, larguraMaxima, percentual))
                {
                    using (var m = new System.IO.MemoryStream())
                    {
                        nova.Save(m, System.Drawing.Imaging.ImageFormat.Jpeg);
                        m.Position = 0;
                        retorno = m.ToArray();
                    }
                }
            }

            return retorno;
        }

        #endregion

        /// <summary>
        /// Salva a imagem no caminho informado.
        /// </summary>
        /// <param name="caminho">Caminho onde a imagem será salva.</param>
        /// <param name="stream">Stream com os dados da imagem.</param>
        /// <returns></returns>
        public static bool SalvarImagem(string caminho, System.IO.Stream stream)
        {
            if (stream == null)
                return false;

            try
            {
                var diretorio = System.IO.Path.GetDirectoryName(caminho);
                // Verifica se o diretório do arquivo existe
                if (!System.IO.Directory.Exists(diretorio))
                    System.IO.Directory.CreateDirectory(diretorio);

                if (caminho.IndexOf(".jpg") > -1)
                {
                    // Carrega a imagem contida na Stream
                    using (var b = System.Drawing.Bitmap.FromStream(stream))
                    {
                        // Verifica se excede os limites da image
                        if (b.Width > LarguraMaxima || b.Height > AlturaMaxima)
                            using (var imageAux = b.Redimensionar(AlturaMaxima, LarguraMaxima, 1))
                                imageAux.Save(caminho, System.Drawing.Imaging.ImageFormat.Jpeg);
                        else
                            b.Save(caminho, System.Drawing.Imaging.ImageFormat.Jpeg);
                    }
                }
                else
                {
                    using (var f = System.IO.File.Create(caminho))
                    {
                        int read = 0;
                        var buffer = new byte[1024];
                        while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                            f.Write(buffer, 0, read);

                        f.Flush();
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                //Glass.Data.DAL.ErroDAO.Instance.InserirFromException("SalvarImagem", ex);
                return false;
            }
        }

        /// <summary>
        /// Salva a imagem contida no buffer informado.
        /// </summary>
        /// <param name="caminho">Caminho onde a imagem será salva.</param>
        /// <param name="buffer">Buffer com os dados da imagem.</param>
        /// <returns></returns>
        public static bool SalvarImagem(string caminho, byte[] buffer)
        {
            if (buffer == null || buffer.Length == 0)
                return false;

            using (var ms = new System.IO.MemoryStream(buffer))
                return SalvarImagem(caminho, ms);
        }

        public static bool RenomearImagem(string caminhoAntigo, string caminhoNovo)
        {
            if (System.IO.File.Exists(caminhoAntigo) && !System.IO.File.Exists(caminhoNovo))
            {
                System.IO.File.Move(caminhoAntigo, caminhoNovo);
                return true;
            }

            return false;
        }
    }
}
