using System;
using Glass.Data.DAL;

namespace WebGlass.Business.CustoFixo.Ajax
{
    public interface IRetificar
    {
        string RetificarCustoFixo(string idCustoFixo, string mesAno, string valor, string diaVenc);
    }

    internal class Retificar : IRetificar
    {
        public string RetificarCustoFixo(string idCustoFixo, string mesAno, string valor, string diaVenc)
        {
            try
            {
                CustoFixoDAO.Instance.RetificarCustoFixo(Glass.Conversoes.StrParaUint(idCustoFixo), mesAno, Glass.Conversoes.StrParaInt(diaVenc),
                    Glass.Conversoes.StrParaDecimal(valor));

                return "ok";
            }
            catch (Exception ex)
            {
                if (ex.Message == "Operação concluída.")
                    return "ok";

                return "Erro\t" + ex.Message.Replace("'", "");
            }
        }
    }
}
