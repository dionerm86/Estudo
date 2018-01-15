using System;
using System.Collections.Generic;
using Glass.Data.DAL;
using Glass.Configuracoes;
using GDA;
using Glass.Estoque.Negocios.Entidades;

namespace WebGlass.Business.Pedido.Fluxo
{
    public sealed class AlterarEstoque : BaseFluxo<AlterarEstoque>, Glass.Estoque.Negocios.IProvedorBaixaEstoque
    {
        static volatile object _baixarEstoqueLock = new object();

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// </summary>
        public void BaixarEstoqueComTransacao(uint idLoja, IEnumerable<DetalhesBaixaEstoque> dadosBaixarEstoque,
            uint? idVolume, uint? idProdImpressaoChapa, bool manual, string observacao = null)
        {
            lock(_baixarEstoqueLock)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        BaixarEstoque(transaction, idLoja, dadosBaixarEstoque, idVolume, idProdImpressaoChapa, manual, observacao);

                        transaction.Commit();
                        transaction.Close();
                    }
                    catch
                    {
                        transaction.Rollback();
                        transaction.Close();
                        throw;
                    }
                }
            }
        }

        public void BaixarEstoque(GDASession sessao, uint idLoja, IEnumerable<DetalhesBaixaEstoque> dadosBaixarEstoque, uint? idVolume, uint? idProdImpressaoChapa, bool manual, string observacao = null)
        {
            var lstProdPed = new List<Glass.Data.Model.ProdutosPedido>();
            Glass.Data.Model.Produto prod;

            foreach (var d in dadosBaixarEstoque)
            {
                var prodPed = ProdutosPedidoDAO.Instance.GetElement(sessao, (uint)d.IdProdPed);
                prod = ProdutoDAO.Instance.GetElement(sessao, prodPed.IdProd);

                // Se a quantidade a ser marcada como saída for maior do que o máximo, não permite marcar saída
                //Chamado 55856
                if ((decimal)d.Qtde > (decimal)prodPed.Qtde - (decimal)prodPed.QtdSaida)
                    throw new Exception("Operação cancelada. O produto " + d.DescricaoBaixa + " teve uma saída maior do que sua quantidade.");
                else
                {
                    int tipoCalc = Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo(sessao, prod.IdGrupoProd, prod.IdSubgrupoProd);

                    // Se a empresa trabalha com venda de alumínio no metro e se produto for alumínio, 
                    // coloca o metro linear baixado no campo m²
                    if (prodPed.IdProd > 0 && (tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL05 ||
                        tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL1 || tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL6 ||
                        tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.ML))
                    {
                        prodPed.TotM = d.Qtde * prodPed.Altura;
                    }
                    // Se produto for calculado por m², dá baixa somente no m² da qtde de peças que foram dado baixa
                    else if (tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2 || tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2Direto)
                        prodPed.TotM = Glass.Global.CalculosFluxo.ArredondaM2(sessao, prodPed.Largura, (int)prodPed.Altura, d.Qtde, (int)prodPed.IdProd, prodPed.Redondo);

                    prodPed.QtdMarcadaSaida = d.Qtde;

                    lstProdPed.Add(prodPed);
                }
            }

            // Se as quantidades marcadas para sair estiverem dentro do limite
            if (lstProdPed != null && lstProdPed.Count > 0)
            {
                var idSaidaEstoque = SaidaEstoqueDAO.Instance.GetNewSaidaEstoque(sessao, idLoja, lstProdPed[0].IdPedido, null, idVolume, manual);
                var idsProdQtde = new Dictionary<int, float>();

                // Marca saída dos produtos
                foreach (var p in lstProdPed)
                {
                    if (p.QtdMarcadaSaida == 0)
                        continue;

                    // Marca quantos produtos do pedido foi marcado como saída
                    ProdutosPedidoDAO.Instance.MarcarSaida(sessao, p.IdProdPed, p.QtdMarcadaSaida, idSaidaEstoque);

                    var tipoCalculo = GrupoProdDAO.Instance.TipoCalculo(sessao, (int)p.IdProd);
                    var qtdSaida = p.QtdMarcadaSaida;

                    if (tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL0 || tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL05 ||
                        tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL1 || tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL6 ||
                        tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.ML)
                        qtdSaida *= p.Altura;

                    var m2 = tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2 || tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2Direto;

                    var tipoSubgrupo = SubgrupoProdDAO.Instance.ObtemTipoSubgrupo(sessao, (int)p.IdProd);

                    // Dá baixa no estoque da loja
                    MovEstoqueDAO.Instance.BaixaEstoquePedido(sessao, p.IdProd, idLoja, p.IdPedido, p.IdProdPed,
                        (decimal)(m2 ? (p.TotM / p.Qtde) * qtdSaida : qtdSaida), (decimal)(m2 ? (p.TotM2Calc / p.Qtde) * qtdSaida : 0),
                         tipoSubgrupo != Glass.Data.Model.TipoSubgrupoProd.ChapasVidro && tipoSubgrupo != Glass.Data.Model.TipoSubgrupoProd.ChapasVidroLaminado, observacao, idVolume, idProdImpressaoChapa);

                    // Salva produto e qtd de saída para executar apenas um sql de atualização de estoque
                    if (!PedidoDAO.Instance.IsProducao(sessao, p.IdPedido))
                    {
                        if (!idsProdQtde.ContainsKey((int)p.IdProd))
                            idsProdQtde.Add((int)p.IdProd, m2 ? p.TotM : qtdSaida);
                        else
                            idsProdQtde[(int)p.IdProd] += m2 ? p.TotM : qtdSaida;
                    }
                }

                // Se o pedido não possuir nenhum vidro e possuir apenas peças de estoque, marca o mesmo como entregue,
                // a menos que a empresa não possua controle de produção

                uint idPedido = lstProdPed[0].IdPedido;

                PedidoDAO.Instance.MarcaPedidoEntregue(sessao, idPedido);

                if (idsProdQtde.Count > 0)
                {
                    var idLojaReserva = PedidoDAO.Instance.ObtemIdLoja(sessao, idPedido);
                    var movimentarReserva = !PedidoConfig.LiberarPedido;

                    if (!movimentarReserva && idVolume > 0)
                    {
                        var dataPrimeiraExpedicaoVolumePedido = VolumeDAO.Instance.ObterDataPrimeiraExpedicaoVolumePedido(sessao, idPedido);
                        var dataPrimeiraLiberacaoPedido = LiberarPedidoDAO.Instance.ObterDataPrimeiraLiberacaoPedido(sessao, idPedido);

                        /* Chamado 54238.
                         * A leitura do volume pode ser feita antes da liberação do pedido. Nesse caso, deve-se movimentar a reserva ao invés da liberação. */
                        movimentarReserva = !dataPrimeiraLiberacaoPedido.HasValue || (dataPrimeiraExpedicaoVolumePedido.HasValue && dataPrimeiraExpedicaoVolumePedido < dataPrimeiraLiberacaoPedido);
                    }

                    if (movimentarReserva)
                        ProdutoLojaDAO.Instance.TirarReserva(sessao, (int)idLojaReserva, idsProdQtde, null, null, null, null, (int)idPedido, null, null, "AlterarEstoque - BaixarEstoque");
                    else
                        ProdutoLojaDAO.Instance.TirarLiberacao(sessao, (int)idLojaReserva, idsProdQtde, null, null, null, null, (int)idPedido, null, null, "AlterarEstoque - BaixarEstoque");
                }
            }
        }

        public void EstornaBaixaEstoque(GDASession sessao, uint idLoja, IEnumerable<DetalhesBaixaEstoque> dadosEstornoEstoque, uint? idVolume, uint? idProdImpressaoChapa)
        {
            var lstProdPed = new List<Glass.Data.Model.ProdutosPedido>();
            Glass.Data.Model.Produto prod;

            foreach (var d in dadosEstornoEstoque)
            {
                var prodPed = ProdutosPedidoDAO.Instance.GetElement(sessao, (uint)d.IdProdPed);
                prod = ProdutoDAO.Instance.GetElement(sessao, prodPed.IdProd);

                // Se a quantidade a ser marcado estornado for maior do que já saiu, não permite marcar o estorno
                if (d.Qtde > prodPed.QtdSaida)
                    throw new Exception("Operação cancelada. O produto " + d.DescricaoBaixa + " está tendo um estono maior do que a quantidade que já joi dado saída.");
                else
                {
                    int tipoCalc = Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo(sessao, prod.IdGrupoProd, prod.IdSubgrupoProd);

                    // Se a empresa trabalha com venda de alumínio no metro e se produto for alumínio, 
                    // coloca o metro linear baixado no campo m²
                    if (prodPed.IdProd > 0 && (tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL05 ||
                        tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL1 || tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL6 ||
                        tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.ML))
                    {
                        prodPed.TotM = d.Qtde * prodPed.Altura;
                    }
                    // Se produto for calculado por m², dá baixa somente no m² da qtde de peças que foram dado baixa
                    else if (tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2 || tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2Direto)
                        prodPed.TotM = Glass.Global.CalculosFluxo.ArredondaM2(prodPed.Largura, (int)prodPed.Altura, d.Qtde, (int)prodPed.IdProd, prodPed.Redondo);

                    prodPed.QtdMarcadaSaida = d.Qtde;

                    lstProdPed.Add(prodPed);
                }
            }

            // Se as quantidades marcadas para estorno estiverem dentro do limite
            if (lstProdPed != null && lstProdPed.Count > 0)
            {
                SaidaEstoqueDAO.Instance.MarcaEstorno(sessao, lstProdPed[0].IdPedido, null, idVolume);

                var idsProdQtde = new Dictionary<int, float>();

                // Marca saída dos produtos
                foreach (var p in lstProdPed)
                {
                    if (p.QtdMarcadaSaida == 0)
                        continue;

                    // Marca quantos produtos do pedido foram estornados
                    ProdutosPedidoDAO.Instance.EstonoSaida(sessao, p.IdProdPed, p.QtdMarcadaSaida);

                    var tipoCalculo = GrupoProdDAO.Instance.TipoCalculo(sessao, (int)p.IdProd);
                    var qtdMov = p.QtdMarcadaSaida;

                    if (tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL0 || tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL05 ||
                        tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL1 || tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL6 ||
                        tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.ML)
                        qtdMov *= p.Altura;

                    var m2 = tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2 || tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2Direto;

                    // Dá baixa no estoque da loja
                    MovEstoqueDAO.Instance.CreditaEstoquePedido(sessao, p.IdProd, idLoja, p.IdPedido, p.IdProdPed,
                        (decimal)(m2 ? (p.TotM / p.Qtde) * qtdMov : qtdMov), true, idVolume, idProdImpressaoChapa);

                    // Salva produto e qtd de saída para executar apenas um sql de atualização de estoque
                    if (!PedidoDAO.Instance.IsProducao(sessao, p.IdPedido))
                    {
                        if (!idsProdQtde.ContainsKey((int)p.IdProd))
                            idsProdQtde.Add((int)p.IdProd, m2 ? p.TotM : qtdMov);
                        else
                            idsProdQtde[(int)p.IdProd] += m2 ? p.TotM : qtdMov;
                    }
                }

                // Se o pedido não possuir nenhum vidro e possuir apenas peças de estoque, marca o mesmo como entregue,
                // a menos que a empresa não possua controle de produção

                var idPedido = lstProdPed[0].IdPedido;

                //Atualiza a situação da produção do pedido
                PedidoDAO.Instance.AtualizaSituacaoProducao(sessao, idPedido, null, DateTime.Now);

                if (idsProdQtde.Count > 0)
                {
                    var movimentarReserva = !PedidoConfig.LiberarPedido;

                    if (!movimentarReserva && idVolume > 0)
                    {
                        var dataPrimeiraExpedicaoVolumePedido = VolumeDAO.Instance.ObterDataPrimeiraExpedicaoVolumePedido(sessao, idPedido);
                        var dataPrimeiraLiberacaoPedido = LiberarPedidoDAO.Instance.ObterDataPrimeiraLiberacaoPedido(sessao, idPedido);

                        /* Chamado 54238.
                         * A leitura do volume pode ser feita antes da liberação do pedido. Nesse caso, deve-se movimentar a reserva ao invés da liberação. */
                        movimentarReserva = !dataPrimeiraLiberacaoPedido.HasValue || (dataPrimeiraExpedicaoVolumePedido.HasValue && dataPrimeiraExpedicaoVolumePedido < dataPrimeiraLiberacaoPedido);
                    }

                    if (movimentarReserva)
                        ProdutoLojaDAO.Instance.ColocarReserva(sessao, (int)idLoja, idsProdQtde, null, null, null, null, (int)idPedido, null, null, "AlterarEstoque - EstornaBaixaEstoque");
                    else
                        ProdutoLojaDAO.Instance.ColocarLiberacao(sessao, (int)idLoja, idsProdQtde, null, null, null, null, (int)idPedido, null, null, "AlterarEstoque - EstornaBaixaEstoque");
                }
            }
        }

        public void CreditaEstoque(GDASession sessao, uint idLoja, IEnumerable<DetalhesBaixaEstoque> dadosEstoque)
        {
            var lstProdPed = new List<Glass.Data.Model.ProdutosPedido>();
            Glass.Data.Model.Produto prod;

            foreach (var d in dadosEstoque)
            {
                var prodPed = ProdutosPedidoDAO.Instance.GetElement(sessao, (uint)d.IdProdPed);
                prod = ProdutoDAO.Instance.GetElement(sessao, prodPed.IdProd);
                int tipoCalc = Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo(sessao, prod.IdGrupoProd, prod.IdSubgrupoProd);

                // Se a empresa trabalha com venda de alumínio no metro e se produto for alumínio, 
                // coloca o metro linear baixado no campo m²
                if (prodPed.IdProd > 0 && (tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL05 ||
                    tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL1 || tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL6 ||
                    tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.ML))
                {
                    prodPed.TotM = d.Qtde * prodPed.Altura;
                }
                // Se produto for calculado por m², dá baixa somente no m² da qtde de peças que foram dado baixa
                else if (tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2 || tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2Direto)
                    prodPed.TotM = Glass.Global.CalculosFluxo.ArredondaM2(sessao, prodPed.Largura, (int)prodPed.Altura, d.Qtde, (int)prodPed.IdProd, prodPed.Redondo);

                prodPed.QtdMarcadaSaida = d.Qtde;

                lstProdPed.Add(prodPed);

            }

            // Se as quantidades marcadas para estorno estiverem dentro do limite
            if (lstProdPed != null && lstProdPed.Count > 0)
            {
                List<uint> lstIdProd = new List<uint>();
                List<float> lstSaida = new List<float>();

                // Marca entrada dos produtos
                foreach (var p in lstProdPed)
                {
                    if (p.QtdMarcadaSaida == 0)
                        continue;

                    int tipoCalculo = Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo(sessao, (int)p.IdProd);
                    float qtdMov = p.QtdMarcadaSaida;

                    if (tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL0 || tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL05 ||
                        tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL1 || tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL6 ||
                        tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.ML)
                        qtdMov *= p.Altura;

                    bool m2 = tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2 || tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2Direto;

                    // Dá entrada no estoque da loja
                    MovEstoqueDAO.Instance.CreditaEstoquePedido(sessao, p.IdProd, idLoja, p.IdPedido, p.IdProdPed,
                        (decimal)(m2 ? (p.TotM / p.Qtde) * qtdMov : qtdMov), true);
                }
            }
        }

        public void EstornoCreditaEstoque(GDASession sessao, uint idLoja, IEnumerable<DetalhesBaixaEstoque> dadosEstornoEstoque)
        {
            var lstProdPed = new List<Glass.Data.Model.ProdutosPedido>();
            Glass.Data.Model.Produto prod;

            foreach (var d in dadosEstornoEstoque)
            {
                var prodPed = ProdutosPedidoDAO.Instance.GetElement(sessao, (uint)d.IdProdPed);
                prod = ProdutoDAO.Instance.GetElement(sessao, prodPed.IdProd);
                int tipoCalc = Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo(sessao, prod.IdGrupoProd, prod.IdSubgrupoProd);

                // Se a empresa trabalha com venda de alumínio no metro e se produto for alumínio, 
                // coloca o metro linear baixado no campo m²
                if (prodPed.IdProd > 0 && (tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL05 ||
                    tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL1 || tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL6 ||
                    tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.ML))
                {
                    prodPed.TotM = d.Qtde * prodPed.Altura;
                }
                // Se produto for calculado por m², dá baixa somente no m² da qtde de peças que foram dado baixa
                else if (tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2 || tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2Direto)
                    prodPed.TotM = Glass.Global.CalculosFluxo.ArredondaM2(sessao, prodPed.Largura, (int)prodPed.Altura, d.Qtde, (int)prodPed.IdProd, prodPed.Redondo);

                prodPed.QtdMarcadaSaida = d.Qtde;

                lstProdPed.Add(prodPed);
            }

            // Se as quantidades marcadas para estorno estiverem dentro do limite
            if (lstProdPed != null && lstProdPed.Count > 0)
            {
                List<uint> lstIdProd = new List<uint>();
                List<float> lstSaida = new List<float>();

                // Marca entrada dos produtos
                foreach (var p in lstProdPed)
                {
                    if (p.QtdMarcadaSaida == 0)
                        continue;

                    int tipoCalculo = Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo(sessao, (int)p.IdProd);
                    float qtdMov = p.QtdMarcadaSaida;

                    if (tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL0 || tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL05 ||
                        tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL1 || tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL6 ||
                        tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.ML)
                        qtdMov *= p.Altura;

                    bool m2 = tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2 || tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2Direto;

                    // Dá saída no estoque da loja
                    MovEstoqueDAO.Instance.BaixaEstoquePedido(sessao, p.IdProd, idLoja, p.IdPedido, p.IdProdPed,
                        (decimal)(m2 ? (p.TotM / p.Qtde) * qtdMov : qtdMov), 0, true, null);
                }
            }
        }
    }
}
