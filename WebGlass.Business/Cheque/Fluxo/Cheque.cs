using System;
using Glass.Data.DAL;
using Glass.Data.Model;
using Glass.Data.Helper;

namespace WebGlass.Business.Cheque.Fluxo
{
    public sealed class Cheque : BaseFluxo<Cheque>
    {
        private Cheque() { }

        public void ChequeInserted(uint idCheque, bool movCaixaGeral, bool movContaBanco,
            uint? idPlanoConta, uint? idContaBanco)
        {
            if (idPlanoConta.GetValueOrDefault() == 0)
                return;

            decimal valorCheque = ChequesDAO.Instance.ObtemValor(idCheque);

            if (movCaixaGeral)
            {
                CaixaGeralDAO.Instance.MovCxCheque(idCheque, null, null, idPlanoConta.Value, 2, 1, 
                    valorCheque, 0, null, true, DateTime.Now);
            }
            else if (movContaBanco && idContaBanco > 0)
            {
                ContaBancoDAO.Instance.MovContaCheque(null, idContaBanco.Value, idPlanoConta.Value, (int)UserInfo.GetUserInfo.IdLoja, null, 
                    idCheque, null, null, 1, valorCheque, DateTime.Now);

                ChequesDAO.Instance.UpdateSituacao(null, idCheque, Cheques.SituacaoCheque.Compensado);
            }
        }
    }
}
