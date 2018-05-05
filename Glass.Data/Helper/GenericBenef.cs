using System;
using Glass.Data.Model;
using Glass.Data.DAL;
using Glass.Comum.Cache;

namespace Glass.Data.Helper
{
    [Serializable]
    public class GenericBenef
    {
        #region Campos Privados

        private static readonly CacheMemoria<BenefConfig, uint> cacheBeneficiamentos;
        private TipoProdutoBeneficiamento _tipo = TipoProdutoBeneficiamento.Nenhum;

        #endregion

        #region Construtor

        static GenericBenef()
        {
            cacheBeneficiamentos = new CacheMemoria<BenefConfig, uint>("beneficiamentos");
        }

        #endregion

        #region Métodos Estáticos

        /// <summary>
        /// Retorna o nome da tabela de beneficiamento do tipo passado.
        /// </summary>
        /// <param name="tipo"></param>
        /// <returns></returns>
        public static string GetTabela(TipoProdutoBeneficiamento tipo)
        {
            switch (tipo)
            {
                case TipoProdutoBeneficiamento.MaterialProjeto: return "material_projeto_benef";
                case TipoProdutoBeneficiamento.ProdutoCompra: return "produtos_compra_benef";
                case TipoProdutoBeneficiamento.ProdutoOrcamento: return "produto_orcamento_benef";
                case TipoProdutoBeneficiamento.ProdutoPedido: return "produto_pedido_benef";
                case TipoProdutoBeneficiamento.ProdutoPedidoEspelho: return "produto_pedido_espelho_benef";
                case TipoProdutoBeneficiamento.Produto: return "produto_benef";
                case TipoProdutoBeneficiamento.ProdutoTrocaDevolucao: return "produto_troca_dev_benef";
                case TipoProdutoBeneficiamento.PecaModeloProjeto: return "peca_modelo_benef";
                case TipoProdutoBeneficiamento.PecaItemProjeto: return "peca_item_proj_benef";
                case TipoProdutoBeneficiamento.ProdutoTrocado: return "produto_trocado_benef";
                default: throw new ArgumentException("Tipo desconhecido.");
            }
        }

        #endregion

        #region Propriedades

        #region Tipo

        public TipoProdutoBeneficiamento TipoProdutoBenef
        {
            get { return _tipo; }
            set { _tipo = value; }
        }

        #endregion

        #region Identificadores das classes convertidas

        public uint IdProdutoPedidoBenef { get; set; }

        public uint IdProdutoPedido { get; set; }

        public uint IdProdutoOrcamentoBenef { get; set; }

        public uint IdProdutoOrcamento { get; set; }

        public uint IdMaterialItemProjeto { get; set; }

        public uint IdMaterialItemProjetoBenef { get; set; }

        public uint IdProdutoPedidoEspelhoBenef { get; set; }

        public uint IdProdutoPedidoEspelho { get; set; }

        public uint IdProdutoCompraBenef { get; set; }

        public uint IdProdutoCompra { get; set; }

        public uint IdProdutoBenef { get; set; }

        public uint IdProduto { get; set; }

        public uint IdProdutoTrocaDevolucaoBenef { get; set; }

        public uint IdProdutoTrocaDevolucao { get; set; }

        public uint IdPecaProjetoModeloBenef { get; set; }

        public uint IdPecaProjetoModelo { get; set; }

        public uint IdPecaItemProjetoBenef { get; set; }

        public uint IdPecaItemProjeto { get; set; }

        public uint IdProdutoTrocadoBenef { get; set; }

        public uint IdProdutoTrocado { get; set; }

        public uint IdProdBaixaEstBenef { get; set; }

        public uint IdProdBaixaEst { get; set; }

        #endregion

        public uint IdBenefConfig { get; set; }

        public int Qtd { get; set; }

        private decimal _valorUnit;

        public decimal ValorUnit
        {
            get { return GetValorUnit(_valorUnit, Valor, Qtd); }
            set { _valorUnit = value; }
        }

        public decimal Valor { get; set; }

        public decimal Custo { get; set; }

        public int LapLarg { get; set; }

        public int LapAlt { get; set; }

        public int BisLarg { get; set; }

        public int BisAlt { get; set; }

        public float EspBisote { get; set; }

        public int EspFuro { get; set; }

        public bool Padrao { get; set; }

        public decimal ValorComissao { get; set; }

        public decimal ValorAcrescimo { get; set; }

        public decimal ValorAcrescimoProd { get; set; }

        public decimal ValorDesconto { get; set; }

        public decimal ValorDescontoProd { get; set; }

        #region Propriedades Estendidas

        private BenefConfig Beneficiamento
        {
            get
            {
                var beneficiamento = cacheBeneficiamentos.RecuperarDoCache(IdBenefConfig);

                if (beneficiamento == null)
                {
                    beneficiamento = BenefConfigDAO.Instance.GetElement(IdBenefConfig);
                    cacheBeneficiamentos.AtualizarItemNoCache(beneficiamento, IdBenefConfig);
                }

                return beneficiamento;
            }
        }

