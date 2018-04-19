using Glass.Comum.Cache;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Glass.Data.Model.Calculos
{
    class DadosAmbienteDTO : BaseCalculoDTO, IDadosAmbiente
    {
        private static readonly CacheMemoria<List<IAmbienteCalculo>, string> cacheAmbientes;

        private readonly string id;
        private Lazy<List<IAmbienteCalculo>> ambientes;
        private readonly Func<IEnumerable<IAmbienteCalculo>> recuperar;

        static DadosAmbienteDTO()
        {
            cacheAmbientes = new CacheMemoria<List<IAmbienteCalculo>, string>("cacheAmbientes");
        }

        public DadosAmbienteDTO(IContainerCalculo container, Func<IEnumerable<IAmbienteCalculo>> recuperar)
        {
            id = ObterIdContainer(container);
            this.recuperar = recuperar;
        }

        public IEnumerable<IAmbienteCalculo> Obter(bool forcarAtualizacao = false)
        {
            if (forcarAtualizacao || ambientes == null)
            {
                ambientes = ObterAmbientes(recuperar, forcarAtualizacao);
            }

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

        private Lazy<List<IAmbienteCalculo>> ObterAmbientes(Func<IEnumerable<IAmbienteCalculo>> recuperar, bool forcarAtualizacao)
        {
            return ObterUsandoCache(
                cacheAmbientes,
                id,
                () => recuperar().ToList(),
                forcarAtualizacao
            );
        }
    }
}
