using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass
{
    public static class Validacoes
    {
        #region Valida CPF

        public static bool ValidaCpf(string cpf)
        {
            int[] multiplicador1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            string tempCpf;
            string digito;
            int soma;
            int resto;

            if (cpf == null)
                return false;

            cpf = cpf.Trim();
            cpf = cpf.Replace(".", "").Replace("-", "");

            if (cpf.Length != 11)
                return false;

            tempCpf = cpf.Substring(0, 9);
            soma = 0;
            for (int i = 0; i < 9; i++)
                soma += Glass.Conversoes.ConverteValor<int>(tempCpf[i].ToString()) * multiplicador1[i];

            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            digito = resto.ToString();

            tempCpf = tempCpf + digito;

            soma = 0;
            for (int i = 0; i < 10; i++)
                soma += Glass.Conversoes.ConverteValor<int>(tempCpf[i].ToString()) * multiplicador2[i];

            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            digito = digito + resto.ToString();

            return cpf.EndsWith(digito);
        }

        #endregion

        #region Valida CNPJ

        /// <summary>
        /// Verifica se o CNPJ é válido
        /// </summary>
        /// <param name="CNPJ"></param>
        /// <returns></returns>
        public static bool ValidaCnpj(String cnpj)
        {
            String numString = "";
            int soma = 0;
            int mult = 5;
            int resultado = 0;

            if (cnpj == null)
                return false;

            cnpj = cnpj.Trim();
            cnpj = cnpj.Replace(".", "").Replace("-", "").Replace("/", "");

            if (cnpj.Length != 14)
                return false;

            for (int i = 0; i < 12; i++)
            {
                numString = (cnpj.Substring(i, 1));
                soma += (Int32.Parse(numString) * mult--);
                if (mult == 1)
                    mult = 9;
            }

            resultado = (11 - (soma % 11));

            if (resultado >= 10)
                resultado = 0;

            int rest1 = resultado;
            soma = 0;
            mult = 6;

            for (int i = 0; i < 13; i++)
            {
                numString = (cnpj.Substring(i, 1));
                soma += (Int32.Parse(numString) * mult--);
                if (mult == 1)
                    mult = 9;
            }

            resultado = (11 - (soma % 11));

            if (resultado >= 10)
                resultado = 0;

            int rest2 = resultado;

            //Os dois ultimos digitos são verificados
            int digito1 = Int32.Parse(cnpj.Substring(12, 1));
            int digito2 = Int32.Parse(cnpj.Substring(13, 1));

            if (rest1 == digito1 && rest2 == digito2)
                return true;
            else
                return false;
        }

        #endregion

        #region Valida Inscrição Estadual

        public static bool ValidaIE(string pUF, string pInscr)
        {
            const string CARACTERES_VALIDOS = "0123456789P";

            bool retorno = false;
            string strBase;
            string strBase2;
            string strOrigem;
            string strDigito1;
            string strDigito2;
            int pos;
            int intValor;
            int soma = 0;
            int resto;
            int intNumero;
            int peso;
            int intDig;
            int dig1 = 0;
            int dig2 = 0;
            strBase = "";
            strBase2 = "";
            strOrigem = "";

            if (String.IsNullOrEmpty(pInscr))
                return false;

            if (String.IsNullOrEmpty(pUF))
                return false;

            if (pInscr.Length < 6)
                return false;

            if (pInscr.Trim().Substring(0, 5).ToUpper() == "ISENT")
                return true;

            // Retira caracteres especiais
            pInscr = pInscr.Replace(".", "").Replace("-", "");

            foreach (char c in pInscr)
                if (!CARACTERES_VALIDOS.Contains(c.ToString().ToUpper()))
                    return false;

            for (pos = 1; pos <= pInscr.Trim().Length; pos++)
                if (((CARACTERES_VALIDOS.IndexOf(pInscr.Substring((pos - 1), 1), 0, System.StringComparison.OrdinalIgnoreCase) + 1) > 0))
                    strOrigem = (strOrigem + pInscr.Substring((pos - 1), 1));

            switch (pUF.ToUpper())
            {
                case "AC":
                    strBase = strOrigem.PadLeft(13, '0').Substring(0, 11);

                    if (strBase.Substring(0, 2) == "01")
                    {
                        soma = 0;
                        intValor = 4;

                        for (pos = 0; pos <= 10; pos++)
                        {
                            soma += Glass.Conversoes.ConverteValor<int>(strBase[pos].ToString()) * intValor--;

                            if (intValor == 1)
                                intValor = 9;
                        }

                        dig1 = 11 - (soma % 11);
                        if (dig1 >= 10) dig1 = 0;
                        strBase += dig1;

                        soma = 0;
                        intValor = 5;

                        for (pos = 0; pos <= 11; pos++)
                        {
                            soma += Glass.Conversoes.ConverteValor<int>(strBase[pos].ToString()) * intValor--;

                            if (intValor == 1)
                                intValor = 9;
                        }

                        dig2 = 11 - (soma % 11);
                        if (dig2 >= 10) { dig2 = 0; }
                        strBase += dig2;

                        retorno = strOrigem == (strBase.Substring(0, 11) + dig1.ToString() + dig2.ToString());
                    }
                    break;
                case "AL":
                    strBase = (strOrigem.Trim() + "000000000").Substring(0, 9);

                    if ((strBase.Substring(0, 2) == "24"))
                    {
                        soma = 0;

                        for (pos = 1; (pos <= 8); pos++)
                        {
                            intValor = Glass.Conversoes.ConverteValor<int>(strBase.Substring((pos - 1), 1));
                            intValor = (intValor * (10 - pos));
                            soma = (soma + intValor);
                        }

                        soma = (soma * 10);
                        resto = (soma % 11);
                        strDigito1 = ((resto == 10) ? "0" : Convert.ToString(resto)).Substring((((resto == 10) ? "0" : Convert.ToString(resto)).Length - 1));
                        strBase2 = (strBase.Substring(0, 8) + strDigito1);

                        if ((strBase2 == strOrigem))
                            retorno = true;
                    }

                    break;

                case "AM":
                    strBase = (strOrigem.Trim() + "000000000").Substring(0, 9);
                    soma = 0;

                    for (pos = 1; (pos <= 8); pos++)
                    {
                        intValor = Glass.Conversoes.ConverteValor<int>(strBase.Substring((pos - 1), 1));
                        intValor = (intValor * (10 - pos));
                        soma = (soma + intValor);
                    }

                    if ((soma < 11))
                        strDigito1 = Convert.ToString((11 - soma)).Substring((Convert.ToString((11 - soma)).Length - 1));
                    else
                    {
                        resto = (soma % 11);
                        strDigito1 = ((resto < 2) ? "0" : Convert.ToString((11 - resto))).Substring((((resto < 2) ? "0" : Convert.ToString((11 - resto))).Length - 1));
                    }

                    strBase2 = (strBase.Substring(0, 8) + strDigito1);

                    if ((strBase2 == strOrigem))
                        retorno = true;

                    break;
                case "AP":
                    strBase = (strOrigem.Trim() + "000000000").Substring(0, 9);
                    peso = 0;
                    intDig = 0;

                    if ((strBase.Substring(0, 2) == "03"))
                    {
                        intNumero = Glass.Conversoes.ConverteValor<int>(strBase.Substring(0, 8));

                        if (((intNumero >= 3000001) && (intNumero <= 3017000)))
                        {
                            peso = 5;
                            intDig = 0;
                        }
                        else if (((intNumero >= 3017001) && (intNumero <= 3019022)))
                        {
                            peso = 9;
                            intDig = 1;
                        }
                        else if ((intNumero >= 3019023))
                        {
                            peso = 0;
                            intDig = 0;
                        }

                        soma = peso;

                        for (pos = 1; (pos <= 8); pos++)
                        {
                            intValor = Glass.Conversoes.ConverteValor<int>(strBase.Substring((pos - 1), 1));
                            intValor = (intValor * (10 - pos));
                            soma = (soma + intValor);
                        }

                        resto = (soma % 11);
                        intValor = (11 - resto);

                        if ((intValor == 10))
                            intValor = 0;
                        else if ((intValor == 11))
                            intValor = intDig;

                        strDigito1 = Convert.ToString(intValor).Substring((Convert.ToString(intValor).Length - 1));
                        strBase2 = (strBase.Substring(0, 8) + strDigito1);
                        retorno = strBase2 == strOrigem;
                    }
                    break;

                case "BA":
                    if (strOrigem.Length != 9)
                        return false;

                    strBase = strOrigem.Trim();
                    if ("0123458".Contains(strBase.Substring(1, 1)))
                    {
                        soma = 0;

                        for (pos = 1; pos <= 7; pos++)
                        {
                            intValor = Glass.Conversoes.ConverteValor<int>(strBase.Substring(pos - 1, 1));
                            intValor = (intValor * (9 - pos));
                            soma += intValor;
                        }

                        resto = (soma % 10);
                        strDigito2 = (resto == 0 ? "0" : (10 - resto).ToString()).Substring(((resto == 0 ? "0" : (10 - resto).ToString()).Length - 1));
                        strBase2 = (strBase.Substring(0, 7) + strDigito2);
                        soma = 0;

                        for (pos = 1; pos <= 8; pos++)
                        {
                            intValor = Glass.Conversoes.ConverteValor<int>(strBase2.Substring(pos - 1, 1));
                            intValor = (intValor * (10 - pos));
                            soma += intValor;
                        }

                        resto = (soma % 10);
                        strDigito1 = ((resto == 0) ? "0" : Convert.ToString((10 - resto))).Substring((((resto == 0) ? "0" : Convert.ToString((10 - resto))).Length - 1));
                    }
                    else
                    {
                        soma = 0;

                        for (pos = 1; pos <= 7; pos++)
                        {
                            intValor = Glass.Conversoes.ConverteValor<int>(strBase.Substring((pos - 1), 1));
                            intValor = (intValor * (9 - pos));
                            soma += intValor;
                        }

                        resto = soma % 11;
                        strDigito2 = (resto < 2 ? "0" : Convert.ToString(11 - resto)).Substring(((resto < 2 ? "0" : Convert.ToString(11 - resto)).Length - 1));
                        strBase2 = (strBase.Substring(0, 7) + strDigito2);
                        soma = 0;

                        for (pos = 1; pos <= 8; pos++)
                        {
                            intValor = Glass.Conversoes.ConverteValor<int>(strBase2.Substring(pos - 1, 1));
                            intValor = (intValor * (10 - pos));
                            soma += intValor;
                        }

                        resto = (soma % 11);
                        strDigito1 = (resto < 2 ? "0" : Convert.ToString(11 - resto)).Substring(((resto < 2 ? "0" : Convert.ToString(11 - resto)).Length - 1));
                    }

                    strBase2 = (strBase.Substring(0, 7) + (strDigito1 + strDigito2));
                    retorno = strBase2 == strOrigem;
                    break;

                case "CE":
                    strBase = (strOrigem.Trim() + "000000000").Substring(0, 9);
                    soma = 0;

                    for (pos = 1; (pos <= 8); pos++)
                    {
                        intValor = Glass.Conversoes.ConverteValor<int>(strBase.Substring((pos - 1), 1));
                        intValor = (intValor * (10 - pos));
                        soma += intValor;
                    }

                    resto = (soma % 11);
                    intValor = (11 - resto);

                    if ((intValor > 9))
                        intValor = 0;

                    strDigito1 = Convert.ToString(intValor).Substring((Convert.ToString(intValor).Length - 1));
                    strBase2 = (strBase.Substring(0, 8) + strDigito1);
                    retorno = strBase2 == strOrigem;
                    break;

                case "DF":
                    strBase = (strOrigem.Trim() + "0000000000000").Substring(0, 13);
                    soma = 0;
                    peso = 4;

                    for (pos = 0; pos < 11; pos++)
                    {
                        soma += Glass.Conversoes.ConverteValor<int>(strBase[pos].ToString()) * peso--;

                        if (peso == 1)
                            peso = 9;
                    }

                    resto = soma % 11;
                    strDigito1 = (resto < 2 ? "0" : Convert.ToString((11 - resto))).Substring((((resto < 2) ? "0" : Convert.ToString((11 - resto))).Length - 1));
                    strBase2 = (strBase.Substring(0, 11) + strDigito1);
                    soma = 0;
                    peso = 5;

                    for (pos = 0; pos < 12; pos++)
                    {
                        soma += Glass.Conversoes.ConverteValor<int>(strBase[pos].ToString()) * peso--;

                        if (peso == 1)
                            peso = 9;
                    }

                    resto = (soma % 11);
                    strDigito2 = ((resto < 2) ? "0" : Convert.ToString((11 - resto))).Substring((((resto < 2) ? "0" : Convert.ToString((11 - resto))).Length - 1));
                    strBase2 = (strBase.Substring(0, 12) + strDigito2);
                    retorno = strBase2 == strOrigem;
                    break;

                case "ES":
                    strBase = (strOrigem.Trim() + "000000000").Substring(0, 9);
                    soma = 0;

                    for (pos = 1; (pos <= 8); pos++)
                    {
                        intValor = Glass.Conversoes.ConverteValor<int>(strBase.Substring((pos - 1), 1));
                        intValor = (intValor * (10 - pos));
                        soma = (soma + intValor);
                    }

                    resto = (soma % 11);
                    strDigito1 = ((resto < 2) ? "0" : Convert.ToString((11 - resto))).Substring((((resto < 2) ? "0" : Convert.ToString((11 - resto))).Length - 1));
                    strBase2 = (strBase.Substring(0, 8) + strDigito1);
                    retorno = strBase2 == strOrigem;
                    break;

                case "GO":
                    strBase = (strOrigem.Trim() + "000000000").Substring(0, 9);

                    if ((("10,11,15".IndexOf(strBase.Substring(0, 2), 0, System.StringComparison.OrdinalIgnoreCase) + 1) > 0))
                    {
                        soma = 0;

                        for (pos = 1; (pos <= 8); pos++)
                        {
                            intValor = Glass.Conversoes.ConverteValor<int>(strBase.Substring((pos - 1), 1));
                            intValor = intValor * (10 - pos);
                            soma += intValor;
                        }

                        resto = (soma % 11);

                        if ((resto == 0))
                            strDigito1 = "0";

                        else if ((resto == 1))
                        {
                            intNumero = Glass.Conversoes.ConverteValor<int>(strBase.Substring(0, 8));

                            strDigito1 = (((intNumero >= 10103105) &&
                                (intNumero <= 10119997)) ? "1" : "0").Substring(((((intNumero >= 10103105) &&
                                (intNumero <= 10119997)) ? "1" : "0").Length - 1));

                        }
                        else
                            strDigito1 = Convert.ToString((11 - resto)).Substring((Convert.ToString((11 - resto)).Length - 1));

                        strBase2 = (strBase.Substring(0, 8) + strDigito1);
                        retorno = strBase2 == strOrigem;
                    }
                    break;

                case "MA":
                    strBase = (strOrigem.Trim() + "000000000").Substring(0, 9);

                    if (strBase.Substring(0, 2) == "12")
                    {
                        soma = 0;

                        for (pos = 1; pos <= 8; pos++)
                        {
                            intValor = Glass.Conversoes.ConverteValor<int>(strBase.Substring((pos - 1), 1));
                            intValor = (intValor * (10 - pos));
                            soma = (soma + intValor);
                        }

                        resto = (soma % 11);
                        strDigito1 = ((resto < 2) ? "0" : Convert.ToString((11 - resto))).Substring((((resto < 2) ? "0" : Convert.ToString((11 - resto))).Length - 1));
                        strBase2 = (strBase.Substring(0, 8) + strDigito1);
                        retorno = strBase2 == strOrigem;
                    }

                    break;

                case "MT":
                    strBase = strOrigem.Trim().PadLeft(11, '0');
                    soma = 0;
                    peso = 2;

                    for (pos = 10; pos >= 1; pos--)
                    {
                        soma += Glass.Conversoes.ConverteValor<int>(strBase.Substring((pos - 1), 1)) * peso++;

                        if ((peso > 9))
                            peso = 2;
                    }

                    resto = (soma % 11);
                    strDigito1 = ((resto < 2) ? "0" : Convert.ToString((11 - resto))).Substring((((resto < 2) ? "0" : Convert.ToString((11 - resto))).Length - 1));
                    retorno = strBase.Substring(10, 1) == strDigito1;
                    break;

                case "MS":
                    strBase = (strOrigem.Trim() + "000000000").Substring(0, 9);

                    if (strBase.Substring(0, 2) == "28")
                    {
                        soma = 0;
                        peso = 9;

                        for (pos = 0; pos < 8; pos++)
                            soma += Glass.Conversoes.ConverteValor<int>(strBase[pos].ToString()) * peso--;

                        resto = soma % 11;
                        dig1 = (resto == 0 || 11 - resto > 9) ? 0 : 11 - resto;

                        retorno = strOrigem.EndsWith(dig1.ToString());
                    }

                    break;

                case "MG":
                    if (strOrigem.Trim().Substring(0, 2) == "PR")
                        return true;
                    if (strOrigem.Trim().Substring(0, 5) == "ISENT")
                        return true;
                    if (strOrigem.Trim().Length != 13)
                        return false;

                    strBase2 = (strOrigem.Substring(0, 3) + ("0" + strOrigem.Substring(3, 8)));
                    intNumero = 1;
                    soma = 0;

                    for (pos = 0; pos < 12; pos++)
                    {
                        intValor = Glass.Conversoes.ConverteValor<int>(strBase2[pos].ToString()) * intNumero;
                        intNumero = intNumero == 2 ? 1 : 2;

                        if (intValor > 9)
                        {
                            strDigito1 = intValor.ToString();
                            intValor = Glass.Conversoes.ConverteValor<int>(strDigito1[0].ToString()) + Glass.Conversoes.ConverteValor<int>(strDigito1[1].ToString());
                        }

                        soma += intValor;
                    }

                    int dezenaAcima = 10;

                    while ((dezenaAcima - soma) < 0)
                        dezenaAcima += 10;

                    // Obtém o dígito 1
                    dig1 = dezenaAcima - soma;

                    strBase2 = (strOrigem.Substring(0, 11) + dig1);
                    soma = 0;
                    peso = 3;

                    for (pos = 0; pos < 12; pos++)
                    {
                        soma += Glass.Conversoes.ConverteValor<int>(strBase2[pos].ToString()) * peso--;

                        if (peso == 1)
                            peso = 11;
                    }

                    dig2 = 11 - (soma % 11);
                    if (dig2 >= 10) dig2 = 0;

                    retorno = (strBase2 + dig2.ToString()) == strOrigem;
                    break;

                case "PA":
                    strBase = (strOrigem.Trim() + "000000000").Substring(0, 9);

                    if ((strBase.Substring(0, 2) == "15"))
                    {
                        soma = 0;

                        for (pos = 1; (pos <= 8); pos++)
                        {
                            intValor = Glass.Conversoes.ConverteValor<int>(strBase.Substring((pos - 1), 1));
                            intValor = (intValor * (10 - pos));
                            soma += intValor;
                        }

                        resto = (soma % 11);
                        strDigito1 = ((resto < 2) ? "0" : Convert.ToString((11 - resto))).Substring((((resto < 2) ? "0" : Convert.ToString((11 - resto))).Length - 1));
                        strBase2 = (strBase.Substring(0, 8) + strDigito1);
                        retorno = strBase2 == strOrigem;
                    }
                    break;

                case "PB":
                    strBase = (strOrigem.Trim() + "000000000").Substring(0, 9);
                    soma = 0;

                    for (pos = 1; (pos <= 8); pos++)
                    {
                        intValor = Glass.Conversoes.ConverteValor<int>(strBase.Substring((pos - 1), 1));
                        intValor = (intValor * (10 - pos));
                        soma += intValor;
                    }

                    resto = (soma % 11);
                    intValor = (11 - resto);

                    if ((intValor > 9))
                        intValor = 0;

                    strDigito1 = Convert.ToString(intValor).Substring((Convert.ToString(intValor).Length - 1));
                    strBase2 = (strBase.Substring(0, 8) + strDigito1);
                    retorno = strBase2 == strOrigem;
                    break;

                case "PE":
                    strBase = strOrigem;

                    if (strBase.Length == 9)
                    {
                        soma = 0;
                        peso = 8;

                        for (pos = 0; pos < 7; pos++)
                            soma += Glass.Conversoes.ConverteValor<int>(strBase[pos].ToString()) * peso--;

                        resto = soma % 11;
                        dig1 = (resto == 1 || resto == 0) ? 0 : 11 - resto;

                        soma = 0;
                        peso = 9;

                        for (pos = 0; pos < 8; pos++)
                            soma += Glass.Conversoes.ConverteValor<int>(strBase[pos].ToString()) * peso--;

                        resto = soma % 11;
                        dig2 = (resto == 1 || resto == 0) ? 0 : 11 - resto;

                        retorno = strOrigem.EndsWith(dig1.ToString() + dig2.ToString());
                    }
                    break;

                case "PI":
                    strBase = (strOrigem.Trim() + "000000000").Substring(0, 9);
                    soma = 0;

                    for (pos = 1; (pos <= 8); pos++)
                    {
                        intValor = Glass.Conversoes.ConverteValor<int>(strBase.Substring((pos - 1), 1));
                        intValor = (intValor * (10 - pos));
                        soma += intValor;
                    }

                    resto = (soma % 11);
                    strDigito1 = ((resto < 2) ? "0" : Convert.ToString((11 - resto))).Substring((((resto < 2) ? "0" : Convert.ToString((11 - resto))).Length - 1));
                    strBase2 = (strBase.Substring(0, 8) + strDigito1);
                    retorno = strBase2 == strOrigem;
                    break;

                case "PR":
                    strBase = (strOrigem.Trim() + "0000000000").Substring(0, 10);
                    soma = 0;
                    peso = 2;

                    for (pos = 8; (pos >= 1); pos--)
                    {
                        intValor = Glass.Conversoes.ConverteValor<int>(strBase.Substring((pos - 1), 1));
                        intValor = (intValor * peso);
                        soma = (soma + intValor);
                        peso = (peso + 1);

                        if ((peso > 7))
                            peso = 2;
                    }

                    resto = (soma % 11);
                    strDigito1 = ((resto < 2) ? "0" : Convert.ToString((11 - resto))).Substring((((resto < 2) ? "0" : Convert.ToString((11 - resto))).Length - 1));
                    strBase2 = (strBase.Substring(0, 8) + strDigito1);
                    soma = 0;
                    peso = 2;

                    for (pos = 9; pos >= 1; pos--)
                    {
                        intValor = Glass.Conversoes.ConverteValor<int>(strBase2.Substring((pos - 1), 1)) * peso;
                        soma += intValor;
                        peso = (peso + 1);

                        if ((peso > 7))
                            peso = 2;
                    }

                    resto = (soma % 11);
                    strDigito2 = ((resto < 2) ? "0" : Convert.ToString((11 - resto))).Substring((((resto < 2) ? "0" : Convert.ToString((11 - resto))).Length - 1));
                    strBase2 = (strBase2 + strDigito2);
                    retorno = strBase2 == strOrigem;
                    break;

                case "RJ":
                    strBase = (strOrigem.Trim() + "00000000").Substring(0, 8);
                    soma = 0;
                    peso = 2;

                    for (pos = 7; pos >= 1; pos--)
                    {
                        intValor = Glass.Conversoes.ConverteValor<int>(strBase.Substring((pos - 1), 1)) * peso;
                        soma = (soma + intValor);
                        peso = (peso + 1);

                        if ((peso > 7))
                            peso = 2;
                    }

                    resto = (soma % 11);
                    strDigito1 = ((resto < 2) ? "0" : Convert.ToString((11 - resto))).Substring((((resto < 2) ? "0" : Convert.ToString((11 - resto))).Length - 1));
                    strBase2 = (strBase.Substring(0, 7) + strDigito1);
                    retorno = strBase2 == strOrigem;
                    break;

                case "RN":
                    strBase = (strOrigem.Trim() + "000000000").Substring(0, 9);

                    if ((strBase.Substring(0, 2) == "20"))
                    {
                        soma = 0;

                        for (pos = 1; (pos <= 8); pos++)
                        {
                            intValor = Glass.Conversoes.ConverteValor<int>(strBase.Substring((pos - 1), 1));
                            intValor = (intValor * (10 - pos));
                            soma = (soma + intValor);
                        }

                        soma = (soma * 10);
                        resto = (soma % 11);
                        strDigito1 = ((resto > 9) ? "0" : Convert.ToString(resto)).Substring((((resto > 9) ? "0" : Convert.ToString(resto)).Length - 1));
                        strBase2 = (strBase.Substring(0, 8) + strDigito1);
                        retorno = strBase2 == strOrigem;
                    }
                    break;

                case "RO":
                    strBase = strOrigem.PadLeft(13, '0');
                    peso = 6;
                    soma = 0;

                    for (pos = 0; pos < 13; pos++)
                    {
                        soma += Glass.Conversoes.ConverteValor<int>(strBase[pos].ToString()) * peso--;

                        if (peso == 1)
                            peso = 9;
                    }

                    resto = (soma % 11);
                    dig1 = (11 - resto);

                    if (dig1 >= 10)
                        dig1 = dig1 - 10;

                    retorno = strOrigem.EndsWith(dig1.ToString());
                    break;

                case "RR":
                    strBase = (strOrigem.Trim() + "000000000").Substring(0, 9);

                    if ((strBase.Substring(0, 2) == "24"))
                    {
                        soma = 0;
                        peso = 1;

                        for (pos = 0; pos < 8; pos++)
                            soma += Glass.Conversoes.ConverteValor<int>(strBase[pos].ToString()) * peso++;

                        resto = soma % 9;
                        strDigito1 = resto.ToString().Substring(resto.ToString().Length - 1);
                        retorno = (strBase.Substring(0, 8) + strDigito1) == strOrigem;
                    }
                    break;

                case "RS":
                    strBase = (strOrigem.Trim() + "0000000000").Substring(0, 10);
                    intNumero = Glass.Conversoes.ConverteValor<int>(strBase.Substring(0, 3));

                    if (intNumero > 0 && intNumero < 468)
                    {
                        soma = 0;
                        peso = 2;

                        for (pos = 9; pos >= 1; pos--)
                        {
                            intValor = Glass.Conversoes.ConverteValor<int>(strBase.Substring((pos - 1), 1)) * peso++;
                            soma += intValor;

                            if ((peso > 9))
                                peso = 2;
                        }

                        resto = (soma % 11);
                        intValor = (11 - resto);

                        if ((intValor > 9))
                            intValor = 0;

                        strDigito1 = Convert.ToString(intValor).Substring((Convert.ToString(intValor).Length - 1));
                        strBase2 = (strBase.Substring(0, 9) + strDigito1);
                        retorno = strBase2 == strOrigem;
                    }
                    break;

                case "SC":
                    strBase = (strOrigem.Trim() + "000000000").Substring(0, 9);
                    soma = 0;

                    for (pos = 1; (pos <= 8); pos++)
                    {
                        intValor = Glass.Conversoes.ConverteValor<int>(strBase.Substring((pos - 1), 1));
                        intValor = (intValor * (10 - pos));
                        soma = (soma + intValor);
                    }

                    resto = (soma % 11);
                    strDigito1 = ((resto < 2) ? "0" : Convert.ToString((11 - resto))).Substring((((resto < 2) ? "0" : Convert.ToString((11 - resto))).Length - 1));
                    strBase2 = (strBase.Substring(0, 8) + strDigito1);
                    retorno = strBase2 == strOrigem;
                    break;

                case "SE":
                    strBase = (strOrigem.Trim() + "000000000").Substring(0, 9);
                    soma = 0;

                    for (pos = 1; (pos <= 8); pos++)
                    {
                        intValor = Glass.Conversoes.ConverteValor<int>(strBase.Substring((pos - 1), 1));
                        intValor = (intValor * (10 - pos));
                        soma = (soma + intValor);
                    }

                    resto = (soma % 11);
                    intValor = (11 - resto);

                    if ((intValor > 9))
                        intValor = 0;

                    strDigito1 = Convert.ToString(intValor).Substring((Convert.ToString(intValor).Length - 1));
                    strBase2 = (strBase.Substring(0, 8) + strDigito1);
                    retorno = strBase2 == strOrigem;
                    break;

                case "SP":
                    if ((strOrigem.Substring(0, 1) == "P"))
                    {
                        strBase = (strOrigem.Trim() + "0000000000000").Substring(0, 13);
                        strBase2 = strBase.Substring(1, 8);
                        soma = 0;
                        peso = 1;

                        for (pos = 1; (pos <= 8); pos++)
                        {
                            intValor = Glass.Conversoes.ConverteValor<int>(strBase.Substring((pos - 1), 1));
                            intValor = (intValor * peso);
                            soma = (soma + intValor);
                            peso = (peso + 1);

                            if ((peso == 2))
                                peso = 3;

                            if ((peso == 9))
                                peso = 10;
                        }

                        resto = (soma % 11);
                        strDigito1 = Convert.ToString(resto).Substring((Convert.ToString(resto).Length - 1));
                        strBase2 = (strBase.Substring(0, 8) + (strDigito1 + strBase.Substring(10, 3)));
                    }
                    else
                    {
                        strBase = (strOrigem.Trim() + "000000000000").Substring(0, 12);
                        soma = 0;
                        peso = 1;

                        for (pos = 1; (pos <= 8); pos++)
                        {
                            intValor = Glass.Conversoes.ConverteValor<int>(strBase.Substring((pos - 1), 1)) * peso;
                            soma = (soma + intValor);
                            peso++;

                            if ((peso == 2))
                                peso = 3;

                            if ((peso == 9))
                                peso = 10;
                        }

                        resto = (soma % 11);

                        strDigito1 = resto.ToString().Substring((Convert.ToString(resto).Length - 1));
                        strBase2 = (strBase.Substring(0, 8) + (strDigito1 + strBase.Substring(9, 2)));
                        soma = 0;
                        peso = 2;

                        for (pos = 11; pos >= 1; pos--)
                        {
                            intValor = Glass.Conversoes.ConverteValor<int>(strBase.Substring((pos - 1), 1)) * peso++;
                            soma += intValor;

                            if ((peso > 10))
                                peso = 2;
                        }

                        resto = soma % 11;
                        strDigito2 = Convert.ToString(resto).Substring((Convert.ToString(resto).Length - 1));
                        strBase2 = strBase2 + strDigito2;
                    }

                    // Compara a IE obtida com a IE passada
                    retorno = strBase2 == strOrigem;
                    break;

                case "TO":
                    strBase = strOrigem.Trim();

                    // Os dois primeiros dígitos devem ser obrigatoriamente "29"
                    if (strBase.Substring(0, 2) != "29")
                        break;

                    soma = 0;

                    for (pos = 0; pos < 8; pos++)
                    {
                        intValor = Glass.Conversoes.ConverteValor<int>(strBase.Substring(pos, 1));
                        intValor = intValor * (9 - pos);
                        soma += intValor;
                    }

                    resto = (soma % 11);
                    strDigito1 = resto < 2 ? "0" : (11 - resto).ToString();

                    if (strBase.Substring(8, 1) == strDigito1)
                        retorno = true;

                    break;
            }

            return retorno;
        }

        #endregion

        #region Valida Email

        /// <summary>
        /// Verifica se email é válido
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static bool ValidaEmail(string email)
        {
            var rg = new System.Text.RegularExpressions.Regex(@"^([\w\-]+\.)*[\w\- ]+@([\w\- ]+\.)+([\w\-]{2,3})$");

            var emails = email
                .Trim()
                .Split(';')
                .Where(f => !string.IsNullOrEmpty(f))
                .Select(f => f.Trim());

            if (emails.Count() == 0)
                return false;

            foreach (var e in emails)
            {
                if (!rg.IsMatch(e))
                    return false;
            }

            return true;
        }

        #endregion

        #region Verifica se string contém apenas letras e números

        /// <summary>
        /// Verifica se string contém apenas letras e números
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool ApenasLetrasNumeros(string value)
        {
            System.Text.RegularExpressions.Regex rg = new System.Text.RegularExpressions.Regex(
                @"^[a-zA-Z0-9]+$");

            return rg.IsMatch(value);
        }

        #endregion

        #region Verifica se string possui apenas números

        /// <summary>
        /// Verifica se string possui apenas números
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static bool IsNumber(string number)
        {
            if (String.IsNullOrEmpty(number))
                return false;

            System.Text.RegularExpressions.Regex rg = new System.Text.RegularExpressions.Regex(@"^[0-9]+$");

            return rg.IsMatch(number);
        }

        #endregion

        #region Desabilitar validadores de campo obrigatório

        private static void DisableRequiredFieldValidator(ControlCollection controles)
        {
            foreach (Control c in controles)
            {
                if (c is RequiredFieldValidator)
                    ((RequiredFieldValidator)c).Enabled = false;
                else if (c.Controls.Count > 0)
                    DisableRequiredFieldValidator(c.Controls);
            }
        }

        /// <summary>
        /// Desabilita os validadores de campo obrigatório da página.
        /// </summary>
        /// <param name="pagina"></param>
        public static void DisableRequiredFieldValidator(Page pagina)
        {
            DisableRequiredFieldValidator(pagina.Controls);
        }

        #endregion
    }
}
