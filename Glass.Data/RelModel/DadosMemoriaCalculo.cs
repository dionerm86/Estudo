using System;
using System.Collections.Generic;
using Glass.Data.Model;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Glass.Configuracoes;
using Glass.Data.Helper.Calculos;

namespace Glass.Data.RelModel
{
    public class DadosMemoriaCalculo
    {
        #region Construtores

        #region Orçamento

        public DadosMemoriaCalculo(ProdutosOrcamento po, Orcamento orcamento)
        {
            Ambiente = po.Ambiente;
            DescrAmbiente = po.IdProdParent > 0 ? ProdutosOrcamentoDAO.Instance.ObtemValorCampo<string>("descricao", "idProd=" + po.IdProdParent.Value) : null;
            Codigo = po.CodInterno;
            _descricao = po.DescrProduto.Trim();
            Redondo = po.Redondo;
            Qtde = po.Qtde != null ? po.Qtde.Value : 1;
            Altura = po.Altura;
            AlturaCalc = po.AlturaCalc;
            _largura = po.Largura;
            TotM2 = po.TotM;
            TotM2Calc = po.TotMCalc;
            Custo = po.CustoUnit;
            CustoTotal = po.Custo;
            Valor = po.ValorProd != null ? po.ValorProd.Value : 0;
            ValorTotal = (po.Total != null ? po.Total.Value : 0);// +po.ValorBenef;
            _valorTabelaCobrado = po.ValorTabela;
            TipoCalculo = po.TipoCalculoUsado;

            if (po.IdProduto != null)
            {
                ValorTabela = po.ValorTabela > 0
                    ? po.ValorTabela
                    : ProdutoDAO.Instance.GetValorTabela((int)po.IdProduto.Value, orcamento.TipoEntrega, orcamento.IdCliente, false, false, po.PercDescontoQtde, null, null, (int?)po.IdOrcamento);

                CalculaValor(orcamento, po);
                CustoTotal = po.Custo;
            }
        }

        public DadosMemoriaCalculo(ProdutoOrcamentoBenef pob)
        {
            BenefConfig bc = BenefConfigDAO.Instance.GetElement(pob.IdBenefConfig);
            ProdutosOrcamento pai = ProdutosOrcamentoDAO.Instance.GetElement(pob.IdProd);
            BenefConfigPreco bcp = BenefConfigPrecoDAO.Instance.GetByIdBenefConfig(null, pob.IdBenefConfig, pai.IdProduto.Value);
            Orcamento orca = OrcamentoDAO.Instance.GetElement(pai.IdOrcamento);

            Codigo = pai.CodInterno;
            Ambiente = pai.Ambiente;
            DescrAmbiente = pai.IdProdParent > 0 ? ProdutosOrcamentoDAO.Instance.ObtemValorCampo<string>("descricao", "idProd=" + pai.IdProdParent.Value) : null;
            _descricao = " " + bc.DescricaoCompleta.Trim();
            Redondo = false;
            Qtde = pob.Qtde;
            ValorTabela = BenefConfigDAO.Instance.GetValorTabela(bcp, orca.TipoEntrega, orca.IdCliente);
            Custo = bcp.Custo;
            CustoTotal = pob.Custo;
            Valor = pob.ValorUnit;
            ValorTotal = pob.Valor;

            if (bc.TipoControle == TipoControleBenef.Bisote || bc.TipoControle == TipoControleBenef.Lapidacao)
                _descricao += " " +
                    Utils.MontaDescrLapBis(pob.BisAlt, pob.BisLarg, pob.LapAlt, pob.LapLarg, pob.EspBisote, null, null, false);

            if (bc.TipoCalculo == TipoCalculoBenef.Porcentagem)
            {
                ValorTabela = Math.Round((ValorTabela / 100) * pai.ValorProd.Value, 2);
                Custo = Math.Round((Custo / 100) * pai.Custo, 2);
            }

            TipoCalculo = 0;
        }

