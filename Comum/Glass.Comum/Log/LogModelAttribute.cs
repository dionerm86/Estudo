using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Reflection;

namespace Glass.Log
{
    [Obsolete("Atributo de log para model não será mais utilizado. Usar o atributo LogAttribute diretamente na entidade, quando possível.")]
    public class LogModelAttribute : Attribute
    {
        #region Construtores

        /// <summary>
        /// Construtor.
        /// </summary>
        /// <param name="campo">O nome do campo que aparecerá no Log. Pode ter até 30 caracteres.</param>
        public LogModelAttribute(string campo)
            : this(TipoLog.Ambos, campo, false, null, null, null, null, false)
        {
        }

        /// <summary>
        /// Construtor.
        /// </summary>
        /// <param name="campo">O nome do campo que aparecerá no Log. Pode ter até 30 caracteres.</param>
        public LogModelAttribute(TipoLog tipoLog, string campo)
            : this(tipoLog, campo, false, null, null, null, null, false)
        {
        }

        /// <summary>
        /// Construtor.
        /// </summary>
        /// <param name="campo">O nome do campo que aparecerá no Log. Pode ter até 30 caracteres.</param>
        public LogModelAttribute(string campo, bool isCampoEFD)
            : this(TipoLog.Ambos, campo, isCampoEFD, null, null, null, null, false)
        {
        }

        /// <summary>
        /// Construtor.
        /// </summary>
        /// <param name="campo">O nome do campo que aparecerá no Log. Pode ter até 30 caracteres.</param>
        public LogModelAttribute(TipoLog tipoLog, string campo, bool isCampoEFD)
            : this(tipoLog, campo, isCampoEFD, null, null, null, null, false)
        {
        }

        /// <summary>
        /// Construtor.
        /// </summary>
        /// <param name="campo">O nome do campo que aparecerá no Log. Pode ter até 30 caracteres.</param>
        /// <param name="campoDescricaoModel">O nome da propriedade da Model que contém a descrição.</param>
        /// <param name="tipoDAO">O tipo da DAO que contém a descrição.</param>
        public LogModelAttribute(string campo, string campoDescricaoModel, Type tipoDAO)
            : this(TipoLog.Ambos, campo, false, campoDescricaoModel, tipoDAO, null, null, false)
        {
        }

        /// <summary>
        /// Construtor.
        /// </summary>
        /// <param name="campo">O nome do campo que aparecerá no Log. Pode ter até 30 caracteres.</param>
        /// <param name="campoDescricaoModel">O nome da propriedade da Model que contém a descrição.</param>
        /// <param name="tipoDAO">O tipo da DAO que contém a descrição.</param>
        public LogModelAttribute(TipoLog tipoLog, string campo, string campoDescricaoModel, Type tipoDAO)
            : this(tipoLog, campo, false, campoDescricaoModel, tipoDAO, null, null, false)
        {
        }

        /// <summary>
        /// Construtor.
        /// </summary>
        /// <param name="campo">O nome do campo que aparecerá no Log. Pode ter até 30 caracteres.</param>
        /// <param name="campoDescricaoModel">O nome da propriedade da Model que contém a descrição.</param>
        /// <param name="tipoDAO">O tipo da DAO.Instance que contém a descrição.</param>
        public LogModelAttribute(string campo, bool isCampoEFD, string campoDescricaoModel, Type tipoDAO)
            : this(TipoLog.Ambos, campo, isCampoEFD, campoDescricaoModel, tipoDAO, null, null, false)
        {
        }

        /// <summary>
        /// Construtor.
        /// </summary>
        /// <param name="campo">O nome do campo que aparecerá no Log. Pode ter até 30 caracteres.</param>
        /// <param name="campoDescricaoModel">O nome da propriedade da Model que contém a descrição.</param>
        /// <param name="tipoDAO.Instance">O tipo da DAO que contém a descrição.</param>
        public LogModelAttribute(TipoLog tipoLog, string campo, bool isCampoEFD, string campoDescricaoModel, Type tipoDAO)
            : this(tipoLog, campo, isCampoEFD, campoDescricaoModel, tipoDAO, null, null, false)
        {
        }

        /// <summary>
        /// Construtor.
        /// </summary>
        /// <param name="campo">O nome do campo que aparecerá no Log. Pode ter até 30 caracteres.</param>
        /// <param name="campoDescricaoModel">O nome da propriedade da Model que contém a descrição.</param>
        /// <param name="tipoDAO.Instance">O tipo da DAO que contém a descrição.</param>
        /// <param name="campoIdModel">O nome da propriedade da Model que contém o ID.</param>
        /// <param name="nomeMetodoDAO">O nome do método que retorna o item da DAO.</param>
        /// <param name="incluirValorPrimeiroParametro">O valor do item deve ser incluído como o primeiro parâmetro?</param>
        /// <param name="parametrosMetodoDAO">Os parâmetros do método de retorno.</param>
        public LogModelAttribute(string campo, string campoDescricaoModel, Type tipoDAO, string campoIdModel, string nomeMetodoDAO,
            bool incluirValorPrimeiroParametro, params object[] parametrosMetodoDAO)
            : this(TipoLog.Ambos, campo, false, campoDescricaoModel, tipoDAO, campoIdModel, nomeMetodoDAO, incluirValorPrimeiroParametro, parametrosMetodoDAO)
        {
        }

