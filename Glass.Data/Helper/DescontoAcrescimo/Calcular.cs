using System;
using Glass.Data.Model;
using Glass.Data.DAL;
using GDA;
using Glass.Configuracoes;
using Glass.Global;
using Glass.Pool;

namespace Glass.Data.Helper.DescontoAcrescimo
{
    internal sealed class Calcular : PoolableObject<Calcular>
    {
        private const int NUMERO_VEZES_RATEAR = 5;

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
        public bool AplicaAcrescimo(GDASession sessao, int tipoAcrescimo, decimal acrescimo, IProdutoDescontoAcrescimo[] produtos, int? idPedido, int? idProjeto, int? idOrcamento)
        {
            return AplicaAcrescimo(sessao, tipoAcrescimo, acrescimo, GetBaseCalculo(sessao, produtos), produtos, false, 1, idPedido, idProjeto, idOrcamento);
        }

        /// <summary>
        /// Aplica acréscimo do ambiente no valor dos produtos.
        /// </summary>
        public bool AplicaAcrescimoAmbiente(GDASession sessao, int tipoAcrescimo, decimal acrescimo, IProdutoDescontoAcrescimo[] produtos, int? idPedido, int? idProjeto, int? idOrcamento)
        {
            return AplicaAcrescimo(sessao, tipoAcrescimo, acrescimo, GetBaseCalculo(sessao, produtos), produtos, true, 1, idPedido, idProjeto, idOrcamento);
        }

        /// <summary>
        /// Aplica acréscimo no valor dos produtos.
        /// </summary>
        private bool AplicaAcrescimo(GDASession sessao, int tipoAcrescimo, decimal acrescimo, decimal totalPedidoOrcamentoProjeto, IProdutoDescontoAcrescimo[] produtos, bool acrescimoAmbiente,
            int numeroVez, int? idPedido, int? idProjeto, int? idOrcamento)
        {
            // Além de verificar se será aplicado acréscimo, verifica também se o valor no qual será aplicado o acréscimo
            // (totalPedidoOrcamentoProjeto) é maior que 0, para não ocorrer divisão por 0
            if (acrescimo == 0 || totalPedidoOrcamentoProjeto == 0)
                return false;

            decimal valorAcrescimo = acrescimo;

            if (tipoAcrescimo == 1)
                valorAcrescimo = totalPedidoOrcamentoProjeto * (acrescimo / 100);

            decimal valorAplicado = 0, valor;
            decimal percAcrescimo = valorAcrescimo / totalPedidoOrcamentoProjeto;
            valorAcrescimo = Math.Round(valorAcrescimo, 2);

            GenericBenefCollection benef;

            foreach (IProdutoDescontoAcrescimo prod in produtos)
            {
                prod.RemoverDescontoQtde = true;

                // Calcula o valor bruto do produto, se necessário
                if (prod.TotalBruto == 0 && (prod.IdProduto == 0 || prod.Total > 0))
                    CalculaValorBruto(sessao, prod);

                // Calcula o acréscimo para os beneficiamentos
                benef = prod.Beneficiamentos;
                foreach (GenericBenef b in benef)
                {
                    valor = Math.Round(percAcrescimo * b.TotalBruto, 2);
                    valorAplicado += valor;

                    if (!acrescimoAmbiente)
                        b.ValorAcrescimo += valor;
                    else
                        b.ValorAcrescimoProd += valor;

                    b.Valor += valor;
                }

                prod.Beneficiamentos = benef;

                if (numeroVez < NUMERO_VEZES_RATEAR || prod.Id != produtos[produtos.Length - 1].Id)
                {
                    // Calcula o acréscimo para o produto
                    valor = Math.Round(percAcrescimo * GetTotalBrutoCalc(prod), 2);
                    valorAplicado += valor;

                    if (!acrescimoAmbiente)
                        prod.ValorAcrescimo += valor;
                    else
                        prod.ValorAcrescimoProd += valor;

                    prod.Total += valor;
                }
                else
                {
                    // Aplica o restante do acréscimo ao produto
                    if (!acrescimoAmbiente)
                        prod.ValorAcrescimo += valorAcrescimo - valorAplicado;
                    else
                        prod.ValorAcrescimoProd += valorAcrescimo - valorAplicado;

                    prod.Total += valorAcrescimo - valorAplicado;
                }
            }

            if (numeroVez < NUMERO_VEZES_RATEAR && valorAcrescimo - valorAplicado != 0)
                AplicaAcrescimo(sessao, 2, (valorAcrescimo - valorAplicado), totalPedidoOrcamentoProjeto, produtos, acrescimoAmbiente, numeroVez + 1, idPedido, idProjeto, idOrcamento);
            else
            {
                foreach (IProdutoDescontoAcrescimo prod in produtos)
                {
                    if (prod.IdProduto > 0)
                        RecalcularValorUnit(sessao, prod, idPedido, idProjeto, idOrcamento);
                    else
                        prod.ValorUnit = prod.Total / (decimal)(prod.Qtde > 0 ? prod.Qtde : 1);
                }
            }

            return true;
        }
        
