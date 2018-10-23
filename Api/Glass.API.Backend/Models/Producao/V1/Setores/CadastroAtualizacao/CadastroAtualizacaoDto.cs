// <copyright file="CadastroAtualizacaoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1.CadastroAtualizacao;
using Glass.Data.Model;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Producao.V1.Setores.CadastroAtualizacao
{
    /// <summary>
    /// Classe que encapsula os dados de cadastro ou atualização de um setor.
    /// </summary>
    [DataContract(Name = "CadastroAtualizacao")]
    public class CadastroAtualizacaoDto : BaseCadastroAtualizacaoDto<CadastroAtualizacaoDto>
    {
        /// <summary>
        /// Obtém ou define o nome do setor.
        /// </summary>
        [DataMember]
        [JsonProperty("nome")]
        public string Nome
        {
            get { return this.ObterValor(c => c.Nome); }
            set { this.AdicionarValor(c => c.Nome, value); }
        }

        /// <summary>
        /// Obtém ou define o código do setor.
        /// </summary>
        [DataMember]
        [JsonProperty("codigo")]
        public string Codigo
        {
            get { return this.ObterValor(c => c.Codigo); }
            set { this.AdicionarValor(c => c.Codigo, value); }
        }

        /// <summary>
        /// Obtém ou define a sequência do setor.
        /// </summary>
        [DataMember]
        [JsonProperty("sequencia")]
        public int Sequencia
        {
            get { return this.ObterValor(c => c.Sequencia); }
            set { this.AdicionarValor(c => c.Sequencia, value); }
        }

        /// <summary>
        /// Obtém ou define a situação do setor.
        /// </summary>
        [DataMember]
        [JsonProperty("situacao")]
        public Situacao Situacao
        {
            get { return this.ObterValor(c => c.Situacao); }
            set { this.AdicionarValor(c => c.Situacao, value); }
        }

        /// <summary>
        /// Obtém ou define a altura máxima de peças que podem entrar nesse setor.
        /// </summary>
        [DataMember]
        [JsonProperty("alturaMaxima")]
        public int AlturaMaxima
        {
            get { return this.ObterValor(c => c.AlturaMaxima); }
            set { this.AdicionarValor(c => c.AlturaMaxima, value); }
        }

        /// <summary>
        /// Obtém ou define a largura máxima de peças que podem entrar nesse setor.
        /// </summary>
        [DataMember]
        [JsonProperty("larguraMaxima")]
        public int LarguraMaxima
        {
            get { return this.ObterValor(c => c.LarguraMaxima); }
            set { this.AdicionarValor(c => c.LarguraMaxima, value); }
        }

        /// <summary>
        /// Obtém ou define a capacidade diária deste setor.
        /// </summary>
        [DataMember]
        [JsonProperty("capacidadeDiaria")]
        public int? CapacidadeDiaria
        {
            get { return this.ObterValor(c => c.CapacidadeDiaria); }
            set { this.AdicionarValor(c => c.CapacidadeDiaria, value); }
        }

        /// <summary>
        /// Obtém ou define um valor que indica se a capacidade diária deste setor será ignorada.
        /// </summary>
        [DataMember]
        [JsonProperty("ignorarCapacidadeDiaria")]
        public bool IgnorarCapacidadeDiaria
        {
            get { return this.ObterValor(c => c.IgnorarCapacidadeDiaria); }
            set { this.AdicionarValor(c => c.IgnorarCapacidadeDiaria, value); }
        }

        /// <summary>
        /// Obtém ou define as cores do setor.
        /// </summary>
        [DataMember]
        [JsonProperty("corSetor")]
        public CorSetor CorSetor
        {
            get { return this.ObterValor(c => c.CorSetor); }
            set { this.AdicionarValor(c => c.CorSetor, value); }
        }

        /// <summary>
        /// Obtém ou define as cores da tela do setor.
        /// </summary>
        [DataMember]
        [JsonProperty("corTela")]
        public CorTelaSetor CorTela
        {
            get { return this.ObterValor(c => c.CorTela); }
            set { this.AdicionarValor(c => c.CorTela, value); }
        }

        /// <summary>
        /// Obtém ou define um valor que indica se os setores que a peças deve passar serão exibidos ao efetuar a leitura na produção.
        /// </summary>
        [DataMember]
        [JsonProperty("exibirSetoresLeituraPeca")]
        public bool ExibirSetoresLeituraPeca
        {
            get { return this.ObterValor(c => c.ExibirSetoresLeituraPeca); }
            set { this.AdicionarValor(c => c.ExibirSetoresLeituraPeca, value); }
        }

        /// <summary>
        /// Obtém ou define um valor que indica se o setor será exibido na consulta de produção e nos relatórios.
        /// </summary>
        [DataMember]
        [JsonProperty("exibirNaListaERelatorio")]
        public bool ExibirNaListaERelatorio
        {
            get { return this.ObterValor(c => c.ExibirNaListaERelatorio); }
            set { this.AdicionarValor(c => c.ExibirNaListaERelatorio, value); }
        }

        /// <summary>
        /// Obtém ou define um valor que indica se o setor será exibido no painel comercial.
        /// </summary>
        [DataMember]
        [JsonProperty("exibirPainelComercial")]
        public bool ExibirPainelComercial
        {
            get { return this.ObterValor(c => c.ExibirPainelComercial); }
            set { this.AdicionarValor(c => c.ExibirPainelComercial, value); }
        }

        /// <summary>
        /// Obtém ou define um valor que indica se o setor será exibido no painel de produção.
        /// </summary>
        [DataMember]
        [JsonProperty("exibirPainelProducao")]
        public bool ExibirPainelProducao
        {
            get { return this.ObterValor(c => c.ExibirPainelProducao); }
            set { this.AdicionarValor(c => c.ExibirPainelProducao, value); }
        }

        /// <summary>
        /// Obtém ou define um valor que indica se ao ler peças neste setor, será exibida a imagem completa do projeto.
        /// </summary>
        [DataMember]
        [JsonProperty("exibirImagemCompleta")]
        public bool ExibirImagemCompleta
        {
            get { return this.ObterValor(c => c.ExibirImagemCompleta); }
            set { this.AdicionarValor(c => c.ExibirImagemCompleta, value); }
        }

        /// <summary>
        /// Obtém ou define um valor que indica se ao ler peças neste setor, será feita uma consulta antes da leitura.
        /// </summary>
        [DataMember]
        [JsonProperty("consultarAntesDaLeitura")]
        public bool ConsultarAntesDaLeitura
        {
            get { return this.ObterValor(c => c.ConsultarAntesDaLeitura); }
            set { this.AdicionarValor(c => c.ConsultarAntesDaLeitura, value); }
        }

        /// <summary>
        /// Obtém ou define o tipo do Setor.
        /// </summary>
        [DataMember]
        [JsonProperty("tipo")]
        public TipoSetor Tipo
        {
            get { return this.ObterValor(c => c.Tipo); }
            set { this.AdicionarValor(c => c.Tipo, value); }
        }

        /// <summary>
        /// Obtém ou define um valor que indica se o setor é de corte.
        /// </summary>
        [DataMember]
        [JsonProperty("corte")]
        public bool Corte
        {
            get { return this.ObterValor(c => c.Corte); }
            set { this.AdicionarValor(c => c.Corte, value); }
        }

        /// <summary>
        /// Obtém ou define um valor que indica se o setor é de forno.
        /// </summary>
        [DataMember]
        [JsonProperty("forno")]
        public bool Forno
        {
            get { return this.ObterValor(c => c.Forno); }
            set { this.AdicionarValor(c => c.Forno, value); }
        }

        /// <summary>
        /// Obtém ou define um valor que indica se o setor é de laminado.
        /// </summary>
        [DataMember]
        [JsonProperty("laminado")]
        public bool Laminado
        {
            get { return this.ObterValor(c => c.Laminado); }
            set { this.AdicionarValor(c => c.Laminado, value); }
        }

        /// <summary>
        /// Obtém ou define um valor que indica se o setor é de entrada de estoque.
        /// </summary>
        [DataMember]
        [JsonProperty("entradaEstoque")]
        public bool EntradaEstoque
        {
            get { return this.ObterValor(c => c.EntradaEstoque); }
            set { this.AdicionarValor(c => c.EntradaEstoque, value); }
        }

        /// <summary>
        /// Obtém ou define um valor que indica se é gerencidada fornada no setor.
        /// </summary>
        [DataMember]
        [JsonProperty("gerenciaFornada")]
        public bool GerenciaFornada
        {
            get { return this.ObterValor(c => c.GerenciaFornada); }
            set { this.AdicionarValor(c => c.GerenciaFornada, value); }
        }

        /// <summary>
        /// Obtém ou define o desafio de perda do setor.
        /// </summary>
        [DataMember]
        [JsonProperty("desafioPerda")]
        public decimal DesafioPerda
        {
            get { return this.ObterValor(c => c.DesafioPerda); }
            set { this.AdicionarValor(c => c.DesafioPerda, value); }
        }

        /// <summary>
        /// Obtém ou define a meta de perda do setor.
        /// </summary>
        [DataMember]
        [JsonProperty("metaPerda")]
        public decimal MetaPerda
        {
            get { return this.ObterValor(c => c.MetaPerda); }
            set { this.AdicionarValor(c => c.MetaPerda, value); }
        }

        /// <summary>
        /// Obtém ou define um valor que indica se é obrigatório fazer leitura neste setor antes de ler a peça em qualquer setor posterior.
        /// </summary>
        [DataMember]
        [JsonProperty("impedirAvanco")]
        public bool ImpedirAvanco
        {
            get { return this.ObterValor(c => c.ImpedirAvanco); }
            set { this.AdicionarValor(c => c.ImpedirAvanco, value); }
        }

        /// <summary>
        /// Obtém ou define um valor que indica se é obrigatório informar rota neste setor.
        /// </summary>
        [DataMember]
        [JsonProperty("informarRota")]
        public bool InformarRota
        {
            get { return this.ObterValor(c => c.InformarRota); }
            set { this.AdicionarValor(c => c.InformarRota, value); }
        }

        /// <summary>
        /// Obtém ou define um valor que indica se é obrigatório informar o cavalete neste setor.
        /// </summary>
        [DataMember]
        [JsonProperty("informarCavalete")]
        public bool InformarCavalete
        {
            get { return this.ObterValor(c => c.InformarCavalete); }
            set { this.AdicionarValor(c => c.InformarCavalete, value); }
        }

        /// <summary>
        /// Obtém ou define um valor que indica se é permitido fazer leitura neste setor mesmo não estando no roteiro da peça.
        /// </summary>
        [DataMember]
        [JsonProperty("permitirLeituraForaRoteiro")]
        public bool PermitirLeituraForaRoteiro
        {
            get { return this.ObterValor(c => c.PermitirLeituraForaRoteiro); }
            set { this.AdicionarValor(c => c.PermitirLeituraForaRoteiro, value); }
        }

        /// <summary>
        /// Obtém ou define o tempo máximo de login deste setor.
        /// </summary>
        [DataMember]
        [JsonProperty("tempoLogin")]
        public int TempoLogin
        {
            get { return this.ObterValor(c => c.TempoLogin); }
            set { this.AdicionarValor(c => c.TempoLogin, value); }
        }

        /// <summary>
        /// Obtém ou define um tempo para alerta de inatividade.
        /// </summary>
        [DataMember]
        [JsonProperty("tempoAlertaInatividade")]
        public int TempoAlertaInatividade
        {
            get { return this.ObterValor(c => c.TempoAlertaInatividade); }
            set { this.AdicionarValor(c => c.TempoAlertaInatividade, value); }
        }
    }
}