        /// <summary>
        /// Construtor.
        /// </summary>
        /// <param name="campo">O nome do campo que aparecerá no Log. Pode ter até 30 caracteres.</param>
        /// <param name="campoDescricaoModel">O nome da propriedade da Model que contém a descrição.</param>
        /// <param name="tipoDAO">O tipo da DAO que contém a descrição.</param>
        /// <param name="campoIdModel">O nome da propriedade da Model que contém o ID.</param>
        /// <param name="nomeMetodoDAO">O nome do método que retorna o item da DAO.</param>
        /// <param name="incluirValorPrimeiroParametro">O valor do item deve ser incluído como o primeiro parâmetro?</param>
        /// <param name="parametrosMetodoDAO">Os parâmetros do método de retorno.</param>
        public LogModelAttribute(TipoLog tipoLog, string campo, string campoDescricaoModel, Type tipoDAO, string campoIdModel,
            string nomeMetodoDAO, bool incluirValorPrimeiroParametro, params object[] parametrosMetodoDAO)
            : this(tipoLog, campo, false, campoDescricaoModel, tipoDAO, campoIdModel, nomeMetodoDAO, incluirValorPrimeiroParametro, parametrosMetodoDAO)
        {
        }

        /// <summary>
        /// Construtor.
        /// </summary>
        /// <param name="tipoLog">O tipo de log a que o atributo pertence.</param>
        /// <param name="campo">O nome do campo que aparecerá no Log. Pode ter até 30 caracteres.</param>
        /// <param name="campoDescricaoModel">O nome da propriedade da Model que contém a descrição.</param>
        /// <param name="tipoDAO">O tipo da DAO que contém a descrição.</param>
        /// <param name="campoIdModel">O nome da propriedade da Model que contém o ID.</param>
        /// <param name="nomeMetodoDAO">O nome do método que retorna o item da DAO.</param>
        /// <param name="incluirValorPrimeiroParametro">O valor do item deve ser incluído como o primeiro parâmetro?</param>
        /// <param name="parametrosMetodoDAO">Os parâmetros do método de retorno.</param>
        public LogModelAttribute(string campo, bool isCampoEFD, string campoDescricaoModel, Type tipoDAO, string campoIdModel,
            string nomeMetodoDAO, bool incluirValorPrimeiroParametro, params object[] parametrosMetodoDAO)
            : this(TipoLog.Ambos, campo, isCampoEFD, campoDescricaoModel, tipoDAO, campoIdModel, nomeMetodoDAO, incluirValorPrimeiroParametro, parametrosMetodoDAO)
        {
        }

        /// <summary>
        /// Construtor.
        /// </summary>
        /// <param name="tipoLog">O tipo de log a que o atributo pertence.</param>
        /// <param name="campo">O nome do campo que aparecerá no Log. Pode ter até 30 caracteres.</param>
        /// <param name="campoDescricaoModel">O nome da propriedade da Model que contém a descrição.</param>
        /// <param name="tipoDAO">O tipo da DAO que contém a descrição.</param>
        /// <param name="campoIdModel">O nome da propriedade da Model que contém o ID.</param>
        /// <param name="nomeMetodoDAO">O nome do método que retorna o item da DAO.</param>
        /// <param name="incluirValorPrimeiroParametro">O valor do item deve ser incluído como o primeiro parâmetro?</param>
        /// <param name="parametrosMetodoDAO.Instance">Os parâmetros do método de retorno.</param>
        public LogModelAttribute(TipoLog tipoLog, string campo, bool isCampoEFD, string campoDescricaoModel, Type tipoDAO, string campoIdModel,
            string nomeMetodoDAO, bool incluirValorPrimeiroParametro, params object[] parametrosMetodoDAO)
        {
            Campo = campo;
            TipoLog = tipoLog;
            IsCampoEFD = isCampoEFD;
            TipoDAO = tipoDAO;
            NomeMetodoDAO = nomeMetodoDAO;
            IncluirValorPrimeiroParametro = incluirValorPrimeiroParametro;
            ParametrosMetodoDAO = parametrosMetodoDAO;
            CampoIdModel = campoIdModel;
            CampoDescricaoModel = campoDescricaoModel;
        }

        #endregion

        #region Propriedades

        public TipoLog TipoLog { get; set; }

        public string Campo { get; set; }

