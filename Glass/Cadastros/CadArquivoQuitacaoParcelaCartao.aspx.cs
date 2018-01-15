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
        private static List<QuitacaoParcelaCartaoPesquisa> _quitacaoParcelaCartaoPesquisa;
        private static Byte[] arquivo;
        private static string nomeArquivo;

        protected void Page_Load(object sender, EventArgs e)
        {
            var idArquivoQuitacaoParcelaCartao = Conversoes.StrParaInt(Request["IdArquivoQuitacaoParcelaCartao"]);

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
                arquivo = fluArquivo.FileBytes;
                nomeArquivo = fluArquivo.FileName;

                var quitacaoParcelaCartaoFluxo = ServiceLocator.Current.GetInstance<IQuitacaoParcelaCartaoFluxo>();

                _quitacaoParcelaCartaoPesquisa = quitacaoParcelaCartaoFluxo.CarregarArquivo(new MemoryStream(arquivo));
                grdQuitacaoParcelaCartao.DataSource = _quitacaoParcelaCartaoPesquisa;
                grdQuitacaoParcelaCartao.DataBind();
                btnImportarArquivo.Visible = _quitacaoParcelaCartaoPesquisa.Any(f => f.Quitada);
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

            // Insere o registro do arquivo e salva o mesmo na pasta
            var stream = new MemoryStream(arquivo);
            var extensao = Path.GetExtension(nomeArquivo);
            var resultado = quitacaoParcelaCartaoFluxo.InserirNovoArquivo(stream, extensao);

            if (!resultado)
            {
                MensagemAlerta.ErrorMsg("Falha ao salvar arquivo.", resultado);
                return;
            }

            var idArquivoQuitacaoParcelaCartao = Conversoes.StrParaInt(resultado.Message.Format());
            // Atualiza o idArquivoQuitacaoParcelaCartao
            if (_quitacaoParcelaCartaoPesquisa != null && _quitacaoParcelaCartaoPesquisa.Count() > 0)
                foreach (var q in _quitacaoParcelaCartaoPesquisa)
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

            btnCancelar_Click(sender, e);
        }

        private List<QuitacaoParcelaCartao> ObterEntidade()
        {
            var quitacao = new QuitacaoParcelaCartao();
            List<QuitacaoParcelaCartao> quitacaoParcelaCartao = new List<QuitacaoParcelaCartao>();
            foreach (var q in _quitacaoParcelaCartaoPesquisa)
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
            Response.Redirect("../Cadastros/CadArquivoQuitacaoParcelaCartao.aspx");
        }
    }
}