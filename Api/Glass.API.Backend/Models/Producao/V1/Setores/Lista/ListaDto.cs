// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.DAL;
using Glass.Data.Model;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Producao.V1.Setores.Lista
{
    /// <summary>
    /// Classe que encapsula um item para a lista de setores.
    /// </summary>
    [DataContract(Name = "Lista")]
    public class ListaDto : IdNomeDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="setor">O turno que será retornado.</param>
        public ListaDto(PCP.Negocios.Entidades.Setor setor)
        {
            this.Id = setor.IdSetor;
            this.Nome = setor.Descricao;
            this.Codigo = setor.Sigla;
            this.Sequencia = setor.NumeroSequencia;
            this.Situacao = new IdNomeDto
            {
                Id = (int)setor.Situacao,
                Nome = Colosoft.Translator.Translate(setor.Situacao).Format(),
            };

            this.Capacidade = new CapacidadeDto
            {
                AlturaMaxima = setor.Altura,
                LarguraMaxima = setor.Largura,
                Diaria = setor.CapacidadeDiaria,
                IgnorarCapacidadeDiaria = setor.IgnorarCapacidadeDiaria,
            };

            this.Cores = new CoresDto
            {
                Setor = new IdNomeDto()
                {
                    Id = (int)setor.Cor,
                    Nome = Colosoft.Translator.Translate(setor.Cor).Format(),
                },

                Tela = new IdNomeDto()
                {
                    Id = (int)setor.CorTela,
                    Nome = Colosoft.Translator.Translate(setor.CorTela).Format(),
                },
            };

            this.Exibicoes = new ExibicoesDto()
            {
                SetoresLeituraPeca = setor.ExibirSetores,
                ListaERelatorio = setor.ExibirRelatorio,
                PainelComercial = setor.ExibirPainelComercial,
                PainelProducao = setor.ExibirPainelProducao,
                ImagemCompleta = setor.ExibirImagemCompleta,
                ConsultarAntesDaLeitura = setor.ConsultarAntes,
            };

            this.Funcoes = new FuncoesDto
            {
                Tipo = new IdNomeDto
                {
                    Id = (int)setor.Tipo,
                    Nome = Colosoft.Translator.Translate(setor.Tipo).Format(),
                },

                Corte = setor.Corte,
                Laminado = setor.Laminado,
                Forno = setor.Forno,
                EntradaEstoque = setor.EntradaEstoque,
                GerenciarFornada = setor.GerenciarFornada,
            };

            this.Perda = new PerdaDto
            {
                Desafio = (decimal)setor.DesafioPerda,
                Meta = (decimal)setor.MetaPerda,
            };

            this.Restricoes = new RestricoesDto
            {
                ImpedirAvanco = setor.ImpedirAvanco,
                InformarRota = setor.InformarRota,
                InformarCavalete = setor.InformarCavalete,
                PermitirLeituraForaRoteiro = setor.PermitirLeituraForaRoteiro,
            };

            this.TempoLogin = new TempoLoginDto
            {
                Maximo = setor.TempoLogin,
                AlertaInatividade = setor.TempoAlertaInatividade,
            };

            this.Permissoes = new PermissoesDto
            {
                LogAlteracoes = LogAlteracaoDAO.Instance.TemRegistro(
                    LogAlteracao.TabelaAlteracao.Setor,
                    (uint)setor.IdSetor,
                    null),
            };
        }

        /// <summary>
        /// Obtém ou define o código do setor.
        /// </summary>
        [DataMember]
        [JsonProperty("codigo")]
        public string Codigo { get; set; }

        /// <summary>
        /// Obtém ou define a sequência do setor.
        /// </summary>
        [DataMember]
        [JsonProperty("sequencia")]
        public int Sequencia { get; set; }

        /// <summary>
        /// Obtém ou define a situação do setor.
        /// </summary>
        [DataMember]
        [JsonProperty("situacao")]
        public IdNomeDto Situacao { get; set; }

        /// <summary>
        /// Obtém ou define dados da capacidade do setor.
        /// </summary>
        [DataMember]
        [JsonProperty("capacidade")]
        public CapacidadeDto Capacidade { get; set; }

        /// <summary>
        /// Obtém ou define cores do setor.
        /// </summary>
        [DataMember]
        [JsonProperty("cores")]
        public CoresDto Cores { get; set; }

        /// <summary>
        /// Obtém ou define visibilidade do setor nas telas do sistema.
        /// </summary>
        [DataMember]
        [JsonProperty("exibicoes")]
        public ExibicoesDto Exibicoes { get; set; }

        /// <summary>
        /// Obtém ou define funções do setor.
        /// </summary>
        [DataMember]
        [JsonProperty("funcoes")]
        public FuncoesDto Funcoes { get; set; }

        /// <summary>
        /// Obtém ou define dados de perda do setor.
        /// </summary>
        [DataMember]
        [JsonProperty("perda")]
        public PerdaDto Perda { get; set; }

        /// <summary>
        /// Obtém ou define restrições do setor.
        /// </summary>
        [DataMember]
        [JsonProperty("restricoes")]
        public RestricoesDto Restricoes { get; set; }

        /// <summary>
        /// Obtém ou define dados de login do setor.
        /// </summary>
        [DataMember]
        [JsonProperty("tempoLogin")]
        public TempoLoginDto TempoLogin { get; set; }

        /// <summary>
        /// Obtém ou define permissões do setor.
        /// </summary>
        [DataMember]
        [JsonProperty("permissoes")]
        public PermissoesDto Permissoes { get; set; }
    }
}
