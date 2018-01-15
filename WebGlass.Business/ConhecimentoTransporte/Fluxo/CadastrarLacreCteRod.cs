using System.Collections.Generic;
using System.Linq;
using GDA;

namespace WebGlass.Business.ConhecimentoTransporte.Fluxo
{
    public class CadastrarLacreCteRod : BaseFluxo<CadastrarLacreCteRod>
    {
        private CadastrarLacreCteRod() { }

        public uint Insert(Entidade.LacreCteRod lacreCteRod)
        {
            return Insert(null, lacreCteRod);
        }

        /// <summary>
        /// insere dados
        /// </summary>
        /// <param name="lacreCteRod"></param>
        /// <returns></returns>
        public uint Insert(GDASession sessao, Entidade.LacreCteRod lacreCteRod)
        {
            return Glass.Data.DAL.CTe.LacreCteRodDAO.Instance.Insert(sessao, Convert(lacreCteRod));
        }

        /// <summary>
        /// atualiza dados
        /// </summary>
        /// <param name="lacreCteRod"></param>
        /// <returns></returns>
        public List<uint> Update(Entidade.LacreCteRod lacreCteRod)
        {
            var listaNumLacre = lacreCteRod.NumeroLacre.Contains(';') ? lacreCteRod.NumeroLacre.Split(';') : new string[] { lacreCteRod.NumeroLacre };
            var retorno = new List<uint>();
            Glass.Data.DAL.CTe.LacreCteRodDAO.Instance.Delete(lacreCteRod.IdCte);
            if (listaNumLacre.Count() > 1)
            {
                foreach (var i in listaNumLacre)
                {
                    var novoLacre = new Glass.Data.Model.Cte.LacreCteRod { IdCte = lacreCteRod.IdCte, NumeroLacre = i };
                    retorno.Add(Insert(new Entidade.LacreCteRod(novoLacre)));
                }
            }
            return retorno;
        }

        /// <summary>
        /// converte dados da entidade na model
        /// </summary>
        /// <param name="lacreCteRod"></param>
        /// <returns></returns>
        internal Glass.Data.Model.Cte.LacreCteRod Convert(Entidade.LacreCteRod lacreCteRod)
        {
            return new Glass.Data.Model.Cte.LacreCteRod
            {
                IdCte = lacreCteRod.IdCte,
                NumeroLacre = lacreCteRod.NumeroLacre
            };
        }
    }
}
