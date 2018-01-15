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
                // Coloca como período o mês anterior inteiro
                DateTime final = DateTime.Now.AddDays(-DateTime.Now.Day);
                DateTime inicial = final.AddDays(1 - final.Day);
    
                ((TextBox)ctrlDataIni.FindControl("txtData")).Text = inicial.ToString("dd/MM/yyyy");
                ((TextBox)ctrlDataFim.FindControl("txtData")).Text = final.ToString("dd/MM/yyyy");
    
                // Coloca como data de vencimento para a parcela da comissão a
                // data atual acrescida de 1 mês
                ((TextBox)ctrlDataComissao.FindControl("txtData")).Text = DateTime.Now.AddMonths(1).ToString("dd/MM/yyyy");
    
                switch (Configuracoes.ComissaoConfig.TotalParaComissao)
                {
                    case Configuracoes.ComissaoConfig.TotalComissaoEnum.TotalSemIcms:
                        lblTotalParaComissao.Text = "* Base calc. comissão deduzido de ICMS.";
                        break;
                    case Configuracoes.ComissaoConfig.TotalComissaoEnum.TotalSemImpostos:
                        lblTotalParaComissao.Text = "* Base calc. comissão deduzido de impostos.";
                        break;
                    case Configuracoes.ComissaoConfig.TotalComissaoEnum.TotalComImpostos:
                    default:
                        lblTotalParaComissao.Visible = false;
                        break;
                }
    
            }
    
            // Exibe ou esconde as colunas dependendo da configuração do sistema
            grdComissao.Columns[4].Visible = !PedidoConfig.LiberarPedido;
            grdComissao.Columns[5].Visible = PedidoConfig.LiberarPedido;
            grdComissao.Columns[6].Visible = drpTipo.SelectedIndex == 2;

            // Verifica se a empresa faz pagamento de comissão para os instaladores
            drpTipo.Items[2].Enabled = Geral.ControleInstalacao && 
                PedidoConfig.Instalacao.ComissaoInstalacao;

            /* Chamado 47577.
             * Não tinha necessidade da lógica abaixo estar dentro do método drpNome_DataBound.
             * O ideal, inclusive, é a lógica estar no Page_Load. */
            // Exibe as colunas de Desconto e Valor da Comissão se a comissão for por valores recebidos
            // ou se houver desconto na comissão se houver desconto no pedido (a coluna com o valor da
            // comissão também é exibida se for percentual único)
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
        /// Altera o DataSource dos funcionários/comissionados de acordo com o tipo
        /// de comissão que será paga.
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
            // Só exibe o controle para gerar comissão se houver pedidos
            gerarComissao.Visible = grdComissao.Rows.Count > 0;
    
            if (grdComissao.Rows.Count == 0)
            {
                grdComissao.EmptyDataText = "Não há comissões para o filtro especificado." +
                    (drpNome.Items.Count > 0 ? "<br />Verifique se a comissão do funcionário foi configurada corretamente." : "");
            }

            // Recupera e exibe os débitos
            Glass.Data.Model.Pedido.TipoComissao tipo = (Glass.Data.Model.Pedido.TipoComissao)Glass.Conversoes.StrParaUint(drpTipo.SelectedValue);
            KeyValuePair<string, decimal> debitos = DebitoComissaoDAO.Instance.GetDebitos(Glass.Conversoes.StrParaUint(drpNome.SelectedValue), tipo);
                    
            lblDebitos.Text = debitos.Value.ToString("0.00");
            panDebitos.Visible = debitos.Value > 0;
        }
    
        protected void btnGerarComissao_Click(object sender, EventArgs e)
        {
            string idPedido = String.Empty;
    
            // Pega o id dos Pedidos que estão selecionados
            foreach (GridViewRow r in grdComissao.Rows)
                if (((CheckBox)r.FindControl("chkSel")).Checked)
                    idPedido += ((HiddenField)r.FindControl("hdfIdPedido")).Value + ",";
    
            try
            {
                // Recupera o valor calculado da comissão e o tipo de comissão selecionado
                decimal valorComissao = decimal.Parse(hdfValorComissao.Value);
                Glass.Data.Model.Pedido.TipoComissao tipo = (Glass.Data.Model.Pedido.TipoComissao)Glass.Conversoes.StrParaUint(drpTipo.SelectedValue);
    
                // Gera a comissão
                ComissaoDAO.Instance.GerarComissao(tipo, Glass.Conversoes.StrParaUint(drpNome.SelectedValue), idPedido.TrimEnd(','),
                    ((TextBox)ctrlDataIni.FindControl("txtData")).Text, ((TextBox)ctrlDataFim.FindControl("txtData")).Text,
                    valorComissao, ((TextBox)ctrlDataComissao.FindControl("txtData")).Text);
    
                Glass.MensagemAlerta.ShowMsg("Comissão gerada!", Page);
    
                grdComissao.DataBind();
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao gerar comissão.", ex, Page);
            }
        }
    
        protected void chkSel_DataBinding(object sender, EventArgs e)
        {
            GridViewRow linha = ((CheckBox)sender).Parent.Parent as GridViewRow;
            Glass.Data.Model.Pedido p = linha != null ? linha.DataItem as Glass.Data.Model.Pedido : null;
    
            if (p == null)
                return;
    
            // Só habilita o checkbox se houver comissão a pagar
            ((CheckBox)sender).Enabled = p.ComissaoAPagar;
    
            // Salva alguns atributos ao checkbox para utilização no JavaScript
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
            // Salva o nome do funcionário/comissionado selecionado para
            // tentar manter a seleção em caso de alteração de filtro
            hdfNome.Value = drpNome.SelectedValue;
        }
    
        protected void drpNome_DataBound(object sender, EventArgs e)
        {
            // Verifica se há algum item pré-selecionado
            if (hdfNome.Value != "")
            {
                // Tenta selecionar o mesmo funcionário/comissionado
                drpNome.SelectedIndex = drpNome.Items.IndexOf(drpNome.Items.FindByValue(hdfNome.Value));
    
                // Se não for possível, limpa o HiddenField
                if (drpNome.SelectedValue != hdfNome.Value)
                    hdfNome.Value = "";
            }
        }
    }
}