        internal string DescricaoParent
        {
            get { return Beneficiamento.DescricaoParent; }
        }

        internal string Descricao
        {
            get { return Beneficiamento.Descricao; }
        }

        public string DescricaoBeneficiamento
        {
            get { return Beneficiamento.DescricaoCompleta; }
        }

        public TipoCalculoBenef TipoCalculo
        {
            get { return Beneficiamento.TipoCalculo; }
        }

        public decimal TotalBruto
        {
            get { return Valor - ValorAcrescimo - ValorAcrescimoProd - ValorComissao + ValorDesconto + ValorDescontoProd; }
        }

        #endregion

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public GenericBenef()
        {
        }

        /// <summary>
        /// Cria uma classe genérica de beneficiamento a partir de uma classe ProdutoPedidoBenef.
        /// </summary>
        /// <param name="ppb"></param>
        public GenericBenef(ProdutoPedidoBenef ppb)
        {
            _tipo = TipoProdutoBeneficiamento.ProdutoPedido;
            IdProdutoPedidoBenef = ppb.IdProdPedBenef;
            IdProdutoPedido = ppb.IdProdPed;
            IdBenefConfig = ppb.IdBenefConfig;
            BisAlt = ppb.BisAlt;
            BisLarg = ppb.BisLarg;
            EspBisote = ppb.EspBisote;
            EspFuro = ppb.EspFuro;
            LapAlt = ppb.LapAlt;
            LapLarg = ppb.LapLarg;
            Qtd = ppb.Qtd;
            _valorUnit = ppb.ValorUnit;
            Valor = ppb.Valor;
            Custo = ppb.Custo;
            Padrao = ppb.Padrao;
            ValorComissao = ppb.ValorComissao;
            ValorAcrescimo = ppb.ValorAcrescimo;
            ValorAcrescimoProd = ppb.ValorAcrescimoProd;
            ValorDesconto = ppb.ValorDesconto;
            ValorDescontoProd = ppb.ValorDescontoProd;
        }

        /// <summary>
        /// Cria uma classe genérica de beneficiamento a partir de uma classe ProdutoOrcamentoBenef.
        /// </summary>
        /// <param name="pob"></param>
        public GenericBenef(ProdutoOrcamentoBenef pob)
        {
            _tipo = TipoProdutoBeneficiamento.ProdutoOrcamento;
            IdProdutoOrcamentoBenef = pob.IdProdOrcaBenef;
            IdProdutoOrcamento = pob.IdProd;
            BisAlt = pob.BisAlt;
            BisLarg = pob.BisLarg;
            EspBisote = pob.EspBisote;
            EspFuro = pob.EspFuro;
            IdBenefConfig = pob.IdBenefConfig;
            LapAlt = pob.LapAlt;
            LapLarg = pob.LapLarg;
            Qtd = pob.Qtde;
            _valorUnit = pob.ValorUnit;
            Valor = pob.Valor;
            Custo = pob.Custo;
            Padrao = pob.Padrao;
            ValorComissao = pob.ValorComissao;
            ValorAcrescimo = pob.ValorAcrescimo;
            ValorAcrescimoProd = pob.ValorAcrescimoProd;
            ValorDesconto = pob.ValorDesconto;
            ValorDescontoProd = pob.ValorDescontoProd;
        }

        /// <summary>
        /// Cria uma classe genérica de beneficiamento a partir de uma classe MaterialProjetoBenef.
        /// </summary>
        /// <param name="mpb"></param>
        public GenericBenef(MaterialProjetoBenef mpb)
        {
            _tipo = TipoProdutoBeneficiamento.MaterialProjeto;
            IdMaterialItemProjetoBenef = mpb.IdMaterProjBenef;
            IdMaterialItemProjeto = mpb.IdMaterItemProj;
            BisAlt = mpb.BisAlt;
            BisLarg = mpb.BisLarg;
            EspBisote = mpb.EspBisote;
            EspFuro = mpb.EspFuro;
            IdBenefConfig = mpb.IdBenefConfig;
            LapAlt = mpb.LapAlt;
            LapLarg = mpb.LapLarg;
            Qtd = mpb.Qtd;
            _valorUnit = mpb.ValorUnit;
            Valor = mpb.Valor;
            Custo = mpb.Custo;
            Padrao = mpb.Padrao;
        }

