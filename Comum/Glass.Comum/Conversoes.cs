using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.ComponentModel;
using System.Security.Cryptography;

namespace Glass
{
    /// <summary>
    /// Classe com metodos auxiliares para conversoes.
    /// </summary>
    public static class Conversoes
    {
        /// <summary>
        /// Converte um valor para outro tipo.
        /// </summary>
        /// <param name="T">O tipo de retorno da conversão.</param>
        /// <param name="valor">O valor que será convertido.</param>
        /// <returns>O valor convertido (ou null, se não for possível converter).</returns>
        public static object ConverteValor(Type T, object valor)
        {
            var converter = typeof(Conversoes).GetMethod("ConverteValor", new Type[] { typeof(object) });
            return converter.MakeGenericMethod(T).Invoke(null, new object[] { valor });
        }

        /// <summary>
        /// Converte um valor para outro tipo.
        /// </summary>
        /// <typeparam name="T">O tipo de retorno da conversão.</typeparam>
        /// <param name="valor">O valor que será convertido.</param>
        /// <returns>O valor convertido (ou null, se não for possível converter).</returns>
        public static T ConverteValor<T>(object valor)
        {
            try
            {
                if (valor == null || valor == DBNull.Value)
                    return default(T);

                if (typeof(T) == typeof(int) && valor.GetType() == typeof(uint))
                {
                    valor =(int)(uint)valor;
                    return (T)valor;
                }

                Type tipoConverter = typeof(T);
                Type[] genericos = tipoConverter.GetGenericArguments();

                if (valor is IEnumerable && typeof(IEnumerable).IsAssignableFrom(tipoConverter) &&
                    genericos.Length > 0)
                {
                    Type tipoLista = typeof(List<>).MakeGenericType(genericos[0]);

                    IList lista = Activator.CreateInstance(tipoLista) as IList;
                    foreach (object o in (valor as IEnumerable))
                        lista.Add(ConverteValor(genericos[0], o));

                    if (tipoConverter.Equals(tipoLista))
                        return (T)lista;
                    else if (typeof(Array).IsAssignableFrom(tipoConverter))
                        return (T)tipoLista.GetMethod("ToArray").Invoke(lista, null);
                    else
                        throw new NotImplementedException("Não foi implementado para o tipo '" + tipoConverter.FullName + "'");
                }

                Type tipoNullable = null;
                if (Nullable.GetUnderlyingType(tipoConverter) != null)
                {
                    tipoNullable = tipoConverter;
                    tipoConverter = Nullable.GetUnderlyingType(tipoConverter);
                }

                Type tipoEnum = null;
                if (typeof(Enum).IsAssignableFrom(tipoConverter) && Enum.GetUnderlyingType(tipoConverter) != null)
                {
                    tipoEnum = tipoConverter;
                    tipoConverter = Enum.GetUnderlyingType(tipoConverter);
                }
 
                if (valor == null || valor.ToString() == "" || valor.ToString() == "0" && tipoConverter == typeof(DateTime))
                    return default(T);

                valor = Convert.ChangeType(valor, tipoConverter);

                if (tipoEnum != null)
                    valor = Enum.Parse(tipoEnum, valor.ToString());

                return (T)valor;
            }
            catch (NotImplementedException)
            {
                throw;
            }
            catch
            {
                return default(T);
            }
        }

