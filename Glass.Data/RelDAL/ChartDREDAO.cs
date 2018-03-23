using System;
using System.Collections.Generic;
using Glass.Data.RelModel;
using Glass.Data.DAL;

namespace Glass.Data.RelDAL
{
    public sealed class ChartDREDAO : BaseDAO<ChartDRE, ChartDREDAO>
    {
        //private ChartDREDAO() { }

        public Dictionary<uint, List<ChartDRE>> ObterDados(uint idCategoriaConta, uint idGrupoConta, uint[] idsPlanoConta, string dataIni,
            string dataFim, int tipoMov, int tipoConta, bool ajustado, List<uint> ids)
        {
            DateTime periodoIni = DateTime.Parse(dataIni);
            DateTime periodoFim = DateTime.Parse(dataFim).AddDays(1);

            Dictionary<uint, List<ChartDRE>> dados = new Dictionary<uint, List<ChartDRE>>();

            foreach (uint u in ids)
                dados.Add(u, new List<ChartDRE>());

            int count = 1;

            while (periodoIni < periodoFim)
            {
                foreach (uint id in ids)
                {
                    var planos = Glass.Data.RelDAL.PlanoContasDAO.Instance.GetList(idCategoriaConta, idGrupoConta, idsPlanoConta, id, periodoIni.ToString("dd/MM/yyyy"),
                        periodoIni.AddMonths(1).AddDays(-1).ToString("dd/MM/yyyy"), tipoMov, tipoConta, ajustado, false, 0, "", 0, int.MaxValue);

                    ChartDRE s = new ChartDRE();
                    s.Periodo = periodoIni.ToString("MMM-yy");
                    s.IdLoja = id;
                    s.NomeLoja = LojaDAO.Instance.GetNome(id);
                    decimal saida = 0;
                    decimal entrada = 0;
                    foreach (PlanoContas p in planos)
                    {
                        if(p.TipoMov == 1)
                            entrada += p.Valor;
                        if (p.TipoMov == 2)
                            saida += p.Valor;
                    }

                    s.Total = entrada - saida;

                    dados[id].Add(s);
                }

                periodoIni = periodoIni.AddMonths(+1);
                count++;
            }

            return dados;
        }

        public ChartDREImagem[] ObterChartDREImagem()
        {
            return new ChartDREImagem[0];
        }
    //    public Dictionary<uint, List<ChartVendas>> ObterDados1(uint idLoja, int tipoFunc, uint idVendedor, uint idCliente, string nomeCliente,
    //string dataIni, string dataFim, int agrupar, string tipoAgrupar, List<uint> ids)
    //    {
    //        DateTime periodoIni = DateTime.Parse(dataIni);
    //        DateTime periodoFim = DateTime.Parse(dataFim).AddDays(1);

    //        Dictionary<uint, List<ChartVendas>> dictVendas = new Dictionary<uint, List<ChartVendas>>();

    //        foreach (uint u in ids)
    //            dictVendas.Add(u, new List<ChartVendas>());

    //        int count = 1;
    //        while (periodoIni < periodoFim)
    //        {
    //            foreach (uint u in ids)
    //            {
    //                switch (tipoAgrupar)
    //                {
    //                    case "loja":
    //                        idLoja = u;
    //                        break;
    //                    case "emissor":
    //                        idVendedor = u;
    //                        break;
    //                    case "cliente":
    //                        idCliente = u;
    //                        break;
    //                }

    //                ChartVendas[] serie = GetVendas(idLoja, tipoFunc, idVendedor, idCliente, nomeCliente, periodoIni.ToString("dd/MM/yyyy"),
    //                    periodoIni.AddMonths(1).AddDays(-1).ToString("dd/MM/yyyy"), agrupar);

    //                foreach (ChartVendas s in serie)
    //                {
    //                    s.Periodo = periodoIni.ToString("MMM-yy");
    //                    switch (tipoAgrupar)
    //                    {
    //                        case "nenhum":
    //                            dictVendas[1].Add(s);
    //                            break;
    //                        case "loja":
    //                            dictVendas[s.IdLoja].Add(s);
    //                            break;
    //                        case "emissor":
    //                            if (ids.Contains(s.IdFunc))
    //                                dictVendas[s.IdFunc].Add(s);
    //                            break;
    //                        case "cliente":
    //                            if (ids.Contains(s.IdCliente))
    //                                dictVendas[s.IdCliente].Add(s);
    //                            break;
    //                    }
    //                }

    //                ChartVendas cv;
    //                foreach (KeyValuePair<uint, List<ChartVendas>> entry in dictVendas)
    //                {
    //                    if (entry.Key == u && entry.Value.Count < count)
    //                    {
    //                        cv = new ChartVendas();
    //                        cv.Periodo = periodoIni.ToString("MMM-yy");

    //                        switch (tipoAgrupar)
    //                        {
    //                            case "loja":
    //                                cv.IdLoja = entry.Key;
    //                                cv.NomeLoja = LojaDAO.Instance.GetNome(entry.Key);
    //                                cv.Agrupar = 1;
    //                                break;
    //                            case "emissor":
    //                                cv.IdFunc = entry.Key;
    //                                cv.NomeVendedor = FuncionarioDAO.Instance.GetNome(entry.Key);
    //                                cv.Agrupar = 2;
    //                                break;
    //                            case "cliente":
    //                                cv.IdCliente = entry.Key;
    //                                cv.NomeCliente = ClienteDAO.Instance.GetNome(entry.Key);
    //                                cv.Agrupar = 3;
    //                                break;
    //                        }

    //                        entry.Value.Add(cv);
    //                    }
    //                }
    //            }

    //            periodoIni = periodoIni.AddMonths(+1);
    //            count++;
    //        }

    //        return dictVendas;
    //    }

    }
}
