using System.Collections.Generic;

namespace WebGlass.Business.ConhecimentoTransporte.Fluxo
{
    public class BuscarProprietarioVeiculo_Veiculo : BaseFluxo<BuscarProprietarioVeiculo_Veiculo>
    {
        private BuscarProprietarioVeiculo_Veiculo() { }

        /// <summary>
        /// Busca proprietário associado a um veículo
        /// </summary>
        /// <param name="placa"></param>
        /// <param name="idProprietario"></param>
        /// <returns></returns>
        public Entidade.ProprietarioVeiculo_Veiculo GetProprietarioVeiculo_Veiculo(string placa, uint idProprietario)
        {
            using (Glass.Data.DAL.CTe.ProprietarioVeiculo_VeiculoDAO dao = Glass.Data.DAL.CTe.ProprietarioVeiculo_VeiculoDAO.Instance)
            {
                return new Entidade.ProprietarioVeiculo_Veiculo(dao.GetElement(placa, idProprietario));
            }
        }

        /// <summary>
        /// Busca lista de proprietários de um veículo
        /// </summary>
        /// <param name="placa"></param>
        /// <param name="idProprietario"></param>
        /// <returns></returns>
        public List<Entidade.ProprietarioVeiculo_Veiculo> GetListProprietarioVeiculo_Veiculo(string placa, uint idProprietario)
        {
            var propVeic_Veic = new List<Entidade.ProprietarioVeiculo_Veiculo>();
            using (Glass.Data.DAL.CTe.ProprietarioVeiculo_VeiculoDAO dao = Glass.Data.DAL.CTe.ProprietarioVeiculo_VeiculoDAO.Instance)
            {
                foreach (var i in dao.GetList(placa, idProprietario))
                    propVeic_Veic.Add(new Entidade.ProprietarioVeiculo_Veiculo(i));

                return propVeic_Veic;
            }
        }
    }
}
