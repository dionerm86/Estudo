using System;
using Glass.Data.Helper;

namespace Glass.UI.Web.Cadastros.Producao
{
    public partial class LstRetalhoProducao : System.Web.UI.Page
    {
        private bool? _exibirImpressao = null;
    
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
        }
    
        protected void lnkPesq_Click(object sender, EventArgs e)
        {
        }
    
        protected bool ExibirImpressao(object podeCancelar)
        {
            if (_exibirImpressao == null)
                _exibirImpressao = 
                    Config.PossuiPermissao(Config.FuncaoMenuPCP.ImprimirEtiquetas);
    
            return _exibirImpressao.GetValueOrDefault() && (bool)podeCancelar;
        }
    }
}
