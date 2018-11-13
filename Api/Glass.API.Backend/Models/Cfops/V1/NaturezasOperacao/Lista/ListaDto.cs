// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.DAL;
using Glass.Data.Model;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Cfops.V1.NaturezasOperacao.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de uma natureza de operação para a tela de listagem.
    /// </summary>
    [DataContract(Name = "NaturezasOperacao")]
    public class ListaDto : IdCodigoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="natureza">A model de natureza de operação.</param>
        internal ListaDto(Fiscal.Negocios.Entidades.NaturezaOperacao natureza)
        {
            this.Id = natureza.IdNaturezaOperacao;
            this.IdCfop = natureza.IdCfop;
            this.Codigo = natureza.CodInterno;
            this.Mensagem = natureza.Mensagem;
            this.AlterarEstoqueFiscal = natureza.AlterarEstoqueFiscal;
            this.CalculoDeEnergiaEletrica = natureza.CalcEnergiaEletrica;
            this.Ncm = natureza.Ncm;
            this.DadosIcms = new DadosIcmsDto
            {
                CstIcms = new CodigoNomeDto
                {
                    Codigo = natureza.CstIcms,
                    Nome = natureza.CstIcms,
                },

                Csosn = new CodigoNomeDto
                {
                    Codigo = natureza.Csosn,
                    Nome = natureza.Csosn,
                },

                CalcularIcms = natureza.CalcIcms,
                CalcularIcmsSt = natureza.CalcIcmsSt,
                IpiIntegraBcIcms = natureza.IpiIntegraBcIcms,
                DebitarIcmsDesoneradoTotalNf = natureza.DebitarIcmsDesonTotalNf,
                PercentualReducaoBcIcms = (decimal)natureza.PercReducaoBcIcms,
                PercentualDiferimento = (decimal)natureza.PercDiferimento,
                CalcularDifal = natureza.CalcularDifal,
            };

            this.DadosIpi = new DadosIpiDto
            {
                CstIpi = new IdNomeDto
                {
                    Id = (int?)natureza.CstIpi,
                    Nome = Colosoft.Translator.Translate(natureza.CstIpi).Format(),
                },

                CalcularIpi = natureza.CalcIpi,
                FreteIntegraBcIpi = natureza.FreteIntegraBcIpi,
                CodigoEnquadramentoIpi = natureza.CodEnqIpi,
            };

            this.DadosPisCofins = new DadosPisCofinsDto
            {
                CstPisCofins = new IdNomeDto
                {
                    Id = natureza.CstPisCofins,
                    Nome = natureza.CstPisCofins?.ToString(),
                },

                CalcularPis = natureza.CalcPis,
                CalcularCofins = natureza.CalcCofins,
            };

            this.Permissoes = new PermissoesDto()
            {
                Excluir = !string.IsNullOrEmpty(natureza.CodInterno),
                LogAlteracoes = LogAlteracaoDAO.Instance.TemRegistro(LogAlteracao.TabelaAlteracao.NaturezaOperacao, (uint)natureza.IdNaturezaOperacao, null),
            };
        }

        /// <summary>
        /// Obtém ou define o identificador do CFOP.
        /// </summary>
        [DataMember]
        [JsonProperty("idCfop")]
        public int IdCfop { get; set; }

        /// <summary>
        /// Obtém ou define a mensagem da natureza de operação.
        /// </summary>
        [DataMember]
        [JsonProperty("mensagem")]
        public string Mensagem { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se a natureza de operação altera o estoque fiscal.
        /// </summary>
        [DataMember]
        [JsonProperty("alterarEstoqueFiscal")]
        public bool AlterarEstoqueFiscal { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se a natureza de operação será usada para cálculo de energia elétrica.
        /// </summary>
        [DataMember]
        [JsonProperty("calculoDeEnergiaEletrica")]
        public bool CalculoDeEnergiaEletrica { get; set; }

        /// <summary>
        /// Obtém ou define o NCM da natureza de operação.
        /// </summary>
        [DataMember]
        [JsonProperty("ncm")]
        public string Ncm { get; set; }

        /// <summary>
        /// Obtém ou define dados de ICMS da natureza de operação.
        /// </summary>
        [DataMember]
        [JsonProperty("dadosIcms")]
        public DadosIcmsDto DadosIcms { get; set; }

        /// <summary>
        /// Obtém ou define dados de IPI da natureza de operação.
        /// </summary>
        [DataMember]
        [JsonProperty("dadosIpi")]
        public DadosIpiDto DadosIpi { get; set; }

        /// <summary>
        /// Obtém ou define dados de PIS/COFINS da natureza de operação.
        /// </summary>
        [DataMember]
        [JsonProperty("dadosPisCofins")]
        public DadosPisCofinsDto DadosPisCofins { get; set; }

        /// <summary>
        /// Obtém ou define a lista de permissões do item.
        /// </summary>
        [DataMember]
        [JsonProperty("permissoes")]
        public PermissoesDto Permissoes { get; set; }
    }
}
