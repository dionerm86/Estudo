using System.Collections.Generic;
using GDA;
using Glass.Data.Helper;
using Glass.Data.DAL;
using System.Linq;
using Glass.Configuracoes;
using Glass.Global;
using Glass.Data.Helper.Calculos;
using Glass.Data.Model.Calculos;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ProdutosLiberarPedidoDAO))]
    [PersistenceClass("produtos_liberar_pedido")]
    public class ProdutosLiberarPedido : IResumoCorte
    {
        #region Propriedades

        [PersistenceProperty("IDPRODLIBERARPEDIDO", PersistenceParameterType.IdentityKey)]
        public uint IdProdLiberarPedido { get; set; }

        [PersistenceProperty("IDLIBERARPEDIDO")]
        public uint IdLiberarPedido { get; set; }

        [PersistenceProperty("IDPEDIDO")]
        public uint IdPedido { get; set; }

        [PersistenceProperty("IDPRODPED")]
        public uint IdProdPed { get; set; }

        [PersistenceProperty("IDPRODPEDPRODUCAO")]
        public uint? IdProdPedProducao { get; set; }

        [PersistenceProperty("QTDE")]
        public float Qtde { get; set; }

        [PersistenceProperty("QTDECALC")]
        public float QtdeCalc { get; set; }

        [PersistenceProperty("VALORICMS")]
        public decimal ValorIcms { get; set; }

        [PersistenceProperty("VALORIPI")]
        public decimal ValorIpi { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("QTDETOTAL", DirectionParameter.InputOptional)]
        public decimal QtdeTotal { get; set; }

        [PersistenceProperty("DESCRPRODUTO", DirectionParameter.InputOptional)]
        public string DescrProduto { get; set; }

        [PersistenceProperty("TOTM2", DirectionParameter.InputOptional)]
        public double TotM2 { get; set; }

        [PersistenceProperty("IDPROD", DirectionParameter.InputOptional)]
        public uint IdProd { get; set; }

        [PersistenceProperty("CODINTERNO", DirectionParameter.InputOptional)]
        public string CodInternoProd { get; set; }

        [PersistenceProperty("ALTURA", DirectionParameter.InputOptional)]
        public float AlturaProd { get; set; }

        [PersistenceProperty("LARGURA", DirectionParameter.InputOptional)]
        public int LarguraProd { get; set; }

        [PersistenceProperty("ALTURAREAL", DirectionParameter.InputOptional)]
        public float AlturaReal { get; set; }

        [PersistenceProperty("LARGURAREAL", DirectionParameter.InputOptional)]
        public int LarguraReal { get; set; }

        [PersistenceProperty("TOTAL", DirectionParameter.InputOptional)]
        public decimal TotalProd { get; set; }

        [PersistenceProperty("VALORDESCONTOQTDE", DirectionParameter.InputOptional)]
        public decimal ValorDescontoQtde { get; set; }

        [PersistenceProperty("VALORDESCONTOTOTAL", DirectionParameter.InputOptional)]
        public decimal ValorDescontoTotal { get; set; }

        [PersistenceProperty("VALORBENEF", DirectionParameter.InputOptional)]
        public decimal ValorBenefProd { get; set; }

        [PersistenceProperty("VALORICMSPROD", DirectionParameter.InputOptional)]
        public decimal ValorIcmsProd { get; set; }

        [PersistenceProperty("VALORIPIPROD", DirectionParameter.InputOptional)]
        public decimal ValorIpiProd { get; set; }

        [PersistenceProperty("QTDEPROD", DirectionParameter.InputOptional)]
        public float QtdeProd { get; set; }

        [PersistenceProperty("PERCDESCONTOQTDE", DirectionParameter.InputOptional)]
        public float PercDescontoQtde { get; set; }

        [PersistenceProperty("ESPESSURAPROD", DirectionParameter.InputOptional)]
        public double EspessuraProd { get; set; }

        [PersistenceProperty("REDONDO", DirectionParameter.InputOptional)]
        public bool Redondo { get; set; }

        [PersistenceProperty("IDGRUPOPROD", DirectionParameter.InputOptional)]
        public uint IdGrupoProd { get; set; }

        [PersistenceProperty("IDSUBGRUPOPROD", DirectionParameter.InputOptional)]
        public uint? IdSubgrupoProd { get; set; }

        [PersistenceProperty("PESOPROD", DirectionParameter.InputOptional)]
        public double Peso { get; set; }

        [PersistenceProperty("TOTM2CALC", DirectionParameter.InputOptional)]
        public double TotM2CalcSql { get; set; }

        [PersistenceProperty("PEDCLI", DirectionParameter.InputOptional)]
        public string PedCli { get; set; }

        [PersistenceProperty("QTDEAMBIENTE", DirectionParameter.InputOptional)]
        public long QtdeAmbiente { get; set; }

        [PersistenceProperty("PEDIDOMAODEOBRA", DirectionParameter.InputOptional)]
        public bool PedidoMaoDeObra { get; set; }

        [PersistenceProperty("IDAMBIENTEPEDIDO", DirectionParameter.InputOptional)]
        public uint? IdAmbientePedido { get; set; }

        [PersistenceProperty("IDAMBIENTEPEDIDOESPELHO", DirectionParameter.InputOptional)]
        public uint? IdAmbientePedidoEspelho { get; set; }

        private string _ambientePedido;

        [PersistenceProperty("AMBIENTE", DirectionParameter.InputOptional)]
        public string AmbientePedido
        {
            get 
            {
                return !PedidoMaoDeObra || IdAmbientePedido == null ? _ambientePedido :
                    AmbientePedidoDAO.Instance.ObtemPecaVidroQtd(IdAmbientePedido.Value);
            }
            set { _ambientePedido = value; }
        }

        private string _ambientePedidoEspelho;

        [PersistenceProperty("AMBIENTEPEDIDOESPELHO", DirectionParameter.InputOptional)]
        public string AmbientePedidoEspelho
        {
            get
            {
                return !PedidoMaoDeObra || IdAmbientePedidoEspelho == null ? _ambientePedidoEspelho :
                    AmbientePedidoEspelhoDAO.Instance.ObtemPecaVidroQtd(IdAmbientePedidoEspelho.Value);
            }
            set { _ambientePedidoEspelho = value; }
        }

        [PersistenceProperty("CODAPLICACAO", DirectionParameter.InputOptional)]
        public string CodAplicacao { get; set; }

        [PersistenceProperty("CODPROCESSO", DirectionParameter.InputOptional)]
        public string CodProcesso { get; set; }

        [PersistenceProperty("ALTURABENEF", DirectionParameter.InputOptional)]
        public int AlturaBenef { get; set; }

        [PersistenceProperty("LARGURABENEF", DirectionParameter.InputOptional)]
        public int LarguraBenef { get; set; }

        [PersistenceProperty("DESCRGRUPOPROD", DirectionParameter.InputOptional)]
        public string DescrGrupoProd { get; set; }

        [PersistenceProperty("DESCRSUBGRUPOPROD", DirectionParameter.InputOptional)]
        public string DescrSubgrupoProd { get; set; }

        [PersistenceProperty("TOTALBRUTO", DirectionParameter.InputOptional)]
        public decimal ValorBrutoProd { get; set; }

        [PersistenceProperty("VALORUNITBRUTO", DirectionParameter.InputOptional)]
        public decimal ValorUnitBruto { get; set; }

        [PersistenceProperty("TIPOPEDIDO", DirectionParameter.InputOptional)]
        public int TipoPedido { get; set; }

        [PersistenceProperty("TIPOVENDA", DirectionParameter.InputOptional)]
        public int? TipoVenda { get; set; }

        [PersistenceProperty("IDCLIENTE", DirectionParameter.InputOptional)]
        public uint IdCliente { get; set; }

        [PersistenceProperty("IDPRODPEDPARENT", DirectionParameter.InputOptional)]
        public uint? IdProdPedParent { get; set; }

        [PersistenceProperty("ISPRODUTOLAMINADOCOMPOSICAO", DirectionParameter.InputOptional)]
        public bool IsProdutoLaminadoComposicao { get; set; }

        [PersistenceProperty("ISPRODFILHOLAMCOMPOSICAO", DirectionParameter.InputOptional)]
        public bool IsProdFilhoLamComposicao { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string Ambiente
        {
            get 
            {
                return !PedidoMaoDeObra ? (IdAmbientePedidoEspelho != null ? _ambientePedidoEspelho : _ambientePedido) :
                    (IdAmbientePedidoEspelho != null ? AmbientePedidoEspelho : AmbientePedido);
            }
        }

        public string TituloAltLarg1
        {
            get { return PedidoConfig.EmpresaTrabalhaAlturaLargura ? "Altura" : "Largura"; }
        }

        public string TituloAltLarg2
        {
            get { return PedidoConfig.EmpresaTrabalhaAlturaLargura ? "Largura" : "Altura"; }
        }

        public string AltLarg1
        {
            get { return PedidoConfig.EmpresaTrabalhaAlturaLargura ? AlturaProd.ToString() : LarguraProd.ToString(); }
        }

        public string AltLarg2
        {
            get { return PedidoConfig.EmpresaTrabalhaAlturaLargura ? LarguraProd.ToString() : AlturaProd.ToString(); }
        }

        /// <summary>
        /// Campo usado para mostrar a altura real da peça
        /// </summary>
        public string AltLargReal1
        {
            get { return PedidoConfig.EmpresaTrabalhaAlturaLargura ? AlturaReal.ToString() : LarguraReal.ToString(); }
        }

        /// <summary>
        /// Campo usado para mostrar a largura real da peça
        /// </summary>
        public string AltLargReal2
        {
            get { return PedidoConfig.EmpresaTrabalhaAlturaLargura ? LarguraReal.ToString() : AlturaReal.ToString(); }
        }

        public decimal ValorProd
        {
            get
            {
                var idLojaPedido = PedidoDAO.Instance.ObtemIdLoja(IdPedido);

                // Soma o desconto por quantidade pois na impressão da liberação o desconto será deduzido do total do pedido,
                // se não somar o desconto por qtd, ficará como se o relatório estivesse aplicando o desconto 2 vezes
                return TotalProd + ValorBenefProd + ValorDescontoQtde + (LojaDAO.Instance.ObtemCalculaIcmsPedido(idLojaPedido) ? ValorIcmsProd : 0) +
                    (LojaDAO.Instance.ObtemCalculaIpiPedido(idLojaPedido) ? ValorIpiProd : 0);
            }
        }

        public decimal TotalProdLiberado
        {
            get 
            {
                // Caso seja pedido de mão-de-obra e não seja liberação por produtos prontos, o ValorProd deve ser dividido pela 
                // QtdeAmbiente * QtdeProd, devido ao fato do ValorProd ser calculado já considerando o valor unitário do produto * QtdeAmbiente e 
                // QtdeProd e o QtdeTotal já considerar QtdeAmbiente * QtdeProd, a menos que a liberação seja por produtos prontos,
                // onde deverá utilizar somente QtdeAmbiente (Cálculo baseado na propriedade QtdeDisponivelLiberacao de ProdutosPedido)
                var idLoja = PedidoDAO.Instance.ObtemIdLoja(IdPedido);
                var NaoIgnorar = !LojaDAO.Instance.GetIgnorarLiberarProdutosProntos(null, idLoja);

                decimal divisor = PedidoMaoDeObra && QtdeAmbiente > 0 ?
                    ((Liberacao.DadosLiberacao.LiberarProdutosProntos && NaoIgnorar) ? QtdeAmbiente : QtdeAmbiente * (decimal)QtdeProd) :
                    (decimal)QtdeProd;

                divisor = divisor > 0 ? divisor : 1;

                // Necessário criar esta opção pelo mesmo motivo que foi criado no TotalCalc em produtos_pedido
                if ((Liberacao.DadosLiberacao.LiberarProdutosProntos && NaoIgnorar) && PedidoMaoDeObra && QtdeAmbiente > 0 && Qtde > 1 && QtdeTotal > 1)
                    QtdeTotal = QtdeAmbiente;

                return ValorProd / divisor * (decimal)QtdeTotal;
            }
        }

        public decimal TotalProdBrutoLiberado
        {
            get 
            {
                // Caso seja pedido de mão-de-obra e não seja liberação por produtos prontos, o ValorProd deve ser dividido pela 
                // QtdeAmbiente * QtdeProd, devido ao fato do ValorProd ser calculado já considerando o valor unitário do produto * QtdeAmbiente e 
                // QtdeProd e o QtdeTotal já considerar QtdeAmbiente * QtdeProd, a menos que a liberação seja por produtos prontos,
                // onde deverá utilizar somente QtdeAmbiente (Cálculo baseado na propriedade QtdeDisponivelLiberacao de ProdutosPedido)
                var idLoja = PedidoDAO.Instance.ObtemIdLoja(IdPedido);
                var naoIgnorar = !LojaDAO.Instance.GetIgnorarLiberarProdutosProntos(null, idLoja);

                decimal divisor = PedidoMaoDeObra && QtdeAmbiente > 0 ?
                    ((Liberacao.DadosLiberacao.LiberarProdutosProntos && naoIgnorar) ? QtdeAmbiente : QtdeAmbiente * (decimal)QtdeProd) :
                    (decimal)QtdeProd;

                divisor = divisor > 0 ? divisor : 1;

                return ValorBrutoProd / divisor * (decimal)QtdeTotal;
            }
        }

        private int TipoCalc
        {
            get { return Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo((int)IdGrupoProd, (int?)IdSubgrupoProd); }
        }

        private ProdutosPedido produtoPedido = null;

        private ProdutosPedido ProdutoPedido
        {
            get
            {
                if (produtoPedido == null)
                {
                    produtoPedido = ProdutosPedidoDAO.Instance.GetElementByPrimaryKey(IdProdPed);
                }

                return produtoPedido;
            }
        }

        private List<ProdutoPedidoBenef> _beneficiamentos = null;

        public GenericBenefCollection Beneficiamentos
        {
            get 
            {
                if (_beneficiamentos == null)
                    _beneficiamentos = ProdutoPedido.Beneficiamentos;

                return _beneficiamentos; 
            }
            set { _beneficiamentos = value; }
        }

        private decimal? _valorUnit;

        public decimal ValorUnit
        {
            get 
            {
                if (_valorUnit == null)
                {
                    var totM2 = ProdutosPedidoDAO.Instance.ObtemTotM(null, IdProdPed);

                    // Recupera o valor unitário do beneficiamento e soma ao valor unitário final, o motivo disso é evitar que o cálculo do
                    // valor unitário fique incorreto caso esteja usando área mínima, situação na qual o beneficiamento calculado por m²
                    // considera a área real e o cálculo do vidro considera a área mínima
                    var valorUnitBenef = Beneficiamentos.Sum(b => {
                        var divisor = totM2 > 0 ? (decimal)totM2 : 1;
                        return b.Valor / divisor;
                    });
                    
                    var pedido = PedidoDAO.Instance.GetElementByPrimaryKey(IdPedido);

                    decimal? valorUnitario = ValorUnitario.Instance.CalcularValor(null, pedido, ProdutoPedido, ValorProd - ValorBenefProd);
                    if (valorUnitario.HasValue)
                    {
                        _valorUnit = valorUnitario + valorUnitBenef;
                    }
                }

                return _valorUnit ?? 0;
            }
        }

        public string QtdeRpt
        {
            get { return QtdeProd + (PedidoMaoDeObra ? " x " + QtdeAmbiente + " p.v." : ""); }
        }

        public string DescrBeneficiamentos { get; set; }

        public string NumEtiquetas { get; set; }

        public string NumCavaletes { get; set; }

        private bool? _isVidro = null;

        public bool IsVidro
        {
            get
            {
                if (_isVidro == null)
                {
                    if (IdGrupoProd != (uint)Glass.Data.Model.NomeGrupoProd.Vidro)
                        _isVidro = false;
                    else if (Liberacao.RelatorioLiberacaoPedido.ConsiderarVidroQualquerProdutoDoGrupoVidro && IdGrupoProd == (uint)Glass.Data.Model.NomeGrupoProd.Vidro)
                        _isVidro = true;
                    else
                        _isVidro = !SubgrupoProdDAO.Instance.IsSubgrupoProducao((int)IdGrupoProd, (int?)IdSubgrupoProd);
                }

                return _isVidro.Value;
            }
        }

        public double TotM2Rpt
        {
            get { return TipoPedido == (int)Pedido.TipoPedidoEnum.MaoDeObra || 
                PedidoConfig.RelatorioPedido.ExibirM2CalcRelatorio ? TotM2Calc : TotM2; }
        }

        public double TotM2Vidro
        {
            get { return IsVidro ? TotM2Rpt : 0; }
        }

        public double TotM2NaoVidro
        {
            get { return IsVidro ? 0 : TotM2Rpt; }
        }

        public double QtdeVidro
        {
            get { return IsVidro ? (double)QtdeTotal : 0; }
        }

        public double QtdeNaoVidro
        {
            get { return IsVidro ? 0 : (double)QtdeTotal; }
        }

        public double PesoVidro
        {
            get { return IsVidro ? Peso : 0; }
        }

        public double PesoNaoVidro
        {
            get { return IsVidro ? 0 : Peso; }
        }

        #endregion

        #region IResumoCorte Members

        public uint Id
        {
            get { return IdProdLiberarPedido; }
        }

        float IResumoCorte.Qtde
        {
            get { return (float)QtdeTotal; }
        }

        public float TotM
        {
            get { return (float)TotM2; }
        }

        public float TotM2Calc
        {
            get { return (float)TotM2CalcSql; }
        }

        public decimal Total
        {
            get 
            {   
                // Subtrai o desconto do pedido no cálculo do total do produto, para que o resumo da liberação fique correto
                decimal total = ((TotalProd + ValorBenefProd) / (decimal)(QtdeProd > 0 ? QtdeProd : 1)) * (decimal)QtdeTotal;
                decimal percDescPed = (decimal)PedidoDAO.Instance.GetPercDesc(IdPedido);

                return total - (total * percDescPed);
            }
        }

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