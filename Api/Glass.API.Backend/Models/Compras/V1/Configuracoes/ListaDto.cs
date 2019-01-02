// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.Configuracoes;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Compras.V1.Configuracoes
{
    /// <summary>
    /// Classe que encapsula as configurações para a tela de listagem de compras.
    /// </summary>
    [DataContract(Name = "Lista")]
    public class ListaDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        internal ListaDto()
        {
            this.ControleFinanceiroPagamento = Config.PossuiPermissao(Config.FuncaoMenuFinanceiroPagto.ControleFinanceiroPagamento);
            this.UsarControleFinalizacaoDeCompra = FinanceiroConfig.Compra.UsarControleFinalizacaoCompra;
            this.ExibirFiltroCentroCustoDivergente = FiscalConfig.UsarControleCentroCusto && CentroCustoDAO.Instance.GetCountReal() > 0;
        }

        /// <summary>
        /// Obtém ou define um valor que indica se o usuário possui permissão para inserir novas compras.
        /// </summary>
        [DataMember]
        [JsonProperty("controleFinanceiroPagamento")]
        public bool? ControleFinanceiroPagamento { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o usuário possui permissão para acessar os controles associados a finalização de compras.
        /// </summary>
        [DataMember]
        [JsonProperty("usarControleFinalizacaoDeCompra")]
        public bool? UsarControleFinalizacaoDeCompra { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o usuário possui permissão para acessar o controle de centro de custo divergente.
        /// </summary>
        [DataMember]
        [JsonProperty("exibirFiltroCentroCustoDivergente")]
        public bool? ExibirFiltroCentroCustoDivergente { get; set; }
    }
}