        #endregion

        #region Projeto

        /// <summary>
        /// Monta dados do projeto para serem exibidos na impressão da memória de cálculo
        /// </summary>
        /// <param name="mip"></param>
        /// <param name="tipoEntregaOrcamento"></param>
        /// <param name="percComissao"></param>
        /// <param name="ambiente"></param>
        /// <param name="tipoAcrescimo"></param>
        /// <param name="acrescimo"></param>
        /// <param name="tipoDesconto"></param>
        /// <param name="desconto"></param>
        /// <param name="valorTotal"></param>
        public DadosMemoriaCalculo(MaterialItemProjeto mip, float percComissao, string ambiente,
            string descrAmbiente, bool reposicao, Orcamento orcamento)
        {
            Produto prod = ProdutoDAO.Instance.GetElementByPrimaryKey(mip.IdProd);

            var idPedido = ProdutosPedidoDAO.Instance.ObtemIdPedido(mip.IdProdPed);

            Ambiente = ambiente;
            DescrAmbiente = descrAmbiente;
            Codigo = mip.CodInterno;
            _descricao = mip.DescrProduto.Trim();
            Redondo = mip.Redondo;
            Qtde = mip.Qtde;
            Altura = mip.Altura;
            AlturaCalc = mip.AlturaCalc;
            _largura = mip.Largura;
            TotM2 = mip.TotM;
            TotM2Calc = mip.TotM2Calc;
            ValorTabela = ProdutoDAO.Instance.GetValorTabela((int)mip.IdProd, orcamento.TipoEntrega, orcamento.IdCliente, false, reposicao, 0F, (int?)idPedido, null, null);
            Custo = prod.CustoCompra;
            _valorTabelaCobrado = mip.Valor;
            TipoCalculo = GrupoProdDAO.Instance.TipoCalculo(prod.IdProd);

            ValorTotal = mip.Total;
            Valor = CalculaValorUnit(mip, orcamento, ValorTotal);

            CalculaValor(orcamento, mip);
            CustoTotal = mip.Custo;
        }

        public DadosMemoriaCalculo(MaterialProjetoBenef mpb, Orcamento.TipoEntregaOrcamento? tipoEntregaOrcamento, 
            float percComissao, string ambiente, string descrAmbiente, uint? idCliente)
        {
            BenefConfig bc = BenefConfigDAO.Instance.GetElement(mpb.IdBenefConfig);
            MaterialItemProjeto pai = MaterialItemProjetoDAO.Instance.GetElementByPrimaryKey(mpb.IdMaterItemProj);
            BenefConfigPreco bcp = BenefConfigPrecoDAO.Instance.GetByIdBenefConfig(null, mpb.IdBenefConfig, pai.IdProd);

            Codigo = pai.CodInterno;
            Ambiente = ambiente;
            DescrAmbiente = descrAmbiente;
            _descricao = " " + bc.DescricaoCompleta.Trim();
            Redondo = false;
            Qtde = mpb.Qtd;
            ValorTabela = BenefConfigDAO.Instance.GetValorTabela(bcp, (int?)tipoEntregaOrcamento, idCliente);
            Custo = bcp.Custo;
            CustoTotal = mpb.Custo;
            Valor = mpb.ValorUnit;
            ValorTotal = mpb.Valor;

            if (bc.TipoControle == TipoControleBenef.Bisote || bc.TipoControle == TipoControleBenef.Lapidacao)
                _descricao += " " +
                    Utils.MontaDescrLapBis(mpb.BisAlt, mpb.BisLarg, mpb.LapAlt, mpb.LapLarg, mpb.EspBisote, null, null, false);

            if (bc.TipoCalculo == TipoCalculoBenef.Porcentagem)
                ValorTabela = Math.Round((ValorTabela / 100) * pai.Valor, 2);

            TipoCalculo = 0;
        }

        #endregion

        #region Pedido

