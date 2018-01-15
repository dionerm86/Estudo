using System;
using System.Collections.Generic;
using Glass.Data.Helper;
using Glass.Data.DAL;
using Glass.Configuracoes;

namespace Glass.UI.Web.Utils
{
    public partial class SetMotivoCancEtiqueta : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Utils.SetMotivoCancEtiqueta));
    
            uint idImpressao = Glass.Conversoes.StrParaUint(Request["idImpressao"]);
            int tipo = Glass.Conversoes.StrParaInt(Request["tipo"]);
    
            if (tipo == (int)ProdutoImpressaoDAO.TipoEtiqueta.NotaFiscal)
            {
                dadosCanc.Rows[0].Visible = false;
                dadosCanc.Rows[1].Visible = true;
                txtNumeroPedido.Text = "0";
                lblInfoCanc.Visible = false;
                lblInfoCancNFe.Visible = true;
            }
            else if (tipo == (int)ProdutoImpressaoDAO.TipoEtiqueta.Box)
            {
                lblInfoCanc.Visible = false;
                txtNumeroPedido.Text = "0";
                txtNumeroNFe.Text = "0";
                dadosCanc.Rows[0].Visible = false;
                dadosCanc.Rows[1].Visible = false;
                dadosCanc.Rows[2].Visible = false;
            }
            else
            {
                dadosCanc.Rows[1].Visible = false;
                txtNumeroNFe.Text = "0";
            }
    
            dadosCanc.Rows[2].Visible = PCPConfig.Etiqueta.UsarPlanoCorte && dadosCanc.Rows[0].Visible;
        }
    
        [Ajax.AjaxMethod()]
        public string CancelarImpressao(string idImpressao, string planoCorte, string idPedidoParam, string numeroNFe, string motivo)
        {
            try
            {
                uint? idPedido = Glass.Conversoes.StrParaUintNullable(idPedidoParam);
    
                // O idPedido convertido deve ser igual ao idPedido do par�metro, o motivo de colocar essa valida��o � que alguns
                // usu�rios estavam digitando o n�mero da etiqueta, como o m�todo retorna 0 quando n�o consegue converter, todos os
                // pedidos da impress�o estavam sendo cancelados
                if (!String.IsNullOrEmpty(idPedidoParam) && idPedidoParam != idPedido.ToString())
                    return "Erro|Pedido inv�lido";
    
                ImpressaoEtiquetaDAO.Instance.CancelarImpressaoComTransacao(UserInfo.GetUserInfo.CodUser, idImpressao.StrParaUint(),
                    idPedido, numeroNFe.StrParaUintNullable(), planoCorte, 0, motivo, true);
    
                return "Ok";
            }
            catch (Exception ex)
            {
                return "Erro|" + Glass.MensagemAlerta.FormatErrorMsg(null, ex);
            }
        }
    
        /// <summary>
        /// Verifica se a impress�o a ser cancelada ja possuiu algum 
        /// pedido liberado.
        /// </summary>
        /// <param name="idImpressao"></param>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public string VerificaLiberacao(string idImpressao, string idPedido)
        {
            try
            {
                string[] idsPedidos;
                List<string> idsPedidosLiberacao = new List<string>();

                if (idPedido == "0")
                {
                    idsPedidos = PedidoDAO.Instance.GetIdsByImpressao(Glass.Conversoes.StrParaUint(idImpressao)).Split(',');
                    idsPedidos = idsPedidos != null && idsPedidos.Length > 0 && idsPedidos[0] == null ? null : idsPedidos;
                }
                else
                    idsPedidos = new string[] { idPedido };
    
                foreach (string id in idsPedidos)
                    if (LiberarPedidoDAO.Instance.GetIdsLiberacaoAtivaByPedido(Glass.Conversoes.StrParaUint(id)).Count > 0)
                        idsPedidosLiberacao.Add(id);
    
                if (idsPedidosLiberacao.Count > 0)
                {
                    string retorno="";
    
                    if (idsPedidosLiberacao.Count == 1)
                        retorno = "O pedido: " + idsPedidosLiberacao[0] +
                            " possui uma libera��o, voc� deve cancel�-la antes de efetuar o cancelamento da impress�o";
                    else
                    {
                        retorno = "Os pedidos: ";
                        foreach (string id in idsPedidosLiberacao)
                            retorno += id + ", ";
                        retorno = retorno.TrimEnd(' ',',');
                        retorno += " possuem libera��es, voc� deve cancel�-las antes de efetuar o cancelamento da impress�o";
                    }
    
                    return "Erro|" + retorno;
                }
                return "Ok";
            }
            catch (Exception ex)
            {
                return "Erro|" + Glass.MensagemAlerta.FormatErrorMsg(null, ex);
            }
        }
    
        /// <summary>
        /// Verifica se alguma etiqueta da impressa est� marcada
        /// em um setor acima da impress�o
        /// </summary>
        /// <param name="idImpressao"></param>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public string VerificaImpressaoEmProducao(string idImpressao, string idPedido)
        {
            try
            {
                if (idPedido == "0")
                    idPedido = String.Empty;
    
                string[] setores = ImpressaoEtiquetaDAO.Instance.ObtemSetoresProd(Glass.Conversoes.StrParaUint(idImpressao), idPedido).Split(',');
    
                foreach (string idSetor in setores)
                    if (Glass.Conversoes.StrParaUint(idSetor) > 1)
                        return "Erro|Esta impress�o possui pe�as que j� est�o em produ��o\n\n"+
                            "DESEJA REALMENTE EXCLUIR A IMPRESS�O?";
    
                return "Ok";
            }
            catch (Exception ex)
            {
                return "Erro|" + Glass.MensagemAlerta.FormatErrorMsg(null, ex);
            }
        }
    
        /// <summary>
        /// Verifica se alguma etiqueta impressa da NF est� marcada
        /// em um setor acima da impress�o
        /// </summary>
        /// <param name="idImpressao"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public string VerificaImpressaoEmProducaoNF(string idImpressao, string numeroNFe)
        {
            try
            {
                if (ChapaCortePecaDAO.Instance.ImpressaoChapaPossuiLeitura(Glass.Conversoes.StrParaUint(idImpressao), 
                    Glass.Conversoes.StrParaUint(numeroNFe)))
                    return "Erro|Esta impress�o possui pe�as que j� est�o em produ��o";
    
                return "Ok";
            }
            catch (Exception ex)
            {
                return "Erro|" + Glass.MensagemAlerta.FormatErrorMsg(null, ex);
            }
        }
    
        /// <summary>
        /// Verifica se alguma etiqueta da impress�o esta reposta.
        /// </summary>
        /// <param name="idImpressao"></param>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public string VerificaPecaResposta(string idImpressao, string idPedido)
        {
            try
            {
                if (ProdutoPedidoProducaoDAO.Instance.GetCountRepoPeca(Glass.Conversoes.StrParaUint(idImpressao), Glass.Conversoes.StrParaUint(idPedido)) > 0)
                    return "Erro|Esta impress�o possui pe�as que foram geradas reposi��o. Ao cancelar a impress�o a reposi��o ser� desfeita.\n\n" +
                                "DESEJA REALMENTE EXCLUIR A IMPRESS�O?";
    
                return "Ok";
            }
            catch (Exception ex)
            {
                return "Erro|" + Glass.MensagemAlerta.FormatErrorMsg(null, ex);
            }
        }
    }
}
