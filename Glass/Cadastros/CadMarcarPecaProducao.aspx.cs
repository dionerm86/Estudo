using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Helper;
using Glass.Data.Model;
using Glass.Data.DAL;
using Glass.Configuracoes;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadMarcarPecaProducao : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadMarcarPecaProducao));
    
            if (!PCPConfig.ControlarProducao)
            {
                if (Glass.Configuracoes.ProducaoConfig.TipoControleReposicao == DataSources.TipoReposicaoEnum.Peca)
                {
                    Response.End();
                    return;
                }
    
                Page.Title = "Marcar Perda";
                separador.Visible = false;
                situacao.Visible = false;
                chkPerda.Checked = true;
            }
            else if (Glass.Configuracoes.ProducaoConfig.TipoControleReposicao == DataSources.TipoReposicaoEnum.Peca)
                chkPerda.Style.Add("Display", "none");
    
            if (drpSetor.SelectedValue != "")
            {
                Setor setor = Data.Helper.Utils.ObtemSetor(Glass.Conversoes.StrParaUint(drpSetor.SelectedValue));
                tbRota.Style.Value = "display: " + (setor.InformarRota ? "table" : "none");
                tbChapa.Visible = setor.Corte && PCPConfig.Etiqueta.UsarControleChapaCorte;
                hdfInformarRota.Value = setor.InformarRota.ToString().ToLower();
            }
            else
            {
                tbRota.Style.Value = "display: none";
                hdfInformarRota.Value = "false";
                tbChapa.Visible = false;
            }
    
            if (!IsPostBack)
                txtNumPedido.Focus();
        }
    
        protected void imbPesq_Click(object sender, ImageClickEventArgs e)
        {
            uint idPedido = Glass.Conversoes.StrParaUint(txtNumPedido.Text);
    
            if (!PedidoDAO.Instance.PedidoExists(idPedido))
            {
                Glass.MensagemAlerta.ShowMsg("Não existe nenhum pedido com o número passado.", Page);
                tbSaida.Visible = false;
            }
            else
                tbSaida.Visible = true;
        }
    
        protected void grdProdutos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                /* Chamado 16291.
                 * Uma etiqueta foi marcada no corte com o mesmo plano mais de uma vez em menos de um minuto,
                 * com fila de operações este problema não ocorrerá novamente. */
                FilaOperacoes.AtualizaSituacaoProdutoPedidoProducao.AguardarVez();

                int tipoPerdaOutros = (int)Glass.Data.DAL.TipoPerdaDAO.Instance.GetIDByNomeExato("Outros");
                
                if (e.CommandName == "Marcar")
                {
                    List<RetalhoProducaoAuxiliar> dadosRetalho = ctrlRetalhoProducao1.Dados;
                    if (!RetalhoProducaoDAO.Instance.ValidaRetalhos(dadosRetalho, e.CommandArgument.ToString().Split(';')[0]))
                    {
                        Glass.MensagemAlerta.ShowMsg("Atenção: As dimensões do retalho não podem ser maiores que as da peça.", Page);
                        return;
                    }
    
                    bool informarRota = bool.Parse(hdfInformarRota.Value);
                    uint idRota = informarRota ? Glass.Conversoes.StrParaUint(hdfIdRota.Value) : 0;
                    uint idSetor = Glass.Conversoes.StrParaUint(drpSetor.SelectedValue);
    
                    if (idSetor == 0)
                    {
                        Glass.MensagemAlerta.ShowMsg("Informe o setor.", Page);
                        return;
                    }
    
                    if (idSetor == SetorDAO.Instance.ObtemIdSetorExpCarregamento())
                        throw new Exception("Utilize a tela de expedição de carregamento para marcar a saída dos produtos.");
    
                    if (!chkPerda.Checked)
                        ProdutoPedidoProducaoDAO.Instance.AtualizaSituacaoComTransacao(UserInfo.GetUserInfo.CodUser, txtCodChapa.Text, e.CommandArgument.ToString().Split(';')[0], idSetor, false, 
                            false, null, null, null, null, idRota, null, null, false, null);
                    else
                    {
                        string obsPerda = txtObsPerda.Text;
                        bool retornarEstoque = drpRetornarEstoque.SelectedValue == "1";
    
                        ProdutoPedidoProducaoDAO.Instance.AtualizaSituacaoComTransacao(UserInfo.GetUserInfo.CodUser, txtCodChapa.Text, e.CommandArgument.ToString().Split(';')[0], idSetor, true,
                            retornarEstoque, ctrlTipoPerda1.IdTipoPerda.Value, ctrlTipoPerda1.IdSubtipoPerda, obsPerda, null, idRota, null, null, false, null);
                    }
    
                    if (dadosRetalho.Count > 0)
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "Imprimir", string.Format("imprimirRetalhos('{0}');", e.CommandArgument.ToString().Split(';')[0]), true);
    
                    grdProdutos.PageIndex = 0;
                    grdProdutos.DataBind();
                }
                else if (e.CommandName == "MarcarTodas")
                {
                    List<RetalhoProducaoAuxiliar> dadosRetalho = ctrlRetalhoProducao1.Dados;
    
                    bool informarRota = bool.Parse(hdfInformarRota.Value);
                    uint idRota = informarRota ? Glass.Conversoes.StrParaUint(hdfIdRota.Value) : 0;
                    uint idSetor = Glass.Conversoes.StrParaUint(drpSetor.SelectedValue);
    
                    if (idSetor == 0)
                    {
                        Glass.MensagemAlerta.ShowMsg("Informe o setor.", Page);
                        return;
                    }
    
                    if (idSetor == SetorDAO.Instance.ObtemIdSetorExpCarregamento())
                        throw new Exception("Utilize a tela de expedição de carregamento para marcar a saída dos produtos.");
    
                    if (!chkPerda.Checked)
                    {
                        ProdutoPedidoProducao[] lstProdPed = ((ProdutoPedidoProducao[])odsProdPedProducao.Select());
                        for (int i = 0; i < lstProdPed.Length; i++)
                            try
                            {
                                ProdutoPedidoProducaoDAO.Instance.AtualizaSituacaoComTransacao(UserInfo.GetUserInfo.CodUser, txtCodChapa.Text, lstProdPed[i].NumEtiqueta,
                                    idSetor, false, false, null, null, null, null, idRota, null, null, false, null);
                            }
                            catch (Exception ex)
                            {
                                if (!ex.Message.ToLower().Contains(" perda."))
                                    throw ex;
                            }
                    }
                    else
                    {
                        string obsPerda = txtObsPerda.Text;
                        bool retornarEstoque = drpRetornarEstoque.SelectedValue == "1";
    
                        ProdutoPedidoProducao[] lstProdPed = ((ProdutoPedidoProducao[])odsProdPedProducao.Select());
                        string[] numeroEtiqueta = new string[lstProdPed.Length];
    
                        for (int i = 0; i < lstProdPed.Length; i++)
                        {
                            if (!RetalhoProducaoDAO.Instance.ValidaRetalhos(dadosRetalho, lstProdPed[i].NumEtiqueta))
                            {
                                Glass.MensagemAlerta.ShowMsg("Atenção: As dimensões do retalho não podem ser maiores que as da peça.", Page);
                                return;
                            }
    
                            numeroEtiqueta[i] = lstProdPed[i].NumEtiqueta;
                            ProdutoPedidoProducaoDAO.Instance.AtualizaSituacaoComTransacao(UserInfo.GetUserInfo.CodUser, txtCodChapa.Text, lstProdPed[i].NumEtiqueta, idSetor,
                                true, retornarEstoque, ctrlTipoPerda1.IdTipoPerda.Value, ctrlTipoPerda1.IdSubtipoPerda, obsPerda, null, idRota, null, null, false, null);
                        }
    
                        if (dadosRetalho.Count > 0)
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "Imprimir", string.Format("imprimirRetalhos('{0}');", string.Join(";", numeroEtiqueta)), true);
                    }
    
                    grdProdutos.PageIndex = 0;
                    grdProdutos.DataBind();
                }
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao marcar peça produção.", ex, Page);
            }
            finally
            {
                FilaOperacoes.AtualizaSituacaoProdutoPedidoProducao.ProximoFila();
            }
        }
    
        [Ajax.AjaxMethod()]
        public string GetRota(string codRota)
        {
            return WebGlass.Business.Rota.Fluxo.BuscarEValidar.Ajax.GetRota(codRota);
        }
    
        protected void drpSetor_SelectedIndexChanged(object sender, EventArgs e)
        {
            grdProdutos.DataBind();
        }
    
        protected void drpSetor_DataBound(object sender, EventArgs e)
        {
            if (!drpSetor.Items.Contains(new ListItem()))
                drpSetor.Items.Insert(0, new ListItem());
        }
    
        [Ajax.AjaxMethod()]
        public string ValidaRetalhos(string altura, string largura, string quantidade, string numEtiqueta)
        {
            bool isValid = RetalhoProducaoDAO.Instance.ValidaRetalhos(altura, largura, quantidade, numEtiqueta);
    
            return isValid ? "True" : "False";
        }
    }
}
