using System;

namespace Glass
{
    /// <summary>
    /// Assinatura do provedor de feriados do sistema.
    /// </summary>
    public interface IProvedorFeriados
    {
        /// <summary>
        /// Verifica se a data informada é um feriado.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        bool Feriado(DateTime data);

        /// <summary>
        /// Verifica se a data informada é um dia útil.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        bool DiaUtil(DateTime data);

        /// <summary>
        /// Recupera a data informada somada dos dias uteis.
        /// </summary>
        /// <param name="data">Data base.</param>
        /// <param name="diasUteis">Quantidade de dias uteis que serão somados.</param>
        /// <returns></returns>
        DateTime ObtemDataDiasUteis(DateTime data, int diasUteis);
    }
}
