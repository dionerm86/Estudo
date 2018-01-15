using System;
using Glass.Data.DAL;
using Glass.Data.Model;
using System.Text;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadRetificarOrdemInst : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadRetificarOrdemInst));
        }
    
        /// <summary>
        /// Busca instalações de uma Ordem de Instalação
        /// </summary>
        /// <param name="idOrdemInst"></param>
        /// <param name="noCache"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public string GetOrdemInst(string idOrdemInst, string noCache)
        {
            try
            {
                // Verifica se a Ordem de Instalação existe
                if (!InstalacaoDAO.Instance.OrdemInstExists(Glass.Conversoes.StrParaUint(idOrdemInst)))
                    return "Erro\tEsta Ordem de Instalação não existe.";
    
                // Busca instalações desta Ordem de Instalação
                var lstInst = InstalacaoDAO.Instance.GetByOrdemInst(Glass.Conversoes.StrParaUint(idOrdemInst));
    
                if (lstInst.Count == 0)
                    return "Erro\tNão há nenhuma Instalação em andamento associada à esta Ordem de Instalação.";
    
                // Recupera os ids das equipes da ordem de instalação
                string equipes = "";
                foreach (EquipeInstalacao ei in EquipeInstalacaoDAO.Instance.GetByOrdemInstalacao(Glass.Conversoes.StrParaUint(idOrdemInst)))
                    equipes += ei.IdEquipe + ",";
    
                StringBuilder str = new StringBuilder();
    
                // Salva dados da ordem de instalação
                str.Append(equipes.TrimEnd(',') + ";");
                str.Append(lstInst[0].DataInstalacao != null ? lstInst[0].DataInstalacao.Value.ToString("dd/MM/yyyy") : String.Empty);
                str.Append("\t");
    
                foreach (var inst in lstInst)
                {
                    str.Append(inst.IdInstalacao + ";");
                    str.Append(inst.IdPedido + ";");
                    str.Append(inst.NomeCliente.Replace("|", "").Replace(";", "") + ";");
                    str.Append(inst.DescrTipoInstalacao.Replace("|", "").Replace(";", "") + ";");
                    str.Append(inst.NomeLoja.Replace("|", "").Replace(";", "") + ";");
                    str.Append(inst.LocalObra.Replace("|", "").Replace(";", "") + ";");
                    str.Append(inst.DataConfPedido != null ? inst.DataConfPedido.Value.ToString("dd/MM/yy") : String.Empty);
                    str.Append('|');
                }
    
                return "ok\t" + str.ToString().TrimEnd('|');
            }
            catch (Exception ex)
            {
                return Glass.MensagemAlerta.FormatErrorMsg("Erro\t", ex);
            }
        }
    
        /// <summary>
        /// Busca instalação pelo idPedido informado diretamente
        /// </summary>
        /// <param name="idPedido"></param>
        /// <param name="noCache"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public string GetInstByPedido(string idPedido, string noCache)
        {
            try
            {
                var lstInst = InstalacaoDAO.Instance.GetAbertasByPedido(Glass.Conversoes.StrParaUint(idPedido));
    
                if (lstInst.Count == 0)
                    return "Erro\tNão há nenhuma Instalação Aberta ou Cancelada para este Pedido.";
    
                StringBuilder str = new StringBuilder();
    
                foreach (var inst in lstInst)
                {
                    str.Append(inst.IdInstalacao + ";");
                    str.Append(inst.IdPedido + ";");
                    str.Append(inst.NomeCliente.Replace("|", "").Replace(";", "") + ";");
                    str.Append(inst.DescrTipoInstalacao.Replace("|", "").Replace(";", "") + ";");
                    str.Append(inst.NomeLoja.Replace("|", "").Replace(";", "") + ";");
                    str.Append(inst.LocalObra.Replace("|", "").Replace(";", "") + ";");
                    str.Append(inst.DataConfPedido != null ? inst.DataConfPedido.Value.ToString("dd/MM/yy") : String.Empty);
                    str.Append('|');
                }
    
                return "ok\t" + str.ToString().TrimEnd('|');
            }
            catch (Exception ex)
            {
                return Glass.MensagemAlerta.FormatErrorMsg("Erro\t", ex);
            }
        }
    
        [Ajax.AjaxMethod()]
        public string Retificar(string idOrdemInst, string idsInstalacao, string idsEquipes, string dataInstalacao, string noCache)
        {
            try
            {
                InstalacaoDAO.Instance.RetificarOrdemInst(Glass.Conversoes.StrParaUint(idOrdemInst), idsInstalacao, idsEquipes, DateTime.Parse(dataInstalacao));
                return "ok\tok";
            }
            catch (Exception ex)
            {
                return Glass.MensagemAlerta.FormatErrorMsg("Erro\tFalha ao gerar Ordem de Instalação.", ex);
            }
        }
    }
}
