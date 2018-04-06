using System;
using System.Collections.Generic;
using Glass.Data.Model;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Glass.Configuracoes;
using Glass.Global;
using Glass.Data.Helper.Calculos;

namespace Glass.Data.RelModel
{
    public class DadosMemoriaCalculo
    {
        #region Construtores

        #region Orçamento

        public DadosMemoriaCalculo(ProdutosOrcamento po, Orcamento.TipoEntregaOrcamento? tipoEntregaOrcamento, uint? idCliente)
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
                ValorTabela = po.ValorTabela > 0 ? po.ValorTabela : ProdutoDAO.Instance.GetValorTabela((int)po.IdProduto.Value, 
                    (int?)tipoEntregaOrcamento, idCliente, false, false, po.PercDescontoQtde, null, null, (int?)po.IdOrcamento);

                KeyValuePair<decimal, decimal> valores = CalculaValor(idCliente.GetValueOrDefault(), po.IdProduto.Value,
                    ValorTabela, Altura, _largura, Qtde, po.Espessura, po.Redondo, Custo, TotM2, po.Beneficiamentos.CountAreaMinima);

                CustoTotal = valores.Value;
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
        public DadosMemoriaCalculo(MaterialItemProjeto mip, Orcamento.TipoEntregaOrcamento? tipoEntregaOrcamento, float percComissao, string ambiente,
            string descrAmbiente, uint? idCliente, bool reposicao, Orcamento orcamento)
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
            ValorTabela = ProdutoDAO.Instance.GetValorTabela((int)mip.IdProd, (int?)tipoEntregaOrcamento, idCliente, false, reposicao, 0F, (int?)idPedido, null, null);
            Custo = prod.CustoCompra;
            CustoTotal = mip.Custo;
            _valorTabelaCobrado = mip.Valor;
            TipoCalculo = Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo(prod.IdProd);

            ValorTotal = mip.Total;
            Valor = CalculaValorUnit(mip, orcamento, ValorTotal);

            KeyValuePair<decimal, decimal> valores = CalculaValor(idCliente.GetValueOrDefault(), mip.IdProd, ValorTabela, Altura, _largura, Qtde, mip.Espessura,
                mip.Redondo, Custo, TotM2, mip.Beneficiamentos.CountAreaMinima);

            CustoTotal = valores.Value;
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

