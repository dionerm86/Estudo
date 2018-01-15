using System;
using Glass.Data.Model;
using GDA;
using Glass.Data.EFD;

namespace Glass.Data.DAL
{
    public sealed class UsoCreditoEfdDAO : BaseDAO<UsoCreditoEfd, UsoCreditoEfdDAO>
    {
        //private UsoCreditoEfdDAO() { }

        public void AtualizaCreditoUsado(uint idCreditoAtual, uint idCreditoNovo)
        {
            objPersistence.ExecuteCommand("update uso_credito_efd set idCredito=" + idCreditoNovo + 
                " where idCredito=" + idCreditoAtual);
        }

        public void DeleteByCredito(uint idCredito)
        {
            objPersistence.ExecuteCommand("delete from uso_credito_efd where idCredito=" + idCredito);
        }

        public void InserirCreditoUsado(uint idLoja, DateTime periodo, DataSourcesEFD.TipoImpostoEnum tipoImposto, 
            DataSourcesEFD.CodCredEnum? codCred, decimal valorUsado)
        {
            foreach (ControleCreditoEfd c in ControleCreditoEfdDAO.Instance.GetForEFD(idLoja, periodo, 
                tipoImposto, codCred).Value)
            {
                if (valorUsado <= 0) break;
                decimal valorCredito = Math.Min(valorUsado, c.ValorRestanteCredito);
                valorUsado -= valorCredito;

                objPersistence.ExecuteCommand("delete from uso_credito_efd where idCredito=" + c.IdCredito +
                    " and periodoUso=?pu", new GDAParameter("?pu", periodo.ToString("MM/yyyy")));

                UsoCreditoEfd u = new UsoCreditoEfd()
                {
                    IdCredito = c.IdCredito,
                    PeriodoUso = periodo.ToString("MM/yyyy"),
                    ValorUsado = valorCredito
                };

                Insert(u);
            }
        }
    }
}
