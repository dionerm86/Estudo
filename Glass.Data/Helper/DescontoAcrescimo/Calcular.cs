using System;
using Glass.Data.Model;
using Glass.Data.DAL;
using GDA;
using Glass.Configuracoes;
using Glass.Global;
using Glass.Pool;
using System.Linq;
using System.Collections.Generic;

namespace Glass.Data.Helper.DescontoAcrescimo
{
    internal sealed class Calcular : PoolableObject<Calcular>
    {
        private Calcular() { }

        #region Classe de suporte (herdada de BaseDAO)

        sealed class TempDAO : BaseDAO<TempDAO.DadosDiferenca, TempDAO>
        {
            //private TempDAO() { }

            #region Model de retorno

            [PersistenceBaseDAO(typeof(TempDAO))]
            internal class DadosDiferenca
            {
                public uint? IdCliente { get; set; }
                public int? TipoEntrega { get; set; }
                public bool IsReposicao { get; set; }
            }

            #endregion

            public DadosDiferenca GetDadosParaDiferenca(GDASession sessao, IProdutoDescontoAcrescimo produto)
            {
                string sqlBase = "select {0} from {1} where {2}=?idParent";
                GDAParameter id = new GDAParameter("?idParent", produto.IdParent);

                uint? idCliente;
                int? tipoEntrega;
                bool reposicao;

                if (produto is ProdutosOrcamento)
                {
                    sqlBase = String.Format(sqlBase, "{0}", "orcamento", "idOrcamento");
                    idCliente = ExecuteScalar<uint?>(sessao, String.Format(sqlBase, "idCliente"), id);
                    tipoEntrega = ExecuteScalar<int?>(sessao, String.Format(sqlBase, "tipoEntrega"), id);
                    reposicao = false;
                }
                else if (produto is ProdutosPedido || produto is ProdutosPedidoEspelho)
                {
                    sqlBase = String.Format(sqlBase, "{0}", "pedido", "idPedido");
                    idCliente = ExecuteScalar<uint?>(sessao, String.Format(sqlBase, "idCli"), id);
                    tipoEntrega = ExecuteScalar<int?>(sessao, String.Format(sqlBase, "tipoEntrega"), id);
                    reposicao = ExecuteScalar<bool>(sessao, String.Format(sqlBase, "tipoVenda=" + (int)Pedido.TipoVendaPedido.Reposição), id);
                }
                else if (produto is ProdutoTrocado)
                {
                    sqlBase = String.Format(sqlBase, "{0}", "troca_devolucao", "idTrocaDevolucao");
                    idCliente = ExecuteScalar<uint?>(sessao, String.Format(sqlBase, "idCliente"), id);
                    tipoEntrega = null;
                    reposicao = false;
                }
                else if (produto is ProdutoTrocaDevolucao)
                {
                    sqlBase = String.Format(sqlBase, "{0}", "troca_devolucao", "idTrocaDevolucao");
                    idCliente = ExecuteScalar<uint?>(sessao, String.Format(sqlBase, "idCliente"), id);
                    tipoEntrega = null;
                    reposicao = false;
                }
                else if (produto is MaterialItemProjeto)
                {
                    sqlBase = @"select coalesce(proj.{0}, coalesce(ped.{1}, coalesce(pedEsp.{2}, orca.{3})))
                        from item_projeto ip left join projeto proj on (ip.idProjeto=proj.idProjeto)
                        left join pedido ped on (ip.idPedido=ped.idPedido)
                        left join pedido pedEsp on (ip.idPedidoEspelho=pedEsp.idPedido)
                        left join orcamento orca on (ip.idOrcamento=orca.idOrcamento)
                        where idItemProjeto=?idParent";

                    idCliente = ExecuteScalar<uint?>(sessao, String.Format(sqlBase, "idCliente", "idCli", "idCli", "idCliente"), id);
                    tipoEntrega = ExecuteScalar<int?>(sessao, String.Format(sqlBase, "tipoEntrega", "tipoEntrega", "tipoEntrega", "tipoEntrega"), id);
                    reposicao = ExecuteScalar<bool>(sessao, "select ped.tipoVenda=" + (int)Pedido.TipoVendaPedido.Reposição + @"
                        from item_projeto ip left join pedido ped on (coalesce(ip.idPedido, ip.idPedidoEspelho)=ped.idPedido)
                        where ip.idItemProjeto=?idParent", id);
                }
                else
                    throw new ArgumentException("A classe '" + produto.GetType().Name + "' ainda não foi configurada no objeto DescontoAcrescimo.", "produto");

                return new DadosDiferenca()
                {
                    IdCliente = idCliente,
                    TipoEntrega = tipoEntrega,
                    IsReposicao = reposicao
                }; ;
            }
        }