        #endregion

        #region Remove acréscimo no valor dos produtos

        /// <summary>
        /// Remove acréscimo no valor dos produtos.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="tipoAcrescimo"></param>
        /// <param name="acrescimo"></param>
        /// <param name="produtos"></param>
        public bool RemoveAcrescimo(GDASession sessao, int tipoAcrescimo, decimal acrescimo, IProdutoDescontoAcrescimo[] produtos, int? idPedido, int? idProjeto, int? idOrcamento)
        {
            return RemoveAcrescimo(sessao, tipoAcrescimo, acrescimo, produtos, false, idPedido, idProjeto, idOrcamento);
        }

        /// <summary>
        /// Remove acréscimo do ambiente no valor dos produtos.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="tipoAcrescimo"></param>
        /// <param name="acrescimo"></param>
        /// <param name="produtos"></param>
        public bool RemoveAcrescimoAmbiente(GDASession sessao, int tipoAcrescimo, decimal acrescimo, IProdutoDescontoAcrescimo[] produtos, int? idPedido, int? idProjeto, int? idOrcamento)
        {
            return RemoveAcrescimo(sessao, tipoAcrescimo, acrescimo, produtos, true, idPedido, idProjeto, idOrcamento);
        }

        /// <summary>
        /// Remove acréscimo no valor dos produtos.
        /// </summary>
        /// <param name="acrescimo"></param>
        private bool RemoveAcrescimo(GDASession sessao, int tipoAcrescimo, decimal acrescimo, IProdutoDescontoAcrescimo[] produtos, bool acrescimoAmbiente, int? idPedido, int? idProjeto, int? idOrcamento)
        {
            if (acrescimo == 0)
                return false;

            bool alterou = false;
            GenericBenefCollection benef;

            foreach (IProdutoDescontoAcrescimo prod in produtos)
            {
                prod.RemoverDescontoQtde = true;

                // Remove o acréscimo dos beneficiamentos
                benef = prod.Beneficiamentos;
                foreach (GenericBenef b in benef)
                    if (acrescimoAmbiente && b.ValorAcrescimoProd != 0)
                    {
                        alterou = true;
                        b.Valor -= b.ValorAcrescimoProd;
                        b.ValorAcrescimoProd = 0;
                    }
                    else if (!acrescimoAmbiente && b.ValorAcrescimo != 0)
                    {
                        alterou = true;
                        b.Valor -= b.ValorAcrescimo;
                        b.ValorAcrescimo = 0;
                    }

                prod.Beneficiamentos = benef;

                // Remove o acréscimo do produto
                if (!acrescimoAmbiente && prod.ValorAcrescimo != 0)
                {
                    alterou = true;
                    prod.Total -= prod.ValorAcrescimo;
                    prod.ValorAcrescimo = 0;
                }
                else if (acrescimoAmbiente && prod.ValorAcrescimoProd != 0)
                {
                    alterou = true;
                    prod.Total -= prod.ValorAcrescimoProd;
                    prod.ValorAcrescimoProd = 0;
                }

                // Recalcula o valor unitário
                if (prod.IdProduto > 0)
                {
                    // Chama este método passando o parâmetro valorBruto como false, para que não seja buscado novamente o valor de tabela do produto,
                    // forçando a utilizar o que já está cadastrado no pedido
                    TempDAO.DadosDiferenca dados = TempDAO.Instance.GetDadosParaDiferenca(sessao, prod);
                    RecalcularValorUnit(sessao, prod, dados.IdCliente, dados.TipoEntrega, true, false, idPedido, idProjeto, idOrcamento);
                }
                else
                    prod.ValorUnit = prod.Total / (decimal)(prod.Qtde > 0 ? prod.Qtde : 1);
            }

            return alterou;
        }

