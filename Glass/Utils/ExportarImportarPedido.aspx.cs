using System;
using System.Web.UI.WebControls;
using Glass.Data.Model;
using Glass.Data.DAL;
using Glass.Configuracoes;
using System.Linq;

namespace Glass.UI.Web.Utils
{
    public partial class ExportarImportarPedido : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Utils.ExportarImportarPedido));
    
            if (!IsPostBack)
                lblTamanhoMaximo.Text += FuncoesGerais.GetTamanhoMaximoUpload() + " MB";
        }
    
        [Ajax.AjaxMethod]
        public string GetDadosPedido(string idPedido)
        {
            try
            {
                if (!PedidoDAO.Instance.PedidoExists(Glass.Conversoes.StrParaUint(idPedido)))
                    throw new Exception("Pedido não encontrado.");

                Glass.Data.Model.Pedido pedido = PedidoDAO.Instance.GetElement(Glass.Conversoes.StrParaUint(idPedido));
                decimal total = !PedidoEspelhoDAO.Instance.ExisteEspelho(pedido.IdPedido) ? pedido.Total : pedido.TotalEspelho;
    
                return "Ok#" + pedido.NomeCliente + "#" + pedido.CodCliente + "#" + pedido.NomeFunc + "#" + 
                    pedido.DataEntrega.Value.ToString("dd/MM/yyyy") + "#" + total.ToString("C");
            }
            catch (Exception ex)
            {
                return "Erro;" + Glass.MensagemAlerta.FormatErrorMsg("Falha ao recuperar dados do pedido.", ex);
            }
        }
    
        [Ajax.AjaxMethod]
        public string PodeAdicionar(string idPedido)
        {
            return PedidoDAO.Instance.PodeExportar(Glass.Conversoes.StrParaUint(idPedido)).ToString().ToLower();
        }
    
        [Ajax.AjaxMethod]
        public string GetAmbientes(string idPedidoStr, string linhaAmbiente)
        {
            try
            {
                string retorno = "";
                string javaScript = "";
    
                uint idPedido = Glass.Conversoes.StrParaUint(idPedidoStr);
                bool usarEspelho = PedidoEspelhoDAO.Instance.ExisteEspelho(idPedido);
                bool isMaoObra = PedidoDAO.Instance.IsMaoDeObra(idPedido);
    
                bool usarAmbientePedido = PedidoConfig.DadosPedido.AmbientePedido;
                var produtos = ProdutosPedidoDAO.Instance.GetByPedidoLite(idPedido, usarEspelho);
    
                if (!PedidoConfig.DadosPedido.AmbientePedido)
                {
                    bool temProdutoSemProjeto = produtos.Any(x => (!usarEspelho ? x.IdAmbientePedido : x.IdAmbientePedidoEspelho) == null);
    
                    if (temProdutoSemProjeto)
                    {
                        string tabelaProdutos = "";
    
                        if (produtos.Count > 0)
                        {
                            tabelaProdutos = "<table id='produtos_" + linhaAmbiente + "' cellspacing='6' cellpadding='0' width='100%'><tr><th></th>" +
                                "<th>Cód.</th><th>Produto</th><th>Qtd.</th><th>Largura</th><th>Altura</th></tr>";
    
                            foreach (ProdutosPedido p in produtos)
                            {
                                if ((!usarEspelho ? p.IdAmbientePedido : p.IdAmbientePedidoEspelho) > 0)
                                {
                                    usarAmbientePedido = true;
                                    continue;
                                }
    
                                tabelaProdutos += "<tr><td><input type='checkbox' value='" + p.IdProdPed + "' checked='checked'></td><td>" + p.CodInterno + "</td><td width='100%'>" +
                                    p.DescrProduto + " " + p.DescrBeneficiamentos + "</td><td>" + p.Qtde + (isMaoObra ? " x " +
                                    p.QtdeAmbiente + " peça(s) de vidro" : "") + "</td><td>" + p.Largura + "</td><td>" + p.AlturaLista + "</td></tr>";
                            }
    
                            tabelaProdutos += "</table>";
                        }
    
                        retorno += "<tr><td>" + tabelaProdutos + "</td></tr>";
                    }
                    else
                        usarAmbientePedido = true;
                }
                
                if (usarAmbientePedido)
                {
                    foreach (AmbientePedido a in AmbientePedidoDAO.Instance.GetByPedido(idPedido, usarEspelho))
                    {
                        string funcaoJavaScript = "exibirProdutosAmbiente({0}, " + a.IdAmbientePedido + ", " + linhaAmbiente + ")";
                        string funcaoApenasVidros = "apenasVidros({0}, " + a.IdAmbientePedido + ", " + linhaAmbiente + ")";
    
                        var produtosAmb = produtos.Where(x => (!usarEspelho ? x.IdAmbientePedido : x.IdAmbientePedidoEspelho) == a.IdAmbientePedido).ToList();                    
    
                        bool temProdutos = false;
                        string tabelaProdutos = "";
    
                        if (produtosAmb.Count > 0)
                        {
                            tabelaProdutos = "<table id='produtosAmbiente_" + a.IdAmbientePedido + "_" + linhaAmbiente + "' cellspacing='6' cellpadding='0' width='100%'" +
                                "style='margin-left: 32px; padding-right: 32px'><tr><th></th><th>Cód.</th><th>Produto</th><th>Qtd.</th><th>Largura</th><th>Altura</th></tr>";
    
                            foreach (ProdutosPedido p in produtosAmb)
                            {
                                temProdutos = true;
                                tabelaProdutos += "<tr><td><input type='checkbox' value='" + p.IdProdPed + "' checked='checked'" + (a.IdItemProjeto > 0 ? " disabled='true'" +
                                    (Glass.Data.DAL.GrupoProdDAO.Instance.IsVidro((int)p.IdGrupoProd) ? " isVidro='true'" : "") : "") + "></td><td>" + p.CodInterno + "</td><td width='100%'>" + 
                                    p.DescrProduto + " " + p.DescrBeneficiamentos + "</td><td>" + p.Qtde + (isMaoObra ? " x " + p.QtdeAmbiente + 
                                    " peça(s) de vidro" : "") + "</td><td>" + p.Largura + "</td><td>" + p.AlturaLista + "</td></tr>";
                            }
    
                            tabelaProdutos += "</table>";
    
                            if (ProjetoConfig.ControleModeloProjeto.ApenasVidrosPadrao)
                                javaScript += String.Format(funcaoApenasVidros, "true") + "; ";
                        }
    
                        string opcoesCheckbox = temProdutos ? " checked='checked'" : " disabled='true'";
    
                        ItemProjeto item = null;
                        if (a.IdItemProjeto > 0)
                            item = ItemProjetoDAO.Instance.GetElementByPrimaryKey(a.IdItemProjeto.Value);
    
                        retorno += "<tr><td><input type='checkbox' value='" + a.IdAmbientePedido + "' id='ambientePed_" + a.IdAmbientePedido + "'" + opcoesCheckbox + " " +
                            "isAmbiente='true' onclick='" + String.Format(funcaoJavaScript, "this.checked") + "'>" +
                            "<label for='ambientePed_" + a.IdAmbientePedido + "'>" + a.Ambiente + "</label>" + (item == null ? "" :
                            "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<input type='checkbox' id='apenasVidros_" + a.IdAmbientePedido + "' isAmbiente='true' " +
                            "onclick='" + String.Format(funcaoApenasVidros, "this.checked") + "'" + (ProjetoConfig.ControleModeloProjeto.ApenasVidrosPadrao || item.ApenasVidros ? " checked='checked'" : "") + 
                            (item.ApenasVidros ? " disabled='true'" : "") + " /><label for='apenasVidros_" + a.IdAmbientePedido + "'>Apenas vidros</label>") + 
                            tabelaProdutos + "</td></tr>";
    
                        if (!temProdutos)
                            javaScript += String.Format(funcaoJavaScript, "false") + "; UnTip(); ";
                    }
                }
    
                return "<table>" + retorno + "</table>¬" + javaScript;
            }
            catch
            {
                return "";
            }
        }
    
        protected void btnImportar_Click(object sender, EventArgs e)
        {
            try
            {
                string msg = "";//UtilsExportacaoPedido.Importar(fluArquivo.FileBytes);
                Glass.MensagemAlerta.ShowMsg(msg, Page);
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg(ex.InnerException != null ? ex.Message + "\n" : "", ex, Page);
            }
        }
    }
}
