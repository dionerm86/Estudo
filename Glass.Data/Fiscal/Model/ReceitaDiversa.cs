using System;
using GDA;
using Glass.Data.EFD;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ReceitaDiversaDAO))]
    [PersistenceClass("receitas_diversas")]
    public class ReceitaDiversa : Sync.Fiscal.EFD.Entidade.IReceitaDiversa
    {
        #region Propriedades

        [PersistenceProperty("IdReceita", PersistenceParameterType.IdentityKey)]
        public uint IdReceita { get; set; }

        [PersistenceProperty("IdLoja")]
        public uint IdLoja { get; set; }

        [PersistenceProperty("TipoReceita")]
        public Sync.Fiscal.Enumeracao.ReceitaDiversa.TipoReceita TipoReceita { get; set; }

        [PersistenceProperty("TipoOperacao")]
        public Sync.Fiscal.Enumeracao.ReceitaDiversa.TipoOperacao TipoOperacao { get; set; }

        [PersistenceProperty("DataReceita")]
        public DateTime DataReceita { get; set; }

        [PersistenceProperty("ValorReceita")]
        public decimal ValorReceita { get; set; }

        [PersistenceProperty("CstPis")]
        public int? CstPis { get; set; }

        [PersistenceProperty("BcPis")]
        public decimal BcPis { get; set; }

        [PersistenceProperty("AliqPis")]
        public float AliquotaPis { get; set; }

        [PersistenceProperty("CstCofins")]
        public int? CstCofins { get; set; }

        [PersistenceProperty("BcCofins")]
        public decimal BcCofins { get; set; }

        [PersistenceProperty("AliqCofins")]
        public float AliquotaCofins { get; set; }

        [PersistenceProperty("NatBcCred")]
        public int? NaturezaBcCredito { get; set; }

        [PersistenceProperty("IndOrigemCred")]
        public Sync.Fiscal.Enumeracao.ReceitaDiversa.IndicadorOrigemCredito IndOrigemCred { get; set; }

        [PersistenceProperty("IdContaContabil")]
        public uint? IdContaContabil { get; set; }

        [PersistenceProperty("IdCentroCusto")]
        public uint? IdCentroCusto { get; set; }

        [PersistenceProperty("Descricao")]
        public string Descricao { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("NomeLoja", DirectionParameter.InputOptional)]
        public string NomeLoja { get; set; }

        [PersistenceProperty("CodInternoContaContabil", DirectionParameter.InputOptional)]
        public string CodInternoContaContabil { get; set; }

        [PersistenceProperty("DescricaoContaContabil", DirectionParameter.InputOptional)]
        public string DescricaoContaContabil { get; set; }

        [PersistenceProperty("DescricaoCentroCusto", DirectionParameter.InputOptional)]
        public string DescricaoCentroCusto { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string TipoReceitaString
        {
            get
            {
                return DataSourcesEFD.Instance.GetDescrTipoReceita((int?)TipoReceita);
            }
        }

        public string TipoOperacaoString
        {
            get
            {
                return DataSourcesEFD.Instance.GetDescrTipoOperacao((int?)TipoOperacao);
            }
        }

        public string CstPisString
        {
            get
            {
                return DataSourcesEFD.Instance.GetDescrCstPisCofins((int?)CstPis);
            }
        }

        public string CstCofinsString
        {
            get
            {
                return DataSourcesEFD.Instance.GetDescrCstPisCofins((int?)CstCofins);
            }
        }

        public string NaturezaBcCreditoString
        {
            get
            {
                return DataSourcesEFD.Instance.GetDescrNaturezaBcCredito((int?)NaturezaBcCredito);
            }
        }

        public string IndOrigemCredString
        {
            get
            {
                return DataSourcesEFD.Instance.GetDescrIndOrigemCred((int?)IndOrigemCred);
            }
        }

        #endregion

        #region IReceitaDiversa Members

        int Sync.Fiscal.EFD.Entidade.IReceitaDiversa.CodigoLoja
        {
            get { return (int)IdLoja; }
        }

        int? Sync.Fiscal.EFD.Entidade.IReceitaDiversa.CodigoContaContabil
        {
            get { return (int?)IdContaContabil; }
        }

        Sync.Fiscal.EFD.DataSources.NaturezaBcCredito Sync.Fiscal.EFD.Entidade.IReceitaDiversa.NaturezaBcCredito
        {
            get { return (Sync.Fiscal.EFD.DataSources.NaturezaBcCredito)NaturezaBcCredito; }
        }

        Sync.Fiscal.Enumeracao.ReceitaDiversa.IndicadorOrigemCredito Sync.Fiscal.EFD.Entidade.IReceitaDiversa.IndicadorOrigemCredito
        {
            get { return IndOrigemCred; }
        }

        Sync.Fiscal.Enumeracao.Cst.CstPisCofins? Sync.Fiscal.EFD.Entidade.IReceitaDiversa.CstPis
        {
            get { return (Sync.Fiscal.Enumeracao.Cst.CstPisCofins?)CstPis; }
        }

        float Sync.Fiscal.EFD.Entidade.IReceitaDiversa.AliqPis
        {
            get { return AliquotaPis; }
        }

        Sync.Fiscal.Enumeracao.Cst.CstPisCofins? Sync.Fiscal.EFD.Entidade.IReceitaDiversa.CstCofins
        {
            get { return (Sync.Fiscal.Enumeracao.Cst.CstPisCofins?)CstCofins; }
        }

        float Sync.Fiscal.EFD.Entidade.IReceitaDiversa.AliqCofins
        {
            get { return AliquotaCofins; }
        }

        #endregion
    }
}