        /// <summary>
        /// Cria uma classe genérica de beneficiamento a partir de uma classe ProdutoPedidoEspelhoBenef.
        /// </summary>
        /// <param name="ppb"></param>
        public GenericBenef(ProdutoPedidoEspelhoBenef ppeb)
        {
            _tipo = TipoProdutoBeneficiamento.ProdutoPedidoEspelho;
            IdProdutoPedidoEspelhoBenef = ppeb.IdProdPedEspBenef;
            IdProdutoPedidoEspelho = ppeb.IdProdPed;
            IdBenefConfig = ppeb.IdBenefConfig;
            BisAlt = ppeb.BisAlt;
            BisLarg = ppeb.BisLarg;
            EspBisote = ppeb.EspBisote;
            EspFuro = ppeb.EspFuro;
            LapAlt = ppeb.LapAlt;
            LapLarg = ppeb.LapLarg;
            Qtd = ppeb.Qtd;
            _valorUnit = ppeb.ValorUnit;
            Valor = ppeb.Valor;
            Custo = ppeb.Custo;
            Padrao = ppeb.Padrao;
            ValorComissao = ppeb.ValorComissao;
            ValorAcrescimo = ppeb.ValorAcrescimo;
            ValorAcrescimoProd = ppeb.ValorAcrescimoProd;
            ValorDesconto = ppeb.ValorDesconto;
            ValorDescontoProd = ppeb.ValorDescontoProd;
        }

        /// <summary>
        /// Cria uma classe genérica de beneficiamento a partir de uma classe ProdutosCompraBenef.
        /// </summary>
        /// <param name="pcb"></param>
        public GenericBenef(ProdutosCompraBenef pcb)
        {
            _tipo = TipoProdutoBeneficiamento.ProdutoCompra;
            IdProdutoCompraBenef = pcb.IdProdCompraBenef;
            IdProdutoCompra = pcb.IdProdCompra;
            IdBenefConfig = pcb.IdBenefConfig;
            BisAlt = pcb.BisAlt;
            BisLarg = pcb.BisLarg;
            EspBisote = pcb.EspBisote;
            EspFuro = pcb.EspFuro;
            LapAlt = pcb.LapAlt;
            LapLarg = pcb.LapLarg;
            Qtd = pcb.Qtde;
            _valorUnit = pcb.ValorUnit;
            Valor = pcb.Valor;
            Custo = pcb.Custo;
        }

        /// <summary>
        /// Cria uma classe genérica de beneficiamento a partir de uma classe ProdutoBenef.
        /// </summary>
        /// <param name="pb"></param>
        public GenericBenef(ProdutoBenef pb)
        {
            _tipo = TipoProdutoBeneficiamento.Produto;
            IdProdutoBenef = (uint)pb.IdProdBenef;
            IdProduto = (uint)pb.IdProd;
            IdBenefConfig = (uint)pb.IdBenefConfig;
            BisAlt = pb.BisAlt;
            BisLarg = pb.BisLarg;
            EspBisote = pb.EspBisote;
            EspFuro = pb.EspFuro;
            LapAlt = pb.LapAlt;
            LapLarg = pb.LapLarg;
            Qtd = pb.Qtd;
        }

        /// <summary>
        /// Cria uma classe genérica de beneficiamento a partir de uma classe ProdutoTrocaDevolucaoBenef.
        /// </summary>
        /// <param name="ptdb"></param>
        public GenericBenef(ProdutoTrocaDevolucaoBenef ptdb)
        {
            _tipo = TipoProdutoBeneficiamento.ProdutoTrocaDevolucao;
            IdProdutoTrocaDevolucaoBenef = ptdb.IdProdTrocaDevBenef;
            IdProdutoTrocaDevolucao = ptdb.IdProdTrocaDev;
            IdBenefConfig = ptdb.IdBenefConfig;
            BisAlt = ptdb.BisAlt;
            BisLarg = ptdb.BisLarg;
            EspBisote = ptdb.EspBisote;
            EspFuro = ptdb.EspFuro;
            LapAlt = ptdb.LapAlt;
            LapLarg = ptdb.LapLarg;
            Qtd = ptdb.Qtd;
            _valorUnit = ptdb.ValorUnit;
            Valor = ptdb.Valor;
            Custo = ptdb.Custo;
        }

        /// <summary>
        /// Cria uma classe genérica de beneficiamento a partir de uma classe PecaModeloBenef.
        /// </summary>
        /// <param name="pmb"></param>
        public GenericBenef(PecaModeloBenef pmb)
        {
            _tipo = TipoProdutoBeneficiamento.PecaModeloProjeto;
            IdPecaProjetoModeloBenef = pmb.IdPecaModeloBenef;
            IdPecaProjetoModelo = pmb.IdPecaProjMod;
            IdBenefConfig = pmb.IdBenefConfig;
            BisAlt = pmb.BisAlt;
            BisLarg = pmb.BisLarg;
            EspBisote = pmb.EspBisote;
            EspFuro = pmb.EspFuro;
            LapAlt = pmb.LapAlt;
            LapLarg = pmb.LapLarg;
            Qtd = pmb.Qtd;
            Valor = pmb.Valor;
            Custo = pmb.Custo;
        }

