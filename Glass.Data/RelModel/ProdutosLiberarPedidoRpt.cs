using Glass.Data.Model;
using GDA;
using Glass.Data.RelDAL;
using Glass.Data.DAL;
using Glass.Configuracoes;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(ProdutosLiberarPedidoRptDAL))]
    public class ProdutosLiberarPedidoRpt : IResumoCorte
    {
        #region Construtores

        public ProdutosLiberarPedidoRpt()
        {
        }

        public ProdutosLiberarPedidoRpt(ProdutosLiberarPedido plp)
        {
            IsPedidoReposicao = PedidoDAO.Instance.IsPedidoReposicao(null, plp.IdPedido.ToString()) &&
                !PedidoReposicaoDAO.Instance.PedidoParaTroca(plp.IdPedido);

            IdAmbientePedido = plp.IdAmbientePedido;
            IdGrupoProd = plp.IdGrupoProd;
            IdSubgrupoProd = plp.IdSubgrupoProd;
            IdPedido = plp.IdPedido;
            IsVidro = plp.IsVidro;
            Ambiente = plp.Ambiente;
            DescrGrupoProd = plp.DescrGrupoProd;
            PedCli = plp.PedCli;
            CodInternoProd = plp.CodInternoProd;
            DescrProduto = plp.DescrProduto;
            AltLarg1 = plp.AltLarg1;
            AltLarg2 = plp.AltLarg2;
            AltLargReal1 = plp.AltLargReal1;
            AltLargReal2 = plp.AltLargReal2;
            TituloAltLarg1 = plp.TituloAltLarg1;
            TituloAltLarg2 = plp.TituloAltLarg2;
            PedidoMaoDeObra = plp.PedidoMaoDeObra;
            TotM2 = plp.TotM2;
            TotM2Calc = plp.TotM2Calc;
            TotM2Rpt = plp.TotM2Rpt;
            _valorBrutoProd = plp.ValorBrutoProd;
            _valorUnitBruto = plp.ValorUnitBruto;
            _valorUnit = plp.ValorUnit;
            _valorProd = plp.ValorProd;
            CodProcesso = plp.CodProcesso;
            CodAplicacao = plp.CodAplicacao;
            QtdeRpt = plp.QtdeRpt;
            Qtde = plp.Qtde;
            QtdeProd = plp.QtdeProd;
            QtdeTotal = (double)plp.QtdeTotal;
            _totalProdBrutoLiberado = plp.TotalProdBrutoLiberado;
            _totalProdLiberado = plp.TotalProdLiberado;
            Peso = plp.Peso;
            DescrBeneficiamentos = plp.DescrBeneficiamentos;
            NumEtiquetas = plp.NumEtiquetas;
            NumCavaletes = plp.NumCavaletes;
            QtdeAmbiente = plp.QtdeAmbiente;
            TotM2Vidro = plp.TotM2Vidro;
            TotM2NaoVidro = plp.TotM2NaoVidro;
            QtdeVidro = plp.QtdeVidro;
            QtdeNaoVidro = plp.QtdeNaoVidro;
            PesoVidro = plp.PesoVidro;
            PesoNaoVidro = plp.PesoNaoVidro;

            var tipoCalculo = GrupoProdDAO.Instance.TipoCalculo(null, (int)IdGrupoProd, (int)IdSubgrupoProd, false);

            IsVidroEstoqueQtde = tipoCalculo == (int)TipoCalculoGrupoProd.Qtd &&
                GrupoProdDAO.Instance.IsVidro((int)IdGrupoProd) && SubgrupoProdDAO.Instance.IsProdutoEstoque((int)IdSubgrupoProd);

            // Campos usados como base para o IResumoCorte
            IdProdLiberarPedido = plp.IdProdLiberarPedido;
            IdProdResumo = plp.IdProd;
            TotalProd = plp.TotalProd;
            ValorBenefProd = plp.ValorBenefProd;
            EspessuraProd = plp.Espessura;
            
            // Utiliza a AlturaReal/LarguraReal para mostrar o tamanho real do produto na via da expedição na impressão da liberação
            AlturaProd = plp.AlturaReal > 0 ? plp.AlturaReal : plp.AlturaProd;
            LarguraProd = plp.LarguraReal > 0 ? plp.LarguraReal : plp.LarguraProd;

            Total = plp.Total;

            //Se for pedido de garantia e estiver marcado para não mostrar valores
            if (Liberacao.RelatorioLiberacaoPedido.NaoMostrarValorPedidoGarantia &&
                plp.TipoVenda == (int)Pedido.TipoVendaPedido.Garantia)
            {
                _totalProdLiberado = 0;
                _valorUnit = 0;
                TotalProd = 0;
                ValorBenefProd = 0;
            }
        }

        #endregion

        #region Propriedades

        public bool IsPedidoReposicao { get; set; }

        public uint? IdAmbientePedido { get; set; }

        public uint IdGrupoProd { get; set; }
        
        public uint? IdSubgrupoProd { get; set; }

        public uint IdPedido { get; set; }

        public uint? IdProdPedParent { get; set; }

        public bool IsProdutoLaminadoComposicao { get; set; }

        public bool IsProdFilhoLamComposicao { get; set; }

        public bool IsVidro { get; set; }

        public string Ambiente { get; set; }

        public string DescrGrupoProd { get; set; }

        public string PedCli { get; set; }

        public string CodInternoProd { get; set; }

        public string DescrProduto { get; set; }

        public string AltLarg1 { get; set; }

        public string AltLarg2 { get; set; }

        public string AltLargReal1 { get; set; }

        public string AltLargReal2 { get; set; }

        public string TituloAltLarg1 { get; set; }

        public string TituloAltLarg2 { get; set; }

        public bool PedidoMaoDeObra { get; set; }

        public double TotM2 { get; set; }

        public float TotM2Calc { get; set; }

        public double TotM2Rpt { get; set; }

        private decimal _valorBrutoProd;

        public decimal ValorBrutoProd
        {
            get { return _valorBrutoProd; }
            set { _valorBrutoProd = value; }
        }

        private decimal _valorUnitBruto;

        public decimal ValorUnitBruto
        {
            get { return _valorUnitBruto; }
            set { _valorUnitBruto = value; }
        }

        private decimal _valorUnit;

        public decimal ValorUnit
        {
            get { return _valorUnit; }
            set { _valorUnit = value; }
        }

        private decimal _valorProd;

        public decimal ValorProd
        {
            get { return _valorProd; }
            set { _valorProd = value; }
        }

        public string CodProcesso { get; set; }

        public string CodAplicacao { get; set; }

        public string QtdeRpt { get; set; }

        public float Qtde { get; set; }

        public float QtdeProd { get; set; }

        public double QtdeTotal { get; set; }

        private decimal _totalProdBrutoLiberado;

        public decimal TotalProdBrutoLiberado
        {
            get { return _totalProdBrutoLiberado; }
            set { _totalProdBrutoLiberado = value; }
        }

        private decimal _totalProdLiberado;

        public decimal TotalProdLiberado
        {
            get { return _totalProdLiberado; }
            set { _totalProdLiberado = value; }
        }

        public double Peso { get; set; }

        public string DescrBeneficiamentos { get; set; }

        public string NumEtiquetas { get; set; }

        public string NumCavaletes { get; set; }

        public long QtdeAmbiente { get; set; }

        public double TotM2Vidro { get; set; }

        public double TotM2NaoVidro { get; set; }

        public double QtdeVidro { get; set; }

        public double QtdeNaoVidro { get; set; }

        public double PesoVidro { get; set; }

        public double PesoNaoVidro { get; set; }

        public bool IsVidroEstoqueQtde { get; set; }

        #endregion

        #region Propriedades usadas somente para IResumoCorte

        public uint IdProdLiberarPedido { get; set; }

        public uint IdProdResumo { get; set; }

        public decimal TotalProd { get; set; }

        public decimal ValorBenefProd { get; set; }

        public double EspessuraProd { get; set; }

        public float AlturaProd { get; set; }

        public int LarguraProd { get; set; }

        #endregion

        #region IResumoCorte Members

        public uint Id
        {
            get { return IdProdLiberarPedido; }
        }

        public uint IdProd
        {
            get { return IdProdResumo; }
        }

        float IResumoCorte.Qtde
        {
            get { return (float)QtdeTotal; }
        }

        public float TotM
        {
            get { return (float)TotM2; }
        }

        public decimal Total { get; set; }

        public float Espessura
        {
            get { return (float)EspessuraProd; }
        }

        public string CodInterno
        {
            get { return CodInternoProd; }
        }

        public float Altura
        {
            get { return AlturaProd; }
        }

        public int Largura
        {
            get { return LarguraProd; }
        }

        public float PesoResumoCorte
        {
            get { return (float)Peso; }
        }

        #endregion
    }
}