        public DadosMemoriaCalculo(ProdutosPedido pp, Pedido pedido, float qtdeAmbienteSoma)
        {
            bool maoDeObra = (pedido as IContainerCalculo).MaoDeObra;
            bool reposicao = (pedido as IContainerCalculo).Reposicao;

            Ambiente = !maoDeObra ? pp.Ambiente : AmbientePedidoDAO.Instance.ObtemPecaVidroQtd(pp.IdAmbientePedido.Value);
            DescrAmbiente = pp.DescrAmbiente;
            Codigo = pp.CodInterno;
            _descricao = !string.IsNullOrEmpty(pp.DescrProduto) ? pp.DescrProduto.Trim() : null;
            Redondo = pp.Redondo;
            Qtde = pp.Qtde;
            QtdeAmbiente = maoDeObra ? pp.QtdeAmbiente : 0;
            QtdeAmbienteSoma = maoDeObra ? qtdeAmbienteSoma : 0;
            Altura = pp.AlturaReal;
            AlturaCalc = pp.Altura;
            _largura = pp.Largura;
            TotM2 = pp.TotM;
            TotM2Calc = pp.TotM2Calc;
            ValorTabela = pp.ValorTabelaPedido > 0
                ? pp.ValorTabelaPedido
                : ProdutoDAO.Instance.GetValorTabela((int)pp.IdProd, pedido.TipoEntrega, pedido.IdCli, false, reposicao, pp.PercDescontoQtde, (int?)pp.IdPedido, null, null);
            Custo = pp.CustoUnit;
            Valor = pp.ValorVendido;
            ValorTotal = pp.Total;// +pp.ValorBenef;
            _valorTabelaCobrado = pp.ValorTabelaPedido;
            TipoCalculo = pp.TipoCalculoUsadoPedido;

            CalculaValor(pedido, pp);
            CustoTotal = pp.CustoProd;

            // Exibe o percentual de desconto por qtd concatenado com a descrição
            if (Geral.ConcatenarDescontoPorQuantidadeNaDescricaoDoProduto && pp.PercDescontoQtde > 0)
                _descricao += "\r\n(Desc. Prod.: " + pp.PercDescontoQtde + "%)";
        }

        public DadosMemoriaCalculo(ProdutoPedidoBenef ppb)
        {
            BenefConfig bc = BenefConfigDAO.Instance.GetElement(ppb.IdBenefConfig);
            ProdutosPedido pai = ProdutosPedidoDAO.Instance.GetElement(ppb.IdProdPed);
            BenefConfigPreco bcp = BenefConfigPrecoDAO.Instance.GetByIdBenefConfig(null, ppb.IdBenefConfig, pai.IdProd);
            int tipoEntrega = PedidoDAO.Instance.ObtemTipoEntrega(pai.IdPedido);
            bool maoDeObra = PedidoDAO.Instance.IsMaoDeObra(null, pai.IdPedido);

            Codigo = pai.CodInterno;
            Ambiente = !maoDeObra ? pai.Ambiente : AmbientePedidoDAO.Instance.ObtemPecaVidroQtd(pai.IdAmbientePedido.Value);
            DescrAmbiente = pai.DescrAmbiente;
            _descricao = " " + bc.DescricaoCompleta.Trim();
            Redondo = false;
            Qtde = ppb.Qtd;
            ValorTabela = BenefConfigDAO.Instance.GetValorTabela(bcp, (int?)tipoEntrega, PedidoDAO.Instance.ObtemIdCliente(null, pai.IdPedido));
            Custo = bcp.Custo;
            CustoTotal = ppb.Custo;
            Valor = ppb.ValorUnit;
            ValorTotal = ppb.Valor;

            if (bc.TipoControle == TipoControleBenef.Bisote || bc.TipoControle == TipoControleBenef.Lapidacao)
                _descricao += " " + Utils.MontaDescrLapBis(ppb.BisAlt, ppb.BisLarg, ppb.LapAlt, ppb.LapLarg, ppb.EspBisote, null, null, false);

            if (bc.TipoCalculo == TipoCalculoBenef.Porcentagem)
                ValorTabela = Math.Round((ValorTabela / 100) * pai.ValorVendido, 2);

            TipoCalculo = 0;
        }

