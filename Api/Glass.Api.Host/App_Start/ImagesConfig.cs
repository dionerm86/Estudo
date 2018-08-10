using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace Glass.Api.Host
{
    public static class ImagesConfig
    {
        private static string _diretorioModelosProjeto = System.Configuration.ConfigurationManager.AppSettings["diretorioModelosProjeto"];
        private static string _arquivoModelosProjetoPath = Path.Combine(_diretorioModelosProjeto, "ModeloProjetos.zip");
        private static string _checksumPath = Path.Combine(_diretorioModelosProjeto, "checksum.txt");

        public static void Configure()
        {
            if (!Directory.Exists(_diretorioModelosProjeto))
                return;

            if (!File.Exists(_checksumPath))
            {
                CriarArquivoZip();
                return;
            }

            var checksum = File.ReadAllText(_checksumPath);
            var checksumAtual = CreateMd5();

            if (checksum != checksumAtual || !File.Exists(_arquivoModelosProjetoPath))
                CriarArquivoZip();
        }

        private static string CreateMd5()
        {
            var files = ObterImagensAtivas().ToArray();
            var md5 = MD5.Create();

            for (int i = 0; i < files.Length; i++)
            {
                var file = files[i];

                if (file.Equals(_checksumPath) || file.Equals(_arquivoModelosProjetoPath))
                    continue;

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
            if (!Directory.Exists(_diretorioModelosProjeto))
                return null;

            Configure();

            var bytes = File.ReadAllBytes(_arquivoModelosProjetoPath);
            var ms = new MemoryStream(bytes);

            return ms;
        }

        private static void CriarArquivoZip()
        {
            if (File.Exists(_arquivoModelosProjetoPath))
                File.Delete(_arquivoModelosProjetoPath);

            if (File.Exists(_checksumPath))
                File.Delete(_checksumPath);

            using (var zipFile = new Ionic.Zip.ZipFile(System.Text.Encoding.UTF8))
            {
                zipFile.AddFiles(ObterImagensAtivas());
                zipFile.Save(_arquivoModelosProjetoPath);
            }

            var checksum = CreateMd5();
            File.WriteAllText(_checksumPath, checksum);
        }

        private static IEnumerable<string> ObterImagensAtivas()
        {
            var imagens = Glass.Data.DAL.ProjetoModeloDAO.Instance.ObterImagens();

            foreach (var img in imagens.Select(f => Path.Combine(_diretorioModelosProjeto, f)).Where(f => File.Exists(f)))
                yield return img;
        }
    }
}