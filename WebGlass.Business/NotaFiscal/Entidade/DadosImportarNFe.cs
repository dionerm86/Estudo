using System.Collections.Generic;

namespace WebGlass.Business.NotaFiscal.Entidade
{
    public class DadosImportarNFe
    {
        public string ChaveAcesso { get; set; }
        public Dictionary<string, object> NaturezaOperacaoProd { get; set; }
        public uint IdFormaPagto { get; set; }
        public uint IdPlanoConta { get; set; }
        public uint IdNaturezaOperacao { get; set; }
        public uint? IdCompra { get; set; }
    }
}