        #endregion

        #region Aplica desconto no valor dos produtos

        /// <summary>
        /// Aplica desconto no valor dos produtos.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="tipoDesconto"></param>
        /// <param name="desconto"></param>
        /// <param name="produtos"></param>
        public bool AplicaDesconto(GDASession sessao, int tipoDesconto, decimal desconto, IProdutoDescontoAcrescimo[] produtos, int? idPedido, int? idProjeto, int? idOrcamento)
        {
            return AplicaDesconto(sessao, tipoDesconto, desconto, GetBaseCalculo(sessao, produtos), produtos, false, false, 1, idPedido, idProjeto, idOrcamento);
        }

        /// <summary>
        /// Aplica desconto do ambiente no valor dos produtos.
        /// </summary>
        /// <param name="tipoDesconto"></param>
        /// <param name="desconto"></param>
        /// <param name="produtos"></param>
        public bool AplicaDescontoAmbiente(int tipoDesconto, decimal desconto, IProdutoDescontoAcrescimo[] produtos, int? idPedido, int? idProjeto, int? idOrcamento)
        {
            return AplicaDescontoAmbiente(null, tipoDesconto, desconto, produtos, idPedido, idProjeto, idOrcamento);
        }

        /// <summary>
        /// Aplica desconto do ambiente no valor dos produtos.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="tipoDesconto"></param>
        /// <param name="desconto"></param>
        /// <param name="produtos"></param>
        public bool AplicaDescontoAmbiente(GDASession sessao, int tipoDesconto, decimal desconto, IProdutoDescontoAcrescimo[] produtos, int? idPedido, int? idProjeto, int? idOrcamento)
        {
            return AplicaDesconto(sessao, tipoDesconto, desconto, GetBaseCalculo(sessao, produtos), produtos, true, false, 1, idPedido, idProjeto, idOrcamento);
        }

        /// <summary>
        /// Aplica desconto por quantidade no valor dos produtos.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="Desconto produto"></param>
        public bool AplicaDescontoQtde(GDASession sessao, IProdutoDescontoAcrescimo produto, int? idPedido, int? idProjeto, int? idOrcamento)
        {
            IProdutoDescontoAcrescimo[] temp = new IProdutoDescontoAcrescimo[] { produto };
            bool retorno = AplicaDesconto(sessao, 0, 0, 0, temp, false, true, 1, idPedido, idProjeto, idOrcamento);
            produto = temp[0];
            return retorno;
        }

