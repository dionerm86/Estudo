using System;
using GDA;
using Glass.Data.DAL;
using Glass.Log;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(MovimentacaoBemAtivoImobDAO))]
    [PersistenceClass("movimentacao_bem_ativo_imob")]
    public class MovimentacaoBemAtivoImob : Sync.Fiscal.EFD.Entidade.IMovimentacaoBemAtivoImobilizado
    {
        #region Enumeradores

        public enum TipoEnum
        {
            SI = 1,
            IM = 2,
            IA = 3,
            CI = 4,
            MC = 5,
            BA = 6,
            AT = 7,
            PE = 8,
            OT = 9
        }

        #endregion

        #region Propriedades

        [PersistenceProperty("IDPRODNF", PersistenceParameterType.Key)]
        public uint IdProdNf { get; set; }

        [PersistenceProperty("IDBEMATIVOIMOBILIZADO")]
        public uint IdBemAtivoImobilizado { get; set; }

        [PersistenceProperty("TIPO")]
        public int? Tipo { get; set; }

        [Log("Valor ICMS Frete")]
        [PersistenceProperty("VALORICMSFRETE")]
        public decimal ValorIcmsFrete { get; set; }

        [Log("Valor ICMS Dif.")]
        [PersistenceProperty("VALORICMSDIF")]
        public decimal ValorIcmsDif { get; set; }

        [Log("Número Parc. ICMS")]
        [PersistenceProperty("NUMPARCICMS")]
        public long NumeroParcIcms { get; set; }

        [Log("Valor Parc. ICMS")]
        [PersistenceProperty("VALORPARCICMS")]
        public decimal ValorParcIcms { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("DATA", DirectionParameter.InputOptional)]
        public DateTime Data { get; set; }

        [PersistenceProperty("VALORICMS", DirectionParameter.InputOptional)]
        public decimal ValorIcms { get; set; }

        [PersistenceProperty("VALORICMSST", DirectionParameter.InputOptional)]
        public decimal ValorIcmsSt { get; set; }

        [PersistenceProperty("NUMERONFE", DirectionParameter.InputOptional)]
        public uint NumeroNFe { get; set; }

        [PersistenceProperty("TIPONFE", DirectionParameter.InputOptional)]
        public int TipoNFe { get; set; }

        [PersistenceProperty("DESCRPROD", DirectionParameter.InputOptional)]
        public string DescrProd { get; set; }

        [PersistenceProperty("IDNF", DirectionParameter.InputOptional)]
        public string IdNf { get; set; }

        #endregion

        #region Propriedades de Suporte

        [Log("Tipo")]
        public string DescricaoTipo
        {
            get
            {
                if (Tipo == null)
                    return "";

                switch ((TipoEnum)Tipo)
                {
                    case TipoEnum.SI: return "Saldo inicial de bens imobilizados";
                    case TipoEnum.IM: return "Imobilização de bem individual";
                    case TipoEnum.IA: return "Imobilização em Andamento - Componente";
                    case TipoEnum.CI: return "Conclusão de Imobilização em Andamento – Bem Resultante";
                    case TipoEnum.MC: return "Imobilização oriunda do Ativo Circulante";
                    case TipoEnum.BA: return "Baixa do bem - Fim do período de apropriação";
                    case TipoEnum.AT: return "Alienação ou Transferência";
                    case TipoEnum.PE: return "Perecimento, Extravio ou Deterioração";
                    case TipoEnum.OT: return "Outras Saídas do Imobilizado";
                    default: return "";
                }
            }
        }

        public string DescricaoTipoNFe
        {
            get { return NotaFiscal.GetTipoDocumento(TipoNFe); }
        }

        #endregion

        #region IMovimentacaoBemAtivoImobilizado Members

        string Sync.Fiscal.EFD.Entidade.IMovimentacaoBemAtivoImobilizado.CodigosNFe
        {
            get { return IdNf; }
        }

        int Sync.Fiscal.EFD.Entidade.IMovimentacaoBemAtivoImobilizado.CodigoBemAtivoImobilizado
        {
            get { return (int)IdBemAtivoImobilizado; }
        }

        Sync.Fiscal.Enumeracao.MovimentacaoBemAtivoImobilizado.Tipo Sync.Fiscal.EFD.Entidade.IMovimentacaoBemAtivoImobilizado.Tipo
        {
            get { return (Sync.Fiscal.Enumeracao.MovimentacaoBemAtivoImobilizado.Tipo)Tipo; }
        }

        int Sync.Fiscal.EFD.Entidade.IMovimentacaoBemAtivoImobilizado.NumeroParcelasIcms
        {
            get { return (int)NumeroParcIcms; }
        }

        decimal Sync.Fiscal.EFD.Entidade.IMovimentacaoBemAtivoImobilizado.ValorParcelasIcms
        {
            get { return ValorParcIcms; }
        }

        #endregion
    }
}