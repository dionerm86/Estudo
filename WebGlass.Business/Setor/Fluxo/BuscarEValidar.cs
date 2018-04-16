using System.Collections.Generic;
using System.Linq;
using Glass.Data.DAL;

namespace WebGlass.Business.Setor.Fluxo
{
    public sealed class BuscarEValidar : BaseFluxo<BuscarEValidar>
    {
        private BuscarEValidar() { }

        public IList<Entidade.SetorParaRoteiro> ObtemSetoresRoteiroProducao()
        {
            var tiposSetores = new List<Glass.Data.Model.TipoSetor>() {
                Glass.Data.Model.TipoSetor.PorRoteiro
            };

            var setores = SetorDAO.Instance.GetOrdered()
                .Where(x => x.NumeroSequencia > 1)
                .Where(x => tiposSetores.Contains(x.Tipo));

            return setores.Select(x => new Entidade.SetorParaRoteiro(x)).ToList();
        }
    }
}
