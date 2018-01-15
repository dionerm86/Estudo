using System.Collections.Generic;
using GDA;

namespace WebGlass.Business.ConhecimentoTransporte.Fluxo
{
    public class CadastrarMotoristaCteRod : BaseFluxo<CadastrarMotoristaCteRod>
    {
        private CadastrarMotoristaCteRod() { }

        public uint Insert(Entidade.MotoristaCteRod motoristaCteRod)
        {
            return Insert(null, motoristaCteRod);
        }

        /// <summary>
        /// insere dados
        /// </summary>
        /// <param name="motoristaCteRod"></param>
        /// <returns></returns>
        public uint Insert(GDASession sessao, Entidade.MotoristaCteRod motoristaCteRod)
        {
            return Glass.Data.DAL.CTe.MotoristaCteRodDAO.Instance.Insert(sessao, Convert(motoristaCteRod));
        }

        /// <summary>
        /// atualiza dados
        /// </summary>
        /// <param name="listaMotoristasCte"></param>
        /// <returns></returns>
        public List<uint> Insert(List<Entidade.MotoristaCteRod> listaMotoristasCte)
        {
            var lista = new List<uint>();
            foreach (var i in listaMotoristasCte)
                lista.Add(Glass.Data.DAL.CTe.MotoristaCteRodDAO.Instance.Insert(Convert(i)));

            return lista;
        }

        /// <summary>
        /// converte dados da entidade na model
        /// </summary>
        /// <param name="listaMotoristasCte"></param>
        /// <returns></returns>
        public List<uint> Update(List<Entidade.MotoristaCteRod> listaMotoristasCte)
        {
            var lista = new List<uint>();
            foreach (var i in listaMotoristasCte)
            {
                Glass.Data.DAL.CTe.MotoristaCteRodDAO.Instance.Delete(i.IdCte);
                lista.Add(Insert(i));
            }

            return lista;
        }

        public int Update(WebGlass.Business.ConhecimentoTransporte.Entidade.MotoristaCteRod motoristaCteRod)
        {
            return Glass.Data.DAL.CTe.MotoristaCteRodDAO.Instance.Update(Convert(motoristaCteRod));
        }

        internal Glass.Data.Model.Cte.MotoristaCteRod Convert(WebGlass.Business.ConhecimentoTransporte.Entidade.MotoristaCteRod motoristaCteRod)
        {
            return new Glass.Data.Model.Cte.MotoristaCteRod
            {
                IdCte = motoristaCteRod.IdCte,
                IdFunc = motoristaCteRod.IdFunc
            };
        }
    }
}