        #endregion

        #region Pedido Espelho

        public DadosMemoriaCalculo(ProdutosPedidoEspelho ppe, Pedido pedido, float qtdeAmbienteSoma)
        {
            bool maoDeObra = (pedido as IContainerCalculo).MaoDeObra;
            bool reposicao = (pedido as IContainerCalculo).Reposicao;

            ProdutosPedido pp = ProdutosPedidoDAO.Instance.GetByProdPedEsp(null, ppe.IdProdPed, true);

            Ambiente = !maoDeObra ? ppe.AmbientePedido : AmbientePedidoEspelhoDAO.Instance.ObtemPecaVidroQtd(ppe.IdAmbientePedido.Value);
            DescrAmbiente = ppe.DescrAmbientePedido;
            Codigo = ppe.CodInterno;
            _descricao = ppe.DescrProduto.Trim();
            Redondo = ppe.Redondo;
            Qtde = ppe.Qtde;
            QtdeAmbiente = maoDeObra ? ppe.QtdeAmbiente : 0;
            QtdeAmbienteSoma = maoDeObra ? qtdeAmbienteSoma : 0;
            Altura = ppe.AlturaReal;
            AlturaCalc = ppe.Altura;
            _largura = ppe.Largura;
            TotM2 = ppe.TotM;
            TotM2Calc = ppe.TotM2Calc;
            ValorTabela = ProdutoDAO.Instance.GetValorTabela((int)ppe.IdProd, pedido.TipoEntrega, pedido.IdCli, false, reposicao, ppe.PercDescontoQtde, (int?)ppe.IdPedido, null, null);
            Custo = ppe.CustoCompraProduto;
            Valor = ppe.ValorVendido;
            ValorTotal = ppe.Total;// +pp.ValorBenef;
            _valorTabelaCobrado = pp.ValorTabelaPedido;
            TipoCalculo = pp.TipoCalculoUsadoPedido;
            IsProdLamComposicao = pp.IsProdLamComposicao;

            CalculaValor(pedido, ppe);
            CustoTotal = (ppe as IProdutoCalculo).CustoProd;

            // Exibe o percentual de desconto por qtd concatenado com a descrição
            if (Geral.ConcatenarDescontoPorQuantidadeNaDescricaoDoProduto && ppe.PercDescontoQtde > 0)
                _descricao += "\r\n(Desc. Prod.: " + ppe.PercDescontoQtde + "%)";
        }

        public DadosMemoriaCalculo(ProdutoPedidoEspelhoBenef ppeb)
        {
            BenefConfig bc = BenefConfigDAO.Instance.GetElement(ppeb.IdBenefConfig);
            ProdutosPedidoEspelho pai = ProdutosPedidoEspelhoDAO.Instance.GetElement(ppeb.IdProdPed, false);
            BenefConfigPreco bcp = BenefConfigPrecoDAO.Instance.GetByIdBenefConfig(null, ppeb.IdBenefConfig, pai.IdProd);
            int tipoEntrega = PedidoDAO.Instance.ObtemTipoEntrega(pai.IdPedido);
            bool maoDeObra = PedidoDAO.Instance.IsMaoDeObra(null, pai.IdPedido);

            Codigo = pai.CodInterno;
            Ambiente = !maoDeObra ? pai.AmbientePedido : AmbientePedidoEspelhoDAO.Instance.ObtemPecaVidroQtd(pai.IdAmbientePedido.Value);
            DescrAmbiente = pai.DescrAmbientePedido;
            _descricao = " " + bc.DescricaoCompleta.Trim();
            Redondo = false;
            Qtde = ppeb.Qtd;
            ValorTabela = BenefConfigDAO.Instance.GetValorTabela(bcp, (int?)tipoEntrega, PedidoDAO.Instance.ObtemIdCliente(null, pai.IdPedido));
            Custo = bcp.Custo;
            CustoTotal = ppeb.Custo;
            Valor = ppeb.ValorUnit;
            ValorTotal = ppeb.Valor;

            if (bc.TipoControle == TipoControleBenef.Bisote || bc.TipoControle == TipoControleBenef.Lapidacao)
                _descricao += " " + Utils.MontaDescrLapBis(ppeb.BisAlt, ppeb.BisLarg, ppeb.LapAlt, ppeb.LapLarg, ppeb.EspBisote, null, null, false);

            if (bc.TipoCalculo == TipoCalculoBenef.Porcentagem)
                ValorTabela = Math.Round((ValorTabela / 100) * pai.ValorVendido, 2);

            TipoCalculo = 0;
        }

