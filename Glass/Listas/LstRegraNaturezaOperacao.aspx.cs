using System;
using System.Web.UI;

namespace Glass.UI.Web.Listas
{
    public partial class LstRegraNaturezaOperacao : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
    
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
    
        }
    
        protected string ObtemDescricaoGrupoSubgrupo(object descricaoGrupo, object descricaoSubgrupo)
        {
            string retorno = String.Empty;
    
            if (descricaoGrupo != null && !String.IsNullOrEmpty(descricaoGrupo.ToString()))
                retorno += descricaoGrupo.ToString();
    
            if (descricaoSubgrupo != null && !String.IsNullOrEmpty(descricaoSubgrupo.ToString()))
                retorno += (retorno != String.Empty ? " / " : "") + descricaoSubgrupo.ToString();
    
            return retorno;
        }
    
        protected string ObtemCorEspessura(object descricaoCorVidro, object descricaoCorAluminio,
            object descricaoCorFerragem, object espessura)
        {
            string retorno = String.Empty;
    
            if (descricaoCorVidro != null && !String.IsNullOrEmpty(descricaoCorVidro.ToString()))
                retorno += descricaoCorVidro.ToString();
    
            else if (descricaoCorAluminio != null && !String.IsNullOrEmpty(descricaoCorAluminio.ToString()))
                retorno += descricaoCorAluminio.ToString();
    
            else if (descricaoCorFerragem != null && !String.IsNullOrEmpty(descricaoCorFerragem.ToString()))
                retorno += descricaoCorFerragem.ToString();
    
            if (espessura != null && !String.IsNullOrEmpty(espessura.ToString()))
                retorno += (retorno != String.Empty ? " / " : "") + espessura.ToString() + "mm";
    
            return retorno;
        }
    }
}
