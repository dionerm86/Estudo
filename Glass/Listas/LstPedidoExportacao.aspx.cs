using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.DAL;
using Glass.Data.Model;
using System.Collections.Generic;

namespace Glass.UI.Web.Listas
{
    public partial class LstPedidoExportacao : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
    
        }
    
        protected void lnkPesquisar_Click(object sender, ImageClickEventArgs e)
        {
            grdPedido.PageIndex = 0;
        }
    
        protected void grdPedido_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Consultar")
            {
                var exportacao = ExportacaoDAO.Instance.GetElement(e.CommandArgument.ToString().StrParaUint());
                var idsPedido = PedidoExportacaoDAO.Instance.PesquisarPedidosExportacao(e.CommandArgument.ToString().StrParaUint());

                var loja = LojaDAO.Instance.GetElement(Data.Helper.UserInfo.GetUserInfo.IdLoja);
                var fornecedor = FornecedorDAO.Instance.GetElement(exportacao.IdFornec);

                var listaPedidos = new Dictionary<uint, bool>();
                foreach (var item in idsPedido)
                    listaPedidos.Add(Glass.Conversoes.StrParaUint(item.ToString()), true);

                byte[] buffer = Data.Helper.UtilsExportacaoPedido.ConfigurarExportacao(listaPedidos, new uint[] { });
                var urlFornecedor = string.Format("{0}{1}", fornecedor.UrlSistema.ToLower().Substring(0, fornecedor.UrlSistema.ToLower()
                    .LastIndexOf("/webglass")).TrimEnd('/'), "/service/wsexportacaopedido.asmx");

                object[] parametros = new object[] { loja.Cnpj, 1, buffer };

                // Consulta a situação do pedido
                object retorno = WebService.ChamarWebService(urlFornecedor, "SyncService", "VerificarExportacaoPedidos", parametros);

                Data.Helper.UtilsExportacaoPedido.AtualizarPedidosExportacao(retorno as string[]);

                Glass.MensagemAlerta.ShowMsg("A Situação dos pedidos foram atualizadas.", Page);
            }
        }
    }
}
