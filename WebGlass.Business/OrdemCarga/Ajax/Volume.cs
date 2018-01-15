using System;

namespace WebGlass.Business.OrdemCarga.Ajax
{
    public interface IVolume
    {
        string FecharVolume(string idVolume);
        //string ReabrirVolume(string idVolume);
    }

    internal class Volume : IVolume
    {
        /// <summary>
        /// Fecha um volume
        /// </summary>
        /// <param name="idVolume"></param>
        /// <returns></returns>
        public string FecharVolume(string idVolume)
        {
            try
            {
                uint idVol = Glass.Conversoes.StrParaUint(idVolume);
                if (idVol == 0)
                    throw new Exception("Nenhum volume informado.");

                Fluxo.VolumeFluxo.Instance.FecharVolume(idVol);

                return "Ok;";
            }
            catch (Exception ex)
            {
                return "Erro;" + ex.Message;
            }
        }

        ///// <summary>
        ///// Reabre um volume
        ///// </summary>
        ///// <param name="idVolume"></param>
        ///// <returns></returns>
        //public string ReabrirVolume(string idVolume)
        //{
        //    try
        //    {
        //        uint idVol = Glass.Conversoes.StrParaUint(idVolume);
        //        if (idVol == 0)
        //            throw new Exception("Nenhum volume informado.");

        //        Fluxo.VolumeFluxo.Instance.ReabrirVolume(idVol);

        //        return "Ok;";
        //    }
        //    catch (Exception ex)
        //    {
        //        return "Erro;" + ex.Message;
        //    }
        //}
    }
}
