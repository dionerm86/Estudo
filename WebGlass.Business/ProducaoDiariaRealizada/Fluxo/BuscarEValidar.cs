using System;
using System.Collections.Generic;
using System.Linq;
using Glass.Data.RelDAL;
using Glass.Data.Helper;
using Glass;

namespace WebGlass.Business.ProducaoDiariaRealizada.Fluxo
{
    public sealed class BuscarEValidar : BaseFluxo<BuscarEValidar>
    {
        private BuscarEValidar() { }

        public IList<Entidade.ProducaoDiariaRealizada> ObtemDadosProducao(DateTime dataProducao)
        {
            return ObtemDadosProducao(0, dataProducao);
        }

        public IList<Entidade.ProducaoDiariaRealizada> ObtemDadosProducao(uint codigoSetor, DateTime dataProducao)
        {
            // Recupera os dados de produção para o dia (esse método ignora as peças já produzidas)
            var previsto = CapacidadeProducaoDAO.Instance.ObtemCapacidadeProducao(codigoSetor, dataProducao);

            // Recupera os dados de peças já produzidas para o dia
            var realizado = ProducaoDiariaRealizadaDAO.Instance.ObtemDadosProducao(codigoSetor, dataProducao);

            var retorno = new List<Glass.Data.RelModel.ProducaoDiariaRealizada>();

            var setores = Utils.GetSetores;
            if (codigoSetor > 0)
                setores = setores.Where(x => x.IdSetor == codigoSetor).ToArray();

            // Adiciona uma model para cada setor que não seja do tipo expedição ou
            // expedição de carregamento e que não seja impressão de etiquetas (num. seq. 1)
            foreach (var s in setores)
            {
                if (s.NumeroSequencia == 1 || s.Tipo == Glass.Data.Model.TipoSetor.ExpCarregamento ||
                    s.Tipo == Glass.Data.Model.TipoSetor.Entregue)
                    continue;

                retorno.Add(new Glass.Data.RelModel.ProducaoDiariaRealizada()
                {
                    IdSetor = (uint)s.IdSetor
                });
            }

            // Soma a capacidade de produção restante no total previsto
            // porque o método de previsão desconsidera as peças já produzidas,
            // retornando o valor correto de produção
            foreach (var ret in retorno)
            {
                var p = previsto.FirstOrDefault(x => x.IdSetor == ret.IdSetor);
                var r = realizado.FirstOrDefault(x => x.IdSetor == ret.IdSetor);

                ret.TotMPrevisto = (p != null ? p.TotM : 0) + (r != null ? r.TotMHoje : 0);
                ret.TotMRealizado = r != null ? r.TotMRealizado : 0;
            }

            return  retorno.Select(x => new Entidade.ProducaoDiariaRealizada(x, dataProducao)).ToList();
        }

        public IList<Entidade.ProducaoDiariaRealizada> ObtemDadosProducao(uint codigoSetor, DateTime dataInicio, DateTime dataFim)
        {
            var retorno = new List<Entidade.ProducaoDiariaRealizada>();

            DateTime data = dataInicio.Date;

            while (data <= dataFim.Date)
            {
                retorno.AddRange(ObtemDadosProducao(codigoSetor, data));
                data = data.AddDays(1);
            }

            return (from r in retorno
                    group r by r.CodigoSetor into g
                    select new Glass.Data.RelModel.ProducaoDiariaRealizada()
                    {
                        IdSetor = g.Key,
                        TotMHoje = 0,
                        TotMPrevisto = g.Sum(x => x.M2Previsto),
                        TotMRealizado = g.Sum(x => x.M2Realizado)
                    }).Select(x => new Entidade.ProducaoDiariaRealizada(x, dataFim)).ToList();
        }

        public IList<Entidade.ProducaoDiariaRealizada> ObtemDadosProducaoForPainelComercialSetores(uint idSetor, DateTime dataFabrica, DateTime dataEntrega)
        {
            //Recupera a previsão de produção da data informada.
            var previsto = PrevisaoProducaoDAO.Instance.ObtemPrevisaoProducao(idSetor, dataFabrica.ToShortDateString(), dataEntrega.ToShortDateString());

            if (previsto.Count == 0)
                previsto.Add(new Glass.Data.RelModel.PrevisaoProducao() { IdSetor = idSetor, DataFabrica = dataFabrica });


            return previsto.Select(f => new Entidade.ProducaoDiariaRealizada(f)).ToList();
        }

        public IList<Entidade.ProducaoDiariaRealizada> ObtemDadosProducaoForPainelComercialClassificacao(int idClassificacao, DateTime dataFabrica, DateTime dataEntrega)
        {
            //Recupera a previsão de produção da data informada.
            var previsto = PrevisaoProducaoDAO.Instance.ObtemPrevisaoProducao(idClassificacao, dataFabrica.ToShortDateString(), dataEntrega.ToShortDateString());

            if (previsto.Count == 0)
                previsto.Add(new Glass.Data.RelModel.PrevisaoProducao() { IdClassificacaoRoteiroProducao = idClassificacao, DataFabrica = dataFabrica });

            return previsto.Select(f => new Entidade.ProducaoDiariaRealizada(f)).ToList();
        }

        public IList<Entidade.ProducaoDiariaRealizada> ObtemDadosProducaoForPainelPlanejamentoSetores(DateTime dataProducao)
        {
            //Recupera a previsão de produção
            var dados = ProducaoDiariaRealizadaDAO.Instance.ObtemProducaoRealizadaPorSetor(0, dataProducao);

            return dados.Select(x => new Entidade.ProducaoDiariaRealizada(x, dataProducao))
                .Where(f => f.Capacidade > 0).ToList();
        }

        public IList<Glass.Data.RelModel.ProducaoDiariaRealizada> ObtemDadosProducaoForPainelPlanejamento()
        {
            var datas = new List<DateTime>();
            var diaAtual = DateTime.Today;
            datas.Add(diaAtual);

            for (int i = 0; i < 5; i++)
            {
                var dataAnt = diaAtual.AddDays(-1);

                while (!dataAnt.DiaUtil())
                    dataAnt = dataAnt.AddDays(-1);

                datas.Add(dataAnt);
                diaAtual = dataAnt;
            }

            diaAtual = DateTime.Today;

            for (int i = 0; i < 5; i++)
            {
                var dataAnt = diaAtual.AddDays(1);

                while (!dataAnt.DiaUtil())
                    dataAnt = dataAnt.AddDays(1);

                datas.Add(dataAnt);
                diaAtual = dataAnt;
            }

            var retorno = new List<Glass.Data.RelModel.ProducaoDiariaRealizada>();

            //Recupera a previsão de produção
            foreach (var data in datas)
                retorno.AddRange(ProducaoDiariaRealizadaDAO.Instance.ObtemProducaoRealizada(data));

            return retorno.OrderBy(f => f.DataFabrica).ToList();
        }
    }
}
