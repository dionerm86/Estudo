using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Glass.Data.Model;
using System.IO;
using Glass.Configuracoes;
using System.Linq;

namespace Glass.UI.Web.Utils
{
    public partial class SelImagemPeca : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var idPedido = Request["idPedido"].StrParaUint();

                if (PedidoDAO.Instance.IsMaoDeObra(idPedido))
                {
                    odsPecas.SelectMethod = "GetMaoDeObra";
                    odsPecas.SelectCountMethod = "GetCountMaoDeObra";
                }
                else if(ProdutosPedidoDAO.Instance.TemProdutoLamComposicao(idPedido) && !PedidoEspelhoDAO.Instance.ExisteEspelho(idPedido))
                {
                    odsPecas.SelectMethod = "ObterParaImagemComposicao";
                    odsPecas.SelectCountMethod = "ObterParaImagemCount";
                    odsPecas.TypeName = "Glass.Data.DAL.ProdutosPedidoDAO";
                    odsPecas.SelectParameters.Add("idProdPed", (Request["idProdPed"]));
                }
                else if (Request["tipo"] == "pedido" && Request["pecaAvulsa"] == "True")
                {
                    odsPecas.SelectMethod = "ObterParaImagemPecaAvulsa";
                    odsPecas.SelectCountMethod = "ObterParaImagemPecaAvulsaCount";
                    odsPecas.TypeName = "Glass.Data.DAL.ProdutosPedidoDAO";
                    odsPecas.SelectParameters.Add("idProdPed", (Request["idProdPed"]));
                }
                else if(Request["tipo"] == "pcp" && Request["pecaAvulsa"] == "True")
                {
                    odsPecas.SelectMethod = "ObterParaImagemPecaAvulsa";
                    odsPecas.SelectCountMethod = "ObterParaImagemPecaAvulsaCount";
                    odsPecas.SelectParameters.Add("idProdPed", (Request["idProdPed"]));
                }
            }
        }

        #region Inserir imagens

        protected void btnAplicar_Click(object sender, EventArgs e)
        {
            try
            {
                for (int i = 0; i < grdPecas.Rows.Count; i++)
                {
                    Table tblImagens = (Table)grdPecas.Rows[i].FindControl("tblImagens");

                    for (int j = 0; j < tblImagens.Rows.Count; j++)
                    {
                        int item = Glass.Conversoes.StrParaInt(tblImagens.Rows[j].Cells[0].Attributes["item"]);

                        FileUpload f = (FileUpload)tblImagens.Rows[j].FindControl("flu" + j);
                        if (f == null || !f.HasFile)
                            continue;

                        uint idPecaItemProj = Glass.Conversoes.StrParaUint(tblImagens.Rows[j].Cells[0].Attributes["idPecaItemProj"]);

                        // Garante que a imagem pode ser alterada na peça
                        if (idPecaItemProj > 0 && !UtilsProjeto.PodeAlterarImagemPeca(PecaItemProjetoDAO.Instance.GetElementExt(null, idPecaItemProj, true), item, j + 1, false))
                            continue;

                        string extensao = f.FileName.Substring(f.FileName.LastIndexOf('.'));
                        if (!Arquivos.IsImagem(extensao))
                            throw new Exception("Apenas imagens podem ser selecionadas.");

                        HiddenField h = (HiddenField)grdPecas.Rows[i].FindControl("hdfIdProdPed");
                        var idPedido = Request["idPedido"].StrParaUint();

                        if (Request["tipo"] == "pedido" && !PedidoEspelhoDAO.Instance.ExisteEspelho(idPedido) && (ProdutosPedidoDAO.Instance.TemProdutoLamComposicao(idPedido) || Request["pecaAvulsa"] == "True"))
                        {
                            //var pp = ProdutosPedidoDAO.Instance.GetElementByPrimaryKey(h.Value.StrParaUint());
                            var idProdPed = h.Value.StrParaUint();
                            var urlImagem = ProdutosPedidoDAO.Instance.ObterUrlImagemSalvar(idProdPed);
                            ManipulacaoImagem.SalvarImagem(urlImagem, f.FileBytes);

                            // Cria Log de alteração da Imagem do Produto Pedido
                            LogAlteracaoDAO.Instance.Insert(new LogAlteracao
                            {
                                Tabela = (int)LogAlteracao.TabelaAlteracao.ImagemProdPed,
                                IdRegistroAlt = (int)idProdPed,
                                Campo = "Imagem Produto Pedido",
                                ValorAtual = f.FileName,
                                DataAlt = DateTime.Now,
                                IdFuncAlt = UserInfo.GetUserInfo.CodUser,
                                Referencia = "Imagem do Produto Pedido " + idProdPed,
                                NumEvento = LogAlteracaoDAO.Instance.GetNumEvento(null, LogAlteracao.TabelaAlteracao.ImagemProdPed, (int)idProdPed)
                            });

                            //Glass.MensagemAlerta.ShowMsg("Imagens alteradas.", Page);
                            //Response.Redirect(Request.Url.ToString());
                        }

                        ProdutosPedidoEspelho ppe = ProdutosPedidoEspelhoDAO.Instance.GetElementByPrimaryKey(Glass.Conversoes.StrParaUint(h.Value));

                        if (ppe != null)
                        {
                            ppe.Item = item;
                            ManipulacaoImagem.SalvarImagem(Server.MapPath(ppe.ImagemUrlSalvarItem), f.FileBytes);

                            // Cria Log de alteração da Imagem do Produto Pedido Espelho
                            LogAlteracaoDAO.Instance.Insert(new LogAlteracao
                            {
                                Tabela = (int)LogAlteracao.TabelaAlteracao.ImagemProdPedEsp,
                                IdRegistroAlt = (int)ppe.IdProdPed,
                                Campo = "Imagem Produto Pedido Espelho",
                                ValorAtual = f.FileName,
                                DataAlt = DateTime.Now,
                                IdFuncAlt = UserInfo.GetUserInfo.CodUser,
                                Referencia = "Imagem do Produto Pedido Espelho " + ppe.IdProdPed,
                                NumEvento = LogAlteracaoDAO.Instance.GetNumEvento(null, LogAlteracao.TabelaAlteracao.ImagemProdPedEsp, (int)ppe.IdProdPed)
                            });

                            if (idPecaItemProj > 0)
                                LogAlteracaoDAO.Instance.LogImagemProducao(idPecaItemProj, item.ToString(), "Nova imagem atribuída à peça");
                        }
                    }
                }

                Glass.MensagemAlerta.ShowMsg("Imagens alteradas.", Page);
                Response.Redirect(Request.Url.ToString());
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao salvar imagem.", ex, Page);
            }
        }
    
        #endregion
    
        #region Excluir imagens
    
        protected void imgExcluir_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                TableRow linha = ((ImageButton)sender).Parent.Parent as TableRow;
                HiddenField h = (HiddenField)linha.Parent.Parent.FindControl("hdfIdProdPed");
                var idPedido = Request["idPedido"].StrParaUint();

                if (Request["tipo"] == "pedido" && !PedidoEspelhoDAO.Instance.ExisteEspelho(idPedido) && ProdutosPedidoDAO.Instance.TemProdutoLamComposicao(idPedido))
                {
                    var urlImagem = ProdutosPedidoDAO.Instance.ObterUrlImagemSalvar(h.Value.StrParaUint());
                    if (File.Exists(urlImagem))
                        File.Delete(urlImagem);

                    Response.Redirect(Request.Url.ToString());
                }

                ProdutosPedidoEspelho ppe = ProdutosPedidoEspelhoDAO.Instance.GetElementByPrimaryKey(Glass.Conversoes.StrParaUint(h.Value));
                ppe.Item = Glass.Conversoes.StrParaInt(linha.Cells[0].Attributes["item"]);
                
                string filePath = Server.MapPath(ppe.ImagemUrlSalvarItem);
                if (!File.Exists(filePath))
                {
                    filePath = Server.MapPath(ppe.ImagemUrlSalvar);
                    if (!File.Exists(filePath))
                        return;
                }
    
                File.Delete(filePath);
    
                uint idPecaItemProj = Glass.Conversoes.StrParaUint(linha.Cells[0].Attributes["idPecaItemProj"]);
                if (idPecaItemProj > 0)
                    LogAlteracaoDAO.Instance.LogImagemProducao(idPecaItemProj, ppe.Item.ToString(), "Remoção da imagem atribuída à peça");
    
                Response.Redirect(Request.Url.ToString());
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao excluir imagem.", ex, Page);
            }
        }
    
        protected void imgExcluir_PreRender(object sender, EventArgs e)
        {
            bool visivel = false;
    
            try
            {
                TableRow linha = ((ImageButton)sender).Parent.Parent as TableRow;
                HiddenField h = (HiddenField)linha.Parent.Parent.FindControl("hdfIdProdPed");
                var idPedido = Request["idPedido"].StrParaUint();

                if (Request["tipo"] == "pedido" && !PedidoEspelhoDAO.Instance.ExisteEspelho(idPedido) && ProdutosPedidoDAO.Instance.TemProdutoLamComposicao(idPedido))
                {
                    var urlImagem = ProdutosPedidoDAO.Instance.ObterUrlImagemSalvar(h.Value.StrParaUint());
                    ((ImageButton)sender).Visible = File.Exists(urlImagem);
                    return;
                }

                ProdutosPedidoEspelho ppe = ProdutosPedidoEspelhoDAO.Instance.GetElementByPrimaryKey(Glass.Conversoes.StrParaUint(h.Value));
                ppe.Item = Glass.Conversoes.StrParaInt(linha.Cells[0].Attributes["item"]);
    
                visivel = File.Exists(Server.MapPath(ppe.ImagemUrlSalvarItem));
                if (!visivel)
                    visivel = File.Exists(Server.MapPath(ppe.ImagemUrlSalvar));
            }
            catch { }
    
            ((ImageButton)sender).Visible = visivel;
        }
    
        #endregion
    
        protected void tblImagens_Load(object sender, EventArgs e)
        {
            try
            {
                Table tblImagens = (Table)sender;
                GridViewRow row = tblImagens.Parent.Parent as GridViewRow;
                HiddenField h = (HiddenField)row.FindControl("hdfIdProdPed");
                var idPedido = Request["idPedido"].StrParaUint();

                if (Request["tipo"] == "pedido" && !PedidoEspelhoDAO.Instance.ExisteEspelho(idPedido) && (ProdutosPedidoDAO.Instance.TemProdutoLamComposicao(idPedido) || Request["pecaAvulsa"] == "True"))
                {
                    var pp = !IsPostBack ? row.DataItem as ProdutosPedido : ProdutosPedidoDAO.Instance.GetElement(null, h.Value.StrParaUint(), false, true, false);
                    CriaLinhaTabela(tblImagens, true, "", null, null, pp, string.Empty);
                    return;
                }

                ProdutosPedidoEspelho ppe = !IsPostBack ? row.DataItem as ProdutosPedidoEspelho :
                    ProdutosPedidoEspelhoDAO.Instance.GetElement(Glass.Conversoes.StrParaUint(h.Value), true);

                PecaItemProjeto peca = null;

                if (ppe.IdMaterItemProj > 0)
                {
                    uint? idPecaItemProj = MaterialItemProjetoDAO.Instance.ObtemIdPecaItemProj(null, ppe.IdMaterItemProj.Value);
                    if (idPecaItemProj.GetValueOrDefault() == 0)
                        return;

                    peca = PecaItemProjetoDAO.Instance.GetElementExt(null, idPecaItemProj.Value, true);
                }

                string msgErro = "Item possui arquivo de otimização (mesa de<br/ >corte) gerado. Não é possível alterá-lo.";

                // Mostra imagem apenas se for instalação
                if (peca != null && peca.Tipo == 1)
                {
                    // Controla o número da etiqueta de acordo com a quantidade (1/3)
                    int itemEtiqueta = 1;

                    // Para cada item desta peça. Ex.: 1 e 2 ou 3 e 4
                    foreach (string item in UtilsProjeto.GetItensFromPeca(peca.Item))
                    {
                        ppe.Item = Glass.Conversoes.StrParaInt(item);
                        bool permitirAlterarImagem = UtilsProjeto.PodeAlterarImagemPeca(peca, ppe.Item, itemEtiqueta++, false, ref msgErro);

                        if (permitirAlterarImagem && Request["tipo"] != "pcp")
                        {
                            permitirAlterarImagem = false;
                            msgErro = "Alteração de imagem apenas no PCP";
                        }

                        CriaLinhaTabela(tblImagens, permitirAlterarImagem, item, peca, ppe, null, msgErro);
                    }
                }
                else if (peca == null)
                {
                    string result = ppe.EtiquetasLegenda;
                    msgErro = String.IsNullOrEmpty(ppe.EtiquetasLegenda) ? String.Empty : msgErro;

                    // Verifica se é permitido alterar imagem da peça
                    var situacaoPedidoEspelho = PedidoEspelhoDAO.Instance.ObtemSituacao(ppe.IdPedido);
                    var permitirAlterarImagem = situacaoPedidoEspelho == PedidoEspelho.SituacaoPedido.Finalizado || 
                        Config.PossuiPermissao(Config.FuncaoMenuPCP.AlterarImagemPecaAposImpressao);

                    if (permitirAlterarImagem)
                    {
                        string[] itens = result != null ? result.ToUpper().Replace(" E ", ",").Split(',') : null;
                        itens = itens.Select(f => f.Trim()).ToArray();

                        permitirAlterarImagem = itens != null && itens.Length > 0 && !string.IsNullOrEmpty(itens[0]);

                        // Recupera o setor de marcação
                        Setor setor = SetorDAO.Instance.ObterSetorPorNome("Marcação");

                        // Se for permitido alterar imagem da peça, verifica se o item não tem arquivo sag e se não passou pelo setor de marcação
                        if (setor != null && setor.IdSetor > 0 && permitirAlterarImagem && itens != null && itens.Length > 0)
                            foreach (string etiq in itens)
                                permitirAlterarImagem = permitirAlterarImagem && !EtiquetaArquivoOtimizacaoDAO.Instance.TemArquivoSAG(etiq) &&
                                    !LeituraProducaoDAO.Instance.PassouSetor(etiq, (uint)setor.IdSetor);
                    }
                    else if (PedidoDAO.Instance.IsMaoDeObra(ppe.IdPedido))
                        permitirAlterarImagem = true;
                    else
                        msgErro = String.IsNullOrEmpty(result) ? "Apenas vidros que serão produzidos<br/ >podem ter imagens anexadas." :
                            "Etiqueta já impressa. Não é<br />possível alterar a imagem";

                    CriaLinhaTabela(tblImagens, permitirAlterarImagem, "", null, ppe, null, msgErro);
                }
            }
            catch (Exception ex)
            {
                MensagemAlerta.ErrorMsg("Falha ao exibir imagens.", ex, Page);
                var urlErro = Request.Url.ToString() == null || Request.Url.ToString() == "" ? "Sel Imagem Peça" : Request.Url.ToString();
                ErroDAO.Instance.InserirFromException(urlErro, ex);
            }
        }
    
        private void CriaLinhaTabela(Table tblImagens, bool permitirAlterarImagem, string item, PecaItemProjeto peca, ProdutosPedidoEspelho ppe, ProdutosPedido pp, string msgErro)
        {
            TableRow linha = new TableRow();
            TableCell cabecalho = new TableCell();
            TableCell logPopup = new TableCell();
            TableCell botoes = new TableCell();
            TableCell upload = new TableCell();
            tblImagens.Rows.Add(linha);
            linha.Cells.AddRange(new TableCell[] { cabecalho, logPopup, botoes });


            if (pp != null)
            {
                var ctrl = (Controls.ctrlImagemPopup)LoadControl("~/Controls/ctrlImagemPopup.ascx");
                ctrl.ImageUrl = pp.ImagemUrl;

                botoes.Controls.Add(ctrl);

                ImageButton exc = new ImageButton();
                exc.OnClientClick = "if (!confirm('Deseja excluir a imagem atribuída à peça?')) return false;";
                exc.ImageUrl = "~/Images/ExcluirGrid.gif";
                exc.Click += new ImageClickEventHandler(imgExcluir_Click);
                exc.PreRender += new EventHandler(imgExcluir_PreRender);

                // Exibe o Log de alteração da Imagem do Produto Pedido
                Controls.ctrlLogPopup logPP = (Controls.ctrlLogPopup)LoadControl("~/Controls/ctrlLogPopup.ascx");
                logPP.Tabela = LogAlteracao.TabelaAlteracao.ImagemProdPed;
                logPP.IdRegistro = pp.IdProdPed;
                logPopup.Controls.Add(logPP);

                botoes.Controls.Add(exc);

                FileUpload upl = new FileUpload();
                upl.ID = "flu" + (tblImagens.Rows.Count - 1);
                upload.Controls.Add(upl);
                linha.Cells.Add(upload);

                return;
            }


            TipoArquivoMesaCorte? tipoArquivo = null;

            if (peca != null)
                tipoArquivo = PecaProjetoModeloDAO.Instance.ObtemTipoArquivoMesaCorte(peca.IdPecaProjMod);

            // Caso seja permitido alterar a imagem da peça, é necessário verificar ainda se a peça possui fml associado.
            if (permitirAlterarImagem &&
                (tipoArquivo == null ||
                (tipoArquivo.Value != TipoArquivoMesaCorte.FML &&
                tipoArquivo != TipoArquivoMesaCorte.DXF)))
            {
                if (peca != null)
                {
                    var flags = FlagArqMesaDAO.Instance.ObtemPorPecaProjMod((int) peca.IdPecaProjMod, true);

                    /* Chamado 24392. */
                    if (flags.Count(f => f.Descricao == TipoArquivoMesaCorte.FML.ToString()) == 0 &&
                        flags.Count(f => f.Descricao == TipoArquivoMesaCorte.DXF.ToString()) == 0)
                        linha.Cells.Add(upload);
                }
                else
                    linha.Cells.Add(upload);
            }

            if (!String.IsNullOrEmpty(item))
                cabecalho.Text = "Item " + item;
    
            cabecalho.Attributes.Add("item", item);
            cabecalho.Attributes.Add("idPecaItemProj", peca != null ? peca.IdPecaItemProj.ToString() : String.Empty);
    
            if (peca != null)
            {
                Controls.ctrlLogPopup log = (Controls.ctrlLogPopup)LoadControl("~/Controls/ctrlLogPopup.ascx");
                log.Tabela = LogAlteracao.TabelaAlteracao.ImagemProducao;
                peca.Item = item;
                log.IdRegistro = peca.IdLog;
                logPopup.Controls.Add(log);
            }

            // Exibe o Log de alteração da Imagem do Produto Pedido Espelho
            Controls.ctrlLogPopup logPPE = (Controls.ctrlLogPopup)LoadControl("~/Controls/ctrlLogPopup.ascx");
            logPPE.Tabela = LogAlteracao.TabelaAlteracao.ImagemProdPedEsp;
            logPPE.IdRegistro = ppe.IdProdPed;
            logPopup.Controls.Add(logPPE);

            if (ppe.TemSvgAssociado)
            {
                Controls.ctrlImageCadProject ctrl = (Controls.ctrlImageCadProject)LoadControl("~/Controls/ctrlImageCadProject.ascx");
                ctrl.IdProdPedEsp = (int)ppe.IdProdPed;
                ctrl.DiminuirMedidasPopUp = true;

                ctrl.Legenda = peca != null ? "Ite" + (peca.Item.IndexOf(" ") > -1 ? "ns" : "m") + ": " + peca.Item : string.Empty;
                if (!string.IsNullOrEmpty(ppe.LegendaImagemPeca))
                    ctrl.Legenda = (peca != null ? "<br />" : string.Empty) + ppe.LegendaImagemPeca;

                botoes.Controls.Add(ctrl);
            }
            else
            {

                Controls.ctrlImagemPopup ctrl = (Controls.ctrlImagemPopup)LoadControl("~/Controls/ctrlImagemPopup.ascx");
                ctrl.ImageUrl = ppe.ImagemUrl;

                ctrl.Legenda = peca != null ? "Ite" + (peca.Item.IndexOf(" ") > -1 ? "ns" : "m") + ": " + peca.Item : String.Empty;
                if (!String.IsNullOrEmpty(ppe.LegendaImagemPeca))
                    ctrl.Legenda = (peca != null ? "<br />" : String.Empty) + ppe.LegendaImagemPeca;

                botoes.Controls.Add(ctrl);
            }
    
            if (permitirAlterarImagem)
            {
                ImageButton exc = new ImageButton();
                exc.OnClientClick = "if (!confirm('Deseja excluir a imagem atribuída à peça?')) return false;";
                exc.ImageUrl = "~/Images/ExcluirGrid.gif";
                exc.Click += new ImageClickEventHandler(imgExcluir_Click);
                exc.PreRender += new EventHandler(imgExcluir_PreRender);
    
                botoes.Controls.Add(exc);
    
                // Se o processo da peça for fixo, não permite anexar imagem
                if (ppe.IdProcesso == null || ppe.IdProcesso != ProjetoConfig.Caixilho.ProcessoCaixilho)
                {
                    FileUpload upl = new FileUpload();
                    upl.ID = "flu" + (tblImagens.Rows.Count - 1);
                    upload.Controls.Add(upl);
                }
            }
            else
            {
                if (ppe.IdProcesso == ProjetoConfig.Caixilho.ProcessoCaixilho)
                    msgErro = "Caixilhos não tem imagens associadas";
    
                Label msg = new Label();
                msg.Text = msgErro;
                msg.ForeColor = System.Drawing.Color.Red;
                msg.Style.Value = "display: inline-block; text-align: right; position: relative; left: 6px";
    
                botoes.Controls.Add(msg);
            }
        }
    }
}
