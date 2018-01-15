namespace Glass
{
    public static class Calculos
    {
        #region Habilita altura/largura de acordo com o tipo de cálculo

        public static bool AlturaEnabled(int tipoCalc)
        {
            return tipoCalc != 1 && tipoCalc != 5;
        }

        public static bool LarguraEnabled(int tipoCalc)
        {
            return tipoCalc != 1 && tipoCalc != 4 && tipoCalc != 5 && tipoCalc != 6 && tipoCalc != 7 && tipoCalc != 8 && tipoCalc != 9;
        }

        #endregion
    }
}
