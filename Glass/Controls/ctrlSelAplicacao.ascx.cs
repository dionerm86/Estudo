using System;
using Glass.Data.DAL;

namespace Glass.UI.Web.Controls
{
    public partial class ctrlSelAplicacao : BaseUserControl
    {
        #region Propriedades
    
        public uint? CodigoAplicacao
        {
            get { return Glass.Conversoes.StrParaUintNullable(selAplicacao.Valor); }
            set
            {
                if (value != null)
                {
                    selAplicacao.Valor = value.Value.ToString();
                    selAplicacao.Descricao = EtiquetaAplicacaoDAO.Instance.ObtemCodInterno(value.Value);
                }
                else
                {
                    selAplicacao.Valor = null;
                    selAplicacao.Descricao = null;
                }
            }
        }
    
        public string ValidationGroup
        {
            get { return selAplicacao.ValidationGroup; }
            set { selAplicacao.ValidationGroup = value; }
        }
    
        public bool PermitirVazio
        {
            get { return selAplicacao.PermitirVazio; }
            set { selAplicacao.PermitirVazio = value; }
        }
    
        #endregion
    
        protected void Page_Load(object sender, EventArgs e)
        {
    
        }
    }
}
