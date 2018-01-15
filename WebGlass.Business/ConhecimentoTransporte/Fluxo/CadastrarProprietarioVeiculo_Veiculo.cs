using System.Collections.Generic;

namespace WebGlass.Business.ConhecimentoTransporte.Fluxo
{
    public class CadastrarProprietarioVeiculo_Veiculo : BaseFluxo<CadastrarProprietarioVeiculo_Veiculo>
    {
        private CadastrarProprietarioVeiculo_Veiculo() { }
        
        /// <summary>
        /// insere dados
        /// </summary>
        /// <param name="propVeic_Veiculo"></param>
        /// <returns></returns>
        public uint Insert(Entidade.ProprietarioVeiculo_Veiculo propVeic_Veiculo)
        {
            return Glass.Data.DAL.CTe.ProprietarioVeiculo_VeiculoDAO.Instance.Insert(Convert(propVeic_Veiculo));
        }

        /// <summary>
        /// insere lista de dados
        /// </summary>
        /// <param name="listaPropVeic_Veic"></param>
        /// <returns></returns>
        public List<uint> Insert(List<Entidade.ProprietarioVeiculo_Veiculo> listaPropVeic_Veic)
        {
            var lista = new List<uint>();
            foreach (var i in listaPropVeic_Veic)
                lista.Add(Glass.Data.DAL.CTe.ProprietarioVeiculo_VeiculoDAO.Instance.Insert(Convert(i)));

            return lista;
        }

        /// <summary>
        /// atualiza lista de proprietarios de veículos
        /// </summary>
        /// <param name="listaPropVeic_Veic"></param>
        /// <returns></returns>
        public List<uint> Update(List<Entidade.ProprietarioVeiculo_Veiculo> listaPropVeic_Veic)
        {
            var lista = new List<uint>();
            foreach (var i in listaPropVeic_Veic)
            {
                Glass.Data.DAL.CTe.ProprietarioVeiculo_VeiculoDAO.Instance.Delete(Convert(i));
                lista.Add(Insert(i));
            }

            return lista;
        }

        /// <summary>
        /// atualiza dados
        /// </summary>
        /// <param name="propVeic_Veic"></param>
        /// <returns></returns>
        public uint Update(Entidade.ProprietarioVeiculo_Veiculo propVeic_Veic)
        {
            Glass.Data.DAL.CTe.ProprietarioVeiculo_VeiculoDAO.Instance.Delete(Convert(propVeic_Veic));
            return Glass.Data.DAL.CTe.ProprietarioVeiculo_VeiculoDAO.Instance.Insert(Convert(propVeic_Veic));
        }

        /// <summary>
        /// converte dados da entidade na model
        /// </summary>
        /// <param name="propVeic_Veic"></param>
        /// <returns></returns>
        internal Glass.Data.Model.Cte.ProprietarioVeiculo_Veiculo Convert(Entidade.ProprietarioVeiculo_Veiculo propVeic_Veic)
        {
            return new Glass.Data.Model.Cte.ProprietarioVeiculo_Veiculo
            {
                IdPropVeic = propVeic_Veic.IdPropVeic,
                Placa = propVeic_Veic.Placa
            };
        }
    }
}
