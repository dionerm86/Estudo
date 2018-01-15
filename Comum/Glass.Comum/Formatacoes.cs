using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace Glass
{
    public static class Formatacoes
    {
        #region Máscaras

        public static string MascaraCpfCnpj(string cpfCnpj)
        {
            if (String.IsNullOrEmpty(cpfCnpj))
                return String.Empty;

            if (cpfCnpj.Length > 11)
                return MascaraCnpj(cpfCnpj);
            else
                return MascaraCpf(cpfCnpj);
        }

        public static string MascaraCpf(string cpf)
        {
            if (String.IsNullOrEmpty(cpf))
                return String.Empty;

            if (cpf.Length > 11)
                return cpf;

            string output = cpf.PadLeft(11, '0').Insert(9, "-").Insert(6, ".").Insert(3, ".");

            return output;
        }

        public static string MascaraCnpj(string cnpj)
        {
            if (String.IsNullOrEmpty(cnpj))
                return String.Empty;

            if (cnpj.Length != 14)
                return cnpj;

            string output = cnpj.PadLeft(14, '0').Insert(12, "-").Insert(8, "/").Insert(5, ".").Insert(2, ".");

            return output;
        }

        /// <summary>
        /// Retorna string com máscara do número da NF-e (000.000.000) aplicada
        /// </summary>
        /// <param name="nroNFe"></param>
        /// <returns></returns>
        public static string MascaraNroNFe(string nroNFe)
        {
            if (String.IsNullOrEmpty(nroNFe))
                return String.Empty;

            string output = nroNFe.PadLeft(9, '0').Insert(6, ".").Insert(3, ".");

            return output;
        }


        /// <summary>
        /// Retorna string com máscara do número do CT-e (000.000.000) aplicado
        /// </summary>
        /// <param name="nroNFe"></param>
        /// <returns></returns>
        public static string MascaraNroCTe(string nroCTe)
        {
            if (String.IsNullOrEmpty(nroCTe))
                return String.Empty;

            string output = nroCTe.PadLeft(9, '0').Insert(6, ".").Insert(3, ".");

            return output;
        }

        /// <summary>
        /// Retorna string com máscara da chave de acesso da NF-e aplicada
        /// </summary>
        /// <param name="chaveAcessoNFe"></param>
        /// <returns></returns>
        public static string MascaraChaveAcessoNFe(string chaveAcessoNFe)
        {
            if (String.IsNullOrEmpty(chaveAcessoNFe))
                return String.Empty;

            if (chaveAcessoNFe.Length != 44)
                return chaveAcessoNFe;

            string output = chaveAcessoNFe;

            for (int i = 40; i >= 4; i -= 4)
                output = output.Insert(i, " ");

            return output;
        }

        /// <summary>
        /// Retorna string com máscara da chave de acesso do CT-e aplicada
        /// </summary>
        /// <param name="chaveAcessoNFe"></param>
        /// <returns></returns>
        public static string MascaraChaveAcessoCTe(string chaveAcessoCTe)
        {
            if (String.IsNullOrEmpty(chaveAcessoCTe))
                return String.Empty;

            if (chaveAcessoCTe.Length != 44)
                return chaveAcessoCTe;

            string output = chaveAcessoCTe;

            for (int i = 40; i >= 4; i -= 4)
                output = output.Insert(i, " ");

            return output;
        }

        /// <summary>
        /// Retorna string com máscara dos dados adicionais da NF-e aplicada
        /// </summary>
        /// <param name="dadosAdicionaisNFe"></param>
        /// <returns></returns>
        public static string MascaraDadosAdicionaisNFe(string dadosAdicionaisNFe)
        {
            if (String.IsNullOrEmpty(dadosAdicionaisNFe))
                return String.Empty;

            if (dadosAdicionaisNFe.Length != 36)
                return dadosAdicionaisNFe;

            string output = dadosAdicionaisNFe;

            for (int i = 32; i >= 4; i -= 4)
                output = output.Insert(i, " ");

            return output;
        }

        /// <summary>
        /// Retorna string com máscara dos dados adicionais do CT-e aplicado
        /// </summary>
        /// <param name="dadosAdicionaisNFe"></param>
        /// <returns></returns>
        public static string MascaraDadosAdicionaisCTe(string dadosAdicionaisCTe)
        {
            if (String.IsNullOrEmpty(dadosAdicionaisCTe))
                return String.Empty;

            if (dadosAdicionaisCTe.Length != 36)
                return dadosAdicionaisCTe;

            string output = dadosAdicionaisCTe;

            for (int i = 32; i >= 4; i -= 4)
                output = output.Insert(i, " ");

            return output;
        }

        #endregion

        #region Formatação de CPF/CNPJ

        /// <summary>
        /// Formata o texto de CPF ou CNPJ.
        /// </summary>
        /// <param name="cpfCnpj"></param>
        /// <returns></returns>
        public static string FormataCpfCnpj(string cpfCnpj)
        {
            if (String.IsNullOrEmpty(cpfCnpj))
                return null;

            cpfCnpj = cpfCnpj.Replace(".", "").Replace("/", "").Replace("-", "");

            if (cpfCnpj.Length < 11)
                return cpfCnpj;

            if (cpfCnpj.Length == 11)
            {
                return String.Format("{0}.{1}.{2}-{3}",
                    cpfCnpj.Substring(0, 3),
                    cpfCnpj.Substring(3, 3),
                    cpfCnpj.Substring(6, 3),
                    cpfCnpj.Substring(9));
            }
            else
            {
                return String.Format("{0}.{1}.{2}/{3}-{4}",
                    cpfCnpj.Substring(0, 2),
                    cpfCnpj.Substring(2, 3),
                    cpfCnpj.Substring(5, 3),
                    cpfCnpj.Substring(8, 4),
                    cpfCnpj.Substring(12));
            }
        }

        #endregion

        #region Retorna valor por extenso

        /// <summary>
        /// Retorna valor por extenso até 999.999.999.999,99
        /// </summary>
        /// <param name="valor"></param>
        /// <returns></returns>
        public static string ValorExtenso(string valor)
        {
            string[] wunidade = { "", " e um", " e dois", " e três", " e quatro", " e cinco", " e seis", " e sete", " e oito", " e nove" };
            string[] wdezes = { "", " e onze", " e doze", " e treze", " e quatorze", " e quinze", " e dezesseis", " e dezessete", " e dezoito", " e dezenove" };
            string[] wdezenas = { "", " e dez", " e vinte", " e trinta", " e quarenta", " e cinquenta", " e sessenta", " e setenta", " e oitenta", " e noventa" };
            string[] wcentenas = { "", " e cento", " e duzentos", " e trezentos", " e quatrocentos", " e quinhentos", " e seiscentos", " e setecentos", " e oitocentos", " e novecentos" };
            string[] wplural = { " bilhões", " milhões", " mil", "" };
            string[] wsingular = { " bilhão", " milhão", " mil", "" };
            string wextenso = "";
            string wfracao;
            valor = valor.Replace("R$", "");
            string wnumero = valor.Replace(",", "").Trim();
            wnumero = wnumero.Replace(".", "").PadLeft(14, '0');
            if (Int64.Parse(wnumero.Substring(0, 12)) > 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    wfracao = wnumero.Substring(i * 3, 3);
                    if (Glass.Conversoes.ConverteValor<int>(wfracao) != 0)
                    {
                        if (Glass.Conversoes.ConverteValor<int>(wfracao.Substring(0, 3)) == 100) wextenso += " e cem";
                        else
                        {
                            wextenso += wcentenas[Glass.Conversoes.ConverteValor<int>(wfracao.Substring(0, 1))];
                            if (Glass.Conversoes.ConverteValor<int>(wfracao.Substring(1, 2)) > 10 && Glass.Conversoes.ConverteValor<int>(wfracao.Substring(1, 2)) < 20)
                                wextenso += wdezes[Glass.Conversoes.ConverteValor<int>(wfracao.Substring(2, 1))];
                            else
                            {
                                wextenso += wdezenas[Glass.Conversoes.ConverteValor<int>(wfracao.Substring(1, 1))];
                                wextenso += wunidade[Glass.Conversoes.ConverteValor<int>(wfracao.Substring(2, 1))];
                            }
                        }
                        if (Glass.Conversoes.ConverteValor<int>(wfracao) > 1) wextenso += wplural[i];
                        else wextenso += wsingular[i];
                    }
                }
                if (Int64.Parse(wnumero.Substring(0, 12)) > 1) wextenso += " reais";
                else wextenso += " real";
            }
            wfracao = wnumero.Substring(12, 2);
            if (Glass.Conversoes.ConverteValor<int>(wfracao) > 0)
            {
                if (Glass.Conversoes.ConverteValor<int>(wfracao.Substring(0, 2)) > 10 && Glass.Conversoes.ConverteValor<int>(wfracao.Substring(0, 2)) < 20) wextenso = wextenso + wdezes[Glass.Conversoes.ConverteValor<int>(wfracao.Substring(1, 1))];
                else
                {
                    wextenso += wdezenas[Glass.Conversoes.ConverteValor<int>(wfracao.Substring(0, 1))];
                    wextenso += wunidade[Glass.Conversoes.ConverteValor<int>(wfracao.Substring(1, 1))];
                }
                if (Glass.Conversoes.ConverteValor<int>(wfracao) > 1) wextenso += " centavos";
                else wextenso += " centavo";
            }
            if (wextenso != "") wextenso = wextenso.Substring(3, 1).ToUpper() + wextenso.Substring(4);
            else wextenso = "Zero";
            return wextenso;
        }

        #endregion

        #region Retorna data por extenso

        /// <summary>
        /// Retorna data por extenso
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string DataExtenso(DateTime data)
        {
            string dataExtenso = data.Day + " de " + FuncoesData.ObtemMes(data.Month, false) + " de " + data.ToString("yyyy");

            return dataExtenso;
        }

        #endregion

        #region Tratamento de dados

        /// <summary>
        /// Trata a string para ser inserida no XML (NF-e / CT-e)
        /// </summary>
        /// <param name="str"></param>
        public static string RetiraCaracteresEspeciais(string str)
        {
            return RetiraCaracteresEspeciais(str, false);
        }

        /// <summary>
        /// Trata a string para ser inserida no XML (NF-e / CT-e)
        /// </summary>
        /// <param name="str"></param>
        /// <param name="telefone"></param>
        public static string RetiraCaracteresEspeciais(string str, bool telefone)
        {
            if (str == null || str.Trim() == String.Empty)
                return String.Empty;

            /* Chamado 60766. */
            var contador = 0;

            // Remove espaços duplos da string
            while (str.IndexOf("  ") > 0 && contador < 20)
            {
                str = str.Replace("  ", " ");
                contador++;
            }

            if (telefone)
                str = str.Replace(" ", "");

            str = str.Replace(",", "").Replace(".", "").Replace("-", "").Replace("/", "").Replace(")", "").Replace("(", "");
            str = str.Replace("à", "a").Replace("á", "a").Replace("â", "a").Replace("ã", "a");
            str = str.Replace("é", "e").Replace("ê", "e").Replace("í", "i").Replace("î", "i");
            str = str.Replace("ó", "o").Replace("ô", "o").Replace("õ", "o").Replace("ú", "u").Replace("û", "u");
            str = str.Replace("À", "A").Replace("Á", "A").Replace("Â", "A").Replace("Ã", "A");
            str = str.Replace("É", "E").Replace("Ê", "E").Replace("Í", "I").Replace("Î", "I");
            str = str.Replace("Ó", "O").Replace("Ô", "O").Replace("Õ", "O").Replace("Ú", "U").Replace("Û", "U");
            str = str.Replace("ç", "c").Replace("Ç", "C");
            str = str.Replace("%", "").Replace("´", "").Replace("`", "").Replace("~", "").Replace("^", "").Replace("&", "&amp;");
            str = str.Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&#39;");
            str = str.Replace("\t", "").Replace("\r", "").Replace("\n", "").Replace("¹", "1").Replace("²", "2").Replace("³", "3");
            str = str.Replace("\\", "").Replace("º", "o").Replace("°", "o").Replace("ª", "a");

            return str.Trim();
        }

        /// <summary>
        /// Restaura a string de um campo do (CTe ou NFe), substituindo valor antigo por novo
        /// </summary>
        /// <param name="valor"></param>
        /// <param name="antigo"></param>
        /// <param name="novo"></param>
        /// <returns></returns>
        private static string RestauraStringDocFiscal(string valor, string antigo, string novo)
        {
            while (valor.Contains(antigo))
                valor = valor.Replace(antigo, novo);

            return valor;
        }

        /// <summary>
        /// Restaura a string de um campo do (CTe ou NFe), por exemplo, voltando o campos "&amp;" para "&"
        /// </summary>
        /// <param name="valor"></param>
        /// <returns></returns>
        public static string RestauraStringDocFiscal(string valor)
        {
            KeyValuePair<string, string>[] mudar = new KeyValuePair<string, string>[]
            {
                new KeyValuePair<string, string>("&amp;", "&"),
                new KeyValuePair<string, string>("&lt;", "<"),
                new KeyValuePair<string, string>("&gt;", ">"),
                new KeyValuePair<string, string>("&quot;", "\""),
                new KeyValuePair<string, string>("&#39;", "'"),
            };

            foreach (KeyValuePair<string, string> item in mudar)
                valor = RestauraStringDocFiscal(valor, item.Key, item.Value);

            return valor;
        }

        /// <summary>
        /// Inverte uma string.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string InverteString(string s)
        {
            char[] arr = s.ToCharArray();
            Array.Reverse(arr);
            return new string(arr);
        }

        #endregion

        /// <summary>
        /// Trata campos de formato número decimal da Nfe, inserindo n casas decimais após a vírgula
        /// </summary>
        /// <param name="valor"></param>
        /// <param name="casasAposVirgula"></param>
        /// <returns></returns>
        public static string FormataValorDecimal(decimal valor, int casasAposVirgula)
        {
            return valor.ToString("N" + casasAposVirgula).Replace(".", "").Replace(',', '.');
        }

        /// <summary>
        /// Trata campos de formato número decimal da Nfe, inserindo n casas decimais após a vírgula
        /// </summary>
        /// <param name="valor"></param>
        /// <param name="casasAposVirgula"></param>
        /// <returns></returns>
        public static string FormataValorDecimal(double valor, int casasAposVirgula)
        {
            return valor.ToString("N" + casasAposVirgula).Replace(".", "").Replace(',', '.');
        }

        /// <summary>
        /// Formata número colocando duas casas decimais
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static string FormataValorDecimal(string number, int numeroCasasDecimais)
        {
            if (String.IsNullOrEmpty(number))
                return 0.ToString("N" + numeroCasasDecimais, System.Globalization.CultureInfo.GetCultureInfoByIetfLanguageTag("pt-BR"));

            // EDIT: (André 23/01/2012) Ao formatar valor de NF-e, ao imprimir o DANFE a partir do XML, 
            // estava retirando a 3ª casa em diante dos valores, o que não pode ser feito
            // Se houver 4 casa decimais nesse número, retorna apenas 2
            //if (number.Substring(number.LastIndexOf('.') + 1).Length > 2)
            //    number = number.Remove(number.LastIndexOf('.') + 3);

            try
            {
                return decimal.Parse(number.Replace('.', ','), System.Globalization.CultureInfo.GetCultureInfoByIetfLanguageTag("pt-BR")).ToString("N" + numeroCasasDecimais, System.Globalization.CultureInfo.GetCultureInfoByIetfLanguageTag("pt-BR"));
            }
            catch
            {
                //ErroDAO.Instance.InserirFromException("Utils.NumberFormat", ex);
                return Single.Parse(number, System.Globalization.CultureInfo.GetCultureInfoByIetfLanguageTag("pt-BR")).ToString("N" + numeroCasasDecimais, System.Globalization.CultureInfo.GetCultureInfoByIetfLanguageTag("pt-BR")).Replace('.', ',');
            }
        }

        #region Trata string arquivo domínio sistemas

        /// <summary>
        /// Trata a string para ser inserida no arquivo Domínio Sistemas.
        /// </summary>
        public static string TrataStringArquivoDominioSistemas(string str)
        {
            if (str == null || str.Trim() == string.Empty)
                return string.Empty;

            var cont = 0;

            // Remove espaços duplos da string
            while (str.IndexOf("  ") > 0 && cont < 50)
            {
                str = str.Replace("  ", " ");
                cont++;
            }

            str = str.Replace(",", "").Replace(".", "").Replace("-", "").Replace(")", "").Replace("(", "");
            str = str.Replace("à", "a").Replace("á", "a").Replace("â", "a").Replace("ã", "a");
            str = str.Replace("é", "e").Replace("ê", "e").Replace("í", "i").Replace("î", "i");
            str = str.Replace("ó", "o").Replace("ô", "o").Replace("õ", "o").Replace("ú", "u").Replace("û", "u");
            str = str.Replace("À", "A").Replace("Á", "A").Replace("Â", "A").Replace("Ã", "A");
            str = str.Replace("É", "E").Replace("Ê", "E").Replace("Í", "I").Replace("Î", "I");
            str = str.Replace("Ó", "O").Replace("Ô", "O").Replace("Õ", "O").Replace("Ú", "U").Replace("Û", "U");
            str = str.Replace("ç", "c").Replace("Ç", "C");
            str = str.Replace("%", "").Replace("´", "").Replace("`", "").Replace("~", "").Replace("^", "").Replace("&", "");
            str = str.Replace("<", "").Replace(">", "").Replace("'", "");
            str = str.Replace("\t", "").Replace("\r", "").Replace("\n", "").Replace("¹", "1").Replace("²", "2").Replace("³", "3");
            str = str.Replace("\\", "").Replace("º", "o").Replace("°", "o").Replace("ª", "a");

            return str.Trim();
        }

        #endregion

        #region Tratamento de dados de NFe

        public static string TrataStringDocFiscal(string str)
        {
            return TrataStringDocFiscal(str, false);
        }

        /// <summary>
        /// Trata a string para ser inserida no XML da NF-e
        /// </summary>
        /// <param name="str"></param>
        public static string TrataStringDocFiscal(string str, bool telefone)
        {
            if (str == null || str.Trim() == String.Empty)
                return String.Empty;

            int cont = 0;

            // Remove espaços duplos da string
            while (str.IndexOf("  ") > 0 && cont < 50)
            {
                str = str.Replace("  ", " ");
                cont++;
            }

            if (telefone)
                str = str.Replace(" ", "");

            str = str.Replace(",", "").Replace(".", "").Replace("-", "").Replace("/", "").Replace(")", "").Replace("(", "");
            str = str.Replace("à", "a").Replace("á", "a").Replace("â", "a").Replace("ã", "a");
            str = str.Replace("é", "e").Replace("ê", "e").Replace("í", "i").Replace("î", "i");
            str = str.Replace("ó", "o").Replace("ô", "o").Replace("õ", "o").Replace("ú", "u").Replace("û", "u");
            str = str.Replace("À", "A").Replace("Á", "A").Replace("Â", "A").Replace("Ã", "A");
            str = str.Replace("É", "E").Replace("Ê", "E").Replace("Í", "I").Replace("Î", "I");
            str = str.Replace("Ó", "O").Replace("Ô", "O").Replace("Õ", "O").Replace("Ú", "U").Replace("Û", "U");
            str = str.Replace("ç", "c").Replace("Ç", "C");
            str = str.Replace("%", "").Replace("´", "").Replace("`", "").Replace("~", "").Replace("^", "").Replace("&", "&amp;");
            str = str.Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&#39;");
            str = str.Replace("\t", "").Replace("\r", "").Replace("\n", "").Replace("¹", "1").Replace("²", "2").Replace("³", "3");
            str = str.Replace("\\", "").Replace("º", "o").Replace("°", "o").Replace("ª", "a");

            return str.Trim();
        }

        /// <summary>
        /// Trata as informações complementares da nota.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string TrataTextoDocFiscal(string str)
        {
            if (str == null || str.Trim() == String.Empty)
                return String.Empty;

            // Remove espaços duplos da string
            while (str.IndexOf("  ") > 0)
                str = str.Replace("  ", " ");

            str = str.Replace("à", "a").Replace("á", "a").Replace("â", "a").Replace("ã", "a");
            str = str.Replace("é", "e").Replace("ê", "e").Replace("í", "i").Replace("î", "i");
            str = str.Replace("ó", "o").Replace("ô", "o").Replace("õ", "o").Replace("ú", "u").Replace("û", "u");
            str = str.Replace("À", "A").Replace("Á", "A").Replace("Â", "A").Replace("Ã", "A");
            str = str.Replace("É", "E").Replace("Ê", "E").Replace("Í", "I").Replace("Î", "I");
            str = str.Replace("Ó", "O").Replace("Ô", "O").Replace("Õ", "O").Replace("Ú", "U").Replace("Û", "U");
            str = str.Replace("ç", "c").Replace("Ç", "C");
            str = str.Replace("´", "").Replace("`", "").Replace("~", "").Replace("^", "").Replace("&", "&amp;");
            str = str.Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&#39;");
            str = str.Replace("\t", "").Replace("\r", "").Replace("\n", "").Replace("¹", "1").Replace("²", "2").Replace("³", "3");
            str = str.Replace("º", "o").Replace("ª", "a");

            return str.Trim();
        }
        
        /// <summary>
        /// Trata as informações complementares da nota.
        /// </summary>
        public static string TrataTextoComparacaoSelPopUp(string texto)
        {
            if (texto == null || texto.Trim() == string.Empty)
                return string.Empty;

            // Remove espaços duplos da string
            while (texto.IndexOf("  ", StringComparison.Ordinal) > 0)
                texto = texto.Replace("  ", " ");

            texto = texto.Replace("à", "a").Replace("á", "a").Replace("â", "a").Replace("ã", "a");
            texto = texto.Replace("é", "e").Replace("ê", "e").Replace("í", "i").Replace("î", "i");
            texto = texto.Replace("ó", "o").Replace("ô", "o").Replace("õ", "o").Replace("ú", "u").Replace("û", "u");
            texto = texto.Replace("À", "A").Replace("Á", "A").Replace("Â", "A").Replace("Ã", "A");
            texto = texto.Replace("É", "E").Replace("Ê", "E").Replace("Í", "I").Replace("Î", "I");
            texto = texto.Replace("Ó", "O").Replace("Ô", "O").Replace("Õ", "O").Replace("Ú", "U").Replace("Û", "U");
            texto = texto.Replace("ç", "c").Replace("Ç", "C");
            texto = texto.Replace("´", "").Replace("`", "").Replace("~", "").Replace("^", "").Replace("&", "");
            texto = texto.Replace("<", "").Replace(">", "").Replace("\"", "").Replace("'", "").Replace("-", "");
            texto = texto.Replace("\t", "").Replace("\r", "").Replace("\n", "").Replace("¹", "1").Replace("²", "2").Replace("³", "3");
            texto = texto.Replace("º", "o").Replace("ª", "a");

            return texto.Trim();
        }

        /// <summary>
        /// Trata campos de formato número decimal, inserindo n casas decimais após a vírgula
        /// </summary>
        /// <param name="valor"></param>
        /// <param name="casasAposVirgula"></param>
        /// <returns></returns>      
        public static string TrataValorDecimal(decimal valor, int casasAposVirgula)
        {
            return valor.ToString("N" + casasAposVirgula).Replace(".", "").Replace(',', '.');
        }

        public static string TrataValorDouble(double valor, int casasAposVirgula)
        {
            return valor.ToString("N" + casasAposVirgula).Replace(".", "").Replace(',', '.');
        }

        public static string SubstituiCaracteresEspeciais(string strline)
        {
            try
            {
                strline = strline.Replace("ã", "a");
                strline = strline.Replace('Ã', 'A');
                strline = strline.Replace('â', 'a');
                strline = strline.Replace('Â', 'A');
                strline = strline.Replace('á', 'a');
                strline = strline.Replace('Á', 'A');
                strline = strline.Replace('à', 'a');
                strline = strline.Replace('À', 'A');
                strline = strline.Replace('ç', 'c');
                strline = strline.Replace('Ç', 'C');
                strline = strline.Replace('é', 'e');
                strline = strline.Replace('É', 'E');
                strline = strline.Replace('õ', 'o');
                strline = strline.Replace('Õ', 'O');
                strline = strline.Replace('ó', 'o');
                strline = strline.Replace('Ó', 'O');
                strline = strline.Replace('ô', 'o');
                strline = strline.Replace('Ô', 'O');
                strline = strline.Replace('ú', 'u');
                strline = strline.Replace('Ú', 'U');
                strline = strline.Replace('ü', 'u');
                strline = strline.Replace('Ü', 'U');
                strline = strline.Replace('í', 'i');
                strline = strline.Replace('Í', 'I');
                strline = strline.Replace('º', ' ');
                return strline.Trim();
            }
            catch (Exception ex)
            {
                Exception tmpEx = new Exception("Erro ao formatar string.", ex);
                throw tmpEx;
            }
        }

        public static string LimpaCpfCnpj(string CpfCnpj)
        {
            try
            {
                CpfCnpj = CpfCnpj.Replace(".", "").Replace("-", "").Replace("/", "");
                return CpfCnpj.Trim();
            }
            catch (Exception ex)
            {
                Exception tmpEx = new Exception("Erro ao formatar string.", ex);
                throw tmpEx;
            }
        }

        #endregion

        public static string RemoverAcentos(string texto)
        {
            string s = texto.Normalize(NormalizationForm.FormD);

            StringBuilder sb = new StringBuilder();

            for (int k = 0; k < s.Length; k++)
            {
                UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(s[k]);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(s[k]);
                }
            }
            return sb.ToString().Replace("-", "").Replace(".", "").Replace("/", "").Replace("\\", "").Replace("|", "");
        }

        public static string RemoverAcentosEspacos(this string texto)
        {
            string s = texto.Normalize(NormalizationForm.FormD);

            StringBuilder sb = new StringBuilder();

            for (int k = 0; k < s.Length; k++)
            {
                UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(s[k]);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(s[k]);
                }
            }
            return sb.ToString().Replace("-", "").Replace(".", "").Replace("/", "").Replace("\\", "")
                .Replace("|", "").Replace(" ", "");
        }

        #region Formata Campo

        /// <summary>
        /// Formata um campo numérico.
        /// </summary>
        /// <param name="texto"></param>
        /// <param name="nomeCampo"></param>
        /// <param name="tamanho"></param>
        /// <returns></returns>
        public static string FormataNumero(this string texto, string nomeCampo, int tamanho)
        {
            return texto.Formata(nomeCampo, tamanho, true, false);
        }

        /// <summary>
        /// Formata um campo numérico com opção de validar ou não o tamanho do campo.
        /// </summary>
        /// <param name="texto"></param>
        /// <param name="nomeCampo"></param>
        /// <param name="tamanho"></param>
        /// <param name="validarTamanhoCampo">Caso o tamanho do campo esteja acima do permitido exibe uma mesagem de erro,
        /// caso este parametro seja falso então o método trata o tamanho do campo.</param>
        /// <returns></returns>
        public static string FormataNumero(this string texto, string nomeCampo, int tamanho, bool validarTamanhoCampo)
        {
            return texto.Formata(nomeCampo, tamanho, true, validarTamanhoCampo);
        }

        /// <summary>
        /// Formata um campo textual.
        /// </summary>
        /// <param name="texto"></param>
        /// <param name="nomeCampo"></param>
        /// <param name="tamanho"></param>
        /// <returns></returns>
        public static string FormataTexto(this string texto, string nomeCampo, int tamanho)
        {
            return texto.Formata(nomeCampo, tamanho, false, false);
        }

        /// <summary>
        /// Formata um campo textual com opção de validar ou não o tamanho do campo.
        /// </summary>
        /// <param name="texto"></param>
        /// <param name="nomeCampo"></param>
        /// <param name="tamanho"></param>
        /// <param name="validarTamanhoCampo">Caso o tamanho do campo esteja acima do permitido exibe uma mesagem de erro,
        /// caso este parametro seja falso então o método trata o tamanho do campo.</param>
        /// <returns></returns>
        public static string FormataTexto(this string texto, string nomeCampo, int tamanho, bool validarTamanhoCampo)
        {
            return texto.Formata(nomeCampo, tamanho, false, validarTamanhoCampo);
        }

        /// <summary>
        /// Formata um campo textual ou numérico.
        /// </summary>
        /// <param name="texto"></param>
        /// <param name="nomeCampo"></param>
        /// <param name="tamanho"></param>
        /// <param name="numerico"></param>
        /// <param name="validarTamanhoCampo">Caso o tamanho do campo esteja acima do permitido exibe uma mesagem de erro,
        /// caso este parametro seja falso então o método trata o tamanho do campo. </param>
        /// <returns></returns>
        private static string Formata(this string texto, string nomeCampo, int tamanho, bool numerico, bool validarTamanhoCampo)
        {
            if (String.IsNullOrEmpty(texto))
                texto = String.Empty;

            if (texto.Length > tamanho)
            {
                if (validarTamanhoCampo)
                    throw new Exception("O campo " + nomeCampo + " possui " + texto.Length +
                        " caracteres e o máximo permitido são " + tamanho + " caracteres.");

                // O Substring deve pegar até tamanho - 2, pois, dessa forma irá pegar o tamanho exato menos um 
                // que é o espaço para o caractere '0' no caso de campo numérico e ' ' no caso de um texto.
                texto = texto.Substring(0, tamanho - 1);
            }

            texto = numerico ? texto.PadLeft(tamanho, '0') : texto.ToUpper().PadRight(tamanho, ' ');

            return texto;
        }

        #endregion

    }
}
