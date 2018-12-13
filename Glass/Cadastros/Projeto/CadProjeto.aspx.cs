using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Helper;
using Glass.Data.Model;
using Glass.Data.DAL;
using Glass.Configuracoes;
using System.Linq;

namespace Glass.UI.Web.Cadastros.Projeto
{
    public partial class CadProjeto : System.Web.UI.Page
    {
        uint idItemProjeto = 0;
        uint idMaterItemProj = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.Projeto.CadProjeto));
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));

            if (!IsPostBack)
            {
                if (dtvProjeto.CurrentMode == DetailsViewMode.Insert)
                {
                    if (Request["idProjeto"] == null)
                    {
                        if (dtvProjeto.FindControl("txtDataCad") != null)
                            ((TextBox)dtvProjeto.FindControl("txtDataCad")).Text = DateTime.Now.ToString("dd/MM/yyyy");
                    }
                    else
                    {
                        hdfIdProjeto.Value = Request["idProjeto"];
                        dtvProjeto.ChangeMode(DetailsViewMode.ReadOnly);

                        CalculaTotalProj();
                    }
                }

                // Verifica se projeto pode ser editado
                if (Request["idProjeto"] != null && !ProjetoDAO.Instance.CanBeEdited(Glass.Conversoes.StrParaUint(Request["idProjeto"])))
                    Response.Redirect("LstProjeto.aspx");
            }

            // Monta o modelo selecionado
            if (!String.IsNullOrEmpty(Request["idProjeto"]) && !String.IsNullOrEmpty(hdfIdItemProjeto.Value))
                MontaModelo(true);

            grdMaterialProjeto.Visible = dtvProjeto.CurrentMode == DetailsViewMode.ReadOnly;

            // Visibilidade de subtitulos e inserir modelo
            bool visib = !String.IsNullOrEmpty(Request["idProjeto"]);
            btnNovoCalculo.Visible = visib;
            lblCalculoProjeto.Visible = visib;
            lblCalculosEfetuados.Visible = visib;
            lblMateriais.Visible = visib;

            // Visibilidade do total do projeto
            lblDescrTotal.Visible = grdItemProjeto.Rows.Count > 0;
            lblTotalProj.Visible = lblDescrTotal.Visible;
            lnkRelatorio.Visible = lblDescrTotal.Visible;
        }

        protected void grdMaterialProjeto_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            grdMaterialProjeto.ShowFooter = e.CommandName != "Edit";
        }

        #region Eventos de Grids

        protected void grdMaterialProjeto_RowDeleted(object sender, GridViewDeletedEventArgs e)
        {
            dtvProjeto.DataBind();
        }

        protected void grdItemProjeto_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            // Editar item_projeto
            if (e.CommandName == "EditarItem")
            {
                LimpaCalculo();
                hdfIdItemProjeto.Value = e.CommandArgument.ToString();
                MontaModelo(true);
            }
        }

        protected void grdMaterialProjeto_PreRender(object sender, EventArgs e)
        {
            string idItemProjeto = hdfIdItemProjeto.Value;

            // Se não houver nenhum produto cadastrado no pedido (e no ambiente passado)
            if (!String.IsNullOrEmpty(idItemProjeto) && MaterialItemProjetoDAO.Instance.GetCountReal(Glass.Conversoes.StrParaUint(idItemProjeto)) == 0)
                grdMaterialProjeto.Rows[0].Visible = false;
            else if (String.IsNullOrEmpty(idItemProjeto))
                grdMaterialProjeto.Visible = false;
        }

        #endregion

        #region Eventos DataSource

        protected void odsProjeto_Inserted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao cadastrar Projeto.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
            else
            {
                hdfIdProjeto.Value = e.ReturnValue.ToString();
                Response.Redirect("CadProjeto.aspx?IdProjeto=" + hdfIdProjeto.Value);
            }
        }

        protected void odsProjeto_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                e.ExceptionHandled = true;
                dtvProjeto.ChangeMode(DetailsViewMode.ReadOnly);
                Glass.MensagemAlerta.ErrorMsg("Falha ao atualizar dados do Projeto.", e.Exception, Page);
            }
            else
                Response.Redirect("CadProjeto.aspx?IdProjeto=" + hdfIdProjeto.Value);
        }

        protected void odsItemProjeto_Deleted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                e.ExceptionHandled = true;
                Glass.MensagemAlerta.ErrorMsg("Falha ao excluir item do projeto.", e.Exception, Page);
            }
            else
            {
                // Verifica se o valor do hiddenfield que contem o id do projeto sendo editado acaba de ser excluído
                if (!String.IsNullOrEmpty(hdfIdItemProjeto.Value) && !ItemProjetoDAO.Instance.Exists(Glass.Conversoes.StrParaUint(hdfIdItemProjeto.Value)))
                    LimpaCalculo();

                CalculaTotalProj();

                dtvProjeto.DataBind();
            }
        }

        protected void odsMaterialItemProjeto_Deleting(object sender, Colosoft.WebControls.VirtualObjectDataSourceMethodEventArgs e)
        {
            idMaterItemProj = ((MaterialItemProjeto)e.InputParameters[0]).IdMaterItemProj;

            idItemProjeto = MaterialItemProjetoDAO.Instance.ObtemIdItemProjeto(idMaterItemProj);
        }

        protected void odsMaterialItemProjeto_Updating(object sender, Colosoft.WebControls.VirtualObjectDataSourceMethodEventArgs e)
        {
            idMaterItemProj = ((MaterialItemProjeto)e.InputParameters[0]).IdMaterItemProj;

            idItemProjeto = MaterialItemProjetoDAO.Instance.ObtemIdItemProjeto(idMaterItemProj);
        }

        protected void odsMaterialItemProjeto_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            try
            {
                ItemProjetoDAO.Instance.UpdateTotalItemProjeto(idItemProjeto);
            }
            catch (Exception ex)
            {
                string msg = ex.InnerException.Message != null ? ex.InnerException.Message : ex.Message;
                MaterialItemProjetoDAO.Instance.Delete(new MaterialItemProjeto { IdMaterItemProj = idMaterItemProj });
                throw new Exception("Falha ao atualizar Valor do Pedido. Erro: " + msg);
            }

            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg(null, e.Exception, Page);
                e.ExceptionHandled = true;
            }
            else
            {
                CalculaTotalItemProj();
                CalculaTotalProj();
                grdItemProjeto.DataBind();
            }
        }

        protected void odsMaterialItemProjeto_Deleted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            try
            {
                ItemProjetoDAO.Instance.UpdateTotalItemProjeto(idItemProjeto);

                uint? idProjeto = ItemProjetoDAO.Instance.GetIdProjeto(idItemProjeto);
                uint? idOrc = ItemProjetoDAO.Instance.GetIdOrcamento(idItemProjeto);

                if (idProjeto > 0)
                    ProjetoDAO.Instance.UpdateTotalProjeto(idProjeto.Value);
                else if (idOrc > 0)
                {
                    var idProd = ProdutosOrcamentoDAO.Instance.ObterIdProdOrcamentoPeloIdItemProjeto(null, (int)idItemProjeto);
                    if (idProd > 0)
                        ProdutosOrcamentoDAO.Instance.UpdateTotaisProdutoOrcamento(null, ProdutosOrcamentoDAO.Instance.GetElementByPrimaryKey(null, (uint)idProd));

                    OrcamentoDAO.Instance.UpdateTotaisOrcamento(null, OrcamentoDAO.Instance.GetElementByPrimaryKey(null, idOrc.Value), false, false);
                }
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg(null, ex, Page);
                e.ExceptionHandled = true;
            }

            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg(null, e.Exception, Page);
                e.ExceptionHandled = true;
            }
            else
            {
                CalculaTotalItemProj();
                CalculaTotalProj();
                grdItemProjeto.DataBind();
            }
        }

        #endregion

        #region Monta o modelo selecionado na tela

        /// <summary>
        /// Monta o modelo selecionado na tela
        /// </summary>
        /// <param name="edit">Identifica se o item_projeto está sendo editado (Já foi informado valores)</param>
        protected void MontaModelo(bool edit)
        {
            // Pega o item que acaba de ser inserido
            ItemProjeto itemProj = ItemProjetoDAO.Instance.GetElement(Glass.Conversoes.StrParaUint(hdfIdItemProjeto.Value));

            // Pega o modelo selecionado
            ProjetoModelo modelo = ProjetoModeloDAO.Instance.GetElementByPrimaryKey(itemProj.IdProjetoModelo);

            // Pega o projeto
            var projeto = ProjetoDAO.Instance.GetElement(Glass.Conversoes.StrParaUint(Request["idProjeto"]));

            #region Busca imagem associada ao modelo escolhido

            dtvImagem.DataBind();
            dtvImagemMini.DataBind();

            // Atualiza o botão de imprimir
            Button btnImprimir = dtvImagem.FindControl("btnImprimir") as Button;
            btnImprimir.OnClientClick = "openWindow(600, 800, '../../Relatorios/Projeto/RelBase.aspx?rel=imagemProjeto&idOrcamento=" + Request["idOrcamento"] + "&idPedido=" + Request["idPedido"] + "&idItemProjeto=" + hdfIdItemProjeto.Value + "'); return false;";
            btnImprimir.Visible = Request["pcp"] != null || ProjetoConfig.TelaCadastroAvulso.ExibirBotaoImprimirProjeto;

            #endregion

            // Monta tabela com Medidas da Área de Instalação
            // Se o código do modelo for OTR01, não monta tabela de medidas
            if (modelo.Codigo != "OTR01")
                UtilsProjeto.CreateTableMedInst(ref tbMedInst, itemProj, modelo, false);

            try
            {
                // Se o código do modelo for OTR01, não monta tabela de peças
                if (modelo.Codigo != "OTR01")
                {
                    if (edit) // Monta a tabela de peças com as medidas já existentes
                        UtilsProjeto.CreateTablePecasItemProjeto(ref tbPecaModelo, itemProj.IdItemProjeto, itemProj, false, false, Request["Parceiro"] == "true");
                    else // Monta a tabela de peças com valores padrões
                        UtilsProjeto.CreateTablePecasModelo(ref tbPecaModelo, null, itemProj, false, false, Request["Parceiro"] == "true");
                }
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ShowMsg(ex.Message, Page);
                return;
            }

            // Mostra o ambiente cadastrado neste item_projeto
            if (txtAmbiente.Text == String.Empty)
                txtAmbiente.Text = itemProj.Ambiente;

            // Mostra tabela para inserir kit se o projeto não for cálculo de vidro apenas, o modelo do item_projeto for do grupo
            // Correr com Kit e não houver nenhum produto do grupo Kit cadastrado nos materiais deste itemProjeto
            tbInsKit.Visible = !projeto.ApenasVidro && (modelo.IdGrupoModelo == (uint)UtilsProjeto.GrupoModelo.CorrerComKit08mm ||
                modelo.IdGrupoModelo == (uint)UtilsProjeto.GrupoModelo.CorrerComKit10mm ||
                MaterialProjetoModeloDAO.Instance.RequerKit(itemProj.IdItemProjeto)) && !MaterialItemProjetoDAO.Instance.ExistsKit(itemProj.IdItemProjeto);

            // Mostra tabela para inserir tubo se não tiver o produto Tubo nos materiais, se o projeto não for cálculo apenas de vidro e
            // o modelo utilizado neste projeto requerer tubo
            tbInsTubo.Visible = !projeto.ApenasVidro && MaterialProjetoModeloDAO.Instance.RequerTubo(itemProj.IdItemProjeto) &&
                !MaterialItemProjetoDAO.Instance.ExistsTubo(itemProj.IdItemProjeto);

            // Visibilidade de campos relacionados ao modelo de projeto
            bool visib = true;
            lblDescrTotal.Visible = visib;
            tbSubtotal.Visible = visib;
            tbAmbiente.Visible = itemProj.EditDeleteVisible && visib;
            lblMateriais.Visible = visib;
            lblMedidas.Visible = itemProj.EditDeleteVisible && modelo.Codigo != "OTR01";
            pchTbTipoInst.Visible = itemProj.EditDeleteVisible && modelo.Codigo != "OTR01";
            tbPecaModelo.Visible = itemProj.EditDeleteVisible && modelo.Codigo != "OTR01";
            lblMedidasInst.Visible = itemProj.EditDeleteVisible && modelo.Codigo != "OTR01";
            btnConfAmbiente.Visible = itemProj.EditDeleteVisible && modelo.Codigo == "OTR01";

            btnNovoCalculoDupl.Visible = visib;
            btnNovoCalculoDupl.Text = btnNovoCalculo.Text + " (" + modelo.Codigo + ")";
            hdfDuplicarCodigo.Value = itemProj.IdProjetoModelo.ToString();
            hdfDuplicarCorVidro.Value = itemProj.IdCorVidro.ToString();
            hdfDuplicarCorAluminio.Value = itemProj.IdCorAluminio.ToString();
            hdfDuplicarCorFerragem.Value = itemProj.IdCorFerragem.ToString();
            hdfDuplicarEspessura.Value = itemProj.EspessuraVidro.ToString();
            hdfDuplicarApenasVidros.Value = itemProj.ApenasVidros.ToString().ToLower();
            hdfDuplicarEspessura.Value = itemProj.EspessuraVidro.ToString().ToLower();
            hdfDuplicarMedidaExata.Value = itemProj.MedidaExata.ToString().ToLower();

            btnCalcMed.Visible = !itemProj.MedidaExata;

            CalculaTotalItemProj();

            // Não retirar o dataBind deste if
            if (!IsPostBack)
            {
                grdItemProjeto.DataBind();
                grdMaterialProjeto.DataBind();
            }
        }

        #endregion

        #region Metodos Ajax

        /// <summary>
        /// Cria um novo Item de Projeto
        /// </summary>
        [Ajax.AjaxMethod()]
        public string NovoItemProjeto(string idProjeto, string idProjetoModelo, string espessuraVidro, string idCorVidro,
            string idCorAluminio, string idCorFerragem, string apenasVidros, string medidaExata)
        {
            try
            {
                var idItemProjeto = ItemProjetoDAO.Instance.NovoItemProjetoVazioComTransacao(idProjeto.StrParaUint(), null, null,
                    null, null, null, idProjetoModelo.StrParaUint(), espessuraVidro.StrParaIntNullable(), idCorVidro.StrParaUint(),
                    idCorAluminio.StrParaUint(), idCorFerragem.StrParaUint(), apenasVidros == "true", medidaExata == "true", false).IdItemProjeto;

                if (idItemProjeto == 0)
                    return "Erro;Falha ao inserir item no projeto. Inserção retornou 0.";

                return "ok;" + idItemProjeto;
            }
            catch (Exception ex)
            {
                return Glass.MensagemAlerta.FormatErrorMsg("Erro;Falha ao criar novo item de projeto. ", ex);
            }
        }

        /// <summary>
        /// Busca o cliente em tempo real
        /// </summary>
        /// <param name="idCli"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public string GetCli(string idCli)
        {
            try
            {
                Cliente cli = ClienteDAO.Instance.GetElementByPrimaryKey(Glass.Conversoes.StrParaUint(idCli));
                IEnumerable<string> motivos;

                if (cli == null || cli.IdCli == 0)
                    return "Erro;Cliente não encontrado.";
                else if (cli.Situacao == (int)SituacaoCliente.Inativo)
                    return "Erro;Cliente inativo. Motivo: " + cli.Obs;
                else if (cli.Situacao == (int)SituacaoCliente.Cancelado)
                    return "Erro;Cliente cancelado. Motivo: " + cli.Obs;
                else if (cli.Situacao == (int)SituacaoCliente.Bloqueado)
                    return "Erro;Cliente bloqueado. Motivo: " + cli.Obs;

                else if (Glass.Data.GerenciadorSituacaoCliente.Gerenciador.VerificarBloqueio(null, cli, out motivos))
                {
                    return $"Erro;Cliente bloqueado. Motivo: {string.Join(";", motivos)}";
                }
                else
                    return "Ok;" + cli.Nome + ";" + cli.Revenda.ToString().ToLower();
            }
            catch
            {
                return "Erro;Cliente não encontrado.";
            }
        }

        /// <summary>
        /// Retorna o Código/Descrição do produto
        /// </summary>
        [Ajax.AjaxMethod()]
        public string GetProduto(string idProjeto, string codInterno, string tipoEntrega, string revenda, string idCliente)
        {
            Produto prod = ProdutoDAO.Instance.GetByCodInterno(codInterno);

            if (prod == null)
                return "Erro;Não existe produto com o código informado.";
            else if (prod.Situacao == Glass.Situacao.Inativo)
                return "Erro;Produto inativo." + (!String.IsNullOrEmpty(prod.Obs) ? " Obs: " + prod.Obs : "");
            else
            {
                /* Chamado 47194. */
                if (idProjeto.StrParaIntNullable() > 0 && prod.IdSubgrupoProd > 0)
                {
                    var idLojaProjeto = ProjetoDAO.Instance.ObterIdLoja(null, idProjeto.StrParaInt());

                    var idsLojaSubgrupoProd = SubgrupoProdDAO.Instance.ObterIdsLoja(null, prod.IdSubgrupoProd.Value);

                    if (idLojaProjeto > 0 && idsLojaSubgrupoProd.Any() && !idsLojaSubgrupoProd.Any(f => f == idLojaProjeto))
                        return "Erro;Esse produto não pode ser utilizado, pois, as lojas do seu subgrupo são diferentes da loja do pedido/projeto.";
                    /* Chamado 48322. */
                    else if (idLojaProjeto == 0 && idsLojaSubgrupoProd.Any())
                        ProjetoDAO.Instance.AtualizarIdLojaProjeto(null, idProjeto.StrParaInt(), (int)idsLojaSubgrupoProd.First());
                }

                string retorno = "Prod;" + prod.IdProd + ";" + prod.Descricao;

                // Recupera o valor de tabela do produto
                int? tipoEntr = !String.IsNullOrEmpty(tipoEntrega) ? (int?)Glass.Conversoes.StrParaInt(tipoEntrega) : null;
                uint? idCli = !String.IsNullOrEmpty(idCliente) ? (uint?)Glass.Conversoes.StrParaUint(idCliente) : null;
                retorno += ";" + ProdutoDAO.Instance.GetValorTabela(prod.IdProd, tipoEntr, idCli, false, false, 0, null, idProjeto.StrParaIntNullable(), null).ToString("F2");

                retorno += ";" + Glass.Data.DAL.GrupoProdDAO.Instance.IsVidro(prod.IdGrupoProd).ToString().ToLower() + ";" +
                    Glass.Data.DAL.GrupoProdDAO.Instance.IsAluminio(prod.IdGrupoProd).ToString().ToLower() + ";" +
                    (prod.AtivarAreaMinima ? prod.AreaMinima.ToString().Replace(',', '.') : "0");

                retorno += ";" + Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo(null, prod.IdGrupoProd, prod.IdSubgrupoProd, false);
                retorno += ";" + prod.Espessura;
                retorno += ";" + prod.Forma;
                retorno += ";" + prod.CustoCompra;
                retorno += ";" + prod.Altura;
                retorno += ";" + prod.Largura;
                retorno += ";" + (prod.IdProcesso.GetValueOrDefault() > 0 ? EtiquetaProcessoDAO.Instance.GetCodInternoByIds(prod.IdProcesso.GetValueOrDefault().ToString()) : "");
                retorno += ";" + (prod.IdAplicacao.GetValueOrDefault() > 0 ? EtiquetaAplicacaoDAO.Instance.GetCodInternoByIds(prod.IdAplicacao.GetValueOrDefault().ToString()) : "");
                return retorno;
            }
        }

        [Ajax.AjaxMethod()]
        public string GetVidro(string idProjeto, string codInterno)
        {
            Produto prod = ProdutoDAO.Instance.GetByCodInterno(codInterno);

            if (prod == null)
                return "Erro;Não existe produto com o código informado.";
            else if (prod.Situacao == Situacao.Inativo)
                return "Erro;Produto inativo." + (!string.IsNullOrEmpty(prod.Obs) ? " Obs: " + prod.Obs : "");

            /* Chamado 47194. */
            if (idProjeto.StrParaIntNullable() > 0 && prod.IdSubgrupoProd > 0)
            {
                var idLojaProjeto = ProjetoDAO.Instance.ObterIdLoja(null, idProjeto.StrParaInt());

                var idsLojaSubgrupoProd = SubgrupoProdDAO.Instance.ObterIdsLoja(null, prod.IdSubgrupoProd.Value);

                if (idLojaProjeto > 0 && idsLojaSubgrupoProd.Any() && !idsLojaSubgrupoProd.Any(f => f == idLojaProjeto))
                    return "Erro;Esse produto não pode ser utilizado, pois, as lojas do seu subgrupo são diferentes da loja do pedido/projeto.";
                /* Chamado 48322. */
                else if (idLojaProjeto == 0 && idsLojaSubgrupoProd.Any())
                    ProjetoDAO.Instance.AtualizarIdLojaProjeto(null, idProjeto.StrParaInt(), (int)idsLojaSubgrupoProd.First());
            }

            return "Prod;" + prod.IdProd + ";" + prod.Descricao;
        }

        /// <summary>
        /// Verifica se o orçamento passado existe e se está em aberto
        /// </summary>
        /// <param name="idOrcamento"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public string ExistsOrcamentoEmAberto(string idOrcamentoStr)
        {
            uint idOrcamento = !String.IsNullOrEmpty(idOrcamentoStr) ? Glass.Conversoes.StrParaUint(idOrcamentoStr) : 0;

            return OrcamentoDAO.Instance.ExistsOrcamentoEmAberto(null, (int?)idOrcamento).ToString().ToLower();
        }

        [Ajax.AjaxMethod()]
        public string SalvaObsItemProjeto(string idItemProjeto, string obs)
        {
            try
            {
                ItemProjetoDAO.Instance.AtualizaObs(Glass.Conversoes.StrParaUint(idItemProjeto), obs);

                return "ok";
            }
            catch (Exception ex)
            {
                return "Erro\t" + Glass.MensagemAlerta.FormatErrorMsg("Falha ao salvar observação.", ex);
            }
        }

        /// <summary>
        /// Verifica se o item projeto está conferido
        /// </summary>
        [Ajax.AjaxMethod()]
        public string EstaConferido(string idItemProjeto)
        {
            return ItemProjetoDAO.Instance.EstaConferido(Glass.Conversoes.StrParaUint(idItemProjeto)).ToString().ToLower();
        }

        [Ajax.AjaxMethod()]
        public string CriarProjetoCADProject(string codProjeto, string idPecaItemProjeto, string url, string pcp)
        {
            return UtilsProjeto.CriarProjetoCADProject(codProjeto, idPecaItemProjeto.StrParaUint(), url, bool.Parse(pcp));
        }

        #endregion

        #region Cancelar edição de projeto

        protected void btnCancelarEdit_Click(object sender, EventArgs e)
        {
            hdfIdProjeto.Value = Request["idProjeto"];

            dtvProjeto.ChangeMode(DetailsViewMode.ReadOnly);

            //lnkProduto.Visible = dtvProjeto.CurrentMode == DetailsViewMode.ReadOnly;
            //grdProdutos.Visible = lnkProduto.Visible;
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            Response.Redirect("LstProjeto.aspx");
        }

        #endregion

        #region Editar Projeto

        protected void btnEditar_Click(object sender, EventArgs e)
        {
            //lnkProduto.Visible = false;
            //grdProdutos.Visible = false;
        }

        #endregion

        #region Insere MaterialItemProjeto

        protected void lnkInsProd_Click(object sender, EventArgs e)
        {
            if (grdMaterialProjeto.PageCount > 1)
                grdMaterialProjeto.PageIndex = grdMaterialProjeto.PageCount - 1;

            Glass.UI.Web.Controls.ctrlBenef benef = (Glass.UI.Web.Controls.ctrlBenef)grdMaterialProjeto.FooterRow.FindControl("ctrlBenefInserir");

            uint idItemProjeto = Glass.Conversoes.StrParaUint(hdfIdItemProjeto.Value);
            int idProd = ProdutoDAO.Instance.GetByCodInterno(((TextBox)grdMaterialProjeto.FooterRow.FindControl("txtCodProdIns")).Text).IdProd;
            string alturaString = ((TextBox)grdMaterialProjeto.FooterRow.FindControl("txtAlturaIns")).Text;
            string alturaCalcString = ((HiddenField)grdMaterialProjeto.FooterRow.FindControl("hdfAlturaCalcIns")).Value;
            string larguraString = ((TextBox)grdMaterialProjeto.FooterRow.FindControl("txtLarguraIns")).Text;
            Single altura = !String.IsNullOrEmpty(alturaString) ? Single.Parse(alturaString, System.Globalization.NumberStyles.AllowDecimalPoint) : 0;
            Single alturaCalc = !String.IsNullOrEmpty(alturaCalcString) ? Single.Parse(alturaCalcString, System.Globalization.NumberStyles.AllowDecimalPoint) : 0;
            int largura = !String.IsNullOrEmpty(larguraString) ? Glass.Conversoes.StrParaInt(larguraString) : 0;
            string idProcessoStr = ((HiddenField)grdMaterialProjeto.FooterRow.FindControl("hdfIdProcesso")).Value;
            string idAplicacaoStr = ((HiddenField)grdMaterialProjeto.FooterRow.FindControl("hdfIdAplicacao")).Value;
            string espessura = ((TextBox)grdMaterialProjeto.FooterRow.FindControl("txtEspessura")).Text;

            // Cria uma instância do ProdutosPedido
            MaterialItemProjeto materItem = new MaterialItemProjeto();
            materItem.IdItemProjeto = idItemProjeto;
            materItem.Qtde = Glass.Conversoes.StrParaInt(((TextBox)grdMaterialProjeto.FooterRow.FindControl("txtQtdeIns")).Text);
            materItem.Valor = Glass.Conversoes.StrParaDecimal(((TextBox)grdMaterialProjeto.FooterRow.FindControl("txtValorIns")).Text);
            materItem.Altura = altura;
            materItem.AlturaCalc = alturaCalc;
            materItem.Largura = largura;
            materItem.IdProd = (uint)idProd;
            materItem.Espessura = Glass.Conversoes.StrParaFloat(espessura);
            if (!String.IsNullOrEmpty(idAplicacaoStr)) materItem.IdAplicacao = Glass.Conversoes.StrParaUint(idAplicacaoStr);
            if (!String.IsNullOrEmpty(idProcessoStr)) materItem.IdProcesso = Glass.Conversoes.StrParaUint(idProcessoStr);
            materItem.Beneficiamentos = benef.Beneficiamentos;
            materItem.PedCli = ((TextBox)grdMaterialProjeto.FooterRow.FindControl("txtPedCli")).Text;

            // Verifica se o produto com a largura e altura especificados já foi adicionado ao pedido
            if (MaterialItemProjetoDAO.Instance.ExistsInItemProjeto(idItemProjeto, (uint)idProd, altura, largura, materItem.IdProcesso, materItem.IdAplicacao))
            {
                Glass.MensagemAlerta.ShowMsg("Este produto já foi inserido.", Page);
                return;
            }

            try
            {
                #region insere Material Item Projeto

                MaterialItemProjetoDAO.Instance.Insert(materItem);

                try
                {
                    ItemProjetoDAO.Instance.UpdateTotalItemProjeto(materItem.IdItemProjeto);
                }
                catch (Exception ex)
                {
                    string msg = ex.InnerException.Message != null ? ex.InnerException.Message : ex.Message;
                    MaterialItemProjetoDAO.Instance.Delete(materItem);
                    throw new Exception("Falha ao atualizar Valor do Item do Projeto. Erro: " + msg);
                }

                #endregion

                grdMaterialProjeto.DataBind();
                grdItemProjeto.DataBind();
                dtvProjeto.DataBind();

                CalculaTotalItemProj();
                CalculaTotalProj();
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao incluir produto.", ex, Page);
                return;
            }
        }

        #endregion

        #region Gerar Pedido/Orçamento

        protected void btnGerarPedido_Click(object sender, EventArgs e)
        {
            try
            {
                bool apenasVidros = false;
                CheckBox check = dtvProjeto.FindControl("chkApenasVidros") as CheckBox;
                if (check != null)
                    apenasVidros = check.Checked;

                uint idPedido = ProjetoDAO.Instance.GerarPedido(Glass.Conversoes.StrParaUint(Request["idProjeto"]), apenasVidros, null, false);

                Response.Redirect("~/Cadastros/CadPedido.aspx?idPedido=" + idPedido);
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao gerar pedido.", ex, Page);
            }
        }

        protected void btnGerarOrcamento_Click(object sender, EventArgs e)
        {
            try
            {
                // Gera um orçamento a partir deste projeto
                uint idOrcamento = OrcamentoDAO.Instance.GerarOrcamento(Request["idProjeto"].StrParaUint(), false);

                if (idOrcamento > 0)
                    Response.Redirect("~/Cadastros/CadOrcamento.aspx?idOrca=" + idOrcamento);
                else
                    Glass.MensagemAlerta.ShowMsg("Falha ao gerar Orçamento. Inserção retornou 0(zero).", Page);
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao gerar orçamento.", ex, Page);
            }
        }

        #endregion

        #region Efetua/Confirma cálculo de projeto

        /// <summary>
        /// Calcula Medidas
        /// </summary>
        protected void btnCalcMed_Click(object sender, EventArgs e)
        {
            var projeto = ProjetoDAO.Instance.GetElement(Glass.Conversoes.StrParaUint(Request["idProjeto"]));

            uint idItemProjeto = Glass.Conversoes.StrParaUint(hdfIdItemProjeto.Value);

            // Busca o grupo deste modelo
            uint idGrupoModelo = ItemProjetoDAO.Instance.GetIdGrupoModelo(idItemProjeto);

            #region Verifica/Insere Tubo e Kit

            // Se o projeto não for cálculo de vidro apenas, o modelo do item_projeto for do grupo Correr com Kit Instalação e
            // não houver nenhum produto do grupo Kit cadastrado nos materiais deste itemProjeto, obriga a cadastrar um produto do grupo Kit
            if (!projeto.ApenasVidro && !ItemProjetoDAO.Instance.ApenasVidros(idItemProjeto) &&
                (idGrupoModelo == (uint)UtilsProjeto.GrupoModelo.CorrerComKit08mm ||
                idGrupoModelo == (uint)UtilsProjeto.GrupoModelo.CorrerComKit10mm ||
                MaterialProjetoModeloDAO.Instance.RequerKit(idItemProjeto)) && !MaterialItemProjetoDAO.Instance.ExistsKit(idItemProjeto))
            {
                // Se o kit tiver sido informado, insere
                if (!String.IsNullOrEmpty(hdfIdKit.Value))
                {
                    // Verifica se todos os dados foram preenchidos
                    if (String.IsNullOrEmpty(txtQtdeKit.Text) || String.IsNullOrEmpty(txtValorKit.Text))
                    {
                        Glass.MensagemAlerta.ShowMsg("Informe todos os dados do kit requerido neste projeto.", Page);
                        return;
                    }

                    // Verifica se o valor do kit está acima do valor mínimo
                    if (!String.IsNullOrEmpty(hdfKitValMin.Value) && Single.Parse(hdfKitValMin.Value) > Single.Parse(txtValorKit.Text))
                    {
                        Glass.MensagemAlerta.ShowMsg("O valor do kit está abaixo do valor mínimo.", Page);
                        return;
                    }

                    MaterialItemProjeto mater = new MaterialItemProjeto();
                    mater.IdItemProjeto = idItemProjeto;
                    mater.IdProd = Glass.Conversoes.StrParaUint(hdfIdKit.Value);
                    mater.Qtde = Glass.Conversoes.StrParaInt(txtQtdeKit.Text);
                    mater.Valor = Glass.Conversoes.StrParaDecimal(txtValorKit.Text);

                    #region insere Material Item Projeto

                    MaterialItemProjetoDAO.Instance.Insert(mater);

                    try
                    {
                        ItemProjetoDAO.Instance.UpdateTotalItemProjeto(mater.IdItemProjeto);
                    }
                    catch (Exception ex)
                    {
                        string msg = ex.InnerException.Message != null ? ex.InnerException.Message : ex.Message;
                        MaterialItemProjetoDAO.Instance.Delete(mater);
                        throw new Exception("Falha ao atualizar Valor do Item do Projeto. Erro: " + msg);
                    }

                    #endregion

                    txtCodKit.Text = String.Empty;
                    hdfIdKit.Value = String.Empty;
                    txtQtdeKit.Text = String.Empty;
                    txtValorKit.Text = String.Empty;

                    tbInsKit.Visible = false;
                }
                else
                {
                    Glass.MensagemAlerta.ShowMsg("Insira o material Kit requerido neste projeto.", Page);
                    return;
                }
            }
            else
                tbInsKit.Visible = false;

            // Verifica se foi cadastrado o produto Tubo nos materiais, se o projeto não for cálculo apenas de vidro e
            // o modelo utilizado neste projeto requerer tubo
            if (!projeto.ApenasVidro && !ItemProjetoDAO.Instance.ApenasVidros(idItemProjeto) &&
                MaterialProjetoModeloDAO.Instance.RequerTubo(idItemProjeto) && !MaterialItemProjetoDAO.Instance.ExistsTubo(idItemProjeto))
            {
                // Se o tubo tiver sido informado, insere
                if (!String.IsNullOrEmpty(hdfIdTubo.Value))
                {
                    // Verifica se todos os dados foram preenchidos
                    if (String.IsNullOrEmpty(txtQtdeTubo.Text) || String.IsNullOrEmpty(txtValorTubo.Text) || String.IsNullOrEmpty(txtComprTubo.Text))
                    {
                        Glass.MensagemAlerta.ShowMsg("Informe todos os dados do tubo requerido neste projeto.", Page);
                        return;
                    }

                    // Verifica se o valor do tubo está acima do valor mínimo
                    if (!String.IsNullOrEmpty(hdfTuboValMin.Value) && Single.Parse(hdfTuboValMin.Value) > Single.Parse(txtValorTubo.Text))
                    {
                        Glass.MensagemAlerta.ShowMsg("O valor do tubo está abaixo do valor mínimo.", Page);
                        return;
                    }

                    MaterialItemProjeto mater = new MaterialItemProjeto();
                    mater.IdItemProjeto = idItemProjeto;
                    mater.IdProd = Glass.Conversoes.StrParaUint(hdfIdTubo.Value);
                    mater.Qtde = Glass.Conversoes.StrParaInt(txtQtdeTubo.Text);
                    mater.Valor = Glass.Conversoes.StrParaDecimal(txtValorTubo.Text);
                    mater.Altura = Glass.Conversoes.StrParaFloat(txtComprTubo.Text);

                    #region insere Material Item Projeto

                    MaterialItemProjetoDAO.Instance.Insert(mater);

                    try
                    {
                        ItemProjetoDAO.Instance.UpdateTotalItemProjeto(mater.IdItemProjeto);
                    }
                    catch (Exception ex)
                    {
                        string msg = ex.InnerException.Message != null ? ex.InnerException.Message : ex.Message;
                        MaterialItemProjetoDAO.Instance.Delete(mater);
                        throw new Exception("Falha ao atualizar Valor do Item do Projeto. Erro: " + msg);
                    }

                    #endregion

                    txtCodTubo.Text = String.Empty;
                    hdfIdTubo.Value = String.Empty;
                    txtQtdeTubo.Text = String.Empty;
                    txtComprTubo.Text = String.Empty;
                    txtValorTubo.Text = String.Empty;

                    tbInsTubo.Visible = false;
                }
                else
                {
                    Glass.MensagemAlerta.ShowMsg("Insira o material Tubo requerido neste projeto.", Page);
                    return;
                }
            }
            else
                tbInsTubo.Visible = false;

            #endregion

            var retornoValidacao = string.Empty;
            var erroAoCalcularMedidas = false;

            try
            {
                // Busca o item_projeto sendo calculado
                ItemProjeto itemProjeto = ItemProjetoDAO.Instance.GetElement(idItemProjeto);

                /* Chamado 50798.
                 * Caso as medidas do projeto sejam calculadas, todas as edições devem ser desfeitas, mesmo que nenhuma medida tenha sido alterada. */
                if (itemProjeto.IdPedido > 0)
                    ProdutosPedidoDAO.Instance.AtualizarEdicaoImagemPecaArquivoMarcacao(null, (int)itemProjeto.IdItemProjeto);
                else if (itemProjeto.IdPedidoEspelho > 0)
                    ProdutosPedidoEspelhoDAO.Instance.AtualizarEdicaoImagemPecaArquivoMarcacao(null, (int)itemProjeto.IdItemProjeto);

                // Verifica se as peças foram alteradas
                HiddenField hdfPecasAlteradas = (HiddenField)tbPecaModelo.FindControl("hdfPecasAlteradas");
                HiddenField hdfMedidasAlteradas = (HiddenField)tbPecaModelo.FindControl("hdfMedidasAlteradas");
                var pecasAlteradas = hdfPecasAlteradas != null && bool.Parse(hdfPecasAlteradas.Value);
                var medidasAlteradas = hdfMedidasAlteradas != null && bool.Parse(hdfMedidasAlteradas.Value);

                /* Chamado 50798. */
                if (hdfPecasAlteradas != null)
                    hdfPecasAlteradas.Value = "false";

                /* Chamado 50798. */
                if (hdfMedidasAlteradas != null)
                    hdfMedidasAlteradas.Value = "false";

                var modelo = ProjetoModeloDAO.Instance.GetElementByPrimaryKey(itemProjeto.IdProjetoModelo);

                // Calcula as medidas das peças, retornando lista
                var lstPecaModelo = UtilsProjeto.CalcularMedidasPecasComBaseNaTelaComTransacao(modelo, itemProjeto, tbMedInst, tbPecaModelo, true, Request["pcp"] == "1", false, out retornoValidacao);

                // Insere Peças na tabela peca_item_projeto
                PecaItemProjetoDAO.Instance.InsertFromPecaModelo(itemProjeto, ref lstPecaModelo);

                // Insere as peças de vidro apenas se todas as Peça Projeto Modelo tiver idProd
                var inserirPecasVidro = !lstPecaModelo.Any(f => f.IdProd == 0);
                if (inserirPecasVidro)
                    // Insere Peças na tabela material_item_projeto
                    MaterialItemProjetoDAO.Instance.InserePecasVidro(null, projeto.IdCliente, projeto.TipoEntrega, itemProjeto, modelo, lstPecaModelo);

                // Atualiza qtds dos materiais apenas se o projeto não for cálculo apenas de vidros
                if (!projeto.ApenasVidro)
                    MaterialItemProjetoDAO.Instance.AtualizaQtd(null, projeto.IdCliente, projeto.TipoEntrega, itemProjeto, modelo);

                #region Update Total Item Projeto

                ItemProjetoDAO.Instance.UpdateTotalItemProjeto(idItemProjeto);

                uint? idProjeto = ItemProjetoDAO.Instance.GetIdProjeto(idItemProjeto);
                uint? idOrcamento = ItemProjetoDAO.Instance.GetIdOrcamento(idItemProjeto);

                if (idProjeto > 0)
                    ProjetoDAO.Instance.UpdateTotalProjeto(idProjeto.Value);
                else if (idOrcamento > 0)
                {
                    var idProd = ProdutosOrcamentoDAO.Instance.ObterIdProdOrcamentoPeloIdItemProjeto(null, (int)idItemProjeto);
                    if (idProd > 0)
                        ProdutosOrcamentoDAO.Instance.UpdateTotaisProdutoOrcamento(null, ProdutosOrcamentoDAO.Instance.GetElementByPrimaryKey(null, (uint)idProd));

                    OrcamentoDAO.Instance.UpdateTotaisOrcamento(null, OrcamentoDAO.Instance.GetElementByPrimaryKey(null, idOrcamento.Value), false, false);
                }

                #endregion

                // Monta tabela de peças
                UtilsProjeto.CreateTablePecasItemProjeto(ref tbPecaModelo, itemProjeto.IdItemProjeto, itemProjeto, false, false, Request["Parceiro"] == "true");

                // Mostra o total deste item e do projeto
                CalculaTotalItemProj();
                CalculaTotalProj();

                grdMaterialProjeto.DataBind();
                grdItemProjeto.DataBind();

                dtvProjeto.DataBind();

                // Atualiza mensagem associada, podendo ter sido desenhado alguma medida na mesma
                dtvImagem.DataBind();
                Page.ClientScript.RegisterStartupScript(GetType(), "focoCalcular", "document.getElementById('" + btnCalcMed.ClientID + "').focus();", true);
            }
            catch (Exception ex)
            {
                erroAoCalcularMedidas = true;

                Glass.MensagemAlerta.ErrorMsg("Falha ao calcular medidas do projeto.\n" + retornoValidacao, ex, Page);
            }

            if (!erroAoCalcularMedidas &&
                !string.IsNullOrEmpty(retornoValidacao))
                MensagemAlerta.ShowMsg(retornoValidacao, Page);
        }

        /// <summary>
        /// Confirma cálculo
        /// </summary>
        protected void btnConfCalc_Click(object sender, EventArgs e)
        {
            var retornoValidacao = string.Empty;

            try
            {
                if (!String.IsNullOrEmpty(hdfIdItemProjeto.Value))
                {
                    uint idItemProjeto = Glass.Conversoes.StrParaUint(hdfIdItemProjeto.Value);

                    // Busca o item_projeto sendo calculado
                    ItemProjeto itemProjeto = ItemProjetoDAO.Instance.GetElement(idItemProjeto);
                    ProjetoModelo modelo = ProjetoModeloDAO.Instance.GetElementByPrimaryKey(itemProjeto.IdProjetoModelo);

                    var medidasAlteradas = false;

                    if (!itemProjeto.MedidaExata)
                        medidasAlteradas = UtilsProjeto.VerificaMedidasAreaInstalacaoAlteradas(null, itemProjeto, modelo, tbMedInst, tbPecaModelo);

                    // Se for cálculo de projeto com medidas exatas da peça, ou seja, sem medida de vão
                    if (itemProjeto.MedidaExata || medidasAlteradas || !itemProjeto.Conferido)
                    {
                        // Busca o projeto
                        var projeto = ProjetoDAO.Instance.GetElement(Glass.Conversoes.StrParaUint(Request["idProjeto"]));

                        // Calcula as medidas das peças, retornando lista
                        var lstPecaModelo = UtilsProjeto.CalcularMedidasPecasComBaseNaTelaComTransacao(modelo, itemProjeto, tbMedInst, tbPecaModelo, false, Request["pcp"] == "1", false,
                            out retornoValidacao);

                        // Insere Peças na tabela peca_item_projeto
                        PecaItemProjetoDAO.Instance.InsertFromPecaModelo(itemProjeto, ref lstPecaModelo);

                        // Insere Peças na tabela material_item_projeto
                        MaterialItemProjetoDAO.Instance.InserePecasVidro(null, projeto.IdCliente, projeto.TipoEntrega, itemProjeto, modelo, lstPecaModelo);

                        #region Update Total Item Projeto

                        ItemProjetoDAO.Instance.UpdateTotalItemProjeto(idItemProjeto);

                        uint? idProjeto = ItemProjetoDAO.Instance.GetIdProjeto(idItemProjeto);
                        uint? idOrcamento = ItemProjetoDAO.Instance.GetIdOrcamento(idItemProjeto);

                        if (idProjeto > 0)
                            ProjetoDAO.Instance.UpdateTotalProjeto(idProjeto.Value);
                        else if (idOrcamento > 0)
                        {
                            var idProd = ProdutosOrcamentoDAO.Instance.ObterIdProdOrcamentoPeloIdItemProjeto(null, (int)idItemProjeto);
                            if (idProd > 0)
                                ProdutosOrcamentoDAO.Instance.UpdateTotaisProdutoOrcamento(null, ProdutosOrcamentoDAO.Instance.GetElementByPrimaryKey(null, (uint)idProd));

                            OrcamentoDAO.Instance.UpdateTotaisOrcamento(null, OrcamentoDAO.Instance.GetElementByPrimaryKey(null, idOrcamento.Value), false, false);
                        }

                        #endregion

                        // Monta tabela de peças
                        UtilsProjeto.CreateTablePecasItemProjeto(ref tbPecaModelo, itemProjeto.IdItemProjeto, itemProjeto, false, false, Request["Parceiro"] == "true");

                        // Mostra o total deste item e do projeto
                        CalculaTotalItemProj();
                        CalculaTotalProj();

                        grdMaterialProjeto.DataBind();
                        grdItemProjeto.DataBind();

                        dtvProjeto.DataBind();

                        // Atualiza imagem associada, podendo ter sido desenhado alguma medida na mesma
                        dtvImagem.DataBind();
                    }
                }

                // Atualiza o campo ambiente no itemProjeto
                if (!String.IsNullOrEmpty(txtAmbiente.Text))
                    ItemProjetoDAO.Instance.AtualizaAmbiente(Glass.Conversoes.StrParaUint(hdfIdItemProjeto.Value), txtAmbiente.Text);
            }
            catch (Exception ex)
            {
                ItemProjetoDAO.Instance.CalculoNaoConferido(hdfIdItemProjeto.Value.StrParaUint());

                Glass.MensagemAlerta.ErrorMsg("Falha ao atualizar confirmar projeto.\n" + retornoValidacao, ex, Page);
                return;
            }

            ItemProjetoDAO.Instance.CalculoConferido(Glass.Conversoes.StrParaUint(hdfIdItemProjeto.Value));

            HiddenField hdfPecasAlteradas = (HiddenField)tbPecaModelo.FindControl("hdfPecasAlteradas");
            HiddenField hdfMedidasAlteradas = (HiddenField)tbPecaModelo.FindControl("hdfMedidasAlteradas");

            /* Chamado 50798. */
            if (hdfPecasAlteradas != null)
                hdfPecasAlteradas.Value = "false";

            /* Chamado 50798. */
            if (hdfMedidasAlteradas != null)
                hdfMedidasAlteradas.Value = "false";

            LimpaCalculo();

            grdItemProjeto.DataBind();

            // Calcula o novo total do projeto
            CalculaTotalProj();

            Glass.MensagemAlerta.ShowMsg("Medidas confirmadas.\n" + retornoValidacao, Page);
        }

        #endregion

        #region Atualiza o ambiente para modelo "Outros"

        protected void btnConfAmbiente_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(txtAmbiente.Text))
                ItemProjetoDAO.Instance.AtualizaAmbiente(Glass.Conversoes.StrParaUint(hdfIdItemProjeto.Value), txtAmbiente.Text);

            LimpaCalculo();

            grdItemProjeto.DataBind();

            // Calcula o novo total do projeto
            CalculaTotalProj();

            Glass.MensagemAlerta.ShowMsg("Ambiente confirmado.", Page);
        }

        #endregion

        #region Cancela cálculo de projeto

        protected void btnCancCalc_Click(object sender, EventArgs e)
        {
            // Limpa o modelo buscado
            LimpaCalculo();
        }

        #endregion

        #region Calcula total do item projeto/projeto

        /// <summary>
        /// Calcula o total do item do projeto e insere o valor na lblSubtotal
        /// </summary>
        protected void CalculaTotalItemProj()
        {
            uint idItemProjeto = Glass.Conversoes.StrParaUint(hdfIdItemProjeto.Value);
            lblSubtotal.Text = ItemProjetoDAO.Instance.GetTotalItemProjeto(idItemProjeto).ToString("C");
            lblM2Vao.Text = ItemProjetoDAO.Instance.GetM2VaoItemProjeto(idItemProjeto).ToString() + "m²";
        }

        /// <summary>
        /// Calcula o total do projeto e insere o valor na lblTotalProj
        /// </summary>
        protected void CalculaTotalProj()
        {
            lblTotalProj.Text = ProjetoDAO.Instance.GetTotalProjeto(Glass.Conversoes.StrParaUint(Request["idProjeto"])).ToString("C");

            lblObsTotal.Visible = false;
        }

        #endregion

        #region Limpa Cálculos

        /// <summary>
        /// Limpa informações relacionadas ao cálculo de projeto atual
        /// </summary>
        protected void LimpaCalculo()
        {
            hdfIdItemProjeto.Value = String.Empty;

            dtvImagem.DataBind();
            dtvImagemMini.DataBind();
            tbPecaModelo.Controls.Clear();
            tbInsKit.Visible = false;
            tbInsTubo.Visible = false;

            lblSubtotal.Text = "";
            lblM2Vao.Text = "";
            txtAmbiente.Text = "";

            // Visibilidade de campos relacionados ao modelo de projeto
            bool visib = false;
            lblMedidas.Visible = visib;
            pchTbTipoInst.Visible = visib;
            tbPecaModelo.Visible = visib;
            tbSubtotal.Visible = visib;
            lblMedidasInst.Visible = visib;
            tbAmbiente.Visible = visib;

            grdMaterialProjeto.DataBind();
            grdItemProjeto.DataBind();
            dtvProjeto.DataBind();
        }

        #endregion

        #region Beneficiamentos

        protected void txtEspessura_DataBinding(object sender, EventArgs e)
        {
            TextBox txt = (TextBox)sender;
            GridViewRow linhaControle = txt.Parent.Parent as GridViewRow;

            MaterialItemProjeto mip = linhaControle.DataItem as MaterialItemProjeto;
            txt.Enabled = mip.Espessura <= 0;
        }

        protected void ctrlBenef_Load(object sender, EventArgs e)
        {
            Glass.UI.Web.Controls.ctrlBenef benef = (Glass.UI.Web.Controls.ctrlBenef)sender;
            GridViewRow linhaControle = benef.Parent.Parent as GridViewRow;

            Control codProd = null;
            if (linhaControle.FindControl("hdfCodInterno") != null)
                codProd = linhaControle.FindControl("hdfCodInterno");
            else
                codProd = linhaControle.FindControl("txtCodProdIns");
            HiddenField hdfAltura = (HiddenField)linhaControle.FindControl("hdfAlturaCalc");
            if (hdfAltura == null)
                hdfAltura = (HiddenField)linhaControle.FindControl("hdfAlturaCalcIns");
            TextBox txtEspessura = (TextBox)linhaControle.FindControl("txtEspessura");
            TextBox txtLargura = (TextBox)linhaControle.FindControl("txtLarguraIns");
            TextBox txtQuantidade = (TextBox)linhaControle.FindControl("txtQtdeIns");
            HiddenField hdfTipoEntrega = (HiddenField)dtvProjeto.FindControl("hdfTipoEntrega");
            Label lblTotalM2 = Beneficiamentos.UsarM2CalcBeneficiamentos ?
                (Label)linhaControle.FindControl("lblTotM2Calc") :
                (Label)linhaControle.FindControl("lblTotM2Ins");
            TextBox txtValorIns = (TextBox)linhaControle.FindControl("txtValorIns");
            HiddenField hdfCliRevenda = (HiddenField)dtvProjeto.FindControl("hdfCliRevenda");
            HiddenField hdfIdCliente = (HiddenField)dtvProjeto.FindControl("hdfIdCliente");
            HiddenField hdfIdPecaItemProj = (HiddenField)linhaControle.FindControl("hdfIdPecaItemProj");
            HiddenField hdfCustoProd = (HiddenField)linhaControle.FindControl("hdfCustoProd");

            benef.CampoAltura = hdfAltura;
            benef.CampoEspessura = txtEspessura;
            benef.CampoLargura = txtLargura;
            benef.CampoQuantidade = txtQuantidade;
            benef.CampoTipoEntrega = hdfTipoEntrega;
            benef.CampoTotalM2 = lblTotalM2;
            benef.CampoValorUnitario = txtValorIns;
            benef.CampoCusto = hdfCustoProd;
            benef.CampoProdutoID = codProd;
            benef.CampoRevenda = hdfCliRevenda;
            benef.CampoClienteID = hdfIdCliente;
            benef.CampoPecaItemProjetoID = hdfIdPecaItemProj;
            benef.CampoAplicacaoID = linhaControle.FindControl("hdfIdAplicacao");
            benef.CampoProcessoID = linhaControle.FindControl("hdfIdProcesso");
            benef.CampoAplicacao = linhaControle.FindControl("txtAplIns");
            benef.CampoProcesso = linhaControle.FindControl("txtProcIns");
        }

        protected void ctrlBenef_PreRender(object sender, EventArgs e)
        {
            Glass.UI.Web.Controls.ctrlBenef benef = (Glass.UI.Web.Controls.ctrlBenef)sender;
            GridViewRow linhaControle = benef.Parent.Parent as GridViewRow;

            if (!benef.CampoQuantidade.Visible)
                benef.CampoQuantidade = (Label)linhaControle.FindControl("lblQtde");
        }

        #endregion

        public string BuscarKitQueryString()
        {
            return "?idGrupoProd=" + (int)Glass.Data.Model.NomeGrupoProd.KitParaBox;
        }

        protected void ctrlCorItemProjeto_CorAlterada(object sender, EventArgs e)
        {
            grdMaterialProjeto.DataBind();
            grdItemProjeto.DataBind();
            dtvProjeto.DataBind();
            CalculaTotalProj();
        }

        protected void ctrlCorItemProjeto2_Load(object sender, EventArgs e)
        {
            ctrlCorItemProjeto2.IdProjeto = Glass.Conversoes.StrParaUintNullable(Request["idProjeto"]);
            ctrlCorItemProjeto2.Visible = ctrlCorItemProjeto2.IdProjeto > 0;
        }

        protected void btnImprimir_PreRender(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(Request["idPedidoEspelho"]) && ((Button)sender).OnClientClick.IndexOf("&pcp=1") == -1)
                ((Button)sender).OnClientClick = ((Button)sender).OnClientClick.Replace("); ", "&pcp=1); ");
        }

        protected string NomeControleBenef()
        {
            return grdMaterialProjeto.EditIndex == -1 ? "ctrlBenefInserir" : "ctrlBenefEditar";
        }

        protected bool IsExportacaoOptyWay()
        {
            return EtiquetaConfig.TipoExportacaoEtiqueta == DataSources.TipoExportacaoEtiquetaEnum.OptyWay;
        }

        protected void txtValorIns_Load(object sender, EventArgs e)
        {
            ((TextBox)sender).Enabled = PedidoConfig.DadosPedido.AlterarValorUnitarioProduto;
        }
    }
}
