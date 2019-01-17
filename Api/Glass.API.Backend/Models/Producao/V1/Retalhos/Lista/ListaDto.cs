// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Glass.Data.Model;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Producao.V1.Retalhos.Lista
{
    /// <summary>
    /// Classe que encapsula um item para a lista de retalhos de produção.
    /// </summary>
    [DataContract(Name = "Lista")]
    public class ListaDto : IdDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="retalhoProducao">A model de retalho de produção.</param>
        public ListaDto(RetalhoProducao retalhoProducao)
        {
            this.Id = retalhoProducao.IdRetalhoProducao;
            this.Produto = new ProdutoDto
            {
                Codigo = retalhoProducao.CodInterno,
                Descricao = retalhoProducao.Descricao,
            };

            this.Medidas = new MedidasDto
            {
                Largura = retalhoProducao.Largura,
                Altura = retalhoProducao.Altura,
                MetroQuadrado = new MetroQuadradoDto
                {
                    Total = (decimal)retalhoProducao.TotM,
                    Usando = (decimal)retalhoProducao.TotMUsando,
                },
            };

            this.Datas = new DatasDto
            {
                Cadastro = retalhoProducao.DataCad,
                Uso = retalhoProducao.DataUso,
            };

            this.Situacao = retalhoProducao.SituacaoString;
            this.CodigoEtiqueta = retalhoProducao.NumeroEtiqueta;
            this.NumeroNotaFiscal = retalhoProducao.NumeroNFe;
            this.Lote = retalhoProducao.Lote;
            this.CodigosEtiquetaUsandoRetalho = retalhoProducao.EtiquetaUsando;
            this.Funcionario = retalhoProducao.NomeFunc;
            this.Observacao = retalhoProducao.Obs;
            this.Permissoes = new PermissoesDto
            {
                Imprimir = Config.PossuiPermissao(Config.FuncaoMenuPCP.ImprimirEtiquetas) && retalhoProducao.CancelarVisible,
                Cancelar = retalhoProducao.CancelarVisible,
                MarcarPerda = retalhoProducao.PerdaVisible,
                ExibirEtiquetasUsando = !string.IsNullOrWhiteSpace(retalhoProducao.EtiquetaUsando),
                LogAlteracoes = LogAlteracaoDAO.Instance.TemRegistro(LogAlteracao.TabelaAlteracao.RetalhoProducao, (uint)retalhoProducao.IdRetalhoProducao, null),
                LogCancelamento = LogCancelamentoDAO.Instance.TemRegistro(LogCancelamento.TabelaCancelamento.RetalhoProducao, (uint)retalhoProducao.IdRetalhoProducao),
            };
        }

        /// <summary>
        /// Obtém ou define os dados referentes ao produto associado ao retalho.
        /// </summary>
        [DataMember]
        [JsonProperty("produto")]
        public ProdutoDto Produto { get; set; }

        /// <summary>
        /// Obtém ou define os dados das medidas referentes ao retalho de produção.
        /// </summary>
        [DataMember]
        [JsonProperty("medidas")]
        public MedidasDto Medidas { get; set; }

        /// <summary>
        /// Obtém ou define os dados das datas associadas ao retalho de produção.
        /// </summary>
        [DataMember]
        [JsonProperty("datas")]
        public DatasDto Datas { get; set; }

        /// <summary>
        /// Obtém ou define a situação do retalho de produção.
        /// </summary>
        [DataMember]
        [JsonProperty("situacao")]
        public string Situacao { get; set; }

        /// <summary>
        /// Obtém ou define o código da etiqueta do retalho de produção.
        /// </summary>
        [DataMember]
        [JsonProperty("codigoEtiqueta")]
        public string CodigoEtiqueta { get; set; }

        /// <summary>
        /// Obtém ou define o número da nota fiscal referente ao retalho de produção.
        /// </summary>
        [DataMember]
        [JsonProperty("numeroNotaFiscal")]
        public int? NumeroNotaFiscal { get; set; }

        /// <summary>
        /// Obtém ou define o lote do retalho de produção.
        /// </summary>
        [DataMember]
        [JsonProperty("lote")]
        public string Lote { get; set; }

        /// <summary>
        /// Obtém ou define os códigos de etiqueta que estão utilizando o retalho.
        /// </summary>
        [DataMember]
        [JsonProperty("codigosEtiquetaUsandoRetalho")]
        public string CodigosEtiquetaUsandoRetalho { get; set; }

        /// <summary>
        /// Obtém ou define o funcionário que cadastrou o retalho.
        /// </summary>
        [DataMember]
        [JsonProperty("funcionario")]
        public string Funcionario { get; set; }

        /// <summary>
        /// Obtém ou define a observação referente ao retalho de produção.
        /// </summary>
        [DataMember]
        [JsonProperty("observacao")]
        public string Observacao { get; set; }

        /// <summary>
        /// Obtém ou define as permissões concebidas ao retalho de produção.
        /// </summary>
        [DataMember]
        [JsonProperty("permissoes")]
        public PermissoesDto Permissoes { get; set; }
    }
}