using System;

namespace Glass.Data.SIntegra
{
    internal abstract class Registro
    {
        #region Propriedades

        public abstract int Tipo { get; }

        protected string NotNullString(string toFormat)
        {
            return toFormat != null ? toFormat : "";
        }

        protected string FormatCpfCnpjInscEst(string toFormat)
        {
            return NotNullString(toFormat).Replace(".", "").Replace("-", "").Replace("/", "");
        }

        protected string FormatTelefone(string toFormat)
        {
            return toFormat.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
        }

        protected string FormatData(DateTime toFormat)
        {
            return toFormat.ToString("yyyyMMdd");
        }

        #endregion
    }
}