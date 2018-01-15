using GDA;
using System.Collections.Generic;
using System.Linq;

namespace WebGlass.Business.ConhecimentoTransporte.Fluxo
{
    public class CadastrarSeguroCte : BaseFluxo<CadastrarSeguroCte>
    {
        private CadastrarSeguroCte() { }

        public uint Insert(Entidade.SeguroCte seguroCte)
        {
            return Insert(null, seguroCte);
        }

        /// <summary>
        /// insere dados
        /// </summary>
        /// <param name="seguroCte"></param>
        /// <returns></returns>
        public uint Insert(GDASession sessao, Entidade.SeguroCte seguroCte)
        {
            return Glass.Data.DAL.CTe.SeguroCteDAO.Instance.Insert(sessao, Convert(seguroCte));
        }

        /// <summary>
        /// insere lista de dados
        /// </summary>
        /// <param name="listaSegurosCte"></param>
        /// <returns></returns>
        public List<uint> Insert(List<Entidade.SeguroCte> listaSegurosCte)
        {
            var lista = new List<uint>();
            foreach (var i in listaSegurosCte)
                lista.Add(Glass.Data.DAL.CTe.SeguroCteDAO.Instance.Insert(Convert(i)));

            return lista;
        }

        public List<uint> Update(List<Entidade.SeguroCte> listaSegurosCte)
        {
            return Update(null, listaSegurosCte);
        }

        /// <summary>
        /// atualiza lista de dados
        /// </summary>
        /// <param name="listaSegurosCte"></param>
        /// <returns></returns>
        public List<uint> Update(GDASession sessao, List<Entidade.SeguroCte> listaSegurosCte)
        {
            var lista = new List<uint>();
            if (listaSegurosCte.Count > 0 && listaSegurosCte.Select(f => f.ResponsavelSeguro < 5).FirstOrDefault())
            {
                Glass.Data.DAL.CTe.SeguroCteDAO.Instance.Delete(sessao, listaSegurosCte.Where(f => f.IdCte > 0).First().IdCte);
                foreach (var i in listaSegurosCte)
                    lista.Add(Insert(sessao, i));
            }
            return lista;
        }

        public int Update(Entidade.SeguroCte seguroCte)
        {
            return Update(null, seguroCte);
        }

        /// <summary>
        /// atualiza dados do seguro
        /// </summary>
        /// <param name="seguroCte"></param>
        /// <returns></returns>
        public int Update(GDASession sessao, Entidade.SeguroCte seguroCte)
        {
            try
            {
                GDA.GDAOperations.Delete(sessao, seguroCte.SeguroCteModel);
            }
            catch { }

            return (int)Glass.Data.DAL.CTe.SeguroCteDAO.Instance.Insert(sessao, Convert(seguroCte));
        }

        /// <summary>
        /// converte dados da entidade na model
        /// </summary>
        /// <param name="seguroCte"></param>
        /// <returns></returns>
        internal Glass.Data.Model.Cte.SeguroCte Convert(Entidade.SeguroCte seguroCte)
        {
            return new Glass.Data.Model.Cte.SeguroCte
            {
                IdCte = seguroCte.IdCte,
                IdSeguradora = seguroCte.IdSeguradora,
                NumeroApolice = seguroCte.NumeroApolice,
                NumeroAverbacao = seguroCte.NumeroAverbacao,
                ResponsavelSeguro = seguroCte.ResponsavelSeguro,
                ValorCargaAverbacao = seguroCte.ValorCargaAverbacao
            };
        }
    }
}
