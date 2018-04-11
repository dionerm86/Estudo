using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Helper;
using Glass.Data.Model;
using Glass.Data.DAL;
using Glass.Configuracoes;
using System.Linq;

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
        }
    
        protected void drpSituacao_DataBound(object sender, EventArgs e)
        {
            if (!IsPostBack && ClienteConfig.ListarAtivosPadrao)
                drpSituacao.SelectedValue = ((int)SituacaoCliente.Ativo).ToString();
        }

        #region Métodos de visibilidade de itens

        protected bool ExcluirVisible()
        {
            return Config.PossuiPermissao(Config.FuncaoMenuCadastro.CadastrarCliente);
        }

        protected bool DescontoVisible(object dataItem)
        {
            var cliente = (Glass.Global.Negocios.Entidades.ClientePesquisa)dataItem;

            return Config.PossuiPermissao(Config.FuncaoMenuCadastro.DescontoAcrescimoProdutoCliente) && cliente.IdTabelaDesconto.GetValueOrDefault() == 0;
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
            // Recupera o funcionário
            var funcionario = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<Glass.Global.Negocios.IFuncionarioFluxo>().ObtemFuncionario((int)UserInfo.GetUserInfo.CodUser);

            // Verificar se ele possui acesso ao menu de preço de tabela
            var possuiMenuPrecoTabela = Microsoft.Practices.ServiceLocation.ServiceLocator.Current
                .GetInstance<Glass.Global.Negocios.IMenuFluxo>()
                .ObterMenusPorFuncionario(funcionario)
                .Any(f => f.Url.ToLower().Contains("listaprecotabcliente"));

            return possuiMenuPrecoTabela;
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
