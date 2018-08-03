using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Data.Helper
{
    /// <summary>
    /// Repreesnta as configurações da bieese.
    /// </summary>
    public sealed class ConfiguracaoBiesse
    {
        #region Variaveis Locais

        public List<Exception> _erros = new List<Exception>();

        #endregion

        #region Propriedades

        /// <summary>
        /// Obtém a instancia da configuração.
        /// </summary>
        public static ConfiguracaoBiesse Instancia { get; } = new ConfiguracaoBiesse();

        /// <summary>
        /// Obtém os contextos de configuração.
        /// </summary>
        public IEnumerable<Contexto> Contextos { get; private set; } = new List<Contexto>();

        /// <summary>
        /// Obtém os erros da configuração.
        /// </summary>
        public IEnumerable<Exception> Erros => _erros;

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        private ConfiguracaoBiesse()
        {
        }

        #endregion

        #region Métodos Privados

        /// <summary>
        /// Carrega os contexto com base nas configurações contidas no diretório informado.
        /// </summary>
        /// <param name="diretorio"></param>
        /// <returns></returns>
        private IEnumerable<Contexto> CarregarContextos(string diretorio)
        {
            if (!System.IO.Directory.Exists(diretorio)) yield break;

            foreach(var dir in System.IO.Directory.GetDirectories(diretorio, "*.*", System.IO.SearchOption.TopDirectoryOnly))
            {
                var setupDataFileName = System.IO.Path.Combine(dir, "SetupData.xml");

                // Verifica se o arquivo base de configuração existe
                if (System.IO.File.Exists(setupDataFileName))
                {
                    CalcEngine.Biesse.DxfImporterContext importerContext = null;
                    try
                    {
                        importerContext = new CalcEngine.Biesse.DxfImporterContext(new System.IO.Abstractions.FileSystem(), dir);
                    }
                    catch (Exception ex)
                    {
                        _erros.Add(ex);
                        // Igonra erros ao carregar o contexto
                        continue;
                    }

                    string maquina = null;
                    string diretorioSaida = Configuracoes.PCPConfig.CaminhoSalvarIntermac;

                    var arquivoMaquina = System.IO.Path.Combine(dir, "Maquina.config");
                    if (System.IO.File.Exists(arquivoMaquina))
                    {
                        var serializer = new System.Xml.Serialization.XmlSerializer(typeof(ConfiguracaoMaquina));
                        try
                        {
                            using (var conteudo = System.IO.File.OpenRead(arquivoMaquina))
                            {
                                var configuracao = (ConfiguracaoMaquina)serializer.Deserialize(conteudo);
                                maquina = configuracao.Maquina;
                                diretorioSaida = configuracao.DiretorioSaida ?? Configuracoes.PCPConfig.CaminhoSalvarIntermac;
                            }
                        }
                        catch (Exception ex)
                        {
                            _erros.Add(ex);
                            // Ignora erros
                        }
                    }

                    if (maquina == null)
                        maquina = "VertMax Sx_Dx";

                    yield return new Contexto(System.IO.Path.GetFileNameWithoutExtension(dir), maquina, diretorioSaida, importerContext);
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Inicializa a configuração.
        /// </summary>
        /// <param name="directorioConfiguracao">Diretório onde ficas dos dados da configuração.</param>
        public void Inicializar(string directorioConfiguracao)
        {
            Contextos = CarregarContextos(directorioConfiguracao).ToList();
        }

        #endregion

        #region Tipos Aninhados

        /// <summary>
        /// Contexto de configuração.
        /// </summary>
        public class Contexto
        {
            #region Propriedades

            /// <summary>
            /// Obtém o nome do contexto.
            /// </summary>
            public string Nome { get; }

            /// <summary>
            /// Obtém o nome da máquina.
            /// </summary>
            public string NomeMaquina { get; }

            /// <summary>
            /// Obtém o caminho do diretório de saída dos arquivos.
            /// </summary>
            public string DiretorioSaida { get; }

            /// <summary>
            /// Obtém o contexto do importador.
            /// </summary>
            public CalcEngine.Biesse.DxfImporterContext ImporterContext { get; }

            #endregion

            #region Construtores

            /// <summary>
            /// Construtor padrão.
            /// </summary>
            /// <param name="nome"></param>
            /// <param name="nomeMaquina"></param>
            /// <param name="diretorioSaida"></param>
            /// <param name="importerContext"></param>
            internal Contexto(string nome, string nomeMaquina, string diretorioSaida, CalcEngine.Biesse.DxfImporterContext importerContext)
            {
                Nome = nome;
                NomeMaquina = nomeMaquina;
                DiretorioSaida = diretorioSaida;
                ImporterContext = importerContext;
            }

            #endregion
        }

        /// <summary>
        /// Representa os dados da configuração da máquina.
        /// </summary>
        public class ConfiguracaoMaquina
        {
            #region Propriedades

            /// <summary>
            /// Obtém ou define o nome da máquina.
            /// </summary>
            public string Maquina { get; set; }

            /// <summary>
            /// Obtém ou define o diretório de saída.
            /// </summary>
            public string DiretorioSaida { get; set; }

            #endregion
        }

        #endregion
    }
}
