using System;
using System.Collections.Generic;
using GDA;
using Glass.Data.EFD;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ObrigacaoRecolhidoRecolherDAO))]
    [PersistenceClass("sped_obrigacoes_recolhido_recolher")]
    public class ObrigacaoRecolhidoRecolher : Sync.Fiscal.EFD.Entidade.IObrigacaoRecolhidoRecolher
    {
        #region Propriedades

        [PersistenceProperty("ID", PersistenceParameterType.IdentityKey)]
        public uint Id { get; set; }

        [PersistenceProperty("COD_OR")]
        public uint CodigoObrigacao { get; set; }

        [PersistenceProperty("VL_OR")]
        public decimal Valor { get; set; }

        [PersistenceProperty("DT_VCTO")]
        public DateTime DataVencimento { get; set; }

        [PersistenceProperty("COD_REC")]
        public uint CodigoReceita { get; set; }

        [PersistenceProperty("NUM_PROC")]
        public string NumeroProcesso { get; set; }

        [PersistenceProperty("IND_PROC")]
        public uint IndicadorOrigem { get; set; }

        [PersistenceProperty("PROC")]
        public string DescricaoProcesso { get; set; }

        [PersistenceProperty("TXT_COMPL")]
        public string DescricaoComplementar { get; set; }

        [PersistenceProperty("MES_REF")]
        public string MesReferencia { get; set; }

        [PersistenceProperty("TIPOIMPOSTO")]
        public int TipoImposto { get; set; }

        [PersistenceProperty("UF")]
        public string Uf { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("DescricaoCodigoReceita", DirectionParameter.InputOptional)]
        public string DescricaoCodigoReceita { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string DescricaoCodigoObrigacao
        {
            get
            {
                List<TabelaSped> lista = DataSourcesEFD.Instance.GetTabelaCodigoObrigacaoICMS();

                TabelaSped obj = lista.Find(delegate(TabelaSped p) { return p.Codigo == CodigoObrigacao.ToString("000"); });

                if (obj != null)
                    return obj.Descricao;

                return "";
            }
        }

        public string DescricaoIndProc
        {
            get
            {
                switch (IndicadorOrigem)
                {
                    case (int)Glass.Data.Model.AjusteApuracaoInfoAdicional.IndProcEnum.JusticaEstadual: return "Justiça Estadual";
                    case (int)Glass.Data.Model.AjusteApuracaoInfoAdicional.IndProcEnum.JusticaFederal: return "Justiça Federal";
                    case (int)Glass.Data.Model.AjusteApuracaoInfoAdicional.IndProcEnum.Outros: return "Outros";
                    case (int)Glass.Data.Model.AjusteApuracaoInfoAdicional.IndProcEnum.Sefaz: return "Sefaz";
                }

                return "";
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

        #region IObrigacaoRecolhidoRecolher Members

        int Sync.Fiscal.EFD.Entidade.IObrigacaoRecolhidoRecolher.CodigoObrigacao
        {
            get { return (int)CodigoObrigacao; }
        }

        int Sync.Fiscal.EFD.Entidade.IObrigacaoRecolhidoRecolher.CodigoReceita
        {
            get { return (int)CodigoReceita; }
        }

        int Sync.Fiscal.EFD.Entidade.IObrigacaoRecolhidoRecolher.IndicadorOrigem
        {
            get { return (int)IndicadorOrigem; }
        }

        #endregion
    }
}