        /// <summary>
        /// Cria uma classe genérica de beneficiamento a partir de uma classe PecaItemProjBenef.
        /// </summary>
        /// <param name="pipb"></param>
        public GenericBenef(PecaItemProjBenef pipb)
        {
            _tipo = TipoProdutoBeneficiamento.PecaItemProjeto;
            IdPecaItemProjetoBenef = pipb.IdPecaItemProjBenef;
            IdPecaItemProjeto = pipb.IdPecaItemProj;
            IdBenefConfig = pipb.IdBenefConfig;
            BisAlt = pipb.BisAlt;
            BisLarg = pipb.BisLarg;
            EspBisote = pipb.EspBisote;
            EspFuro = pipb.EspFuro;
            LapAlt = pipb.LapAlt;
            LapLarg = pipb.LapLarg;
            Qtd = pipb.Qtd;
        }

        /// <summary>
        /// Cria uma classe genérica de beneficiamento a partir de uma classe ProdutoTrocadoBenef.
        /// </summary>
        /// <param name="ptdb"></param>
        public GenericBenef(ProdutoTrocadoBenef ptb)
        {
            _tipo = TipoProdutoBeneficiamento.ProdutoTrocado;
            IdProdutoTrocadoBenef = ptb.IdProdTrocadoBenef;
            IdProdutoTrocado = ptb.IdProdTrocado;
            IdBenefConfig = ptb.IdBenefConfig;
            BisAlt = ptb.BisAlt;
            BisLarg = ptb.BisLarg;
            EspBisote = ptb.EspBisote;
            EspFuro = ptb.EspFuro;
            LapAlt = ptb.LapAlt;
            LapLarg = ptb.LapLarg;
            Qtd = ptb.Qtd;
            _valorUnit = ptb.ValorUnit;
            Valor = ptb.Valor;
            Custo = ptb.Custo;
        }

        /// <summary>
        /// Cria uma classe genérica de beneficiamento a partir de uma classe ProdutoTrocadoBenef.
        /// </summary>
        /// <param name="ptdb"></param>
        public GenericBenef(ProdutoBaixaEstoqueBenef pbeb)
        {
            _tipo = TipoProdutoBeneficiamento.ProdutoBaixaEst;
            IdProdBaixaEstBenef = (uint)pbeb.IdProdBaixaEstBenef;
            IdProdBaixaEst = (uint)pbeb.IdProdBaixaEst;
            IdBenefConfig = (uint)pbeb.IdBenefConfig;
            BisAlt = pbeb.BisAlt;
            BisLarg = pbeb.BisLarg;
            EspBisote = pbeb.EspBisote;
            EspFuro = pbeb.EspFuro;
            LapAlt = pbeb.LapAlt;
            LapLarg = pbeb.LapLarg;
            Qtd = pbeb.Qtd;
        }

        #endregion

        #region Conversores

        /// <summary>
        /// Converte essa classe genérica em uma classe ProdutoPedidoBenef.
        /// </summary>
        /// <returns></returns>
        public ProdutoPedidoBenef ToProdutoPedido(uint idProdPed)
        {
            if (idProdPed > 0)
                IdProdutoPedido = idProdPed;

            ProdutoPedidoBenef ppb = new ProdutoPedidoBenef();
            ppb.IdProdPedBenef = IdProdutoPedidoBenef;
            ppb.IdProdPed = IdProdutoPedido;
            ppb.IdBenefConfig = IdBenefConfig;
            ppb.BisAlt = BisAlt;
            ppb.BisLarg = BisLarg;
            ppb.EspBisote = EspBisote;
            ppb.EspFuro = EspFuro;
            ppb.LapAlt = LapAlt;
            ppb.LapLarg = LapLarg;
            ppb.Qtd = Qtd;
            ppb.ValorUnit = _valorUnit;
            ppb.Valor = Valor;
            ppb.Custo = Custo;
            ppb.Padrao = Padrao;
            ppb.ValorComissao = ValorComissao;
            ppb.ValorAcrescimo = ValorAcrescimo;
            ppb.ValorAcrescimoProd = ValorAcrescimoProd;
            ppb.ValorDesconto = ValorDesconto;
            ppb.ValorDescontoProd = ValorDescontoProd;

            return ppb;
        }

        /// <summary>
        /// Converte essa classe genérica em uma classe ProdutoOrcamentoBenef.
        /// </summary>
        /// <returns></returns>
        public ProdutoOrcamentoBenef ToProdutoOrcamento(uint idProdOrca)
        {
            if (idProdOrca > 0)
                IdProdutoOrcamento = idProdOrca;

            ProdutoOrcamentoBenef pob = new ProdutoOrcamentoBenef();
            pob.IdProdOrcaBenef = IdProdutoOrcamentoBenef;
            pob.IdProd = IdProdutoOrcamento;
            pob.BisAlt = BisAlt;
            pob.BisLarg = BisLarg;
            pob.EspBisote = EspBisote;
            pob.EspFuro = EspFuro;
            pob.IdBenefConfig = IdBenefConfig;
            pob.LapAlt = LapAlt;
            pob.LapLarg = LapLarg;
            pob.Qtde = Qtd;
            pob.ValorUnit = _valorUnit;
            pob.Valor = Valor;
            pob.Custo = Custo;
            pob.Padrao = Padrao;
            pob.ValorComissao = ValorComissao;
            pob.ValorAcrescimo = ValorAcrescimo;
            pob.ValorAcrescimoProd = ValorAcrescimoProd;
            pob.ValorDesconto = ValorDesconto;
            pob.ValorDescontoProd = ValorDescontoProd;

            return pob;
        }

