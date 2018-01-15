using System;
using GDA;
using Glass.Data.Model;

namespace Glass.Data.DAL
{
    public sealed class VeiculoDAO : BaseCadastroDAO<Veiculo, VeiculoDAO>
	{
        //private VeiculoDAO() { }

        public Veiculo GetElement(string placa)
        {
            string sql = "Select * From veiculo Where Placa='" + placa + "'";

            return CurrentPersistenceObject.LoadOneData(sql);
        }

        public string GetDescVeiculo(string placa)
        {
            return ObtemValorCampo<string>("CONCAT(placa, ' ', modelo, ' ', Cor, ' ', AnoFab)", "placa=?p", new GDAParameter("?p", placa));
        }

        public Veiculo[] GetOrdered()
        {
            string sql = string.Format("Select * From veiculo Where Situacao = {0} Order By Placa", (int)Glass.Situacao.Ativo);

            return CurrentPersistenceObject.LoadData(sql).ToList().ToArray();
        }

        /// <summary>
        /// Verifica se o veículo está associado à alguma equipe
        /// </summary>
        /// <param name="placa"></param>
        /// <returns></returns>
        public bool EstaAssociado(string placa)
        {
            string sql = "Select Count(*) From equipe where placa='" + placa + "'";

            return Glass.Conversoes.StrParaInt(CurrentPersistenceObject.ExecuteScalar(sql).ToString()) > 0;
        }

        public override int Delete(Veiculo objDelete)
        {
            // Verifica se este veículo está associado à alguma equipe
            if (EstaAssociado(objDelete.Placa))
                throw new Exception("Este veículo está associado à uma equipe de instalação e não pode ser excluído.");

            return base.Delete(objDelete);
        }

        public float ObtemCapacidadeKgVeiculo(string placa)
        {
            return ObtemCapacidadeKgVeiculo(null, placa);
        }

        public float ObtemCapacidadeKgVeiculo(GDASession sessao, string placa)
        {
            return ObtemValorCampo<float>(sessao, "CAPACIDADEKG", "Placa='" + placa + "'");
        }

    }
}