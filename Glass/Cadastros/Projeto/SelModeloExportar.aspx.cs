using System;
using System.Collections.Generic;
using System.Web.UI;
using Glass.Data.Model;

namespace Glass.UI.Web.Cadastros.Projeto
{
    public partial class SelModeloExportar : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdModelosProjeto.PageIndex = 0;
        }
    
        protected void lnkAddAll_Click(object sender, EventArgs e)
        {
            // Busca as contas a receber que estiverem na tela
            List<ProjetoModelo> lstProjetoMod = new List<ProjetoModelo>((IEnumerable<ProjetoModelo>)odsProjetoModelo.Select());
    
            // Gera o script para adicionar todas elas na tela
            string script = String.Empty;
    
            foreach (ProjetoModelo proj in lstProjetoMod)
                script += "window.opener.setModelo(" + proj.IdProjetoModelo + ", '" + proj.Codigo + "', '" +
                    proj.DescrGrupo + "', '" + proj.Descricao + "', " + proj.Espessura + "); ";
    
            script += "closeWindow()";
    
            ClientScript.RegisterClientScriptBlock(typeof(string), "addAll", script, true);
        }
    }
}
