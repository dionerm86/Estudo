// <copyright file="ConfiguracaoBiesse.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;

namespace Glass.Data.Helper
{
    /// <summary>
    /// Repreesnta as configurações da bieese.
    /// </summary>
    public sealed class ConfiguracaoBiesse
    {
        private readonly List<Exception> erros = new List<Exception>();

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ConfiguracaoBiesse"/>.
        /// </summary>
        private ConfiguracaoBiesse()
        {
        }

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
        public IEnumerable<Exception> Erros => erros;

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

            var arquivoConfiguracoes = System.IO.Path.Combine(diretorio, "Biesse.config");
            if (System.IO.File.Exists(arquivoConfiguracoes))
            {
                Configuracoes configuracoes = null;
                var serializer = new System.Xml.Serialization.XmlSerializer(typeof(Configuracoes));
                try
                {
                    using (var conteudo = System.IO.File.OpenRead(arquivoConfiguracoes))
                    {
                        configuracoes = (Configuracoes)serializer.Deserialize(conteudo);
                    }
                }
                catch (Exception ex)
                {
                    this.erros.Add(ex);
                    yield break;
                }

                if (configuracoes != null)
                {
                    foreach (var config in configuracoes.Contextos)
                    {
                        yield return new Contexto(config.Nome, config.DiretorioSaida ?? Glass.Configuracoes.PCPConfig.CaminhoSalvarIntermac);
                    }

                    yield break;
                }
            }

            yield return new Contexto("default", Glass.Configuracoes.PCPConfig.CaminhoSalvarIntermac);
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
            /// Inicia uma nova instância da classe <see cref="Contexto"/>.
            /// </summary>
            /// <param name="nome">Nome do contexto.</param>
            /// <param name="diretorioSaida">Diretório de saída.</param>
            internal Contexto(string nome, string diretorioSaida)
            {
                this.Nome = nome;
                this.DiretorioSaida = diretorioSaida;
            }

            /// <summary>
            /// Obtém o nome do contexto.
            /// </summary>
            public string Nome { get; }

            /// <summary>
            /// Obtém o caminho do diretório de saída dos arquivos.
            /// </summary>
            public string DiretorioSaida { get; }
        }

        /// <summary>
        /// Representa os dados da configuração da máquina.
        /// </summary>
        public class ConfiguracaoContexto
        {
            /// <summary>
            /// Obtém ou define o nome do contexto.
            /// </summary>
            [System.Xml.Serialization.XmlAttribute("nome")]
            public string Nome { get; set; }

            /// <summary>
            /// Obtém ou define o diretório de saída.
            /// </summary>
            [System.Xml.Serialization.XmlAttribute("diretorioSaida")]
            public string DiretorioSaida { get; set; }
        }

        /// <summary>
        /// Representa as configurações.
        /// </summary>
        public class Configuracoes
        {
            /// <summary>
            /// Obtém os contextos.
            /// </summary>
            [System.Xml.Serialization.XmlArrayItem("Contexto")]
            public List<ConfiguracaoContexto> Contextos { get; } = new List<ConfiguracaoContexto>();
        }
    }
}
