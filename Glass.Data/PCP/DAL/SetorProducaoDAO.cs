using System;
using System.Collections.Generic;
using Glass.Data.Model;
using Glass.Data.Helper;
using GDA;
using System.Linq;

namespace Glass.Data.DAL
{
    public sealed class SetorProducaoDAO : BaseDAO<SetorProducao, SetorProducaoDAO>
    {
        //private SetorProducaoDAO() { }

        private string Sql(string ids)
        {
            string idsSetores = "";
            string datasSetores = "";
            string nomesSetores = "";
            string funcLeituras = "";
            string nomesFunc = "";
            string setoresCorte = "";
            string setorCorte = "";
            string setoresRoteiro = "";
            string setorRoteiro = "";

            int numeroSetor = 0;
            for (int i = 0; i < Utils.GetSetores.Length; i++)
            {
                if (!Utils.GetSetores[i].ExibirRelatorio)
                    continue;

                idsSetores += Utils.GetSetores[i].IdSetor + ",";

                datasSetores += "if(lp.idSetor=" + Utils.GetSetores[i].IdSetor + ", dataLeitura, null) as setor" + 
                    numeroSetor + ", ";
                
                funcLeituras += "if(lp.idSetor=" + Utils.GetSetores[i].IdSetor + " and dataLeitura is not null, " +
                    "f.nome, null) as func" + numeroSetor + ", ";
                
                setorCorte += "if(lp.idSetor=" + Utils.GetSetores[i].IdSetor + ", " + (Utils.GetSetores[i].Corte || Utils.GetSetores[i].Laminado) + 
                    ", null) as setorCorte" + numeroSetor + ", ";

                setorRoteiro += "if(lp.idSetor=" + Utils.GetSetores[i].IdSetor + ", rpe.idProdPedProducao is not null, null) as setorRoteiro" + numeroSetor + ", ";

                nomesSetores += "cast(group_concat(setor" + numeroSetor + ") as datetime) as setor" + numeroSetor + ", ";
                nomesFunc += "cast(group_concat(func" + numeroSetor + ") as char) as func" + numeroSetor + ", ";
                setoresCorte += "cast(group_concat(setorCorte" + numeroSetor + ") as char)='1' as setorCorte" + numeroSetor + ", ";
                setoresRoteiro += "cast(group_concat(setorRoteiro" + numeroSetor + ") as char)='1' as setorRoteiro" + numeroSetor + ", ";

                numeroSetor++;
            }

            string sql = @"
                select idProdPedProducao, '" + idsSetores.TrimEnd(',') + "' as idsSetores, " + nomesSetores.TrimEnd(',', ' ') + ", " +
                    nomesFunc.TrimEnd(',', ' ') + ", " + setoresCorte.TrimEnd(',', ' ') + ", " + setoresRoteiro.TrimEnd(',', ' ') + @"
                from (
                    select lp.idProdPedProducao, " + datasSetores.TrimEnd(',', ' ') + ", " + funcLeituras.TrimEnd(',', ' ') + ", " +
                        setorCorte.TrimEnd(',', ' ') + ", " + setorRoteiro.TrimEnd(',', ' ') + @"
                    from (
                        select s.idProdPedProducao, s.idSetor, lp.idFuncLeitura, lp.dataLeitura
                        from (
                            select distinct s.idSetor, ppp.idProdPedProducao
                            from setor s, produto_pedido_producao ppp
                            where ppp.idProdPedProducao in (" + ids + @")
                        ) s
                            left join leitura_producao lp on (s.idSetor=lp.idSetor and s.idProdPedProducao=lp.idProdPedProducao)
	                ) lp
                left join funcionario f ON (lp.idFuncLeitura = f.idFunc)
                left join roteiro_producao_etiqueta rpe ON (lp.idProdPedProducao = rpe.idProdPedProducao and lp.idSetor=rpe.idSetor)) as setor_producao
                group by idProdPedProducao";

            return sql;
        }

        private string SqlUltLeituras(string ids, int idSetor)
        {
            string sql = @"
                            SELECT lp.IdProdPedProducao,
                                   null AS IdsSetores,
                                   lp.DataLeitura AS SETOR0,
                                   f.Nome AS FUNC0
                            FROM leitura_producao lp
                            LEFT JOIN funcionario f ON (lp.IdFuncLeitura = f.IdFunc)
                            WHERE lp.IdProdPedProducao IN (" + ids + @") AND
                                  lp.IdSetor = " + idSetor;
//                                  lp.DataLeitura = (SELECT MAX(DataLeitura) 
//                                                    FROM leitura_producao   
//                                                    WHERE IdProdPedProducao = lp.IdProdPedProducao)";

            return sql;
        }

        public IList<SetorProducao> GetLeiturasSetores(string ids)
        {
            return GetLeiturasSetores(null, ids);
        }

        public IList<SetorProducao> GetLeiturasSetores(GDASession sessao, string ids)
        {
            return objPersistence.LoadData(sessao, Sql(ids)).ToList();
        }
        
        public IList<SetorProducao> GetLeiturasSetores(ProdutoPedidoProducao[] itens)
        {
            return GetLeiturasSetores(null, itens);
        }

        public IList<SetorProducao> GetLeiturasSetores(GDASession sessao, ProdutoPedidoProducao[] itens)
        {
            if (itens.Length == 0)
                return new SetorProducao[0];

            string ids = String.Join(",", itens.Select(x => x.IdProdPedProducao.ToString()).ToArray());

            return GetLeiturasSetores(ids.TrimEnd(','));
        }

        public IList<SetorProducao> GetUltLeiturasSetores(string ids, int idSetor)
        {
            return objPersistence.LoadData(SqlUltLeituras(ids, idSetor)).ToList();
        }

        public IList<SetorProducao> GetUltLeiturasSetores(ProdutoPedidoProducao[] itens, int idSetor)
        {
            if (itens.Length == 0)
                return new SetorProducao[0];

            string ids = String.Join(",", itens.Select(x => x.IdProdPedProducao.ToString()).ToArray());

            return GetUltLeiturasSetores(ids.TrimEnd(','), idSetor);
        }
    }
}
