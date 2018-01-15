using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Helper;
using Glass.Configuracoes;
using Microsoft.Practices.ServiceLocation;

namespace Glass.UI.Web.Listas
{
    public partial class LstFornecedor : System.Web.UI.Page
    {
        /// <summary>
        /// Verifica se pode inativar o fornecedor.
        /// </summary>
        /// <param name="fornecedor"></param>
        /// <returns></returns>
        protected bool PodeInativar()
        {
            return Config.PossuiPermissao(Config.FuncaoMenuCadastro.AtivarInativarFornecedor);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            grdFornecedor.Register();
            odsFornecedor.Register();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!FinanceiroConfig.FormaPagamento.CreditoFornecedor)
            {
                grdFornecedor.Columns[grdFornecedor.Columns.Count - 2].Visible = false;
                chkCredito.Style.Add("display", "none");
                imgPesqFornComCredito.Visible = false;
            }

            lnkInserir.Visible = Config.PossuiPermissao(Config.FuncaoMenuCadastro.CadastrarFornecedor);
        }

        /// <summary>
        /// Verifica se pode editar o fornecedor.
        /// </summary>
        /// <returns></returns>
        protected bool PodeEditar()
        {
            return Config.PossuiPermissao(Config.FuncaoMenuCadastro.CadastrarFornecedor);
        }

        /// <summary>
        /// Verifica se pode apagar o fornecedor.
        /// </summary>
        /// <returns></returns>
        protected bool PodeApagar()
        {
            return Config.PossuiPermissao(Config.FuncaoMenuCadastro.CadastrarFornecedor);
        }

        protected bool FotosVisible()
        {
            return Config.PossuiPermissao(Config.FuncaoMenuCadastro.AnexarArquivosFornecedor);
        }

        protected void lnkInserir_Click(object sender, EventArgs e)
        {
            Response.Redirect("../Cadastros/CadFornecedor.aspx");
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdFornecedor.PageIndex = 0;
        }
    
        protected void grdFornec_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "Inativar")
                {
                    int idFornec = int.Parse(e.CommandArgument.ToString());
                    var fluxo = ServiceLocator.Current
                        .GetInstance<Glass.Global.Negocios.IFornecedorFluxo>();

                    var fornecedor = fluxo.ObtemFornecedor(idFornec);

                    fornecedor.Situacao =
                        fornecedor.Situacao == Data.Model.SituacaoFornecedor.Ativo ?
                        Data.Model.SituacaoFornecedor.Inativo : Data.Model.SituacaoFornecedor.Ativo;

                    var resultado = fluxo.SalvarFornecedor(fornecedor);

                    if (!resultado)
                        Glass.MensagemAlerta.ErrorMsg("Não foi possível alterar a situação do fornecedor.", resultado);
                    else
                        grdFornecedor.DataBind();
                }
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("", ex, Page);
            }
        }
    }
}
