using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.DAL;
using Glass.Data.Helper;

namespace Glass.UI.Web.Utils
{
    public partial class SelProdMateriaPrima : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            grdProdMateriaPrima.Register(true, true);
            odsProdMateriaPrima.Register();
        }

        /// <summary>
        /// Instancia do produto que está sendo salvo.
        /// </summary>
        private Glass.Estoque.Negocios.Entidades.ProdutoBaixaEstoque _produto;

        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
        }

        #region Beneficiamentos

        protected void ctrlBenef1_Load(object sender, EventArgs e)
        {
            hdfIdProd.Value = ProdutoDAO.Instance.GetFirstProdutoCodInterno((int)Glass.Data.Model.NomeGrupoProd.Vidro);

            Glass.UI.Web.Controls.ctrlBenef ctrlBenef = (Glass.UI.Web.Controls.ctrlBenef)sender;
            ctrlBenef.CampoProdutoID = hdfIdProd;
        }

        #endregion

        protected void lnkInserir_Click(object sender, EventArgs e)
        {
            var produto = grdProdMateriaPrima.FooterRow.FindControl("ctrlSelProd") as Glass.UI.Web.Controls.ctrlSelProduto;

            var alturaString = ((TextBox)grdProdMateriaPrima.FooterRow.FindControl("txtAltura")).Text; 
            var larguraString = ((TextBox)grdProdMateriaPrima.FooterRow.FindControl("txtLargura")).Text;
            var qtdeString = ((TextBox)grdProdMateriaPrima.FooterRow.FindControl("txtQtde")).Text;
            var formaString = ((TextBox)grdProdMateriaPrima.FooterRow.FindControl("txtForma")).Text;
            var idProcessoString = ((HiddenField)grdProdMateriaPrima.FooterRow.FindControl("hdfIdProcesso")).Value;
            var idAplicacaoString = ((HiddenField)grdProdMateriaPrima.FooterRow.FindControl("hdfIdAplicacao")).Value;

            var prodBaixaEst = new Glass.Estoque.Negocios.Entidades.ProdutoBaixaEstoque();

            prodBaixaEst.IdProd = Request["idProd"].StrParaInt();
            prodBaixaEst.Qtde = qtdeString.StrParaInt();
            prodBaixaEst.IdProdBaixa = produto.IdProd != null ? (int)produto.IdProd.Value : 0;
            prodBaixaEst.Altura = alturaString.StrParaInt();
            prodBaixaEst.Largura = larguraString.StrParaInt();
            prodBaixaEst.Forma = formaString;
            prodBaixaEst.IdProcesso = idProcessoString.StrParaInt();
            prodBaixaEst.IdAplicacao = idAplicacaoString.StrParaInt();

            var controleBenef = ((Glass.UI.Web.Controls.ctrlBenef)grdProdMateriaPrima.FooterRow.FindControl("ctrlBenef1"));

            var benef = new GenericBenefCollection();

            benef.ServicosInfo = hdfBenef.Value;

            if (benef != null)
                foreach (Glass.Global.Negocios.Entidades.ProdutoBaixaEstoqueBenef i in controleBenef.Converter(benef))
                    prodBaixaEst.ProdutoBaixaEstBeneficiamentos.Add(i);

            try
            {
                var resultado = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<Glass.Global.Negocios.IProdutoFluxo>().SalvarProdutoBaixaEstoque(prodBaixaEst);

                _produto = prodBaixaEst;

                if (!resultado)
                    Glass.MensagemAlerta.ShowMsg("Falha ao inserir matéria prima." + resultado.Message, Page);
                else
                {
                    var imagem = (FileUpload)grdProdMateriaPrima.FooterRow.FindControl("filImagem");

                    ProdutoSalvo(imagem);
                }
                grdProdMateriaPrima.DataBind();
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao inserir matéria prima.", ex, Page);
            }
        }

        /// <summary>
        /// Método acionado qaundo o produto for salvo.
        /// </summary>
        /// <param name="saveResult"></param>
        private void ProdutoSalvo(FileUpload imagem)
        {
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
                    .Current.GetInstance<Glass.IProdutoBaixaEstoqueRepositorioImagens>();

                // Salva a imagem do produto
                repositorio.SalvarImagem(_produto.IdProdBaixaEst, imagem.PostedFile.InputStream);
            }
        }

        protected void imbAtualizar_Click(object sender, ImageClickEventArgs e)
        {
            var produto = ((ImageButton)sender).Parent.FindControl("ctrlSelProd") as Glass.UI.Web.Controls.ctrlSelProduto;

            var alturaString = ((TextBox)((ImageButton)sender).Parent.FindControl("txtAltura")).Text; 
            var larguraString = ((TextBox)((ImageButton)sender).Parent.FindControl("txtLargura")).Text;
            var qtdeString = ((TextBox)((ImageButton)sender).Parent.FindControl("txtQtde")).Text;
            var formaString = ((TextBox)((ImageButton)sender).Parent.FindControl("txtForma")).Text;
            var idProcessoString = ((HiddenField)((ImageButton)sender).Parent.FindControl("hdfIdProcesso")).Value;
            var idAplicacaoString = ((HiddenField)((ImageButton)sender).Parent.FindControl("hdfIdAplicacao")).Value;

            var idProdBaixaEst = ((HiddenField)((ImageButton)sender).Parent.FindControl("hdfIdProdBaixaEst")).Value.StrParaInt();

            var prod = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<Glass.Global.Negocios.IProdutoFluxo>().ObtemProduto(Request["idProd"].StrParaInt());

            var prodBaixaEst = prod.BaixasEstoque.Where(f => f.IdProdBaixaEst == idProdBaixaEst).FirstOrDefault();

            prodBaixaEst.IdProd = Request["idProd"].StrParaInt();
            prodBaixaEst.Qtde = qtdeString.StrParaInt();
            prodBaixaEst.IdProdBaixa = produto.IdProd != null ? (int)produto.IdProd.Value : 0;
            prodBaixaEst.Altura = alturaString.StrParaInt();
            prodBaixaEst.Largura = larguraString.StrParaInt();
            prodBaixaEst.Forma = formaString;
            prodBaixaEst.IdProcesso = idProcessoString.StrParaInt();
            prodBaixaEst.IdAplicacao = idAplicacaoString.StrParaInt();

            var controleBenef = ((Glass.UI.Web.Controls.ctrlBenef)((ImageButton)sender).Parent.FindControl("ctrlBenef1"));

            if (prodBaixaEst.ProdutoBaixaEstBeneficiamentos != null)
                prodBaixaEst.ProdutoBaixaEstBeneficiamentos.Clear();

            var benef = new GenericBenefCollection();

            benef.ServicosInfo = hdfBenef.Value;

            if (benef != null)

                if (benef.Count() == 0)
                    prodBaixaEst.ProdutoBaixaEstBeneficiamentos.Clear();

            foreach (Glass.Global.Negocios.Entidades.ProdutoBaixaEstoqueBenef i in controleBenef.Converter(benef))
                prodBaixaEst.ProdutoBaixaEstBeneficiamentos.Add(i);

            try
            {
                var resultado = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<Glass.Global.Negocios.IProdutoFluxo>().SalvarProdutoBaixaEstoque(prodBaixaEst);

                _produto = prodBaixaEst;

                if (!resultado)
                    Glass.MensagemAlerta.ShowMsg("Falha ao inserir matéria prima." + resultado.Message, Page);
                else
                {
                    var imagem = (FileUpload)((ImageButton)sender).Parent.FindControl("filImagem");

                    ProdutoSalvo(imagem);

                    grdProdMateriaPrima.DataBind();
                    grdProdMateriaPrima.EditIndex = -1;
                    grdProdMateriaPrima.ShowFooter = true;
                }
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao inserir matéria prima.", ex, Page);
            }
        }

        protected void grdProdMateriaPrima_RowEditing(object sender, GridViewEditEventArgs e)
        {
            grdProdMateriaPrima.ShowFooter = false;
        }

        protected void imbExcluirImagem_Click(object sender, ImageClickEventArgs e)
        {
            var idProdBaixaEst = ((HiddenField)((ImageButton)sender).Parent.FindControl("hdfIdProdBaixaEst")).Value.StrParaInt();

            var repositorio = Microsoft.Practices.ServiceLocation.ServiceLocator
                   .Current.GetInstance<Glass.IProdutoBaixaEstoqueRepositorioImagens>();

            var caminhoImagem = repositorio.ObtemCaminho(idProdBaixaEst);

            if (!string.IsNullOrEmpty(caminhoImagem))
                System.IO.File.Delete(caminhoImagem);

            grdProdMateriaPrima.DataBind();
            grdProdMateriaPrima.EditIndex = -1;
            grdProdMateriaPrima.ShowFooter = true;
        }
    }
}