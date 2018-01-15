using System;
using System.Collections.Generic;
using System.Linq;
using Colosoft;
using Glass.Configuracoes;

namespace Glass.Global.Negocios.Componentes
{
    /// <summary>
    /// Implementação do fluxo de datas.
    /// </summary>
    public class DataFluxo : IDataFluxo
    {
        /// <summary>
        /// Verifica se a data informada é um feriado.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool Feriado(DateTime data)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.Feriado>()
                .Where("Mes=?mes AND Dia=?dia")
                .Add("?mes", data.Month)
                .Add("?dia", data.Day)
                .ExistsResult();
        }

        /// <summary>
        /// Verifica se a data informada é um dia útil.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool DiaUtil(DateTime data)
        {
            return (data.DayOfWeek != DayOfWeek.Saturday || Geral.ConsiderarSabadoDiaUtil) &&
                (data.DayOfWeek != DayOfWeek.Sunday || Geral.ConsiderarDomingoDiaUtil) &&
                !Feriado(data);
        }

        /// <summary>
        /// Recupera a data informada somada dos dias uteis.
        /// </summary>
        /// <param name="data">Data base.</param>
        /// <param name="diasUteis">Quantidade de dias uteis que serão somados.</param>
        /// <returns></returns>
        public DateTime ObtemDataDiasUteis(DateTime data, int diasUteis)
        {
            sbyte diasSomar = (sbyte)(diasUteis > 0 ? 1 : -1);
            diasUteis = Math.Abs(diasUteis);

            long contador = 0;

            while (contador < diasUteis)
            {
                data = data.AddDays(diasSomar);

                if (DiaUtil(data))
                    contador++;
            }

            return data;
        }

        #region Feriado

        /// <summary>
        /// Pesquisa os feriados do sistema.
        /// </summary>
        /// <returns></returns>
        public IList<Entidades.Feriado> PesquisarFeriados()
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.Feriado>()
                .OrderBy("Mes, Dia")
                .ToVirtualResult<Entidades.Feriado>();
        }

        /// <summary>
        /// Recupera as relação dos feriados do sistema.
        /// </summary>
        /// <returns></returns>
        public IList<Colosoft.IEntityDescriptor> ObtemFeriados()
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.Feriado>()
                .OrderBy("Mes, Dia")
                .ProcessResultDescriptor<Entidades.Feriado>()
                .ToList();
        }

        /// <summary>
        /// Recupera os dados feriado.
        /// </summary>
        /// <param name="idFeriado"></param>
        /// <returns></returns>
        public Entidades.Feriado ObtemFeriado(int idFeriado)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.Feriado>()
                .Where("IdFeriado=?id")
                .Add("?id", idFeriado)
                .ProcessResult<Entidades.Feriado>()
                .FirstOrDefault();
        }

        /// <summary>
        /// Salva os dados do feriado.
        /// </summary>
        /// <param name="feriado"></param>
        /// <returns></returns>
        public Colosoft.Business.SaveResult SalvarFeriado(Entidades.Feriado feriado)
        {
            feriado.Require("feriado").NotNull();

            if (feriado.IdFeriado > 0 && !feriado.ExistsInStorage)
                feriado.DataModel.ExistsInStorage = true;

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = feriado.Save(session);

                if (!resultado)
                    return resultado;

                return session.Execute(false).ToSaveResult();
            }
        }

        /// <summary>
        /// Apaga os dados do feriado.
        /// </summary>
        /// <param name="idFeriado"></param>
        /// <returns></returns>
        public Colosoft.Business.DeleteResult ApagarFeriado(int idFeriado)
        {
            var feriado = ObtemFeriado(idFeriado);

            if (feriado == null)
                return new Colosoft.Business.DeleteResult(false, "Feriado não encontrado.".GetFormatter());

            return ApagarFeriado(feriado);
        }

        /// <summary>
        /// Apaga os dados do feriado.
        /// </summary>
        /// <param name="feriado"></param>
        /// <returns></returns>
        public Colosoft.Business.DeleteResult ApagarFeriado(Entidades.Feriado feriado)
        {
            feriado.Require("feriado").NotNull();

            if (!feriado.ExistsInStorage && feriado.Uid > 0)
                feriado = ObtemFeriado(feriado.Uid);
            
            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = feriado.Delete(session);

                if (!resultado)
                    return resultado;

                return session.Execute(false).ToDeleteResult();
            }
        }

        #endregion
    }
}
