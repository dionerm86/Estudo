using System;
using System.Web.UI.WebControls;
using Glass.Data.DAL;
using Microsoft.Practices.ServiceLocation;

namespace Glass.UI.Web.Utils
{
    public partial class PrecoFornecedor : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            grdPrecoFornec.Register(true, true);
            odsPrecoFornec.Register();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Utils.PrecoFornecedor));
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
    
            bool prod = false, fornec = false;
            lblTipo.Text = String.Empty;
    
            if (!String.IsNullOrEmpty(Request["idProd"]))
            {
                lblTipo.Text += "<br />Produto: " + ProdutoDAO.Instance.ObtemDescricao(Glass.Conversoes.StrParaInt(Request["idProd"]));
                grdPrecoFornec.Columns[4].Visible = false;
    
                prod = true;
            }
    
            if (!String.IsNullOrEmpty(Request["idFornec"]))
            {
                lblTipo.Text += "<br />Fornecedor: " + FornecedorDAO.Instance.GetNome(Glass.Conversoes.StrParaUint(Request["idFornec"]));
                grdPrecoFornec.Columns[1].Visible = false;
    
                fornec = true;
            }
    
            hdfExibirSemData.Value = (!prod || !fornec).ToString();

            if (lblTipo.Text.Length >= 6)
                lblTipo.Text = lblTipo.Text.Substring(6);

            grdPrecoFornec.Columns[5].Visible = prod && fornec;

            if (prod && grdPrecoFornec.FooterRow.FindControl("txtCustoCompra") != null)
                ((TextBox)grdPrecoFornec.FooterRow.FindControl("txtCustoCompra")).Text = ProdutoDAO.Instance.ObtemCustoCompra(Glass.Conversoes.StrParaInt(Request["idProd"])).ToString();
        }
    
        protected bool ExibirDataVigencia()
        {
            return !grdPrecoFornec.Columns[5].Visible;
        }
    
        #region Métodos Ajax
    
        [Ajax.AjaxMethod]
        public string GetFornec(string idFornec)
        {
            return MetodosAjax.GetFornec(idFornec);
        }
    
        [Ajax.AjaxMethod]
        public string GetProduto(string codInterno)
        {
            string retorno = MetodosAjax.GetProd(codInterno);
            if (retorno.Split(';')[0] == "Prod")
                retorno += ";" + ProdutoDAO.Instance.GetByCodInterno(codInterno).CustoCompra;
    
            return retorno;
        }
    
        [Ajax.AjaxMethod]
        public string Inserir(string idFornec, string idProd, string custo, string codFornec, string prazoEntrega, string dataVigencia)
        {
            try
            {
                var novo = new Glass.Global.Negocios.Entidades.ProdutoFornecedor();
                novo.IdFornec = Glass.Conversoes.StrParaInt(idFornec);
                novo.IdProd = Glass.Conversoes.StrParaInt(idProd);
                novo.DataVigencia = Glass.Conversoes.StrParaDate(dataVigencia);
                novo.CustoCompra = Glass.Conversoes.StrParaDecimal(custo);
                novo.CodFornec = codFornec;
                novo.PrazoEntregaDias = Glass.Conversoes.StrParaInt(prazoEntrega);

                var fluxo = ServiceLocator.Current
                    .GetInstance<Glass.Global.Negocios.IFornecedorFluxo>();

                var resultado = fluxo.SalvarProdutoFornecedor(novo);

                if (!resultado)
                    return string.Format("Error;{0}", resultado.Message.Format());

                return "Ok";
            }
            catch (Exception ex)
            {
                return "Erro;" + Glass.MensagemAlerta.FormatErrorMsg("Falha ao inserir preço de fornecedor.", ex);
            }
        }
    
        #endregion
    }
}
