using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Helper;
using Glass.Data.DAL;
using Glass.Data.Model;
using System.Web.UI.HtmlControls;
using Glass.Configuracoes;
using System.Linq;

namespace Glass.UI.Web.WebGlassParceiros
{
    public partial class CadProjeto : System.Web.UI.Page
    {
        uint idItemProjeto = 0;
        uint idMaterItemProj = 0;
    
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(WebGlassParceiros.CadProjeto));
            Ajax.Utility.RegisterTypeForAjax(typeof(Cadastros.Projeto.CadProjetoAvulso));
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
    
            if (!IsPostBack)
            {
                if (dtvProjeto.CurrentMode == DetailsViewMode.Insert)
                {
                    if (Request["idProjeto"] == null)
                    {
                        Cliente cli = ClienteDAO.Instance.GetElementByPrimaryKey((int)UserInfo.GetUserInfo.IdCliente);

                        if(cli.IdTransportador.GetValueOrDefault() != 0)
                            ((DropDownList)dtvProjeto.FindControl("drpTransportador")).SelectedValue = cli.IdTransportador.ToString();

                        if (dtvProjeto.FindControl("txtDataCad") != null)
                            ((TextBox)dtvProjeto.FindControl("txtDataCad")).Text = DateTime.Now.ToString("dd/MM/yyyy");
                    }
                    else
                    {
                        hdfIdProjeto.Value = Request["idProjeto"];
                        dtvProjeto.ChangeMode(DetailsViewMode.ReadOnly);
    
                        CalculaTotalProj();

                        if (ProjetoDAO.Instance.GetTipoVenda(null, Conversoes.StrParaUint(Request["idProjeto"])) == (uint)Glass.Data.Model.Pedido.TipoPedidoEnum.Revenda)
                        {
                            hdfProdutosEstoque.Value = "1";
                            var ips = ItemProjetoDAO.Instance.GetList(Conversoes.StrParaUint(Request["idProjeto"]), null, 0, 10);
                            var otr01 = ips.Where(f => f.CodigoModelo == "OTR01").FirstOrDefault();
                            if (otr01 != null)
                            {
                                hdfIdItemProjeto.Value = otr01.IdItemProjeto.ToString();
                                MontaModelo(true);
                            }                           
                        }
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

            if (!IsPostBack)
                txtObsLiberacao.Text = ProjetoDAO.Instance.ObtemObsLiberacao(Conversoes.StrParaInt(Request["idProjeto"]));
        }

        protected void drpTipoPedido_DataBound(object sender, EventArgs e)
        {
            ((DropDownList)sender).Items.FindByValue(((int)Glass.Data.Model.Pedido.TipoPedidoEnum.MaoDeObra).ToString()).Enabled = false;
            ((DropDownList)sender).Items.FindByValue(((int)Glass.Data.Model.Pedido.TipoPedidoEnum.Producao).ToString()).Enabled = false;
            var maoDeObraEspecial = ((DropDownList)sender).Items.FindByValue(((int)Glass.Data.Model.Pedido.TipoPedidoEnum.MaoDeObraEspecial).ToString());

            //Verifica se deve permitir apenas pedidos de venda
            if (PedidoConfig.PermitirApenasPedidosDeVendaNoEcommerce)
            {
                //Desabilita o tipo revenda
                ((DropDownList)sender).Items.FindByValue(((int)Glass.Data.Model.Pedido.TipoPedidoEnum.Revenda).ToString()).Enabled = false;
                //Seleciona como padrão o tipo venda
                ((DropDownList)sender).Items.FindByValue(((int)Glass.Data.Model.Pedido.TipoPedidoEnum.Venda).ToString()).Selected = true;
                //Desabilita o dropdown para que não haja alterações.
                ((DropDownList)sender).Enabled = false;
            }

            /* Chamado 51931. */
            if (maoDeObraEspecial != null)
                maoDeObraEspecial.Enabled = false;
        }

        protected void grdMaterialProjeto_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            grdMaterialProjeto.ShowFooter = e.CommandName != "Edit";
        }

        protected void btnSalvar_Click(object sender, EventArgs e)
        {
            ProjetoDAO.Instance.SalvarObsLiberacao(Conversoes.StrParaInt(Request["idProjeto"]), txtObsLiberacao.Text);
            ClientScript.RegisterClientScriptBlock(this.GetType(), "ok", "alert('Observação salva com sucesso.'); closeWindow();", true);

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

                if (ProjetoDAO.Instance.GetTipoVenda(null, Conversoes.StrParaUint(e.ReturnValue.ToString())) == (uint)Glass.Data.Model.Pedido.TipoPedidoEnum.Revenda)
                {
                    var idProjetoModelo = ProjetoModeloDAO.Instance.ObtemId("OTR01");

                    // Em projetos de Revenda são inseridos somente os produtos e o sistema está configurado para
                    // buscar automaticamente o projeto de código OTR01 e exibir somente a grid de materiais.
                    if (idProjetoModelo == 0)
                        throw new Exception("Cadastre o projeto de código OTR01 para que seja possível emitir pedidos de Revenda. Contate o suporte WebGlass.");

                    NovoItemProjeto(e.ReturnValue.ToString(), idProjetoModelo.ToString(), "0", "1", "1", "1", "true", "false");
                }

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
            {
                if (Request["idProjeto"].StrParaUintNullable() > 0)
                {
                    var idsItemProjeto = ItemProjetoDAO.Instance.GetList(Request["idProjeto"].StrParaUint(), null, 0, 10);
                    var otr01 = idsItemProjeto.Where(f => f.CodigoModelo == "OTR01").FirstOrDefault();

                    // Caso o projeto não possua itens com o modelo OTR01, significa que o tipo de venda foi alterado.
                    // Neste caso, um novo item de projeto deve ser criado.
                    if (otr01 == null && ProjetoDAO.Instance.GetTipoVenda(null, Request["idProjeto"].StrParaUint()) == (uint)Glass.Data.Model.Pedido.TipoPedidoEnum.Revenda)
                    {
                        var idProjetoModelo = ProjetoModeloDAO.Instance.ObtemId("OTR01");

                        // Em projetos de Revenda são inseridos somente os produtos e o sistema está configurado para
                        // buscar automaticamente o projeto de código OTR01 e exibir somente a grid de materiais.
                        if (idProjetoModelo == 0)
                            throw new Exception("Cadastre o projeto de código OTR01 para que seja possível emitir pedidos de Revenda. Contate o suporte WebGlass.");

                        NovoItemProjeto(Request["idProjeto"], idProjetoModelo.ToString(), "0", "1", "1", "1", "true", "false");
                    }
                }

                Response.Redirect("CadProjeto.aspx?IdProjeto=" + hdfIdProjeto.Value);
            }
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
    
        protected void odsMaterialItemProjeto_Updating(object sender, Colosoft.WebControls.VirtualObjectDataSourceMethodEventArgs e)
        {
            var idMaterItemProj = ((MaterialItemProjeto)e.InputParameters[0]).IdMaterItemProj;
    
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
    
        protected void odsMaterialItemProjeto_Deleting(object sender, Colosoft.WebControls.VirtualObjectDataSourceMethodEventArgs e)
        {
            var idMaterItemProj = ((MaterialItemProjeto)e.InputParameters[0]).IdMaterItemProj;
    
            idItemProjeto = MaterialItemProjetoDAO.Instance.ObtemIdItemProjeto(idMaterItemProj);
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
                    uint idProd = ProdutosOrcamentoDAO.Instance.ObtemIdProdutoPorIdItemProjeto(idItemProjeto);
                    if (idProd > 0)
                        ProdutosOrcamentoDAO.Instance.UpdateTotaisProdutoOrcamento(idProd);
    
                    OrcamentoDAO.Instance.UpdateTotaisOrcamento(idOrc.Value);
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
            btnImprimir.OnClientClick = "openWindow(600, 800, '" + ResolveClientUrl("~/Relatorios/Projeto/RelBase.aspx") + 
                "?rel=imagemProjeto&idOrcamento=" + Request["idOrcamento"] + "&idPedido=" + Request["idPedido"] + "&idItemProjeto=" + hdfIdItemProjeto.Value + "'); return false;";
    
            btnImprimir.Visible = true;
    
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
                        UtilsProjeto.CreateTablePecasItemProjeto(ref tbPecaModelo, itemProj.IdItemProjeto, itemProj, false, false, true);
                    else // Monta a tabela de peças com valores padrões
                        UtilsProjeto.CreateTablePecasModelo(ref tbPecaModelo, null, itemProj, false, false, true);
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
            hdfDuplicarEspessura.Value = itemProj.EspessuraVidro.ToString();
            hdfDuplicarCorVidro.Value = itemProj.IdCorVidro.ToString();
            hdfDuplicarCorAluminio.Value = itemProj.IdCorAluminio.ToString();
            hdfDuplicarCorFerragem.Value = itemProj.IdCorFerragem.ToString();
            hdfDuplicarApenasVidros.Value = itemProj.ApenasVidros.ToString().ToLower();
            hdfDuplicarMedidaExata.Value = itemProj.MedidaExata.ToString().ToLower();
    
            btnCalcMed.Visible = !itemProj.MedidaExata;
    
            CalculaTotalItemProj();
    
            // Deve realizar o bind na grdMaterialProjeto apenas se não for postback, senão
            // ao inserir um novo material à mão, os valores são apagados.
            if (!IsPostBack)
            {
                grdMaterialProjeto.DataBind();
                grdItemProjeto.DataBind();
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
                    null, null, null, null, idProjetoModelo.StrParaUint(), espessuraVidro.StrParaIntNullable(),
                    idCorVidro.StrParaUint(), idCorAluminio.StrParaUint(), idCorFerragem.StrParaUint(), apenasVidros == "true",
                    medidaExata == "true", false).IdItemProjeto;
    
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
    
                if (cli == null || cli.IdCli == 0)
                    return "Erro;Cliente não encontrado.";
                else if (cli.Situacao == (int)SituacaoCliente.Inativo)
                    return "Erro;Cliente inativo. Motivo: " + cli.Obs;
                else if (cli.Situacao == (int)SituacaoCliente.Cancelado)
                    return "Erro;Cliente cancelado. Motivo: " + cli.Obs;
                else if (cli.Situacao == (int)SituacaoCliente.Bloqueado)
                    return "Erro;Cliente bloqueado. Motivo: " + cli.Obs;
                else
                    return "Ok;" + cli.Nome + ";" + cli.Revenda.ToString().ToLower() + ";" + cli.IdTransportador;
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
        public string GetProduto(string codInterno, string tipoEntrega, string revenda, string idCliente, string idProjeto)
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
                    
                    var idLojaSubgrupoProd = SubgrupoProdDAO.Instance.ObterIdLoja(null, prod.IdSubgrupoProd.Value);

                    if (idLojaProjeto > 0 && idLojaSubgrupoProd > 0 && idLojaProjeto != idLojaSubgrupoProd)
                        return "Erro;Esse produto não pode ser utilizado, pois, a loja do seu subgrupo é diferente da loja do pedido/projeto.";
                    /* Chamado 48322. */
                    else if (idLojaProjeto == 0 && idLojaSubgrupoProd > 0)
                        ProjetoDAO.Instance.AtualizarIdLojaProjeto(null, idProjeto.StrParaInt(), (int)idLojaSubgrupoProd);

                    //Chamado 50753
                    if(!ClienteDAO.Instance.ValidaSubgrupo(idCliente.StrParaUint(), codInterno))
                        return "Erro;Esse produto não pode ser utilizado, pois o subgrupo não esta vinculado ao cliente.";

                    if(SubgrupoProdDAO.Instance.ObterBloquearEcommerce(null, prod.IdSubgrupoProd.Value))
                        return "Erro;Este produto não pode ser selecionado na plataforma e-commerce. Entre em contato com a empresa para mais informações";
                }

                if (ProjetoDAO.Instance.GetTipoVenda(null, Conversoes.StrParaUint(idProjeto)) == (uint)Glass.Data.Model.Pedido.TipoPedidoEnum.Revenda)
                {
                    var idGrupoProd = ProdutoDAO.Instance.ObtemIdGrupoProd(prod.IdProd);
                    var idSubGrupoProd = ProdutoDAO.Instance.ObtemIdSubgrupoProd(prod.IdProd);
                    var grupo = GrupoProdDAO.Instance.GetElementByPrimaryKey(idGrupoProd);
                    if (grupo.IdGrupoProd == (int)NomeGrupoProd.Vidro && !SubgrupoProdDAO.Instance.GetElementByPrimaryKey(idSubGrupoProd.Value).ProdutosEstoque)
                        return "Erro;Produtos do grupo vidro só podem ser inseridos nesse pedido se tiverem a configuração (Produtos para Estoque) marcada no subgrupo associado.";
                }

                string retorno = "Prod;" + prod.IdProd + ";" + prod.Descricao;
    
                // Recupera o valor de tabela do produto
                int? tipoEntr = !String.IsNullOrEmpty(tipoEntrega) ? (int?)Glass.Conversoes.StrParaInt(tipoEntrega) : null;
                uint? idCli = !String.IsNullOrEmpty(idCliente) ? (uint?)Glass.Conversoes.StrParaUint(idCliente) : null;
                retorno += ";" + ProdutoDAO.Instance.GetValorTabela(prod.IdProd, tipoEntr, idCli, revenda.ToLower() == "true", false, 0, null, idProjeto.StrParaInt(), null).ToString("F2");
    
                retorno += ";" + Glass.Data.DAL.GrupoProdDAO.Instance.IsVidro(prod.IdGrupoProd).ToString().ToLower() + ";" +
                    Glass.Data.DAL.GrupoProdDAO.Instance.IsAluminio(prod.IdGrupoProd).ToString().ToLower() + ";" +
                    (prod.AtivarAreaMinima ? prod.AreaMinima.ToString().Replace(',', '.') : "0");
    
                retorno += ";" + Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo(prod.IdGrupoProd, prod.IdSubgrupoProd);
                retorno += ";" + prod.Espessura;
                retorno += ";" + prod.Forma;
                retorno += ";" + prod.CustoCompra;
                retorno += ";" + prod.Altura;
                retorno += ";" + prod.Largura;
                retorno += ";" + (prod.IdProcesso.GetValueOrDefault() > 0 ? EtiquetaProcessoDAO.Instance.GetCodInternoByIds(prod.IdProcesso.GetValueOrDefault().ToString()) : "");
                retorno += ";" + (prod.IdAplicacao.GetValueOrDefault() > 0 ? EtiquetaAplicacaoDAO.Instance.GetCodInternoByIds(prod.IdAplicacao.GetValueOrDefault().ToString()) : "");

                retorno += ";" + ProdutoDAO.Instance.ExibirMensagemEstoque(prod.IdProd).ToString().ToLower();
                bool bloquearEstoque = GrupoProdDAO.Instance.BloquearEstoque(prod.IdGrupoProd, prod.IdSubgrupoProd);
                retorno += ";" + (bloquearEstoque ? ProdutoLojaDAO.Instance.GetEstoque(null, (uint)ProjetoDAO.Instance.ObterIdLoja(null, idProjeto.StrParaInt()), (uint)prod.IdProd).ToString() : "999999999");
                retorno += ";" + ProdutoLojaDAO.Instance.GetEstoque(null, (uint)ProjetoDAO.Instance.ObterIdLoja(null, idProjeto.StrParaInt()), (uint)prod.IdProd).ToString();

                return retorno;
            }
        }
    
        [Ajax.AjaxMethod()]
        public string GetVidro(string idProjeto, string codInterno, string idCliente)
        {
            Produto prod = ProdutoDAO.Instance.GetByCodInterno(codInterno);
    
            if (prod == null)
                return "Erro;Não existe produto com o código informado.";
            else if (prod.Situacao == Glass.Situacao.Inativo)
                return "Erro;Produto inativo." + (!String.IsNullOrEmpty(prod.Obs) ? " Obs: " + prod.Obs : "");

            /* Chamado 47194. */
            if (idProjeto.StrParaIntNullable() > 0 && prod.IdSubgrupoProd > 0)
            {
                var idLojaProjeto = ProjetoDAO.Instance.ObterIdLoja(null, idProjeto.StrParaInt());
                
                var idLojaSubgrupoProd = SubgrupoProdDAO.Instance.ObterIdLoja(null, prod.IdSubgrupoProd.Value);

                if (idLojaProjeto > 0 && idLojaSubgrupoProd > 0 && idLojaProjeto != idLojaSubgrupoProd)
                    return "Erro;Esse produto não pode ser utilizado, pois, a loja do seu subgrupo é diferente da loja do pedido/projeto.";
                /* Chamado 48322. */
                else if (idLojaProjeto == 0 && idLojaSubgrupoProd > 0)
                    ProjetoDAO.Instance.AtualizarIdLojaProjeto(null, idProjeto.StrParaInt(), (int)idLojaSubgrupoProd);

                //Chamado 50753
                if (!ClienteDAO.Instance.ValidaSubgrupo(idCliente.StrParaUint(), codInterno))
                    return "Erro;Esse produto não pode ser utilizado, pois o subgrupo não esta vinculado ao cliente.";
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
    
            return OrcamentoDAO.Instance.ExistsOrcamentoEmAberto(idOrcamento).ToString().ToLower();
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

        //Valida se pode marcar a opção de fast delivery
        [Ajax.AjaxMethod]
        public string PodeMarcarFastDelivery(string idProjeto)
        {
            if (string.IsNullOrEmpty(idProjeto) || idProjeto == "0")
                return "false";

            var idProj = idProjeto.StrParaUint();

            var matItemProj = MaterialItemProjetoDAO.Instance.GetByProjeto(idProj);

            if (matItemProj != null && matItemProj.FirstOrDefault() != null)
            {
                foreach (var material in matItemProj)
                {
                    bool naoPermitirFastDelivery = false;

                    if (material.IdAplicacao.GetValueOrDefault() > 0)
                    {
                        naoPermitirFastDelivery = EtiquetaAplicacaoDAO.Instance.NaoPermitirFastDelivery(material.IdAplicacao.Value);
                    }

                    if (naoPermitirFastDelivery)
                        return string.Format("Erro|O produto {0} está associado à uma aplicacao que não permite fast delivery.", material.DescrProduto);
                }
            }

            return "true";
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

            uint idProd = Conversoes.StrParaUint(hdfIdProdMater.Value);

            if (ProjetoDAO.Instance.GetTipoVenda(null, hdfIdProjeto.Value.StrParaUint()) == (uint)Glass.Data.Model.Pedido.TipoPedidoEnum.Revenda)
            {
                var idGrupoProd = ProdutoDAO.Instance.ObtemIdGrupoProd((int)idProd);
                var idSubGrupoProd = ProdutoDAO.Instance.ObtemIdSubgrupoProd((int)idProd);
                var grupo = GrupoProdDAO.Instance.GetElementByPrimaryKey(idGrupoProd);

                if (grupo.IdGrupoProd == (int)NomeGrupoProd.Vidro && !SubgrupoProdDAO.Instance.GetElementByPrimaryKey(idSubGrupoProd.Value).ProdutosEstoque)
                {
                    MensagemAlerta.ShowMsg("Produtos do grupo vidro só podem ser inseridos nesse pedido se tiverem a configuração (Produtos para Estoque) marcada no subgrupo associado.", Page);
                    return;
                }
            }

            Controls.ctrlBenef benef = (Controls.ctrlBenef)grdMaterialProjeto.FooterRow.FindControl("ctrlBenefInserir");    
            uint idItemProjeto = Glass.Conversoes.StrParaUint(hdfIdItemProjeto.Value);     
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

            if (materItem.Qtde == 0)
            {
                MensagemAlerta.ShowMsg("Informe a quantidade do produto.", Page);
                return;
            }

            materItem.Valor = decimal.Parse(((TextBox)grdMaterialProjeto.FooterRow.FindControl("txtValorIns")).Text);
            materItem.Altura = altura;
            materItem.AlturaCalc = alturaCalc;
            materItem.Largura = largura;
            materItem.IdProd = idProd;
            materItem.Espessura = Glass.Conversoes.StrParaInt(espessura);
            if (!String.IsNullOrEmpty(idAplicacaoStr)) materItem.IdAplicacao = Glass.Conversoes.StrParaUint(idAplicacaoStr);
            if (!String.IsNullOrEmpty(idProcessoStr)) materItem.IdProcesso = Glass.Conversoes.StrParaUint(idProcessoStr);
            materItem.Beneficiamentos = benef.Beneficiamentos;
            materItem.PedCli = ((TextBox)grdMaterialProjeto.FooterRow.FindControl("txtPedCli")).Text;
    
            // Verifica se o produto com a largura e altura especificados já foi adicionado ao pedido
            if (MaterialItemProjetoDAO.Instance.ExistsInItemProjeto(idItemProjeto, idProd, altura, largura, materItem.IdProcesso, materItem.IdAplicacao))
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
            uint idPedido = 0;
            
            try
            {
                var obsLiberacao = ProjetoDAO.Instance.ObtemObsLiberacao(Conversoes.StrParaInt(Request["idProjeto"]));
                if (obsLiberacao != txtObsLiberacao.Text)
                    ProjetoDAO.Instance.SalvarObsLiberacao(Conversoes.StrParaInt(Request["idProjeto"]), txtObsLiberacao.Text);

                bool apenasVidros = false;
                CheckBox check = dtvProjeto.FindControl("chkApenasVidros") as CheckBox;
                if (check != null)
                    apenasVidros = check.Checked;
    
                Exception erro;
                idPedido = ProjetoDAO.Instance.GerarPedidoParceiro(Glass.Conversoes.StrParaUint(Request["idProjeto"]), apenasVidros, out erro);
                
                if (erro != null)
                    throw erro;
    
                Page.ClientScript.RegisterClientScriptBlock(GetType(), "gerarPedido", "alert('Pedido gerado com sucesso! Pedido: " + idPedido + "');\n" +
                    "redirectUrl('LstPedidos.aspx');\n", true);
            }
            catch (Exception ex)
            {
                string msg = Glass.MensagemAlerta.FormatErrorMsg("", ex) + (idPedido > 0 ? " Pedido gerado: " + idPedido : "");
    
                Page.ClientScript.RegisterClientScriptBlock(GetType(), "erroGerarPedido", "alert('" + msg + "');\n", true);
                if (idPedido > 0)
                    Page.ClientScript.RegisterClientScriptBlock(GetType(), "pedidoGerado", "redirectUrl('LstPedidos.aspx');\n", true);
    
                return;
            }
        }
    
        protected void btnGerarOrcamento_Click(object sender, EventArgs e)
        {
            try
            {
                // Gera um orçamento a partir deste projeto
                uint idOrcamento = OrcamentoDAO.Instance.GerarOrcamento(Request["idProjeto"].StrParaUint(), true);
    
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
                    mater.Valor = decimal.Parse(txtValorKit.Text);                
    
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
                    mater.Valor = decimal.Parse(txtValorTubo.Text);
                    mater.Altura = Single.Parse(txtComprTubo.Text, System.Globalization.NumberStyles.AllowDecimalPoint);                
    
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
                var lstPecaModelo = UtilsProjeto.CalcularMedidasPecasComBaseNaTelaComTransacao(modelo, itemProjeto, tbMedInst, tbPecaModelo, true, false, false, out retornoValidacao);

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
                    uint idProd = ProdutosOrcamentoDAO.Instance.ObtemIdProdutoPorIdItemProjeto(idItemProjeto);
                    if (idProd > 0)
                        ProdutosOrcamentoDAO.Instance.UpdateTotaisProdutoOrcamento(idProd);
    
                    OrcamentoDAO.Instance.UpdateTotaisOrcamento(idOrcamento.Value);
                }
    
                #endregion
    
                // Monta tabela de peças
                UtilsProjeto.CreateTablePecasItemProjeto(ref tbPecaModelo, itemProjeto.IdItemProjeto, itemProjeto, false, false, true);
    
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
                // Verifica se o ambiente foi informado
                if (txtAmbiente.Text == String.Empty)
                {
                    Glass.MensagemAlerta.ShowMsg("Informe o ambiente.", Page);
                    return;
                }

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
                        var lstPecaModelo = UtilsProjeto.CalcularMedidasPecasComBaseNaTelaComTransacao(modelo, itemProjeto, tbMedInst, tbPecaModelo, false, true, medidasAlteradas,
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
                            uint idProd = ProdutosOrcamentoDAO.Instance.ObtemIdProdutoPorIdItemProjeto(idItemProjeto);
                            if (idProd > 0)
                                ProdutosOrcamentoDAO.Instance.UpdateTotaisProdutoOrcamento(idProd);
    
                            OrcamentoDAO.Instance.UpdateTotaisOrcamento(idOrcamento.Value);
                        }
    
                        #endregion
    
                        // Monta tabela de peças
                        UtilsProjeto.CreateTablePecasItemProjeto(ref tbPecaModelo, itemProjeto.IdItemProjeto, itemProjeto, false, false, true);
    
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

            ItemProjetoDAO.Instance.CalculoConferido(hdfIdItemProjeto.Value.StrParaUint());

            HiddenField hdfPecasAlteradas = (HiddenField)tbPecaModelo.FindControl("hdfPecasAlteradas");
            HiddenField hdfMedidasAlteradas = (HiddenField)tbPecaModelo.FindControl("hdfMedidasAlteradas");

            /* Chamado 50798. */
            if (hdfPecasAlteradas != null)
                hdfPecasAlteradas.Value = "false";

            /* Chamado 50798. */
            if (hdfMedidasAlteradas != null)
                hdfMedidasAlteradas.Value = "false";

           //LimpaCalculo();

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

            // Verifica se o ambiente foi informado
            if (txtAmbiente.Text == String.Empty)
            {
                Glass.MensagemAlerta.ShowMsg("Informe o ambiente.", Page);
                return;
            }

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
    
        #region Exclui projeto
    
        /// <summary>
        /// Calcula Medidas
        /// </summary>
        protected void btnExcluirProjeto_Click(object sender, EventArgs e)
        {
            ExcluirProjeto(Glass.Conversoes.StrParaUint(hdfIdItemProjeto.Value));
    
            LimpaCalculo();
            CalculaTotalProj();
        }
    
        protected void ExcluirProjeto(uint idItemProjeto)
        {
            try
            {
                // Verifica se o itemProjeto existe
                if (!ItemProjetoDAO.Instance.Exists(idItemProjeto))
                {
                    Glass.MensagemAlerta.ShowMsg("Este projeto já foi excluído.", Page);
                    return;
                }
    
                // Exclui o item_projeto
                ItemProjetoDAO.Instance.ExcluiProjeto(idItemProjeto, null, null, null);
                ProjetoDAO.Instance.UpdateTotalProjeto(Glass.Conversoes.StrParaUint(Request["idProjeto"]));
    
                grdMaterialProjeto.DataBind();
                grdItemProjeto.DataBind();
    
                ClientScript.RegisterClientScriptBlock(typeof(string), "Ok", "window.opener.refreshPage();", true);
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao excluir projeto.", ex, Page);
            }
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
            tbMedInst.Controls.Clear();
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
            Controls.ctrlBenef benef = (Controls.ctrlBenef)sender;
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
            Controls.ctrlBenef benef = (Controls.ctrlBenef)sender;
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

            if ((ProjetoDAO.Instance.GetTipoVenda(null, Conversoes.StrParaUint(Request["idProjeto"])) == (uint)Glass.Data.Model.Pedido.TipoPedidoEnum.Revenda) ||
                ProjetoConfig.TelaCadastroParceiros.EsconderCamposAlteraCorItemProjeto)
                ctrlCorItemProjeto2.Visible = false;
            else
                ctrlCorItemProjeto2.Visible = ctrlCorItemProjeto2.IdProjeto > 0;
        }
    
        protected void ctrProjeto_Load(object sender, EventArgs e)
        {
            // Se for versão WebGlass Projetos, não mostrar campo idCliente
            if (sender is WebControl)
                ((WebControl)sender).Attributes.Add("style", "display: none;");
            else if (sender is HtmlControl)
                ((HtmlControl)sender).Attributes.Add("style", "display: none;");
            else
                ((Control)sender).Visible = false;
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
    
        protected void lblCliente_Load(object sender, EventArgs e)
        {
            ((Label)sender).Text = UserInfo.GetUserInfo.Nome;
        }
    
        protected void hdfCliente_Load(object sender, EventArgs e)
        {
            ((HiddenField)sender).Value = UserInfo.GetUserInfo.IdCliente.Value.ToString();
        }
    
        protected void btnCancelar_Click1(object sender, EventArgs e)
        {
            Response.Redirect(Request.Url.ToString());
        }
    
        protected void chkApenasVidro_Load(object sender, EventArgs e)
        {
            ((CheckBox)sender).Checked = true;
        }
    
        protected bool IsExportacaoOptyWay()
        {
            return EtiquetaConfig.TipoExportacaoEtiqueta == DataSources.TipoExportacaoEtiquetaEnum.OptyWay;
        }
    
        protected void lblTipoEntrega_Load(object sender, EventArgs e)
        {
            // Esconde label tipo de entrega
            if (Glass.Configuracoes.ProjetoConfig.TelaCadastroParceiros.EsconderCamposTipoEntrega)
                ((Label)sender).Style.Add("display", "none");
        }
    
        protected void drpTipoEntrega_DataBound(object sender, EventArgs e)
        {
            // Esconde tipo de entrega
            if (Glass.Configuracoes.ProjetoConfig.TelaCadastroParceiros.EsconderCamposTipoEntrega)
                ((DropDownList)sender).Style.Add("display", "none");

            if (Glass.Configuracoes.ProjetoConfig.TelaCadastroParceiros.BloquearTipoEntregaClientesRota)
            {
                ((DropDownList)sender).Enabled = false;

                if (dtvProjeto.CurrentMode == DetailsViewMode.Insert)
                {
                    var clienteRota = RotaClienteDAO.Instance.IsClienteAssociado(UserInfo.GetUserInfo.IdCliente.Value);

                    ((DropDownList)sender).SelectedValue = (clienteRota ?
                        DataSources.Instance.GetTipoEntregaEntrega() :
                        DataSources.Instance.GetTipoEntregaBalcao()).ToString();
                }
            }

            if (Configuracoes.ProjetoConfig.TelaCadastroParceiros.BloquearTipoEntregaEntrega)
            {
                ((DropDownList)sender).Enabled = false;
                ((DropDownList)sender).SelectedValue = DataSources.Instance.GetTipoEntregaEntrega().ToString();
            }
        }

        protected void lblCalculoProjeto_Load(object sender, EventArgs e)
        {
            if (ProjetoDAO.Instance.GetTipoVenda(null, Conversoes.StrParaUint(Request["idProjeto"])) == (uint)Glass.Data.Model.Pedido.TipoPedidoEnum.Revenda)
                lblCalculoProjeto.Visible = false;
        }

        protected void lblCalculosEfetuados_Load(object sender, EventArgs e)
        {
            if (ProjetoDAO.Instance.GetTipoVenda(null, Conversoes.StrParaUint(Request["idProjeto"])) == (uint)Glass.Data.Model.Pedido.TipoPedidoEnum.Revenda)
                lblCalculosEfetuados.Visible = false;
        }         

        protected void trProjeto_Load(object sender, EventArgs e)
        {
            if (ProjetoDAO.Instance.GetTipoVenda(null, Conversoes.StrParaUint(Request["idProjeto"])) == (uint)Glass.Data.Model.Pedido.TipoPedidoEnum.Revenda)
                trProjeto.Style["display"] = "none";
        }
    }
}
