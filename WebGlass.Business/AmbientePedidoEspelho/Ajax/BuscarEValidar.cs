using System;
using Glass.Data.DAL;

namespace WebGlass.Business.AmbientePedidoEspelho.Ajax
{
    public interface IBuscarEValidar
    {
        string GetAmbientesCompraPcp(string idPedidoStr);
        string GetAmbientesOrdemInst(string idPedido, string linhaAmbiente);
    }

    internal class BuscarEValidar : IBuscarEValidar
    {
        public string GetAmbientesCompraPcp(string idPedidoStr)
        {
            try
            {
                uint idPedido = Glass.Conversoes.StrParaUint(idPedidoStr);
                string ambientes = "";

                foreach (var a in AmbientePedidoEspelhoDAO.Instance.GetByPedido(idPedido))
                    ambientes += a.IdAmbientePedido + "^" + a.Ambiente + "~";

                return "Ok~" + ambientes.TrimEnd('~');
            }
            catch (Exception ex)
            {
                return "Erro~" + Glass.MensagemAlerta.FormatErrorMsg("Falha ao recuperar ambientes.", ex);
            }
        }

        public string GetAmbientesOrdemInst(string idPedido, string linhaAmbiente)
        {
            try
            {
                string retorno = "";
                string esconder = "";

                if (Glass.Configuracoes.Geral.SistemaLite)
                {
                    foreach (var a in AmbientePedidoDAO.Instance.GetByPedido(Glass.Conversoes.StrParaUint(idPedido)))
                    {
                        string funcaoJavaScript = "exibirProdutosAmbiente({0}, " + a.IdAmbientePedido + ", " + linhaAmbiente + ")";

                        var produtos = ProdutosPedidoDAO.Instance.GetByAmbienteInstalacao(a.IdAmbientePedido);
                        bool temProdutos = false;
                        string tabelaProdutos = "";

                        if (produtos.Count > 0)
                        {
                            tabelaProdutos = "<table id='produtosAmbiente_" + a.IdAmbientePedido + "_" + linhaAmbiente + "' cellspacing='6' cellpadding='0' " +
                                "style='margin-left: 32px'><tr><th></th><th>Cód.</th><th>Produto</th><th>Qtd.</th><th>Qtd. Inst.</th><th>Largura</th><th>Altura</th></tr>";

                            foreach (var p in produtos)
                            {
                                if ((p.Qtde - (float)p.QtdeInstalada) == 0)
                                    continue;

                                temProdutos = true;
                                tabelaProdutos += "<tr><td><input type='checkbox' value='" + p.IdProdPed + "' checked='checked'></td><td>" + p.CodInterno + "</td><td>" +
                                    p.DescrProduto + " " + p.DescrBeneficiamentos + "</td><td>" + p.Qtde + (PedidoDAO.Instance.IsMaoDeObra(p.IdPedido) ? " x " +
                                    p.QtdeAmbiente + " peça(s) de vidro" : "") + "</td><td>" + p.QtdeInstalada + "</td><td>" + p.Largura + "</td><td>" + p.AlturaLista + "</td></tr>";
                            }

                            tabelaProdutos += "</table>";
                        }

                        string opcoesCheckbox = temProdutos ? " checked='checked'" : " disabled='true'";
                        retorno += "<tr><td><input type='checkbox' value='" + a.IdAmbientePedido + "' id='ambientePed_" + a.IdAmbientePedido + "'" + opcoesCheckbox + " " +
                            "isAmbiente='true' onclick='" + String.Format(funcaoJavaScript, "this.checked") + "'>" +
                            "<label for='ambientePed_" + a.IdAmbientePedido + "'>" + a.Ambiente + "</label>" + tabelaProdutos + "</td></tr>";

                        if (!temProdutos)
                            esconder += String.Format(funcaoJavaScript, "false") + "; UnTip(); ";
                    }
                }
                else
                {

                    foreach (var a in AmbientePedidoEspelhoDAO.Instance.GetByPedido(Glass.Conversoes.StrParaUint(idPedido)))
                    {
                        string funcaoJavaScript = "exibirProdutosAmbiente({0}, " + a.IdAmbientePedido + ", " + linhaAmbiente + ")";

                        var produtos = ProdutosPedidoEspelhoDAO.Instance.GetByAmbienteInstalacao(a.IdAmbientePedido);
                        bool temProdutos = false;
                        string tabelaProdutos = "";

                        if (produtos.Count > 0)
                        {
                            tabelaProdutos = "<table id='produtosAmbiente_" + a.IdAmbientePedido + "_" + linhaAmbiente + "' cellspacing='6' cellpadding='0' " +
                                "style='margin-left: 32px'><tr><th></th><th>Cód.</th><th>Produto</th><th>Qtd.</th><th>Qtd. Inst.</th><th>Largura</th><th>Altura</th></tr>";

                            foreach (var p in produtos)
                            {
                                uint id = ProdutosPedidoDAO.Instance.GetIdProdPedByProdPedEsp(p.IdProdPed);

                                var pp = ProdutosPedidoDAO.Instance.GetElementInst(id);
                                if ((pp.Qtde - (float)pp.QtdeInstalada) == 0)
                                    continue;

                                temProdutos = true;
                                tabelaProdutos += "<tr><td><input type='checkbox' value='" + id + "' checked='checked'></td><td>" + p.CodInterno + "</td><td>" +
                                    p.DescrProduto + " " + p.DescrBeneficiamentos + "</td><td>" + p.Qtde + (PedidoDAO.Instance.IsMaoDeObra(p.IdPedido) ? " x " +
                                    p.QtdeAmbiente + " peça(s) de vidro" : "") + "</td><td>" + pp.QtdeInstalada + "</td><td>" + p.Largura + "</td><td>" + p.AlturaLista + "</td></tr>";
                            }

                            tabelaProdutos += "</table>";
                        }

                        string opcoesCheckbox = temProdutos ? " checked='checked'" : " disabled='true'";
                        retorno += "<tr><td><input type='checkbox' value='" + a.IdAmbientePedido + "' id='ambientePed_" + a.IdAmbientePedido + "'" + opcoesCheckbox + " " +
                            "isAmbiente='true' onclick='" + String.Format(funcaoJavaScript, "this.checked") + "'>" +
                            "<label for='ambientePed_" + a.IdAmbientePedido + "'>" + a.Ambiente + "</label>" + tabelaProdutos + "</td></tr>";

                        if (!temProdutos)
                            esconder += String.Format(funcaoJavaScript, "false") + "; UnTip(); ";
                    }
                }

                return "<table>" + retorno + "</table>¬" + esconder;
            }
            catch
            {
                return "";
            }
        }
    }
}
