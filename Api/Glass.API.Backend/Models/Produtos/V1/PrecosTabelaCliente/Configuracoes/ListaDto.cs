// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Glass.Configuracoes;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Produtos.V1.PrecosTabelaCliente.Configuracoes
{
    /// <summary>
    /// Classe que encapsula as configurações para a tela de preços de tabela por cliente.
    /// </summary>
    [DataContract(Name = "Lista")]
    public class ListaDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        internal ListaDto()
        {
            var idNomeAlterarSubgruposSelecionados = ProdutoConfig.TelaPrecoTabelaClienteRelatorio.AlterarSubgruposSelecionados;
            var subgruposPadraoFiltro = ProdutoConfig.TelaPrecoTabelaClienteRelatorio.SubgruposPadraoFiltro;

            this.UsarLiberacaoPedido = PedidoConfig.LiberarPedido;
            this.SubgruposPadraoParaFiltro = !string.IsNullOrWhiteSpace(subgruposPadraoFiltro) ? subgruposPadraoFiltro.Split(',') : null;
            this.AlterarSubgruposSelecionados = new IdNomeDto
            {
                Id = idNomeAlterarSubgruposSelecionados.Key != null
                    ? int.Parse(idNomeAlterarSubgruposSelecionados.Key)
                    : 0,

                Nome = idNomeAlterarSubgruposSelecionados.Key != null
                    ? idNomeAlterarSubgruposSelecionados.Value
                    : string.Empty,
            };
        }

        /// <summary>
        /// Obtém ou define um valor que indica se a liberação de pedidos está habilitada.
        /// </summary>
        [DataMember]
        [JsonProperty("usarLiberacaoPedido")]
        public bool UsarLiberacaoPedido { get; set; }

        /// <summary>
        /// Obtém ou define os subgrupos padrões para uso no controle da tela.
        /// </summary>
        [DataMember]
        [JsonProperty("subgruposPadraoParaFiltro")]
        public IEnumerable<string> SubgruposPadraoParaFiltro { get; set; }

        /// <summary>
        /// Obtém ou define um subgrupo padrão que deve ficar selecionado no controle da tela.
        /// </summary>
        [DataMember]
        [JsonProperty("alterarSubgruposSelecionados")]
        public IdNomeDto AlterarSubgruposSelecionados { get; set; }
    }
}