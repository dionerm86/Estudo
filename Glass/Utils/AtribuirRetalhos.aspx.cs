using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Helper;
using Glass.Data.DAL;
using System.Text;
using System.IO;
using System.Collections.Generic;

namespace Glass.UI.Web.Utils
{
    public partial class AtribuirRetalhos : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Só exibe a página como popup
            if (!(Page.Master as Painel).IsPopup())
            {
                Response.Redirect(Request.Url.ToString() + (Request.QueryString.Count > 0 ? "&" : "?") + "popup=true");
                return;
            }
    
            Ajax.Utility.RegisterTypeForAjax(typeof(Utils.AtribuirRetalhos));
        }
    
        [Ajax.AjaxMethod]
        public string GetRetalhos(string idProdPed, string numeroLinha, string numeroPeca, string idSelecionado)
        {
            List<string> ids = new List<string>();
    
            CheckBoxList lst = new CheckBoxList
            {
                ID = "retalhos_" + numeroLinha + "_" + numeroPeca,
                DataSource = RetalhoProducaoDAO.Instance.ObterRetalhosProducao(Glass.Conversoes.StrParaUint(idProdPed.TrimStart('R')), true),
                DataTextField = "DescricaoRetalhoComEtiqueta",
                DataValueField = "IdRetalhoProducao",
                CssClass = "pos"
            };
    
            lst.DataBind();
            bool selecionado = false;
    
            foreach (ListItem item in lst.Items)
            {
                item.Attributes.Add("idRetalhoProducao", item.Value);
                item.Attributes.Add("OnClick", "atribuir(this)");
                ids.Add(item.Value);
    
                if (idSelecionado == item.Value)
                {
                    item.Selected = true;
                    selecionado = true;
                }
            }
    
            StringBuilder sb = new StringBuilder();
    
            using (StringWriter sw = new StringWriter(sb))
                using (HtmlTextWriter writer = new HtmlTextWriter(sw))
                {
                    lst.RenderControl(writer);
                }
    
            return sb.ToString() + "|" + String.Join(",", ids.ToArray()) + "|" + (selecionado ? idSelecionado : "");
        }
    
        [Ajax.AjaxMethod]
        public string Otimizar(string dadosRetalhos)
        {
            List<OtimizarRetalhos.DadosRetalho> retalhos = Newtonsoft.Json.JsonConvert.DeserializeObject<List<OtimizarRetalhos.DadosRetalho>>(dadosRetalhos);
    
            // Otimiza os retalhos
            OtimizarRetalhos.Otimizar(ref retalhos);
    
            return Newtonsoft.Json.JsonConvert.SerializeObject(retalhos);
        }
    }
}
