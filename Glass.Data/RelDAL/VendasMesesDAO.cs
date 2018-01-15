using System.Collections.Generic;
using Glass.Data.RelModel;

namespace Glass.Data.RelDAL
{
    public sealed class VendasMesesDAO : Glass.Pool.PoolableObject<VendasMesesDAO>
    {
        //private VendasMesesDAO() { }

        public VendasMeses[] GetVendasMeses(uint idCliente, string nomeCliente, string idsRota, bool revenda, uint idComissionado, string nomeComissionado, int mesInicio,
            int anoInicio, int mesFim, int anoFim, string tipoMedia, int ordenar, int tipoVendas, string idsFunc, string NomeFunc, string idsFuncAssociaCliente, decimal valorMinimo,
            decimal valorMaximo, uint idLoja, bool lojaCliente, string tipoCliente, int situacaoCliente, bool incluirDadosTotalM2eTotalItens)
        {
            var vendas = VendasDAO.Instance.GetList(idCliente, nomeCliente, idsRota, revenda, idComissionado, nomeComissionado, 
                mesInicio, anoInicio, mesFim, anoFim, ordenar, tipoMedia, tipoVendas, idsFunc, NomeFunc, 
                idsFuncAssociaCliente, valorMinimo, valorMaximo, situacaoCliente, 0, idLoja, lojaCliente, tipoCliente);

            var meses = VendasDAO.Instance.GetMesesVenda(idCliente, nomeCliente, idsRota, revenda, idComissionado, nomeComissionado, mesInicio, anoInicio, mesFim, anoFim,
                tipoMedia, tipoVendas, idsFunc, NomeFunc, idLoja, lojaCliente, tipoCliente, situacaoCliente);

            List<VendasMeses> retorno = new List<VendasMeses>();
            foreach (Vendas v in vendas)
            {
                decimal total = 0;

                foreach (string mes in meses)
                {
                    VendasMeses novo = new VendasMeses();
                    novo.IdCliente = v.IdCliente;
                    novo.IdComissionado = v.IdComissionado;
                    novo.IdNome = tipoVendas == 0 ? v.IdNomeCliente : v.IdNomeComissionado;
                    novo.Mes = mes;
                    novo.Valor = 0;
                    novo.IdFuncionario = v.IdFuncionario;
                    novo.NomeFuncionario = v.NomeFuncionario;
                    novo.IdClienteComissionado = tipoVendas == 0 ? v.IdCliente : v.IdComissionado;
                    novo.Nome = tipoVendas == 0 ? v.NomeCliente : v.NomeComissionado;
                    novo.Total = v.Total;
                    novo.Format = "C";

                    decimal valorMediaIniCli = 0, valorMediaFimCli = 0;

                    if (novo.IdCliente > 0)
                    {
                        valorMediaIniCli = Glass.Data.DAL.ClienteDAO.Instance.ObtemValorCampo<decimal>("valorMediaIni", "id_Cli=" + novo.IdCliente);
                        valorMediaFimCli = Glass.Data.DAL.ClienteDAO.Instance.ObtemValorCampo<decimal>("valorMediaFim", "id_Cli=" + novo.IdCliente);

                        novo.MediaCompraCliente = valorMediaIniCli > 0 || valorMediaFimCli > 0 ? valorMediaIniCli.ToString("N") + " a " + valorMediaFimCli.ToString("N") : "";
                    }

                    for (int i = 0; i < v.MesVenda.Length; i++)
                        if (mes == v.MesVenda[i])
                        {
                            novo.Valor = v.ValorVenda[i];
                            total += novo.Valor;
                            break;
                        }

                    retorno.Add(novo);
                }

                if (incluirDadosTotalM2eTotalItens)
                {
                    VendasMeses novo = new VendasMeses();
                    novo.IdCliente = v.IdCliente;
                    novo.IdComissionado = v.IdComissionado;
                    novo.Mes = "Total";
                    novo.Valor = total;
                    novo.Format = "C";
                    retorno.Add(novo);

                    novo = new VendasMeses();
                    novo.IdCliente = v.IdCliente;
                    novo.IdComissionado = v.IdComissionado;
                    novo.Mes = "Total M²";
                    novo.Valor = (decimal)v.TotM2;
                    novo.Format = "N";
                    retorno.Add(novo);

                    novo = new VendasMeses();
                    novo.IdCliente = v.IdCliente;
                    novo.IdComissionado = v.IdComissionado;
                    novo.Mes = "Total Itens";
                    novo.Valor = (decimal)v.TotalItens;
                    novo.Format = "N";
                    retorno.Add(novo);
                }
            }

            return retorno.ToArray();
        }
    }
}