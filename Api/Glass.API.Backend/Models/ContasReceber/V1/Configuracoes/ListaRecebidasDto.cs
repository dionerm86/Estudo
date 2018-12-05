// <copyright file="ListaRecebidasDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.Configuracoes;
using Glass.Data.DAL;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.ContasReceber.V1.Configuracoes
{
    /// <summary>
    /// Classe que encapsula as configurações para a tela de listagem de contas recebidas.
    /// </summary>
    [DataContract(Name = "ListaRecebidas")]
    public class ListaRecebidasDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaRecebidasDto"/>.
        /// </summary>
        internal ListaRecebidasDto()
        {
            this.UtilizarCnab = FinanceiroConfig.FinanceiroRec.ExibirCnab;
            this.ExibirComissao = FinanceiroConfig.RelatorioContasRecebidas.ExibirComissao;
            this.ComissaoPorContasRecebida = ComissaoDAO.Instance.VerificarComissaoContasRecebidas();
            this.FiltrarContasVinculadasPorPadrao = FinanceiroConfig.FiltroContasVinculadasMarcadoPorPadrao;
            this.UsarLiberacaoPedido = PedidoConfig.LiberarPedido;
            this.GerarArquivoGCon = FinanceiroConfig.GerarArquivoGCon;
            this.GerarArquivoProsoft = FinanceiroConfig.GerarArquivoProsoft;
            this.GerarArquivoDominio = FinanceiroConfig.GerarArquivoDominio;
            this.ExisteRotaCadastrada = RotaDAO.Instance.ExisteRota();
        }

        /// <summary>
        /// Obtém ou define um valor que indica se a empresa utiliza o controle de CNAB.
        /// </summary>
        [DataMember]
        [JsonProperty("utilizarCnab")]
        public bool UtilizarCnab { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se é para exibir dados de comissão na lista de contas recebidas.
        /// </summary>
        [DataMember]
        [JsonProperty("exibirComissao")]
        public bool ExibirComissao { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se a empresa utiliza o controle de comissão por contas recebidas.
        /// </summary>
        [DataMember]
        [JsonProperty("comissaoPorContasRecebida")]
        public bool ComissaoPorContasRecebida { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se deverá filtrar por padrão contas vinculadas.
        /// </summary>
        [DataMember]
        [JsonProperty("filtrarContasVinculadasPorPadrao")]
        public bool FiltrarContasVinculadasPorPadrao { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se a empresa utiliza o controle liberação de pedidos.
        /// </summary>
        [DataMember]
        [JsonProperty("usarLiberacaoPedido")]
        public bool UsarLiberacaoPedido { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se a empresa gera arquivo GCon.
        /// </summary>
        [DataMember]
        [JsonProperty("gerarArquivoGCon")]
        public bool GerarArquivoGCon { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se a empresa gera arquivo Prosoft.
        /// </summary>
        [DataMember]
        [JsonProperty("gerarArquivoProsoft")]
        public bool GerarArquivoProsoft { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se a empresa gera arquivo da Domínio.
        /// </summary>
        [DataMember]
        [JsonProperty("gerarArquivoDominio")]
        public bool GerarArquivoDominio { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se há alguma rota cadastrada no sistema.
        /// </summary>
        [DataMember]
        [JsonProperty("existeRotaCadastrada")]
        public bool ExisteRotaCadastrada { get; set; }
    }
}
