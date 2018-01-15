using System;
using System.Collections.Generic;
using System.Linq;
using Glass.Data.Model;
using GDA;
using Glass.Data.Helper;
using Glass.Configuracoes;

namespace Glass.Data.DAL
{
    public sealed class CapacidadeProducaoDiariaDAO : BaseDAO<CapacidadeProducaoDiaria, CapacidadeProducaoDiariaDAO>
    {
        //private CapacidadeProducaoDiariaDAO() { }

        public IList<CapacidadeProducaoDiaria> ObtemPeloPeriodo(DateTime dataInicio, DateTime dataFim)
        {
            return ObtemPeloPeriodo(null, dataInicio, dataFim);
        }

        public IList<CapacidadeProducaoDiaria> ObtemPeloPeriodo(GDASession session, DateTime dataInicio, DateTime dataFim)
        {
            var retorno = new List<CapacidadeProducaoDiaria>();
            var data = dataInicio.Date;

            while (data.Date <= dataFim.Date)
            {
                var capacidade = new CapacidadeProducaoDiaria()
                {
                    Data = data,
                    MaximoVendasM2 = ObtemValorCampo<int?>(session, "maximoVendasM2", "date(data)=date(?data)",
                        new GDAParameter("?data", data)) ?? PedidoConfig.Pedido_MaximoVendas.MaximoVendasM2
                };

                retorno.Add(capacidade);

                data = data.AddDays(1);
            }

            return retorno;
        }

        public int MaximoVendasData(DateTime data)
        {
            return MaximoVendasData(null, data);
        }

        public int MaximoVendasData(GDASession session, DateTime data)
        {
            var capacidade = ObtemPeloPeriodo(session, data, data)[0];
            return capacidade.MaximoVendasM2.GetValueOrDefault();
        }

        public void Salvar(CapacidadeProducaoDiaria objInsert, IEnumerable<CapacidadeProducaoDiariaSetor> setores)
        {
            //Verifica se o valor informado não é menor que o já previsto para produção
            foreach (var s in setores)
            {
                if (s.Capacidade == 0)
                    continue;

                var totMPrevisaoProducao = Glass.Data.RelDAL.PrevisaoProducaoDAO.Instance
                    .ObtemPrevisaoProducao(s.IdSetor, s.Data.ToShortDateString())
                    .Sum(f => f.TotM);

                if (totMPrevisaoProducao > s.Capacidade)
                    throw new Exception(string.Format("Não é possível definir a capacidade para o setor {0} ({1:0.##} m²)" +
                        " no dia {2}, pois a previsão de produção atual para essa data é de {3:0.##} m²",
                        Utils.ObtemSetor(s.IdSetor).Descricao,
                        s.Capacidade,
                        s.Data.ToShortDateString(),
                        totMPrevisaoProducao));
            }

            var atual = ObtemParaLog(objInsert.IdParaLog);
            string temp = atual.DadosSetor; // Busca as configurações atuais do setor e salva no objeto

            if (PedidoConfig.Pedido_MaximoVendas.MaximoVendas)
            {
                objPersistence.ExecuteCommand("delete from capacidade_producao_diaria where date(data)=date(?data)",
                    new GDAParameter("?data", objInsert.Data));

                if (objInsert.MaximoVendasM2 != null && objInsert.MaximoVendasM2 != PedidoConfig.Pedido_MaximoVendas.MaximoVendasM2)
                    Insert(objInsert);
            }

            if (Glass.Configuracoes.ProducaoConfig.CapacidadeProducaoPorSetor)
                CapacidadeProducaoDiariaSetorDAO.Instance.Salvar(objInsert.Data, setores);

            LogAlteracaoDAO.Instance.LogCapacidadeProducaoDiaria(atual);
        }

        public CapacidadeProducaoDiaria ObtemParaLog(uint idParaLog)
        {
            return ObtemParaLog(null, idParaLog);
        }

        public CapacidadeProducaoDiaria ObtemParaLog(GDASession session, uint idParaLog)
        {
            var retorno = new CapacidadeProducaoDiaria()
            {
                IdParaLog = idParaLog
            };

            retorno.MaximoVendasM2 = MaximoVendasData(session, retorno.Data);
            return retorno;
        }
    }
}