        /// <summary>
        /// Converte essa classe genérica em uma classe MaterialProjetoBenef.
        /// </summary>
        /// <param name="idMaterialProj"></param>
        /// <returns></returns>
        public MaterialProjetoBenef ToMaterialProjeto(uint idMaterialProj)
        {
            if (idMaterialProj > 0)
                IdMaterialItemProjeto = idMaterialProj;

            MaterialProjetoBenef mpb = new MaterialProjetoBenef();
            mpb.IdMaterProjBenef = IdMaterialItemProjetoBenef;
            mpb.IdMaterItemProj = IdMaterialItemProjeto;
            mpb.BisAlt = BisAlt;
            mpb.BisLarg = BisLarg;
            mpb.EspBisote = EspBisote;
            mpb.EspFuro = EspFuro;
            mpb.IdBenefConfig = IdBenefConfig;
            mpb.LapAlt = LapAlt;
            mpb.LapLarg = LapLarg;
            mpb.Qtd = Qtd;
            mpb.ValorUnit = _valorUnit;
            mpb.Valor = Valor;
            mpb.Custo = Custo;
            mpb.Padrao = Padrao;

            return mpb;
        }

        /// <summary>
        /// Converte essa classe genérica em uma classe ProdutoPedidoEspelhoBenef.
        /// </summary>
        /// <returns></returns>
        public ProdutoPedidoEspelhoBenef ToProdutoPedidoEspelho(uint idProdPedEsp)
        {
            if (idProdPedEsp > 0)
                IdProdutoPedidoEspelho = idProdPedEsp;

            ProdutoPedidoEspelhoBenef ppeb = new ProdutoPedidoEspelhoBenef();
            ppeb.IdProdPedEspBenef = IdProdutoPedidoEspelhoBenef;
            ppeb.IdProdPed = IdProdutoPedidoEspelho;
            ppeb.IdBenefConfig = IdBenefConfig;
            ppeb.BisAlt = BisAlt;
            ppeb.BisLarg = BisLarg;
            ppeb.EspBisote = EspBisote;
            ppeb.EspFuro = EspFuro;
            ppeb.LapAlt = LapAlt;
            ppeb.LapLarg = LapLarg;
            ppeb.Qtd = Qtd;
            ppeb.ValorUnit = _valorUnit;
            ppeb.Valor = Valor;
            ppeb.Custo = Custo;
            ppeb.Padrao = Padrao;
            ppeb.ValorComissao = ValorComissao;
            ppeb.ValorAcrescimo = ValorAcrescimo;
            ppeb.ValorAcrescimoProd = ValorAcrescimoProd;
            ppeb.ValorDesconto = ValorDesconto;
            ppeb.ValorDescontoProd = ValorDescontoProd;

            return ppeb;
        }

        /// <summary>
        /// Converte essa classe genérica em uma classe ProdutosCompraBenef.
        /// </summary>
        /// <returns></returns>
        public ProdutosCompraBenef ToProdutoCompra(uint idProdCompra)
        {
            if (idProdCompra > 0)
                IdProdutoCompra = idProdCompra;

            ProdutosCompraBenef pcb = new ProdutosCompraBenef();
            pcb.IdProdCompraBenef = IdProdutoCompraBenef;
            pcb.IdProdCompra = IdProdutoCompra;
            pcb.IdBenefConfig = IdBenefConfig;
            pcb.BisAlt = BisAlt;
            pcb.BisLarg = BisLarg;
            pcb.EspBisote = EspBisote;
            pcb.EspFuro = EspFuro;
            pcb.LapAlt = LapAlt;
            pcb.LapLarg = LapLarg;
            pcb.Qtde = Qtd;
            pcb.ValorUnit = _valorUnit;
            pcb.Valor = Valor;
            pcb.Custo = Custo;

            return pcb;
        }

        /// <summary>
        /// Converte essa classe genérica em uma classe ProdutoBenef.
        /// </summary>
        /// <returns></returns>
        public ProdutoBenef ToProduto(uint idProd)
        {
            if (idProd > 0)
                IdProduto = idProd;

            ProdutoBenef pb = new ProdutoBenef();
            pb.IdProdBenef = (int)IdProdutoBenef;
            pb.IdProd = (int)IdProduto;
            pb.IdBenefConfig = (int)IdBenefConfig;
            pb.BisAlt = BisAlt;
            pb.BisLarg = BisLarg;
            pb.EspBisote = EspBisote;
            pb.EspFuro = EspFuro;
            pb.LapAlt = LapAlt;
            pb.LapLarg = LapLarg;
            pb.Qtd = Qtd;

            return pb;
        }

