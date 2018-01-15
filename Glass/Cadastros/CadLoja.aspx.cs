using System;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadLoja : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            dtvLoja.Register("../Listas/LstLoja.aspx");
            odsLoja.Register();           
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["idLoja"] != null)
                dtvLoja.ChangeMode(DetailsViewMode.Edit);
    
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));

            EsconderConfiguracoesLoja();
        }
    
        protected void EsconderConfiguracoesLoja()
        {
            if (!Configuracoes.PedidoConfig.DadosPedido.BloquearItensCorEspessura &&
                (!Configuracoes.Liberacao.DadosLiberacao.LiberarProdutosProntos ||
                Configuracoes.FinanceiroConfig.DadosLiberacao.PermitirLiberacaoPedidosLojasDiferentes))
            {
                dtvLoja.Rows[26].Visible = false;
            }
            else
            {
                var chkCorEspesura = (CheckBox)dtvLoja.FindControl("chkCorEspesura");
                var chkProdutosProntos = (CheckBox)dtvLoja.FindControl("chkProdutosProntos");

                if (!Configuracoes.PedidoConfig.DadosPedido.BloquearItensCorEspessura)
                    chkCorEspesura.Visible = false;
                if (!Configuracoes.Liberacao.DadosLiberacao.LiberarProdutosProntos || Configuracoes.FinanceiroConfig.DadosLiberacao.PermitirLiberacaoPedidosLojasDiferentes)
                    chkProdutosProntos.Visible = false;
            }
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            Response.Redirect("../Listas/LstLoja.aspx");
        }

        /// <summary>
        /// Processa a loja salva.
        /// </summary>
        /// <param name="saveResult"></param>
        protected string LojaSalva(Colosoft.Business.SaveResult saveResult)
        {
            if (!saveResult)
                throw new Colosoft.DetailsException(saveResult.Message);

            var retorno = string.Empty;

            // Recupera o control de upload
            var imagemCor = (FileUpload)dtvLoja.FindControl("filImagemCor");
            var imagemSemCor = (FileUpload)dtvLoja.FindControl("filImagemSemCor");

            if (imagemCor != null)
            {
                if (imagemCor.HasFile && imagemCor.FileName.EndsWith(".png"))
                {
                    var repositorio = Microsoft.Practices.ServiceLocation.ServiceLocator
                        .Current.GetInstance<Glass.Global.Negocios.Entidades.ILojaRepositorioImagens>();

                    // Salva a imagem com cor
                    repositorio.SalvarImagem(Request["IdLoja"].StrParaInt(), true, imagemCor.PostedFile.InputStream);
                }
                else if (!string.IsNullOrEmpty(imagemCor.FileName))
                {
                    retorno += string.Format("O arquivo ({0}) não pode ser inserido. Verifique a extensão da imagem e tente novamente. ", imagemCor.FileName);
                }
            }

            if (imagemSemCor != null)
            {
                if (imagemSemCor.HasFile && imagemSemCor.FileName.EndsWith(".png"))
                {
                    var repositorio = Microsoft.Practices.ServiceLocation.ServiceLocator
                        .Current.GetInstance<Glass.Global.Negocios.Entidades.ILojaRepositorioImagens>();

                    // Salva a imagem sem cor
                    repositorio.SalvarImagem(Request["IdLoja"].StrParaInt(), false, imagemSemCor.PostedFile.InputStream);
                }
                else if (!string.IsNullOrEmpty(imagemSemCor.FileName))
                {
                    retorno += string.Format("O arquivo ({0}) não pode ser inserido. Verifique a extensão da imagem e tente novamente.", imagemSemCor.FileName);
                }
            }

            return retorno;
        }

        protected void odsLoja_Inserted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
                throw e.Exception;

            else
            {
                var retorno = LojaSalva(e.ReturnValue as Colosoft.Business.SaveResult);
                if (!string.IsNullOrEmpty(retorno))
                    throw new Exception(retorno);
            }
        }

        protected void odsLoja_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
                throw e.Exception;

            else
            {
                var retorno = LojaSalva(e.ReturnValue as Colosoft.Business.SaveResult);
                if (!string.IsNullOrEmpty(retorno))
                    throw new Exception(retorno);
            }
        }
    }
}