        #endregion

        #region Aplica acréscimo no valor dos produtos

        /// <summary>
        /// Aplica acréscimo no valor dos produtos.
        /// </summary>
        public bool AplicaAcrescimo(int tipoAcrescimo, decimal acrescimo, IEnumerable<IProdutoDescontoAcrescimo> produtos,
            IContainerDescontoAcrescimo container)
        {
            var estrategia = CalculoStrategyFactory.Instance.RecuperaEstrategia(
                Estrategia.Enum.TipoCalculo.Acrescimo,
                Estrategia.Enum.TipoAplicacao.Geral
            );

            return estrategia.Aplicar(
                (Estrategia.Enum.TipoValor)tipoAcrescimo,
                acrescimo,
                produtos,
                container
            );
        }

        /// <summary>
        /// Aplica acréscimo do ambiente no valor dos produtos.
        /// </summary>
        public bool AplicaAcrescimoAmbiente(int tipoAcrescimo, decimal acrescimo, IEnumerable<IProdutoDescontoAcrescimo> produtos,
            IContainerDescontoAcrescimo container)
        {
            var estrategia = CalculoStrategyFactory.Instance.RecuperaEstrategia(
                Estrategia.Enum.TipoCalculo.Acrescimo,
                Estrategia.Enum.TipoAplicacao.Ambiente
            );

            return estrategia.Aplicar(
                (Estrategia.Enum.TipoValor)tipoAcrescimo,
                acrescimo,
                produtos,
                container
            );
        }

        #endregion

        #region Remove acréscimo no valor dos produtos

        /// <summary>
        /// Remove acréscimo no valor dos produtos.
        /// </summary>
        public bool RemoveAcrescimo(IEnumerable<IProdutoDescontoAcrescimo> produtos, IContainerDescontoAcrescimo container)
        {
            var estrategia = CalculoStrategyFactory.Instance.RecuperaEstrategia(
                Estrategia.Enum.TipoCalculo.Acrescimo,
                Estrategia.Enum.TipoAplicacao.Geral
            );

            return estrategia.Remover(
                produtos,
                container
            );
        }

        /// <summary>
        /// Remove acréscimo do ambiente no valor dos produtos.
        /// </summary>
        public bool RemoveAcrescimoAmbiente(IEnumerable<IProdutoDescontoAcrescimo> produtos, IContainerDescontoAcrescimo container)
        {
            var estrategia = CalculoStrategyFactory.Instance.RecuperaEstrategia(
                Estrategia.Enum.TipoCalculo.Acrescimo,
                Estrategia.Enum.TipoAplicacao.Ambiente
            );

            return estrategia.Remover(
                produtos,
                container
            );
        }

        #endregion

        #region Aplica desconto no valor dos produtos

