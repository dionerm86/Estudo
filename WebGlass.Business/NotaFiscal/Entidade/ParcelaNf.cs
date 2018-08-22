using System;

namespace WebGlass.Business.NotaFiscal.Entidade
{
    /// <summary>
    /// Representa as parcelas da nota fiscal para a importação do XML
    /// </summary>
    public class ParcelaNf
    {
        /// <summary>
        /// Valor da parcela
        /// </summary>
        public decimal Valor { get; set; }
        /// <summary>
        /// Data de vencimento da parcela
        /// </summary>
        public DateTime DataVenc { get; set; }

        /// <summary>
        /// Numero do boleto
        /// </summary>
        public string NumBoleto { get; set; }
    }
}
