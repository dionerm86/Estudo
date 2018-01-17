using System;
using GDA;
using Glass.Data.DAL;
using Glass.Log;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(NaturezaOperacaoDAO))]
    [PersistenceClass("natureza_operacao")]
    public class NaturezaOperacao : Colosoft.Data.BaseModel, Sync.Fiscal.EFD.Entidade.INaturezaOperacao
    {
        #region Propriedades

        [PersistenceProperty("IDNATUREZAOPERACAO", PersistenceParameterType.IdentityKey)]
        public int IdNaturezaOperacao { get; set; }

        [Log("CFOP", "CodInterno", typeof(CfopDAO))]
        [PersistenceProperty("IDCFOP")]
        [PersistenceForeignKey(typeof(Cfop), "IdCfop")]
        public int IdCfop { get; set; }

        [Log("Código")]
        [PersistenceProperty("CODINTERNO")]
        public string CodInterno { get; set; }

        [Log("Mensagem")]
        [PersistenceProperty("MENSAGEM")]
        public string Mensagem { get; set; }

        [Log("Calcular ICMS")]
        [PersistenceProperty("CALCICMS")]
        public bool CalcIcms { get; set; }

        [Log("Calcular ICMS ST")]
        [PersistenceProperty("CALCICMSST")]
        public bool CalcIcmsSt { get; set; }

        [Log("Calcular IPI")]
        [PersistenceProperty("CALCIPI")]
        public bool CalcIpi { get; set; }

        [Log("Calcular PIS")]
        [PersistenceProperty("CALCPIS")]
        public bool CalcPis { get; set; }

        [Log("Calcular Cofins")]
        [PersistenceProperty("CALCCOFINS")]
        public bool CalcCofins { get; set; }

        [Log("IPI Integra Base de Cálculo ICMS")]
        [PersistenceProperty("IPIINTEGRABCICMS")]
        public bool IpiIntegraBcIcms { get; set; }

        [Log("Frete Integra Base de Cálculo IPI")]
        [PersistenceProperty("FRETEINTEGRABCIPI")]
        public bool FreteIntegraBcIpi { get; set; }

        [Log("Alterar Estoque Fiscal")]
        [PersistenceProperty("ALTERARESTOQUEFISCAL")]
        public bool AlterarEstoqueFiscal { get; set; }

        [Log("CST ICMS")]
        [PersistenceProperty("CSTICMS")]
        public string CstIcms { get; set; }

        [Log("Perc. Redução BC ICMS")]
        [PersistenceProperty("PERCREDUCAOBCICMS")]
        public float PercReducaoBcIcms { get; set; }

        [Log("CST IPI")]
        [PersistenceProperty("CSTIPI")]
        public ProdutoCstIpi? CstIpi { get; set; }

        [Log("CST Pis/Cofins")]
        [PersistenceProperty("CSTPISCOFINS")]
        public int? CstPisCofins { get; set; }

        [Log("CSOSN")]
        [PersistenceProperty("CSOSN")]
        public string Csosn { get; set; }

        [Log("Cód. Enq. IPI")]
        [PersistenceProperty("CODENQIPI")]
        public string CodEnqIpi { get; set; }
 
        [Log("Calcular Difal")]
        [PersistenceProperty("CALCULARDIFAL")]
        public bool CalcularDifal { get; set; }

        /// <summary>
        /// Indica que a natureza de operação é pra calcular icms de compra de energia elétrica
        /// </summary>
        [Log("Calc. Energia Elétrica")]
        [PersistenceProperty("CalcEnergiaEletrica")]
        public bool CalcEnergiaEletrica { get; set; }

        [Log("NCM")]
        [PersistenceProperty("Ncm")]
        public string Ncm { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("CODCFOP", DirectionParameter.InputOptional)]
        public string CodCfop { get; set; }

        [PersistenceProperty("DESCRICAOCFOP", DirectionParameter.InputOptional)]
        public string DescricaoCfop { get; set; }

        #endregion

        #region Propriedades de suporte

        public string CodCompleto
        {
            get { return CodCfop + (!String.IsNullOrEmpty(CodInterno) ? "-" + CodInterno : ""); }
        }

        /// <summary>
        /// Criado para que seja possível buscar a natureza de operação sem ter que antes inserir o código do CFOP.
        /// </summary>
        public string CodigoControleUsar
        {
            get { return !String.IsNullOrEmpty(CodInterno) ? CodInterno : CodCfop; }
        }

        #endregion

        #region INaturezaOperacao Members

        int Sync.Fiscal.EFD.Entidade.INaturezaOperacao.CodigoCfop
        {
            get { return IdCfop; }
        }

        bool Sync.Fiscal.EFD.Entidade.INaturezaOperacao.CalculaPis
        {
            get { return CalcPis; }
        }

        bool Sync.Fiscal.EFD.Entidade.INaturezaOperacao.CalculaCofins
        {
            get { return CalcCofins; }
        }

        #endregion
    }
}