        /// <summary>
        /// Aplica desconto no valor dos produtos.
        /// </summary>
        public bool AplicaDesconto(int tipoDesconto, decimal desconto, IEnumerable<IProdutoDescontoAcrescimo> produtos,
            IContainerDescontoAcrescimo container)
        {
            var estrategia = CalculoStrategyFactory.Instance.RecuperaEstrategia(
                Estrategia.Enum.TipoCalculo.Desconto,
                Estrategia.Enum.TipoAplicacao.Geral
            );

            return estrategia.Aplicar(
                (Estrategia.Enum.TipoValor)tipoDesconto,
                desconto,
                produtos,
                container
            );
        }

        /// <summary>
        /// Aplica desconto do ambiente no valor dos produtos.
        /// </summary>
        public bool AplicaDescontoAmbiente(int tipoDesconto, decimal desconto, IEnumerable<IProdutoDescontoAcrescimo> produtos,
            IContainerDescontoAcrescimo container)
        {
            var estrategia = CalculoStrategyFactory.Instance.RecuperaEstrategia(
                Estrategia.Enum.TipoCalculo.Desconto,
                Estrategia.Enum.TipoAplicacao.Ambiente
            );

            return estrategia.Aplicar(
                (Estrategia.Enum.TipoValor)tipoDesconto,
                desconto,
                produtos,
                container
            );
        }

        /// <summary>
        /// Aplica desconto por quantidade no valor dos produtos.
        /// </summary>
        public bool AplicaDescontoQtde(IProdutoDescontoAcrescimo produto, IContainerDescontoAcrescimo container)
        {
            var estrategia = CalculoStrategyFactory.Instance.RecuperaEstrategia(
                Estrategia.Enum.TipoCalculo.Desconto,
                Estrategia.Enum.TipoAplicacao.Quantidade
            );

            return estrategia.Aplicar(
                Estrategia.Enum.TipoValor.Percentual,
                (decimal)produto.PercDescontoQtde,
                new[] { produto },
                container
            );
        }

        #endregion

        #region Remove desconto no valor dos produtos

        /// <summary>
        /// Remove desconto no valor dos produtos.
        /// </summary>
        public bool RemoveDesconto(IEnumerable<IProdutoDescontoAcrescimo> produtos, IContainerDescontoAcrescimo container)
        {
            var estrategia = CalculoStrategyFactory.Instance.RecuperaEstrategia(
                Estrategia.Enum.TipoCalculo.Desconto,
                Estrategia.Enum.TipoAplicacao.Geral
            );

            return estrategia.Remover(
                produtos,
                container
            );
        }

        /// <summary>
        /// Remove desconto do ambiente no valor dos produtos.
        /// </summary>
        public bool RemoveDescontoAmbiente(IEnumerable<IProdutoDescontoAcrescimo> produtos, IContainerDescontoAcrescimo container)
        {
            var estrategia = CalculoStrategyFactory.Instance.RecuperaEstrategia(
                Estrategia.Enum.TipoCalculo.Desconto,
                Estrategia.Enum.TipoAplicacao.Ambiente
            );

            return estrategia.Remover(
                produtos,
                container
            );
        }

        /// <summary>
        /// Remove desconto por quantidade no valor dos produtos.
        /// </summary>
        public bool RemoveDescontoQtde(IProdutoDescontoAcrescimo produto, IContainerDescontoAcrescimo container)
        {
            var estrategia = CalculoStrategyFactory.Instance.RecuperaEstrategia(
                Estrategia.Enum.TipoCalculo.Desconto,
                Estrategia.Enum.TipoAplicacao.Quantidade
            );

            return estrategia.Remover(
                new[] { produto },
                container
            );
        }

        #endregion

        #region Aplica comissão no valor dos produtos

        /// <summary>
        /// Aplica desconto por quantidade no valor dos produtos.
        /// </summary>
        public bool AplicaComissao(float percentualComissao, IEnumerable<IProdutoDescontoAcrescimo> produtos,
            IContainerDescontoAcrescimo container)
        {
            var estrategia = CalculoStrategyFactory.Instance.RecuperaEstrategia(
                Estrategia.Enum.TipoCalculo.Comissao,
                Estrategia.Enum.TipoAplicacao.Geral
            );

            return estrategia.Aplicar(
                Estrategia.Enum.TipoValor.Percentual,
                (decimal)percentualComissao,
                produtos,
                container
            );
        }

