using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Helper;
using Glass.Data.Model;
using System.Collections.Generic;
using Glass.Data.DAL;
using Glass.Configuracoes;
using System.Linq;

namespace Glass.UI.Web.Utils
{
    public partial class SystemConfig : System.Web.UI.Page
    {  
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Utils.SystemConfig));
    
            if (UserInfo.GetUserInfo.TipoUsuario != (uint)Data.Helper.Utils.TipoFuncionario.Administrador)
                Response.Redirect("~/WebGlass/Main.aspx");
            
            if (!IsPostBack)
            {
                drpLoja.DataBind();
                if (Request["idLoja"] != null)
                    drpLoja.SelectedIndex = drpLoja.Items.IndexOf(drpLoja.Items.FindByValue(Request["idLoja"]));
    
                if (!String.IsNullOrEmpty(Request["aba"]))
                    hdfAba.Value = Request["aba"];
    
                lnkConsultaEnvio.Text = "Consulta Envio de Email";

                if (PCPConfig.EmailSMS.EnviarSMSPedidoPronto || PCPConfig.EmailSMS.EnviarSMSAdministrador)
                    lnkConsultaEnvio.Text += " e SMS";
    
                CarregaPlanoConta();

                for (int i = 4; i <= 8; i++)
                    dtvComissao.Fields[i].Visible = !Glass.Configuracoes.PedidoConfig.Comissao.PerComissaoPedido && !PedidoConfig.Comissao.UsarComissaoPorTipoPedido;

                //Mostra ou n�o de acordo com a config os campos de comiss�o por tipo de pedido
                dtvComissao.Fields[1].Visible = PedidoConfig.Comissao.UsarComissaoPorTipoPedido;
                dtvComissao.Fields[2].Visible = !PedidoConfig.Comissao.UsarComissaoPorTipoPedido;
                dtvComissao.Fields[3].Visible = !PedidoConfig.Comissao.UsarComissaoPorTipoPedido;

                if (Geral.SistemaLite || !Geral.ControlePCP)
                    aba_pcp.Style.Add("Display", "none");

                if (!UserInfo.GetUserInfo.IsAdminSync)
                    aba_internas.Style.Add("Display", "none");
            }

            int idLoja;
            if (int.TryParse(drpLoja.SelectedValue, out idLoja))
            {
                ctrlFaixasRentabilidadeComissao.IdLoja = idLoja;
                ctrlFaixasRentabilidadeLiberacao.IdLoja = idLoja;
            }

            // A op��o de gerar backup deve ficar vis�vel apenas para admin sync
            lnkRelatorioDinamico.Visible = UserInfo.GetUserInfo.IsAdminSync;            
        }
    
        protected void lnkUsuariosLogados_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Utils/UsuariosLogados.aspx");
        }
    
        protected void lnkControleBenef_Click(object sender, EventArgs e)
        {
            Response.Redirect("../Listas/LstBenef.aspx");
        }
    
        protected void drpLoja_SelectedIndexChanged(object sender, EventArgs e)
        {
            Response.Redirect("~/Utils/Config.aspx?idLoja=" + drpLoja.SelectedValue + "&aba=" + hdfAba.Value);
        }
    
        protected void lnkConsultaEnvio_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Utils/ConsultaEnvio.aspx");
        }

        protected void lnkRelatorioDinamico_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Relatorios/Dinamicos/LstRelatorioDinamico.aspx");
        }

        #region Verifica se itens existem

        [Ajax.AjaxMethod]
        public string AplicacaoExiste(string codInterno)
        {
            return (EtiquetaAplicacaoDAO.Instance.GetByCodInterno(codInterno) != null).ToString().ToLower();
        }
    
        [Ajax.AjaxMethod]
        public string ProcessoExiste(string codInterno)
        {
            return (EtiquetaProcessoDAO.Instance.GetByCodInterno(codInterno) != null).ToString().ToLower();
        }
    
        #endregion   
    
        #region Comiss�o
    
        protected void dtvDescontoComissao_Load(object sender, EventArgs e)
        {
            var comissaoPorPedido = Glass.Configuracoes.PedidoConfig.Comissao.PerComissaoPedido;
    
            // Se o tipo de comiss�o for por pedido deve ser mostrado somente o percentual de comiss�o da 1� faixa, pois,
            // caso o cliente n�o possua um percentual cadastrado o percentual da 1� faixa do funcion�rio ser� considerado
            // ao finalizar o pedido.
            if (comissaoPorPedido)
            {
                dtvComissao.Rows[4].Visible = false;
                dtvComissao.Rows[5].Visible = false;
                dtvComissao.Rows[6].Visible = false;
                dtvComissao.Rows[7].Visible = false;
            }
        }
    
        protected bool ExibirFaixaValor()
        {
            var exibirFaixaValor = !Glass.Configuracoes.PedidoConfig.Comissao.PerComissaoPedido;
    
            return exibirFaixaValor;
        }
    
        protected void descontoComissao_Load(object sender, EventArgs e)
        {
            descontoComissao.Visible = Glass.Configuracoes.ComissaoConfig.DescontarComissaoPerc;
        }
    
        protected void drpFunc_SelectedIndexChanged(object sender, EventArgs e)
        {
            dtvComissao.DataBind();
            dtvDescontoComissao.DataBind();
            ((HiddenField)dtvComissao.FindControl("hdfIdFunc")).Value = drpFunc.SelectedValue == "0" ? String.Empty : drpFunc.SelectedValue;
            ((HiddenField)dtvDescontoComissao.FindControl("hdfIdFunc")).Value = drpFunc.SelectedValue == "0" ? String.Empty : drpFunc.SelectedValue;
        }
    
        protected void odsComissao_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao configurar comiss�o.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
            else
            {
                PedidoDAO.Instance.CriaComissaoFuncionario(Glass.Data.Model.Pedido.TipoComissao.Funcionario, 0, null, null, "");
    
                Glass.MensagemAlerta.ShowMsg("Comiss�o configurada!", Page);
            }
        }
    
        protected void odsDescontoComissao_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao configurar desconto de comiss�o.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
            else
                Glass.MensagemAlerta.ShowMsg("Desconto da comiss�o configurado!", Page);
        }
    
        protected bool UsarDescontoComissao()
        {
            return Glass.Configuracoes.ComissaoConfig.DescontarComissaoPerc && Glass.Configuracoes.PedidoConfig.Comissao.PerComissaoPedido;
        }

        #endregion
    
        #region Pre�o do produto
    
        protected void ddlGrupoProd_SelectedIndexChanged(object sender, EventArgs e)
        {
            dtvLogProduto.DataBind();
            grdViewLogProduto.DataBind();
        }
    
        protected void ddlSubgrupoProd_SelectedIndexChanged(object sender, EventArgs e)
        {
            dtvLogProduto.DataBind();
            grdViewLogProduto.DataBind();
        }
    
        protected void hdfGrupoProd_DataBinding(object sender, EventArgs e)
        {
            ((HiddenField)sender).Value = ddlGrupoProd.SelectedValue;
        }
    
        protected void hdfSubgrupoProd_DataBinding(object sender, EventArgs e)
        {
            ((HiddenField)sender).Value = ddlSubgrupoProd.SelectedValue;
        }
    
        protected void GrupoSubgrupo_DataBinding(object sender, EventArgs e)
        {
            dtvLogProduto.DataBind();
            grdViewLogProduto.DataBind();
        }
    
        protected void odsLogProduto_Inserted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                e.ExceptionHandled = true;
                Glass.MensagemAlerta.ErrorMsg("Falha ao reajustar pre�o dos produtos.", e.Exception, Page);
            }
    
            dtvLogProduto.DataBind();
            grdViewLogProduto.DataBind();
        }
    
        protected void odsLogProduto_Inserting(object sender, Colosoft.WebControls.VirtualObjectDataSourceMethodEventArgs e)
        {
            LogProduto log = (LogProduto)e.InputParameters[0];
            log.DataAjuste = DateTime.Now;
    
            if (log.IdGrupoProd == 0) log.IdGrupoProd = null;
            if (log.IdSubgrupoProd == 0) log.IdSubgrupoProd = null;
        }
    
        protected void hdfIdFunc_DataBinding(object sender, EventArgs e)
        {
            ((HiddenField)sender).Value = UserInfo.GetUserInfo.CodUser.ToString();
        }
    
        protected void drpPrecoBase_SelectedIndexChanged(object sender, EventArgs e)
        {
            dtvLogProduto.DataBind();
        }
    
        protected void hdfTipoPrecoBase_DataBinding(object sender, EventArgs e)
        {
            ((HiddenField)sender).Value = drpPrecoBase.SelectedValue;
        }
    
        #endregion
    
        #region Pre�o do beneficiamento
    
        protected void drpTipoBenef_SelectedIndexChanged(object sender, EventArgs e)
        {
            dtvLogBenef.DataBind();
            grdViewLogBenef.DataBind();
        }
    
        protected void hdfBenef_DataBinding(object sender, EventArgs e)
        {
            ((HiddenField)sender).Value = "0";
        }
    
        protected void hdfTipoBenef_DataBinding(object sender, EventArgs e)
        {
            ((HiddenField)sender).Value = drpTipoBenef.SelectedValue;
        }
    
        protected void odsLogBenef_Inserted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                e.ExceptionHandled = true;
                Glass.MensagemAlerta.ErrorMsg("Falha ao reajustar pre�o dos beneficiamentos.", e.Exception, Page);
            }
    
            dtvLogBenef.DataBind();
            grdViewLogBenef.DataBind();
        }
    
        protected void odsLogBenef_Inserting(object sender, Colosoft.WebControls.VirtualObjectDataSourceMethodEventArgs e)
        {
            //LogProduto log = (LogProduto)e.InputParameters[0];
            //log.DataAjuste = DateTime.Now;
    
            //log.IdGrupoProd = 0;
            //if (log.IdSubgrupoProd == 0) log.IdSubgrupoProd = null;
            //log.TipoPrecoBase = 1;
        }
    
        protected void hdfIdFuncBenef_DataBinding(object sender, EventArgs e)
        {
            ((HiddenField)sender).Value = UserInfo.GetUserInfo.CodUser.ToString();
        }
    
        #endregion
    
        #region Retorna o ID da loja
    
        private uint GetIdLoja()
        {
            if (drpLoja.SelectedValue == "" || drpLoja.Items.Count == 0)
                drpLoja.DataBind();
    
            return Glass.Conversoes.StrParaUint(drpLoja.SelectedValue);
        }
    
        #endregion
    
        #region M�todos para as tabelas de configura��o
    
        private TableRow[] LinhasItens(IEnumerable<Configuracao> itens, string nomeAba, uint? idPai)
        {
            List<TableRow> retorno = new List<TableRow>();
    
            int numeroItem = 0;
            Panel grupoAtual = null;
    
            foreach (Configuracao item in itens)
            {
                if ((grupoAtual != null && (item.Grupo != grupoAtual.GroupingText)) || 
                    (grupoAtual == null && !String.IsNullOrEmpty(item.Grupo)))
                {
                    if (grupoAtual != null)
                    {
                        TableRow linhaGrupo = new TableRow();
                        TableCell celulaGrupo = new TableCell();
                        linhaGrupo.Cells.Add(celulaGrupo);
    
                        celulaGrupo.ColumnSpan = 3;
                        celulaGrupo.HorizontalAlign = HorizontalAlign.Center;
                        celulaGrupo.Controls.Add(grupoAtual);
                        retorno.Add(linhaGrupo);
                    }
    
                    if (!String.IsNullOrEmpty(item.Grupo))
                    {
                        grupoAtual = new Panel();
                        grupoAtual.ID = item.Grupo.Replace(" ", "_") + "_" + item.IdConfig;
                        grupoAtual.Controls.Add(new Table());
                        ((Table)grupoAtual.Controls[0]).Width = new Unit("100%");
                        grupoAtual.GroupingText = item.Grupo;
                    }
                    else
                        grupoAtual = null;
                }
    
                ConfiguracaoLoja configLoja = ConfiguracaoLojaDAO.Instance.GetItem((Glass.Data.Helper.Config.ConfigEnum)item.IdConfig, GetIdLoja());
                TableRow linha = new TableRow();
    
                TableCell texto = new TableCell();
                texto.Style.Add("vertical-align", "middle");
                texto.Style.Add("text-align", "left");
                texto.Style.Add("padding-right", "6px");
                texto.Width = new Unit("100%");
                texto.Font.Bold = item.ExibirApenasAdminSync;
    
                Label descr = new Label();
                descr.Text = item.Descricao;
                texto.Controls.Add(descr);
                
                if (item.UsarLoja)
                {
                    Label loja = new Label();
                    loja.Font.Italic = true;
                    loja.Text = "<br />(configura��o por loja)";
                    texto.Controls.Add(loja);
                }

                if (!string.IsNullOrEmpty(item.Observacao))
                {
                    Image imgObs = new Image();
                    imgObs.ImageUrl = "~/Images/help.gif";
                    imgObs.ToolTip = item.Observacao;
                    texto.Controls.Add(imgObs);
                }
    
                TableCell valor = new TableCell();
                texto.Style.Add("vertical-align", "middle");
                valor.Style.Add("text-align", "left");
    
                string tipo = "input";
                WebControl controle = null;
                
                switch ((Glass.Data.Helper.Config.TipoConfigEnum)item.Tipo)
                {
                    case Glass.Data.Helper.Config.TipoConfigEnum.Decimal:
                        TextBox txtDecimal = new TextBox();
                        txtDecimal.ID = "txtDecimal_" + item.IdConfig;
                        txtDecimal.Attributes.Add("onkeypress", string.Format("return soNumeros(event, false, {0})", item.PermitirNegativo ? "false" : "true"));
                        txtDecimal.Text = configLoja != null ? configLoja.ValorDecimal.GetValueOrDefault().ToString().Replace(".", ",") : null;
                        txtDecimal.EnableViewState = false;
                        controle = txtDecimal;
                        break;
    
                    case Glass.Data.Helper.Config.TipoConfigEnum.Inteiro:
                        TextBox txtInteiro = new TextBox();
                        txtInteiro.ID = "txtInteiro_" + item.IdConfig;
                        txtInteiro.Attributes.Add("onkeypress", string.Format("return soNumeros(event, true, {0})", item.PermitirNegativo ? "false" : "true"));
                        txtInteiro.Text = configLoja != null ? configLoja.ValorInteiro.GetValueOrDefault().ToString() : null;
                        txtInteiro.EnableViewState = false;
                        controle = txtInteiro;
                        break;
    
                    case Glass.Data.Helper.Config.TipoConfigEnum.Logico:
                        CheckBox chkLogico = new CheckBox();
                        chkLogico.ID = "chkLogico_" + item.IdConfig;
                        chkLogico.Checked = configLoja != null ? configLoja.ValorBooleano : false;
                        chkLogico.EnableViewState = false;
                        controle = chkLogico;
                        break;
    
                    case Glass.Data.Helper.Config.TipoConfigEnum.Texto:
                    case Glass.Data.Helper.Config.TipoConfigEnum.TextoCurto:
                        TextBox txtTexto = new TextBox();
                        txtTexto.ID = "txtTexto_" + item.IdConfig;
                        txtTexto.Text = configLoja != null ? configLoja.ValorTexto : null;
                        txtTexto.MaxLength = 1000;
    
                        switch ((Glass.Data.Helper.Config.TipoConfigEnum)item.Tipo)
                        {
                            case Glass.Data.Helper.Config.TipoConfigEnum.Texto:
                                txtTexto.TextMode = TextBoxMode.MultiLine;
                                txtTexto.Rows = 3;
                                txtTexto.Width = new Unit("300px");
                                break;
    
                            case Glass.Data.Helper.Config.TipoConfigEnum.TextoCurto:
                                txtTexto.TextMode = TextBoxMode.SingleLine;
                                txtTexto.Width = new Unit("150px");
                                break;
                        }
    
                        txtTexto.EnableViewState = false;
                        controle = txtTexto;
                        break;
    
                    case Glass.Data.Helper.Config.TipoConfigEnum.Enum:
                    case Glass.Data.Helper.Config.TipoConfigEnum.ListaMetodo:
                        tipo = "div";
    
                        object[] parametros = item.IdConfig == (int)Glass.Data.Helper.Config.ConfigEnum.CodigoAjusteAproveitamentoCreditoIcms ?
                            new object[] { new ControlParameter("idLoja", TypeCode.UInt32, "drpLoja", "SelectedValue") } : null;
    
                        var odsDataSource = ConfigDAO.Instance.GetForConfig(item.IdConfig, parametros);
                        odsDataSource.ID = "odsDataSource_" + item.IdConfig;
    
                        var selPopup = LoadControl("~/Controls/ctrlSelPopup.ascx") as Controls.ctrlSelPopup;
                        selPopup.ID = "selPopup_" + item.IdConfig;
                        selPopup.DataSourceID = odsDataSource.ID;
                        selPopup.DataTextField = "Descr";
                        selPopup.DataValueField = "Id";
                        selPopup.TextWidth = new Unit("200px");
                        selPopup.Valor = configLoja != null ? (configLoja.ValorInteiro != null ?
                            configLoja.ValorInteiro.GetValueOrDefault().ToString() : null) : null;
                        selPopup.TituloTela = "Selecione: " + item.Descricao;
                        selPopup.TitulosColunas = "C�d.|Valor";
                        selPopup.PermitirVazio = true;
                        selPopup.EnableViewState = false;
                        selPopup.ValidationGroup = nomeAba.Replace("aba_", "");
                        
                        Panel panEnum = new Panel();
                        panEnum.ID = "panPopup_" + item.IdConfig;
                        panEnum.Controls.Add(odsDataSource);
                        panEnum.Controls.Add(selPopup);
                        panEnum.Style.Value = "margin-left: 2px; padding-right: 2px";
                        controle = panEnum;
                        break;
    
                    case Glass.Data.Helper.Config.TipoConfigEnum.GrupoEnumMetodo:
                        tipo = "fieldset";
                        HiddenField hdfGrupoEnumMetodo = new HiddenField();
                        hdfGrupoEnumMetodo.ID = "hdfGrupoEnumMetodo_" + item.IdConfig;
                        hdfGrupoEnumMetodo.Value = configLoja != null ? configLoja.ValorTexto : null;
                        Table tblGrupoEnumMetodo = new Table();
                        tblGrupoEnumMetodo.ID = "tblGrupoEnumMetodo_" + item.IdConfig;
                        tblGrupoEnumMetodo.Rows.AddRange(LinhasItens(ConfiguracaoDAO.Instance.GetFilhos(item), nomeAba, item.IdConfig));
                        tblGrupoEnumMetodo.Width = new Unit("100%");
                        Panel panGrupoEnumMetodo = new Panel();
                        panGrupoEnumMetodo.GroupingText = item.Descricao + (item.UsarLoja ? " (configura��o por loja)" : String.Empty);
                        panGrupoEnumMetodo.Controls.Add(hdfGrupoEnumMetodo);
                        panGrupoEnumMetodo.Controls.Add(tblGrupoEnumMetodo);
                        panGrupoEnumMetodo.Style.Value = "display: block";
                        controle = panGrupoEnumMetodo;
    
                        string[] valores = configLoja != null && !String.IsNullOrEmpty(configLoja.ValorTexto) ? configLoja.ValorTexto.Split(';') : new string[0];
                        for (int i = 0; i < tblGrupoEnumMetodo.Rows.Count; i++)
                        {
                            Control c = tblGrupoEnumMetodo.Rows[i].Cells[1].Controls[0];
                            if (c is TextBox)
                                ((TextBox)c).Text = valores.Length > i ? valores[i] : "";
                            else if (c is CheckBox)
                                ((CheckBox)c).Checked = valores.Length > 1 ? bool.Parse(valores[i]) : false;
                        }
                        break;
    
                    case Glass.Data.Helper.Config.TipoConfigEnum.Data:
                        Controls.ctrlData datData = LoadControl("~/Controls/ctrlData.ascx") as Controls.ctrlData;
                        datData.ID = "datData_" + item.IdConfig;
                        datData.DataString = configLoja.ValorTexto;
                        datData.EnableViewState = false;
                        datData.ExibirHoras = false;
                        datData.ValidateEmptyText = false;
                        datData.ValidationGroup = nomeAba.Replace("aba_", "");
                        controle = datData.FindControl("txtData") as WebControl;
                        break;
                }
    
                if (idPai > 0)
                {
                    controle.Attributes.Add("OnChange", "FindControl('hdfGrupoEnumMetodo_" + idPai + "', 'input').value = retornaValor('" + controle.ID + "', '" + tipo + "')");
                    controle.ID += "_" + numeroItem;
                }
    
                valor.Controls.Add(controle);
    
                if (item.Tipo != (int)Glass.Data.Helper.Config.TipoConfigEnum.GrupoEnumMetodo)
                    linha.Cells.Add(texto);
                else
                    valor.ColumnSpan = 2;
                
                linha.Cells.Add(valor);
    
                if (idPai.GetValueOrDefault() == 0 && configLoja != null)
                {
                    Controls.ctrlLogPopup popup = LoadControl("../Controls/ctrlLogPopup.ascx") as Controls.ctrlLogPopup;
                    popup.IdRegistro = configLoja.IdConfigLoja;
                    popup.Tabela = LogAlteracao.TabelaAlteracao.ConfigLoja;
    
                    TableCell log = new TableCell();
                    log.Style.Add("text-align", "right");
                    log.Controls.Add(popup);
    
                    linha.Cells.Add(log);
                }
    
                if (item.Grupo == null)
                    retorno.Add(linha);
                else if (grupoAtual != null)
                    ((Table)grupoAtual.Controls[0]).Rows.Add(linha);
    
                numeroItem++;
            }
    
            if (grupoAtual != null)
            {
                TableRow linhaGrupo = new TableRow();
                TableCell celulaGrupo = new TableCell();
                linhaGrupo.Cells.Add(celulaGrupo);
    
                celulaGrupo.ColumnSpan = 3;
                celulaGrupo.HorizontalAlign = HorizontalAlign.Center;
                celulaGrupo.Controls.Add(grupoAtual);
                retorno.Add(linhaGrupo);
            }
    
            return retorno.ToArray();
        }  
        
        private void Config_EndLoad(Table tabela, TableRow[] linhas, string nomeAba)
        {
            if (linhas.Length == 0)
                Page.ClientScript.RegisterStartupScript(GetType(), "esconderAba_" + nomeAba, "document.getElementById('" + nomeAba + "').style.display = 'none';\n", true);
            else
            {
                tabela.Rows.Clear();
                tabela.Rows.AddRange(linhas);
            }
        }
    
        [Ajax.AjaxMethod]
        public string Salvar(string dados, string idLojaStr)
        {
            try
            {
                var idLoja = idLojaStr.StrParaUint();
                var itens = dados.Split('~');

                // Lista de idConfig alterados
                var lstIdConfigAlt = new List<int>();

                var mensagemRetorno = string.Empty;

                foreach (var item in itens)
                {
                    if (item.Contains("undefined"))
                        continue;

                    var dadosItem = item.Split('^');
                    var id = Glass.Conversoes.StrParaInt(dadosItem[0].Substring(dadosItem[0].LastIndexOf("_") + 1));
                    object valor = dadosItem[1];
                    
                    Glass.Data.Helper.Config.ConfigEnum idConfig = (Glass.Data.Helper.Config.ConfigEnum)id;

                    var valorAlterado = false;
                    var config = ConfiguracaoDAO.Instance.GetItem(idConfig);
                    var configLoja = ConfiguracaoLojaDAO.Instance.GetItem(idConfig, idLoja);

                    if (configLoja == null)
                    {
                        configLoja = new ConfiguracaoLoja();
                        configLoja.IdConfig = config.IdConfig;
                    }

                    if (config.UsarLoja && configLoja.IdLoja == null)
                        configLoja.IdLoja = idLoja;

                    switch (ConfigDAO.Instance.GetTipo(idConfig))
                    {
                        case Glass.Data.Helper.Config.TipoConfigEnum.Decimal:
                            valor = valor != null && valor.ToString() != "" ? Glass.Conversoes.StrParaFloatNullable(valor.ToString()) : 0;
                            valorAlterado = (configLoja.ValorDecimal == null ? 0 : configLoja.ValorDecimal.Value) != valor.ToString().StrParaDecimal();
                            break;
    
                        case Glass.Data.Helper.Config.TipoConfigEnum.Inteiro:
                        case Glass.Data.Helper.Config.TipoConfigEnum.Enum:
                        case Glass.Data.Helper.Config.TipoConfigEnum.ListaMetodo:
                            valor = valor != null && valor.ToString() != "" ? Glass.Conversoes.StrParaIntNullable(valor.ToString()) : null;
                            valorAlterado = configLoja.ValorInteiro != (int?)valor;
                            break;
    
                        case Glass.Data.Helper.Config.TipoConfigEnum.Logico:
                            valor = bool.Parse(valor.ToString());
                            valorAlterado = configLoja.ValorBooleano != (bool)valor;
                            break;
                    }

                    #region Valida algumas informa��es antes de salvar

                    if (valorAlterado)
                    {
                        // N�o permite desmarcar a op��o de controlar produ��o se algumas outras relacionadas a ela estejam marcadas
                        if (idConfig == Config.ConfigEnum.ControlePCP && !(bool)valor)
                        {
                            if (OrdemCargaConfig.UsarControleOrdemCarga)
                                return "Erro|Para desmarcar esta op��o � necess�rio desmarcar o controle de ordem de carga antes.";

                            if (PCPConfig.Etiqueta.UsarControleRetalhos)
                                return "Erro|Para desmarcar esta op��o � necess�rio desmarcar o controle de retalhos antes.";
                        }

                        if (idConfig == Config.ConfigEnum.RatearDescontoProdutos)
                        {
                            var idPedido = PedidoDAO.Instance.ObterUltimoIdPedidoInserido();

                            if (idPedido > 0)
                                return "Erro|Para habilitar ou desabilitar esta op��o, n�o pode existir pedido no sistema.";
                        }

                        //N�o permite que a op��o usar controle de ordem de carga seja marcada se a op��o de separar pedidos venda de revenda 
                        // estiver desmarcada e vice-versa
                        if (idConfig == Config.ConfigEnum.UsarControleOrdemCarga && (bool)valor && !PedidoConfig.DadosPedido.BloquearItensTipoPedido && !Geral.NaoVendeVidro())
                            return "Erro|N�o � poss�vel utilizar a op��o de controle de ordem de carga se a op��o de bloquear itens de revenda em pedidos de venda n�o estiver marcada.";

                        if (idConfig == Config.ConfigEnum.BloquearItensTipoPedido && !(bool)valor && OrdemCargaConfig.UsarControleOrdemCarga && !Geral.NaoVendeVidro())
                            return "Erro|N�o � poss�vel desmarcar a op��o de bloquear itens de revenda em pedidos de venda se a op��o de controle de ordem de carga estiver marcada.";

                        if (idConfig == Config.ConfigEnum.UsarControleDescontoFormaPagamentoDadosProduto && (bool)valor)
                        {
                            if (FinanceiroConfig.UsarDescontoEmParcela)
                                return "Erro|N�o � poss�vel utilizar o controle, de desconto por forma de pagamento e dados do produto, caso a configura��o Usar controle de desconto em parcela esteja habilitada.";

                            if (!Liberacao.BloquearLiberacaoParcelasDiferentes)
                                return "Erro|N�o � poss�vel utilizar o controle, de desconto por forma de pagamento e dados do produto, caso a configura��o Bloquear libera��o de pedidos caso tenham parcelas diferentes esteja desabilitada.";
                        }

                        if (idConfig == Config.ConfigEnum.PermitirLiberacaoPedidosLojasDiferentes && (bool)valor)
                        {
                            var existeMensagemErro = false;
                            var mensagemErro = "Erro|Para marcar a op��o 'Permitir Liberacao de pedidos de lojas diferentes' � necess�rio desmarcar a(s) op��o(�es):\n";

                            if (PedidoConfig.Impostos.CalcularIcmsPedido)
                            {
                                mensagemErro += "Calcular ICMS no Pedido\n";
                                existeMensagemErro = true;
                            }
                            if (PedidoConfig.Impostos.CalcularIpiPedido)
                            {
                                mensagemErro += "Calcular IPI no Pedido\n";
                                existeMensagemErro = true;
                            }                            
                            if (Liberacao.Impostos.CalcularIcmsLiberacao)
                            {
                                mensagemErro += "Calcular ICMS na Libera��o\n";
                                existeMensagemErro = true;
                            }
                            if (Liberacao.Impostos.CalcularIpiLiberacao)
                            {
                                mensagemErro += "Calcular IPI na Libera��o\n";
                                existeMensagemErro = true;
                            }

                            if (existeMensagemErro)
                                return mensagemErro;

                            mensagemRetorno = "\nDevido a altera��o da configura��o (Permitir Liberacao de Pedidos de Lojas Diferentes), todas as lojas ter�o a op��o (Ignorar configura��o 'Liberar apenas produtos prontos') desmarcada.";
                        }

                        if (idConfig == Config.ConfigEnum.LiberarProdutosProntos && (bool)valor)
                            mensagemRetorno = "\nPara que a configura��o (Liberar apenas produtos prontos) funcione corretamente,\nverifique no cadastro dessa loja a op��o (Ignorar configura��o 'Liberar apenas produtos prontos').";

                        // N�o permite que a op��o "permitir gerar conferencia de pedido de revenda" seja marcada se a op��o de separar pedidos venda de revenda 
                        // estiver desmarcada e vice-versa
                        if (PedidoConfig.DadosPedido.BloquearItensTipoPedido && idConfig == Config.ConfigEnum.PermitirGerarConferenciaPedidoRevenda && (bool)valor)
                            return "Erro|N�o � poss�vel utilizar a op��o de gerar confer�ncia de pedido de revenda se a op��o de bloquear itens de revenda em pedidos de venda estiver marcada.";

                        // N�o permite que a op��o "permitir gerar conferencia de pedido de revenda" seja marcada se a op��o de separar pedidos venda de revenda 
                        // estiver desmarcada e vice-versa
                        if (PCPConfig.PermitirGerarConferenciaPedidoRevenda && idConfig == Config.ConfigEnum.BloquearItensTipoPedido && (bool)valor)
                            return "Erro|N�o � poss�vel utilizar a op��o de bloquear itens de revenda em pedidos de venda se a op��o de gerar confer�ncia de pedido de revenda estiver marcada.";

                        // N�o permite habilitar controle de instala��o, sem antes marcar a op��o de impedir liberar pedido sem passar pelo PCP
                        if (idConfig == Config.ConfigEnum.ControleInstalacao && (bool)valor && !PCPConfig.ImpedirLiberacaoPedidoSemPCP)
                            return "Erro|N�o � poss�vel utilizar a op��o de controle de instala��o se a op��o de impedir libera��o do pedido sem passar no PCP estiver desmarcada.";

                        // N�o permite desabilitar a op��o impedir liberar pedido sem PCP, sem antes desmarcar a op��o de Contole de instala��o
                        if (idConfig == Config.ConfigEnum.ImpedirLiberacaoPedidoSemPCP && !(bool)valor && Geral.ControleInstalacao)
                            return "Erro|Para desabilitar essa op��o primeiro � necess�rio que desabilite a op��o de Controle de Instala��o";

                        // N�o permite marcar a op��o de Calcular ICMS Pedido se algumas outras relacionadas a ela estejam marcadas
                        if (idConfig == Config.ConfigEnum.CalcularIcmsPedido && (bool)valor)
                        {
                            if (FinanceiroConfig.DadosLiberacao.PermitirLiberacaoPedidosLojasDiferentes)
                                return "Erro|Para marcar a op��o 'Calcular ICMS no pedido' � necess�rio desmarcar a op��o 'Permitir Liberacao de Pedidos de Lojas Diferentes'.";

                            mensagemRetorno = string.Format("\nDevido a altera��o da configura��o (Calcular ICMS no pedido), todas as lojas ter�o a op��o (Calcular ICMS no pedido) {0}.", (bool)valorAlterado ? "marcada" : "desmarcada");                            
                        }

                        // N�o permite marcar a op��o de Calcular IPI Pedido se algumas outras relacionadas a ela estejam marcadas
                        if ((idConfig == Config.ConfigEnum.CalcularIpiPedido) && (bool)valor)
                        {
                            if (FinanceiroConfig.DadosLiberacao.PermitirLiberacaoPedidosLojasDiferentes)
                                return "Erro|Para marcar a op��o 'Calcular IPI no pedido' � necess�rio desmarcar a op��o 'Permitir Liberacao de Pedidos de Lojas Diferentes'.";

                            mensagemRetorno = string.Format("\nDevido a altera��o da configura��o (Calcular IPI no pedido), todas as lojas ter�o a op��o (Calcular IPI no pedido) {0}.", (bool)valorAlterado ? "marcada" : "desmarcada");
                        }

                        // N�o permite marcar a op��o de Calcular ICMS Liberacao se algumas outras relacionadas a ela estejam marcadas
                        if (idConfig == Config.ConfigEnum.CalcularIcmsLiberacao && (bool)valor)
                        {
                            if (FinanceiroConfig.DadosLiberacao.PermitirLiberacaoPedidosLojasDiferentes)
                                return "Erro|Para marcar a op��o 'Calcular ICMS na Libera��o' � necess�rio desmarcar a op��o 'Permitir Liberacao de Pedidos de Lojas Diferentes'.";

                            mensagemRetorno = string.Format("\nDevido a altera��o da configura��o (Calcular ICMS na Libera��o), todas as lojas ter�o a op��o (Calcular ICMS na Libera��o) {0}.", (bool)valorAlterado ? "marcada" : "desmarcada");
                        }

                        // N�o permite marcar a op��o de Calcular IPI Liberacao se algumas outras relacionadas a ela estejam marcadas
                        if ((idConfig == Config.ConfigEnum.CalcularIpiLiberacao) && (bool)valor)
                        {
                            if (FinanceiroConfig.DadosLiberacao.PermitirLiberacaoPedidosLojasDiferentes)
                                return "Erro|Para marcar a op��o 'Calcular IPI na Libera��o' � necess�rio desmarcar a op��o 'Permitir Liberacao de Pedidos de Lojas Diferentes'.";

                            mensagemRetorno = string.Format("\nDevido a altera��o da configura��o (Calcular IPI na Libera��o), todas as lojas ter�o a op��o (Calcular IPI na Libera��o) {0}.", (bool)valorAlterado ? "marcada" : "desmarcada");
                        }

                        if(idConfig == Config.ConfigEnum.ExibirOpcaoDeveTransferir && (bool)valor && OrdemCargaConfig.UsarOrdemCargaParcial)
                                return "Erro|N�o � possivel habilitar a op��o de 'Exibir op��o \"deve transferir\" no cadastro do pedido' caso a config Utilizar Ordem de Carga Parcial esteja habilitada";

                        if (idConfig == Config.ConfigEnum.UsarComissaoPorTipoPedido && (bool)valor && !PedidoConfig.Comissao.PerComissaoPedido)
                            return "Erro|N�o � possivel habilitar a op��o de 'Usar comiss�o por tipo de pedido' caso a config 'Percentual de Comiss�o por Pedido' esteja desabilitada";

                        if (idConfig == Config.ConfigEnum.PerComissaoPedido && !(bool)valor && PedidoConfig.Comissao.UsarComissaoPorTipoPedido)
                            return "Erro|N�o � possivel desabilitar a op��o de 'Percentual de Comiss�o por Pedido' caso a config 'Usar comiss�o por tipo de pedido' esteja habilitada";
                    }

                    #endregion

                    if (ConfigDAO.Instance.SetValue(idConfig, idLoja, valor))
                        lstIdConfigAlt.Add((int)idConfig);
                }
                
                // Se alguma configura��o associada ao controle de menus tiver sido alterada, limpa todas as informa��es que estiverem salvas em mem�ria
                if (lstIdConfigAlt.Count > 0)
                    Microsoft.Practices.ServiceLocation.ServiceLocator
                        .Current.GetInstance<Glass.Global.Negocios.IMenuFluxo>().RemoveMenuMemoria(lstIdConfigAlt.ToArray());

                if (!string.IsNullOrEmpty(mensagemRetorno))
                    return "Ok|Configura��o salva com sucesso!" + mensagemRetorno;

                return "Ok|Configura��o salva com sucesso!";
            }
            catch (Exception ex)
            {
                return "Erro|" + ex.Message;
            }
        }

        #endregion

        #region Comiss�o

        protected void tblComissao_Load(object sender, EventArgs e)
        {
            TableRow[] linhas = LinhasItens(ConfigDAO.Instance.GetItensComissao(), "aba_comissao", null);
            Config_EndLoad(tblComissao, linhas, "aba_comissao");
        }

        #endregion

        #region Rentabilidade

        protected void tblRentabilidade_Load(object sender, EventArgs e)
        {
            TableRow[] linhas = LinhasItens(ConfigDAO.Instance.GetItensRentabilidade(), "aba_rentabilidade", null);
            Config_EndLoad(tblRentabilidade, linhas, "aba_rentabilidade");
        }

        protected void drpFuncFaixaRentabilidadeComissao_SelectedIndexChanged(object sender, EventArgs e)
        {
            int idFunc;
            if (int.TryParse(drpFunc.SelectedValue == "0" ? string.Empty : drpFunc.SelectedValue, out idFunc))
                ctrlFaixasRentabilidadeComissao.IdFunc = idFunc;
            else
                ctrlFaixasRentabilidadeComissao.IdFunc = null;
        }

        #endregion

        #region Financeiro

        protected void tblFinanceiro_Load(object sender, EventArgs e)
        {
            TableRow[] linhas = LinhasItens(ConfigDAO.Instance.GetItensFinanceiro(), "aba_financeiro", null);
            Config_EndLoad(tblFinanceiro, linhas, "aba_financeiro");
        }
    
        #endregion
    
        #region Plano Conta
    
        #region Carrega aba Plano Conta
    
        protected void CarregaPlanoConta()
        {
            object idContaTxAntecip = ConfigDAO.Instance.GetValue(Glass.Data.Helper.Config.ConfigEnum.PlanoContaTaxaAntecip, Glass.Conversoes.StrParaUint(drpLoja.SelectedValue));
            object idContaJurosAntecip = ConfigDAO.Instance.GetValue(Glass.Data.Helper.Config.ConfigEnum.PlanoContaJurosAntecip, Glass.Conversoes.StrParaUint(drpLoja.SelectedValue));
            object idContaIOFAntecip = ConfigDAO.Instance.GetValue(Glass.Data.Helper.Config.ConfigEnum.PlanoContaIOFAntecip, Glass.Conversoes.StrParaUint(drpLoja.SelectedValue));
    
            object idContaJurosReceb = ConfigDAO.Instance.GetValue(Glass.Data.Helper.Config.ConfigEnum.PlanoContaJurosReceb, Glass.Conversoes.StrParaUint(drpLoja.SelectedValue));
            object idContaMultaReceb = ConfigDAO.Instance.GetValue(Glass.Data.Helper.Config.ConfigEnum.PlanoContaMultaReceb, Glass.Conversoes.StrParaUint(drpLoja.SelectedValue));
            object idContaJurosPagto = ConfigDAO.Instance.GetValue(Glass.Data.Helper.Config.ConfigEnum.PlanoContaJurosPagto, Glass.Conversoes.StrParaUint(drpLoja.SelectedValue));
            object idContaMultaPagto = ConfigDAO.Instance.GetValue(Glass.Data.Helper.Config.ConfigEnum.PlanoContaMultaPagto, Glass.Conversoes.StrParaUint(drpLoja.SelectedValue));
    
            object idEstContaJurosReceb = ConfigDAO.Instance.GetValue(Glass.Data.Helper.Config.ConfigEnum.PlanoContaEstornoJurosReceb, Glass.Conversoes.StrParaUint(drpLoja.SelectedValue));
            object idEstContaMultaReceb = ConfigDAO.Instance.GetValue(Glass.Data.Helper.Config.ConfigEnum.PlanoContaEstornoMultaReceb, Glass.Conversoes.StrParaUint(drpLoja.SelectedValue));
            object idEstContaJurosPagto = ConfigDAO.Instance.GetValue(Glass.Data.Helper.Config.ConfigEnum.PlanoContaEstornoJurosPagto, Glass.Conversoes.StrParaUint(drpLoja.SelectedValue));
            object idEstContaMultaPagto = ConfigDAO.Instance.GetValue(Glass.Data.Helper.Config.ConfigEnum.PlanoContaEstornoMultaPagto, Glass.Conversoes.StrParaUint(drpLoja.SelectedValue));
    
            object idComissao = ConfigDAO.Instance.GetValue(Glass.Data.Helper.Config.ConfigEnum.PlanoContaComissao, Glass.Conversoes.StrParaUint(drpLoja.SelectedValue));

            object idQuitacaoParcelaCartao = ConfigDAO.Instance.GetValue(Config.ConfigEnum.PlanoContaQuitacaoParcelaCartao, Glass.Conversoes.StrParaUint(drpLoja.SelectedValue));
            object idEstornoQuitacaoParcelaCartao = ConfigDAO.Instance.GetValue(Config.ConfigEnum.PlanoContaEstornoQuitacaoParcelaCartao, Glass.Conversoes.StrParaUint(drpLoja.SelectedValue));

            object idJurosCartao = ConfigDAO.Instance.GetValue(Glass.Data.Helper.Config.ConfigEnum.PlanoContaJurosCartao, Glass.Conversoes.StrParaUint(drpLoja.SelectedValue));
            object idEstJurosCartao = ConfigDAO.Instance.GetValue(Glass.Data.Helper.Config.ConfigEnum.PlanoContaEstornoJurosCartao, Glass.Conversoes.StrParaUint(drpLoja.SelectedValue));

            object idContaTarifaUsoBoleto = ConfigDAO.Instance.GetValue(Glass.Data.Helper.Config.ConfigEnum.PlanoContaTarifaUsoBoleto, Glass.Conversoes.StrParaUint(drpLoja.SelectedValue));
            object idContaTarifaUsoProtesto = ConfigDAO.Instance.GetValue(Glass.Data.Helper.Config.ConfigEnum.PlanoContaTarifaUsoProtesto, Glass.Conversoes.StrParaUint(drpLoja.SelectedValue));
            
            if (idContaTxAntecip != null && idContaTxAntecip.ToString() != String.Empty && idContaTxAntecip.ToString() != "0")
            {
                PlanoContas pc = PlanoContasDAO.Instance.GetByIdConta(Glass.Conversoes.StrParaUint(idContaTxAntecip.ToString()));

                if (pc != null)
                {
                    lblTxAntecip.Text = pc.DescrPlanoGrupo;
                    hdfTxAntecip.Value = pc.IdConta.ToString();
                }
            }
    
            if (idContaJurosAntecip != null && idContaJurosAntecip.ToString() != String.Empty && idContaJurosAntecip.ToString() != "0")
            {
                PlanoContas pc = PlanoContasDAO.Instance.GetByIdConta(Glass.Conversoes.StrParaUint(idContaJurosAntecip.ToString()));

                if (pc != null)
                {
                    lblJurosAntecip.Text = pc.DescrPlanoGrupo;
                    hdfJurosAntecip.Value = pc.IdConta.ToString();
                }
            }
    
            if (idContaIOFAntecip != null && idContaIOFAntecip.ToString() != String.Empty && idContaIOFAntecip.ToString() != "0")
            {
                PlanoContas pc = PlanoContasDAO.Instance.GetByIdConta(Glass.Conversoes.StrParaUint(idContaIOFAntecip.ToString()));

                if (pc != null)
                {
                    lblIOFAntecip.Text = pc.DescrPlanoGrupo;
                    hdfIOFAntecip.Value = pc.IdConta.ToString();
                }
            }
    
            if (idContaJurosReceb != null && idContaJurosReceb.ToString() != String.Empty && idContaJurosReceb.ToString() != "0")
            {
                PlanoContas pc = PlanoContasDAO.Instance.GetByIdConta(Glass.Conversoes.StrParaUint(idContaJurosReceb.ToString()));

                if (pc != null)
                {
                    lblJurosReceb.Text = pc.DescrPlanoGrupo;
                    hdfJurosReceb.Value = pc.IdConta.ToString();
                }
            }
    
            if (idContaMultaReceb != null && idContaMultaReceb.ToString() != String.Empty && idContaMultaReceb.ToString() != "0")
            {
                PlanoContas pc = PlanoContasDAO.Instance.GetByIdConta(Glass.Conversoes.StrParaUint(idContaMultaReceb.ToString()));

                if (pc != null)
                {
                    lblMultaReceb.Text = pc.DescrPlanoGrupo;
                    hdfMultaReceb.Value = pc.IdConta.ToString(); 
                }
            }
    
            if (idContaJurosPagto != null && idContaJurosPagto.ToString() != String.Empty && idContaJurosPagto.ToString() != "0")
            {
                PlanoContas pc = PlanoContasDAO.Instance.GetByIdConta(Glass.Conversoes.StrParaUint(idContaJurosPagto.ToString()));

                if (pc != null)
                {
                    lblJurosPagto.Text = pc.DescrPlanoGrupo;
                    hdfJurosPagto.Value = pc.IdConta.ToString();
                }
            }
    
            if (idContaMultaPagto != null && idContaMultaPagto.ToString() != String.Empty && idContaMultaPagto.ToString() != "0")
            {
                PlanoContas pc = PlanoContasDAO.Instance.GetByIdConta(Glass.Conversoes.StrParaUint(idContaMultaPagto.ToString()));

                if (pc != null)
                {
                    lblMultaPagto.Text = pc.DescrPlanoGrupo;
                    hdfMultaPagto.Value = pc.IdConta.ToString();
                }
            }
    
            if (idEstContaJurosReceb != null && idEstContaJurosReceb.ToString() != String.Empty && idEstContaJurosReceb.ToString() != "0")
            {
                PlanoContas pc = PlanoContasDAO.Instance.GetByIdConta(Glass.Conversoes.StrParaUint(idEstContaJurosReceb.ToString()));

                if (pc != null)
                {
                    lblEstJurosReceb.Text = pc.DescrPlanoGrupo;
                    hdfEstJurosReceb.Value = pc.IdConta.ToString();
                }
            }
    
            if (idEstContaMultaReceb != null && idEstContaMultaReceb.ToString() != String.Empty && idEstContaMultaReceb.ToString() != "0")
            {
                PlanoContas pc = PlanoContasDAO.Instance.GetByIdConta(Glass.Conversoes.StrParaUint(idEstContaMultaReceb.ToString()));

                if (pc != null)
                {
                    lblEstMultaReceb.Text = pc.DescrPlanoGrupo;
                    hdfEstMultaReceb.Value = pc.IdConta.ToString();
                }
            }
    
            if (idEstContaJurosPagto != null && idEstContaJurosPagto.ToString() != String.Empty && idEstContaJurosPagto.ToString() != "0")
            {
                PlanoContas pc = PlanoContasDAO.Instance.GetByIdConta(Glass.Conversoes.StrParaUint(idEstContaJurosPagto.ToString()));
                lblEstJurosPagto.Text = pc.DescrPlanoGrupo;
                hdfEstJurosPagto.Value = pc.IdConta.ToString();
            }
    
            if (idEstContaMultaPagto != null && idEstContaMultaPagto.ToString() != String.Empty && idEstContaMultaPagto.ToString() != "0")
            {
                PlanoContas pc = PlanoContasDAO.Instance.GetByIdConta(Glass.Conversoes.StrParaUint(idEstContaMultaPagto.ToString()));

                if (pc != null)
                {
                    lblEstMultaPagto.Text = pc.DescrPlanoGrupo;
                    hdfEstMultaPagto.Value = pc.IdConta.ToString();
                }
            }
    
            if (idComissao != null && idComissao.ToString() != String.Empty && idComissao.ToString() != "0")
            {
                PlanoContas pc = PlanoContasDAO.Instance.GetByIdConta(Glass.Conversoes.StrParaUint(idComissao.ToString()));

                if (pc != null)
                {
                    lblComissao.Text = pc.DescrPlanoGrupo;
                    hdfComissao.Value = pc.IdConta.ToString();
                }
            }

            if (idQuitacaoParcelaCartao != null && idQuitacaoParcelaCartao.ToString() != String.Empty && idQuitacaoParcelaCartao.ToString() != "0")
            {
                PlanoContas pc = PlanoContasDAO.Instance.GetByIdConta(Glass.Conversoes.StrParaUint(idQuitacaoParcelaCartao.ToString()));

                if (pc != null)
                {
                    lblQuitacaoParcelaCartao.Text = pc.DescrPlanoGrupo;
                    hdfQuitacaoParcelaCartao.Value = pc.IdConta.ToString();
                }
            }

            if (idEstornoQuitacaoParcelaCartao != null && idEstornoQuitacaoParcelaCartao.ToString() != String.Empty && idEstornoQuitacaoParcelaCartao.ToString() != "0")
            {
                PlanoContas pc = PlanoContasDAO.Instance.GetByIdConta(Glass.Conversoes.StrParaUint(idEstornoQuitacaoParcelaCartao.ToString()));

                if (pc != null)
                {
                    lblEstornoQuitacaoParcelaCartao.Text = pc.DescrPlanoGrupo;
                    hdfEstornoQuitacaoParcelaCartao.Value = pc.IdConta.ToString();
                }
            }

            if (idJurosCartao != null && idJurosCartao.ToString() != String.Empty && idJurosCartao.ToString() != "0")
            {
                PlanoContas pc = PlanoContasDAO.Instance.GetByIdConta(Glass.Conversoes.StrParaUint(idJurosCartao.ToString()));

                if (pc != null)
                {
                    lblJurosCartao.Text = pc.DescrPlanoGrupo;
                    hdfJurosCartao.Value = pc.IdConta.ToString();
                }
            }
    
            if (idEstJurosCartao != null && idEstJurosCartao.ToString() != String.Empty && idEstJurosCartao.ToString() != "0")
            {
                PlanoContas pc = PlanoContasDAO.Instance.GetByIdConta(Glass.Conversoes.StrParaUint(idEstJurosCartao.ToString()));

                if (pc != null)
                {
                    lblEstJurosCartao.Text = pc.DescrPlanoGrupo;
                    hdfEstJurosCartao.Value = pc.IdConta.ToString();
                }
            }

            if (idContaTarifaUsoBoleto != null && idContaTarifaUsoBoleto.ToString() != String.Empty && idContaTarifaUsoBoleto.ToString() != "0")
            {
                PlanoContas pc = PlanoContasDAO.Instance.GetByIdConta((uint)(int)idContaTarifaUsoBoleto);

                if (pc != null)
                {
                    lblTarifaUsoBoleto.Text = pc.DescrPlanoGrupo;
                    hdfTarifaUsoBoleto.Value = pc.IdConta.ToString();
                }
            }

            if (idContaTarifaUsoProtesto != null && idContaTarifaUsoProtesto.ToString() != String.Empty && idContaTarifaUsoProtesto.ToString() != "0")
            {
                PlanoContas pc = PlanoContasDAO.Instance.GetByIdConta((uint)(int)idContaTarifaUsoProtesto);

                if (pc != null)
                {
                    lblTarifaUsoProtesto.Text = pc.DescrPlanoGrupo;
                    hdfTarifaUsoProtesto.Value = pc.IdConta.ToString();
                }
            }
        }
    
        #endregion
    
        protected void btnSalvarPlanoConta_Click(object sender, EventArgs e)
        {
            // Busca planos de conta selecionados
            uint? idContaTxAntecip = !String.IsNullOrEmpty(hdfTxAntecip.Value) ? (uint?)Glass.Conversoes.StrParaUint(hdfTxAntecip.Value) : null;
            uint? idContaJurosAntecip = !String.IsNullOrEmpty(hdfJurosAntecip.Value) ? (uint?)Glass.Conversoes.StrParaUint(hdfJurosAntecip.Value) : null;
            uint? idContaIOFAntecip = !String.IsNullOrEmpty(hdfIOFAntecip.Value) ? (uint?)Glass.Conversoes.StrParaUint(hdfIOFAntecip.Value) : null;
    
            uint? idContaJurosReceb = !String.IsNullOrEmpty(hdfJurosReceb.Value) ? (uint?)Glass.Conversoes.StrParaUint(hdfJurosReceb.Value) : null;
            uint? idContaMultaReceb = !String.IsNullOrEmpty(hdfMultaReceb.Value) ? (uint?)Glass.Conversoes.StrParaUint(hdfMultaReceb.Value) : null;
            uint? idContaJurosPagto = !String.IsNullOrEmpty(hdfJurosPagto.Value) ? (uint?)Glass.Conversoes.StrParaUint(hdfJurosPagto.Value) : null;
            uint? idContaMultaPagto = !String.IsNullOrEmpty(hdfMultaPagto.Value) ? (uint?)Glass.Conversoes.StrParaUint(hdfMultaPagto.Value) : null;
    
            uint? idContaEstJurosReceb = !String.IsNullOrEmpty(hdfEstJurosReceb.Value) ? (uint?)Glass.Conversoes.StrParaUint(hdfEstJurosReceb.Value) : null;
            uint? idContaEstMultaReceb = !String.IsNullOrEmpty(hdfEstMultaReceb.Value) ? (uint?)Glass.Conversoes.StrParaUint(hdfEstMultaReceb.Value) : null;
            uint? idContaEstJurosPagto = !String.IsNullOrEmpty(hdfEstJurosPagto.Value) ? (uint?)Glass.Conversoes.StrParaUint(hdfEstJurosPagto.Value) : null;
            uint? idContaEstMultaPagto = !String.IsNullOrEmpty(hdfEstMultaPagto.Value) ? (uint?)Glass.Conversoes.StrParaUint(hdfEstMultaPagto.Value) : null;
    
            uint? idContaComissao = !String.IsNullOrEmpty(hdfComissao.Value) ? (uint?)Glass.Conversoes.StrParaUint(hdfComissao.Value) : null;

            uint? idContaQuitacaoParcelaCartao = !String.IsNullOrEmpty(hdfQuitacaoParcelaCartao.Value) ? (uint?)Glass.Conversoes.StrParaUint(hdfQuitacaoParcelaCartao.Value) : null;
            uint? idContaEstornoQuitacaoParcelaCartao = !String.IsNullOrEmpty(hdfEstornoQuitacaoParcelaCartao.Value) ? (uint?)Glass.Conversoes.StrParaUint(hdfEstornoQuitacaoParcelaCartao.Value) : null;

            uint? idContaJurosCartao = !String.IsNullOrEmpty(hdfJurosCartao.Value) ? (uint?)Glass.Conversoes.StrParaUint(hdfJurosCartao.Value) : null;
            uint? idContaEstJurosCartao = !String.IsNullOrEmpty(hdfEstJurosCartao.Value) ? (uint?)Glass.Conversoes.StrParaUint(hdfEstJurosCartao.Value) : null;

            uint? idContaTarifaUsoBoleto = !String.IsNullOrEmpty(hdfTarifaUsoBoleto.Value) ? (uint?)Glass.Conversoes.StrParaUint(hdfTarifaUsoBoleto.Value) : null;
            uint? idContaTarifaUsoProtesto = !String.IsNullOrEmpty(hdfTarifaUsoProtesto.Value) ? (uint?)Glass.Conversoes.StrParaUint(hdfTarifaUsoProtesto.Value) : null;
    
            #region Verifica se o plano pode ser alterado
    
            // Recupera o valor de cada idConta j� salvo
            uint idContaTxAntecipOld = FinanceiroConfig.PlanoContaTaxaAntecip;
            uint idContaJurosAntecipOld = FinanceiroConfig.PlanoContaJurosAntecip;
            uint idContaIOFAntecipOld = FinanceiroConfig.PlanoContaIOFAntecip;
            uint idContaJurosRecebOld = FinanceiroConfig.PlanoContaJurosReceb;
            uint idContaMultaRecebOld = FinanceiroConfig.PlanoContaMultaReceb;
            uint idContaJurosPagtoOld = FinanceiroConfig.PlanoContaJurosPagto;
            uint idContaMultaPagtoOld = FinanceiroConfig.PlanoContaMultaPagto;
            uint idContaEstJurosRecebOld = FinanceiroConfig.PlanoContaEstornoJurosReceb;
            uint idContaEstMultaRecebOld = FinanceiroConfig.PlanoContaEstornoMultaReceb;
            uint idContaEstJurosPagtoOld = FinanceiroConfig.PlanoContaEstornoJurosPagto;
            uint idContaEstMultaPagtoOld = FinanceiroConfig.PlanoContaEstornoMultaPagto;
            uint idContaTarifaUsoBoletoOld = FinanceiroConfig.PlanoContaTarifaUsoBoleto;
            uint idContaTarifaUsoProtestoOld = FinanceiroConfig.PlanoContaTarifaUsoProtesto;
            //uint idContaComissaoOld = FinanceiroConfig.PlanoContaComissao;
    
            try
            {
                if (idContaTxAntecip > 0 && idContaTxAntecipOld > 0 && idContaTxAntecipOld != idContaTxAntecip &&
                    PlanoContasDAO.Instance.EstaSendoUsado(idContaTxAntecipOld, true))
                    throw new Exception("O plano de conta " + PlanoContasDAO.Instance.GetDescricao(idContaTxAntecipOld, false) +
                    " n�o pode ser alterado pois j� est� sendo usado.");
    
                if (idContaJurosAntecip > 0 && idContaJurosAntecipOld > 0 && idContaJurosAntecipOld != idContaJurosAntecip &&
                    PlanoContasDAO.Instance.EstaSendoUsado(idContaJurosAntecipOld, true))
                    throw new Exception("O plano de conta " + PlanoContasDAO.Instance.GetDescricao(idContaJurosAntecipOld, false) +
                    " n�o pode ser alterado pois j� est� sendo usado.");
    
                if (idContaIOFAntecip > 0 && idContaIOFAntecipOld > 0 && idContaIOFAntecipOld != idContaIOFAntecip &&
                    PlanoContasDAO.Instance.EstaSendoUsado(idContaIOFAntecipOld, true))
                    throw new Exception("O plano de conta " + PlanoContasDAO.Instance.GetDescricao(idContaIOFAntecipOld, false) +
                    " n�o pode ser alterado pois j� est� sendo usado.");
    
                if (idContaJurosReceb > 0 && idContaJurosRecebOld > 0 && idContaJurosRecebOld != idContaJurosReceb &&
                    PlanoContasDAO.Instance.EstaSendoUsado(idContaJurosRecebOld, true))
                    throw new Exception("O plano de conta " + PlanoContasDAO.Instance.GetDescricao(idContaJurosRecebOld, false) +
                    " n�o pode ser alterado pois j� est� sendo usado.");
    
                if (idContaMultaReceb > 0 && idContaMultaRecebOld > 0 && idContaMultaRecebOld != idContaMultaReceb &&
                    PlanoContasDAO.Instance.EstaSendoUsado(idContaMultaRecebOld, true))
                    throw new Exception("O plano de conta " + PlanoContasDAO.Instance.GetDescricao(idContaMultaRecebOld, false) +
                    " n�o pode ser alterado pois j� est� sendo usado.");
    
                if (idContaJurosPagto > 0 && idContaJurosPagtoOld > 0 && idContaJurosPagtoOld != idContaJurosPagto &&
                    PlanoContasDAO.Instance.EstaSendoUsado(idContaJurosPagtoOld, true))
                    throw new Exception("O plano de conta " + PlanoContasDAO.Instance.GetDescricao(idContaJurosPagtoOld, false) +
                    " n�o pode ser alterado pois j� est� sendo usado.");
    
                if (idContaMultaPagto > 0 && idContaMultaPagtoOld > 0 && idContaMultaPagtoOld != idContaMultaPagto &&
                    PlanoContasDAO.Instance.EstaSendoUsado(idContaMultaPagtoOld, true))
                    throw new Exception("O plano de conta " + PlanoContasDAO.Instance.GetDescricao(idContaMultaPagtoOld, false) +
                    " n�o pode ser alterado pois j� est� sendo usado.");
    
                if (idContaEstJurosReceb > 0 && idContaEstJurosRecebOld > 0 && idContaEstJurosRecebOld != idContaEstJurosReceb &&
                    PlanoContasDAO.Instance.EstaSendoUsado(idContaEstJurosRecebOld, true))
                    throw new Exception("O plano de conta " + PlanoContasDAO.Instance.GetDescricao(idContaEstJurosRecebOld, false) +
                    " n�o pode ser alterado pois j� est� sendo usado.");
    
                if (idContaEstMultaReceb > 0 && idContaEstMultaRecebOld > 0 && idContaEstMultaRecebOld != idContaEstMultaReceb &&
                    PlanoContasDAO.Instance.EstaSendoUsado(idContaEstMultaRecebOld, true))
                    throw new Exception("O plano de conta " + PlanoContasDAO.Instance.GetDescricao(idContaEstMultaRecebOld, false) +
                    " n�o pode ser alterado pois j� est� sendo usado.");
    
                if (idContaEstJurosPagto > 0 && idContaEstJurosPagtoOld > 0 && idContaEstJurosPagtoOld != idContaEstJurosPagto &&
                    PlanoContasDAO.Instance.EstaSendoUsado(idContaEstJurosPagtoOld, true))
                    throw new Exception("O plano de conta " + PlanoContasDAO.Instance.GetDescricao(idContaEstJurosPagtoOld, false) +
                    " n�o pode ser alterado pois j� est� sendo usado.");
    
                if (idContaEstMultaPagto > 0 && idContaEstMultaPagtoOld > 0 && idContaEstMultaPagtoOld != idContaEstMultaPagto &&
                    PlanoContasDAO.Instance.EstaSendoUsado(idContaEstMultaPagtoOld, true))
                    throw new Exception("O plano de conta " + PlanoContasDAO.Instance.GetDescricao(idContaEstMultaPagtoOld, false) +
                    " n�o pode ser alterado pois j� est� sendo usado.");

                if (idContaTarifaUsoBoleto > 0 && idContaTarifaUsoBoletoOld > 0 && idContaTarifaUsoBoletoOld != idContaTarifaUsoBoleto &&
                   PlanoContasDAO.Instance.EstaSendoUsado(idContaTarifaUsoBoletoOld, true))
                    throw new Exception("O plano de conta " + PlanoContasDAO.Instance.GetDescricao(idContaTarifaUsoBoletoOld, false) +
                    " n�o pode ser alterado pois j� est� sendo usado.");

                if (idContaTarifaUsoProtesto > 0 && idContaTarifaUsoProtestoOld > 0 && idContaTarifaUsoProtestoOld != idContaTarifaUsoProtesto &&
                   PlanoContasDAO.Instance.EstaSendoUsado(idContaTarifaUsoProtestoOld, true))
                    throw new Exception("O plano de conta " + PlanoContasDAO.Instance.GetDescricao(idContaTarifaUsoProtestoOld, false) +
                    " n�o pode ser alterado pois j� est� sendo usado.");
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg(null, ex, Page);
                return;
            }
    
            #endregion
    
            // Salva o novo valor atribu�do ao plano de contas
            ConfigDAO.Instance.SetValue(Glass.Data.Helper.Config.ConfigEnum.PlanoContaTaxaAntecip, Glass.Conversoes.StrParaUint(drpLoja.SelectedValue), idContaTxAntecip);
            ConfigDAO.Instance.SetValue(Glass.Data.Helper.Config.ConfigEnum.PlanoContaJurosAntecip, Glass.Conversoes.StrParaUint(drpLoja.SelectedValue), idContaJurosAntecip);
            ConfigDAO.Instance.SetValue(Glass.Data.Helper.Config.ConfigEnum.PlanoContaIOFAntecip, Glass.Conversoes.StrParaUint(drpLoja.SelectedValue), idContaIOFAntecip);
    
            ConfigDAO.Instance.SetValue(Glass.Data.Helper.Config.ConfigEnum.PlanoContaJurosReceb, Glass.Conversoes.StrParaUint(drpLoja.SelectedValue), idContaJurosReceb);
            ConfigDAO.Instance.SetValue(Glass.Data.Helper.Config.ConfigEnum.PlanoContaMultaReceb, Glass.Conversoes.StrParaUint(drpLoja.SelectedValue), idContaMultaReceb);
            ConfigDAO.Instance.SetValue(Glass.Data.Helper.Config.ConfigEnum.PlanoContaJurosPagto, Glass.Conversoes.StrParaUint(drpLoja.SelectedValue), idContaJurosPagto);
            ConfigDAO.Instance.SetValue(Glass.Data.Helper.Config.ConfigEnum.PlanoContaMultaPagto, Glass.Conversoes.StrParaUint(drpLoja.SelectedValue), idContaMultaPagto);
    
            ConfigDAO.Instance.SetValue(Glass.Data.Helper.Config.ConfigEnum.PlanoContaEstornoJurosReceb, Glass.Conversoes.StrParaUint(drpLoja.SelectedValue), idContaEstJurosReceb);
            ConfigDAO.Instance.SetValue(Glass.Data.Helper.Config.ConfigEnum.PlanoContaEstornoMultaReceb, Glass.Conversoes.StrParaUint(drpLoja.SelectedValue), idContaEstMultaReceb);
            ConfigDAO.Instance.SetValue(Glass.Data.Helper.Config.ConfigEnum.PlanoContaEstornoJurosPagto, Glass.Conversoes.StrParaUint(drpLoja.SelectedValue), idContaEstJurosPagto);
            ConfigDAO.Instance.SetValue(Glass.Data.Helper.Config.ConfigEnum.PlanoContaEstornoMultaPagto, Glass.Conversoes.StrParaUint(drpLoja.SelectedValue), idContaEstMultaPagto);
    
            ConfigDAO.Instance.SetValue(Glass.Data.Helper.Config.ConfigEnum.PlanoContaComissao, Glass.Conversoes.StrParaUint(drpLoja.SelectedValue), idContaComissao);

            ConfigDAO.Instance.SetValue(Glass.Data.Helper.Config.ConfigEnum.PlanoContaQuitacaoParcelaCartao, Glass.Conversoes.StrParaUint(drpLoja.SelectedValue), idContaQuitacaoParcelaCartao);
            ConfigDAO.Instance.SetValue(Glass.Data.Helper.Config.ConfigEnum.PlanoContaEstornoQuitacaoParcelaCartao, Glass.Conversoes.StrParaUint(drpLoja.SelectedValue), idContaEstornoQuitacaoParcelaCartao);

            ConfigDAO.Instance.SetValue(Glass.Data.Helper.Config.ConfigEnum.PlanoContaJurosCartao, Glass.Conversoes.StrParaUint(drpLoja.SelectedValue), idContaJurosCartao);
            ConfigDAO.Instance.SetValue(Glass.Data.Helper.Config.ConfigEnum.PlanoContaEstornoJurosCartao, Glass.Conversoes.StrParaUint(drpLoja.SelectedValue), idContaEstJurosCartao);

            ConfigDAO.Instance.SetValue(Glass.Data.Helper.Config.ConfigEnum.PlanoContaTarifaUsoBoleto, Glass.Conversoes.StrParaUint(drpLoja.SelectedValue), idContaTarifaUsoBoleto);
            ConfigDAO.Instance.SetValue(Glass.Data.Helper.Config.ConfigEnum.PlanoContaTarifaUsoProtesto, Glass.Conversoes.StrParaUint(drpLoja.SelectedValue), idContaTarifaUsoProtesto);
    
            Glass.MensagemAlerta.ShowMsg("Planos de conta alterados!", Page);
            CarregaPlanoConta();
        }
    
        #endregion
    
        #region Geral
    
        protected void tblGeral_Load(object sender, EventArgs e)
        {
            TableRow[] linhas = LinhasItens(ConfigDAO.Instance.GetItensGeral(), "aba_geral", null);
            Config_EndLoad(tblGeral, linhas, "aba_geral");
            
            tblGeral.PreRender += new EventHandler(
                delegate(object s, EventArgs ea)
                {
                    configGeral.Visible = (s as Table).Visible;
                }
            );
        }
    
        #endregion
    
        #region Or�amento
    
        protected void tblOrcamento_Load(object sender, EventArgs e)
        {
            TableRow[] linhas = LinhasItens(ConfigDAO.Instance.GetItensOrcamento(), "aba_orcamento", null);
            Config_EndLoad(tblOrcamento, linhas, "aba_orcamento");
        }
    
        #endregion
    
        #region Pedido
    
        protected void tblPedido_Load(object sender, EventArgs e)
        {
            TableRow[] linhas = LinhasItens(ConfigDAO.Instance.GetItensPedido(), "aba_pedido", null);
            Config_EndLoad(tblPedido, linhas, "aba_pedido");
        }
    
        #endregion
    
        #region Libera��o
    
        protected void tblLiberarPedido_Load(object sender, EventArgs e)
        {
            TableRow[] linhas = LinhasItens(ConfigDAO.Instance.GetItensLiberarPedido(), "aba_liberarPedido", null);
            Config_EndLoad(tblLiberarPedido, linhas, "aba_liberarPedido");
        }
    
        #endregion
    
        #region Projeto
    
        protected void tblProjeto_Load(object sender, EventArgs e)
        {
            TableRow[] linhas = LinhasItens(ConfigDAO.Instance.GetItensProjeto(), "aba_projeto", null);
            Config_EndLoad(tblProjeto, linhas, "aba_projeto");
        }
    
        #endregion
    
        #region Fiscal
    
        protected void tblNFe_Load(object sender, EventArgs e)
        {
            TableRow[] linhas = LinhasItens(ConfigDAO.Instance.GetItensNFe(), "aba_nfe", null);
            Config_EndLoad(tblNFe, linhas, "aba_nfe");
        }
    
        #endregion
    
        #region PCP
    
        protected void tblPCP_Load(object sender, EventArgs e)
        {
            TableRow[] linhas = LinhasItens(ConfigDAO.Instance.GetItensPCP(), "aba_pcp", null);
            Config_EndLoad(tblPCP, linhas, "aba_pcp");
        }

        #endregion

        #region Aresta

        protected void ctrlConfigAresta_Load(object sender, EventArgs e)
        {
            var config = ConfigDAO.Instance.GetValue(Config.ConfigEnum.ConfigAresta, drpLoja.SelectedValue.StrParaUint());

            if (config != null)
                ((Controls.ctrlConfigAresta)sender).ConfigAresta = config.ToString();
        }

        [Ajax.AjaxMethod]
        public string SalvarConfiguracaoAresta(string idLoja, string configuracaoNova)
        {
            var configuracaoAntiga = string.Empty;

            try
            {
                configuracaoAntiga = PCPConfig.ObtemArestaConfig;
                ConfigDAO.Instance.SetValue(Config.ConfigEnum.ConfigAresta, idLoja.StrParaUint(), configuracaoNova);
                LogAlteracaoDAO.Instance.LogConfiguracaoAresta(null, configuracaoAntiga, configuracaoNova);

                return ";Configura��o da aresta salva";
            }
            catch (Exception ex)
            {
                return $@";N�o foi poss�vel salvar a configura��o da aresta. Falha: { ex?.Message ?? ex?.InnerException?.Message ?? string.Empty }.
                    Configura��o nova: { configuracaoNova }.
                    Configura��o atual: { configuracaoAntiga }.";
            }
        }

        #endregion

        #region Internas

        protected void tblInternas_Load(object sender, EventArgs e)
        {
            if (UserInfo.GetUserInfo.IsAdminSync)
            {
                TableRow[] linhas = LinhasItens(ConfigDAO.Instance.GetItensInternas(), "aba_internas", null);
                Config_EndLoad(tblInternas, linhas, "aba_internas");
            }
        }

        #endregion

        protected void btnSalvar_Click(object sender, EventArgs e)
        {
            var idLoja = drpLoja.SelectedValue.StrParaUint();
            var idFunc = ((HiddenField)dtvComissaoGerente.FindControl("hdfIdFuncGerente")).Value.StrParaUint();

            if (idFunc == 0)
                throw new Exception("Selecione o gerente.");

            var percentualVenda = ((TextBox)dtvComissaoGerente.FindControl("txbVenda")).Text.StrParaDecimal();
            var percentualRevenda = ((TextBox)dtvComissaoGerente.FindControl("txbRevenda")).Text.StrParaDecimal();
            var percentualMaoDeObra = ((TextBox)dtvComissaoGerente.FindControl("txbMaoDeObra")).Text.StrParaDecimal();
            var percentualMaoDeObraEspecial = ((TextBox)dtvComissaoGerente.FindControl("txbMaoDeObraEspecial")).Text.StrParaDecimal();

            var comissaoGerenteAtualizar = ComissaoConfigGerenteDAO.Instance.GetByIdFuncIdLoja(idLoja, idFunc);

            if (comissaoGerenteAtualizar == null || comissaoGerenteAtualizar.Count == 0)
            {
                var comissao = new ComissaoConfigGerente();
                // Salva a comiss�o atual antes de ser atualizada, para Log
                var comissaoAtual = new ComissaoConfigGerente
                {
                    IdLoja = comissao.IdLoja,
                    IdFuncionario = comissao.IdFuncionario,
                    PercentualVenda = comissao.PercentualVenda,
                    PercentualRevenda = comissao.PercentualRevenda,
                    PercentualMaoDeObra = comissao.PercentualMaoDeObra,
                    PercentualMaoDeObraEspecial = comissao.PercentualMaoDeObraEspecial
                };

                comissao.IdFuncionario = idFunc;
                comissao.IdLoja = idLoja;
                comissao.PercentualVenda = percentualVenda;
                comissao.PercentualRevenda = percentualRevenda;
                comissao.PercentualMaoDeObra = percentualMaoDeObra;
                comissao.PercentualMaoDeObraEspecial = percentualMaoDeObraEspecial;

                ComissaoConfigGerenteDAO.Instance.InsertOrUpdate(comissao);
                LogAlteracaoDAO.Instance.LogComissaoConfigGerente(comissaoAtual, comissao);
            }
            else
                foreach (var comissao in comissaoGerenteAtualizar)
                {
                    // Salva a comiss�o atual antes de ser atualizada, para Log
                    var comissaoAtual = new ComissaoConfigGerente{
                        IdLoja = comissao.IdLoja,
                        IdFuncionario = comissao.IdFuncionario,
                        PercentualVenda = comissao.PercentualVenda,
                        PercentualRevenda = comissao.PercentualRevenda,
                        PercentualMaoDeObra = comissao.PercentualMaoDeObra,
                        PercentualMaoDeObraEspecial = comissao.PercentualMaoDeObraEspecial
                    };

                    comissao.IdFuncionario = idFunc;
                    comissao.IdLoja = idLoja;
                    comissao.PercentualVenda = percentualVenda;
                    comissao.PercentualRevenda = percentualRevenda;
                    comissao.PercentualMaoDeObra = percentualMaoDeObra;
                    comissao.PercentualMaoDeObraEspecial = percentualMaoDeObraEspecial;

                    ComissaoConfigGerenteDAO.Instance.InsertOrUpdate(comissao);
                    LogAlteracaoDAO.Instance.LogComissaoConfigGerente(comissaoAtual, comissao);
                }
        }

        protected void drpGerente_SelectedIndexChanged(object sender, EventArgs e)
        {
            dtvComissaoGerente.DataBind();
            ((HiddenField)dtvComissaoGerente.FindControl("hdfIdFuncGerente")).Value = drpGerente.SelectedValue == "0" ? 
                string.Empty : drpGerente.SelectedValue;
        }

        protected void dtvComissaoGerente_DataBound(object sender, EventArgs e)
        {
            var gerentes = FuncionarioDAO.Instance.GetGerentesForComissao();
            var retorno = string.Format("Gerentes Cadastrados: {0}",  string.Join(", ", gerentes.Select(f => f.Nome)));

            if (((Label)dtvComissaoGerente.FindControl("lblGerentes")) != null)
            {
                ((Label)dtvComissaoGerente.FindControl("lblGerentes")).Text = retorno;
                ((Label)dtvComissaoGerente.FindControl("lblGerentes")).ForeColor = System.Drawing.Color.Red;
            }
        }
    }
}
