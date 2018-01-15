using System;
using System.Collections.Generic;
using Glass.Data.DAL;
using Glass.Data.Helper;

namespace WebGlass.Business.ConciliacaoBancaria.Fluxo
{
    public sealed class CRUD : BaseFluxo<CRUD>
    {
        private CRUD() { }

        #region Create

        public uint NovaConciliacaoBancaria(uint codigoContaBancaria, DateTime dataConciliacao)
        {
            ConciliacaoBancariaDAO.Instance.VerificaDataConciliacao(codigoContaBancaria, dataConciliacao);

            Entidade.ConciliacaoBancaria item = new Entidade.ConciliacaoBancaria()
            {
                CodigoContaBancaria = codigoContaBancaria,
                DataConciliada = dataConciliacao,
                DataCadastro = DateTime.Now,
                CodigoFuncionarioCadastro = UserInfo.GetUserInfo.CodUser,
                Situacao = Entidade.ConciliacaoBancaria.SituacaoEnum.Ativa
            };

            return ConciliacaoBancariaDAO.Instance.Insert(item._conciliacao);
        }

        #endregion

        #region Read

        public Entidade.ConciliacaoBancaria[] ObtemListaConciliacoesBancarias(uint codigoContaBancaria, string sortExpression, int startRow, int pageSize)
        {
            var conciliacoes = ConciliacaoBancariaDAO.Instance.ObtemListaConciliacoesBancarias(codigoContaBancaria ,sortExpression, startRow, pageSize);
            return Array.ConvertAll(conciliacoes, x => new Entidade.ConciliacaoBancaria(x));
        }

        public int ObtemNumeroConciliacoesBancarias(uint codigoContaBancaria)
        {
            return ConciliacaoBancariaDAO.Instance.ObtemNumeroConciliacoesBancarias(codigoContaBancaria);
        }

        public IList<Entidade.ConciliacaoBancaria> ObtemParaRelatorio(uint codigoConciliacaoBancaria)
        {
            var item = ConciliacaoBancariaDAO.Instance.ObtemElemento(codigoConciliacaoBancaria);
            
            var retorno = new List<Entidade.ConciliacaoBancaria>();
            if (item != null)
                retorno.Add(new Entidade.ConciliacaoBancaria(item));

            return retorno;
        }

        #endregion

        #region Delete

        public void CancelarConciliacao(uint codigo, string motivo)
        {
            CancelarConciliacao(codigo, motivo, true);
        }

        internal void CancelarConciliacao(uint codigo, string motivo, bool manual)
        {
            ConciliacaoBancariaDAO.Instance.Cancelar(codigo, motivo, manual);
        }

        #endregion
    }
}
