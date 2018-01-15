using System.Collections.Generic;
using Glass.Data.DAL;

namespace WebGlass.Business.ArquivoRemessa.Fluxo
{
    public class RegistroArquivoRemessaFluxo : BaseFluxo<RegistroArquivoRemessaFluxo>
    {
        #region Contrutor

        private RegistroArquivoRemessaFluxo() { }

        #endregion

        #region Recupera a lista de contas a receber\recebidas

        public IList<Glass.Data.Model.ContasReceber> GetListWithExpression(uint idContaR, uint idPedido, uint idLiberarPedido, uint idAcerto, uint idAcertoParcial,
            uint idTrocaDevolucao, uint numNFe, uint idCli, string nomeCli, uint idLoja, bool lojaCliente, uint numArqRemessa, uint idFormaPagto,
            string obs, string dtIniVenc, string dtFimVenc, string dtIniRec, string dtFimRec, decimal valorVecIni, decimal valorVecFim,
            decimal valorRecIni, decimal valorRecFim, int recebida, int codOcorrencia, string nossoNumero, string numDoc, string usoEmpresa, int idContaBanco,
            string sortExpression, int startRow, int pageSize)
        {
            return Glass.Data.DAL.ContasReceberDAO.Instance.GetListRegistroArquivoRemessa(idContaR, idPedido, idLiberarPedido, idAcerto, idAcertoParcial,
                idTrocaDevolucao, numNFe, idCli, nomeCli, idLoja, lojaCliente, numArqRemessa, idFormaPagto, obs, dtIniVenc, dtFimVenc, dtIniRec,
                dtFimRec, valorVecIni, valorVecFim, valorRecIni, valorRecFim, recebida, codOcorrencia, nossoNumero, numDoc, usoEmpresa, idContaBanco,
                sortExpression, startRow, pageSize);
        }

        public int GetListWithExpressionCount(uint idContaR, uint idPedido, uint idLiberarPedido, uint idAcerto, uint idAcertoParcial,
            uint idTrocaDevolucao, uint numNFe, uint idCli, string nomeCli, uint idLoja, bool lojaCliente, uint numArqRemessa, uint idFormaPagto,
            string obs, string dtIniVenc, string dtFimVenc, string dtIniRec, string dtFimRec, decimal valorVecIni, decimal valorVecFim,
            decimal valorRecIni, decimal valorRecFim, int recebida, int codOcorrencia, string nossoNumero, string numDoc, string usoEmpresa, int idContaBanco)
        {
            return Glass.Data.DAL.ContasReceberDAO.Instance.GetListRegistroArquivoRemessaCount(idContaR, idPedido, idLiberarPedido, idAcerto, idAcertoParcial,
                idTrocaDevolucao, numNFe, idCli, nomeCli, idLoja, lojaCliente, numArqRemessa, idFormaPagto, obs, dtIniVenc, dtFimVenc, dtIniRec,
                dtFimRec, valorVecIni, valorVecFim, valorRecIni, valorRecFim, recebida, codOcorrencia, nossoNumero, numDoc, usoEmpresa, idContaBanco);
        }

        public IList<Glass.Data.Model.ContasReceber> GetListForRpt(uint idContaR, uint idPedido, uint idLiberarPedido, uint idAcerto, uint idAcertoParcial,
            uint idTrocaDevolucao, uint numNFe, uint idCli, string nomeCli, uint idLoja, bool lojaCliente, uint numArqRemessa, uint idFormaPagto,
            string obs, string dtIniVenc, string dtFimVenc, string dtIniRec, string dtFimRec, decimal valorVecIni, decimal valorVecFim,
            decimal valorRecIni, decimal valorRecFim, int recebida, int codOcorrencia, string nossoNumero, string numDoc, string usoEmpresa, int idContaBanco)
        {
            return Glass.Data.DAL.ContasReceberDAO.Instance.GetListRegistroArquivoRemessaForRpt(idContaR, idPedido, idLiberarPedido, idAcerto, idAcertoParcial,
                idTrocaDevolucao, numNFe, idCli, nomeCli, idLoja, lojaCliente, numArqRemessa, idFormaPagto, obs, dtIniVenc, dtFimVenc, dtIniRec,
                dtFimRec, valorVecIni, valorVecFim, valorRecIni, valorRecFim, recebida, codOcorrencia, nossoNumero, numDoc, usoEmpresa, idContaBanco);
        }

        #endregion

        #region Recupera a lista de registro de importações de uma conta a receber \ recebida

        public IList<Glass.Data.Model.RegistroArquivoRemessa> GetListRegistros(uint idContaR)
        {
            return RegistroArquivoRemessaDAO.Instance.GetListRegistros(idContaR);
        }

        public IList<Glass.Data.Model.RegistroArquivoRemessa> GetListRegistros(string idsContasR)
        {
            return RegistroArquivoRemessaDAO.Instance.GetListRegistros(idsContasR);
        }

        #endregion
    }
}
