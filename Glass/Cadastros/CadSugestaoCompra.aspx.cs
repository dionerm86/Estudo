using System;
using System.Web.UI;
using Glass.Data.Model;
using Glass.Data.DAL;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadSugestaoCompra : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadSugestaoCompra));
            if (IsProducao())
            {
                Page.Title = "Sugestão de Produção";
                totalCompra.Style.Add("Display", "none");
                btnGerarCompra.Text = "Gerar Pedido";
                btnGerarCompra.OnClientClick = "gerarPedido(); return false";
            }
    
            if (!IsPostBack)
            {
                bool buscar = false;
    
                if (Request["idLoja"] != null)
                {
                    drpLoja.DataBind();
                    drpLoja.SelectedValue = Request["idLoja"];
                }
    
                if (Request["idGrupoProd"] != null)
                {
                    buscar = true;
                    drpGrupo.DataBind();
                    drpGrupo.SelectedValue = Request["idGrupoProd"];
                }
    
                if (Request["idSubgrupoProd"] != null)
                {
                    buscar = true;
                    drpSubgrupo.DataBind();
                    drpSubgrupo.SelectedValue = Request["idSubgrupoProd"];
                }
    
                if (Request["codInterno"] != null)
                {
                    buscar = true;
                    txtCodProd.Text = Request["codInterno"];
                }
    
                if (Request["descricao"] != null)
                {
                    buscar = true;
                    txtDescrProd.Text = Request["descricao"];
                }
    
                if (buscar)
                    Page.ClientScript.RegisterStartupScript(GetType(), "adicionarInicial", "adicionar();\n", true);
            }
        }
    
        protected bool IsProducao()
        {
            return Request["producao"] == "1";
        }
    
        [Ajax.AjaxMethod]
        public string GetSubgrupos(string idGrupoStr, string isProducao)
        {
            try
            {
                int idGrupo = Glass.Conversoes.StrParaInt(idGrupoStr);
                
                string subgrupos = "";
                foreach (SubgrupoProd s in SubgrupoProdDAO.Instance.GetForFilter(idGrupo.ToString(), isProducao == "1", false))
                    subgrupos += String.Format("<option value='{0}'>{1}</option>", s.IdSubgrupoProd, s.Descricao);
    
                if (isProducao == "1")
                    subgrupos = "<option value='0'>Todos</option>" + subgrupos;
    
                return "Ok#" + subgrupos;
            }
            catch (Exception ex)
            {
                return "Erro#" + Glass.MensagemAlerta.FormatErrorMsg("Falha ao recuperar subgrupos.", ex);
            }
        }
    
        [Ajax.AjaxMethod]
        public string GetProdutos(string idLojaStr, string idGrupoStr, string idSubgrupoStr,
            string codInterno, string descricao, string isProducao, string mesesVendas, string mesesEstoque)
        {
            try
            {
                uint idLoja = Glass.Conversoes.StrParaUint(idLojaStr);
                uint idGrupo = Glass.Conversoes.StrParaUint(idGrupoStr);
                uint idSubgrupo = Glass.Conversoes.StrParaUint(idSubgrupoStr);
    
                string produtos = "";

                var dados = ProdutoDAO.Instance.GetForSugestaoCompra(idLoja, idGrupo, idSubgrupo, codInterno,
                    descricao, isProducao == "1", mesesVendas.StrParaInt(), mesesEstoque.StrParaInt());

                foreach (var p in dados)
                {
                    var valor = Glass.Configuracoes.CompraConfig.UsarCustoFornSugestaoCompra ? p.Custofabbase : p.CustoCompra;

                    float qtde = isProducao != "1" ? p.SugestaoCompra : p.SugestaoProducao;

                    if (mesesVendas.StrParaInt() > 0)
                        qtde = (float)p.SugestaoCompraMensal;

                    produtos += p.IdProd + ";" + p.CodInterno + ";" + p.Descricao + ";" + p.DescrGrupo + ";" +
                        p.DescrSubgrupo + ";" + p.EstoqueMinimo + ";" + Math.Round(qtde, 2) + ";" +
                        Math.Round((p.QtdeEstoque - p.Reserva - (Configuracoes.PedidoConfig.LiberarPedido ? p.Liberacao : 0)), 2) + ";" +
                        valor.ToString("C") + ";" + (valor * (decimal)qtde).ToString("C") + ";" + Math.Round(p.MediaVendaMensal, 2) + "|";
                }
    
                
                return "Ok#" + produtos.TrimEnd('|');
            }
            catch (Exception ex)
            {
                return "Erro#" + Glass.MensagemAlerta.FormatErrorMsg("Falha ao buscar produtos.", ex);
            }
        }
    
        [Ajax.AjaxMethod]
        public string GerarCompra(string idLojaStr, string idsProd, string mesesVendas, string mesesEstoque)
        {
            try
            {
                if (String.IsNullOrEmpty(idsProd))
                    return "Erro#" + "Selecione pelo menos um produto.";

                uint idLoja = Glass.Conversoes.StrParaUint(idLojaStr);
                uint idCompra = ProdutoDAO.Instance.GerarCompraSugerida(idLoja, idsProd.TrimEnd(',', ' '), mesesVendas.StrParaInt(), mesesEstoque.StrParaInt());
                return "Ok#" + idCompra;
            }
            catch (Exception ex)
            {
                return "Erro#" + Glass.MensagemAlerta.FormatErrorMsg("Falha ao gerar compra.", ex);
            }
        }
    
        [Ajax.AjaxMethod]
        public string GerarPedido(string idLojaStr, string idsProd)
        {
            try
            {
                uint idLoja = Glass.Conversoes.StrParaUint(idLojaStr);
                uint idPedido = ProdutoDAO.Instance.GerarPedidoSugerido(idLoja, idsProd.TrimEnd(',', ' '));
                return "Ok#" + idPedido;
            }
            catch (Exception ex)
            {
                return "Erro#" + Glass.MensagemAlerta.FormatErrorMsg("Falha ao gerar pedido.", ex);
            }
        }
    
        protected void drpGrupo_DataBound(object sender, EventArgs e)
        {
            if (IsProducao())
            {
                drpGrupo.SelectedValue = ((int)Glass.Data.Model.NomeGrupoProd.Vidro).ToString();
                drpGrupo.Enabled = false;
                string atual = Request["idSubgrupoProd"] != null ? Request["idSubgrupoProd"] : 
                    drpSubgrupo.SelectedValue != "" ? drpSubgrupo.SelectedValue : "0";
                Page.ClientScript.RegisterStartupScript(GetType(), "subgrupos", "var idSubgrupoAtual=" + atual + 
                    "; getSubgrupos(FindControl('drpGrupo', 'select').value); FindControl('drpSubgrupo', 'select').value = idSubgrupoAtual;\n", true);
            }
        }
    }
}
