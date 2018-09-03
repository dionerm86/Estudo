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
        private readonly List<Exception> _erros = new List<Exception>();

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

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ConfiguracaoBiesse"/>.
        /// </summary>
        private ConfiguracaoBiesse()
        {
        }

        /// <summary>
        /// Carrega os contexto com base nas configurações contidas no diretório informado.
        /// </summary>
        /// <param name="diretorio">Diretório onde estão os contextos.</param>
        /// <returns>Contextos.</returns>
        private IEnumerable<Contexto> CarregarContextos(string diretorio)
        {
            if (!System.IO.Directory.Exists(diretorio))
            {
                yield break;
            }

            foreach (var dir in System.IO.Directory.GetDirectories(diretorio, "*.*", System.IO.SearchOption.TopDirectoryOnly))
            {
                var setupDataFileName = System.IO.Path.Combine(dir, "SetupData.xml");

                // Verifica se o arquivo base de configuração existe
                if (System.IO.File.Exists(setupDataFileName))
                {
                    CalcEngine.Biesse.OutputGenerator geradorSaida = CalcEngine.Biesse.OutputGenerator.iCam;

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
                    {
                        maquina = "VertMax Sx_Dx";
                    }

                    CalcEngine.Biesse.DxfImporterContext importerContext = null;
                    try
                    {
                        importerContext = new CalcEngine.Biesse.DxfImporterContext(new System.IO.Abstractions.FileSystem(), dir, geradorSaida);
                    }
                    catch (Exception ex)
                    {
                        _erros.Add(ex);
                        // Igonra erros ao carregar o contexto
                        continue;
                    }

                    yield return new Contexto(System.IO.Path.GetFileNameWithoutExtension(dir), maquina, diretorioSaida, importerContext);
                }
            }
        }

        /// <summary>
        /// Inicializa a configuração.
        /// </summary>
        /// <param name="directorioConfiguracao">Diretório onde ficas dos dados da configuração.</param>
        public void Inicializar(string directorioConfiguracao)
        {
            this.Contextos = this.CarregarContextos(directorioConfiguracao).ToList();
        }

        /// <summary>
        /// Contexto de configuração.
        /// </summary>
        public class Contexto
        {
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

            /// <summary>
            /// Inicia uma nova instância da classe <see cref="Contexto"/>.
            /// </summary>
            /// <param name="nome">Nome do contexto.</param>
            /// <param name="nomeMaquina">Nome da máquina do contexto.</param>
            /// <param name="diretorioSaida">Diretório de saída.</param>
            /// <param name="importerContext">Contexto do importador.</param>
            internal Contexto(string nome, string nomeMaquina, string diretorioSaida, CalcEngine.Biesse.DxfImporterContext importerContext)
            {
                this.Nome = nome;
                this.NomeMaquina = nomeMaquina;
                this.DiretorioSaida = diretorioSaida;
                this.ImporterContext = importerContext;
            }
        }

        /// <summary>
        /// Representa os dados da configuração da máquina.
        /// </summary>
        public class ConfiguracaoMaquina
        {
            /// <summary>
            /// Obtém ou define o nome da máquina.
            /// </summary>
            public string Maquina { get; set; }

            /// <summary>
            /// Obtém ou define o diretório de saída.
            /// </summary>
            public string DiretorioSaida { get; set; }

            /// <summary>
            /// Obtém ou define o tipo do gerador de saída.
            /// </summary>
            public CalcEngine.Biesse.OutputGenerator GeradorSaida { get; set; }
        }
    }
}
