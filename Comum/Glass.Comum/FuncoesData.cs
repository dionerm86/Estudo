using Colosoft;
using System;
using System.Web.UI.WebControls;

namespace Glass
{
    /// <summary>
    /// Possíveis intervalos de data.
    /// </summary>
    public enum DateInterval
    {
        /// <summary>
        /// Dia.
        /// </summary>
        Day,
        /// <summary>
        /// Dia do ano.
        /// </summary>
        DayOfYear,
        /// <summary>
        /// Hora.
        /// </summary>
        Hour,
        /// <summary>
        /// Minuto.
        /// </summary>
        Minute,
        /// <summary>
        /// Mês.
        /// </summary>
        Month,
        /// <summary>
        /// Trimestre.
        /// </summary>
        Quarter,
        /// <summary>
        /// Segundos.
        /// </summary>
        Second,
        /// <summary>
        /// Dia da semana.
        /// </summary>
        Weekday,
        /// <summary>
        /// Semana do ano.
        /// </summary>
        WeekOfYear,
        /// <summary>
        /// Ano.
        /// </summary>
        Year
    }

    /// <summary>
    /// Classe com funções de data.
    /// </summary>
    public static class FuncoesData
    {
        #region Métodos Privados

        /// <summary>
        /// Recupera o trimestre com base no mês informado.
        /// </summary>
        /// <param name="nMonth"></param>
        /// <returns></returns>
        private static int ObtemTrimestre(int nMonth)
        {
            if (nMonth <= 3)
                return 1;
            if (nMonth <= 6)
                return 2;
            if (nMonth <= 9)
                return 3;
            return 4;
        }

