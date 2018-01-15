using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebGlass.Business.InventarioEstoque.Entidade;

namespace Glass.UI.Web.Listas
{
    public partial class LstInventarioEstoque : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Listas.LstInventarioEstoque));
        }
    
        protected string ObtemDescricaoGrupoSubgrupo(object descricaoGrupo, object descricaoSubgrupo)
        {
            string retorno = String.Empty;
    
            if (descricaoGrupo != null && !String.IsNullOrEmpty(descricaoGrupo.ToString()))
                retorno += descricaoGrupo.ToString();
    
            if (descricaoSubgrupo != null && !String.IsNullOrEmpty(descricaoSubgrupo.ToString()))
                retorno += (retorno != String.Empty ? " / " : "") + descricaoSubgrupo.ToString();
    
            return retorno;
        }
    
        protected bool ExibirEditar(object situacao)
        {
            var situacoes = new List<InventarioEstoque.SituacaoEnum>() {
                InventarioEstoque.SituacaoEnum.Aberto
            };
    
            return situacoes.Contains((InventarioEstoque.SituacaoEnum)situacao);
        }
    
        protected bool ExibirCancelar(object situacao)
        {
            var situacoes = new List<InventarioEstoque.SituacaoEnum>() {
                InventarioEstoque.SituacaoEnum.Aberto,
                InventarioEstoque.SituacaoEnum.EmContagem
            };
    
            return situacoes.Contains((InventarioEstoque.SituacaoEnum)situacao);
        }
    
        protected bool ExibirEmContagem(object situacao)
        {
            return ExibirEditar(situacao);
        }
    
        protected bool ExibirFinalizar(object situacao)
        {
            var situacoes = new List<InventarioEstoque.SituacaoEnum>() {
                InventarioEstoque.SituacaoEnum.EmContagem
            };
    
            return situacoes.Contains((InventarioEstoque.SituacaoEnum)situacao);
        }
    
        protected bool ExibirConfirmar(object situacao)
        {
            var situacoes = new List<InventarioEstoque.SituacaoEnum>() {
                InventarioEstoque.SituacaoEnum.Finalizado
            };
    
            return situacoes.Contains((InventarioEstoque.SituacaoEnum)situacao);
        }
    
        protected bool ExibirRelatorio(object situacao)
        {
            return !ExibirEditar(situacao);
        }
    
        protected void grdInventarioEstoque_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "IniciarContagem")
            {
                try
                {
                    uint codigo = Glass.Conversoes.StrParaUint(e.CommandArgument.ToString());
                    WebGlass.Business.InventarioEstoque.Fluxo.IniciarContagem.Instance.Iniciar(codigo);
                    
                    grdInventarioEstoque.DataBind();
                    Page.ClientScript.RegisterStartupScript(GetType(), "contagem", 
                        String.Format("abrirRelatorio({0});\n", codigo), true);
                }
                catch (Exception ex)
                {
                    Glass.MensagemAlerta.ErrorMsg("Falha ao iniciar contagem.", ex, Page);
                }
            }
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
    
        }
    
        [Ajax.AjaxMethod]
        public string Confirmar(string codigoInventarioEstoque)
        {
            return WebGlass.Business.InventarioEstoque.Fluxo.Confirmar.Ajax.ConfirmarInventario(codigoInventarioEstoque);
        }
    }
}