        public DadosMemoriaCalculo(ProdutosPedido pp, bool maoDeObra, float qtdeAmbienteSoma, 
            Pedido.TipoEntregaPedido? tipoEntrega, uint idCliente, bool reposicao)
        {
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
            ValorTabela = pp.ValorTabelaPedido > 0 ? pp.ValorTabelaPedido : ProdutoDAO.Instance.GetValorTabela((int)pp.IdProd, 
                (int?)tipoEntrega, idCliente, false, reposicao, pp.PercDescontoQtde, (int?)pp.IdPedido, null, null);
            Custo = pp.CustoUnit;
            CustoTotal = pp.CustoProd;
            Valor = pp.ValorVendido;
            ValorTotal = pp.Total;// +pp.ValorBenef;
            _valorTabelaCobrado = pp.ValorTabelaPedido;
            TipoCalculo = pp.TipoCalculoUsadoPedido;

            KeyValuePair<decimal, decimal> valores = CalculaValor(idCliente, pp.IdProd, ValorTabela, Altura, _largura, Qtde, pp.Espessura, pp.Redondo,
                Custo, TotM2, pp.Beneficiamentos.CountAreaMinima);

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
            bool maoDeObra = PedidoDAO.Instance.IsMaoDeObra(pai.IdPedido);

            Codigo = pai.CodInterno;
            Ambiente = !maoDeObra ? pai.Ambiente : AmbientePedidoDAO.Instance.ObtemPecaVidroQtd(pai.IdAmbientePedido.Value);
            DescrAmbiente = pai.DescrAmbiente;
            _descricao = " " + bc.DescricaoCompleta.Trim();
            Redondo = false;
            Qtde = ppb.Qtd;
            ValorTabela = BenefConfigDAO.Instance.GetValorTabela(bcp, (int?)tipoEntrega, PedidoDAO.Instance.ObtemIdCliente(pai.IdPedido));
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

        public DadosMemoriaCalculo(ProdutosPedidoEspelho ppe, bool maoDeObra, float qtdeAmbienteSoma, 
            Pedido.TipoEntregaPedido? tipoEntrega, uint idCliente, bool reposicao)
        {
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
            ValorTabela = ProdutoDAO.Instance.GetValorTabela((int)ppe.IdProd, (int?)tipoEntrega, idCliente, false, reposicao, ppe.PercDescontoQtde, (int?)ppe.IdPedido, null, null);
            Custo = ppe.CustoCompraProduto;
            Valor = ppe.ValorVendido;
            ValorTotal = ppe.Total;// +pp.ValorBenef;
            _valorTabelaCobrado = pp.ValorTabelaPedido;
            TipoCalculo = pp.TipoCalculoUsadoPedido;
            IsProdLamComposicao = pp.IsProdLamComposicao;

            KeyValuePair<decimal, decimal> valores = CalculaValor(idCliente, ppe.IdProd, ValorTabela, Altura, _largura, Qtde, ppe.Espessura, ppe.Redondo,
                Custo, TotM2, ppe.Beneficiamentos.CountAreaMinima);

            CustoTotal = valores.Value;

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
            bool maoDeObra = PedidoDAO.Instance.IsMaoDeObra(pai.IdPedido);

            Codigo = pai.CodInterno;
            Ambiente = !maoDeObra ? pai.AmbientePedido : AmbientePedidoEspelhoDAO.Instance.ObtemPecaVidroQtd(pai.IdAmbientePedido.Value);
            DescrAmbiente = pai.DescrAmbientePedido;
            _descricao = " " + bc.DescricaoCompleta.Trim();
            Redondo = false;
            Qtde = ppeb.Qtd;
            ValorTabela = BenefConfigDAO.Instance.GetValorTabela(bcp, (int?)tipoEntrega, PedidoDAO.Instance.ObtemIdCliente(pai.IdPedido));
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

        private static decimal CalculaValor(uint idBenefConfig, decimal valorBenef, float totM2, float altura, int lapBisAlt, int largura,
            int lapBisLarg, float qtde, int qtdBenef, decimal valorUnit, double percAcrescimo, double percDesconto)
        {
            BenefConfig bc = BenefConfigDAO.Instance.GetElement(idBenefConfig);

            float baseCalc = 1;
            switch (bc.TipoCalculo)
            {
                case TipoCalculoBenef.MetroLinear:
                    baseCalc = qtde * ((altura * lapBisAlt) + (largura * lapBisLarg)) / 1000;
                    break;

                case TipoCalculoBenef.MetroQuadrado:
                    baseCalc = totM2;
                    break;

                case TipoCalculoBenef.Porcentagem:
                    baseCalc = (float)valorUnit / 100 * totM2;
                    baseCalc = (float)(baseCalc * (1 + percAcrescimo - percDesconto));
                    break;

                case TipoCalculoBenef.Quantidade:
                    baseCalc = qtde * qtdBenef;
                    break;
            }

            return Math.Round((decimal)baseCalc * valorBenef, 2);
        }

        private static KeyValuePair<decimal, decimal> CalculaValor(uint idCliente, uint idProd, decimal valorProd, float altura, int largura, float qtde, float espessura, bool redondo, decimal custo, float totM2, int numeroBenef)
        {
            decimal valor = valorProd;
            decimal custoTemp = custo;
            float alturaTemp = altura;
            float totM2Temp = totM2;
            Glass.Data.DAL.ProdutoDAO.Instance.CalcTotaisItemProd(idCliente, (int)idProd, largura, qtde, 1, valorProd, espessura, redondo, 2, false, ref custoTemp, ref alturaTemp, ref totM2Temp, ref valor, false, numeroBenef);

            return new KeyValuePair<decimal, decimal>(Math.Round(valor, 2), Math.Round(custoTemp, 2));
        }

        private static decimal CalculaValorUnit(IProdutoCalculo produto, IContainerCalculo container, decimal total)
        {
            decimal? valor = ValorUnitario.Instance.CalcularValor(null, produto, container, total);
            return Math.Round(valor ?? total, 2);
        }

        #endregion
    }
}