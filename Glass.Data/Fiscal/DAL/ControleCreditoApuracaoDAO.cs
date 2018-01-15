using Glass.Data.Model;
using Glass.Data.EFD;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class ControleCreditoApuracaoDAO : BaseDAO<ControleCreditoApuracao, ControleCreditoApuracaoDAO>
    {
        //private ControleCreditoApuracaoDAO() { }

        public ControleCreditoApuracao ObterCreditoMesAnterior(string periodo, DataSourcesEFD.TipoImpostoEnum tipoImposto)
        {
            try
            {
                string sql = "SELECT * FROM controle_credito_apuracao WHERE PeriodoGeracao=?periodo AND TipoImposto = ?tipoImposto";

                GDAParameter[] param = new GDAParameter[] { new GDAParameter("?periodo", periodo), new GDAParameter("?tipoImposto", tipoImposto) };

                return objPersistence.LoadOneData(sql, param);
            }
            catch
            {
                return null;
            }
        }

        public void InserirCredito(string periodo, DataSourcesEFD.TipoImpostoEnum tipoImposto, decimal valor)
        {
            uint? idCreditoAtual = ObtemValorCampo<uint?>("idCredito", "periodoGeracao=?periodo " + 
                                    "and tipoImposto=" + (int)tipoImposto, new GDAParameter("?periodo", periodo));


            ControleCreditoApuracao novo = new ControleCreditoApuracao();
            novo.PeriodoGeracao = periodo;
            novo.TipoImposto = (int)tipoImposto;
            novo.ValorGerado = valor;

            if (idCreditoAtual == 0 || idCreditoAtual == null)
            {
                uint idNovoCredito = Insert(novo);
            }
            else
            {
                novo.IdCredito = (uint)idCreditoAtual;
                Update(novo);
            }
        }
    }
}