        /// <summary>
        /// Converte essa classe genérica em uma classe ProdutoTrocaDevolucaoBenef.
        /// </summary>
        /// <returns></returns>
        public ProdutoTrocaDevolucaoBenef ToProdutoTrocaDevolucao(uint idProdTrocaDev)
        {
            if (idProdTrocaDev > 0)
                IdProdutoTrocaDevolucao = idProdTrocaDev;

            ProdutoTrocaDevolucaoBenef ptdb = new ProdutoTrocaDevolucaoBenef();
            ptdb.IdProdTrocaDevBenef = IdProdutoTrocaDevolucaoBenef;
            ptdb.IdProdTrocaDev = IdProdutoTrocaDevolucao;
            ptdb.IdBenefConfig = IdBenefConfig;
            ptdb.BisAlt = BisAlt;
            ptdb.BisLarg = BisLarg;
            ptdb.EspBisote = EspBisote;
            ptdb.EspFuro = EspFuro;
            ptdb.LapAlt = LapAlt;
            ptdb.LapLarg = LapLarg;
            ptdb.Qtd = Qtd;
            ptdb.ValorUnit = _valorUnit;
            ptdb.Valor = Valor;
            ptdb.Custo = Custo;

            return ptdb;
        }

        /// <summary>
        /// Converte essa classe genérica em uma classe PecaModeloBenef.
        /// </summary>
        /// <returns></returns>
        public PecaModeloBenef ToPecaProjetoModelo(uint idPecaProjMod)
        {
            if (idPecaProjMod > 0)
                IdPecaProjetoModelo = idPecaProjMod;

            PecaModeloBenef pmb = new PecaModeloBenef();
            pmb.IdPecaModeloBenef = IdPecaProjetoModeloBenef;
            pmb.IdPecaProjMod = IdPecaProjetoModelo;
            pmb.IdBenefConfig = IdBenefConfig;
            pmb.BisAlt = BisAlt;
            pmb.BisLarg = BisLarg;
            pmb.EspBisote = EspBisote;
            pmb.EspFuro = EspFuro;
            pmb.LapAlt = LapAlt;
            pmb.LapLarg = LapLarg;
            pmb.Qtd = Qtd;

            return pmb;
        }

        /// <summary>
        /// Converte essa classe genérica em uma classe PecaItemProjBenef.
        /// </summary>
        /// <returns></returns>
        public PecaItemProjBenef ToPecaItemProjeto(uint idPecaItemProj)
        {
            if (idPecaItemProj > 0)
                IdPecaItemProjeto = idPecaItemProj;

            PecaItemProjBenef pipb = new PecaItemProjBenef();
            pipb.IdPecaItemProjBenef = IdPecaItemProjetoBenef;
            pipb.IdPecaItemProj = IdPecaItemProjeto;
            pipb.IdBenefConfig = IdBenefConfig;
            pipb.BisAlt = BisAlt;
            pipb.BisLarg = BisLarg;
            pipb.EspBisote = EspBisote;
            pipb.EspFuro = EspFuro;
            pipb.LapAlt = LapAlt;
            pipb.LapLarg = LapLarg;
            pipb.Qtd = Qtd;

            return pipb;
        }

        /// <summary>
        /// Converte essa classe genérica em uma classe ProdutoTrocadoBenef.
        /// </summary>
        /// <returns></returns>
        public ProdutoTrocadoBenef ToProdutoTrocado(uint idProdTrocado)
        {
            if (idProdTrocado > 0)
                IdProdutoTrocado = idProdTrocado;

            ProdutoTrocadoBenef ptb = new ProdutoTrocadoBenef();
            ptb.IdProdTrocadoBenef = IdProdutoTrocadoBenef;
            ptb.IdProdTrocado = IdProdutoTrocado;
            ptb.IdBenefConfig = IdBenefConfig;
            ptb.BisAlt = BisAlt;
            ptb.BisLarg = BisLarg;
            ptb.EspBisote = EspBisote;
            ptb.EspFuro = EspFuro;
            ptb.LapAlt = LapAlt;
            ptb.LapLarg = LapLarg;
            ptb.Qtd = Qtd;
            ptb.ValorUnit = _valorUnit;
            ptb.Valor = Valor;
            ptb.Custo = Custo;

            return ptb;
        }

        /// <summary>
        /// Converte essa classe genérica em uma classe ProdutoBaixaEstoqueBenef.
        /// </summary>
        /// <returns></returns>
        public ProdutoBaixaEstoqueBenef ToProdutoBaixaEst(uint idProdBaixaEst)
        {
            if (idProdBaixaEst > 0)
                IdProdBaixaEst = idProdBaixaEst;

            ProdutoBaixaEstoqueBenef pbeb = new ProdutoBaixaEstoqueBenef();
            pbeb.IdProdBaixaEstBenef = (int)IdProdBaixaEstBenef;
            pbeb.IdProdBaixaEst = (int)IdProdBaixaEst;
            pbeb.IdBenefConfig = (int)IdBenefConfig;
            pbeb.BisAlt = BisAlt;
            pbeb.BisLarg = BisLarg;
            pbeb.EspBisote = EspBisote;
            pbeb.EspFuro = EspFuro;
            pbeb.LapAlt = LapAlt;
            pbeb.LapLarg = LapLarg;
            pbeb.Qtd = Qtd;

            return pbeb;
        }

