using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace Glass
{
    public static class Arquivos
    {
        /// <summary>
        /// Constante que identifica o cabeçalho de um zip.
        /// </summary>
        private const uint ZIP_LEAD_BYTES = 0x04034b50;
        /// <summary>
        /// Constante que identifica o cabeçalho de um GZip.
        /// </summary>
        private const ushort GZIP_LEAD_BYTES = 0x8b1f;

        #region Métodos de compactação

        /// <summary>
        /// Compacta um vetor de bytes.
        /// </summary>
        /// <param name="dados"></param>
        public static byte[] Compactar(byte[] dados)
        {
            using (MemoryStream m = new MemoryStream())
            {
                using (GZipStream z = new GZipStream(m, CompressionMode.Compress, true))
                {
                    z.Write(dados, 0, dados.Length);
                }

                m.Position = 0;
                using (BinaryReader r = new BinaryReader(m))
                    return r.ReadBytes((int)m.Length);
            }
        }

        /// <summary>
        /// Descompacta um vetor de bytes.
        /// </summary>
        /// <param name="dados"></param>
        public static byte[] Descompactar(byte[] dados)
        {
            List<byte> retorno = new List<byte>();
            using (MemoryStream m = new MemoryStream(dados))
            {
                using (GZipStream z = new GZipStream(m, CompressionMode.Decompress, true))
                {
                    byte[] buffer = new byte[1024];
                    int read = 0;
                    while ((read = z.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        if (read < buffer.Length)
                        {
                            byte[] temp = new byte[read];
                            Buffer.BlockCopy(buffer, 0, temp, 0, read);
                            retorno.AddRange(temp);
                        }
                        else
                            retorno.AddRange(buffer);
                    }
                }
            }

            return retorno.ToArray();
        }

        #endregion

        /// <summary>
        /// Verifica se no buffer informado possui um arquivo ZIP.
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static bool VerificarArquivoZip(byte[] buffer)
        {
            // Se o 4 primeiros bytes do buffer possui a assinatura ZIP para dados comprimidos
            return buffer != null && 
                ((buffer.Length >= 4 && (BitConverter.ToUInt32(buffer, 0) == ZIP_LEAD_BYTES)) ||
                 (buffer.Length >= 2 && (BitConverter.ToUInt16(buffer, 0) == GZIP_LEAD_BYTES)));
        }

        public static bool IsImagem(string extensao)
        {
            var ext = new List<string>(new string[] { ".jpg", ".bmp", ".png", ".jpeg", ".gif" });
            return ext.Contains(extensao.ToLower());
        }        
    }
}
