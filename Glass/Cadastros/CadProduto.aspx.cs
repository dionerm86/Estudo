using System;
using System.Linq;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Glass.Configuracoes;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadProduto : System.Web.UI.Page
    {
        #region Variáveis Locais

        /// <summary>
        /// Instancia do produto que está sendo salvo.
        /// </summary>
        private Glass.Global.Negocios.Entidades.Produto _produto;

        #endregion

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            string urlSucesso = null;

            if (string.IsNullOrEmpty(Request["gr"]))
                urlSucesso = ResolveUrl("~/Listas/LstProduto.aspx?gr=" + Request["gr"] + "&sb=" + Request["sb"]);
            else
                urlSucesso = ResolveUrl("~/Listas/LstProduto.aspx");

            dtvProduto.Register(urlSucesso);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["idProd"] == null)
            {
                if (!IsPostBack && dtvProduto.FindControl("txtCodInterno") != null)
                    ((TextBox)dtvProduto.FindControl("txtCodInterno")).Text = ProdutoDAO.Instance.GetLastId();
            }
            else
                dtvProduto.ChangeMode(DetailsViewMode.Edit);
    
            // Se o usuário não tiver permissão para cadastrar produto, retorna para listagem
            if (!Config.PossuiPermissao(Config.FuncaoMenuCadastro.CadastrarProduto))
                Response.Redirect("../Listas/LstProduto.aspx");
        
            // Somente se a empresa for do tipo "NaoVendeVidro", a opção Local de Armazenagem estará visível.
            if (!Glass.Configuracoes.Geral.NaoVendeVidro())
            {
                foreach(DataControlField field in dtvProduto.Fields)
                {
                    if (field.SortExpression == "LocalArmazenagem")
                    {
                        field.Visible = false;
                        break;
                    }
                }
            }

            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadProduto));
        }
    
        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            Response.Redirect(ResolveUrl("~/Listas/LstProduto.aspx?gr=" + Request["gr"] + "&sb=" + Request["sb"]));
        }

        /// <summary>
        /// Método acionado qaundo o produto for salvo.
        /// </summary>
        /// <param name="saveResult"></param>
        private void ProdutoSalvo(Colosoft.Business.SaveResult saveResult)
        {
            if (!saveResult)
                throw new Colosoft.DetailsException(saveResult.Message);

            // Recupera o control de upload
            var imagem = (FileUpload)dtvProduto.FindControl("filImagem");

            if (imagem.HasFile)
            {
                try
                {
                    //Verifica se o arquivo é uma imagem
                    System.Drawing.Bitmap.FromStream(imagem.PostedFile.InputStream);
                }
                catch
                {
                    throw new Exception("Tipo de arquivo invalido, não é possivel salvar imagens do tipo selecionado. Tipos Permitidos: gif, png, jpeg, jpg");
                }


                var repositorio = Microsoft.Practices.ServiceLocation.ServiceLocator
                    .Current.GetInstance<Glass.IProdutoRepositorioImagens>();

                // Salva a imagem do produto
                repositorio.SalvarImagem(!string.IsNullOrEmpty(Request["idProd"]) ? Request["idProd"].StrParaInt() : _produto.IdProd, imagem.PostedFile.InputStream);
            }
        }

        protected void odsProduto_Inserting(object sender, Colosoft.WebControls.VirtualObjectDataSourceMethodEventArgs e)
        {
            _produto = e.InputParameters[0] as Glass.Global.Negocios.Entidades.Produto;
        }
    
        protected void odsProduto_Inserted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
                throw e.Exception;
            
            else
                ProdutoSalvo(e.ReturnValue as Colosoft.Business.SaveResult);
        }
    
        protected void odsProduto_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
           if (e.Exception != null)
                throw e.Exception;
            
            else
                ProdutoSalvo(e.ReturnValue as Colosoft.Business.SaveResult);
        }
    
        protected void drpGrupoProdEdit_DataBound(object sender, EventArgs e)
        {
            DropDownList drpGrupo = (DropDownList)sender;
            DropDownList drpSubgrupo = dtvProduto.FindControl("drpSubgrupo") as DropDownList;

            var grupoProdutoFluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<Glass.Global.Negocios.IGrupoProdutoFluxo>();

            foreach (var s in grupoProdutoFluxo.ObtemSubgruposProduto(drpGrupo.SelectedValue.StrParaInt()))
                drpSubgrupo.Items.Add(new ListItem(s.Name, s.Id.ToString()));
    
            if (drpSubgrupo.Items.Count > 0 && drpSubgrupo.Items[0].Value != "")
                drpSubgrupo.Items.Insert(0, new ListItem());
        }

        #region Métodos Ajax

        [Ajax.AjaxMethod]
        public string GetProduto(string codInterno)
        {
            return WebGlass.Business.Produto.Fluxo.BuscarEValidar.Ajax.GetProdutoProduto(codInterno);
        }
    
        [Ajax.AjaxMethod]
        public string ExibirProducao(string idGrupo, string idSubgrupo)
        {
            return WebGlass.Business.SubgrupoProd.Fluxo.BuscarEValidar.Ajax.ExibirProducao(idGrupo, idSubgrupo);
        }
    
        [Ajax.AjaxMethod]
        public string IsSubgrupoProducao(string idGrupo, string idSubgrupo)
        {
            return WebGlass.Business.SubgrupoProd.Fluxo.BuscarEValidar.Ajax.IsSubgrupoProducao(idGrupo, idSubgrupo);
        }
    
        [Ajax.AjaxMethod]
        public string ExibirBenef(string idGrupo, string idSubgrupo)
        {
            return WebGlass.Business.SubgrupoProd.Fluxo.BuscarEValidar.Ajax.ExibirBenef(idGrupo, idSubgrupo);
        }
    
        [Ajax.AjaxMethod]
        public string GetSubgrupos(string idGrupo)
        {
            return WebGlass.Business.SubgrupoProd.Fluxo.BuscarEValidar.Ajax.GetSubgrupos(idGrupo);
        }

        [Ajax.AjaxMethod]
        public string ExibirAlturaLargura(string idGrupo, string idSubgrupo)
        {
            return WebGlass.Business.SubgrupoProd.Fluxo.BuscarEValidar.Ajax.ExibirAlturaLargura(idGrupo, idSubgrupo);
        }

        [Ajax.AjaxMethod]
        public string ObrigarAlturaLargura(string idSubgrupo)
        {
            return WebGlass.Business.SubgrupoProd.Fluxo.BuscarEValidar.Ajax.ObrigarAlturaLargura(idSubgrupo);
        }

        [Ajax.AjaxMethod]
        public string ObterTipoSubgrupoPeloSubgrupo(string idSubgrupoStr)
        {
            return WebGlass.Business.SubgrupoProd.Fluxo.BuscarEValidar.Ajax.ObterTipoSubgrupoPeloSubgrupo(idSubgrupoStr);
        }

        #endregion

        #region Beneficiamentos

        protected void ctrlBenef1_Load(object sender, EventArgs e)
        {
            //Esconde a opção de beneficiamento caso for uma inserção de produto
            if (dtvProduto.CurrentMode != DetailsViewMode.Edit)
                dtvProduto.Fields[33].Visible = false;

            if (dtvProduto.CurrentMode == DetailsViewMode.Edit)
                hdfIdPrimeiroProduto.Value = ProdutoDAO.Instance.GetCodInterno(Glass.Conversoes.StrParaInt(Request["idProd"]));
            else              
                hdfIdPrimeiroProduto.Value = ProdutoDAO.Instance.GetFirstProdutoCodInterno();
    
            Glass.UI.Web.Controls.ctrlBenef ctrlBenef = (Glass.UI.Web.Controls.ctrlBenef)sender;
            ctrlBenef.CampoAltura = (TextBox)dtvProduto.FindControl("txtAlturaIns");
            ctrlBenef.CampoLargura = (TextBox)dtvProduto.FindControl("txtLarguraIns");
            ctrlBenef.CampoEspessura = (TextBox)dtvProduto.FindControl("txtEspessura");
            ctrlBenef.CampoProdutoID = hdfIdPrimeiroProduto;
            ctrlBenef.ValidarEspessura = false;
            ctrlBenef.BloquearBeneficiamentos = String.IsNullOrEmpty(hdfIdPrimeiroProduto.Value);
            ctrlBenef.MensagemBloqueioBenef = "Não é possível cadastrar beneficiamentos para o primeiro produto a ser cadastrado. " +
                "Após inserí-lo você poderá colocar os beneficiamentos através da opção de edição do produto.";
    
            if (dtvProduto.FindControl("txtCodInterno") != null && ((TextBox)dtvProduto.FindControl("txtCodInterno")).Text == String.Empty)
                ((TextBox)dtvProduto.FindControl("txtCodInterno")).Text = ProdutoDAO.Instance.GetLastId();
        }
    
        #endregion
    
        protected void ctrlSelCorProd1_Load(object sender, EventArgs e)
        {
            var grupo = dtvProduto.FindControl("drpGrupoProd");
            (sender as Glass.UI.Web.Controls.ctrlSelCorProd).ControleGrupo = grupo;
        }

        protected void tbMarkup_Load(object sender, EventArgs e)
        {
            if (!CompraConfig.AtualizarValorProdutoFinalizarCompraComBaseMarkUp && sender is HtmlTableCell)
                ((HtmlTableCell)sender).Style.Add("display", "none");
        }
    }
}
