using System;
using System.Collections.Generic;
using GDA;
using Glass.Data.EFD;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(AjusteApuracaoIdentificacaoDocFiscalDAO))]
    [PersistenceClass("sped_ajuste_apuracao_ident_doc_fiscal")]
    public class AjusteApuracaoIdentificacaoDocFiscal : Sync.Fiscal.EFD.Entidade.IAjusteApuracaoIdentificacaoDocFiscal
    {
        #region Propriedades

        [PersistenceProperty("Id", PersistenceParameterType.IdentityKey)]
        public uint Id { get; set; }

        [PersistenceProperty("IdABIA")]
        public uint IdABIA { get; set; }

        [PersistenceProperty("IdProd")]
        public uint IdProd { get; set; }

        [PersistenceProperty("IdNf")]
        public uint IdNf { get; set; }

        [PersistenceProperty("CodMod")]
        public string CodMod { get; set; }

        [PersistenceProperty("ValAjItem")]
        public decimal ValAjItem { get; set; }

        [PersistenceProperty("TIPOIMPOSTO")]
        public int TipoImposto { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("DescricaoProduto", DirectionParameter.InputOptional)]
        public string DescricaoProduto { get; set; }

        [PersistenceProperty("NumeroNFE", DirectionParameter.InputOptional)]
        public uint? NumeroNFE { get; set; }

        [PersistenceProperty("Serie", DirectionParameter.InputOptional)]
        public string Serie { get; set; }

        [PersistenceProperty("SubSerie", DirectionParameter.InputOptional)]
        public uint? SubSerie { get; set; }

        [PersistenceProperty("DataEmissao", DirectionParameter.InputOptional)]
        public DateTime? DataEmissao { get; set; }

        [PersistenceProperty("CodigoAjuste", DirectionParameter.InputOptional)]
        public string CodigoAjuste { get; set; }

        [PersistenceProperty("DataAjuste", DirectionParameter.InputOptional)]
        public DateTime DataAjuste { get; set; }

        [PersistenceProperty("ObservacaoAjuste", DirectionParameter.InputOptional)]
        public string ObservacaoAjuste { get; set; }

        [PersistenceProperty("CodInternoProduto", DirectionParameter.InputOptional)]
        public string CodInternoProduto { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string DescricaoCodMod
        {
            get
            {
                List<TabelaSped> lista = DataSourcesEFD.Instance.GetTabelaDocumentosFiscaisICMS();

                TabelaSped obj = lista.Find(delegate(TabelaSped p) { return p.Codigo == CodMod; });

                if (obj != null)
                    return obj.Descricao;

                return "";
            }
        }

        public string DescricaoAjuste
        {
            get
            {
                return CodigoAjuste + " - " + DataAjuste.ToString("dd/MM/yyyy") + " - " + ObservacaoAjuste;
            }
        }

        public string DescricaoTipoImposto
        {
            get
            {
                switch (TipoImposto)
                {
                    case (int)ConfigEFD.TipoImpostoEnum.ICMS: return "ICMS";
                    case (int)ConfigEFD.TipoImpostoEnum.ICMSST: return "ICMS ST";
                    case (int)ConfigEFD.TipoImpostoEnum.IPI: return "IPI";
                }

                return "";
            }
        }

        #endregion

        #region IAjusteApuracaoIdentificacaoDocFiscal Members

        int Sync.Fiscal.EFD.Entidade.IAjusteApuracaoIdentificacaoDocFiscal.CodigoNFe
        {
            get { return (int)IdNf; }
        }

        int Sync.Fiscal.EFD.Entidade.IAjusteApuracaoIdentificacaoDocFiscal.CodigoProduto
        {
            get { return (int)IdProd; }
        }

        string Sync.Fiscal.EFD.Entidade.IAjusteApuracaoIdentificacaoDocFiscal.SubSerie
        {
            get { return SubSerie.ToString(); }
        }

        int Sync.Fiscal.EFD.Entidade.IAjusteApuracaoIdentificacaoDocFiscal.NumeroNFe
        {
            get { return (int?)NumeroNFE ?? 0; }
        }

        DateTime Sync.Fiscal.EFD.Entidade.IAjusteApuracaoIdentificacaoDocFiscal.DataEmissao
        {
            get { return DataEmissao ?? new DateTime(); }
        }

        decimal Sync.Fiscal.EFD.Entidade.IAjusteApuracaoIdentificacaoDocFiscal.ValorAjusteItem
        {
            get { return ValAjItem; }
        }

        #endregion
    }
}