        /// <summary>
        /// Aplica desconto no valor dos produtos.
        /// </summary>
        /// <param name="desconto"></param>
        private bool AplicaDesconto(GDASession sessao, int tipoDesconto, decimal desconto, decimal totalPedidoOrcamentoProjeto, IProdutoDescontoAcrescimo[] produtos, bool descontoAmbiente,
            bool descontoQtde, int numeroVez, int? idPedido, int? idProjeto, int? idOrcamento)
        {
            if ((!descontoQtde && (desconto == 0 || !PedidoConfig.RatearDescontoProdutos)) || (descontoQtde && produtos[0].PercDescontoQtde == 0))
                return false;

            decimal valorDesconto = desconto;

            if (tipoDesconto == 1)
                valorDesconto = totalPedidoOrcamentoProjeto * (desconto / 100);

            decimal valorAplicado = 0, valor = 0;
            decimal percDesconto = !descontoQtde && totalPedidoOrcamentoProjeto > 0 ? valorDesconto / totalPedidoOrcamentoProjeto : 0;
            valorDesconto = Math.Round(valorDesconto, 2);

            GenericBenefCollection benef;

            foreach (IProdutoDescontoAcrescimo prod in produtos)
            {
                prod.RemoverDescontoQtde = prod.RemoverDescontoQtde || !descontoQtde;

                // Calcula o valor bruto do produto, se necessário
                if (prod.TotalBruto == 0 && (prod.IdProduto == 0 || prod.Total > 0))
                    CalculaValorBruto(sessao, prod);

                if (!descontoQtde)
                {
                    // Calcula o desconto para os beneficiamentos
                    benef = prod.Beneficiamentos;
                    foreach (GenericBenef b in benef)
                    {
                        valor = Math.Round(percDesconto * b.TotalBruto, 2);
                        valorAplicado += valor;

                        if (!descontoAmbiente)
                            b.ValorDesconto += valor;
                        else
                            b.ValorDescontoProd += valor;

                        b.Valor -= valor;
                    }

                    prod.Beneficiamentos = benef;
                }

                if (numeroVez < NUMERO_VEZES_RATEAR || descontoQtde || prod.Id != produtos[produtos.Length - 1].Id)
                {
                    if (!descontoQtde)
                    {
                        // Calcula o desconto para o produto
                        valor = Math.Round(percDesconto * GetTotalBrutoCalc(prod), 2);
                        valorAplicado += valor;

                        if (!descontoAmbiente)
                            prod.ValorDesconto += valor;
                        else
                            prod.ValorDescontoProd += valor;
                    }
                    else
                    {
                        // Calcula o desconto de Qtde para o produto
                        valor = Math.Round((prod.RemoverDescontoQtde ? GetTotalBrutoCalc(prod) : prod.Total) * (decimal)(prod.PercDescontoQtde / 100), 2);
                        prod.ValorDescontoQtde = valor;
                    }

                    prod.Total -= valor;
                }
                else
                {
                    // Aplica o restante do desconto ao produto
                    if (!descontoAmbiente)
                        prod.ValorDesconto += valorDesconto - valorAplicado;
                    else
                        prod.ValorDescontoProd += valorDesconto - valorAplicado;

                    prod.Total -= valorDesconto - valorAplicado;
                }
            }

            if (numeroVez < NUMERO_VEZES_RATEAR && valorDesconto - valorAplicado != 0 && !descontoQtde)
                AplicaDesconto(sessao, 2, (valorDesconto - valorAplicado),
                    totalPedidoOrcamentoProjeto, produtos, descontoAmbiente, descontoQtde, numeroVez + 1, idPedido, idProjeto, idOrcamento);
            else
            {
                foreach (IProdutoDescontoAcrescimo prod in produtos)
                {
                    if (prod.IdProduto > 0)
                        RecalcularValorUnit(sessao, prod, idPedido, idProjeto, idOrcamento);
                    else
                        prod.ValorUnit = prod.Total / (decimal)(prod.Qtde > 0 ? prod.Qtde : 1);
                }
            }

            return true;
        }

        #endregion

        #region Remove desconto no valor dos produtos

        /// <summary>
        /// Remove desconto no valor dos produtos.
        /// </summary>
        /// <param name="desconto"></param>
        public bool RemoveDesconto(GDASession sessao, int tipoDesconto, decimal desconto, IProdutoDescontoAcrescimo[] produtos, int? idPedido, int? idProjeto, int? idOrcamento)
        {
            return RemoveDesconto(sessao, tipoDesconto, desconto, produtos, false, false, idPedido, idProjeto, idOrcamento);
        }

        /// <summary>
        /// Remove desconto do ambiente no valor dos produtos.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="tipoDesconto"></param>
        /// <param name="desconto"></param>
        /// <param name="produtos"></param>
        public bool RemoveDescontoAmbiente(GDASession sessao, int tipoDesconto, decimal desconto, IProdutoDescontoAcrescimo[] produtos, int? idPedido, int? idProjeto, int? idOrcamento)
        {
            return RemoveDesconto(sessao, tipoDesconto, desconto, produtos, true, false, idPedido, idProjeto, idOrcamento);
        }

        /// <summary>
        /// Remove desconto por quantidade no valor dos produtos.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="desconto"></param>
        public bool RemoveDescontoQtde(GDASession sessao, IProdutoDescontoAcrescimo produto, int? idPedido, int? idProjeto, int? idOrcamento)
        {
            if (!produto.RemoverDescontoQtde)
                return true;

            IProdutoDescontoAcrescimo[] temp = new IProdutoDescontoAcrescimo[] { produto };
            bool retorno = RemoveDesconto(sessao, 0, 0, temp, false, true, idPedido, idProjeto, idOrcamento);
            produto = temp[0];
            return retorno;
        }