        #endregion

        #region Remove comissão no valor dos produtos

        /// <summary>
        /// Remove desconto por quantidade no valor dos produtos.
        /// </summary>
        public bool RemoveComissao(IEnumerable<IProdutoDescontoAcrescimo> produtos, IContainerDescontoAcrescimo container)
        {
            var estrategia = CalculoStrategyFactory.Instance.RecuperaEstrategia(
                Estrategia.Enum.TipoCalculo.Comissao,
                Estrategia.Enum.TipoAplicacao.Geral
            );

            return estrategia.Remover(
                produtos,
                container
            );
        }

        #endregion

        #region Calcula o desconto/acréscimo do cliente

        public void DiferencaCliente(IProdutoDescontoAcrescimo produto, IContainerDescontoAcrescimo container)
        {
            DescontoAcrescimo.DiferencaCliente.Instance.Calcular(produto, container);
        }

        #endregion

        #region Calcula os valores brutos do produto

        /// <summary>
        /// Calcula os valores brutos do produto.
        /// </summary>
        /// <param name="produto"></param>
        public void CalculaValorBruto(IProdutoDescontoAcrescimo produto)
        {
            CalculaValorBruto(null, produto);
        }

        /// <summary>
        /// Calcula os valores brutos do produto.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="produto"></param>
        public void CalculaValorBruto(GDASession sessao, IProdutoDescontoAcrescimo produto)
        {
            produto.TotalBruto = produto.Total - produto.ValorAcrescimo - produto.ValorAcrescimoCliente + produto.ValorDesconto + produto.ValorDescontoCliente + produto.ValorDescontoQtde -
                produto.ValorComissao - produto.ValorAcrescimoProd + produto.ValorDescontoProd;

            uint idCliente = TempDAO.Instance.GetDadosParaDiferenca(sessao, produto).IdCliente.GetValueOrDefault();
            decimal valorUnitario = 0;
            var alturaBenef = produto.AlturaBenef == null || (produto.AlturaBenef == 0 && produto.LarguraBenef == 0) ? 2 : produto.AlturaBenef.Value;
            var larguraBenef = produto.LarguraBenef == null || (produto.AlturaBenef == 0 && produto.LarguraBenef == 0) ? 2 : produto.LarguraBenef.Value;

            var isPedidoProducaoCorte = false;
            if (produto is ProdutosPedido || produto is ProdutosPedidoEspelho)
                isPedidoProducaoCorte = PedidoDAO.Instance.IsPedidoProducaoCorte(sessao, produto.IdParent);

            CalculosFluxo.CalcValorUnitItemProd(sessao, idCliente, (int)produto.IdProduto, produto.Largura, produto.Qtde, produto.QtdeAmbiente, produto.TotalBruto,
                produto.Espessura, produto.Redondo, 1, false, !isPedidoProducaoCorte, produto.Altura, produto.TotM, ref valorUnitario, produto.Beneficiamentos.CountAreaMinimaSession(sessao), 
                alturaBenef, larguraBenef);

            produto.ValorUnitarioBruto = valorUnitario;
        }

        #endregion

        #region Calcula o valor unitário do produto (usado ao recalcular)

        private void RecalcularValorUnit(GDASession sessao, IProdutoDescontoAcrescimo prod, int? idPedido, int? idProjeto, int? idOrcamento)
        {
            TempDAO.DadosDiferenca dados = TempDAO.Instance.GetDadosParaDiferenca(sessao, prod);
            RecalcularValorUnit(sessao, prod, dados.IdCliente, dados.TipoEntrega, false, false, idPedido, idProjeto, idOrcamento);
        }

