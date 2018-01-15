using GDA;
using System.Collections.Generic;
using System.Linq;

namespace WebGlass.Business.ConhecimentoTransporte.Fluxo
{
    public class CadastrarVeiculoCte : BaseFluxo<CadastrarVeiculoCte>
    {
        private CadastrarVeiculoCte() { }

        public List<uint> Insert(List<Entidade.VeiculoCte> listaVeiculosCte)
        {
            return Insert(null, listaVeiculosCte);
        }

        public uint Insert(Entidade.VeiculoCte veiculoCte)
        {
            return Insert(null, veiculoCte);
        }

        /// <summary>
        /// insere dados
        /// </summary>
        /// <param name="veiculoCte"></param>
        /// <returns></returns>
        public uint Insert(GDASession sessao, Entidade.VeiculoCte veiculoCte)
        {
            return Glass.Data.DAL.CTe.VeiculoCteDAO.Instance.Insert(sessao, Convert(veiculoCte));
        }

        /// <summary>
        /// insere lista de dados
        /// </summary>
        /// <param name="listaVeiculosCte"></param>
        /// <returns></returns>
        public List<uint> Insert(GDASession sessao, List<Entidade.VeiculoCte> listaVeiculosCte)
        {
            var lista = new List<uint>();
            foreach (var i in listaVeiculosCte)
                lista.Add(Glass.Data.DAL.CTe.VeiculoCteDAO.Instance.Insert(sessao, Convert(i)));

            return lista;
        }

        public List<uint> Update(List<Entidade.VeiculoCte> listaVeiculosCte)
        {
            return Update(null, listaVeiculosCte);
        }

        /// <summary>
        /// atualiza lista de dados
        /// </summary>
        /// <param name="listaVeiculosCte"></param>
        /// <returns></returns>
        public List<uint> Update(GDASession sessao, List<Entidade.VeiculoCte> listaVeiculosCte)
        {
            var lista = new List<uint>();
            if (listaVeiculosCte.Select(f => !string.IsNullOrEmpty(f.Placa) || f.ValorFrete != 0).FirstOrDefault())
            {
                Glass.Data.DAL.CTe.VeiculoCteDAO.Instance.Delete(sessao, listaVeiculosCte.Where(f => f.IdCte > 0).First().IdCte);
                foreach (var i in listaVeiculosCte)
                    lista.Add(Insert(sessao, i));
            }
            return lista;
        }

        /// <summary>
        /// atualiza dados
        /// </summary>
        /// <param name="veiculoCte"></param>
        /// <returns></returns>
        public int Update(Entidade.VeiculoCte veiculoCte)
        {
            return Glass.Data.DAL.CTe.VeiculoCteDAO.Instance.Update(Convert(veiculoCte));
        }

        /// <summary>
        /// converte dados da entidade na model
        /// </summary>
        /// <param name="veiculoCte"></param>
        /// <returns></returns>
        internal Glass.Data.Model.Cte.VeiculoCte Convert(Entidade.VeiculoCte veiculoCte)
        {
            return new Glass.Data.Model.Cte.VeiculoCte
            {
                IdCte = veiculoCte.IdCte,
                Placa = veiculoCte.Placa,
                ValorFrete = veiculoCte.ValorFrete
            };
        }
    }
}
