using System;
using System.Web.UI;
using Glass.Data.DAL;

namespace Glass.UI.Web.Utils
{
    public partial class ListaTotalMarcacao : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Recupera os pedidos espelho com base nos filtros da tela de pedidos em conferência.
            var pedidos = PedidoEspelhoDAO.Instance.GetList(Request["idPedido"].StrParaUint(), Request["idCliente"].StrParaUint(),
                Request["nomeCliente"], Request["idLoja"].StrParaUint(), Request["idFunc"].StrParaUint(),
                Request["idFuncionarioConferente"].StrParaUint(), Request["situacao"].StrParaInt(), Request["situacaoPedOri"],
                Request["idsProcesso"], Request["dataIniEnt"], Request["dataFimEnt"], Request["dataIniFab"], Request["dataFimFab"],
                Request["dataIniFin"], Request["dataFimFin"], Request["dataIniConf"], Request["dataFimConf"], Request["dataIniEmis"],
                Request["dataFimEmis"], false, Request["pedidosSemAnexos"] != null && Request["pedidosSemAnexos"].ToLower() == "true",
                Request["situacaoCnc"], Request["dataIniSituacaoCnc"], Request["dataFimSituacaoCnc"],
                Request["pedidosAComprar"] != null && Request["pedidosAComprar"].ToLower() == "true", Request["tipoPedido"],
                Request["idsRotas"], Request["origemPedido"].StrParaInt(), 0, null, null, 0, 0);

            decimal totalComMarcacao = 0, totalSemMarcacao = 0;

            // Percorre cada pedido.
            foreach (var pedido in pedidos)
            {
                // Recupera os itens de projeto do pedido espelho.
                var itensProjeto = ItemProjetoDAO.Instance.GetByPedidoEspelho(pedido.IdPedido);

                // Percorre cada item de projeto.
                foreach (var itemProjeto in itensProjeto)
                {
                    // Recupera as peças do item de projeto.
                    var pecasItemProjeto = PecaItemProjetoDAO.Instance.ObtemPecaItemProjetoParaTotalMarcacao(itemProjeto.IdItemProjeto,
                        itemProjeto.IdProjetoModelo);

                    // Percorre cada peça do item de projeto.
                    foreach (var pecaItemProjeto in pecasItemProjeto)
                    {
                        // Serão consideradas somente peças do tipo INSTALAÇÃO nos totais de peças com/sem marcação.
                        if (pecaItemProjeto.Tipo != 1)
                            continue;

                        /* É considerado que a peça possui marcação se estiver associada a um arquivo de mesa,
                         * se o tipo do arquivo não for nulo e se a peça não tiver sido alterada pelo usuário
                         * (Alteração de imagem, associação de imagem ao produto ou à peça do item projeto ou edição da imagem). */
                        if (pecaItemProjeto.IdArquivoMesaCorte.GetValueOrDefault() > 0 &&
                            pecaItemProjeto.TipoArquivoMesaCorte.GetValueOrDefault() > 0 &&
                            !pecaItemProjeto.ImagemEditada &&
                            !PecaItemProjetoDAO.Instance.PossuiFiguraAssociada(pecaItemProjeto.IdPecaItemProj) &&
                            pecaItemProjeto.IdProdPed.GetValueOrDefault() > 0 &&
                            !ProdutosPedidoEspelhoDAO.Instance.PossuiImagemAssociada(pecaItemProjeto.IdProdPed.Value)
                            )
                            totalComMarcacao +=
                                pecaItemProjeto.QtdeProdPed.GetValueOrDefault() > 0 ?
                                    pecaItemProjeto.QtdeProdPed.Value : 1;
                        else
                            totalSemMarcacao +=
                                pecaItemProjeto.QtdeProdPed.GetValueOrDefault() > 0 ?
                                    pecaItemProjeto.QtdeProdPed.Value : 1;
                    }
                }
            }

            var totalPecas = totalComMarcacao + totalSemMarcacao;
            var percentualComMarcacao = totalComMarcacao / totalPecas;
            var percentualSemMarcacao = totalSemMarcacao / totalPecas;

            lblQuantidadeComMarcacao.Text = string.Format("{0:P} ({1} peça(s))", percentualComMarcacao, totalComMarcacao);
            lblQuantidadeSemMarcacao.Text = string.Format("{0:P} ({1} peça(s))", percentualSemMarcacao, totalSemMarcacao);
        }
    }
}