        /// <summary>
        /// Converte uma data (string -> DateTime).
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static DateTime? ConverteData(this string data)
        {
            try
            {
                if (String.IsNullOrEmpty(data))
                    return null;

                return Convert.ToDateTime(data, System.Globalization.CultureInfo.CurrentCulture);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Converte uma data (string -> DateTime) que não seja nula.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static DateTime ConverteDataNotNull(string data)
        {
            return Convert.ToDateTime(data, System.Globalization.CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Converte uma data (DateTime -> string).
        /// </summary>
        /// <param name="data"></param>
        /// <param name="horas"></param>
        /// <returns></returns>
        public static string ConverteData(DateTime? data, bool horas)
        {
            if (data == null)
                return null;

            string strFormato = "dd/MM/yyyy" + (horas ? " HH:mm" : "");
            return DateTime.Parse(data.ToString()).ToString(strFormato);
        }

        /// <summary>
        /// Converte uma data (DateTime -> string).
        /// </summary>
        /// <param name="data"></param>
        /// <param name="horas"></param>
        /// <returns></returns>
        public static string ConverteData(DateTime data, bool horas)
        {
            string strFormato = "dd/MM/yyyy" + (horas ? " HH:mm" : "");
            return DateTime.Parse(data.ToString()).ToString(strFormato);
        }

        #region Converte número decimal para string

        public static string FloatParaStr(float valor)
        {
            return valor.ToString().Replace('.', ',');
        }

        public static string DoubleParaStr(double valor)
        {
            return valor.ToString().Replace('.', ',');
        }

        public static string DecimalParaStr(decimal valor)
        {
            return valor.ToString().Replace('.', ',');
        }

        public static string DecimalParaStr(decimal valor, int numeroCasasDecimais)
        {
            // É necessário que seja numeroCasasDecimais + 1, caso contrário considerar apenas uma casa decimal
            return valor.ToString("0" + (numeroCasasDecimais > 0 ? ".".PadRight(numeroCasasDecimais + 1, '0') : ""));
        }

        public static string UintParaStr(uint? valor)
        {
            return valor != null ? valor.ToString() : null;
        }

        #endregion

        #region Converte string para número decimal/inteiro

        public static int StrParaInt(this string valor)
        {
            return StrParaIntNullable(valor).GetValueOrDefault();
        }

        public static int? StrParaIntNullable(this string valor)
        {
            int temp;
            return int.TryParse(valor, out temp) ? (int?)temp : null;
        }

        public static uint StrParaUint(this string valor)
        {
            return StrParaUintNullable(valor).GetValueOrDefault();
        }

        public static uint? StrParaUintNullable(this string valor)
        {
            uint temp;
            return uint.TryParse(valor, out temp) ? (uint?)temp : null;
        }

        public static float StrParaFloat(this string valor)
        {
            return StrParaFloatNullable(valor).GetValueOrDefault();
        }

        public static float? StrParaFloatNullable(string valor)
        {
            if (String.IsNullOrEmpty(valor))
                return null;

            if (valor.Contains(".") && valor.Contains(","))
                valor = valor.Replace(".", "");

            return float.Parse(valor.Replace("R$", string.Empty).Replace(" ", string.Empty).Replace('.', ','));
        }

        public static double StrParaDouble(string valor)
        {
            return StrParaDoubleNullable(valor).GetValueOrDefault();
        }

        public static double? StrParaDoubleNullable(string valor)
        {
            if (String.IsNullOrEmpty(valor))
                return null;

            if (valor.Contains(".") && valor.Contains(","))
                valor = valor.Replace(".", "");

            return double.Parse(valor.Replace("R$", string.Empty).Replace(" ", string.Empty).Replace('.', ','));
        }

        public static decimal StrParaDecimal(this string valor)
        {
            return StrParaDecimalNullable(valor).GetValueOrDefault();
        }

        public static decimal? StrParaDecimalNullable(this string valor)
        {
            if (String.IsNullOrEmpty(valor))
                return null;

            if (valor.Contains(".") && valor.Contains(","))
                valor = valor.Replace(".", "");

            decimal retorno = 0;

            return decimal.TryParse(valor.Replace("R$", string.Empty).Replace(" ", string.Empty).Replace('.', ','), out retorno) ? retorno : 0;
        }

        public static DateTime? StrParaDate(this string valor)
        {
            if (String.IsNullOrEmpty(valor))
                return null;

            return DateTime.Parse(valor);
        }

        #endregion

        public static string CodificaPara64(byte[] toEncodeAsBytes)
        {
            //byte[] toEncodeAsBytes = System.Text.Encoding.Unicode.GetBytes(toEncode);

            string returnValue = System.Convert.ToBase64String(toEncodeAsBytes);
            return returnValue;
        }

        public static byte[] DecodificaPara64(string encodedData)
        {
            byte[] encodedDataAsBytes = System.Convert.FromBase64String(encodedData);
            //string returnValue =  System.Text.Encoding.Unicode.GetString(encodedDataAsBytes);
            return encodedDataAsBytes;

        }

        public static Nullable<T> ParaNullable<T>(string s) where T : struct
        {
            Nullable<T> result = new Nullable<T>();
            try
            {
                if (!string.IsNullOrEmpty(s) && s.Trim().Length > 0)
                {
                    TypeConverter conv = TypeDescriptor.GetConverter(typeof(T));
                    result = (T)conv.ConvertFrom(s);
                }
            }
            catch { }
            return result;
        }

        #region Converte uma string para hexadecimal

        /// <summary>
        /// Converte uma string para hexadecimal
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string StrParaHex(this string input)
        {
            return string.Join("", input.ToCharArray()
                .Select(f => ((int)f).ToString("x")));
        }

        public static string HexParaStr(this string hex)
        {

            byte[] raw = new byte[hex.Length / 2];
            for (int i = 0; i < raw.Length; i++)
            {
                raw[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }

            return Encoding.ASCII.GetString(raw);
        }

        #endregion

        #region Codifica uma string com o Hash SHA1

        public static string CodificaParaSHA1(this string input)
        {
            using (var sha1 = new SHA1Managed())
            {
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));

                return string.Join("", hash.Select(f => f.ToString("x2")));
            }
        }

        #endregion

        public static T StrParaEnum<T>(this string value)
        {
            if (string.IsNullOrEmpty(value))
                value = "0";

            return (T)Enum.Parse(typeof(T), value, true);
        }

        #region Conversoes de imagens

        /// <summary>
        /// Converte uma string de SVG para um vetor de bytes PNG
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] ConverteSvgParaPng(string value)
        {
            string path = System.IO.Path.GetTempPath() + "otimizacao" + DateTime.Now.Millisecond + ".svg";
            var svg = new System.IO.FileInfo(path);

            using (var sw = svg.CreateText())
            {
                sw.WriteLine(value);
            }

            using (var img = new ImageMagick.MagickImage(svg))
            {
                img.Format = ImageMagick.MagickFormat.Png;
                var arrImg = img.ToByteArray();

                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);

                return arrImg;
            }
        }

        #endregion

        #region Base64

        public static string Base64Encode(this string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(this string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        #endregion

        /// <summary>
        /// Obtém somente caracteres númericos do texto informado.
        /// </summary>
        /// <param name="texto">Texto que será formatado.</param>
        /// <returns>Texto formatado com somente caracteres numéricos.</returns>
        public static string SomenteNumero(this string texto)
        {
            if (!string.IsNullOrWhiteSpace(texto))
            {
                return new string(texto.Where(char.IsNumber).ToArray());
            }

            return texto;
        }
    }
}