        public bool IsCampoEFD { get; set; }

        public Type TipoDAO { get; set; }

        public string NomeMetodoDAO { get; set; }

        public bool IncluirValorPrimeiroParametro { get; set; }

        public object[] ParametrosMetodoDAO { get; set; }

        public string CampoDescricaoModel { get; set; }

        public string CampoIdModel { get; set; }

        public bool BuscarValor
        {
            get { return CampoDescricaoModel != null && TipoDAO != null; }
        }

        #endregion

        #region Métodos de recuperação de valor

        /// <summary>
        /// Retorna o valor formatado.
        /// </summary>
        /// <param name="value">O valor que será formatado.</param>
        /// <returns>Uma string com o valor formatado.</returns>
        internal static string FormatValue(object value)
        {
            if (value is bool)
                return (bool)value ? "Sim" : "Não";
            else
                return value != null ? value.ToString() : null;
        }

        /// <summary>
        /// Retorna o valor do campo que será usado no Log.
        /// </summary>
        /// <param name="propriedade">A propriedade que será retornada.</param>
        /// <param name="item">O item que contém o valor.</param>
        /// <returns>O valor que será usado no Log.</returns>
        public string GetValue(PropertyInfo propriedade, object item)
        {
            // Recupera o valor do item
            object valorItem = item != null ? propriedade.GetValue(item, null) : null;

            // Verifica se deve ser buscado o valor (ou se o valor do item é nulo)
            if (!BuscarValor || item == null || valorItem == null)
                return FormatValue(valorItem);
            else
            {
                // Variáveis de suporte
                string nomeMetodo = NomeMetodoDAO;
                Type tipoDAO = TipoDAO;

                int offsetParametros = IncluirValorPrimeiroParametro ? 1 : 0;
                object[] parametros = new object[ParametrosMetodoDAO.Length + offsetParametros];
                if (IncluirValorPrimeiroParametro) parametros[0] = valorItem;
                Array.Copy(ParametrosMetodoDAO, 0, parametros, offsetParametros, ParametrosMetodoDAO.Length);

                // Verifica se o método 'GetElementByPrimaryKey' (padrão) deve ser usado
                if (parametros.Length == 0 && nomeMetodo == null)
                {
                    nomeMetodo = "GetElementByPrimaryKey";
                    parametros = new object[] { valorItem };
                }

                // Carrega os tipos dos parâmetros (usados para recuperar o método)
                Type[] tiposParametros = new Type[parametros.Length];
                for (int i = 0; i < parametros.Length; i++)
                    tiposParametros[i] = parametros[i].GetType();

                // Recupera a DAO
                PropertyInfo instance = tipoDAO.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
                object DAO = instance != null ? instance.GetValue(null, null) : null;

                // Recupera o método
                MethodInfo metodo = tipoDAO.GetMethod(nomeMetodo, tiposParametros);
                while (metodo == null && tipoDAO.BaseType != null)
                {
                    tipoDAO = tipoDAO.BaseType;
                    metodo = tipoDAO.GetMethod(nomeMetodo, tiposParametros);
                }

                // Se o método não for encontrado retorna o valor da propriedade
                if (metodo == null)
                    return FormatValue(valorItem);

                // Recupera a model
                object model = null;

                try
                {
                    model = metodo.Invoke(DAO, parametros);
                    if (model is IEnumerable)
                    {
                        // Se não houver o nome do campo do ID
                        if (String.IsNullOrEmpty(CampoIdModel))
                            return FormatValue(valorItem);

                        // Variável temporária
                        object temp = null;

                        // Procura a model na enumeração
                        foreach (object m in (IEnumerable)model)
                        {
                            // Recupera o ID da model
                            object valorModel = m.GetType().GetProperty(CampoIdModel).GetValue(m, null);

                            // Cria um objeto comparador
                            object comparador = typeof(Comparer<>).MakeGenericType(propriedade.PropertyType).GetProperty("Default").GetValue(null, null);

                            // Compara os 2 valores
                            int c = (int)comparador.GetType().GetMethod("Compare").Invoke(comparador, new object[] { 
                                Conversoes.ConverteValor(propriedade.PropertyType, valorModel), valorItem });

                            if (c == 0)
                            {
                                temp = m;
                                break;
                            }
                        }

                        // Retorna caso a model não seja encontrada
                        if (temp == null)
                            return FormatValue(valorItem);

                        // Seleciona a model
                        model = temp;
                    }
                }
                catch
                {
                    model = null;
                }

                // Retorna caso a model não seja encontrada
                if (model == null)
                    return null;

                // Recupera e retorna o valor
                PropertyInfo propInfo = model.GetType().GetProperty(CampoDescricaoModel);
                object value = propInfo != null ? propInfo.GetValue(model, null) : null;
                return FormatValue(value);
            }
        }

        #endregion
    }
}