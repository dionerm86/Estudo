using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Helper;
using Glass.Data.Model;
using Glass.Data.DAL;
using Glass.Configuracoes;

namespace Glass.UI.Web.Listas
{
    public partial class LstCliente : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            grdCli.Register();
            odsCliente.Register();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Listas.LstCliente));

            if (!IsPostBack)
            {
                bool permissaoCadCliente = Config.PossuiPermissao(Config.FuncaoMenuCadastro.CadastrarCliente);
                bool exibirTotalComprado = Config.PossuiPermissao(Config.FuncaoMenuCadastro.ExibirTotalCompradoCliente);
    
                if (!permissaoCadCliente || !exibirTotalComprado)
                {
                    // Caso o funcionário não tenha permissão de cadastro de cliente ele não pode imprimir a listagem de clientes e também não consegue visualizar a situação atual do cliente.
                    if (!permissaoCadCliente)
                        grdCli.Columns[7].Visible = false;

                    // Caso o funcionário não possa visualizar o total comprado do cliente a coluna na listagem de registros é escondida.
                    if (!exibirTotalComprado)
                        grdCli.Columns[10].Visible = false;
                }

                grdCli.Columns[0].Visible = permissaoCadCliente;
                lnkInserir.Visible = permissaoCadCliente;
                
                // Esconde a impressão da ficha do cliente.
                bool imprimir = Config.PossuiPermissao(Config.FuncaoMenuCadastro.ExportarImprimirDadosClientes);
                grdCli.Columns[13].Visible = imprimir;
                lnkImprimirFicha.Visible = imprimir;
                lnkExportarFicha.Visible = imprimir;
                lnkImprimir.Visible = imprimir;
                lnkExportarExcel.Visible = imprimir;

                if (!string.IsNullOrEmpty(hdfCidade.Value) && Glass.Conversoes.StrParaUint(hdfCidade.Value) > 0)
                    txtCidade.Text = CidadeDAO.Instance.GetNome(Glass.Conversoes.StrParaUint(hdfCidade.Value)).ToUpper();
    
                if (!Geral.UsarTabelasDescontoAcrescimoCliente)
                    tbDescAcresc.Visible = false;
            }
            
            if (Session["pgIndCliente"] != null)
                grdCli.PageIndex = Glass.Conversoes.StrParaInt(Session["pgIndCliente"].ToString());
        }
    
        protected void lnkInserir_Click(object sender, EventArgs e)
        {
            Response.Redirect("../Cadastros/CadCliente.aspx");
        }
    
        #region Métodos AJAX
    
        /// <summary>
        /// Busca o cliente em tempo real
        /// </summary>
        /// <param name="idCli"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public string GetCli(string idCli)
        {
            if (!ClienteDAO.Instance.Exists(Glass.Conversoes.StrParaUint(idCli)))
                return "Erro;Cliente não encontrado.";
            else
            {
                var nomeUtilizar = string.Empty; 

                var nomeFantasia = ClienteDAO.Instance.GetNomeFantasia(null, Glass.Conversoes.StrParaUint(idCli));
                var nome = ClienteDAO.Instance.GetNome(Glass.Conversoes.StrParaUint(idCli));

                var nomeConsiderar = Configuracoes.Liberacao.RelatorioLiberacaoPedido.TipoNomeExibirRelatorioPedido ==
                    Data.Helper.DataSources.TipoNomeExibirRelatorioPedido.NomeFantasia ?
                    nomeFantasia ?? nome :
                    nome ?? nomeFantasia;

                return "Ok;" + nomeConsiderar.ToUpper();
            }
        }
    
        #endregion
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            lblStatus.Text = "";
            grdCli.PageIndex = 0;
        }
    
        protected void grdCli_PageIndexChanged(object sender, EventArgs e)
        {
            if (grdCli.PageIndex >= 0)
            {
                if (Session["pgIndCliente"] == null)
                    Session.Add("pgIndCliente", grdCli.PageIndex.ToString());
                else
                    Session["pgIndCliente"] = grdCli.PageIndex.ToString();
            }
        }
    
        protected void grdCli_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "Inativar")
                {
                    uint idCli = Glass.Conversoes.StrParaUint(e.CommandArgument.ToString());
                    ClienteDAO.Instance.AlteraSituacao(idCli);
                    grdCli.DataBind();
                }
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("", ex, Page);
            }
        }
    
        protected void grdCli_DataBound(object sender, EventArgs e)
        {
            lnkAlterarVendedor.Visible = UserInfo.GetUserInfo.TipoUsuario == (uint)Data.Helper.Utils.TipoFuncionario.Administrador && 
                grdCli.Rows.Count > 0;

            lnkAtivarTodos.Visible = ctrlDataInIni.DataNullable.HasValue ||
                ctrlDataInFim.DataNullable.HasValue;
        }
    
        protected void lnkAtivarTodos_Click(object sender, EventArgs e)
        {
            odsAtivarClientesInativos.Update();

            /* var codCliente = txtNumCli.Text;
            var nome = txtNome.Text;
            var codRota = txtRota.Text;
            var idLoja = String.IsNullOrEmpty(drpLoja.SelectedValue) ? 0 : Glass.Conversoes.StrParaUint(drpLoja.SelectedValue);
            var idFunc = String.IsNullOrEmpty(drpFuncionario.SelectedValue) ? 0 : Glass.Conversoes.StrParaUint(drpFuncionario.SelectedValue);
            var endereco = txtEndereco.Text;
            var bairro = txtBairro.Text;
            var telefone = txtTelefone.Text;
            var cpfCnpj = txtCnpj.Text;
            var dataCadIni = ctrlDataCadIni.DataString;
            var dataCadFim = ctrlDataCadFim.DataString;
            var dataSemCompraIni = ctrlDataIni.DataString;
            var dataSemCompraFim = ctrlDataFim.DataString;
            var dataInativadoIni = ctrlDataInIni.DataString;
            var dataInativadoFim = ctrlDataInFim.DataString;
            var idCidade = String.IsNullOrEmpty(hdfCidade.Value) ? 0 : Glass.Conversoes.StrParaUint(hdfCidade.Value);
            var idTipoCliente = String.IsNullOrEmpty(drpTipoCliente.SelectedValue) ? 0 : Glass.Conversoes.StrParaUint(drpTipoCliente.SelectedValue);
            var tipoFiscal = cblTipoFiscal.SelectedValue;
            var idTabelaDesconto = String.IsNullOrEmpty(drpTabelaDescontoAcrescimo.SelectedValue) ? 0 : Glass.Conversoes.StrParaUint(drpTabelaDescontoAcrescimo.SelectedValue);
            var apenasSemRota = chkApenasSemRota.Checked;
    
            //Pega todos os clientes inativos baseados no filtro da página
            var cliInativos = ClienteDAO.Instance.GetFilter(codCliente, nome, codRota, idLoja, idFunc, endereco, bairro, telefone, cpfCnpj, 2, apenasSemRota, dataCadIni, dataCadFim,
                dataSemCompraIni, dataSemCompraFim, dataInativadoIni, dataInativadoFim, idTipoCliente, tipoFiscal, idCidade, idTabelaDesconto, null, 0, 0);
    
            if (cliInativos.Count < 1)
                return;
    
            int updateCount = 0;
    
            foreach (Cliente c in cliInativos)
            {
                try
                {
                    c.Situacao = 1;
                    ClienteDAO.Instance.Update(c);
                    updateCount++;
                }
                catch
                {
                    lblStatus.Text = "Erro ao ativar um ou mais clientes, por favor tente novamente.";
                    lblStatus.ForeColor = System.Drawing.Color.Red;
                    return;
                }
            }
    
            lblStatus.Text = updateCount + " clientes ativados.";
            lblStatus.ForeColor = System.Drawing.Color.Black;
            grdCli.DataBind(); */
        }
    
        protected void drpSituacao_DataBound(object sender, EventArgs e)
        {
            if (!IsPostBack && ClienteConfig.ListarAtivosPadrao)
                drpSituacao.SelectedValue = ((int)SituacaoCliente.Ativo).ToString();
        }
    
        protected void drpTabelaDescontoAcrescimo_Load(object sender, EventArgs e)
        {

        }

        #region Métodos de visibilidade de itens

        protected bool ExcluirVisible()
        {
            return Config.PossuiPermissao(Config.FuncaoMenuCadastro.CadastrarCliente);
        }

        protected bool DescontoVisible(object dataItem)
        {
            var cliente = (Glass.Global.Negocios.Entidades.ClientePesquisa)dataItem;

            return Config.PossuiPermissao(Config.FuncaoMenuCadastro.DescontoAcrescimoProdutoCliente) &&
                   (!Geral.UsarTabelasDescontoAcrescimoCliente || cliente.IdTabelaDesconto.GetValueOrDefault() == 0);
        }

        protected bool FotosVisible()
        {
            return Config.PossuiPermissao(Config.FuncaoMenuCadastro.AnexarArquivosCliente);
        }

        protected bool SugestoesVisible()
        {
            return Config.PossuiPermissao(Config.FuncaoMenuCadastro.CadastrarSugestoesClientes);
        }

        protected bool InativarVisible(object dataItem)
        {
            var cliente = (Glass.Global.Negocios.Entidades.ClientePesquisa)dataItem;

            return Config.PossuiPermissao(Config.FuncaoMenuCadastro.AtivarInativarCliente) && cliente.Situacao != SituacaoCliente.Cancelado; 
        }

        protected bool ExibirPrecoTabelaCliente()
        {
            return PedidoConfig.RelatorioPedido.RelatorioPrecoTabelaClientes;
        }

        #endregion

        #region Métodos de formatação de dados

        protected string Nome(object dataItem)
        {
            var cliente = (Glass.Global.Negocios.Entidades.ClientePesquisa)dataItem;

            return cliente.IdNome;
        }

        protected string DataCadastro(object dataItem)
        {
            var cliente = (Glass.Global.Negocios.Entidades.ClientePesquisa)dataItem;

            return "Data de cadastro: " + cliente.DataCad.ToShortDateString();
        }

        protected string NomeUsuarioCadastro(object dataItem)
        {
            var cliente = (Glass.Global.Negocios.Entidades.ClientePesquisa)dataItem;

            return "Usuário que cadastrou: " + cliente.DescrUsuCad;
        }

        protected string DataAlteracao(object dataItem)
        {
            var cliente = (Glass.Global.Negocios.Entidades.ClientePesquisa)dataItem;

            return "Data de alteração: " + (cliente.DataAlt.HasValue ? 
                                            cliente.DataAlt.Value.ToShortDateString() : 
                                            String.Empty);
        }

        protected string NomeUsuarioAlteracao(object dataItem)
        {
            var cliente = (Glass.Global.Negocios.Entidades.ClientePesquisa)dataItem;

            return "Usuário que alterou: " + cliente.DescrUsuAlt;
        }

        protected string ERevenda(object dataItem)
        {
            var cliente = (Glass.Global.Negocios.Entidades.ClientePesquisa)dataItem;

            return cliente.Revenda ? "Cliente é revenda" : "Cliente não é revenda";
        }

        #endregion

        protected void btnAlterarVendedorCliente_Click(object sender, EventArgs e)
        {
            /* Chamado 17753. */
            Colosoft.Business.SaveResult result = null;
            odsClienteAtualizarVendedor.Updated += (sender1, e1) =>
                {
                    result = e1.ReturnValue as Colosoft.Business.SaveResult;
                };
            odsClienteAtualizarVendedor.Update();

            if (!result.Success)
            {
                var mensagemErro = "Falha ao alterar o vendedor dos clientes.\n\n";

                mensagemErro += result.Message.ToString().Replace(";", "\n\n");

                MensagemAlerta.ShowMsg(mensagemErro, Page);
            }
            else
                MensagemAlerta.ShowMsg("Clientes alterados com sucesso.", Page);
        }

        protected void btnAlterarRotaCliente_Click(object sender, EventArgs e)
        {
            Colosoft.Business.SaveResult result = null;
            odsClienteAlterarRota.Updated += (sender1, e1) =>
            {
                result = e1.ReturnValue as Colosoft.Business.SaveResult;
            };
            odsClienteAlterarRota.Update();

            if (!result.Success)
            {
                var mensagemErro = "Falha ao alterar a rota dos clientes.\n\n";
                mensagemErro += result.Message.ToString().Replace(";", "\n\n");
                MensagemAlerta.ShowMsg(mensagemErro, Page);
            }
            else
                MensagemAlerta.ShowMsg("Clientes alterados com sucesso.", Page);
        }
    }
}
