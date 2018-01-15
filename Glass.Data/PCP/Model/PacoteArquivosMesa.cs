using System;
using System.Collections.Generic;

namespace Glass.Data.Model
{
    /// <summary>
    /// Salva os arquivos gerados.
    /// </summary>
    public class PacoteArquivosMesa
    {
        #region Variáveis Locais

        private List<byte[]> _arquivos;
        private List<string> _nomes;
        private List<KeyValuePair<string, Exception>> _erros;

        #endregion

        #region Construtores

        /// <summary>
        /// Contrutor padrão.
        /// </summary>
        /// <param name="arquivos"></param>
        /// <param name="nomes"></param>
        /// <param name="erros"></param>
        public PacoteArquivosMesa(List<byte[]> arquivos, List<string> nomes, List<KeyValuePair<string, Exception>> erros)
        {
            _arquivos = arquivos;
            _nomes = nomes;
            _erros = erros;
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Compacta os arquivos gerados.
        /// </summary>
        /// <param name="outputStream"></param>
        public void Compactar(System.IO.Stream outputStream)
        {
            // Adiciona os arquivos FML
            using (var zip = new Ionic.Utils.Zip.ZipFile(outputStream))
            {
                var buffer = new byte[1024];
                var read = 0;
                for (var i = 0; i < _arquivos.Count; i++)
                {
                    if (_erros[i].Value != null)
                    {
                        zip.AddStringAsFile(string.Format("Etiqueta: {0} Erro: {1}.", _erros[i].Key, _erros[i].Value.Message),
                            string.Format("{0}{1}", System.IO.Path.GetFileNameWithoutExtension(_nomes[i]), ".error"), "");
                    }
                    else
                    {
                        // Verifica se o arquivo é um zip ou seja um arquivo GLO
                        if (Glass.Arquivos.VerificarArquivoZip(_arquivos[i]))
                        {
                            // Descompacta o arquivo zip
                            using (var ms = new System.IO.MemoryStream(_arquivos[i]))
                            {
                                var zip2 = new CalcEngine.Compression.ZipArchive(ms, System.IO.FileAccess.Read);
                                foreach (var file in zip2.Files)
                                {
                                    using (var stream2 = file.OpenRead())
                                    {
                                        var ms2 = new System.IO.MemoryStream((int)file.Length);

                                        while ((read = stream2.Read(buffer, 0, buffer.Length)) > 0)
                                            ms2.Write(buffer, 0, read);

                                        var fileName = System.IO.Path.GetFileNameWithoutExtension(_nomes[i]);
                                        var directory = System.IO.Path.GetDirectoryName(file.Name);

                                        zip.AddFileStream(fileName + System.IO.Path.GetExtension(file.Name), directory, ms2);
                                    }
                                }
                            }
                        }
                        else
                            zip.AddFileStream(_nomes[i].Replace("  ", "").Replace(" ", ""), "", new System.IO.MemoryStream(_arquivos[i]));
                    }
                }

                zip.Save();
            }
        }

        /// <summary>
        /// Salva os arquivos no diretório informado
        /// </summary>
        /// <param name="diretorio"></param>
        public void SalvarArquivos(string diretorio)
        {
            var buffer = new byte[1024];
            var read = 0;
            for (var i = 0; i < _arquivos.Count; i++)
            {
                if (_erros[i].Value != null)
                {
                    System.IO.File.WriteAllText(
                        System.IO.Path.Combine(diretorio, string.Format("{0}{1}", System.IO.Path.GetFileNameWithoutExtension(_nomes[i]))),
                        string.Format("Etiqueta: {0} Erro: {1}.", _erros[i].Key, _erros[i].Value.Message));
                }
                else
                {
                    // Verifica se o arquivo é um zip ou seja um arquivo GLO
                    if (Glass.Arquivos.VerificarArquivoZip(_arquivos[i]))
                    {
                        // Descompacta o arquivo zip
                        using (var ms = new System.IO.MemoryStream(_arquivos[i]))
                        {
                            var zip2 = new CalcEngine.Compression.ZipArchive(ms, System.IO.FileAccess.Read);
                            foreach (var file in zip2.Files)
                            {
                                using (var stream2 = file.OpenRead())
                                {
                                    var fileName = System.IO.Path.GetFileNameWithoutExtension(_nomes[i]);

                                    var ms2 = System.IO.File.Create(System.IO.Path.Combine(diretorio, fileName + System.IO.Path.GetExtension(file.Name)));

                                    while ((read = stream2.Read(buffer, 0, buffer.Length)) > 0)
                                        ms2.Write(buffer, 0, read);

                                }
                            }
                        }
                    }
                    else
                        System.IO.File.WriteAllBytes(System.IO.Path.Combine(diretorio, _nomes[i].Replace("  ", "").Replace(" ", "")), _arquivos[i]);
                }
            }
        }

        #endregion
    }
}