        /// <summary>
        /// Recalcula o valor unitário de um produto.
        /// </summary>
        public void RecalcularValorUnit(IProdutoDescontoAcrescimo prod, uint? idCliente, int? tipoEntrega, int? idPedido, int? idProjeto, int? idOrcamento)
        {
            RecalcularValorUnit(prod, idCliente, tipoEntrega, false, false, idPedido, idProjeto, idOrcamento);
        }

        /// <summary>
        /// Recalcula o valor unitário de um produto.
        /// </summary>
        public void RecalcularValorUnit(IProdutoDescontoAcrescimo prod, uint? idCliente, int? tipoEntrega,
            bool valorBruto, bool calcularAreaMinima, int? idPedido, int? idProjeto, int? idOrcamento)
        {
            RecalcularValorUnit(null, prod, idCliente, tipoEntrega, valorBruto, calcularAreaMinima, idPedido, idProjeto, idOrcamento);
        }

        /// <summary>
        /// Recalcula o valor unitário de um produto.
        /// </summary>
        public void RecalcularValorUnit(GDASession sessao, IProdutoDescontoAcrescimo prod, uint? idCliente, int? tipoEntrega,
            bool valorBruto, bool calcularAreaMinima, int? idPedido, int? idProjeto, int? idOrcamento)
        {
            RecalcularValorUnit(sessao, prod, idCliente, tipoEntrega, valorBruto, calcularAreaMinima, null, idPedido, idProjeto, idOrcamento);
        }

