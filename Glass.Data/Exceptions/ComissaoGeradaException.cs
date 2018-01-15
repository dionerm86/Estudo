using System;
using Glass.Configuracoes;

namespace Glass.Data.Exceptions
{
    public class ComissaoGeradaException : Exception
    {
        public ComissaoGeradaException(string idsComissoes, bool forJavascript)
            : base(GetMessage(idsComissoes, false, forJavascript))
        {
        }

        private static string GetMessage(string idsComissoes, bool exibirComissoes, bool forJavascript)
        {
            bool comissaoComissionado = PedidoConfig.Comissao.ComissaoPedido;
            bool comissaoInstalador = Geral.ControleInstalacao;

            string exibirIdsComissoes = !exibirComissoes ? "" :
                " (comiss" + (idsComissoes.Split(',').Length > 1 ? "ões " : "ão ") + idsComissoes + ")";

            string mensagem = "Há uma ou mais comissões geradas para esse pedido" + exibirIdsComissoes + ".\n" +
                "Você deseja gerar débito para o funcionário" + (comissaoComissionado && comissaoInstalador ? ", " :
                comissaoComissionado || comissaoInstalador ? " e " : "") + (comissaoComissionado ? "comissionado" : "") +
                (comissaoComissionado && comissaoInstalador ? " e " : "") + (comissaoInstalador ? "instalador" : "") +
                " desse pedido (se já houver sido gerada comissão para ele" + (comissaoComissionado || comissaoInstalador ? "s" : "") + ")?\n" +
                "Se o débito não for gerado não será possível cancelar o pedido.";

            if (forJavascript)
                mensagem = mensagem.Replace("\n", "\\n");

            return mensagem;
        }
    }
}