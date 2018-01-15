using System;
using System.Collections.Generic;
using System.Linq;
using Glass.Data.DAL;
using Microsoft.Practices.ServiceLocation;

namespace WebGlass.Business.CapacidadeProducaoDiaria.Fluxo
{
    public class CapacidadeProducaoDiaria : BaseFluxo<CapacidadeProducaoDiaria>
    {
        private CapacidadeProducaoDiaria() { }

        public IList<Entidade.CapacidadeProducaoDiaria> ObtemPeloPeriodo(DateTime dataInicio, DateTime dataFim)
        {
            var itens = CapacidadeProducaoDiariaDAO.Instance.ObtemPeloPeriodo(dataInicio, dataFim);
            return itens.Select(x => new Entidade.CapacidadeProducaoDiaria(x)).ToList();
        }

        public void Salvar(Entidade.CapacidadeProducaoDiaria capacidade)
        {
            CapacidadeProducaoDiariaDAO.Instance.Salvar(capacidade._capacidade, capacidade._setores);

            var result = ServiceLocator.Current.GetInstance<Glass.PCP.Negocios.ICapacidadeProducaoDiariaClassificacaoFluxo>()
                .Salvar(capacidade.Data, capacidade._classificacoes);

            if (!result.Success)
                throw new Exception(result.Message.Format());
        }
    }
}