        /// <summary>
        /// Remove desconto no valor dos produtos.
        /// </summary>
        /// <param name="desconto"></param>
        private bool RemoveDesconto(GDASession sessao, int tipoDesconto, decimal desconto, IProdutoDescontoAcrescimo[] produtos, bool descontoAmbiente, bool descontoQtde, int? idPedido, int? idProjeto, int? idOrcamento)
        {
            if ((!descontoQtde && (desconto == 0 || !PedidoConfig.RatearDescontoProdutos)) || (descontoQtde && produtos[0].PercDescontoQtde == 0))
                return false;

            bool alterou = false;
            GenericBenefCollection benef;

            foreach (IProdutoDescontoAcrescimo prod in produtos)
            {
                prod.RemoverDescontoQtde = prod.RemoverDescontoQtde || !descontoQtde;

                if (!descontoQtde)
                {
                    // Remove o desconto dos beneficiamentos
                    benef = prod.Beneficiamentos;
                    foreach (GenericBenef b in benef)
                        if (descontoAmbiente && b.ValorDescontoProd != 0)
                        {
                            alterou = true;
                            b.Valor += b.ValorDescontoProd;
                            b.ValorDescontoProd = 0;
                        }
                        else if (!descontoAmbiente && b.ValorDesconto != 0)
                        {
                            alterou = true;
                            b.Valor += b.ValorDesconto;
                            b.ValorDesconto = 0;
                        }

                    prod.Beneficiamentos = benef;

                    // Remove o desconto do produto
                    if (descontoAmbiente && prod.ValorDescontoProd != 0)
                    {
                        alterou = true;
                        prod.Total += prod.ValorDescontoProd;
                        prod.ValorDescontoProd = 0;
                    }
                    else if (!descontoAmbiente && prod.ValorDesconto != 0)
                    {
                        alterou = true;
                        prod.Total += prod.ValorDesconto;
                        prod.ValorDesconto = 0;
                    }
                }

                // Remove o desconto de Qtde do produto
                else if (prod.ValorDescontoQtde != 0)
                {
                    alterou = true;
                    prod.Total += prod.ValorDescontoQtde;
                    prod.ValorDescontoQtde = 0;
                }

                // Recalcula o valor unitário do produto
                if (prod.IdProduto > 0)
                    RecalcularValorUnit(sessao, prod, idPedido, idProjeto, idOrcamento);
                else
                    prod.ValorUnit = prod.Total / (decimal)(prod.Qtde > 0 ? prod.Qtde : 1);
            }

            return alterou;
        }

        #endregion

        #region Aplica comissão no valor dos produtos

        /// <summary>
        /// Aplica comissão no valor dos produtos.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="percComissao"></param>
        /// <param name="produtos"></param>
        /// <returns></returns>
        public bool AplicaComissao(GDASession sessao, float percComissao, IProdutoDescontoAcrescimo[] produtos, int? idPedido, int? idProjeto, int? idOrcamento)
        {
            return AplicaComissao(sessao, percComissao, GetBaseCalculo(sessao, produtos), produtos, idPedido, idProjeto, idOrcamento);
        }

