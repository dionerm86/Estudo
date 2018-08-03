using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Otimizacao
{
    /// <summary>
    /// Representa o repositório da solução de otimização em um diretório do disco.
    /// </summary>
    public class RepositorioSolucaoOtimizacao : IRepositorioSolucaoOtimizacao
    {
        #region Propriedades

        /// <summary>
        /// Obtém o diretório base.
        /// </summary>
        public string DiretorioBase { get; }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="diretorioBase"></param>
        public RepositorioSolucaoOtimizacao(string diretorioBase)
        {
            if (string.IsNullOrEmpty(diretorioBase))
                throw new ArgumentNullException(nameof(diretorioBase));

            DiretorioBase = diretorioBase;
        }

        #endregion

        #region Métodos Privados

        private string ObterDiretorio(ISolucaoOtimizacao solucao)
        {
            return System.IO.Path.Combine(DiretorioBase, solucao.Uid.ToString());
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Verifica se existe o arquivo com o nome informado associado com a solução.
        /// </summary>
        /// <param name="solucaoOtimizacao"></param>
        /// <param name="nome">Nome do arquivo que será pesquisado.</param>
        /// <returns></returns>
        public bool ArquivoExiste(ISolucaoOtimizacao solucaoOtimizacao, string nome)
        {
            var diretorio = ObterDiretorio(solucaoOtimizacao);
            var caminho = System.IO.Path.Combine(diretorio, nome);

            return System.IO.File.Exists(caminho);
        }

        /// <summary>
        /// Obtém os arquivos da solução.
        /// </summary>
        /// <param name="solucaoOtimizacao"></param>
        /// <returns></returns>
        public IEnumerable<IArquivoSolucaoOtimizacao> ObterArquivos(ISolucaoOtimizacao solucaoOtimizacao)
        {
            var diretorio = ObterDiretorio(solucaoOtimizacao);
            if (System.IO.Directory.Exists(diretorio))
                return System.IO.Directory.GetFiles(diretorio)
                    .Select(f => new Arquivo(f))
                    .ToArray();

            return new IArquivoSolucaoOtimizacao[0];
        }

        /// <summary>
        /// Salva os arquivos da solução de otimização.
        /// </summary>
        /// <param name="solucaoOtimizacao">Dados da solução para onde os arquivos serão salvos.</param>
        /// <param name="arquivos">Arquivos que serão salvos.</param>
        public void SalvarArquivos(ISolucaoOtimizacao solucaoOtimizacao, IEnumerable<IArquivoSolucaoOtimizacao> arquivos)
        {
            var diretorio = ObterDiretorio(solucaoOtimizacao);
            if (!System.IO.Directory.Exists(diretorio))
                System.IO.Directory.CreateDirectory(diretorio);

            var buffer = new byte[1024];
            var read = 0;

            foreach (var arquivo in arquivos)
            {
                var nomeArquivo = System.IO.Path.Combine(diretorio, arquivo.Nome);
                using (var entrada = arquivo.Abrir())
                using (var saida = System.IO.File.Create(nomeArquivo))
                {
                    while ((read = entrada.Read(buffer, 0, buffer.Length)) > 0)
                        saida.Write(buffer, 0, read);

                    saida.Flush();
                }
            }
        }

        #endregion

        #region Nested Types

        /// <summary>
        /// Representa um arquivo do repositório.
        /// </summary>
        class Arquivo : IArquivoSolucaoOtimizacao
        {
            #region Propriedades

            /// <summary>
            /// Obtém o nome completo do arquivo.
            /// </summary>
            public string Caminho { get; }

            /// <summary>
            /// Obtém Nome do arquivo.
            /// </summary>
            public string Nome => System.IO.Path.GetFileName(Caminho);

            #endregion

            #region Construtores

            /// <summary>
            /// Construtor padrão.
            /// </summary>
            /// <param name="caminho"></param>
            public Arquivo(string caminho)
            {
                Caminho = caminho;
            }

            #endregion

            #region Métodos Públicos

            /// <summary>
            /// Abrea a leitura do arquivo.
            /// </summary>
            /// <returns></returns>
            public System.IO.Stream Abrir() => System.IO.File.OpenRead(Caminho);

            #endregion
        }

        #endregion
    }
}
