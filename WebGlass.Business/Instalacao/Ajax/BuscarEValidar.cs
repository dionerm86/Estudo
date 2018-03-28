using System;
using System.Text;
using Glass.Data.DAL;

namespace WebGlass.Business.Instalacao.Ajax
{
    public interface IBuscarEValidar
    {
        string ValidarPedido(string idPedidoStr);
        string VerificarInstalacao(string idPedidoStr, string tipoInstalacaoStr);
        string GetInstByPedido(string idPedidoStr, string noCache);
        string VerificarPedidoJaInstalado(string idPedidoStr);
        string VerificarPedidoJaFinalizadoPCP(string idPedidoStr);
    }

    internal class BuscarEValidar : IBuscarEValidar
    {
        public string ValidarPedido(string idPedidoStr)
        {
            if (String.IsNullOrEmpty(idPedidoStr))
                return "Erro;Digite o número do pedido.";

            uint idPedido = Glass.Conversoes.StrParaUint(idPedidoStr);

            if (!PedidoDAO.Instance.IsPedidoConfirmadoLiberado(idPedido))
                return "Erro;Pedido não está confirmado.";

            return "Ok;";
        }

        public string VerificarInstalacao(string idPedidoStr, string tipoInstalacaoStr)
        {
            uint idPedido = Glass.Conversoes.StrParaUint(idPedidoStr);
            int tipoInstalacao = Glass.Conversoes.StrParaInt(tipoInstalacaoStr);

            return InstalacaoDAO.Instance.ExisteAbertaByPedidoTipo(idPedido, tipoInstalacao).ToString().ToLower();
        }

        public string GetInstByPedido(string idPedidoStr, string noCache)
        {
            try
            {
                uint idPedido = Glass.Conversoes.StrParaUint(idPedidoStr);

                if (Glass.Configuracoes.Geral.ControlePCP && PedidoDAO.Instance.GetTipoPedido(idPedido) != Glass.Data.Model.Pedido.TipoPedidoEnum.Revenda &&
                    (!PedidoEspelhoDAO.Instance.ExisteEspelho(idPedido) || PedidoEspelhoDAO.Instance.ObtemSituacao(idPedido) == Glass.Data.Model.PedidoEspelho.SituacaoPedido.Aberto))
                    return "Erro\tPara gerar uma nova ordem de instalação deste pedido é necessário que o mesmo esteja finalizado no PCP.";

                var lstInst = InstalacaoDAO.Instance.GetAbertasByPedido(idPedido);

                if (lstInst.Count == 0)
                    return "Erro\tNão há nenhuma Instalação Aberta ou Cancelada para este Pedido.";

                StringBuilder str = new StringBuilder();

                foreach (var inst in lstInst)
                {
                    str.Append(inst.IdInstalacao + ";");
                    str.Append(inst.IdPedido + ";");
                    str.Append(inst.NomeCliente.Replace("|", "").Replace(";", "").Replace("\t", "").Replace("\n", "") + ";");
                    str.Append(inst.DescrTipoInstalacao.Replace("|", "").Replace(";", "") + ";");
                    str.Append(inst.NomeLoja.Replace("|", "").Replace(";", "") + ";");
                    str.Append(inst.LocalObra.Replace("|", "").Replace(";", "").Replace("\t", "").Replace("\n", "") + ";");
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

        public string VerificarPedidoJaInstalado(string idPedidoStr)
        {
            uint idPedido = Glass.Conversoes.StrParaUint(idPedidoStr);

            var listProdPed = ProdutosPedidoDAO.Instance.ObtemProdPedInstByPedido(idPedido);
            foreach(var prodPed in listProdPed)
            {
                if ((decimal)prodPed.Qtde > prodPed.QtdeInstalada)
                    return "false";
            }
            return "true";
        }

        public string VerificarPedidoJaFinalizadoPCP(string idPedidoStr)
        {
            uint idPedido = Glass.Conversoes.StrParaUint(idPedidoStr);

            if (Glass.Configuracoes.Geral.ControlePCP && PedidoDAO.Instance.GetTipoPedido(idPedido) != Glass.Data.Model.Pedido.TipoPedidoEnum.Revenda &&
                (!PedidoEspelhoDAO.Instance.ExisteEspelho(idPedido) || PedidoEspelhoDAO.Instance.ObtemSituacao(idPedido) == Glass.Data.Model.PedidoEspelho.SituacaoPedido.Aberto))
                return "Erro\tPara gerar uma nova ordem de instalação deste pedido é necessário que o mesmo esteja finalizado no PCP.";

            return "true";
        }
    }
}