        /// <summary>
        /// Aplica comissão no valor dos produtos.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="percComissao"></param>
        /// <param name="totalPedidoOrcamentoProjeto"></param>
        /// <param name="produtos"></param>
        private bool AplicaComissao(GDASession sessao, float percComissao, decimal totalPedidoOrcamentoProjeto, IProdutoDescontoAcrescimo[] produtos, int? idPedido, int? idProjeto, int? idOrcamento)
        {
            // Além de verificar se será aplicado comissão, verifica também se o valor no qual será aplicado o acréscimo
            // (totalPedidoOrcamentoProjeto) é maior que 0, para não ocorrer divisão por 0
            if (!PedidoConfig.Comissao.ComissaoPedido || percComissao == 0 || totalPedidoOrcamentoProjeto == 0)
                return false;

            decimal divisor = (100 - (decimal)percComissao) / 100;

            decimal valor;
            GenericBenefCollection benef;

            foreach (IProdutoDescontoAcrescimo prod in produtos)
            {
                prod.RemoverDescontoQtde = true;

                // Calcula o valor bruto do produto, se necessário
                if (prod.TotalBruto == 0 && (prod.IdProduto == 0 || prod.Total > 0))
                    CalculaValorBruto(sessao, prod);

                // Recupera o valor do produto com desconto/acréscimo por cliente
                valor = GetTotalBrutoCalc(prod) + prod.ValorAcrescimo;
                valor = Math.Round(valor / divisor, 2) - valor;

                prod.ValorComissao += valor;
                prod.Total += valor;

                benef = prod.Beneficiamentos;
                foreach (GenericBenef b in benef)
                {
                    valor = b.TotalBruto;
                    valor = Math.Round(valor / divisor, 2) - valor;

                    b.ValorComissao += valor;
                    b.Valor += valor;
                }

                prod.Beneficiamentos = benef;
            }

            foreach (IProdutoDescontoAcrescimo prod in produtos)
            {
                if (prod.IdProduto > 0)
                    RecalcularValorUnit(sessao, prod, idPedido, idProjeto, idOrcamento);
                else
                    prod.ValorUnit = prod.Total / (decimal)(prod.Qtde > 0 ? prod.Qtde : 1);
            }

            return true;
        }

        #endregion

        #region Remove comissão no valor dos produtos

        /// <summary>
        /// Remove comissão no valor dos produtos.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="percComissao"></param>
        /// <param name="produtos"></param>
        public bool RemoveComissao(GDASession sessao, float percComissao, IProdutoDescontoAcrescimo[] produtos, int? idPedido, int? idProjeto, int? idOrcamento)
        {
            if (!PedidoConfig.Comissao.ComissaoPedido || percComissao == 0)
                return false;

            bool alterou = false;
            GenericBenefCollection benef;

            foreach (IProdutoDescontoAcrescimo prod in produtos)
            {
                prod.RemoverDescontoQtde = true;

                if (prod.ValorComissao > 0)
                {
                    alterou = true;
                    prod.Total -= prod.ValorComissao;
                    prod.ValorComissao = 0;
                }

                benef = prod.Beneficiamentos;
                foreach (GenericBenef b in benef)
                    if (b.ValorComissao != 0)
                    {
                        alterou = true;
                        b.Valor -= b.ValorComissao;
                        b.ValorComissao = 0;
                    }

                prod.Beneficiamentos = benef;

                if (prod.IdProduto > 0)
                    RecalcularValorUnit(sessao, prod, idPedido, idProjeto, idOrcamento);
                else
                    prod.ValorUnit = prod.Total / (decimal)(prod.Qtde > 0 ? prod.Qtde : 1);
            }

            return alterou;
        }

        #endregion

        #region Calcula o desconto/acréscimo do cliente

        private struct DadosDiferencaCliente
        {
            public int TipoCalculoProd;
            public decimal DescontoTotal, AcrescimoTotal;
        }

        private DadosDiferencaCliente GetTotalDiferencaCliente(GDASession sessao, IProdutoDescontoAcrescimo produto)
        {
            DadosDiferencaCliente retorno = new DadosDiferencaCliente();

            retorno.TipoCalculoProd = Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo(sessao, (int)produto.IdProduto);
            retorno.DescontoTotal = CalculosFluxo.CalcTotaisItemProdFast(sessao, retorno.TipoCalculoProd, 
                produto.AlturaCalc, produto.Largura, produto.Qtde, produto.TotM2Calc, produto.ValorDescontoCliente);
            retorno.AcrescimoTotal = CalculosFluxo.CalcTotaisItemProdFast(sessao, retorno.TipoCalculoProd, produto.AlturaCalc,
                produto.Largura, produto.Qtde, produto.TotM2Calc, produto.ValorAcrescimoCliente);

            return retorno;
        }

        private decimal GetTotalBrutoCalc(IProdutoDescontoAcrescimo produto)
        {
            return produto.TotalBruto - produto.ValorDescontoCliente + produto.ValorAcrescimoCliente;
        }

