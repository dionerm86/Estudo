using System;
using System.Collections.Generic;
using System.Linq;

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
                var errosGeracaoMarcacao = string.Empty;
                var buffer = new byte[1024];
                var read = 0;

                for (var i = 0; i < _arquivos.Count; i++)
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

                // Verifica se existe algum erro tratado no momento da geração do arquivo.
                if (_erros != null && _erros.Any(f => f.Value != null))
                {
                    // Monta um texto com todos os problemas ocorridos ao gerar o arquivo de mesa, ao final do método, o texto é salvo em um arquivo separado e é zipado junto com o ASC.
                    errosGeracaoMarcacao = string.Format("Situações com arquivos de mesa: </br></br>{0}",
                        string.Join("</br>", _erros.Where(f => f.Value != null).Select(f => string.Format("Etiqueta: {0} Erro: {1}.", f.Key, MensagemAlerta.FormatErrorMsg(null, f.Value)))));

                    zip.AddStringAsFile(errosGeracaoMarcacao, "Situações com arquivos de mesa.error", string.Empty);
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
            var errosGeracaoMarcacao = string.Empty;

            for (var i = 0; i < _arquivos.Count; i++)
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
            
            // Verifica se existe algum erro tratado no momento da geração do arquivo.
            if (_erros != null && _erros.Any(f => f.Value != null))
            {
                // Monta um texto com todos os problemas ocorridos ao gerar o arquivo de mesa, ao final do método, o texto é salvo em um arquivo separado e é zipado junto com o ASC.
                errosGeracaoMarcacao = string.Format("Situações com arquivos de mesa: </br></br>{0}",
                    string.Join("</br>", _erros.Where(f => f.Value != null).Select(f => string.Format("Etiqueta: {0} Erro: {1}.", f.Key, MensagemAlerta.FormatErrorMsg(null, f.Value)))));

                System.IO.File.WriteAllText(System.IO.Path.Combine(diretorio, "Situações com arquivos de mesa.error"), errosGeracaoMarcacao);
            }
        }

        #endregion
    }
}
