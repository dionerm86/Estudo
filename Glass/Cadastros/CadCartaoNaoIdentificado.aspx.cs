using System;
using System.Web.UI.WebControls;
using Glass.Data.DAL;
using Glass.Data.Model;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadCartaoNaoIdentificado : System.Web.UI.Page
    {
        #region Vari�veis Locais

        private Financeiro.UI.Web.Process.CartaoNaoIdentificado.CadastroCartaoNaoIdentificadoFluxo _cadastroCNI;

        #endregion

        /// <summary>
        /// M�todo acionado quando o p�gina estiver sendo inicializada.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            _cadastroCNI = Microsoft.Practices.ServiceLocation.ServiceLocator.Current
                .GetInstance<Financeiro.UI.Web.Process.CartaoNaoIdentificado.CadastroCartaoNaoIdentificadoFluxo>();

            if (!string.IsNullOrEmpty(Request["idCartaoNaoIdentificado"]))
                _cadastroCNI.IdCNI = int.Parse(Request["idCartaoNaoIdentificado"]);

            odsCartaoNaoIdentificado.ObjectCreating += DataSourceObjectCreating;

            odsCartaoNaoIdentificado.Register();

            dtvCartaoNaoIdentificado.Register();

            base.OnInit(e);
        }

        /// <summary>
        /// M�todo acionado quando for solicitada a cria��o do DataSource do Orcamento.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataSourceObjectCreating(object sender, Colosoft.WebControls.VirtualObjectDataSourceEventArgs e)
        {
            e.ObjectInstance = _cadastroCNI;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(CadCartaoNaoIdentificado));
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));

            if (!IsPostBack)
            {
                var idCartao = Conversoes.StrParaInt(Request["idCartaoNaoIdentificado"]);
                if (idCartao > 0)
                {
                    _cadastroCNI.IdCNI = idCartao;

                    if (_cadastroCNI.CNI.Situacao != SituacaoCartaoNaoIdentificado.Ativo)
                        Response.Redirect("~/Listas/LstCartaoNaoIdentificado.aspx");
    
                    dtvCartaoNaoIdentificado.ChangeMode(DetailsViewMode.Edit);
                }
            }
        }

        protected void odsCartaoNaoIdentificado_Inserted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            var retorno = e.ReturnValue as Colosoft.Business.SaveResult;
            if (!retorno)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao cadastrar cart�o n�o identificado.", retorno);
                e.ExceptionHandled = true;
            }
            else
                Response.Redirect("../Listas/LstCartaoNaoIdentificado.aspx");
        }
    
        protected void odsCartaoNaoIdentificado_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            var retorno = e.ReturnValue as Colosoft.Business.SaveResult;
            if (!retorno)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao atualizar cart�o n�o identificado.", retorno);
                e.ExceptionHandled = true;
            }
            else
                Response.Redirect("../Listas/LstCartaoNaoIdentificado.aspx", false);
        }
    
        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Listas/LstCartaoNaoIdentificado.aspx");
        }

        #region M�todos Ajax

        [Ajax.AjaxMethod]
        public string ObterTipoCartao(string idTipoCartao)
        {
            if (string.IsNullOrEmpty(idTipoCartao))
                return "Erro|N�o foi poss�vel recuperar o tipo do cart�o.";

            return "Ok|" + ((int)TipoCartaoCreditoDAO.Instance.ObterTipoCartao(null, idTipoCartao.StrParaInt())).ToString();
        }

        #endregion
    }
}
