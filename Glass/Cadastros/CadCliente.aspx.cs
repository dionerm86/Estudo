using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Model;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Colosoft.WebControls;
using Glass.Configuracoes;
using System.Collections.Generic;
using System.Linq;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadCliente : System.Web.UI.Page
    {
        private int _idCliente;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (Request["popup"] != "1")
                dtvCliente.Register("../Listas/LstCliente.aspx");

            odsCliente.Register();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // Se a empresa permitir apenas quem estiver no controle de usuário para incluir/editar cliente
            if (!IsPostBack && !Config.PossuiPermissao(Config.FuncaoMenuCadastro.CadastrarCliente))
                Response.Redirect("../Listas/LstCliente.aspx");

            if (Request["idCli"] != null)
            {
                dtvCliente.ChangeMode(DetailsViewMode.Edit);

                if (dtvCliente.FindControl("btnAlterarSenha") != null)
                    ((Button)dtvCliente.FindControl("btnAlterarSenha")).Attributes.Add
                        ("OnClick", "openWindow(150, 300, '../Utils/TrocarSenha.aspx?IdCli=" + Request["IdCli"] + "'); return false;");

                hdfCNPJ.Value = ClienteDAO.Instance.ObtemCpfCnpj(Glass.Conversoes.StrParaUint(Request["idCli"]));
            }
            else if (!IsPostBack)
            {
                ((TextBox)dtvCliente.FindControl("txtPercSinalMin")).Text = FinanceiroConfig.PercMinimoSinalPedidoPadrao.ToString();
                ((TextBox)dtvCliente.FindControl("txtPercReducaoNfeRevenda")).Text = FinanceiroConfig.PercDescontoRevendaPadrao.ToString();
                ((CheckBox)dtvCliente.FindControl("chkPagamentoAntesProducao")).Checked = FinanceiroConfig.OpcaoPagtoAntecipadoPadraoMarcada;
            }

            if (!IsPostBack)
                hdfTipoUsuario.Value = UserInfo.GetUserInfo.TipoUsuario.ToString();

            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadCliente));
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            if (Request["popup"] != null)
                ClientScript.RegisterClientScriptBlock(this.GetType(), "busca", "closeWindow();", true);
            else
                Response.Redirect("../Listas/LstCliente.aspx");
        }

        protected void odsCli_Inserting(object sender, VirtualObjectDataSourceMethodEventArgs e)
        {
            var cliente = ((Glass.Global.Negocios.Entidades.Cliente)e.InputParameters[0]);
            _idCliente = cliente.IdCli;
        }

        protected void odsCli_Inserted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception == null)
            {
                var resultado = (Glass.Negocios.Global.SalvarClienteResultado)e.ReturnValue;

                ClienteSalvo(resultado);

                // Depois de salvar o cliente e a logo, se tiver, caso o cliente for cadastrado de um popup, retorna o idCliente para a tela que a chamou
                if (Request["popup"] != null)
                {
                    var negCliente = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<Glass.Global.Negocios.IClienteFluxo>();

                    ClientScript.RegisterClientScriptBlock(this.GetType(), "busca", "window.opener.setCliente(" + resultado.IdCliente + ",'" +
                        negCliente.ObtemDescritorCliente(resultado.IdCliente).Name + "', window); closeWindow();", true);
                }
                else
                    Response.Redirect("../Listas/LstCliente.aspx");
            }
        }

        protected void odsCliente_Updating(object sender, VirtualObjectDataSourceMethodEventArgs e)
        {
            var cliente = ((Glass.Global.Negocios.Entidades.Cliente)e.InputParameters[0]);
            _idCliente = cliente.IdCli;
        }

        protected void odsCliente_Updated(object sender, VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
                throw e.Exception;
            else
            {
                // Necessário porque o Insert Cliente retorna um SalvarClienteResultado e o Update um SaveResult
                var resultado = ObterSalvarClienteResultado(e.ReturnValue as Colosoft.Business.SaveResult);
                ClienteSalvo(resultado);
            }
        }

        public void ClienteSalvo(Glass.Negocios.Global.SalvarClienteResultado saveResult)
        {
            if (!saveResult)
                throw new Colosoft.DetailsException(saveResult.Message);

            // Recupera o control de upload
            var imagem = (FileUpload)dtvCliente.FindControl("filLogoCliente");

            if (imagem != null)
            {
                if (imagem.HasFile && imagem.FileName.EndsWith(".png"))
                {
                    var repositorio = Microsoft.Practices.ServiceLocation.ServiceLocator
                        .Current.GetInstance<Glass.Global.Negocios.Entidades.IClienteRepositorioImagens>();

                    // Salva a imagem do cliente
                    repositorio.SalvarImagem(_idCliente, imagem.PostedFile.InputStream);
                }
                else if (!string.IsNullOrEmpty(imagem.FileName))
                {
                    throw new Exception(string.Format("O arquivo ({0}) não pode ser inserido. Verifique se a extensão da imagem é PNG e tente novamente. ", imagem.FileName));
                }
            }
        }

        /// <summary>
        /// Converte o saveResult para SalvarClienteResultado
        /// </summary>
        public Glass.Negocios.Global.SalvarClienteResultado ObterSalvarClienteResultado(Colosoft.Business.SaveResult saveResult)
        {
            if (saveResult)
                return new Glass.Negocios.Global.SalvarClienteResultado(_idCliente);

            return new Glass.Negocios.Global.SalvarClienteResultado(false, saveResult.Message);
        }

        /// <summary>
        /// Método utilizado para manter a situação do cliente inativo no momento do cadastro
        /// </summary>
        protected void drpSituacao_Load(object sender, EventArgs e)
        {
            var tipoFunc = UserInfo.GetUserInfo.TipoUsuario;

            ((DropDownList)sender).Enabled = Config.PossuiPermissao(Config.FuncaoMenuCadastro.AtivarInativarCliente);

            if ((Glass.Configuracoes.ClienteConfig.CadastrarClienteInativo && dtvCliente.CurrentMode == DetailsViewMode.Insert) &&
                !Config.PossuiPermissao(Config.FuncaoMenuFinanceiro.ControleFinanceiroRecebimento))
            {
                ((DropDownList)sender).SelectedValue = SituacaoCliente.Inativo.ToString();
                ((DropDownList)sender).Enabled = false;
            }

            if (!Config.PossuiPermissao(Config.FuncaoMenuCadastro.AtivarInativarCliente))
                ((DropDownList)sender).Enabled = false;
        }

        [Ajax.AjaxMethod()]
        public string CheckIfExists(string cpfCnpj)
        {
            return ClienteDAO.Instance.CheckIfExists(cpfCnpj).ToString().ToLower();
        }

        [Ajax.AjaxMethod()]
        public string ComparaCpfCnpj(string cpfCnpjSalvo, string cpfCnpjNovo)
        {
            if (cpfCnpjSalvo == null || cpfCnpjNovo == null)
                return "false";

            return (cpfCnpjSalvo.Replace(".", "").Replace("-", "").Replace("/", "") ==
                cpfCnpjNovo.Replace(".", "").Replace("-", "").Replace("/", "")).ToString().ToLower();
        }

        protected void txtPercSinalMin_Load(object sender, EventArgs e)
        {
            // Apenas administrador pode alterar percentual mínimo de sinal
            ((TextBox)sender).Enabled = Config.PossuiPermissao(Config.FuncaoMenuCadastro.AlterarSinalMinimoCliente);
        }

        protected void drpFuncionario_DataBound(object sender, EventArgs e)
        {
            if (((DropDownList)sender).SelectedValue == "" && dtvCliente.CurrentMode == DetailsViewMode.Insert && ((DropDownList)sender).Items.FindByValue(UserInfo.GetUserInfo.CodUser.ToString()) != null)
                ((DropDownList)sender).SelectedValue = UserInfo.GetUserInfo.CodUser.ToString();

            // Habilita a alteração de vendedor somente para quem tiver permissão ou é administrador
            ((DropDownList)sender).Enabled = Config.PossuiPermissao(Config.FuncaoMenuCadastro.AlterarVendedorCadCliente);
        }

        protected void chkIgnorarBloqueio_Load(object sender, EventArgs e)
        {
            // Se não houver bloqueio de emissão de pedido caso o cliente não busque pedidos prontos,
            // esconde a opção na tela
            if (PedidoConfig.NumeroDiasPedidoProntoAtrasado == 0 ||
                (Glass.Configuracoes.ClienteConfig.TelaCadastro.ExibirIgnorarBloqueioApenasAdministrador &&  UserInfo.GetUserInfo.TipoUsuario != (uint)Data.Helper.Utils.TipoFuncionario.Administrador) &&
                !Config.PossuiPermissao(Config.FuncaoMenuCadastro.AlterarBloqueioPedidoCadCliente))
                ((WebControl)sender).Style.Add("display", "none");
        }

        protected void lblRota_Load(object sender, EventArgs e)
        {
            if (!RotaDAO.Instance.ExisteRota())
                ((Label)sender).Style.Add("display", "none");
        }

        protected void drpRota_Load(object sender, EventArgs e)
        {
            if (!RotaDAO.Instance.ExisteRota())
                ((DropDownList)sender).Style.Add("display", "none");
        }

        protected void chkBloquearCheques_Load(object sender, EventArgs e)
        {
            if (Glass.Configuracoes.ClienteConfig.TelaCadastro.MarcarBloquearChequesAoInserir && !IsPostBack)
                ((CheckBox)sender).Checked = true;
        }

        protected void txtLimite_Load(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(((TextBox)sender).Text) && ((TextBox)sender).ID == "txtLimite")
                ((TextBox)sender).Text = FinanceiroConfig.LimitePadraoCliente.ToString();

            ((TextBox)sender).Enabled = Config.PossuiPermissao(Config.FuncaoMenuCadastro.AlterarLimiteCliente);
        }

        protected void UrlSistema_Load(object sender, EventArgs e)
        {
            if (!Config.PossuiPermissao(Config.FuncaoMenuPedido.ExportarImportarPedido))
                ((Control)sender).Visible = false;
        }

        protected void chkGerarOrcamento_Load(object sender, EventArgs e)
        {
            ((WebControl)sender).Visible = PCPConfig.GerarOrcamentoFerragensAluminiosPCP;
        }

        protected void drpTabelaDescontoAcrescimo_Load(object sender, EventArgs e)
        {
            // Desabilita a opção de inserir tabela de desconto se for vendedor
            ((DropDownList)sender).Enabled = Config.PossuiPermissao(Config.FuncaoMenuCadastro.AlterarTabelaDescontoAcrescimoCliente);
        }

        protected bool HabilitarCampoCredito()
        {
            return false;
        }

        protected bool ExigirFuncionarioAoInserir()
        {
            return Glass.Configuracoes.ClienteConfig.TelaCadastro.ExigirFuncionarioAoInserir;
        }

        protected bool PermitirCpfCnpjTudo9AoInserir()
        {
            return Glass.Configuracoes.ClienteConfig.TelaCadastro.PermitirCpfCnpjTudo9AoInserir;
        }

        protected bool ExigirEmailClienteAoInserirOuAtualizar()
        {
            return Glass.Configuracoes.ClienteConfig.TelaCadastro.ExigirEmailClienteAoInserirOuAtualizar;
        }

        /// <summary>
        /// Recupera a configuração de bonificação do cliente.
        /// </summary>
        /// <returns>Configuração de bonificação do cliente.</returns>
        protected bool UsarPercentualBonificacaoCliente()
        {
            return Glass.Configuracoes.Liberacao.UsarPercentualBonificacaoCliente;
        }

        protected bool NaoExigirEnderecoConsumidorFinal()
        {
            return Geral.NaoExigirEnderecoConsumidorFinal;
        }

        protected bool ExibirInformacoesFinanceiras()
        {
            return Glass.Configuracoes.ClienteConfig.TelaCadastro.ExibirInformacoesFinanceiras;
        }

        protected string BancosDisponiveis()
        {
            var codigoBancos = new List<string>();

            var valores = Enum.GetValues(typeof(Sync.Utils.CodigoBanco)).Cast<int>().ToList().ToArray();

            foreach (var item in valores)
            {
                codigoBancos.Add(string.Format("\t{0}-{1}\n", item, (Sync.Utils.CodigoBanco)item));
            }

            return string.Format("Só aparecerão neste campo as contas bancárias:\n-Com código de Convênio Cadastrado.\n-Bancos:\n{0}", string.Join(" ", codigoBancos));
        }

        protected string ExibirLimiteCheques()
        {
            return FinanceiroConfig.LimitarChequesPorCpfOuCnpj ? "padding: 2px" : "display: none";
        }

        public bool ExibirPercRedNfe()
        {
            return UserInfo.GetUserInfo.IsAdministrador || Config.PossuiPermissao(Config.FuncaoMenuCadastro.AlterarPercRedNfe);
        }

        protected string ExibirPercentualComissao()
        {
            return PedidoConfig.Comissao.PerComissaoPedido ? "" : "display: none";
        }

        protected string ExibirEstoqueClientes()
        {
            return EstoqueConfig.ControlarEstoqueVidrosClientes ? "" : "display: none";
        }

        protected string ExibirNaoEnviarEmailLiberacao()
        {
            return PedidoConfig.LiberarPedido ? "" : "display: none";
        }

        protected string UsarControleOrdemCarga()
        {
            return OrdemCargaConfig.UsarControleOrdemCarga ? "" : "display: none";
        }

        protected bool PercReducaoNfeVisible()
        {
            return Config.PossuiPermissao(Config.FuncaoMenuCadastro.AlterarPercRedNfe);
        }

        protected string ControlarPedidosImportados()
        {
            return OrdemCargaConfig.ControlarPedidosImportados ? "" : "display: none";
        }

        protected void drpContaBanco_DataBinding(object sender, EventArgs e)
        {
            int idCliente = Glass.Conversoes.StrParaInt(Request["idCli"]);
            int idContaBanco = ClienteDAO.Instance.ObtemIdContaBanco(null, idCliente);

            // Se a conta bancária deste cliente estiver inativa, inclui a mesmo na listagem para não ocorrer erro
            if (!new List<ContaBanco>(ContaBancoDAO.Instance.GetAll()).Any(f => f.IdContaBanco == idContaBanco))
            {
                string contaBanco = ContaBancoDAO.Instance.GetDescricao((uint)idContaBanco);
                ((DropDownList)sender).Items.Add(new ListItem(contaBanco, idContaBanco.ToString()));
            }
        }
    }
}
