using System;
using System.Collections.Generic;
using System.Linq;
using Glass.Data.Model;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class CapacidadeProducaoDiariaSetorDAO : BaseDAO<CapacidadeProducaoDiariaSetor, CapacidadeProducaoDiariaSetorDAO>
    {
        //private CapacidadeProducaoDiariaSetorDAO() { }

        public IList<CapacidadeProducaoDiariaSetor> ObtemPelaData(DateTime data)
        {
            return ObtemPelaData(null, data);
        }

        public IList<CapacidadeProducaoDiariaSetor> ObtemPelaData(GDASession session, DateTime data)
        {
            string sql = @"
                select coalesce(c.data, date(?data)) as data, s.idSetor, 
	                coalesce(c.capacidade, s.capacidadeDiaria) as capacidade
                from setor s
	                left join capacidade_producao_diaria_setor c on (s.idSetor=c.idSetor and date(c.data)=date(?data))
                where s.numSeq > 1 and s.tipo not in (" +
                    (int)TipoSetor.Entregue + "," + (int)TipoSetor.ExpCarregamento + @")
                    AND COALESCE(s.ignorarCapacidadeDiaria, false)=false
                    AND s.situacao = " + (int)Glass.Situacao.Ativo + @"
                order by s.numSeq";

            return objPersistence.LoadData(session, sql, new GDAParameter("?data", data)).ToList();
        }

        public int CapacidadeSetorPelaData(DateTime data, uint idSetor)
        {
            return CapacidadeSetorPelaData(null, data, idSetor);
        }

        public int CapacidadeSetorPelaData(GDASession session, DateTime data, uint idSetor)
        {
            var setor = ObtemPelaData(session, data).FirstOrDefault(x => x.IdSetor == idSetor);
            return (setor ?? new CapacidadeProducaoDiariaSetor()).Capacidade;
        }

        public void Salvar(DateTime data, IEnumerable<CapacidadeProducaoDiariaSetor> dados)
        {
            objPersistence.ExecuteCommand("delete from capacidade_producao_diaria_setor where date(data)=date(?data)",
                new GDAParameter("?data", data));

            foreach (var d in dados)
                Insert(d);
        }
    }
}
