using System;
using System.IO;
using System.Security.Cryptography;

namespace Glass.Api.Host
{
    public static class ImagesConfig
    {
        private static string _checksum;

        public static void Configure(string checksum = null)
        {
            try
            {
                var diretorioModelosProjeto = System.Configuration.ConfigurationManager.AppSettings["diretorioModelosProjeto"];
                if (!Directory.Exists(diretorioModelosProjeto))
                    return;

                var arquivoModelosProjeto = Path.Combine(diretorioModelosProjeto, "ModeloProjetos.zip");

                if (!string.IsNullOrWhiteSpace(checksum))
                    _checksum = checksum;
                else
                    _checksum = CreateMd5ForFolder(diretorioModelosProjeto);

                CriarArquivoZip(diretorioModelosProjeto, arquivoModelosProjeto);
                
            }
            catch
            {
            }
        }

        public static string CreateMd5ForFolder(string path)
        {
            var files = Directory.GetFiles(path);
            var md5 = MD5.Create();

            for (int i = 0; i < files.Length; i++)
            {
                var file = files[i];

                // hash contents
                byte[] contentBytes = File.ReadAllBytes(file);

                if (i == files.Length - 1)
                    md5.TransformFinalBlock(contentBytes, 0, contentBytes.Length);
                else
                    md5.TransformBlock(contentBytes, 0, contentBytes.Length, contentBytes, 0);
            }

            return BitConverter.ToString(md5.Hash).Replace("-", "").ToLower();
        }

        public static MemoryStream ObterImagensProjeto()
        {
            var diretorioModelosProjeto = System.Configuration.ConfigurationManager.AppSettings["diretorioModelosProjeto"];

            if (!Directory.Exists(diretorioModelosProjeto))
                return null;

            var arquivoModelosProjeto = Path.Combine(diretorioModelosProjeto, "ModeloProjetos.zip");

            var checksumAtual = CreateMd5ForFolder(diretorioModelosProjeto);

            if (checksumAtual != _checksum)
                Configure(checksumAtual);

            var bytes = File.ReadAllBytes(arquivoModelosProjeto);
            var ms = new MemoryStream(bytes);

            return ms;
        }

        private static void CriarArquivoZip(string diretorioPath, string filePath)
        {
            if (File.Exists(filePath))
                File.Delete(filePath);

            using (var zipFile = new Ionic.Zip.ZipFile(System.Text.Encoding.UTF8))
            {
                zipFile.AddDirectory(diretorioPath);
                zipFile.Save(filePath);
            }
        }
    }
}