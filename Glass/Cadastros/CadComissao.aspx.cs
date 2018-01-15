using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.DAL;
using Glass.Data.Model;
using System.Collections.Generic;
using Glass.Configuracoes;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadComissao : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadComissao));
    
            if (!IsPostBack)
            {
                // Coloca como per�odo o m�s anterior inteiro
                DateTime final = DateTime.Now.AddDays(-DateTime.Now.Day);
                DateTime inicial = final.AddDays(1 - final.Day);
    
                ((TextBox)ctrlDataIni.FindControl("txtData")).Text = inicial.ToString("dd/MM/yyyy");
                ((TextBox)ctrlDataFim.FindControl("txtData")).Text = final.ToString("dd/MM/yyyy");
    
                // Coloca como data de vencimento para a parcela da comiss�o a
                // data atual acrescida de 1 m�s
                ((TextBox)ctrlDataComissao.FindControl("txtData")).Text = DateTime.Now.AddMonths(1).ToString("dd/MM/yyyy");
    
                switch (Configuracoes.ComissaoConfig.TotalParaComissao)
                {
                    case Configuracoes.ComissaoConfig.TotalComissaoEnum.TotalSemIcms:
                        lblTotalParaComissao.Text = "* Base calc. comiss�o deduzido de ICMS.";
                        break;
                    case Configuracoes.ComissaoConfig.TotalComissaoEnum.TotalSemImpostos:
                        lblTotalParaComissao.Text = "* Base calc. comiss�o deduzido de impostos.";
                        break;
                    case Configuracoes.ComissaoConfig.TotalComissaoEnum.TotalComImpostos:
                    default:
                        lblTotalParaComissao.Visible = false;
                        break;
                }
    
            }
    
            // Exibe ou esconde as colunas dependendo da configura��o do sistema
            grdComissao.Columns[4].Visible = !PedidoConfig.LiberarPedido;
            grdComissao.Columns[5].Visible = PedidoConfig.LiberarPedido;
            grdComissao.Columns[6].Visible = drpTipo.SelectedIndex == 2;

            // Verifica se a empresa faz pagamento de comiss�o para os instaladores
            drpTipo.Items[2].Enabled = Geral.ControleInstalacao && 
                PedidoConfig.Instalacao.ComissaoInstalacao;

            /* Chamado 47577.
             * N�o tinha necessidade da l�gica abaixo estar dentro do m�todo drpNome_DataBound.
             * O ideal, inclusive, � a l�gica estar no Page_Load. */
            // Exibe as colunas de Desconto e Valor da Comiss�o se a comiss�o for por valores recebidos
            // ou se houver desconto na comiss�o se houver desconto no pedido (a coluna com o valor da
            // comiss�o tamb�m � exibida se for percentual �nico)
            grdComissao.Columns[8].Visible = Glass.Configuracoes.ComissaoConfig.DescontarComissaoPerc;

            if (!PedidoConfig.Comissao.PerComissaoPedido && drpTipo.SelectedValue != "3")
                grdComissao.Columns[11].Visible = false;
            else if (sender is DropDownList)
                grdComissao.Columns[11].Visible = Glass.Configuracoes.ComissaoConfig.DescontarComissaoPerc ||
                    ComissaoConfigDAO.Instance.IsFaixaUnica(Glass.Conversoes.StrParaUint(((DropDownList)sender).SelectedValue));
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdComissao.PageIndex = 0;
            grdComissao.DataBind();
        }
    
        /// <summary>
        /// Altera o DataSource dos funcion�rios/comissionados de acordo com o tipo
        /// de comiss�o que ser� paga.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void drpTipo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (drpTipo.SelectedIndex == 0)
            {
                drpNome.DataSourceID = "odsFuncionario";
                drpNome.DataTextField = "Nome";
                drpNome.DataValueField = "IdFunc";
            }
            else if (drpTipo.SelectedIndex == 1)
            {
                drpNome.DataSourceID = "odsComissionado";
                drpNome.DataTextField = "Nome";
                drpNome.DataValueField = "IdComissionado";
            }
            else if(drpTipo.SelectedIndex == 2)
            {
                drpNome.DataSourceID = "odsInstalador";
                drpNome.DataTextField = "Nome";
                drpNome.DataValueField = "IdFunc";
            }
            else
            {
                drpNome.DataSourceID = "odsGerente";
                drpNome.DataTextField = "Nome";
                drpNome.DataValueField = "IdFunc";
            }

            /* Chamado 47577. */
            drpNome.Items.Clear();
            drpNome.Items.Add("");
            hdfNome.Value = "";
            drpNome.DataBind();
            imgPesq_Click(null, new ImageClickEventArgs(0, 0));
        }
    
        protected void grdComissao_DataBound(object sender, EventArgs e)
        {
            // S� exibe o controle para gerar comiss�o se houver pedidos
            gerarComissao.Visible = grdComissao.Rows.Count > 0;
    
            if (grdComissao.Rows.Count == 0)
            {
                grdComissao.EmptyDataText = "N�o h� comiss�es para o filtro especificado." +
                    (drpNome.Items.Count > 0 ? "<br />Verifique se a comiss�o do funcion�rio foi configurada corretamente." : "");
            }

            // Recupera e exibe os d�bitos
            Glass.Data.Model.Pedido.TipoComissao tipo = (Glass.Data.Model.Pedido.TipoComissao)Glass.Conversoes.StrParaUint(drpTipo.SelectedValue);
            KeyValuePair<string, decimal> debitos = DebitoComissaoDAO.Instance.GetDebitos(Glass.Conversoes.StrParaUint(drpNome.SelectedValue), tipo);
                    
            lblDebitos.Text = debitos.Value.ToString("0.00");
            panDebitos.Visible = debitos.Value > 0;
        }
    
        protected void btnGerarComissao_Click(object sender, EventArgs e)
        {
            string idPedido = String.Empty;
    
            // Pega o id dos Pedidos que est�o selecionados
            foreach (GridViewRow r in grdComissao.Rows)
                if (((CheckBox)r.FindControl("chkSel")).Checked)
                    idPedido += ((HiddenField)r.FindControl("hdfIdPedido")).Value + ",";
    
            try
            {
                // Recupera o valor calculado da comiss�o e o tipo de comiss�o selecionado
                decimal valorComissao = decimal.Parse(hdfValorComissao.Value);
                Glass.Data.Model.Pedido.TipoComissao tipo = (Glass.Data.Model.Pedido.TipoComissao)Glass.Conversoes.StrParaUint(drpTipo.SelectedValue);
    
                // Gera a comiss�o
                ComissaoDAO.Instance.GerarComissao(tipo, Glass.Conversoes.StrParaUint(drpNome.SelectedValue), idPedido.TrimEnd(','),
                    ((TextBox)ctrlDataIni.FindControl("txtData")).Text, ((TextBox)ctrlDataFim.FindControl("txtData")).Text,
                    valorComissao, ((TextBox)ctrlDataComissao.FindControl("txtData")).Text);
    
                Glass.MensagemAlerta.ShowMsg("Comiss�o gerada!", Page);
    
                grdComissao.DataBind();
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao gerar comiss�o.", ex, Page);
            }
        }
    
        protected void chkSel_DataBinding(object sender, EventArgs e)
        {
            GridViewRow linha = ((CheckBox)sender).Parent.Parent as GridViewRow;
            Glass.Data.Model.Pedido p = linha != null ? linha.DataItem as Glass.Data.Model.Pedido : null;
    
            if (p == null)
                return;
    
            // S� habilita o checkbox se houver comiss�o a pagar
            ((CheckBox)sender).Enabled = p.ComissaoAPagar;
    
            // Salva alguns atributos ao checkbox para utiliza��o no JavaScript
            ((CheckBox)sender).Attributes.Add("ValorComissao", p.ValorComissaoPagar.ToString().Replace(",", "."));
            ((CheckBox)sender).Attributes.Add("ValorPedido", p.Total.ToString().Replace(",", "."));
            ((CheckBox)sender).Attributes.Add("IdPedido", p.IdPedido.ToString().Replace(",", "."));
            ((CheckBox)sender).Attributes.Add("ValorBaseCalcComissao", p.ValorBaseCalcComissao.ToString().Replace(",", "."));
        }
    
        [Ajax.AjaxMethod]
        public string CalculaComissao(string tipoFuncStr, string idFuncStr, string dataIni, string dataFim, string idsPedidos)
        {
            int tipoFunc = !string.IsNullOrEmpty(tipoFuncStr) ? Conversoes.StrParaInt(tipoFuncStr) : -1;
            uint idFunc = !string.IsNullOrEmpty(idFuncStr) ? Conversoes.StrParaUint(idFuncStr) : 0;
            
            return ComissaoConfigDAO.Instance.GetComissaoValor((Glass.Data.Model.Pedido.TipoComissao)tipoFunc, idFunc, dataIni, dataFim, idsPedidos).ToString().Replace(',', '.');            
        }
    
        protected void drpNome_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Salva o nome do funcion�rio/comissionado selecionado para
            // tentar manter a sele��o em caso de altera��o de filtro
            hdfNome.Value = drpNome.SelectedValue;
        }
    
        protected void drpNome_DataBound(object sender, EventArgs e)
        {
            // Verifica se h� algum item pr�-selecionado
            if (hdfNome.Value != "")
            {
                // Tenta selecionar o mesmo funcion�rio/comissionado
                drpNome.SelectedIndex = drpNome.Items.IndexOf(drpNome.Items.FindByValue(hdfNome.Value));
    
                // Se n�o for poss�vel, limpa o HiddenField
                if (drpNome.SelectedValue != hdfNome.Value)
                    hdfNome.Value = "";
            }
        }
    }
}
