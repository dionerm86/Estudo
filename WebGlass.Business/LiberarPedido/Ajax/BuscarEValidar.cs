using System;
using System.Collections.Generic;
using Glass.Data.DAL;

namespace WebGlass.Business.LiberarPedido.Ajax
{
    public interface IBuscarEValidar
    {
        string LiberacaoExiste(string idLiberacao);
        string IsLiberacaoAberta(string idLiberacao);
        string IdsPedidosLiberacoes(string idsLiberacoes);
        string LiberacoesPedidos(string idsLiberacoes, string idsPedidos);
    }

    internal class BuscarEValidar : IBuscarEValidar
    {
        public string LiberacaoExiste(string idLiberacao)
        {
            try
            {
                return LiberarPedidoDAO.Instance.LiberacaoExists(Glass.Conversoes.StrParaUint(idLiberacao)).ToString().ToLower();
            }
            catch
            {
                return "false";
            }
        }

        public string IsLiberacaoAberta(string idLiberacao)
        {
            try
            {
                return LiberarPedidoDAO.Instance.IsLiberacaoAberta(Glass.Conversoes.StrParaUint(idLiberacao)).ToString().ToLower();
            }
            catch
            {
                return "true";
            }
        }

        public string IdsPedidosLiberacoes(string idsLiberacoes)
        {
            return LiberarPedidoDAO.Instance.IdsPedidos(null, idsLiberacoes);
        }

        public string LiberacoesPedidos(string idsLiberacoes, string idsPedidos)
        {
            try
            {
                if (idsPedidos.Length == 0)
                    return "";

                List<string> retorno = new List<string>();

                foreach (string l in idsLiberacoes.Split(','))
                {
                    bool encontrado = false;
                    uint idLib = Glass.Conversoes.StrParaUint(l);

                    foreach (string p in idsPedidos.Split(','))
                    {
                        uint idPed = Glass.Conversoes.StrParaUint(p);
                        List<uint> liberacoes = new List<uint>(LiberarPedidoDAO.Instance.GetIdsLiberacaoByPedido(idPed));

                        if (liberacoes.Contains(idLib))
                        {
                            encontrado = true;
                            break;
                        }
                    }

                    if (encontrado)
                        retorno.Add(l);
                }

                return String.Join(",", retorno.ToArray());
            }
            catch
            {
                return idsLiberacoes;
            }
        }
    }
}
