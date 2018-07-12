using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Helper;
using Glass.Data.DAL;
using Glass.Data.Model;
using Glass.Configuracoes;
using System.Linq;

namespace Glass.UI.Web.Cadastros.Projeto
{
    public partial class CadProjetoAvulso : System.Web.UI.Page
    {
        uint idItemProjeto = 0;
        uint idMaterItemProj = 0;

        private string url;

        // Define se as medidas das peças poderão ser alteradas
        protected bool AlterarMedidasPecas()
        {
            return Request["pcp"] != null;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.Projeto.CadProjetoAvulso));
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));

            if (Glass.Configuracoes.ProjetoConfig.TelaCadastroAvulso.AlterarTextoLabelAmbiente)
                lblAmbiente.Text = "Ped. Cliente/Ambiente";

            if (!IsPostBack)
            {
                hdfIdOrcamento.Value = Request["IdOrcamento"];
                hdfIdAmbienteOrca.Value = Request["IdAmbienteOrca"];
                hdfIdPedidoOriginal.Value = Request["IdPedido"];
                hdfIdPedidoEspelho.Value = Request["IdPedidoEspelho"];
                hdfTipoEntrega.Value = Request["TipoEntrega"];
                hdfIdCliente.Value = Request["IdCliente"];
                hdfCliRevenda.Value = !String.IsNullOrEmpty(Request["IdCliente"]) ? ClienteDAO.Instance.IsRevenda(Glass.Conversoes.StrParaUint(Request["IdCliente"])).ToString().ToLower() : "false";

                var idPedido = Glass.Conversoes.StrParaUint(!String.IsNullOrEmpty(Request["IdPedido"]) ?
                    Request["idPedido"] : Request["idPedidoEspelho"]);

                if (idPedido > 0)
                {
                    hdfTipoPedido.Value = ((int)PedidoDAO.Instance.GetTipoPedido(null, idPedido)).ToString();
                    hdfTipoVenda.Value = ((int)PedidoDAO.Instance.ObtemTipoVenda(null, idPedido)).ToString();
                }

                if (String.IsNullOrEmpty(hdfIdAmbientePedido.Value))
                    hdfIdAmbientePedido.Value = Request["IdAmbientePedido"];

                if (String.IsNullOrEmpty(hdfIdAmbientePedidoEspelho.Value))
                    hdfIdAmbientePedidoEspelho.Value = Request["IdAmbientePedidoEspelho"];

                bool buscarReposicao = true;
                var parceiro = Request["Parceiro"] == "true" ? true : false;

                if (!String.IsNullOrEmpty(Request["idProdOrca"]))
                    hdfIdItemProjeto.Value = ProdutosOrcamentoDAO.Instance.BuscaItemProjeto(Glass.Conversoes.StrParaUint(Request["idProdOrca"])).ToString();
                else if (!String.IsNullOrEmpty(hdfIdAmbientePedido.Value))
                    hdfIdItemProjeto.Value = AmbientePedidoDAO.Instance.ObtemItemProjeto(Glass.Conversoes.StrParaUint(Request["idAmbientePedido"])).ToString();
                else if (!String.IsNullOrEmpty(hdfIdAmbientePedidoEspelho.Value))
                    hdfIdItemProjeto.Value = AmbientePedidoEspelhoDAO.Instance.ObtemItemProjeto(Glass.Conversoes.StrParaUint(Request["idAmbientePedidoEspelho"])).ToString();
                else if (parceiro && !Configuracoes.ProjetoConfig.TelaCadastroParceiros.ExibirCorAluminioFerragemWebGlassParceiros)
                {
                    // Abre a seleção de modelo de projeto para WebglassParceiros
                    ClientScript.RegisterStartupScript(typeof(string), "SelModelo", "novoModelo(true);", true);
                    buscarReposicao = false;
                }
                else
                {
                    // Abre a seleção de modelo de projeto
                    ClientScript.RegisterStartupScript(typeof(string), "SelModelo", "novoModelo();", true);
                    buscarReposicao = false;
                }

                var itemProjeto = ItemProjetoDAO.Instance.GetElementByPrimaryKey(hdfIdItemProjeto.Value.StrParaUint());

                hdfIsReposicao.Value = (buscarReposicao && (itemProjeto != null ? itemProjeto.Reposicao : false)).ToString().ToLower();

                // Só exibe a coluna "Obs." para projetos em pedidos espelho
                grdMaterialProjeto.Columns[12].Visible = !String.IsNullOrEmpty(hdfIdAmbientePedidoEspelho.Value);
                
                //Se for lite não exibe processo e aplicação
                if (Glass.Configuracoes.Geral.SistemaLite)
                {
                    grdMaterialProjeto.Columns[8].Visible = false;
                    grdMaterialProjeto.Columns[9].Visible = false;
                }

                //Se a tela tiver sido aberta no callback do CADProject
                if (!string.IsNullOrEmpty(Request["idPecaItemProj"]))
                {
                    try
                    {
                        UtilsProjeto.SalvarProjetoCADProject(Request["idPecaItemProj"].Split(',')[0].StrParaUint(), Request["pcp"] == "1", Request["cancel"] == "true");
                    }
                    catch (Exception ex)
                    {
                        MensagemAlerta.ErrorMsg("Importar arquivo CADProject", ex, Page);
                    }
                }
            }

            // Monta o modelo selecionado
            if ((!String.IsNullOrEmpty(Request["idOrcamento"]) || !String.IsNullOrEmpty(Request["idPedido"]) ||
                !String.IsNullOrEmpty(Request["idPedidoEspelho"])) && !String.IsNullOrEmpty(hdfIdItemProjeto.Value))
                MontaModelo(true);
        }

        protected void grdMaterialProjeto_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            grdMaterialProjeto.ShowFooter = e.CommandName != "Edit";
        }

        #region Eventos de Grids

        protected void grdMaterialProjeto_PreRender(object sender, EventArgs e)
        {
            if (hdfIdItemProjeto == null)
                throw new Exception("Falha ao carregar informações, abra a página de projetos novamente.");
            string idItemProjeto = hdfIdItemProjeto.Value;
            if (idItemProjeto == null) throw new Exception("Falha ao carregar informações, abra a página de projetos novamente.");

            // Se não houver nenhum produto cadastrado no pedido (e no ambiente passado)
            if (!String.IsNullOrEmpty(idItemProjeto) && MaterialItemProjetoDAO.Instance.GetCountReal(Glass.Conversoes.StrParaUint(idItemProjeto)) == 0)
                grdMaterialProjeto.Rows[0].Visible = false;
            else if (String.IsNullOrEmpty(idItemProjeto))
                grdMaterialProjeto.Visible = false;
        }

        protected void grdItemProjeto_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            // Editar item_projeto
            if (e.CommandName == "EditarItem")
            {
                url = Request.Url.ToString();
                if (Request["idOrcamento"] != null)
                {
                    int start = url.IndexOf("IdProdOrca=", StringComparison.Ordinal) + 11;
                    uint? idProdOrca = ProdutosOrcamentoDAO.Instance.GetIdByIdItemProjeto(Glass.Conversoes.StrParaUint(e.CommandArgument.ToString()));
                    url = url.Substring(0, start) + idProdOrca + url.Substring(url.IndexOf("&", start, StringComparison.Ordinal));
                }
                else if (Request["idPedido"] != null)
                {
                    int start = url.IndexOf("IdAmbientePedido=", StringComparison.Ordinal) + 17;
                    uint? idAmbientePedido = AmbientePedidoDAO.Instance.GetIdByItemProjeto(Glass.Conversoes.StrParaUint(e.CommandArgument.ToString()));
                    url = url.Substring(0, start) + idAmbientePedido + url.Substring(url.IndexOf("&", start, StringComparison.Ordinal));
                }
                else if (Request["idPedidoEspelho"] != null)
                {
                    int start = url.IndexOf("IdAmbientePedidoEspelho=", StringComparison.Ordinal) + 24;
                    uint? idAmbientePedido = AmbientePedidoEspelhoDAO.Instance.GetIdByItemProjeto(Glass.Conversoes.StrParaUint(e.CommandArgument.ToString()));
                    url = url.Substring(0, start) + idAmbientePedido + url.Substring(url.IndexOf("&", start, StringComparison.Ordinal));
                }

                // Só redireciona a página se o chamado for feito de um botão
                // (O parâmetro sender será passado como null ao confirmar o projeto pela primeira vez)
                if (sender != null)
                    Response.Redirect(url);
            }
            else if (e.CommandName == "ExcluirProjeto")
            {
                ExcluirProjeto(Glass.Conversoes.StrParaUint(e.CommandArgument.ToString()));
            }
        }

        #endregion

        #region Eventos DataSource

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
            }
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
                string msg = ex.InnerException.Message ?? ex.Message;
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
                ConfirmaProjeto();
                
                //Caso o produto for Material Projeto Modelo mostra outra mensagem de alerta
                if(MaterialItemProjetoDAO.Instance.ObtemIdMaterProjMod(null, idMaterItemProj))
                {
                    MensagemAlerta.ShowMsg("Não é possivel alterar as medidas e quantidade deste material!", Page);
                }
                else
                {
                    MensagemAlerta.ShowMsg("Material atualizado com sucesso!", Page);
                }
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
                Glass.MensagemAlerta.ShowMsg("Confirme o projeto para aplicar esta alteração.", Page);
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

            if (itemProj == null)
                return;

            // Pega o modelo selecionado
            ProjetoModelo modelo = ProjetoModeloDAO.Instance.GetElementByPrimaryKey(itemProj.IdProjetoModelo);

            #region Busca imagem associada ao modelo escolhido

            dtvImagem.DataBind();
            dtvImagemMini.DataBind();

            #endregion

            // Monta tabela com Medidas da Área de Instalação
            // Se o código do modelo for OTR01, não monta tabela de medidas
            if (modelo.Codigo != "OTR01")
                UtilsProjeto.CreateTableMedInst(ref tbMedInst, itemProj, modelo, Request["pcp"] != null);

            try
            {
                // Se o código do modelo for OTR01, não monta tabela de peças
                if (modelo.Codigo != "OTR01")
                {
                    if (edit) // Monta a tabela de peças com as medidas já existentes
                        UtilsProjeto.CreateTablePecasItemProjeto(ref tbPecaModelo, itemProj.IdItemProjeto, itemProj, AlterarMedidasPecas(), Request["visualizar"] == "1", Request["Parceiro"] == "true");
                    else // Monta a tabela de peças com valores padrões
                        UtilsProjeto.CreateTablePecasModelo(ref tbPecaModelo, null, itemProj, AlterarMedidasPecas(), Request["visualizar"] == "1", Request["Parceiro"] == "true");
                }
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ShowMsg(ex.Message, Page);
            }

            // Mostra o ambiente cadastrado neste item_projeto
            if (txtAmbiente.Text == String.Empty)
                txtAmbiente.Text = itemProj.Ambiente;

            // Se o campo ambiente estiver vazio, busca o ped cli do pedido, se a empresa estiver configurada para isso
            if (txtAmbiente.Text == String.Empty && ProjetoConfig.BuscarPedCliAoInserirProjeto &&
                (!String.IsNullOrEmpty(hdfIdPedidoOriginal.Value) || !String.IsNullOrEmpty(hdfIdPedidoEspelho.Value)))
                txtAmbiente.Text = PedidoDAO.Instance.ObtemPedCli(null, Glass.Conversoes.StrParaUint(!String.IsNullOrEmpty(hdfIdPedidoOriginal.Value) ?
                    hdfIdPedidoOriginal.Value : hdfIdPedidoEspelho.Value));

            // Define se o projeto está sendo visualizado após confirmado
            bool readOnly = Request["pcp"] == "1" && Request["visualizar"] == "1";

            bool alterarFigura = readOnly && itemProj.IdPedidoEspelho > 0;
            if (alterarFigura)
            {
                HiddenField alt = tbPecaModelo.Rows[0].Cells[0].FindControl("hdfAlteraImagemPeca") as HiddenField;
                alterarFigura = alt != null && bool.Parse(alt.Value);
            }

            // Mostra tabela para inserir kit se o projeto não for cálculo de vidro apenas, o modelo do item_projeto for do grupo 
            // Correr com Kit e não houver nenhum produto do grupo Kit cadastrado nos materiais deste itemProjeto
            tbInsKit.Visible = !itemProj.ApenasVidros && !readOnly && (modelo.IdGrupoModelo == (uint)UtilsProjeto.GrupoModelo.CorrerComKit08mm ||
                modelo.IdGrupoModelo == (uint)UtilsProjeto.GrupoModelo.CorrerComKit10mm ||
                MaterialProjetoModeloDAO.Instance.RequerKit(itemProj.IdItemProjeto)) && !MaterialItemProjetoDAO.Instance.ExistsKit(itemProj.IdItemProjeto);

            // Mostra tabela para inserir tubo se não tiver o produto Tubo nos materiais, se o projeto não for cálculo apenas de vidro e
            // o modelo utilizado neste projeto requerer tubo
            tbInsTubo.Visible = !itemProj.ApenasVidros && !readOnly && MaterialProjetoModeloDAO.Instance.RequerTubo(itemProj.IdItemProjeto) &&
                !MaterialItemProjetoDAO.Instance.ExistsTubo(itemProj.IdItemProjeto);

            var parceiro = Request["Parceiro"] == "true" ? true : false;

            // Visibilidade de campos relacionados ao modelo de projeto
            bool visib = true;
            tbSubtotal.Visible = visib;
            tbAmbiente.Visible = !readOnly && itemProj.EditDeleteVisible && visib;
            lblMateriais.Visible = visib;
            lblMedidas.Visible = !readOnly && itemProj.EditDeleteVisible && modelo.Codigo != "OTR01";
            pchTbTipoInst.Visible = itemProj.EditDeleteVisible && modelo.Codigo != "OTR01";
            tbPecaModelo.Visible = (!readOnly || alterarFigura) && itemProj.EditDeleteVisible && modelo.Codigo != "OTR01";
            lblMedidasInst.Visible = !readOnly && itemProj.EditDeleteVisible && modelo.Codigo != "OTR01";
            btnConfAmbiente.Visible = itemProj.EditDeleteVisible && modelo.Codigo == "OTR01";
            grdMaterialProjeto.Visible = visib;
            grdItemProjeto.Visible = visib;
            btnNovoCalculo.Visible = !readOnly && visib;
            btnCalcMed.Visible = !itemProj.MedidaExata;
            ctrlCorItemProjeto2.Visible = !parceiro? !readOnly && visib :
                Configuracoes.ProjetoConfig.TelaCadastroParceiros.EsconderCamposAlteraCorItemProjeto ? false : !readOnly && visib;

            // Se for visualização, esconde botões de cálculo e trocar o botão fechar por voltar
            if (readOnly)
            {
                btnCalcMed.Visible = false;
                btnConfAmbiente.Visible = false;
                btnConfCalc.Visible = false;
                btnExcluirProjeto.Visible = false;

                btnFechar.Text = "Voltar";
                btnFechar.OnClientClick = "history.back(); return false;";

                // Desabilita textboxes
                foreach (TableCell td in tbMedInst.Controls[1].Controls)
                    if (td.Controls[0].GetType() == typeof(TextBox))
                        ((TextBox)td.Controls[0]).Enabled = false;
            }

            btnNovoCalculoDupl.Visible = !readOnly && visib;
            btnNovoCalculoDupl.Text = btnNovoCalculo.Text + " (" + modelo.Codigo + ")";
            hdfDuplicarCodigo.Value = itemProj.IdProjetoModelo.ToString();
            hdfDuplicarEspessura.Value = itemProj.EspessuraVidro.ToString();
            hdfDuplicarCorVidro.Value = itemProj.IdCorVidro.ToString();
            hdfDuplicarCorAluminio.Value = itemProj.IdCorAluminio.ToString();
            hdfDuplicarCorFerragem.Value = itemProj.IdCorFerragem.ToString();
            hdfDuplicarApenasVidros.Value = itemProj.ApenasVidros.ToString().ToLower();
            hdfDuplicarMedidaExata.Value = itemProj.MedidaExata.ToString().ToLower();

            grdMaterialProjeto.ShowFooter = !readOnly;

            CalculaTotalItemProj();

            // Deve realizar o bind na grdMaterialProjeto apenas se não for postback, senão
            // ao inserir um novo material à mão, os valores são apagados.
            if (!IsPostBack)
            {
                grdMaterialProjeto.DataBind();
                grdItemProjeto.DataBind();
            }

            grdItemProjeto.Columns[0].Visible = !readOnly;
            grdItemProjeto.Columns[4].Visible = !readOnly;
            grdMaterialProjeto.Columns[0].Visible = !readOnly;
        }

        #endregion

        #region Metodos Ajax

        [Ajax.AjaxMethod]
        public string IsProdutoObra(string idPedido, string codInterno)
        {
            uint? idObra = PedidoDAO.Instance.GetIdObra(null, Glass.Conversoes.StrParaUint(idPedido));
            if (idObra > 0)
            {
                ProdutoObraDAO.DadosProdutoObra retorno = ProdutoObraDAO.Instance.IsProdutoObra(idObra.Value, codInterno);
                if (!retorno.ProdutoValido)
                    return "Erro;" + retorno.MensagemErro;
                else
                    return "Ok;" + retorno.ValorUnitProduto + ";" + retorno.M2Produto + ";" + retorno.AlterarValorUnitario.ToString().ToLower();
            }

            return "Ok;0;0;" + PedidoConfig.DadosPedido.AlterarValorUnitarioProduto.ToString().ToLower();
        }

        [Ajax.AjaxMethod]
        public string GetTamanhoMaximoProduto(string idPedido, string codInterno, string totM2Produto)
        {
            uint? idObra = PedidoDAO.Instance.GetIdObra(null, Glass.Conversoes.StrParaUint(idPedido));
            if (idObra > 0 && PedidoConfig.DadosPedido.UsarControleNovoObra)
            {
                ProdutoObra prod = ProdutoObraDAO.Instance.GetByCodInterno(idObra.Value, codInterno);
                if (prod == null)
                    return "Erro;Esse produto não está cadastrado no pagamento antecipado.";

                float tamanhoProdutos = ProdutosPedidoDAO.Instance.TotalMedidasObra(idObra.Value, codInterno, null);
                float tamanhoProduto = float.Parse(totM2Produto.Replace(".", ","));

                return "Ok;" + (prod.TamanhoMaximo - tamanhoProdutos + tamanhoProduto);
            }

            return "Ok;0";
        }

        [Ajax.AjaxMethod]
        public string GetValorMinimo(string codInterno, string tipoEntrega, string idCliente, string revenda,
            string reposicao, string tipoPedido, string idMaterItemProjStr, string percDescontoQtdeStr, string idPedido)
        {
            float percDescontoQtde = !String.IsNullOrEmpty(percDescontoQtdeStr) ? float.Parse(percDescontoQtdeStr.Replace(".", ",")) : 0;
            uint idMaterItemProj;

            if (uint.TryParse(idMaterItemProjStr, out idMaterItemProj))
            {
                if (ItemProjetoDAO.Instance.ObtemIdPedidoEspelho(MaterialItemProjetoDAO.Instance.ObtemIdItemProjeto(idMaterItemProj)) > 0)
                    return MaterialItemProjetoDAO.Instance.ObtemValor(idMaterItemProj).ToString(CultureInfo.InvariantCulture);
                else
                    return ProdutoDAO.Instance.GetValorMinimo(idMaterItemProj, ProdutoDAO.TipoBuscaValorMinimo.MaterialItemProjeto, revenda.ToLower() == "true",
                        percDescontoQtde, idPedido.StrParaIntNullable(), null, null).ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                // Recupera o valor mínimo do produto
                int? tipoEntr = !String.IsNullOrEmpty(tipoEntrega) ? (int?)Glass.Conversoes.StrParaInt(tipoEntrega) : null;
                uint? idCli = !String.IsNullOrEmpty(idCliente) ? (uint?)Glass.Conversoes.StrParaUint(idCliente) : null;
                return ProdutoDAO.Instance.GetValorMinimo(ProdutoDAO.Instance.ObtemIdProd(codInterno), tipoEntr, idCli, revenda == "true",
                    reposicao == "true", percDescontoQtde, idPedido.StrParaIntNullable(), null, null).ToString(CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Cria um novo Item de Projeto
        /// </summary>
        [Ajax.AjaxMethod()]
        public string NovoItemProjeto(string idOrcamentoString, string idAmbienteOrcaString, string idPedidoString,
            string idAmbientePedString, string idPedidoEspString, string idAmbientePedEspString, string idProjetoModelo,
            string espessuraVidro, string idCorVidro, string idCorAluminio, string idCorFerragem, string apenasVidros, string medidaExata)
        {
            try
            {
                uint? idOrcamento =
                    !String.IsNullOrEmpty(idOrcamentoString) ?
                        (uint?)Glass.Conversoes.StrParaUint(idOrcamentoString) : null;

                uint? idAmbienteOrca =
                    !String.IsNullOrEmpty(idAmbienteOrcaString) ?
                        (uint?)Glass.Conversoes.StrParaUint(idAmbienteOrcaString) : null;

                uint? idPedido =
                    !String.IsNullOrEmpty(idPedidoString) ?
                        (uint?)Glass.Conversoes.StrParaUint(idPedidoString) : null;

                uint? idAmbientePedido =
                    !String.IsNullOrEmpty(idAmbientePedString) ?
                        (uint?)Glass.Conversoes.StrParaUint(idAmbientePedString) : null;

                uint? idPedidoEspelho =
                    !String.IsNullOrEmpty(idPedidoEspString) ?
                        (uint?)Glass.Conversoes.StrParaUint(idPedidoEspString) : null;

                uint? idAmbientePedidoEspelho =
                    !String.IsNullOrEmpty(idAmbientePedEspString) ?
                        (uint?)Glass.Conversoes.StrParaUint(idAmbientePedEspString) : null;

                if (idPedido > 0)
                {
                    Glass.Data.Model.Pedido.SituacaoPedido situacao = PedidoDAO.Instance.ObtemSituacao(null, idPedido.Value);
                    if (situacao != Glass.Data.Model.Pedido.SituacaoPedido.Ativo && situacao != Glass.Data.Model.Pedido.SituacaoPedido.AtivoConferencia &&
                        situacao != Glass.Data.Model.Pedido.SituacaoPedido.EmConferencia)
                        return "Erro;O pedido precisa estar aberto para incluir novos projetos no mesmo.";
                }
                else if (idPedidoEspelho > 0)
                {
                    PedidoEspelho.SituacaoPedido situacao = PedidoEspelhoDAO.Instance.ObtemSituacao(idPedidoEspelho.Value);
                    if (situacao != PedidoEspelho.SituacaoPedido.Aberto && situacao != PedidoEspelho.SituacaoPedido.ImpressoComum)
                        return "Erro;O pedido precisa estar aberto para incluir novos projetos no mesmo.";
                }

                var coresVidroPossives = ProjetoModeloDAO.Instance.ObtemIdsCorVidro(idProjetoModelo.StrParaUint());

                if (coresVidroPossives != null && !coresVidroPossives.Split(',').Contains(idCorVidro))
                    return "Erro;O projeto não pode ser duplicado pois a cor do vidro do mesmo não está mais disponível.";

                #region Inserir novo item projeto

                var itemProj = ItemProjetoDAO.Instance.NovoItemProjetoVazioComTransacao(null, idOrcamento, idAmbienteOrca, idPedido, idAmbientePedido,
                    idPedidoEspelho, idAmbientePedidoEspelho, idProjetoModelo.StrParaUint(), espessuraVidro.StrParaIntNullable(),
                    idCorVidro.StrParaUint(), idCorAluminio.StrParaUint(), idCorFerragem.StrParaUint(), apenasVidros == "true", medidaExata == "true", true);

                #endregion

                if (itemProj == null || itemProj.IdItemProjeto == 0)
                    return "Erro;Falha ao inserir item no projeto. Inserção retornou 0.";

                if (idPedido > 0 && (idAmbientePedido == null || idAmbientePedido == 0))
                    return "ok;" + itemProj.IdItemProjeto + ";hdfIdAmbientePedido;" + AmbientePedidoDAO.Instance.ObtemIdAmbiente(itemProj.IdItemProjeto);

                if (idPedidoEspelho > 0 && (idAmbientePedidoEspelho == null || idAmbientePedidoEspelho == 0))
                    return "ok;" + itemProj.IdItemProjeto + ";hdfIdAmbientePedidoEspelho;" + AmbientePedidoEspelhoDAO.Instance.ObtemIdAmbiente(itemProj.IdItemProjeto);

                return "ok;" + itemProj.IdItemProjeto;
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
        public string GetProduto(string idPedido, string idOrcamento, string codInterno, string tipoEntrega, string revenda, string idCliente)
        {
            Produto prod = ProdutoDAO.Instance.GetByCodInterno(codInterno);

            if (prod == null)
                return "Erro;Não existe produto com o código informado.";
            else if (prod.Compra)
                return "Erro;Produto utilizado apenas na compra.";
            else if (prod.Situacao == Glass.Situacao.Inativo)
                return "Erro;Produto inativo." + (!String.IsNullOrEmpty(prod.Obs) ? " Obs: " + prod.Obs : "");
            else
            {
                /* Chamado 47194. */
                if (prod.IdSubgrupoProd > 0 && (idOrcamento.StrParaIntNullable() > 0 || idPedido.StrParaIntNullable() > 0))
                {
                    var idLoja = idOrcamento.StrParaIntNullable() > 0 ? OrcamentoDAO.Instance.GetIdLoja(null, idOrcamento.StrParaUint()) :
                        idPedido.StrParaIntNullable() > 0 ? PedidoDAO.Instance.ObtemIdLoja(null, idPedido.StrParaUint()) : 0;

                    if (idLoja > 0)
                    {
                        var idsLojaSubgrupoProd = SubgrupoProdDAO.Instance.ObterIdsLoja(null, prod.IdSubgrupoProd.Value);

                        if (idsLojaSubgrupoProd.Any() && !idsLojaSubgrupoProd.Any(f => f == idLoja))
                            return "Erro;Esse produto não pode ser utilizado, pois, as lojas do seu subgrupo são diferentes da loja do pedido/projeto.";
                    }
                }

                bool pedidoReposicao = false;
                bool isPedidoMaoObraEspecial = false;

                if (!String.IsNullOrEmpty(idPedido) && idPedido != "0")
                {
                    pedidoReposicao = PedidoDAO.Instance.ObtemTipoVenda(null, Glass.Conversoes.StrParaUint(idPedido)) == (int)Glass.Data.Model.Pedido.TipoVendaPedido.Reposição;
                    isPedidoMaoObraEspecial = PedidoDAO.Instance.GetTipoPedido(null, Glass.Conversoes.StrParaUint(idPedido)) == Glass.Data.Model.Pedido.TipoPedidoEnum.MaoDeObraEspecial;
                }

                if (isPedidoMaoObraEspecial)
                {
                    if (prod.IdGrupoProd != (uint)Glass.Data.Model.NomeGrupoProd.Vidro || SubgrupoProdDAO.Instance.IsSubgrupoProducao(prod.IdGrupoProd, prod.IdSubgrupoProd))
                        return "Erro;Apenas produtos do grupo 'Vidro', e que não são marcados como 'Produtos para Estoque', podem ser utilizados nesse pedido.";
                }

                if (UserInfo.GetUserInfo.IsCliente && SubgrupoProdDAO.Instance.ObterBloquearEcommerce(null, prod.IdSubgrupoProd.Value))
                    return "Erro;Este produto não pode ser selecionado na plataforma e-commerce. Entre em contato com a empresa para mais informações";

                string retorno = "Prod;" + prod.IdProd + ";" + prod.Descricao;

                decimal valorProduto = 0;

                if (!isPedidoMaoObraEspecial)
                {
                    // Recupera o valor de tabela do produto
                    int? tipoEntr = !String.IsNullOrEmpty(tipoEntrega) ? (int?)Glass.Conversoes.StrParaInt(tipoEntrega) : null;
                    uint? idCli = !String.IsNullOrEmpty(idCliente) ? (uint?)Glass.Conversoes.StrParaUint(idCliente) : null;
                    float percDescontoQtde = 0; // !String.IsNullOrEmpty(percDescontoQtdeStr) ? float.Parse(percDescontoQtdeStr.Replace(".", ",")) : 0;
                    valorProduto = ProdutoDAO.Instance.GetValorTabela(prod.IdProd, tipoEntr, idCli, revenda == "true",
                        pedidoReposicao, percDescontoQtde, idPedido.StrParaIntNullable(), null, null);

                    /* if (PedidoConfig.Comissao.ComissaoPedido && PedidoConfig.Comissao.ComissaoAlteraValor)
                        valorProduto = valorProduto / (decimal)((100 - float.Parse(percComissao)) / 100); */
                }

                retorno += ";" + valorProduto.ToString("F2");

                retorno += ";" + Glass.Data.DAL.GrupoProdDAO.Instance.IsVidro(prod.IdGrupoProd).ToString().ToLower() + ";" +
                    Glass.Data.DAL.GrupoProdDAO.Instance.IsAluminio(prod.IdGrupoProd).ToString().ToLower() + ";" +
                    (prod.AtivarAreaMinima ? prod.AreaMinima.ToString(CultureInfo.InvariantCulture).Replace(',', '.') : "0");

                retorno += ";" + Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo(prod.IdGrupoProd, prod.IdSubgrupoProd);
                retorno += ";" + prod.Espessura;
                retorno += ";" + prod.Altura;
                retorno += ";" + prod.Largura;
                retorno += ";" + (prod.IdProcesso.GetValueOrDefault() > 0 ? EtiquetaProcessoDAO.Instance.GetCodInternoByIds(prod.IdProcesso.GetValueOrDefault().ToString()) : "");
                retorno += ";" + (prod.IdAplicacao.GetValueOrDefault() > 0 ? EtiquetaAplicacaoDAO.Instance.GetCodInternoByIds(prod.IdAplicacao.GetValueOrDefault().ToString()) : "");
                return retorno;
            }
        }

        [Ajax.AjaxMethod()]
        public string GetVidro(string idPedido, string idOrcamento, string codInterno)
        {
            Produto prod = ProdutoDAO.Instance.GetByCodInterno(codInterno);

            if (prod == null)
                return "Erro;Não existe produto com o código informado.";
            else if (prod.Situacao == Glass.Situacao.Inativo)
                return "Erro;Produto inativo." + (!String.IsNullOrEmpty(prod.Obs) ? " Obs: " + prod.Obs : "");
            else if (prod.IdGrupoProd != (uint)Glass.Data.Model.NomeGrupoProd.Vidro)
                return "Erro;Produto não é vidro.";

            /* Chamado 47194. */
            if (prod.IdSubgrupoProd > 0 && (idOrcamento.StrParaIntNullable() > 0 || idPedido.StrParaIntNullable() > 0))
            {
                var idLoja = idOrcamento.StrParaIntNullable() > 0 ? OrcamentoDAO.Instance.GetIdLoja(null, idOrcamento.StrParaUint()) :
                    idPedido.StrParaIntNullable() > 0 ? PedidoDAO.Instance.ObtemIdLoja(null, idPedido.StrParaUint()) : 0;

                if (idLoja > 0)
                {
                    var idsLojaSubgrupoProd = SubgrupoProdDAO.Instance.ObterIdsLoja(null, prod.IdSubgrupoProd.Value);

                    if (idsLojaSubgrupoProd.Any() && !idsLojaSubgrupoProd.Any(f => f == idLoja))
                        return "Erro;Esse produto não pode ser utilizado, pois, as lojas do seu subgrupo são diferentes da loja do pedido/projeto.";
                }
            }

            return "Prod;" + prod.IdProd + ";" + prod.Descricao;
        }

        /// <summary>
        /// Verifica se o item projeto está conferido
        /// </summary>
        [Ajax.AjaxMethod()]
        public string EstaConferido(string idItemProjeto)
        {
            return ItemProjetoDAO.Instance.EstaConferido(Glass.Conversoes.StrParaUint(idItemProjeto)).ToString().ToLower();
        }

        /// <summary>
        /// Verifica se o item projeto está conferido
        /// </summary>
        [Ajax.AjaxMethod()]
        public string PecaPossuiMaterial(string idPecaItemProj)
        {
            if (string.IsNullOrEmpty(idPecaItemProj))
                return "false";

            return MaterialItemProjetoDAO.Instance.PecaPossuiMaterial(idPecaItemProj.StrParaInt()).ToString().ToLower();
        }

        [Ajax.AjaxMethod()]
        public string SalvaObsItemProjeto(string idItemProjeto, string obs)
        {
            try
            {
                if (String.IsNullOrEmpty(idItemProjeto) || Conversoes.StrParaInt(idItemProjeto) == 0)
                    return "Erro;O projeto não foi informado. Feche a tela e entre novamente";

                ItemProjetoDAO.Instance.AtualizaObs(Glass.Conversoes.StrParaUint(idItemProjeto), obs);

                return "ok";
            }
            catch (Exception ex)
            {
                return "Erro;" + Glass.MensagemAlerta.FormatErrorMsg("Falha ao salvar observação.", ex);
            }
        }

        [Ajax.AjaxMethod()]
        public string CriarProjetoCADProject(string codProjeto, string idPecaItemProjeto, string url, string pcp)
        {
            return UtilsProjeto.CriarProjetoCADProject(codProjeto, idPecaItemProjeto.StrParaUint(), url, bool.Parse(pcp));
        }

        [Ajax.AjaxMethod()]
        public void ValidaCorProdutoProjeto(string codInterno, string idItemProjeto)
        {
            var idProjetoModelo = ItemProjetoDAO.Instance.ObtemIdProjetoModelo(null, idItemProjeto.StrParaUint());
            var idsCorArr = ProjetoModeloDAO.Instance.ObtemIdsCorVidroArr(null, idProjetoModelo);
            
            if (idsCorArr.Count() == 0)
                return;

            var idProd = ProdutoDAO.Instance.ObtemIdProd(codInterno);
            var idCor = ProdutoDAO.Instance.ObtemIdCorVidro(idProd);
            if (!idsCorArr.Contains((uint)idCor.GetValueOrDefault(0)))
            {
                var cor = CorVidroDAO.Instance.GetNome(null, (uint)idCor.Value);
                var projeto = ProjetoModeloDAO.Instance.ObtemCodigo(null, idProjetoModelo);
                throw new Exception(string.Format("Não é possivel utilizar a cor: {0} no projeto {1}, pois ela não esta vinculada ao mesmo.", cor, projeto));
            }

        }

        #endregion

        #region Insere MaterialItemProjeto

        protected void lnkInsProd_Click(object sender, EventArgs e)
        {
            if (grdMaterialProjeto.PageCount > 1)
                grdMaterialProjeto.PageIndex = grdMaterialProjeto.PageCount - 1;

            Glass.UI.Web.Controls.ctrlBenef benef = (Glass.UI.Web.Controls.ctrlBenef)grdMaterialProjeto.FooterRow.FindControl("ctrlBenefInserir");

            uint idItemProj = Glass.Conversoes.StrParaUint(hdfIdItemProjeto.Value);
            uint idProd = Glass.Conversoes.StrParaUint(hdfIdProdMater.Value);
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
            materItem.IdItemProjeto = idItemProj;
            materItem.Qtde = Glass.Conversoes.StrParaInt(((TextBox)grdMaterialProjeto.FooterRow.FindControl("txtQtdeIns")).Text);
            materItem.Valor = Glass.Conversoes.StrParaDecimal(((TextBox)grdMaterialProjeto.FooterRow.FindControl("txtValorIns")).Text);
            materItem.Altura = altura;
            materItem.AlturaCalc = alturaCalc;
            materItem.Largura = largura;
            materItem.IdProd = idProd;
            materItem.Espessura = Glass.Conversoes.StrParaFloat(espessura);
            if (!String.IsNullOrEmpty(idAplicacaoStr)) materItem.IdAplicacao = Glass.Conversoes.StrParaUint(idAplicacaoStr);
            if (!String.IsNullOrEmpty(idProcessoStr)) materItem.IdProcesso = Glass.Conversoes.StrParaUint(idProcessoStr);
            materItem.Beneficiamentos = benef.Beneficiamentos;
            materItem.PedCli = ((TextBox)grdMaterialProjeto.FooterRow.FindControl("txtPedCli")).Text;

            if (grdMaterialProjeto.Columns[12].Visible)
                materItem.Obs = ((TextBox)grdMaterialProjeto.FooterRow.FindControl("txtObsIns")).Text;

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

                CalculaTotalItemProj();

                ConfirmaProjeto();
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao incluir produto.", ex, Page);
                return;
            }
        }

        #endregion

        #region Calcula projeto

        /// <summary>
        /// Calcula Medidas
        /// </summary>
        protected void btnCalcMed_Click(object sender, EventArgs e)
        {
            uint idItemProj = Glass.Conversoes.StrParaUint(hdfIdItemProjeto.Value);

            // Busca o item_projeto sendo calculado
            ItemProjeto itemProjeto = ItemProjetoDAO.Instance.GetElement(idItemProj);

            if (itemProjeto == null)
                return;

            //Chamado 54871
            idItemProjeto = idItemProj;

            // Este bloqueio deve ser mantido para que as medidas calculadas não fiquem diferentes das imagens editadas.
            // Caso o projeto já tenha sido editado, o usuário poderá somente confirmá-lo.
            if (!itemProjeto.MedidaExata && PecaItemProjetoDAO.Instance.ObtemEditarImagemProjeto(Glass.Conversoes.StrParaUint(hdfIdItemProjeto.Value)))
            {
                Glass.MensagemAlerta.ShowMsg("Medidas não podem ser calculadas uma vez que houve edição manual de alguma imagem do projeto", Page);
                return;
            }

            uint? idObra = null, idCliente = null;
            int? tipoEntrega = null;

            if (!String.IsNullOrEmpty(Request["IdOrcamento"]))
            {
                uint idOrcamento = Glass.Conversoes.StrParaUint(Request["idOrcamento"]);
                idCliente = OrcamentoDAO.Instance.ObtemIdCliente(idOrcamento);
                tipoEntrega = OrcamentoDAO.Instance.ObtemTipoEntrega(idOrcamento);
            }

            if (!String.IsNullOrEmpty(Request["IdPedido"]))
            {
                uint idPedido = Glass.Conversoes.StrParaUint(Request["idPedido"]);
                idCliente = PedidoDAO.Instance.ObtemIdCliente(null, idPedido);
                tipoEntrega = PedidoDAO.Instance.ObtemTipoEntrega(idPedido);

                idObra = PedidoConfig.DadosPedido.UsarControleNovoObra ? PedidoDAO.Instance.GetIdObra(null, idPedido) : null;
            }

            if (!String.IsNullOrEmpty(Request["IdPedidoEspelho"]))
            {
                uint idPedido = Glass.Conversoes.StrParaUint(Request["idPedidoEspelho"]);
                idCliente = PedidoDAO.Instance.ObtemIdCliente(null, idPedido);
                tipoEntrega = PedidoDAO.Instance.ObtemTipoEntrega(idPedido);

                idObra = PedidoConfig.DadosPedido.UsarControleNovoObra ? PedidoDAO.Instance.GetIdObra(null, idPedido) : null;
            }

            // Busca o grupo deste modelo
            uint idGrupoModelo = ItemProjetoDAO.Instance.GetIdGrupoModelo(idItemProj);

            #region Verifica/Insere Tubo e Kit

            // Se o projeto não for cálculo de vidro apenas, o modelo do item_projeto for do grupo Correr com Kit Instalação e 
            // não houver nenhum produto do grupo Kit cadastrado nos materiais deste itemProjeto, obriga a cadastrar um produto do grupo Kit
            if (!ItemProjetoDAO.Instance.ApenasVidros(idItemProjeto) && (idGrupoModelo == (uint)UtilsProjeto.GrupoModelo.CorrerComKit08mm ||
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
            if (!ItemProjetoDAO.Instance.ApenasVidros(idItemProjeto) && MaterialProjetoModeloDAO.Instance.RequerTubo(idItemProjeto) &&
                !MaterialItemProjetoDAO.Instance.ExistsTubo(idItemProjeto))
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
                var lstPecaModelo = UtilsProjeto.CalcularMedidasPecasComBaseNaTelaComTransacao(modelo, itemProjeto, tbMedInst, tbPecaModelo, true, AlterarMedidasPecas(), false, out retornoValidacao);

                // Insere Peças na tabela peca_item_projeto
                PecaItemProjetoDAO.Instance.InsertFromPecaModelo(itemProjeto, ref lstPecaModelo);

                // Insere as peças de vidro apenas se todas as Peça Projeto Modelo tiver idProd
                var inserirPecasVidro = !lstPecaModelo.Any(f => f.IdProd == 0);
                if (inserirPecasVidro)
                    // Insere Peças na tabela material_item_projeto
                    MaterialItemProjetoDAO.Instance.InserePecasVidro(idObra, idCliente, tipoEntrega, itemProjeto, modelo, lstPecaModelo);

                // Atualiza qtds dos materiais apenas
                MaterialItemProjetoDAO.Instance.AtualizaQtd(idObra, idCliente, tipoEntrega, itemProjeto, modelo);

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
                UtilsProjeto.CreateTablePecasItemProjeto(ref tbPecaModelo, itemProjeto.IdItemProjeto, itemProjeto, AlterarMedidasPecas(), Request["visualizar"] == "1", Request["Parceiro"] == "true");
                
                // Mostra o total deste item e do projeto
                CalculaTotalItemProj();

                grdMaterialProjeto.DataBind();
                grdItemProjeto.DataBind();

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

        #endregion

        #region Confirma projeto

        protected void btnConfCalc_Load(object sender, EventArgs e)
        {
            if (Request["idPedido"] != null && PedidoDAO.Instance.IsGeradoParceiro(null, Glass.Conversoes.StrParaUint(Request["idPedido"])))
                btnConfCalc.OnClientClick = "if (!confirm('AS MEDIDAS CALCULADAS NESSE PROJETO SÃO DE SUA RESPONSABILIDADE.\\nDESEJA CONFIRMAR AS MEDIDAS?')) return false";
        }

        /// <summary>
        /// Confirma cálculo
        /// </summary>
        protected void btnConfCalc_Click(object sender, EventArgs e)
        {
            // Verifica se o ambiente foi informado
            if (txtAmbiente.Text == String.Empty)
            {
                Glass.MensagemAlerta.ShowMsg("Informe o ambiente.", Page);
                return;
            }

            try
            {
                // Redirecionamento comentado para resolver chamado 9526
                var redireciona = /*ConfirmaProjeto() ? "redirectUrl('" + url + "');\n" :*/ "";
                if (!ConfirmaProjeto())
                    return;

                var imagemEditada = PecaItemProjetoDAO.Instance.ObtemEditarImagemProjeto(Glass.Conversoes.StrParaUint(hdfIdItemProjeto.Value));
                string msgImagemEditada = imagemEditada ? ", este projeto possui imagens editadas, devido a isso alterações de medidas não serão aplicadas às mesmas. CONFIRA NOVAMENTE AS IMAGENS DAS PEÇAS DESTE PROJETO." : "";

                ClientScript.RegisterClientScriptBlock(typeof(string), "Ok", "window.opener.refreshPage(); alert('Medidas confirmadas" + msgImagemEditada + "');" + redireciona, true);
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao confirmar projeto.", ex, Page);
                return;
            }
        }

        /// <summary>
        /// O retorno do método indica se o item foi inserido ou não (atualizado).
        /// </summary>
        /// <returns></returns>
        protected bool ConfirmaProjeto()
        {
            uint? idOrcamento = null;
            uint? idAmbienteOrca = null;
            uint? idPedido = null;
            uint? idAmbientePedido = null;
            uint? idPedidoEsp = null;
            uint? idAmbientePedidoEsp = null;

            var recarregarPagina = false;
            var erroAoConfirmar = false;
            var retornoValidacao = string.Empty;

            try
            {
                if (!String.IsNullOrEmpty(Request["idOrcamento"]))
                    idOrcamento = Glass.Conversoes.StrParaUint(Request["idOrcamento"]);

                if (!String.IsNullOrEmpty(Request["IdPedido"]))
                    idPedido = Glass.Conversoes.StrParaUint(Request["IdPedido"]);

                if (!String.IsNullOrEmpty(Request["IdPedidoEspelho"]))
                    idPedidoEsp = Glass.Conversoes.StrParaUint(Request["IdPedidoEspelho"]);

                if (!String.IsNullOrEmpty(hdfIdAmbienteOrca.Value))
                    idAmbienteOrca = Glass.Conversoes.StrParaUint(hdfIdAmbienteOrca.Value);
                else if (!String.IsNullOrEmpty(Request["IdAmbienteOrca"]))
                    idAmbienteOrca = Glass.Conversoes.StrParaUint(Request["IdAmbienteOrca"]);

                if (!String.IsNullOrEmpty(hdfIdAmbientePedido.Value))
                    idAmbientePedido = Glass.Conversoes.StrParaUint(hdfIdAmbientePedido.Value);
                else if (!String.IsNullOrEmpty(Request["IdAmbientePedido"]))
                    idAmbientePedido = Glass.Conversoes.StrParaUint(Request["IdAmbientePedido"]);

                if (!String.IsNullOrEmpty(hdfIdAmbientePedidoEspelho.Value))
                    idAmbientePedidoEsp = Glass.Conversoes.StrParaUint(hdfIdAmbientePedidoEspelho.Value);
                else if (!String.IsNullOrEmpty(Request["IdAmbientePedidoEspelho"]))
                    idAmbientePedidoEsp = Glass.Conversoes.StrParaUint(Request["IdAmbientePedidoEspelho"]);

                // Busca o item_projeto sendo calculado
                var itemProjeto = ItemProjetoDAO.Instance.GetElement(Glass.Conversoes.StrParaUint(hdfIdItemProjeto.Value));

                // Esta função não funciona corretamente, pois caso a pessoa queira mudar as medidas das peças direto nas peças no PCP não funciona
                if (PedidoConfig.LiberarPedido && !itemProjeto.Conferido && !itemProjeto.MedidaExata)
                    btnCalcMed_Click(btnCalcMed, EventArgs.Empty);

                // Verifica se as peças foram alteradas
                HiddenField hdfPecasAlteradas = (HiddenField)tbPecaModelo.FindControl("hdfPecasAlteradas");
                HiddenField hdfMedidasAlteradas = (HiddenField)tbPecaModelo.FindControl("hdfMedidasAlteradas");
                var pecasAlteradas = hdfPecasAlteradas != null && bool.Parse(hdfPecasAlteradas.Value);
                var medidasAlteradas = hdfMedidasAlteradas != null && bool.Parse(hdfMedidasAlteradas.Value);

                ItemProjetoDAO.Instance.Confirmar(itemProjeto, idOrcamento.GetValueOrDefault(), idPedido.GetValueOrDefault(),
                    idPedidoEsp.GetValueOrDefault(), idAmbienteOrca.GetValueOrDefault(), idAmbientePedido.GetValueOrDefault(),
                    idAmbientePedidoEsp.GetValueOrDefault(), !String.IsNullOrEmpty(txtAmbiente.Text) ? txtAmbiente.Text : "",
                    pecasAlteradas, AlterarMedidasPecas(), Request["visualizar"] == "1", ref tbPecaModelo, ref tbMedInst,
                    out retornoValidacao, ref medidasAlteradas, Request["Parceiro"] == "true");

                // Mostra o total deste item e do projeto
                CalculaTotalItemProj();

                grdMaterialProjeto.DataBind();
                grdItemProjeto.DataBind();

                // Atualiza imagem associada, podendo ter sido desenhado alguma medida na mesma
                dtvImagem.DataBind();

                /* Chamado 50798. */
                if (hdfPecasAlteradas != null)
                    hdfPecasAlteradas.Value = "false";

                /* Chamado 50798. */
                if (hdfMedidasAlteradas != null)
                    hdfMedidasAlteradas.Value = "false";

                // Recarrega a tela no caso do cadastro do item
                // No orçamento não é necessário verificar se o item foi inserido, 
                // já que o anterior é apagado do banco e o novo é sempre inserido
                if ((idOrcamento.GetValueOrDefault() > 0 /*&& idAmbienteOrca == 0 */) ||
                    (idPedido.GetValueOrDefault() > 0 && idAmbientePedido.GetValueOrDefault() == 0) ||
                    (idPedidoEsp.GetValueOrDefault() > 0 && idAmbientePedidoEsp.GetValueOrDefault() == 0))
                    recarregarPagina = true;
            }
            catch (Exception ex)
            {
                erroAoConfirmar = true;

                var idItemProj = hdfIdItemProjeto != null ? hdfIdItemProjeto.Value : "null";

                ErroDAO.Instance.InserirFromException(string.Format("Confirmar Projeto - IdItemProjeto: {0} IdOrcamento: {1} " +
                    "IdPedido: {2} IdPedidoEsp: {3} IdAmbienteOrca: {4} IdAmbientePedido: {5} IdAmbientePedidoEsp: {6}",
                    idItemProj, idOrcamento.GetValueOrDefault(), idPedido.GetValueOrDefault(), idPedidoEsp.GetValueOrDefault(),
                    idAmbienteOrca.GetValueOrDefault(), idAmbientePedido.GetValueOrDefault(), idAmbientePedidoEsp.GetValueOrDefault()), ex);

                // Marca que cálculo de projeto não foi conferido
                if (idPedido.GetValueOrDefault() > 0 || idPedidoEsp.GetValueOrDefault() > 0)
                    ItemProjetoDAO.Instance.CalculoNaoConferido(Glass.Conversoes.StrParaUint(hdfIdItemProjeto.Value));

                Glass.MensagemAlerta.ErrorMsg("Falha ao confirmar projeto.\n" + retornoValidacao, ex, Page);
            }

            if (!erroAoConfirmar &&
                !string.IsNullOrEmpty(retornoValidacao))
                MensagemAlerta.ShowMsg(retornoValidacao, Page);

            grdItemProjeto.DataBind();

            if (recarregarPagina)
                grdItemProjeto_RowCommand(null, new GridViewCommandEventArgs(null, new CommandEventArgs("EditarItem", idItemProjeto.ToString())));

            return !erroAoConfirmar && !retornoValidacao.ToLower().Contains("erro");
        }
    
        #endregion
    
        #region Exclui projeto
    
        /// <summary>
        /// Calcula Medidas
        /// </summary>
        protected void btnExcluirProjeto_Click(object sender, EventArgs e)
        {
            /* Chamado 18115. */
            btnNovoCalculoDupl.Visible = false;

            ExcluirProjeto(Glass.Conversoes.StrParaUint(hdfIdItemProjeto.Value));
    
            LimpaCalculo();
        }
    
        protected void ExcluirProjeto(uint idItemProjeto)
        {
            try
            {
                uint? idOrcamento = null;
                uint? idPedido = null;
                uint? idPedidoEsp = null;
    
                if (!String.IsNullOrEmpty(Request["idOrcamento"]))
                    idOrcamento = Glass.Conversoes.StrParaUint(Request["idOrcamento"]);
    
                if (!String.IsNullOrEmpty(Request["IdPedido"]))
                    idPedido = Glass.Conversoes.StrParaUint(Request["IdPedido"]);
    
                if (!String.IsNullOrEmpty(Request["IdPedidoEspelho"]))
                    idPedidoEsp = Glass.Conversoes.StrParaUint(Request["IdPedidoEspelho"]);
    
                // Verifica se o itemProjeto existe
                if (!ItemProjetoDAO.Instance.Exists(idItemProjeto))
                {
                    Glass.MensagemAlerta.ShowMsg("Este projeto já foi excluído.", Page);
                    return;
                }
    
                // Exclui o item_projeto
                ItemProjetoDAO.Instance.ExcluiProjeto(idItemProjeto, idOrcamento, idPedido, idPedidoEsp);
    
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
    
        #region Atualiza o ambiente para modelo "Outros"
    
        /// <summary>
        /// Confirmar somente o projeto de código OTR01. O botão fica visível somente para este projeto.
        /// </summary>
        protected void btnConfAmbiente_Click(object sender, EventArgs e)
        {
            
            // Verifica se o ambiente foi informado
            if (txtAmbiente.Text == String.Empty)
            {
                Glass.MensagemAlerta.ShowMsg("Informe o ambiente.", Page);
                return;
            }
    
            uint idItemProjeto = Glass.Conversoes.StrParaUint(hdfIdItemProjeto.Value);
            uint? idOrcamento = null;
            uint? idAmbienteOrca = null;
            uint? idPedido = null;
            uint? idAmbientePedido = null;
            uint? idPedidoEsp = null;
            uint? idAmbientePedidoEsp = null;
    
            try
            {
                if (!String.IsNullOrEmpty(Request["idOrcamento"]))
                    idOrcamento = Glass.Conversoes.StrParaUint(Request["idOrcamento"]);
    
                if (!String.IsNullOrEmpty(Request["IdAmbienteOrca"]))
                    idAmbienteOrca = Glass.Conversoes.StrParaUint(Request["IdAmbienteOrca"]);
    
                if (!String.IsNullOrEmpty(Request["IdPedido"]))
                    idPedido = Glass.Conversoes.StrParaUint(Request["IdPedido"]);
    
                if (!String.IsNullOrEmpty(Request["IdAmbientePedido"]))
                    idAmbientePedido = Glass.Conversoes.StrParaUint(Request["IdAmbientePedido"]);
                else if (!String.IsNullOrEmpty(hdfIdAmbientePedido.Value))
                    idAmbientePedido = Glass.Conversoes.StrParaUint(hdfIdAmbientePedido.Value);
    
                if (!String.IsNullOrEmpty(Request["IdPedidoEspelho"]))
                    idPedidoEsp = Glass.Conversoes.StrParaUint(Request["IdPedidoEspelho"]);
    
                if (!String.IsNullOrEmpty(Request["IdAmbientePedidoEspelho"]))
                    idAmbientePedidoEsp = Glass.Conversoes.StrParaUint(Request["IdAmbientePedidoEspelho"]);
                else if (!String.IsNullOrEmpty(hdfIdAmbientePedidoEspelho.Value))
                    idAmbientePedidoEsp = Glass.Conversoes.StrParaUint(hdfIdAmbientePedidoEspelho.Value);
    
                if (!String.IsNullOrEmpty(txtAmbiente.Text))
                    ItemProjetoDAO.Instance.AtualizaAmbiente(Glass.Conversoes.StrParaUint(hdfIdItemProjeto.Value), txtAmbiente.Text);
    
                ItemProjeto itemProjeto = ItemProjetoDAO.Instance.GetElement(idItemProjeto);
    
                if (idOrcamento > 0)
                    ProdutosOrcamentoDAO.Instance.InsereAtualizaProdProj(idOrcamento.Value, idAmbienteOrca, itemProjeto);

                if (idPedido > 0)
                {
                    var pedido = PedidoDAO.Instance.GetElementByPrimaryKey(idPedido.Value);
                    ProdutosPedidoDAO.Instance.InsereAtualizaProdProj(null, pedido, idAmbientePedido, itemProjeto, true);
                }

                if (idPedidoEsp > 0)
                {
                    ProdutosPedidoEspelhoDAO.Instance.InsereAtualizaProdProj(null, idPedidoEsp.Value, idAmbientePedidoEsp, itemProjeto, true);
                }
    
                // Marca que cálculo de projeto foi conferido
                if (idPedido > 0 || idPedidoEsp > 0)
                    ItemProjetoDAO.Instance.CalculoConferido(Glass.Conversoes.StrParaUint(hdfIdItemProjeto.Value));
    
                ClientScript.RegisterClientScriptBlock(typeof(string), "Ok", "window.opener.refreshPage(); alert('Medidas confirmadas'); closeWindow();", true);
            }
            catch (Exception ex)
            {
                // Marca que cálculo de projeto não foi conferido
                if (idPedido > 0 || idPedidoEsp > 0)
                    ItemProjetoDAO.Instance.CalculoNaoConferido(Glass.Conversoes.StrParaUint(hdfIdItemProjeto.Value));
    
                Glass.MensagemAlerta.ErrorMsg("Falha ao confirmar projeto.", ex, Page);
                return;
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
        }
    
        #endregion
    
        #region Calcula total do item projeto/projeto
    
        /// <summary>
        /// Calcula o total do item do projeto e insere o valor na lblSubtotal
        /// </summary>
        protected void CalculaTotalItemProj()
        {
            uint idItemProj = Glass.Conversoes.StrParaUint(hdfIdItemProjeto.Value);
            lblSubtotal.Text = ItemProjetoDAO.Instance.GetTotalItemProjeto(idItemProj).ToString("C");
            lblM2Vao.Text = ItemProjetoDAO.Instance.GetM2VaoItemProjeto(idItemProj).ToString(CultureInfo.InvariantCulture) + "m²";
        }
    
        #endregion
    
        #region Beneficiamentos
    
        protected void txtEspessura_DataBinding(object sender, EventArgs e)
        {
            TextBox txt = (TextBox)sender;
            GridViewRow linhaControle = txt.Parent.Parent as GridViewRow;

            if (linhaControle != null)
            {
                MaterialItemProjeto mip = linhaControle.DataItem as MaterialItemProjeto;
                if (mip != null) txt.Enabled = mip.Espessura <= 0;
            }
        }
    
        protected void ctrlBenef_Load(object sender, EventArgs e)
        {
            Glass.UI.Web.Controls.ctrlBenef benef = (Glass.UI.Web.Controls.ctrlBenef)sender;
            GridViewRow linhaControle = benef.Parent.Parent as GridViewRow;
    
            Control codProd = null;
            if (linhaControle != null && linhaControle.FindControl("hdfCodInterno") != null)
                codProd = linhaControle.FindControl("hdfCodInterno");
            else if (linhaControle != null) codProd = linhaControle.FindControl("txtCodProdIns");
            if (linhaControle != null)
            {
                HiddenField hdfAltura = (HiddenField)linhaControle.FindControl("hdfAlturaCalc");
                if (hdfAltura == null)
                    hdfAltura = (HiddenField)linhaControle.FindControl("hdfAlturaCalcIns");
                TextBox txtEspessura = (TextBox)linhaControle.FindControl("txtEspessura");
                TextBox txtLargura = (TextBox)linhaControle.FindControl("txtLarguraIns");
                Control quantidade = (TextBox)linhaControle.FindControl("txtQtdeIns");
                if (quantidade == null)
                    quantidade = (Label)linhaControle.FindControl("lblQtde");
                Label lblTotalM2 = Beneficiamentos.UsarM2CalcBeneficiamentos ?
                    (Label)linhaControle.FindControl("lblTotM2Calc") :
                    (Label)linhaControle.FindControl("lblTotM2Ins");
                TextBox txtValorIns = (TextBox)linhaControle.FindControl("txtValorIns");
                HiddenField hdfTipoEntregaBenef = hdfTipoEntrega;
                HiddenField hdfCliRevendaBenef = hdfCliRevenda;
                HiddenField hdfIdClienteBenef = hdfIdCliente;
                HiddenField hdfCustoProd = (HiddenField)linhaControle.FindControl("hdfCustoProd");
    
                benef.CampoAltura = hdfAltura;
                benef.CampoEspessura = txtEspessura;
                benef.CampoLargura = txtLargura;
                benef.CampoQuantidade = quantidade;
                benef.CampoTipoEntrega = hdfTipoEntrega;
                benef.CampoTotalM2 = lblTotalM2;
                benef.CampoValorUnitario = txtValorIns;
                benef.CampoCusto = hdfCustoProd;
            }
            benef.CampoProdutoID = codProd;
            benef.CampoRevenda = hdfCliRevenda;
            benef.CampoClienteID = hdfIdCliente;
            if (linhaControle != null)
            {
                benef.CampoAplicacaoID = linhaControle.FindControl("hdfIdAplicacao");
                benef.CampoProcessoID = linhaControle.FindControl("hdfIdProcesso");
                benef.CampoAplicacao = linhaControle.FindControl("txtAplIns");
                benef.CampoProcesso = linhaControle.FindControl("txtProcIns");
            }

            uint idPedido = !string.IsNullOrEmpty(Request["IdPedido"]) ? Conversoes.StrParaUint(Request["IdPedido"]) :
                !string.IsNullOrEmpty(Request["IdPedidoEspelho"]) ? Conversoes.StrParaUint(Request["IdPedidoEspelho"]) : 0;
    
            if (idPedido > 0)
            {
                benef.TipoBenef = PedidoDAO.Instance.GetTipoPedido(null, idPedido) == Glass.Data.Model.Pedido.TipoPedidoEnum.MaoDeObraEspecial ?
                    TipoBenef.MaoDeObraEspecial : TipoBenef.Venda;
            }
        }
    
        protected void ctrlBenef_PreRender(object sender, EventArgs e)
        {
            Glass.UI.Web.Controls.ctrlBenef benef = (Glass.UI.Web.Controls.ctrlBenef)sender;
            GridViewRow linhaControle = benef.Parent.Parent as GridViewRow;
    
            if (!benef.CampoQuantidade.Visible)
                if (linhaControle != null) benef.CampoQuantidade = (Label)linhaControle.FindControl("lblQtde");
        }
    
        #endregion
    
        protected void ctrlCorItemProjeto_CorAlterada(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(hdfIdItemProjeto.Value))
                MontaModelo(true);
            else
                LimpaCalculo();
    
            grdMaterialProjeto.DataBind();
            grdItemProjeto.DataBind();
            ClientScript.RegisterClientScriptBlock(typeof(string), "Ok", "window.opener.refreshPage();", true);
        }
    
        public string BuscarKitQueryString()
        {
            return "?idGrupoProd=" + (int)Glass.Data.Model.NomeGrupoProd.KitParaBox;
        }
    
        protected void ctrlCorItemProjeto2_Load(object sender, EventArgs e)
        {
            ctrlCorItemProjeto2.IdOrcamento = Glass.Conversoes.StrParaUintNullable(Request["idOrcamento"]);
            ctrlCorItemProjeto2.IdPedido = Glass.Conversoes.StrParaUintNullable(Request["IdPedido"]);
            ctrlCorItemProjeto2.IdPedidoEspelho = Glass.Conversoes.StrParaUintNullable(Request["IdPedidoEspelho"]);
        }
    
        protected void grdItemProjeto_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (!String.IsNullOrEmpty(Request["idPedidoEspelho"]))
            {
                HiddenField hdfConferido = (HiddenField)e.Row.Cells[0].FindControl("hdfConferido");
    
                if (hdfConferido == null)
                    return;
    
                bool conferido = hdfConferido.Value.ToLower() == "true";
    
                foreach (TableCell c in e.Row.Cells)
                    c.ForeColor = conferido ? System.Drawing.Color.Green : System.Drawing.Color.Red;
            }
        }
    
        protected void btnImprimir_PreRender(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(Request["idPedidoEspelho"]) && ((Button)sender).OnClientClick.IndexOf("&pcp=1") == -1)
                ((Button)sender).OnClientClick = ((Button)sender).OnClientClick.Replace("'); ", "&pcp=1'); ");
        }
    
        protected string NomeControleBenef()
        {
            return grdMaterialProjeto.EditIndex == -1 ? "ctrlBenefInserir" : "ctrlBenefEditar";
        }
    
        protected void grdItemProjeto_PageIndexChanged(object sender, EventArgs e)
        {
            if (!FuncoesGerais.IsChrome(Page) && grdItemProjeto.PageIndex > 0)
            {
                if (Session["pgIndProj"] == null)
                    Session.Add("pgIndProj", grdItemProjeto.PageIndex.ToString());
                else
                    Session["pgIndProj"] = grdItemProjeto.PageIndex.ToString();
            }
        }
    
        protected void grdItemProjeto_DataBound(object sender, EventArgs e)
        {
            if (!!FuncoesGerais.IsChrome(Page) && Session["pgIndProj"] != null && grdItemProjeto.PageCount > 1)
            {
                int pgIndex = Glass.Conversoes.StrParaInt(Session["pgIndProj"].ToString());
                if (pgIndex < grdItemProjeto.PageCount)
                    grdItemProjeto.PageIndex = pgIndex;
            }
        }
    
        protected void txtValorIns_Load(object sender, EventArgs e)
        {
            ((TextBox)sender).Enabled = PedidoConfig.DadosPedido.AlterarValorUnitarioProduto;
        }
    
        protected void dtvImagem_DataBound(object sender, EventArgs e)
        {
            // Atualiza o botão de imprimir, preenchendo com o idItemProjeto e alterando sua visibilidade
            if (dtvImagem.FindControl("btnImprimir") != null)
            {
                ((Button)dtvImagem.FindControl("btnImprimir")).OnClientClick = "openWindow(600, 800, '../../Relatorios/Projeto/RelBase.aspx?rel=imagemProjeto&idOrcamento=" + Request["idOrcamento"] +
                    "&imprAlumFerr=true&idPedido=" + Request["idPedido"] + "&idItemProjeto=" + hdfIdItemProjeto.Value + "'); return false;";
                ((Button)dtvImagem.FindControl("btnImprimir")).Visible = Request["pcp"] != null || 
                    Glass.Configuracoes.ProjetoConfig.TelaCadastroAvulso.ExibirBotaoImprimirProjeto;
            }
        }
    
        protected int CodigoTipoPedidoMaoObraEspecial()
        {
            return (int)Glass.Data.Model.Pedido.TipoPedidoEnum.MaoDeObraEspecial;
        }
    }
}
