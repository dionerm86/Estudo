using System;
using System.Web.UI;
using Glass.Data.DAL;

namespace Glass.UI.Web.Utils
{
    public partial class ListaTotalMarcacao : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Recupera os pedidos espelho com base nos filtros da tela de pedidos em confer�ncia.
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
                    // Recupera as pe�as do item de projeto.
                    var pecasItemProjeto = PecaItemProjetoDAO.Instance.ObtemPecaItemProjetoParaTotalMarcacao(itemProjeto.IdItemProjeto,
                        itemProjeto.IdProjetoModelo);

                    // Percorre cada pe�a do item de projeto.
                    foreach (var pecaItemProjeto in pecasItemProjeto)
                    {
                        // Ser�o consideradas somente pe�as do tipo INSTALA��O nos totais de pe�as com/sem marca��o.
                        if (pecaItemProjeto.Tipo != 1)
                            continue;

                        /* � considerado que a pe�a possui marca��o se estiver associada a um arquivo de mesa,
                         * se o tipo do arquivo n�o for nulo e se a pe�a n�o tiver sido alterada pelo usu�rio
                         * (Altera��o de imagem, associa��o de imagem ao produto ou � pe�a do item projeto ou edi��o da imagem). */
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

            lblQuantidadeComMarcacao.Text = string.Format("{0:P} ({1} pe�a(s))", percentualComMarcacao, totalComMarcacao);
            lblQuantidadeSemMarcacao.Text = string.Format("{0:P} ({1} pe�a(s))", percentualSemMarcacao, totalSemMarcacao);
        }
    }
}
