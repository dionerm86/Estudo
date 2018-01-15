using System;
using Glass.Data.DAL;
using Glass.Configuracoes;

namespace WebGlass.Business.Pedido.Ajax
{
    public interface IPedidoReposicao
    {
        string AddProduto(string idProduto, string pedido, string ambiente, string etiqueta);
    }

    internal class PedidoReposicao : IPedidoReposicao
    {
        public string AddProduto(string idProduto, string pedido, string ambiente, string etiqueta)
        {
            try
            {
                uint idProdPed;
                uint idProdPedAnterior = Glass.Conversoes.StrParaUint(idProduto);
                uint idPedido = Glass.Conversoes.StrParaUint(pedido);

                var produtoNovo = ProdutosPedidoDAO.Instance.GetElementByPrimaryKey(idProdPedAnterior);

                float qtdOriginal = produtoNovo.Qtde;

                if (Glass.Configuracoes.ProducaoConfig.ReporApenasProduzidos && !ProdutoPedidoProducaoDAO.Instance.PodeReporPeca(idProdPedAnterior, etiqueta))
                    throw new Exception("Esta etiqueta não pode ser reposta pois já há uma reposição para a mesma.");

                #region Recupera e "clona" o produto

                if (produtoNovo.IdProdPedEsp > 0)
                {
                    produtoNovo.Largura = ProdutosPedidoEspelhoDAO.Instance.ObtemLarguraProducao(produtoNovo.IdProdPedEsp.Value);
                    produtoNovo.Altura = ProdutosPedidoEspelhoDAO.Instance.ObtemAlturaProducao(produtoNovo.IdProdPedEsp.Value);
                }

                produtoNovo.IdProdPed = 0;
                produtoNovo.IdProdPedEsp = null;
                produtoNovo.IdProdPedAnterior = idProdPedAnterior;
                produtoNovo.IdPedido = idPedido;
                produtoNovo.TotM = Glass.Configuracoes.ProducaoConfig.ReporApenasProduzidos ? produtoNovo.TotM / produtoNovo.Qtde : produtoNovo.TotM;
                produtoNovo.Qtde = Glass.Configuracoes.ProducaoConfig.ReporApenasProduzidos ? 1 : produtoNovo.Qtde;
                produtoNovo.NumEtiquetaRepos = etiqueta;
                produtoNovo.IdProdPedProdRepos = !String.IsNullOrEmpty(etiqueta) ? (uint?)ProdutoPedidoProducaoDAO.Instance.GetByEtiqueta(etiqueta).IdProdPedProducao : null;
                produtoNovo.InvisivelFluxo = false;
                produtoNovo.InvisivelPedido = false;
                produtoNovo.IdItemProjeto = null;
                produtoNovo.IdMaterItemProj = null;
                produtoNovo.IdMaterProjMod = null;
                produtoNovo.ValorVendido = !PedidoConfig.PermitirTrocaPorPedido ?
                    ProdutoDAO.Instance.GetValorTabela((int)produtoNovo.IdProd,
                    PedidoDAO.Instance.ObtemTipoEntrega(idPedido), PedidoDAO.Instance.ObtemIdCliente(idPedido), false, true, produtoNovo.PercDescontoQtde, (int?)idPedido, null, null) : 
                    produtoNovo.ValorVendido;

                if (PedidoConfig.DadosPedido.AmbientePedido)
                    produtoNovo.IdAmbientePedido = Glass.Conversoes.StrParaUint(ambiente);
                else
                    produtoNovo.IdAmbientePedido = null;

                #endregion

                // Adiciona o produto à lista de reposição
                idProdPed = ProdutosPedidoDAO.Instance.Insert(produtoNovo);

                #region Recupera e "clona" os beneficiamentos

                var beneficiamentos = ProdutoPedidoBenefDAO.Instance.GetByProdutoPedido(idProdPedAnterior);
                foreach (var b in beneficiamentos)
                {
                    b.IdProdPedBenef = 0;
                    b.IdProdPed = idProdPed;
                    b.Valor = b.Valor / (decimal)qtdOriginal * (decimal)produtoNovo.Qtde;

                    ProdutoPedidoBenefDAO.Instance.Insert(b);
                }

                #endregion

                // Calcula novamente o valor dos beneficiamentos
                ProdutosPedidoDAO.Instance.UpdateValorBenef(idProdPed);

                // Calcula novamente o valor do total do pedido
                PedidoDAO.Instance.UpdateTotalPedido(Glass.Conversoes.StrParaUint(pedido));

                return "";
            }
            catch (Exception ex)
            {
                return "Erro ao inserir produto: " + ex.Message;
            }
        }
    }
}
