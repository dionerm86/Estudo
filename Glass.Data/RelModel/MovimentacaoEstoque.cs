using System;
using GDA;
using Glass.Data.Model;
using Glass.Data.RelDAL;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(MovimentacaoEstoqueDAO))]
    [PersistenceClass("movimentacao_estoque")]
    public class MovimentacaoEstoque : Sync.Fiscal.EFD.Entidade.IMovimentacaoEstoque
    {
        #region Propriedades

        [PersistenceProperty("IDMOVESTOQUE")]
        public uint IdMovEstoque { get; set; }

        [PersistenceProperty("IDPROD")]
        public uint IdProd { get; set; }

        [PersistenceProperty("CODINTERNOPROD")]
        public string CodInternoProd { get; set; }

        [PersistenceProperty("DESCRPROD")]
        public string DescrProd { get; set; }

        [PersistenceProperty("NCM")]
        public string Ncm { get; set; }

        [PersistenceProperty("DESCRTIPOPROD")]
        public string DescrTipoProd { get; set; }

        [PersistenceProperty("DATA")]
        public DateTime Data { get; set; }

        [PersistenceProperty("NUMERODOCUMENTO")]
        public string NumeroDocumento { get; set; }

        [PersistenceProperty("TIPO")]
        public long Tipo { get; set; }

        [PersistenceProperty("QTDE")]
        public decimal Qtde { get; set; }

        [PersistenceProperty("VALOR")]
        public decimal Valor { get; set; }

        [PersistenceProperty("ESTOQUEINICIAL")]
        public bool EstoqueInicial { get; set; }

        private long _tipoCalc;

        [PersistenceProperty("TIPOCALC")]
        public long TipoCalc
        {
            get { return _tipoCalc > 0 ? _tipoCalc : (int)Glass.Data.Model.TipoCalculoGrupoProd.Qtd; }
            set { _tipoCalc = value; }
        }

        [PersistenceProperty("UNIDADEPROD")]
        public string UnidadeProd { get; set; }

        [PersistenceProperty("CRITERIO")]
        public string Criterio { get; set; }

        #endregion

        #region Saldo

        [PersistenceProperty("QTDESALDO")]
        public decimal QtdeSaldo { get; set; }

        [PersistenceProperty("VALORSALDO")]
        public decimal ValorSaldo { get; set; }

        [PersistenceProperty("VALORSALDOANT")]
        public decimal ValorSaldoAnt { get; set; }

        public decimal PrecoMedioSaldo
        {
            get { return QtdeSaldo != 0 ? Math.Abs(ValorSaldo) / (decimal)QtdeSaldo : 0; }
        }
        
        #endregion

        #region Propriedades de Suporte

        public string DescrTipo
        {
            get
            {
                switch ((MovEstoque.TipoMovEnum)Tipo)
                {
                    case MovEstoque.TipoMovEnum.Entrada: return "Entrada";
                    case MovEstoque.TipoMovEnum.Saida: return "Saída";
                    default: return "";
                }
            }
        }

        public string Unidade
        {
            get { return Glass.Global.CalculosFluxo.GetDescrTipoCalculo((int)TipoCalc, true); }
        }

        #region Valores entrada

        public decimal QtdeEntrada
        {
            get { return Tipo == (int)MovEstoque.TipoMovEnum.Entrada ? Qtde : 0; }
        }

        public string QtdeEntradaRpt
        {
            get { return QtdeEntrada.ToString("0.".PadRight(2 + Configuracoes.Geral.NumeroCasasDecimaisTotM, '#')); }
        }

        public decimal ValorUnitMovEntrada
        {
            get { return Tipo == (int)MovEstoque.TipoMovEnum.Entrada ? (Qtde != 0 ? ValorEntrada / Qtde : 0) : 0; }
        }

        public string ValorUnitMovEntradaRpt
        {
            get { return ValorUnitMovEntrada.ToString("C"); }
        }

        public decimal ValorEntrada
        {
            get { return Tipo == (int)MovEstoque.TipoMovEnum.Entrada ? Valor : 0; }
        }

        public string ValorEntradaRpt
        {
            get { return ValorEntrada.ToString("C"); }
        }

        public decimal ValorSomarEntrada
        {
            get { return Tipo == (int)MovEstoque.TipoMovEnum.Entrada ? (Valor > ValorSaldo ? ValorSaldo : Valor) : 0; }
        }

        public string ValorSomarEntradaRpt
        {
            get { return ValorSomarEntrada.ToString("C"); }
        }

        #endregion

        #region Valores saída

        public decimal QtdeSaida
        {
            get { return Tipo == (int)MovEstoque.TipoMovEnum.Entrada ? 0 : Qtde; }
        }

        public string QtdeSaidaRpt
        {
            get { return QtdeSaida.ToString("0.".PadRight(2 + Configuracoes.Geral.NumeroCasasDecimaisTotM, '#')); }
        }

        public decimal ValorUnitMovSaida
        {
            get { return Tipo == (int)MovEstoque.TipoMovEnum.Entrada ? 0 : (Qtde != 0 ? ValorSaida / Qtde : 0); }
        }

        public string ValorUnitMovSaidaRpt
        {
            get { return ValorUnitMovSaida.ToString("C"); }
        }

        public decimal ValorSaida
        {
            get { return Tipo == (int)MovEstoque.TipoMovEnum.Entrada ? 0 : (Valor > 0 ? Valor : (ValorSaldoAnt > 0 ? ValorSaldoAnt : 0)); }
        }

        public string ValorSaidaRpt
        {
            get { return ValorSaida.ToString("C"); }
        }

        public decimal ValorSomarSaida
        {
            get { return Tipo == (int)MovEstoque.TipoMovEnum.Entrada ? 0 : ValorSaida; }
        }

        public string ValorSomarSaidaRpt
        {
            get { return ValorSomarSaida.ToString("C"); }
        }

        #endregion

        public string ComplementoQtd { get; set; }

        public decimal QtdeSaldoRealFiscal { get; set; }

        public string ComplementoQtdRealFiscal { get; set; }

        public decimal ValorVenda { get; set; }

        public decimal ValorTotalVenda { get; set; }

        #endregion

        #region IMovimentacaoEstoque Members

        int Sync.Fiscal.EFD.Entidade.IMovimentacaoEstoque.CodigoProduto
        {
            get { return (int)IdProd; }
        }

        decimal Sync.Fiscal.EFD.Entidade.IMovimentacaoEstoque.SaldoQuantidade
        {
            get { return QtdeSaldo; }
        }

        decimal Sync.Fiscal.EFD.Entidade.IMovimentacaoEstoque.SaldoValor
        {
            get { return ValorSaldo; }
        }

        #endregion
    }
}