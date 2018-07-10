using System;
using System.Web.UI;
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
            Ajax.Utility.RegisterTypeForAjax(typeof(LstCliente));

            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(hdfCidade.Value) && Glass.Conversoes.StrParaUint(hdfCidade.Value) > 0)
                    txtCidade.Text = CidadeDAO.Instance.GetNome(Glass.Conversoes.StrParaUint(hdfCidade.Value)).ToUpper();
            }

            //lnkAtivarTodos.Visible = ctrlDataInIni.DataNullable.HasValue ||
            //    ctrlDataInFim.DataNullable.HasValue;
        }

        /// <summary>
        /// Busca o cliente em tempo real.
        /// </summary>
        /// <param name="idCli"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod]
        public string GetCli(string idCli)
        {
            if (!ClienteDAO.Instance.Exists(Conversoes.StrParaUint(idCli)))
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
    
        protected void lnkAtivarTodos_Click(object sender, EventArgs e)
        {
            odsAtivarClientesInativos.Update();
        }
    
        protected void drpSituacao_DataBound(object sender, EventArgs e)
        {
            if (!IsPostBack && ClienteConfig.ListarAtivosPadrao)
                drpSituacao.SelectedValue = ((int)SituacaoCliente.Ativo).ToString();
        }

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
