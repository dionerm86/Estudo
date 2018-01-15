using GDA;
using Glass.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Data.DAL
{
    public class IestUfLojaDAO : BaseDAO<IestUfLoja, IestUfLojaDAO>
    {
        public string ObterIestUfLoja(uint idLoja, string nomeUf)
        {
            return ObtemValorCampo<string>("InscEstSt", string.Format("IdLoja={0} AND NomeUf=?nomeUf", idLoja), new GDAParameter("?nomeUf", nomeUf));
        }
    }
}
