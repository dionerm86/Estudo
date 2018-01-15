using System;
using Glass.Data.Helper;

namespace Glass.Data.EFD
{
    public sealed class EFD : Glass.Pool.PoolableObject<EFD>
    {
        private EFD() { }

        /// <summary>
        /// Retorna os registros do EFD Fiscal (ICMS/IPI) da loja e período especificados.
        /// </summary>
        /// <param name="idLoja"></param>
        /// <param name="idContabilista">Contabilista responsável pela geração do arquivo</param>
        /// <param name="inicio"></param>
        /// <param name="fim"></param>
        /// <returns></returns>
        public string RecuperaRegistrosFiscal(bool arquivoRetificador, string numReciboOriginal, uint idLoja,
            uint idContabilista, DateTime inicio, DateTime fim, DateTime[] intervalos, bool gerarBlocoH, string aidf, bool gerarBlocoK)
        {
            return Sync.Fiscal.EFD.Gerar.Instance.RecuperaRegistrosFiscal(RecuperaDados.Instance, RecuperaConfiguracao.Instance, arquivoRetificador,
                numReciboOriginal, (int)idLoja, (int)idContabilista, inicio, fim, intervalos, gerarBlocoH, aidf, gerarBlocoK);
        }

        /// <summary>
        /// Retorna os registros do EFD Contribuições (PIS/COFINS) das lojas e período especificados.
        /// </summary>
        /// <param name="idLoja"></param>
        /// <param name="idContabilista">Contabilista responsável pela geração do arquivo</param>
        /// <param name="inicio"></param>
        /// <param name="fim"></param>
        /// <returns></returns>
        public string RecuperaRegistrosContribuicoes(bool arquivoRetificador, string numReciboOriginal, string idsLojas,
            uint idContabilista, DateTime inicio, DateTime fim, int codIncTrib, int? indAproCred, int codTipoCont)
        {
            return Sync.Fiscal.EFD.Gerar.Instance.RecuperaRegistrosContribuicoes(RecuperaDados.Instance, RecuperaConfiguracao.Instance, arquivoRetificador,
                numReciboOriginal, idsLojas, (int)idContabilista, inicio, fim, codIncTrib, indAproCred, codTipoCont);
        }

        public string RecuperaRegistrosFCI(uint idArquivoFci)
        {
            return Sync.Fiscal.EFD.Gerar.Instance.RecuperaRegistrosFCI(RecuperaDados.Instance, (int)idArquivoFci, (int)UserInfo.GetUserInfo.IdLoja);
        }
    }
}