using System;
using System.Linq;
using Glass.Data.DAL;

namespace WebGlass.Business.EncontroContas.Fluxo
{
    public sealed class EncontroContas : BaseFluxo<EncontroContas>
    {
        private EncontroContas() { }

        /// <summary>
        /// Recupera o total a pagar a receber e o saldo de um encontro de contas
        /// </summary>
        /// <param name="idEncontroContas"></param>
        /// <param name="totalPagar"></param>
        /// <param name="totalReceber"></param>
        /// <param name="saldo"></param>
        public void GetTotais(uint idEncontroContas, ref decimal totalPagar, ref decimal totalReceber, ref decimal saldo)
        {
            totalPagar = EncontroContasDAO.Instance.ObtemTotalPagar(null, idEncontroContas);
            totalReceber = EncontroContasDAO.Instance.ObtemTotalReceber(null, idEncontroContas);

            if (totalPagar > totalReceber)
                saldo = totalPagar - totalReceber;
            else
                saldo = totalReceber - totalPagar;
        }

        /// <summary>
        /// Finaliza um encontro de contas
        /// </summary>
        /// <param name="idEncontroContas"></param>
        public string Finalizar(uint idEncontroContas, string dataVenc)
        {
            //Valida o encontro
            DateTime? dtVenc = Glass.Conversoes.StrParaDate(dataVenc);
            BuscarEValidar.Instance.ValidaEncontro(idEncontroContas, dtVenc);

            // Finaliza encontro
            int contaGerada = 0;
            decimal valorGerado = 0;
            EncontroContasDAO.Instance.FinalizaEncontro(idEncontroContas, dtVenc.Value, ref contaGerada, ref valorGerado);

            string msgRetorno = "Encontro de contas a pagar/receber finalizado com sucesso. ";

            if (contaGerada > 0)
                msgRetorno += "Foi gerado uma conta a " + (contaGerada == 1 ? "pagar" : "receber") + " no valor de " + valorGerado.ToString("C");

            return msgRetorno;
        }

        /// <summary>
        /// Retifica um encontro de contas
        /// </summary>
        public string Retificar(int idEncontroContas, string contasPg, string contasR, string dataVenc)
        {
            var idsContasPg = contasPg.Split(',').ToList();
            var idsContasR = contasR.Split(',').ToList();
            var contaGerada = 0;
            decimal valorGerado = 0;

            DateTime? dtVenc = Glass.Conversoes.StrParaDate(dataVenc);

            //Valida para retificar
            BuscarEValidar.Instance.ValidaEncontroRetificar((uint)idEncontroContas, contasPg, contasR, dtVenc);

            EncontroContasDAO.Instance.RetificaEncontro(idEncontroContas, idsContasPg, idsContasR, dtVenc.Value, ref contaGerada, ref valorGerado);

            string msgRetorno = "Encontro de contas a pagar/receber retificado com sucesso. ";

            if (contaGerada > 0)
                msgRetorno += "Foi gerado uma conta a " + (contaGerada == 1 ? "pagar" : "receber") + " no valor de " + valorGerado.ToString("C");

            return msgRetorno;
        }

        /// <summary>
        /// Cancela um encontro de contas
        /// </summary>
        public void Cancelar(uint idEncontroContas, string motivo)
        {
            //Valida para cancelamento
            BuscarEValidar.Instance.ValidaEncontroCancelamento(idEncontroContas);

            //Cancela o encontro
            EncontroContasDAO.Instance.CancelaEncontro(idEncontroContas, motivo);
        }
    }
}
