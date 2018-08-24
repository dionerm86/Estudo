// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.Configuracoes;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Newtonsoft.Json;
using System.Linq;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Liberacoes.Configuracoes
{
    /// <summary>
    /// Classe que encapsula as configurações para a tela de listagem de liberações.
    /// </summary>
    [DataContract(Name = "Lista")]
    public class ListaDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        internal ListaDto()
        {
            this.ImprimirRelatorioCompleto = Liberacao.RelatorioLiberacaoPedido.ExibirApenasViaExpAlmPedidosEntrega ||
                Liberacao.RelatorioLiberacaoPedido.ExibirApenasViaExpAlmPedidosBalcao;
            this.ImprimirRelatorioCliente = Liberacao.TelaLiberacao.ExibirRelatorioCliente;
            this.ExibirIcms = PedidoConfig.Impostos.CalcularIcmsPedido;
            this.ApenasConsultaLiberacao = !Config.PossuiPermissao(Config.FuncaoMenuFinanceiro.ControleFinanceiroRecebimento);
        }

        /// <summary>
        /// Obtém ou define um valor que indica se poderá imprimir o relatório completo da liberação.
        /// </summary>
        [DataMember]
        [JsonProperty("imprimirRelatorioCompleto")]
        public bool ImprimirRelatorioCompleto { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se poderá imprimir a via do cliente apenas do relatório de liberação.
        /// </summary>
        [DataMember]
        [JsonProperty("imprimirRelatorioCliente")]
        public bool ImprimirRelatorioCliente { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se deverão ser exibidos valores de ICMS da liberação.
        /// </summary>
        [DataMember]
        [JsonProperty("exibirIcms")]
        public bool ExibirIcms { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o usuário terá permissão apenas de consulta de liberação.
        /// </summary>
        [DataMember]
        [JsonProperty("apenasConsultaLiberacao")]
        public bool ApenasConsultaLiberacao { get; set; }
    }
}
