using System;
using System.Web.UI.WebControls;
using Glass.Data.Model;
using Glass.Data.DAL;

namespace Glass.UI.Web.Cadastros.Projeto
{
    public partial class CadValidacaoPecaModelo : System.Web.UI.Page
    {
        /// <summary>
        /// Carregamento da página.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.Projeto.CadValidacaoPecaModelo));
            hdfIdProjetoModelo.Value = Request["idProjetoModelo"];
            hdfIdPecaProjMod.Value = Request["idPecaProjMod"];
        }

        #region Eventos grid

        /// <summary>
        /// Carregamento da listagem de validações, caso não haja nenhuma validação cadastrada o footer deve ser exibido.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grdValidacaoPecaModelo_PreRender(object sender, EventArgs e)
        {
            var idPecaProjMod = Conversoes.StrParaIntNullable(hdfIdPecaProjMod.Value);

            // Se não houver nenhuma peça cadastrada no pedido
            if (idPecaProjMod.GetValueOrDefault() > 0 &&
                ValidacaoPecaModeloDAO.Instance.ObtemQuantidadeValidacoes(idPecaProjMod.Value) == 0)
                grdValidacaoPecaModelo.Rows[0].Visible = false;
        }

        /// <summary>
        /// Exibe ou não o rodapé.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grdValidacaoPecaModelo_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            grdValidacaoPecaModelo.ShowFooter = e.CommandName != "Edit";
        }

        #endregion

        #region Insere validação

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkInsValidacaoPecaModelo_Click(object sender, EventArgs e)
        {
            try
            {
                ValidacaoPecaModelo validacaoPecaModelo = new ValidacaoPecaModelo();
                validacaoPecaModelo.IdPecaProjMod = Glass.Conversoes.StrParaInt(hdfIdPecaProjMod.Value);
                validacaoPecaModelo.PrimeiraExpressaoValidacao =
                    ((TextBox)grdValidacaoPecaModelo.FooterRow.FindControl("txtPrimeiraExpressaoValidacao")).Text;
                validacaoPecaModelo.SegundaExpressaoValidacao =
                    ((TextBox)grdValidacaoPecaModelo.FooterRow.FindControl("txtSegundaExpressaoValidacao")).Text;
                validacaoPecaModelo.TipoComparador =
                    ((DropDownList)grdValidacaoPecaModelo.FooterRow.FindControl("drpTipoComparador"))
                        .SelectedValue.StrParaInt();
                validacaoPecaModelo.Mensagem = ((TextBox)grdValidacaoPecaModelo.FooterRow.FindControl("txtMensagem")).Text;
                validacaoPecaModelo.TipoValidacao =
                    ((DropDownList)grdValidacaoPecaModelo.FooterRow.FindControl("drpTipoValidacao"))
                        .SelectedValue.StrParaInt();

                ValidacaoPecaModeloDAO.Instance.Insert(validacaoPecaModelo);
                grdValidacaoPecaModelo.DataBind();
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao inserir validação da peça.", ex, Page);
            }
        }

        #endregion
    }
}
