using System;
using Glass.Data.RelDAL;
using GDA;

namespace Glass.Data.RelModel
{
    public enum TipoContaPagarReceber
    {
        Pagar=1,
        Receber
    }

    [PersistenceBaseDAO(typeof(ContasPagarReceberDAO))]
    public class ContasPagarReceber
    {
        #region Propiedades

        public uint IdContaPagRec { get; set; }

        public string Referencia { get; set; }

        public uint IdCliFornec { get; set; }

        public string NomeCliFornec { get; set; }

        public decimal ValorVencPag { get; set; }

        public decimal ValorVencRec { get; set; }

        public DateTime DataVenc { get; set; }

        public TipoContaPagarReceber TipoConta { get; set; }

        public string Criterio { get; set; }

        #endregion

        #region propiedades de Suporte

        public string DataVencStr { get { return DataVenc.ToString("dd/MM/yyyy"); } }

        public string ValorVencPagStr { get { return ValorVencPag.ToString("C"); } }

        public string ValorVencRecStr { get { return ValorVencRec.ToString("C"); } }

        public string TipoContaStr
        {
            get
            {
                switch (TipoConta)
                {
                    case TipoContaPagarReceber.Pagar:
                        return "A Pagar";
                    case TipoContaPagarReceber.Receber:
                        return "A Receber";
                    default:
                        return "";
                }
            }
        }

        public string IdNomeCliFornec { get { return IdCliFornec + " - " + NomeCliFornec; } }

        #endregion
    }
}
