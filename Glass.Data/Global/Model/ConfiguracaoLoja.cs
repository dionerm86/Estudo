using System;
using GDA;
using Glass.Data.Helper;
using Glass.Data.DAL;
using Glass.Log;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ConfiguracaoLojaDAO))]
    [PersistenceClass("config_loja")]
    public class ConfiguracaoLoja
    {
        #region Propriedades

        [PersistenceProperty("IDCONFIGLOJA", PersistenceParameterType.IdentityKey)]
        public uint IdConfigLoja { get; set; }

        [Log("Configuração", "Descricao", typeof(ConfiguracaoDAO))]
        [PersistenceProperty("IDCONFIG")]
        public uint IdConfig { get; set; }

        [Log("Loja", "NomeFantasia", typeof(LojaDAO))]
        [PersistenceProperty("IDLOJA")]
        public uint? IdLoja { get; set; }

        [PersistenceProperty("VALORINTEIRO")]
        public int? ValorInteiro { get; set; }

        [PersistenceProperty("VALORDECIMAL")]
        public decimal? ValorDecimal { get; set; }

        [PersistenceProperty("VALORBOOLEANO")]
        public bool ValorBooleano { get; set; }

        [PersistenceProperty("VALORTEXTO")]
        public string ValorTexto { get; set; }

        #endregion

        #region Propriedades de Suporte

        [Log("Valor")]
        public string DescrValor
        {
            get
            {
                Config.TipoConfigEnum tipo = ConfigDAO.Instance.GetTipo((Config.ConfigEnum)IdConfig);

                object[] parametros = IdConfig == (int)Config.ConfigEnum.CodigoAjusteAproveitamentoCreditoIcms ?
                    new object[] { IdLoja.GetValueOrDefault() } : null;

                switch (tipo)
                {
                    case Config.TipoConfigEnum.Inteiro: 
                        return ValorInteiro.ToString();

                    case Config.TipoConfigEnum.Logico: 
                        return ValorBooleano.ToString();

                    case Config.TipoConfigEnum.Decimal:
                        return ValorDecimal.ToString();

                    case Config.TipoConfigEnum.Texto:
                    case Config.TipoConfigEnum.TextoCurto:
                    case Config.TipoConfigEnum.Data:
                        return ValorTexto;

                    case Config.TipoConfigEnum.Enum:
                    case Config.TipoConfigEnum.ListaMetodo:
                        foreach (GenericModel m in ConfigDAO.Instance.GetListForConfig(IdConfig, parametros))
                            if (m.Id == ValorInteiro)
                                return m.Descr;
                        return "";

                    case Config.TipoConfigEnum.GrupoEnumMetodo:
                        var valores = (ValorTexto ?? string.Empty).Split(';');
                        var texto = string.Empty;
                        var i = 0;

                        foreach (GenericModel m in ConfigDAO.Instance.GetListForConfig(IdConfig, parametros))
                        {
                            if (valores.Length > i && !string.IsNullOrWhiteSpace(valores[i]))
                                texto += m.Descr + ": " + valores[i] + ", ";

                            i++;
                        }

                        return texto.TrimEnd(',', ' ');
                }

                return "";
            }
        }

        #endregion
    }
}