        /// <summary>
        /// Arredonda o valor informado.
        /// </summary>
        /// <param name="dVal"></param>
        /// <returns></returns>
        private static long Round(double dVal)
        {
            if (dVal >= 0)
                return (long)Math.Floor(dVal);
            return (long)Math.Ceiling(dVal);
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Instancia do provedor de feriados do sistema.
        /// </summary>
        private static IProvedorFeriados ProvedorFeriados
        {
            get
            {
                return Microsoft.Practices.ServiceLocation.ServiceLocator
                    .Current.GetInstance<IProvedorFeriados>();
            }
        }
        
        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Verifica se a data informada é um feriado.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool Feriado(this DateTime data)
        {
            return ProvedorFeriados.Feriado(data);
        }

        /// <summary>
        /// Verifica se a data informada é um dia útil.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool DiaUtil(this DateTime data)
        {
            return ProvedorFeriados.DiaUtil(data);
        }

        /// <summary>
        /// Recuper a descrição do dia da semana informado.
        /// </summary>
        /// <param name="dia">Dia que será recupera o nome.</param>
        /// <returns></returns>
        public static Colosoft.IMessageFormattable ObtemNomeDiaSemana(int dia)
        {
            switch (dia)
            {
                case 1: return "Domingo".GetFormatter();
                case 2: return "Segunda-Feira".GetFormatter();
                case 3: return "Terça-Feira".GetFormatter();
                case 4: return "Quarta-Feira".GetFormatter();
                case 5: return "Quinta-Feira".GetFormatter();
                case 6: return "Sexta-Feira".GetFormatter();
                case 7: return "Sábado".GetFormatter();
                default: return "".GetFormatter();
            }
        }

        /// <summary>
        /// Recupera a data informada somada dos dias uteis.
        /// </summary>
        /// <param name="data">Data base.</param>
        /// <param name="diasUteis">Quantidade de dias uteis que serão somados.</param>
        /// <returns></returns>
        public static DateTime ObtemDataDiasUteis(this DateTime data, int diasUteis)
        {
            return ProvedorFeriados.ObtemDataDiasUteis(data, diasUteis);
        }

        /// <summary>
        /// Retorna em formato dd/MM/yyyy o primeiro dia do último mês
        /// </summary>
        /// <returns></returns>
        public static string ObtemDataPrimeiroDiaUltimoMes()
        {
            return DateTime.Parse("01/" + DateTime.Now.Month + "/" + DateTime.Now.Year).AddMonths(-1).ToString("dd/MM/yyyy");
        }

        /// <summary>
        /// Retorna em formato dd/MM/yyyy o último dia do último mês
        /// </summary>
        /// <returns></returns>
        public static string ObtemDataUltimoDiaUltimoMes()
        {
            return DateTime.Parse("01/" + DateTime.Now.Month + "/" + DateTime.Now.Year).AddDays(-1).ToString("dd/MM/yyyy");
        }

        /// <summary>
        /// Retorna o último dia do mês
        /// </summary>
        /// <returns></returns>
        public static DateTime ObtemUltimoDiaMesAtual(this DateTime data)
        {
            return new DateTime(data.Year, data.Month, DateTime.DaysInMonth(data.Year, data.Month));
        }

        /// <summary>
        /// Retorna o primeiro dia do mês
        /// </summary>
        /// <returns></returns>
        public static DateTime ObtemPrimeiroDiaMesAtual(this DateTime data)
        {
            return new DateTime(data.Year, data.Month, 1);
        }

        /// <summary>
        /// Retorna nome do mês a partir do seu número
        /// </summary>
        /// <param name="mes"></param>
        /// <param name="tresLetras">Identifica se deve-se retornar apenas as três primeiras letras</param>
        /// <returns></returns>
        public static string ObtemMes(int mes, bool tresLetras)
        {
            string resultado = String.Empty;

            switch (mes)
            {
                case 1:
                    resultado = "Janeiro"; break;
                case 2:
                    resultado = "Fevereiro"; break;
                case 3:
                    resultado = "Março"; break;
                case 4:
                    resultado = "Abril"; break;
                case 5:
                    resultado = "Maio"; break;
                case 6:
                    resultado = "Junho"; break;
                case 7:
                    resultado = "Julho"; break;
                case 8:
                    resultado = "Agosto"; break;
                case 9:
                    resultado = "Setembro"; break;
                case 10:
                    resultado = "Outubro"; break;
                case 11:
                    resultado = "Novembro"; break;
                case 12:
                    resultado = "Dezembro"; break;
                default:
                    resultado = "Erro"; break;
            }

            return tresLetras ? resultado.Substring(0, 3) : resultado;
        }

        /// <summary>
        /// Calcula a diferença entre as datas informadas.
        /// </summary>
        /// <param name="interval"></param>
        /// <param name="dt1"></param>
        /// <param name="dt2"></param>
        /// <returns></returns>
        public static long DateDiff(DateInterval interval, DateTime dt1, DateTime dt2)
        {
            return DateDiff(interval, dt1, dt2, System.Globalization.DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek);
        }

        /// <summary>
        /// Calcula a diferença entre as datas informadas.
        /// </summary>
        /// <param name="interval">Intervalo que será utilizado.</param>
        /// <param name="date1">Data 1.</param>
        /// <param name="date2">Data 2</param>
        /// <param name="firstDayOfWeek">Primeiro dia da semana.</param>
        /// <returns></returns>
        public static long DateDiff(DateInterval interval, DateTime date1, DateTime date2, DayOfWeek firstDayOfWeek)
        {
            if (interval == DateInterval.Year)
                return date2.Year - date1.Year;

            if (interval == DateInterval.Month)
                return (date2.Month - date1.Month) + (12 * (date2.Year - date1.Year));

            TimeSpan ts = date2 - date1;

            if (interval == DateInterval.Day || interval == DateInterval.DayOfYear)
                return Round(ts.TotalDays);

            if (interval == DateInterval.Hour)
                return Round(ts.TotalHours);

            if (interval == DateInterval.Minute)
                return Round(ts.TotalMinutes);

            if (interval == DateInterval.Second)
                return Round(ts.TotalSeconds);

            if (interval == DateInterval.Weekday)
            {
                return Round(ts.TotalDays / 7.0);
            }

            if (interval == DateInterval.WeekOfYear)
            {
                while (date2.DayOfWeek != firstDayOfWeek)
                    date2 = date2.AddDays(-1);
                while (date1.DayOfWeek != firstDayOfWeek)
                    date1 = date1.AddDays(-1);
                ts = date2 - date1;
                return Round(ts.TotalDays / 7.0);
            }

            if (interval == DateInterval.Quarter)
            {
                double d1Quarter = ObtemTrimestre(date1.Month);
                double d2Quarter = ObtemTrimestre(date2.Month);
                double d1 = d2Quarter - d1Quarter;
                double d2 = (4 * (date2.Year - date1.Year));
                return Round(d1 + d2);
            }

            return 0;

        }

        /// <summary>
        /// Valida a data informada.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool ValidaData(string data)
        {
            DateTime dataTeste;
            return DateTime.TryParse(data, out dataTeste);

            //return dataTeste != null && dataTeste != DateTime.MinValue;
        }

        /// <summary>
        /// Recupera a lista dos meses.
        /// </summary>
        /// <returns></returns>
        public static ListItemCollection ObtemMeses()
        {
            ListItemCollection itens = new ListItemCollection();

            itens.Add(new ListItem("Janeiro", "1"));
            itens.Add(new ListItem("Fevereiro", "2"));
            itens.Add(new ListItem("Março", "3"));
            itens.Add(new ListItem("Abril", "4"));
            itens.Add(new ListItem("Maio", "5"));
            itens.Add(new ListItem("Junho", "6"));
            itens.Add(new ListItem("Julho", "7"));
            itens.Add(new ListItem("Agosto", "8"));
            itens.Add(new ListItem("Setembro", "9"));
            itens.Add(new ListItem("Outubro", "10"));
            itens.Add(new ListItem("Novembro", "11"));
            itens.Add(new ListItem("Dezembro", "12"));

            //SelectedValue = DateTime.Now.Month.ToString();

            return itens;
        }

        /// <summary>
        /// Recupera a lista dos anos.
        /// </summary>
        /// <returns></returns>
        public static ListItemCollection ObtemAnos()
        {
            ListItemCollection itens = new ListItemCollection();

            int inicio = DateTime.Now.Year - 10;
            for (int i = 0; i < 20; i++)
            {
                itens.Add(new ListItem((inicio + i).ToString(), (inicio + i).ToString()));
            }

            //ddlAno.SelectedValue = DateTime.Now.Year.ToString();

            return itens;
        }

        #endregion
    }
}