        #endregion

        #region Recupera o valor unitário do beneficiamento

        internal static decimal GetValorUnit(decimal valorUnit, decimal valor, int qtd)
        {
            return valorUnit > 0 ? valorUnit : valor / (decimal)Math.Max(qtd, 1);
        }

        #endregion

        #region Método de retorno de dados do produto

        public struct DadosProduto
        {
            public uint Id;
            public float Qtd;
            public decimal ValorUnit;
            public decimal ValorTotal;
            public float Altura;
            public int Largura;
            public float TotalM2;
        }

        /// <summary>
        /// Retorna os dados do produto associado ao beneficiamento.
        /// </summary>
        /// <returns></returns>
        public DadosProduto GetProduto()
        {
            string where;
            DadosProduto retorno = new DadosProduto();

            switch (_tipo)
            {
                case TipoProdutoBeneficiamento.MaterialProjeto:
                    where = "idMaterItemProj=" + IdMaterialItemProjeto;
                    retorno.Id = IdMaterialItemProjeto;
                    retorno.Qtd = MaterialItemProjetoDAO.Instance.ObtemValorCampo<float>("qtde", where);
                    retorno.ValorUnit = MaterialItemProjetoDAO.Instance.ObtemValorCampo<decimal>("valor", where); ;
                    retorno.ValorTotal = MaterialItemProjetoDAO.Instance.ObtemValorCampo<decimal>("total", where); ;
                    retorno.Altura = MaterialItemProjetoDAO.Instance.ObtemValorCampo<float>("altura", where); ;
                    retorno.Largura = MaterialItemProjetoDAO.Instance.ObtemValorCampo<int>("largura", where); ;
                    retorno.TotalM2 = MaterialItemProjetoDAO.Instance.ObtemValorCampo<float>("totM", where); ;
                    break;

                case TipoProdutoBeneficiamento.Produto:
                    retorno.Id = IdProduto;
                    retorno.Qtd = 0;
                    retorno.ValorUnit = 0;
                    retorno.ValorTotal = 0;
                    retorno.Altura = 0;
                    retorno.Largura = 0;
                    retorno.TotalM2 = 0;
                    break;

                case TipoProdutoBeneficiamento.ProdutoCompra:
                    where = "idProdCompra=" + IdProdutoCompra;
                    retorno.Id = IdProdutoCompra;
                    retorno.Qtd = ProdutosCompraDAO.Instance.ObtemValorCampo<float>("qtde", where);
                    retorno.ValorUnit = ProdutosCompraDAO.Instance.ObtemValorCampo<decimal>("valor", where);
                    retorno.ValorTotal = ProdutosCompraDAO.Instance.ObtemValorCampo<decimal>("total", where);
                    retorno.Altura = ProdutosCompraDAO.Instance.ObtemValorCampo<float>("altura", where);
                    retorno.Largura = ProdutosCompraDAO.Instance.ObtemValorCampo<int>("largura", where);
                    retorno.TotalM2 = ProdutosCompraDAO.Instance.ObtemValorCampo<float>("totM", where);
                    break;

                case TipoProdutoBeneficiamento.ProdutoOrcamento:
                    where = "idProd=" + IdProdutoOrcamento;
                    retorno.Id = IdProdutoOrcamento;
                    retorno.Qtd = ProdutosOrcamentoDAO.Instance.ObtemValorCampo<float>("qtde", where);
                    retorno.ValorUnit = ProdutosOrcamentoDAO.Instance.ObtemValorCampo<decimal>("valorProd", where);
                    retorno.ValorTotal = ProdutosOrcamentoDAO.Instance.ObtemValorCampo<decimal>("total", where);
                    retorno.Altura = ProdutosOrcamentoDAO.Instance.ObtemValorCampo<float>("alturaCalc", where);
                    retorno.Largura = ProdutosOrcamentoDAO.Instance.ObtemValorCampo<int>("largura", where);
                    retorno.TotalM2 = ProdutosOrcamentoDAO.Instance.ObtemValorCampo<float>("totM", where);
                    break;

                case TipoProdutoBeneficiamento.ProdutoPedido:
                    where = "idProdPed=" + IdProdutoPedido;
                    retorno.Id = IdProdutoPedido;
                    retorno.Qtd = ProdutosPedidoDAO.Instance.ObtemValorCampo<float>("qtde", where);
                    retorno.ValorUnit = ProdutosPedidoDAO.Instance.ObtemValorCampo<decimal>("valorVendido", where);
                    retorno.ValorTotal = ProdutosPedidoDAO.Instance.ObtemValorCampo<decimal>("total", where);
                    retorno.Altura = ProdutosPedidoDAO.Instance.ObtemValorCampo<float>("altura", where);
                    retorno.Largura = ProdutosPedidoDAO.Instance.ObtemValorCampo<int>("largura", where);
                    retorno.TotalM2 = ProdutosPedidoDAO.Instance.ObtemValorCampo<float>("totM2Calc", where);
                    break;

                case TipoProdutoBeneficiamento.ProdutoPedidoEspelho:
                    where = "idProdPed=" + IdProdutoPedido;
                    retorno.Id = IdProdutoPedidoEspelho;
                    retorno.Qtd = ProdutosPedidoEspelhoDAO.Instance.ObtemValorCampo<float>("qtde", where);
                    retorno.ValorUnit = ProdutosPedidoEspelhoDAO.Instance.ObtemValorCampo<decimal>("valorVendido", where);
                    retorno.ValorTotal = ProdutosPedidoEspelhoDAO.Instance.ObtemValorCampo<decimal>("total", where);
                    retorno.Altura = ProdutosPedidoEspelhoDAO.Instance.ObtemValorCampo<float>("altura", where);
                    retorno.Largura = ProdutosPedidoEspelhoDAO.Instance.ObtemValorCampo<int>("largura", where);
                    retorno.TotalM2 = ProdutosPedidoEspelhoDAO.Instance.ObtemValorCampo<float>("totM2Calc", where);
                    break;

                case TipoProdutoBeneficiamento.ProdutoTrocaDevolucao:
                    where = "idProdTrocaDev=" + IdProdutoPedido;
                    retorno.Id = IdProdutoTrocaDevolucao;
                    retorno.Qtd = ProdutoTrocaDevolucaoDAO.Instance.ObtemValorCampo<float>("qtde", where);
                    retorno.ValorUnit = ProdutoTrocaDevolucaoDAO.Instance.ObtemValorCampo<decimal>("valorVendido", where);
                    retorno.ValorTotal = ProdutoTrocaDevolucaoDAO.Instance.ObtemValorCampo<decimal>("total", where);
                    retorno.Altura = ProdutoTrocaDevolucaoDAO.Instance.ObtemValorCampo<float>("altura", where);
                    retorno.Largura = ProdutoTrocaDevolucaoDAO.Instance.ObtemValorCampo<int>("largura", where);
                    retorno.TotalM2 = ProdutoTrocaDevolucaoDAO.Instance.ObtemValorCampo<float>("totM2Calc", where);
                    break;

                case TipoProdutoBeneficiamento.PecaModeloProjeto:
                    retorno.Id = IdPecaProjetoModelo;
                    retorno.Qtd = 0;
                    retorno.ValorUnit = 0;
                    retorno.ValorTotal = 0;
                    retorno.Altura = 0;
                    retorno.Largura = 0;
                    retorno.TotalM2 = 0;
                    break;

                case TipoProdutoBeneficiamento.PecaItemProjeto:
                    retorno.Id = IdPecaItemProjeto;
                    retorno.Qtd = 0;
                    retorno.ValorUnit = 0;
                    retorno.ValorTotal = 0;
                    retorno.Altura = 0;
                    retorno.Largura = 0;
                    retorno.TotalM2 = 0;
                    break;

                case TipoProdutoBeneficiamento.ProdutoTrocado:
                    where = "idProdTrocado=" + IdProdutoPedido;
                    retorno.Id = IdProdutoTrocado;
                    retorno.Qtd = ProdutoTrocadoDAO.Instance.ObtemValorCampo<float>("qtde", where);
                    retorno.ValorUnit = ProdutoTrocadoDAO.Instance.ObtemValorCampo<decimal>("valorVendido", where);
                    retorno.ValorTotal = ProdutoTrocadoDAO.Instance.ObtemValorCampo<decimal>("total", where);
                    retorno.Altura = ProdutoTrocadoDAO.Instance.ObtemValorCampo<float>("altura", where);
                    retorno.Largura = ProdutoTrocadoDAO.Instance.ObtemValorCampo<int>("largura", where);
                    retorno.TotalM2 = ProdutoTrocadoDAO.Instance.ObtemValorCampo<float>("totM2Calc", where);
                    break;

                case TipoProdutoBeneficiamento.ProdutoBaixaEst:
                    where = "IdProdBaixaEst=" + IdProdBaixaEst;
                    retorno.Id = IdProdBaixaEst;
                    retorno.Qtd = ProdutoBaixaEstoqueDAO.Instance.ObtemValorCampo<float>("qtde", where); ;
                    retorno.ValorUnit = 0;
                    retorno.ValorTotal = 0;
                    retorno.Altura = 0;
                    retorno.Largura = 0;
                    retorno.TotalM2 = 0;
                    break;
            }

            return retorno;
        }

        #endregion
    }
}