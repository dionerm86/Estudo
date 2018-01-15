using Glass.Data.DAL;
using Glass.Data.Model;
using Glass.Data.Helper;
using System;
using System.Collections.Generic;

namespace Glass.Data.DAL
{
    public class AvaliacaoAtendimentoDAO : BaseDAO<AvaliacaoAtendimento, AvaliacaoAtendimentoDAO>
    {
        public List<AvaliacaoAtendimento> GetList()
        {
            var sql = string.Format("Select * From avaliacao_atendimento Where Coalesce(Avaliacao, 0)=0 And IdFunc={0} Order By IdAvaliacaoAtendimento Desc", UserInfo.GetUserInfo.CodUser);

            return objPersistence.LoadData(sql);
        }

        /// <summary>
        /// Verifica se há alguma avaliação a ser feita pelo usuário logado
        /// </summary>
        /// <returns></returns>
        public bool PossuiAvaliacaoPendente()
        {
            if (UserInfo.GetUserInfo == null)
                return false;

            return ExecuteScalar<bool>("Select Count(*)>0 From avaliacao_atendimento Where Coalesce(Avaliacao, 0)=0 And IdFunc=" + UserInfo.GetUserInfo.CodUser);
        }

        /// <summary>
        /// Aprova o atendimento
        /// </summary>
        /// <param name="idAvaliacaoAtendimento"></param>
        public void AvaliaAtendimento(uint idAvaliacaoAtendimento, AvaliacaoAtendimento.SatisfacaoEnum satisfacao, string obs, bool aprovado)
        {
            if ((int)satisfacao == 0)
                throw new Exception("Informe a satisfação para avaliar o chamado!");

            if (!aprovado && string.IsNullOrWhiteSpace(obs))
                throw new Exception("A Obs é de preenchimento obrigatório ao negar o chamado!");

            var avaliacao = GetElementByPrimaryKey(idAvaliacaoAtendimento);
            avaliacao.Avaliacao = aprovado ? 1 : 2;
            avaliacao.Satisfacao = satisfacao;
            avaliacao.Obs = obs;
            avaliacao.DataAvaliacao = DateTime.Now;
            Update(avaliacao);
        }
    }
}