        /// <summary>
        /// Recalcula o valor unitário de um produto.
        /// </summary>
        public void RecalcularValorUnit(GDASession sessao, IProdutoDescontoAcrescimo prod, uint? idCliente, int? tipoEntrega,
            bool valorBruto, bool calcularAreaMinima, Pedido.TipoVendaPedido? tipoVenda, int? idPedido, int? idProjeto, int? idOrcamento)
        {
            if (prod.IdObra > 0 && PedidoConfig.DadosPedido.UsarControleNovoObra)
                return;

            bool pedidoReposicao = false;
            if (prod is ProdutosPedido || prod is ProdutosPedidoEspelho)
                pedidoReposicao =
                    tipoVenda.HasValue ?
                        tipoVenda.Value == Pedido.TipoVendaPedido.Reposição :
                        PedidoDAO.Instance.GetTipoVenda(sessao, prod.IdParent) == (int)Pedido.TipoVendaPedido.Reposição;

            var clienteRevenda = false;

            if (idPedido > 0)
                clienteRevenda = ClienteDAO.Instance.IsRevenda(PedidoDAO.Instance.ObtemIdCliente((uint)idPedido));

            else if (idProjeto > 0)
                clienteRevenda = ClienteDAO.Instance.IsRevenda(ProjetoDAO.Instance.ObtemIdCliente((uint)idProjeto));

            else if (idOrcamento > 0)
                clienteRevenda = ClienteDAO.Instance.IsRevenda(OrcamentoDAO.Instance.ObtemIdCliente((uint)idOrcamento));

            float altura = prod.AlturaCalc, totM2 = prod.TotM, totM2Calc = prod.TotM2Calc;
            decimal custo = 0, total = ProdutoDAO.Instance.GetValorTabela(sessao, (int)prod.IdProduto, tipoEntrega, idCliente, clienteRevenda, pedidoReposicao,
                prod.PercDescontoQtde, idPedido, idProjeto, idOrcamento);

            // Ao efetuar troca de produto, deve-se manter o valor vendido no pedido.
            if (prod is ProdutoTrocado && prod.ValorTabelaPedido > 0)
                total = prod.ValorTabelaPedido;

            // Alteração necessária para calcular corretamente o tubo inserido no projeto, ao recalcular orçamento
            if (altura == 0)
                altura = prod.Altura;

            var idGrupoProd = ProdutoDAO.Instance.ObtemIdGrupoProd(sessao, (int)prod.IdProduto);

            // O campo AlturaCalc no material_item_projeto considera o valor das folgas
            if (prod is MaterialItemProjeto && Glass.Data.DAL.GrupoProdDAO.Instance.IsVidro(idGrupoProd))
                altura = prod.Altura;

            // Considera o campo Altura para alumínios ML Direto
            else if (Glass.Data.DAL.GrupoProdDAO.Instance.IsAluminio(idGrupoProd) &&
                Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo(sessao, (int)prod.IdProduto) == (int)Glass.Data.Model.TipoCalculoGrupoProd.ML)
                altura = prod.Altura;

            var alturaBenef = prod.AlturaBenef == null || (prod.AlturaBenef == 0 && prod.LarguraBenef == 0) ? 2 : prod.AlturaBenef.Value;
            var larguraBenef = prod.LarguraBenef == null || (prod.AlturaBenef == 0 && prod.LarguraBenef == 0) ? 2 : prod.LarguraBenef.Value;

            // Deve passar o parâmetro usarChapaVidro como true, para que caso o produto tenha sido calculado por chapa,
            // não calcule incorretamente o total do mesmo (retornado pela variável total abaixo), estava ocorrendo
            // erro ao chamar esta função a partir de ProdutosPedidoDAO.InsereAtualizaProdProj(), sendo que o produto sendo calculado
            // possuía acréscimo de 25% em caso da área do vidro ser superior à 4m²
            Glass.Data.DAL.ProdutoDAO.Instance.CalcTotaisItemProd(sessao, idCliente.GetValueOrDefault(), (int)prod.IdProduto, prod.Largura, prod.Qtde,
                prod.QtdeAmbiente, total, prod.Espessura, prod.Redondo, 2, prod is ProdutosCompra, true, ref custo, ref altura, ref totM2, ref totM2Calc,
                ref total, alturaBenef, larguraBenef, prod is ProdutosNf, prod.Beneficiamentos.CountAreaMinimaSession(sessao), true, calcularAreaMinima);

            if (PedidoConfig.DadosPedido.AlterarValorUnitarioProduto)
            {
                CalculaValorBruto(sessao, prod);

                if (Math.Round(total, 2) != Math.Round(prod.TotalBruto - prod.ValorDescontoCliente + prod.ValorAcrescimoCliente, 2))
                {
                    var produtoPossuiValorTabela  = ProdutoDAO.Instance.ProdutoPossuiValorTabela(sessao, prod.IdProduto, prod.ValorUnitarioBruto);
                    
                    if (total == 0 || !produtoPossuiValorTabela || (produtoPossuiValorTabela && DescontoAcrescimoClienteDAO.Instance.ProdutoPossuiDesconto(sessao, (int)idCliente.GetValueOrDefault(0), (int)prod.IdProduto)))
                        total = Math.Max(total, prod.TotalBruto - prod.ValorDescontoCliente + prod.ValorAcrescimoCliente);
                }
            }

            if (!valorBruto)
                total += prod.ValorComissao + prod.ValorAcrescimo + prod.ValorAcrescimoProd -
                    (!PedidoConfig.RatearDescontoProdutos ? 0 : prod.ValorDesconto + prod.ValorDescontoProd);

            decimal valorUnit = 0;
            CalculosFluxo.CalcValorUnitItemProd(sessao, idCliente.GetValueOrDefault(), (int)prod.IdProduto, prod.Largura, prod.Qtde, prod.QtdeAmbiente,
                total, prod.Espessura, prod.Redondo, 2, prod is ProdutosCompra, true, altura, totM2, ref valorUnit,
                prod is ProdutosNf, prod.Beneficiamentos.CountAreaMinimaSession(sessao), calcularAreaMinima, alturaBenef, larguraBenef);

            prod.ValorUnit = valorUnit;
        }

        #endregion
    }
}