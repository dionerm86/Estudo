using Glass.Data.DAL;
using Glass.Data.Model;

namespace Glass.Data.CFeUtils
{
    public static class ConfigCFe
    {
        #region Propriedades

        public static int CstPisCofins(uint idLoja)
        {
            int crt = LojaDAO.Instance.BuscaCrtLoja(null, idLoja);

            return crt == (int)CrtLoja.LucroPresumido || crt == (int)CrtLoja.LucroReal ? 1 : 8;
        }

        #endregion
    }
}