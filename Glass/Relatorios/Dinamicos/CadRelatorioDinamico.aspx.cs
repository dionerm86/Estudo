using System;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Relatorios.Dinamicos
{
    public partial class CadRelatorioDinamico : System.Web.UI.Page
    {
        #region Variáveis Locais

        private Glass.Global.UI.Web.Process.RelatorioDinamico.CadastroRelatorioDinamicoFluxo _cadastroRelatorioDinamicoFluxo;

        #endregion

        #region Propriedades

        /// <summary>
        /// Instancia do relatório que está sendo editado.
        /// </summary>
        protected Glass.Global.Negocios.Entidades.RelatorioDinamico RelatorioDinamico
        {
            get { return _cadastroRelatorioDinamicoFluxo.RelatorioDinamico; }
        }

        #endregion

        protected override void OnInit(EventArgs e)
        {
            _cadastroRelatorioDinamicoFluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
               .Current.GetInstance<Glass.Global.UI.Web.Process.RelatorioDinamico.CadastroRelatorioDinamicoFluxo>();

            if (IdRelatorioDinamico() > 0)
                _cadastroRelatorioDinamicoFluxo.IdRelatorioDinamico = IdRelatorioDinamico();

            base.OnInit(e);
            
            odsRelatorioDinamico.ObjectCreating += DataSourceObjectCreating;
            odsFiltro.ObjectCreating += DataSourceObjectCreating;
            odsIcone.ObjectCreating += DataSourceObjectCreating;
            odsRelatorioDinamico.Inserted += odsRelatorioDinamico_Inserted;
            odsRelatorioDinamico.Updated += odsRelatorioDinamico_Updated;

            odsRelatorioDinamico.Register();
            odsFiltro.Register();
            odsIcone.Register();

            dtvRelatorioDinamico.Register();
            grdFiltros.Register(true, true);
            grdIcones.Register(true, true);

            dtvRelatorioDinamico.ItemUpdated += dtvRelatorioDinamico_ItemUpdated;
            dtvRelatorioDinamico.ItemInserted += dtvRelatorioDinamico_ItemInserted;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (dtvRelatorioDinamico.CurrentMode == DetailsViewMode.Insert)
                if (IdRelatorioDinamico() > 0)
                    dtvRelatorioDinamico.ChangeMode(DetailsViewMode.ReadOnly);

            grdFiltros.Visible = dtvRelatorioDinamico.CurrentMode == DetailsViewMode.ReadOnly;
            grdIcones.Visible = dtvRelatorioDinamico.CurrentMode == DetailsViewMode.ReadOnly;
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            Response.Redirect("LstRelatorioDinamico.aspx");
        }

        /// <summary>
        /// Método acionado quando for solicitada a criação do DataSource do Relatório Dinâmico.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataSourceObjectCreating(object sender, Colosoft.WebControls.VirtualObjectDataSourceEventArgs e)
        {
            e.ObjectInstance = _cadastroRelatorioDinamicoFluxo;
        }

        /// <summary>
        /// Método acionado quando o relatório for inserido pelo DetailsView.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dtvRelatorioDinamico_ItemInserted(object sender, DetailsViewInsertedEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao cadastrar relatório dinâmico.", e.Exception, Page);
                e.KeepInInsertMode = true;
            }
            else
                // Redireciona para a visualização
                Response.Redirect(string.Format("CadRelatorioDinamico.aspx?id={0}", RelatorioDinamico.IdRelatorioDinamico));
        }

        /// <summary>
        /// Método acionado quando o relatório for atualizado pelo DetailsView.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dtvRelatorioDinamico_ItemUpdated(object sender, DetailsViewUpdatedEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao atualizar dados do relatório dinâmico.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
            else
                // Redireciona para a visualização
                Response.Redirect(string.Format("CadRelatorioDinamico.aspx?id={0}", RelatorioDinamico.IdRelatorioDinamico));
        }

        /// <summary>
        /// Método acionado quando o relatório dinâmico for inserido.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void odsRelatorioDinamico_Inserted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception == null)
            {
                var relatorioDinamico = _cadastroRelatorioDinamicoFluxo.RelatorioDinamico;
                SalvarArquivoRdlc(_cadastroRelatorioDinamicoFluxo.RelatorioDinamico);

                Response.Redirect("CadRelatorioDinamico.aspx?idRelatorioDinamico=" + relatorioDinamico.IdRelatorioDinamico);
            }
        }

        /// <summary>
        /// Método acionado quando o relatório dinâmico for atualizado.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void odsRelatorioDinamico_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception == null && ((Colosoft.Business.SaveResult)e.ReturnValue).Success)
            {
                var relatorioDinamico = _cadastroRelatorioDinamicoFluxo.RelatorioDinamico;
                SalvarArquivoRdlc(_cadastroRelatorioDinamicoFluxo.RelatorioDinamico);

                Response.Redirect("CadRelatorioDinamico.aspx?idRelatorioDinamico=" + RelatorioDinamico.IdRelatorioDinamico);
            }
        }

        /// <summary>
        /// Salva o relatório
        /// </summary>
        /// <param name="relatorioDinamico"></param>
        protected string ObterCaminhoArquivoRdlc(int idRelatorioDinamico)
        {
            var repositorioArquivos = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<Glass.Global.Negocios.Entidades.IRelatorioDinamicoRepositorioArquivos>();
            
            var url = repositorioArquivos.ObterUrl(idRelatorioDinamico);

            return Server.MapPath(url);
        }

        /// <summary>
        /// Salva o relatório
        /// </summary>
        /// <param name="relatorioDinamico"></param>
        private void SalvarArquivoRdlc(Glass.Global.Negocios.Entidades.RelatorioDinamico relatorioDinamico)
        {
            var repositorioArquivos = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<Glass.Global.Negocios.Entidades.IRelatorioDinamicoRepositorioArquivos>();

            var fluArquivoRdlc = (FileUpload)dtvRelatorioDinamico.FindControl("fluArquivoRdlc");

            // Verifica se o arquivo foi postado
            if (fluArquivoRdlc.HasFile)
                repositorioArquivos.SalvarArquivo(relatorioDinamico.IdRelatorioDinamico, fluArquivoRdlc.FileContent);
        }

        protected void lnkInsIcone_Click(object sender, EventArgs e)
        {
            try
            {
                var icone = _cadastroRelatorioDinamicoFluxo.CriarIcone();
                icone.IdRelatorioDinamico = IdRelatorioDinamico();
                icone.NomeIcone = ((TextBox)grdIcones.FooterRow.FindControl("txtDescricao")).Text;
                icone.FuncaoJavaScript = ((TextBox)grdIcones.FooterRow.FindControl("txtFuncaoJavaScript")).Text;
                icone.MetodoVisibilidade = ((TextBox)grdIcones.FooterRow.FindControl("txtMetodoVisibilidade")).Text;
                icone.Icone = ((FileUpload)grdIcones.FooterRow.FindControl("fluIcone")).FileBytes;
                icone.MostrarFinalGrid = ((CheckBox)grdIcones.FooterRow.FindControl("chkMostrarFinalGrid")).Checked;
                icone.NumSeq = ((TextBox)grdIcones.FooterRow.FindControl("txtNumSeq")).Text.StrParaInt();

                var resultado = _cadastroRelatorioDinamicoFluxo.SalvarIcone(icone);

                if (!resultado)
                {
                    MensagemAlerta.ErrorMsg("Falha ao inserir ícone.", resultado);
                    return;
                }

                grdIcones.DataBind();
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao inserir ícone.", ex, Page);
            }
        }

        protected void lnkInsFiltro_Click(object sender, EventArgs e)
        {
            try
            {
                var filtro = _cadastroRelatorioDinamicoFluxo.CriarFiltro();
                filtro.IdRelatorioDinamico = IdRelatorioDinamico();
                filtro.NomeFiltro = ((TextBox)grdFiltros.FooterRow.FindControl("txtDescricao")).Text;
                filtro.NomeColunaSql = ((TextBox)grdFiltros.FooterRow.FindControl("txtNomeColunaSql")).Text;
                filtro.TipoControle = (Data.Model.TipoControle)Enum.Parse(typeof(Data.Model.TipoControle), ((DropDownList)grdFiltros.FooterRow.FindControl("drpTipoControle")).SelectedValue);
                filtro.Opcoes = ((TextBox)grdFiltros.FooterRow.FindControl("txtOpcoes")).Text;
                filtro.NumSeq = ((TextBox)grdIcones.FooterRow.FindControl("txtNumSeq")).Text.StrParaInt();

                var resultado = _cadastroRelatorioDinamicoFluxo.SalvarFiltro(filtro);

                if (!resultado)
                {
                    MensagemAlerta.ErrorMsg("Falha ao inserir filtro.", resultado);
                    return;
                }
                
                grdFiltros.DataBind();
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao inserir filtro.", ex, Page);
            }
        }

        public int IdRelatorioDinamico()
        {
            return Request["idRelatorioDinamico"].StrParaInt();
        }
    }
}