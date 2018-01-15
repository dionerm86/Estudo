using System;
using Glass.Data.Model;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class RotaClienteDAO : BaseDAO<RotaCliente, RotaClienteDAO>
    {
        //private RotaClienteDAO() { }

        /// <summary>
        /// Associa cliente à uma rota
        /// </summary>
        /// <param name="idRota"></param>
        /// <param name="idCliente"></param>
        public void AssociaCliente(GDASession sessao, uint idRota, uint idCliente)
        {
            if (idRota == 0)
                throw new Exception("Não foi informada uma rota válida.");

            if (IsClienteAssociado(idRota, idCliente))
                throw new Exception("Este cliente já está associado à esta rota.");

            RotaCliente rotaCliente = new RotaCliente();
            rotaCliente.IdRota = (int)idRota;
            rotaCliente.IdCliente = (int)idCliente;
            rotaCliente.NumSeq = ProximoNumSeq(sessao, idRota);
            Insert(sessao, rotaCliente);
        }

        /// <summary>
        /// Troca posição da categoria de contas
        /// </summary>
        /// <param name="idBenefConfig"></param>
        /// <param name="acima"></param>
        public void ChangePosition(uint idRota, uint idCliente, bool acima)
        {
            int numSeq = ObtemValorCampo<int>("numSeq", "idRota=" + idRota + " and idCliente=" + idCliente);

            int numSeqAdjacente = ObtemValorCampo<int>("numSeq", "idRota=" + idRota + " and numSeq" + (acima ? "<" : ">") + numSeq +
                " order by numSeq " + (acima ? "desc" : "asc") + " limit 1");

            if (numSeqAdjacente > 0)
            {
                // Altera a posição do cliente adjacente
                objPersistence.ExecuteCommand("Update rota_cliente Set numSeq=" + numSeq + " Where idRota=" + idRota + " And numSeq=" + numSeqAdjacente);

                // Altera a posição do cliente clicado
                objPersistence.ExecuteCommand("Update rota_cliente Set numSeq=" + numSeqAdjacente + " Where idRota=" + idRota + " And idCliente=" + idCliente);
            }            
        }

        public int ProximoNumSeq(GDASession sessao, uint idRota)
        {
            string sql = "Select Max(Coalesce(numSeq, 0))+1 From rota_cliente Where idRota=" + idRota;

            object obj = objPersistence.ExecuteScalar(sessao, sql);

            return obj != null && !String.IsNullOrEmpty(obj.ToString()) ? Glass.Conversoes.StrParaInt(obj.ToString()) : 1;
        }

        /// <summary>
        /// Desassocia cliente associado à uma rota
        /// </summary>
        /// <param name="idRota"></param>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        public int DesassociaCliente(GDASession sessao, uint idRota, uint idCliente)
        {
            string sql = "Delete From rota_cliente Where idRota=" + idRota + " And idCliente=" + idCliente;

            return objPersistence.ExecuteCommand(sessao, sql);
        }

        /// <summary>
        /// Verifica se o cliente já foi associado a uma rota
        /// </summary>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        public bool IsClienteAssociado(uint idCliente)
        {
            return IsClienteAssociado(null, idCliente);
        }

        /// <summary>
        /// Verifica se o cliente já foi associado a uma rota
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        public bool IsClienteAssociado(GDASession sessao, uint idCliente)
        {
            string sql = "Select Count(*) From rota_cliente Where idCliente=" + idCliente;

            return objPersistence.ExecuteSqlQueryCount(sessao, sql) > 0;
        }

        /// <summary>
        /// Verifica se o cliente já foi associado a rota passada
        /// </summary>
        /// <param name="idRota"></param>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        public bool IsClienteAssociado(uint idRota, uint idCliente)
        {
            string sql = "Select Count(*) From rota_cliente Where idRota=" + idRota + " And idCliente=" + idCliente;

            return objPersistence.ExecuteSqlQueryCount(sql) > 0;
        }
    }
}
