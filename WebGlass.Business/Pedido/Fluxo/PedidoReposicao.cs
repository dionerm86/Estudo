using System;
using System.Linq;
using Glass.Data.Helper;
using Glass.Data.DAL;
using Glass.Configuracoes;

namespace WebGlass.Business.Pedido.Fluxo
{
    public sealed class PedidoReposicao : BaseFluxo<PedidoReposicao>
    {
        private PedidoReposicao() { }

        #region Ajax

        private static Ajax.IPedidoReposicao _ajax = null;

        public static Ajax.IPedidoReposicao Ajax
        {
            get
            {
                if (_ajax == null)
                    _ajax = new Ajax.PedidoReposicao();

                return _ajax;
            }
        }

        #endregion

        public uint Repor(Glass.Data.Model.Pedido pedido)
        {
            uint idPedidoAtual, idPedidoNovo, idAmbienteNovo;
            Glass.Data.Model.AmbientePedido[] ambientes = null;

            idPedidoAtual = pedido.IdPedido;

            if (pedido.TipoPedido == (int)Glass.Data.Model.Pedido.TipoPedidoEnum.MaoDeObra)
                throw new Exception("Nâo é possível repor um pedido de mão-de-obra.");

            #region "Clona" o pedido

            var pedidoAntigo = PedidoDAO.Instance.GetElement(idPedidoAtual);

            var pedidoNovo = new Glass.Data.Model.Pedido();

            pedidoNovo.TipoVenda = 3;
            pedidoNovo.Situacao = Glass.Data.Model.Pedido.SituacaoPedido.Ativo;
            pedidoNovo.IdPedidoAnterior = idPedidoAtual;
            pedidoNovo.IdFunc = UserInfo.GetUserInfo.CodUser;
            pedidoNovo.IdCli = pedidoAntigo.IdCli;
            pedidoNovo.IdLoja = pedidoAntigo.IdLoja;
            pedidoNovo.TipoEntrega = pedidoAntigo.TipoEntrega;
            pedidoNovo.IdFormaPagto = pedidoAntigo.IdFormaPagto;
            pedidoNovo.EnderecoObra = pedidoAntigo.EnderecoObra;
            pedidoNovo.BairroObra = pedidoAntigo.BairroObra;
            pedidoNovo.CidadeObra = pedidoAntigo.CidadeObra;
            pedidoNovo.DataEntrega = pedidoAntigo.DataEntrega;
            pedidoNovo.CodCliente = pedidoAntigo.CodCliente;

            #endregion

            // Insere o pedido e salva o código
            idPedidoNovo = PedidoDAO.Instance.Insert(pedidoNovo);
            PedidoDAO.Instance.UpdateTotalPedido(idPedidoNovo);

            // Verifica se a empresa usa ambientes
            if (PedidoConfig.DadosPedido.AmbientePedido)
            {
                #region Recupera e junta os ambientes do pedido

                ambientes = AmbientePedidoDAO.Instance.GetByPedido(idPedidoAtual).ToArray();
                var ambienteNovo = new Glass.Data.Model.AmbientePedido();

                ambienteNovo.IdPedido = idPedidoNovo;
                ambienteNovo.Ambiente = "Reposição";

                #endregion

                // Insere o ambiente e salva o código
                idAmbienteNovo = AmbientePedidoDAO.Instance.Insert(ambienteNovo);
            }

            #region Cria o registro na tabela Pedido_Reposicao

            var reposicaoNova = new Glass.Data.Model.PedidoReposicao();
            reposicaoNova.IdPedido = idPedidoNovo;
            reposicaoNova.PodeUtilizarTroca = PedidoConfig.PermitirTrocaPorPedido;

            PedidoReposicaoDAO.Instance.Insert(reposicaoNova);

            #endregion

            return idPedidoNovo;
        }

        public void Finalizar(uint idPedido, bool byVend, out string script)
        {
            var pedido = PedidoDAO.Instance.GetElementByPrimaryKey(idPedido);
            var pedRepos = PedidoReposicaoDAO.Instance.GetByPedido(idPedido);
            var lstProd = ProdutosPedidoDAO.Instance.GetByPedido(idPedido);
            int countProdPed = lstProd.Count;

            if (Glass.Configuracoes.PedidoConfig.TelaCadastroPedidoReposicao.ExigirDataClienteInformadoReposicaoAoFinalizar && pedRepos.DataClienteInformado == null)
                throw new Exception("Informe a data em que o cliente foi informado da reposição.");

            if (string.IsNullOrEmpty(pedRepos.Assunto))
                throw new Exception("Informe o campo assunto.");

            if (String.IsNullOrEmpty(pedRepos.Solucao))
                throw new Exception("Informe o campo solução.");

            // Verifica se o Pedido possui produtos
            if (countProdPed == 0)
                throw new Exception("Inclua pelo menos um produto no pedido para finalizá-lo.");

            else
            {
                var lstProdPed = ProdutosPedidoDAO.Instance.GetByPedido(idPedido);
                string descrProd;

                foreach (var p in lstProdPed)
                {
                    descrProd = p.DescrProduto;

                    if (!String.IsNullOrEmpty(descrProd) && (descrProd.Trim().ToLower() == "t o t a l" ||
                        descrProd.Trim().ToLower() == "total" || descrProd.Trim().ToLower() == "pedido em conferencia"))
                    {
                        throw new Exception("Inclua pelo menos um produto no pedido que não seja o produto TOTAL ou " +
                            "PEDIDO EM CONFERENCIA para finalizá-lo.");
                    }
                }
            }

            if (UserInfo.GetUserInfo.TipoUsuario == (int)Utils.TipoFuncionario.Vendedor &&
                !Config.PossuiPermissao(Config.FuncaoMenuPedido.GerarReposicao))
            {
                throw new Exception("Apenas o gerente pode emitir pedido de reposição.");
            }

            // Confirma o pedido
            PedidoDAO.Instance.ConfirmaGarantiaReposicaoComTransacao(pedido.IdPedido, false);
            
            string pathName = "../Listas/LstPedidos.aspx";

            if (byVend)
                pathName += "?ByVend=1";

            script = @"
                openWindow(600, 800, '../Relatorios/RelPedidoRepos.aspx?idPedido=" + pedido.IdPedido + @"');
                redirectUrl('" + pathName + "');";
        }
    }
}