        /// <summary>
        /// Calcula a diferença entre o valor de tabela e o valor para o cliente de um produto.
        /// </summary>
        /// <param name="Desconto produto"></param>
        /// <returns></returns>
        public void DiferencaCliente(IProdutoDescontoAcrescimo produto, int? idPedido, int? idProjeto, int? idOrcamento)
        {
            DiferencaCliente(null, produto, idPedido, idProjeto, idOrcamento);
        }

        /// <summary>
        /// Calcula a diferença entre o valor de tabela e o valor para o cliente de um produto.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="Desconto produto"></param>
        /// <returns></returns>
        public void DiferencaCliente(GDASession sessao, IProdutoDescontoAcrescimo produto, int? idPedido, int? idProjeto, int? idOrcamento)
        {
            TempDAO.DadosDiferenca dadosCliente = TempDAO.Instance.GetDadosParaDiferenca(sessao, produto);
            if (dadosCliente.IdCliente.GetValueOrDefault() == 0)
                return;

            DiferencaCliente(sessao, produto, dadosCliente.IdCliente, dadosCliente.TipoEntrega, dadosCliente.IsReposicao, idPedido, idProjeto, idOrcamento);
        }

        /// <summary>
        /// Calcula a diferença entre o valor de tabela e o valor para o cliente de um produto.
        /// </summary>
        /// <param name="idProd"></param>
        /// <param name="idCliente"></param>
        /// <param name="tipoEntrega"></param>
        /// <returns></returns>
        public void DiferencaCliente(GDASession sessao, IProdutoDescontoAcrescimo produto, uint? idCliente, int? tipoEntrega, bool reposicao, int? idPedido, int? idProjeto, int? idOrcamento)
        {
            bool revenda = ClienteDAO.Instance.IsRevenda(sessao, idCliente);

            // Recupera os valores do produto: o valor de tabela e o valor para o cliente
            decimal valorTabela = ProdutoDAO.Instance.GetValorTabela(sessao, (int)produto.IdProduto, tipoEntrega, null, revenda,
                reposicao, produto.PercDescontoQtde, idPedido, idProjeto, idOrcamento);

            decimal valorCliente = ProdutoDAO.Instance.GetValorTabela(sessao, (int)produto.IdProduto, tipoEntrega, idCliente, revenda,
                reposicao, produto.PercDescontoQtde, idPedido, idProjeto, idOrcamento);

            // Compara os valores
            if (valorTabela < valorCliente)
            {
                produto.ValorAcrescimoCliente = valorCliente - valorTabela;
                produto.ValorDescontoCliente = 0;
            }
            else if (valorTabela > valorCliente)
            {
                produto.ValorAcrescimoCliente = 0;
                produto.ValorDescontoCliente = valorTabela - valorCliente;
            }
            else
            {
                produto.ValorAcrescimoCliente = 0;
                produto.ValorDescontoCliente = 0;
            }

            // Salva a diferença total entre os produtos (desconto e acréscimo)
            DadosDiferencaCliente dados = GetTotalDiferencaCliente(sessao, produto);
            produto.ValorAcrescimoCliente = dados.AcrescimoTotal;
            produto.ValorDescontoCliente = dados.DescontoTotal;
        }

        #endregion

        #region Calcula o valor bruto usado como base de cálculo para desconto/acréscimo

        /// <summary>
        /// Calcula o valor bruto usado como base de cálculo para desconto/acréscimo.
        /// </summary>
        private decimal GetBaseCalculo(IProdutoDescontoAcrescimo[] produtos)
        {
            return GetBaseCalculo(null, produtos);
        }

        /// <summary>
        /// Calcula o valor bruto usado como base de cálculo para desconto/acréscimo.
        /// </summary>
        private decimal GetBaseCalculo(GDASession session, IProdutoDescontoAcrescimo[] produtos)
        {
            decimal valorCalc = 0;
            Array.ForEach(produtos, x =>
            {
                // Calcula o valor bruto do produto, se necessário
                if (x.TotalBruto == 0 && (x.IdProduto == 0 || x.Total > 0))
                    CalculaValorBruto(session, x);

                decimal valorBenefBruto = 0;
                Array.ForEach(x.Beneficiamentos.ToArray(), y => valorBenefBruto += y.TotalBruto);

                valorCalc += GetTotalBrutoCalc(x) + valorBenefBruto;
            });

            return valorCalc;
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