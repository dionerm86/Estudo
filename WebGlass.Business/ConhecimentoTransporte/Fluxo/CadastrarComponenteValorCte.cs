using GDA;
using System.Collections.Generic;
using System.Linq;

namespace WebGlass.Business.ConhecimentoTransporte.Fluxo
{
    public class CadastrarComponenteValorCte : BaseFluxo<CadastrarComponenteValorCte>
    {
        private CadastrarComponenteValorCte() { }

        public List<uint> Insert(List<Entidade.ComponenteValorCte> listaComponentesCte)
        {
            return Insert(null, listaComponentesCte);
        }

        public uint Insert(Entidade.ComponenteValorCte componente)
        {
            return Insert(null, componente);
        }

        /// <summary>
        /// insere dados
        /// </summary>
        /// <param name="componente"></param>
        /// <returns></returns>
        public uint Insert(GDASession sessao, Entidade.ComponenteValorCte componente)
        {
            return Glass.Data.DAL.CTe.ComponenteValorCteDAO.Instance.Insert(sessao, Convert(componente));
        }

        /// <summary>
        /// insere lista de dados
        /// </summary>
        /// <param name="listaComponentesCte"></param>
        /// <returns></returns>
        public List<uint> Insert(GDASession sessao, List<Entidade.ComponenteValorCte> listaComponentesCte)
        {
            var lista = new List<uint>();
            foreach (var i in listaComponentesCte)
                lista.Add(Glass.Data.DAL.CTe.ComponenteValorCteDAO.Instance.Insert(sessao, Convert(i)));

            return lista;
        }

        public List<uint> Update(List<Entidade.ComponenteValorCte> listaComponentesCte)
        {
            return Update(listaComponentesCte);
        }

        /// <summary>
        /// atualiza lista dados
        /// </summary>
        /// <param name="listaComponentesCte"></param>
        /// <returns></returns>
        public List<uint> Update(GDASession sessao, List<Entidade.ComponenteValorCte> listaComponentesCte)
        {
            var lista = new List<uint>();
            Glass.Data.DAL.CTe.ComponenteValorCteDAO.Instance.Delete(sessao, listaComponentesCte.Where(f => f.IdCte > 0).First().IdCte);
            foreach (var i in listaComponentesCte)
                lista.Add(Insert(sessao, i));

            return lista;
        }

        /// <summary>
        /// atualiza dados
        /// </summary>
        /// <param name="componente"></param>
        /// <returns></returns>
        public int Update(Entidade.ComponenteValorCte componente)
        {
            return Glass.Data.DAL.CTe.ComponenteValorCteDAO.Instance.Update(Convert(componente));
        }

        /// <summary>
        /// converte dados da entidade para a model
        /// </summary>
        /// <param name="componente"></param>
        /// <returns></returns>
        internal Glass.Data.Model.Cte.ComponenteValorCte Convert(Entidade.ComponenteValorCte componente)
        {
            return new Glass.Data.Model.Cte.ComponenteValorCte
            {
                IdCte = componente.IdCte,
                NomeComponente = componente.NomeComponente,
                ValorComponente = componente.ValorComponente
            };
        }
    }
}