        #endregion
    
        #endregion
    
        #region Propriedades

        public string Ambiente { get; set; }

        public string DescrAmbiente { get; set; }

        public string Codigo { get; set; }

        private string _descricao;

        public string Descricao
        {
            get 
            {
                return _descricao + (Redondo && !BenefConfigDAO.Instance.CobrarRedondo() && !_descricao.ToLower().Contains("redondo") ? " REDONDO" : "") + 
                    (TipoCalculo > 0 ? " (" + Glass.Global.CalculosFluxo.GetDescrTipoCalculo(TipoCalculo, true) + ")" : ""); 
            }
            set { _descricao = value; }
        }

        public float Qtde { get; set; }

        public float QtdeAmbiente { get; set; }

        public float QtdeAmbienteSoma { get; set; }

        public float Altura { get; set; }

        public float AlturaCalc { get; set; }

        private int _largura;

        public int Largura
        {
            get { return !Redondo ? _largura : 0; }
            set { _largura = value; }
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
            get { return PedidoConfig.EmpresaTrabalhaAlturaLargura ? Altura.ToString() : Largura.ToString(); }
        }

        public string AltLarg2
        {
            get { return PedidoConfig.EmpresaTrabalhaAlturaLargura ? Largura.ToString() : Altura.ToString(); }
        }

        public float TotM2 { get; set; }

        public float TotM2Calc { get; set; }

        public decimal ValorTabela { get; set; }

        public decimal Custo { get; set; }

        public decimal CustoTotal { get; set; }

        public decimal Valor { get; set; }

        public decimal ValorTotal { get; set; }

        public bool Redondo { get; set; }

        private decimal _valorTabelaCobrado;

        public decimal ValorTabelaCobrado
        {
            get { return _valorTabelaCobrado > 0 ? _valorTabelaCobrado : ValorTabela; }
            set { _valorTabelaCobrado = value; }
        }

        public int TipoCalculo { get; set; }

        public bool ValorTabelaMudou
        {
            get { return _valorTabelaCobrado > 0 && ValorTabela != _valorTabelaCobrado; }
        }

        public string TotM2Completo
        {
            get { return TotM2 != TotM2Calc ? TotM2 + " (" + TotM2Calc + ")" : TotM2.ToString(); }
        }

        public bool IsProdLamComposicao { get; set; }

        #endregion

        #region Métodos Privados

        private static void CalculaValor(IContainerCalculo container, IProdutoCalculo produto)
        {
            Helper.Calculos.ValorTotal.Instance.Calcular(
                null,
                container,
                produto,
                Helper.Calculos.Estrategia.ValorTotal.Enum.ArredondarAluminio.ArredondarApenasCalculo,
                true,
                produto.Beneficiamentos.CountAreaMinima);
        }

        private static decimal CalculaValorUnit(IProdutoCalculo produto, IContainerCalculo container, decimal total)
        {
            decimal? valor = ValorUnitario.Instance.CalcularValor(null, container, produto, total);
            return Math.Round(valor ?? total, 2);
        }

        #endregion
    }
}