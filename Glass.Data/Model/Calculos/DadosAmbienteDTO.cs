using Glass.Comum.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Data.Model.Calculos
{
    class DadosAmbienteDTO : BaseCalculoDTO, IDadosAmbiente
    {
        private static readonly CacheMemoria<List<IAmbienteCalculo>, string> cacheAmbientes;

        private readonly string id;
        private readonly Lazy<List<IAmbienteCalculo>> ambientes;

        static DadosAmbienteDTO()
        {
            cacheAmbientes = new CacheMemoria<List<IAmbienteCalculo>, string>("cacheAmbientes");
        }

        public DadosAmbienteDTO(IContainerCalculo container, Func<IEnumerable<IAmbienteCalculo>> recuperar)
        {
            id = ObterIdContainer(container);
            ambientes = ObterAmbientes(recuperar);
        }

        public IEnumerable<IAmbienteCalculo> Obter()
        {
            return ambientes.Value;
        }

        private string ObterIdContainer(IContainerCalculo container)
        {
            return string.Format(
                "{0}:{1}",
                container.GetType().FullName,
                container.Id
            );
        }

        private Lazy<List<IAmbienteCalculo>> ObterAmbientes(Func<IEnumerable<IAmbienteCalculo>> recuperar)
        {
            return ObterUsandoCache(
                cacheAmbientes,
                id,
                () => recuperar().ToList()
            );
        }
    }
}
