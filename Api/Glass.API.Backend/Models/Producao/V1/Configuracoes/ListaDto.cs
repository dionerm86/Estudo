// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Producao.V1.Lista;
using Glass.Configuracoes;
using Glass.Data.Helper;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Producao.V1.Configuracoes
{
    /// <summary>
    /// Classe que encapsula as configurações para a tela de consulta de produção.
    /// </summary>
    [DataContract(Name = "Configuracoes")]
    public class ListaDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        public ListaDto()
        {
            this.EmpresaTrabalhaComAlturaELargura = PedidoConfig.EmpresaTrabalhaAlturaLargura;
            this.ExibirSetores = UserInfo.GetUserInfo.TipoUsuario != (int)Utils.TipoFuncionario.Vendedor
                || !ProducaoConfig.TelaConsulta.EsconderLinksImpressaoParaVendedores;

            this.ExibirRelatorios = this.ExibirSetores;
            this.ExibirNumeroEtiquetaNoInicioDaTabela = ProducaoConfig.TelaConsulta.ExibirNumeroEtiquetaNoInicioDaTabela;
            this.UsarControleDeChapaDeCorte = PCPConfig.Etiqueta.UsarControleChapaCorte;
            this.UsarOrdemDeCarga = OrdemCargaConfig.UsarControleOrdemCarga;
            this.ControleReposicaoPorPeca = ProducaoConfig.TipoControleReposicao == DataSources.TipoReposicaoEnum.Peca;
            this.UsarDataFabrica = ProducaoConfig.BuscarDataFabricaConsultaProducao;
            this.EmpresaLiberaPedido = PedidoConfig.LiberarPedido;
            this.EmpresaControlaCavaletes = PCPConfig.ControleCavalete;
            this.SetoresExibicao = this.ObterSetoresExibicao();
            this.IdGrupoProdutoVidro = Data.Model.NomeGrupoProd.Vidro;
            this.TipoSituacaoProducao = Lista.TipoSituacaoProducao.ApenasProdutosSetorAtual;
            this.TiposPecasExibir = this.ObterTiposPecasExibir();
            this.TipoProdutoComposicao = Lista.TipoProdutoComposicao.NaoIncluirProdutosComposicao;
        }

        /// <summary>
        /// Obtém ou define um valor que indica se a empresa trabalha com altura e largura
        /// (ao invés de largura e altura).
        /// </summary>
        [DataMember]
        [JsonProperty("empresaTrabalhaComAlturaELargura")]
        public bool EmpresaTrabalhaComAlturaELargura { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o usuário atual pode visualizar os setores.
        /// </summary>
        [DataMember]
        [JsonProperty("exibirSetores")]
        public bool ExibirSetores { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o usuário atual pode abrir os relatórios.
        /// </summary>
        [DataMember]
        [JsonProperty("exibirRelatorios")]
        public bool ExibirRelatorios { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o número de etiqueta da peça será
        /// exibido no início da tabela de produção.
        /// </summary>
        [DataMember]
        [JsonProperty("exibirNumeroEtiquetaNoInicioDaTabela")]
        public bool ExibirNumeroEtiquetaNoInicioDaTabela { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se a empresa controla os cortes de chapa.
        /// </summary>
        [DataMember]
        [JsonProperty("usarControleDeChapaDeCorte")]
        public bool UsarControleDeChapaDeCorte { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se a empresa usa ordem de carga.
        /// </summary>
        [DataMember]
        [JsonProperty("usarOrdemDeCarga")]
        public bool UsarOrdemDeCarga { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se a empresa faz controle de reposição por peças.
        /// </summary>
        [DataMember]
        [JsonProperty("controleReposicaoPorPeca")]
        public bool ControleReposicaoPorPeca { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se a data de fábrica será usada na tela de consulta.
        /// </summary>
        [DataMember]
        [JsonProperty("usarDataFabrica")]
        public bool UsarDataFabrica { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se a empresa realiza liberação de pedidos.
        /// </summary>
        [DataMember]
        [JsonProperty("empresaLiberaPedido")]
        public bool EmpresaLiberaPedido { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se a empresa controla os cavaletes de produção.
        /// </summary>
        [DataMember]
        [JsonProperty("empresaControlaCavaletes")]
        public bool EmpresaControlaCavaletes { get; set; }

        /// <summary>
        /// Obtém ou define os setores que serão exibidos na tela de consulta de produção.
        /// </summary>
        [DataMember]
        [JsonProperty("setoresExibicao")]
        public IEnumerable<SetorDto> SetoresExibicao { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do grupo de produtos 'vidro'.
        /// </summary>
        [DataMember]
        [JsonProperty("idGrupoProdutoVidro")]
        public Data.Model.NomeGrupoProd IdGrupoProdutoVidro { get; set; }

        /// <summary>
        /// Obtém ou define o tipo padrão de situação de produção.
        /// </summary>
        [DataMember]
        [JsonProperty("tipoSituacaoProducao")]
        public TipoSituacaoProducao? TipoSituacaoProducao { get; set; }

        /// <summary>
        /// Obtém ou define os tipos padrão para exibição das peças.
        /// </summary>
        [DataMember]
        [JsonProperty("tiposPecasExibir")]
        public IEnumerable<TipoPecaExibir> TiposPecasExibir { get; set; }

        /// <summary>
        /// Obtém ou define o tipo padrão de produto de composição.
        /// </summary>
        [DataMember]
        [JsonProperty("tipoProdutoComposicao")]
        public TipoProdutoComposicao? TipoProdutoComposicao { get; set; }

        private IEnumerable<SetorDto> ObterSetoresExibicao()
        {
            if (!this.ExibirSetores)
            {
                return new SetorDto[0];
            }

            return Utils.GetSetores
                .Where(s => s.ExibirRelatorio)
                .Select(s => new SetorDto
                {
                    Id = s.IdSetor,
                    Nome = s.Descricao,
                    Ordem = s.NumeroSequencia,
                    PertencenteARoteiro = s.SetorPertenceARoteiro,
                });
        }

        private IEnumerable<TipoPecaExibir> ObterTiposPecasExibir()
        {
            var retorno = new List<TipoPecaExibir> { TipoPecaExibir.EmProducao };

            if (PCPConfig.ExibirPecasCancMaoObraPadrao)
            {
                retorno.Add(TipoPecaExibir.CanceladasMaoDeObra);
            }

            return retorno;
        }
    }
}
