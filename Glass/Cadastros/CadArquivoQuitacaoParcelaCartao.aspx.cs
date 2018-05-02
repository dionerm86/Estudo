using Glass.Financeiro.Negocios;
using Glass.Financeiro.Negocios.Entidades;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadArquivoQuitacaoParcelaCartao : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var idArquivoQuitacaoParcelaCartao = Conversoes.StrParaInt(Request["IdArquivoQuitacaoParcelaCartao"]);

            if (!IsPostBack)
            {
                Session["QuitacaoParcelaCartao"] = null;
                Session["FluArquivoQuitacaoParcelaCartao"] = null;
            }

            // Se for para visualizar informações do arquivo
            if(idArquivoQuitacaoParcelaCartao > 0)
            {
                lblArquivo.Visible = false;
                fluArquivo.Visible = false;
                btnEnviarArquivo.Visible = false;

                var quitacaoParcelaCartaoFluxo = ServiceLocator.Current.GetInstance<IQuitacaoParcelaCartaoFluxo>();

                grdQuitacaoParcelaCartao.DataSource = quitacaoParcelaCartaoFluxo.PesquisarQuitacaoParcelaCartao(idArquivoQuitacaoParcelaCartao);
                grdQuitacaoParcelaCartao.DataBind();
                btnVoltar.Visible = true;
            }
            // Se for para importar novo arquivo
            else
            {
                lblArquivo.Visible = true;
                fluArquivo.Visible = true;
                btnEnviarArquivo.Visible = true;
            }
        }

        protected void grdQuitacaoParcelaCartao_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var corLinha = ((Glass.Financeiro.Negocios.Entidades.QuitacaoParcelaCartaoPesquisa)e.Row.DataItem).CorLinha;
                foreach (TableCell cell in e.Row.Cells)
                    cell.ForeColor = corLinha;
            }
        }

        protected void btnEnviarArquivo_Click(object sender, EventArgs e)
        {
            try
            {
                lblArquivo.Visible = false;
                fluArquivo.Visible = false;
                btnEnviarArquivo.Visible = false;

                Session["FluArquivoQuitacaoParcelaCartao"] = fluArquivo;

                var quitacaoParcelaCartaoFluxo = ServiceLocator.Current.GetInstance<IQuitacaoParcelaCartaoFluxo>();

                var quitacaoParcelaCartaoPesquisa = quitacaoParcelaCartaoFluxo.CarregarArquivo(new MemoryStream(fluArquivo.FileBytes));
                Session["QuitacaoParcelaCartao"] = quitacaoParcelaCartaoPesquisa;
                grdQuitacaoParcelaCartao.DataSource = quitacaoParcelaCartaoPesquisa;
                grdQuitacaoParcelaCartao.DataBind();
                btnImportarArquivo.Visible = quitacaoParcelaCartaoPesquisa.Any(f => f.Quitada);
                btnCancelar.Visible = true;
                if (grdQuitacaoParcelaCartao.Rows.Count > 0)
                    grdQuitacaoParcelaCartao.Columns[0].Visible = false;
            }
            catch (Exception ex)
            {
                MensagemAlerta.ErrorMsg("Falha ao carregar arquivo.", ex, Page);
            }
        }

        protected void btnImportarArquivo_Click(object sender, EventArgs e)
        {
            var quitacaoParcelaCartaoFluxo = ServiceLocator.Current.GetInstance<IQuitacaoParcelaCartaoFluxo>();

            fluArquivo = (System.Web.UI.WebControls.FileUpload)Session["FluArquivoQuitacaoParcelaCartao"];

            // Insere o registro do arquivo e salva o mesmo na pasta
            var stream = new MemoryStream(fluArquivo.FileBytes);
            var extensao = Path.GetExtension(fluArquivo.FileName);
            var resultado = quitacaoParcelaCartaoFluxo.InserirNovoArquivo(stream, extensao);

            if (!resultado)
            {
                MensagemAlerta.ErrorMsg("Falha ao salvar arquivo.", resultado);
                return;
            }

            var idArquivoQuitacaoParcelaCartao = Conversoes.StrParaInt(resultado.Message.Format());

            var quitacaoParcelaCartaoPesquisa = (List<QuitacaoParcelaCartaoPesquisa>)Session["QuitacaoParcelaCartao"];

            // Atualiza o idArquivoQuitacaoParcelaCartao
            if (quitacaoParcelaCartaoPesquisa != null && quitacaoParcelaCartaoPesquisa.Count() > 0)
                foreach (var q in quitacaoParcelaCartaoPesquisa)
                    q.IdArquivoQuitacaoParcelaCartao = idArquivoQuitacaoParcelaCartao;

            try
            {
                var retorno = quitacaoParcelaCartaoFluxo.QuitarParcelas(ObterEntidade());

                if (!retorno)
                {
                    Response.Clear();
                    Response.ClearHeaders();

                    Response.AddHeader("Content-Length", retorno.Message.Format().Length.ToString());
                    Response.ContentType = "text/plain";
                    Response.AppendHeader("content-disposition", "attachment;filename=\"Falhas.txt\"");

                    Response.Write(retorno.Message.Format());
                    Response.End();
                }
                else
                    MensagemAlerta.ShowMsg("Parcelas quitadas com sucesso!", this);
            }
            catch (Exception ex)
            {
                MensagemAlerta.ErrorMsg("Falha ao importar arquivo.", ex, Page);
            }

            Session["QuitacaoParcelaCartao"] = null;
            Session["FluArquivoQuitacaoParcelaCartao"] = null;
            btnCancelar_Click(sender, e);
        }

        private List<QuitacaoParcelaCartao> ObterEntidade()
        {
            var quitacao = new QuitacaoParcelaCartao();
            List<QuitacaoParcelaCartao> quitacaoParcelaCartao = new List<QuitacaoParcelaCartao>();
            var quitacaoParcelaCartaoPesquisa = (List<QuitacaoParcelaCartaoPesquisa>)Session["QuitacaoParcelaCartao"];

            foreach (var q in quitacaoParcelaCartaoPesquisa)
            {
                quitacao = new QuitacaoParcelaCartao();
                quitacao.IdArquivoQuitacaoParcelaCartao = q.IdArquivoQuitacaoParcelaCartao;
                quitacao.NumAutCartao = q.NumAutCartao;
                quitacao.UltimosDigitosCartao = q.UltimosDigitosCartao;
                quitacao.Valor = q.Valor;
                quitacao.Tipo = q.Tipo;
                quitacao.Bandeira = q.Bandeira;
                quitacao.NumParcela = q.NumParcela;
                quitacao.NumParcelaMax = q.NumParcelaMax;
                quitacao.Tarifa = q.Tarifa;
                quitacao.Quitada = q.Quitada;
                quitacao.DataVencimento = q.DataVencimento;
                quitacao.DataCadastro = q.DataCadastro;
                //quitacao.IdUsuarioCadastro

                quitacaoParcelaCartao.Add(quitacao);
            }
            return quitacaoParcelaCartao;
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            Session["QuitacaoParcelaCartao"] = null;
            Session["FluArquivoQuitacaoParcelaCartao"] = null;
            Response.Redirect("../Cadastros/CadArquivoQuitacaoParcelaCartao.aspx");
        }
    }
}