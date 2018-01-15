using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.UI.WebControls;
using System.Collections;

namespace Glass
{
    public static class MetodosExtensao
    {
        #region Método de agrupamento de listas

        #region Suporte

        private class ChaveAgrupar
        {
            public object[] Itens;

            public override bool Equals(object obj)
            {
                ChaveAgrupar comp = obj as ChaveAgrupar;
                if (comp == null || comp.Itens.Length != Itens.Length)
                    return false;

                for (int i = 0; i < Itens.Length; i++)
                    if ((Itens[i] == null || comp.Itens[i] == null) && Itens[i] != comp.Itens[i])
                        return false;
                    else if (Itens[i] != null && comp.Itens[i] != null && Itens[i].GetType().IsArray && comp.Itens[i].GetType().IsArray)
                    {
                        List<object> t = new List<object>();
                        foreach (object o in (Itens[i] as IEnumerable))
                            t.Add(o);

                        List<object> c = new List<object>();
                        foreach (object o in (comp.Itens[i] as IEnumerable))
                            c.Add(o);

                        if (t.Count != c.Count)
                            return false;

                        for (int j = 0; j < t.Count; j++)
                            if ((t[j] == null || c[j] == null) && t[j] != c[j])
                                return false;
                            else if (t[j] != null && c[j] != null && !t[j].Equals(c[j]))
                                return false;
                    }
                    else if (Itens[i] != null && comp.Itens[i] != null && !Itens[i].Equals(comp.Itens[i]))
                        return false;

                return true;
            }

            public override int GetHashCode()
            {
                int codigo = 0;
                for (int i = 0; i < Itens.Length; i++)
                    if (Itens[i] != null && Itens[i].GetType().IsValueType)
                        codigo += Conversoes.ConverteValor<int>(Itens[i]);

                return codigo;
            }
        }

        public static T Clonar<T>(T item, params object[] parametros)
        {
            Type[] tiposParametros = new Type[parametros.Length];
            for (int i = 0; i < parametros.Length; i++)
                tiposParametros[i] = parametros[i].GetType();

            ConstructorInfo[] ci = typeof(T).GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance |
                BindingFlags.Static);

            if (ci == null)
                throw new Exception("Não foi encontrado construtor para o tipo " + typeof(T).Name);

            ci = Array.FindAll(ci, x =>
            {
                ParameterInfo[] p = x.GetParameters();

                if (p.Length != tiposParametros.Length)
                    return false;

                for (int i = 0; i < p.Length; i++)
                    if (p[i].ParameterType != tiposParametros[i])
                        return false;

                return true;
            });

            if (ci.Length == 0)
                throw new Exception("Não foi encontrado construtor para o tipo " + typeof(T).Name);

            ConstructorInfo c = ci[0];

            T retorno = Conversoes.ConverteValor<T>(c.Invoke(parametros));
            if (retorno == null)
                throw new Exception("Construtor inválido para o tipo " + typeof(T).Name);

            foreach (PropertyInfo p in typeof(T).GetProperties(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                try
                {
                    p.SetValue(retorno, p.GetValue(item, null), null);
                }
                catch { }
            }

            return retorno;
        }

        private static PropertyInfo[] GetPropriedades(object item, string[] propriedades)
        {
            List<PropertyInfo> p = new List<PropertyInfo>(item.GetType().GetProperties(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance));

            for (int i = p.Count - 1; i >= 0; i--)
            {
                bool encontrou = false;

                for (int j = 0; j < propriedades.Length; j++)
                    if (p[i].Name == propriedades[j])
                    {
                        encontrou = true;
                        break;
                    }

                if (!encontrou)
                    p.RemoveAt(i);
            }

            return p.ToArray();
        }

        private static object[] GetValues(object item, PropertyInfo[] propriedades)
        {
            // Monta o vetor de retorno
            object[] retorno = new object[propriedades.Length];

            for (int i = 0; i < retorno.Length; i++)
                retorno[i] = propriedades[i].GetValue(item, null);

            // Retorna o vetor com os valores
            return retorno;
        }

        #endregion

        /// <summary>
        /// Agrupa um IEnumerable&lt;T&gt;.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="itens">Os itens que serão agrupados.</param>
        /// <param name="propriedadesAgrupar">As propriedades que serão usadas como agrupamento.</param>
        /// <param name="propriedadesSomar">As propriedades que serão somadas.</param>
        /// <param name="parametrosConstrutorT">Os parâmetros para o método construtor de um objeto do tipo T.</param>
        /// <returns></returns>
        public static IEnumerable<T> Agrupar<T>(IEnumerable<T> itens, string[] propriedadesAgrupar,
            string[] propriedadesSomar, params object[] parametrosConstrutorT)
        {
            // Retorna a lista se não houver propriedades para agrupar
            if (itens == null || propriedadesAgrupar == null || propriedadesAgrupar.Length == 0)
                return itens;

            Dictionary<ChaveAgrupar, T> agrupamento = new Dictionary<ChaveAgrupar, T>();
            PropertyInfo[] pa = null, ps = null;

            foreach (T item in itens)
            {
                if (pa == null)
                    pa = GetPropriedades(item, propriedadesAgrupar);

                if (ps == null)
                {
                    List<PropertyInfo> p = new List<PropertyInfo>(GetPropriedades(item, propriedadesSomar));
                    ps = p.FindAll(x => x.PropertyType.IsValueType || x.PropertyType == typeof(string)).ToArray();
                }

                ChaveAgrupar chave = new ChaveAgrupar();
                chave.Itens = GetValues(item, pa);

                if (!agrupamento.ContainsKey(chave))
                {
                    T novo = Clonar<T>(item, parametrosConstrutorT);
                    agrupamento.Add(chave, novo);
                }
                else
                {
                    foreach (PropertyInfo p in ps)
                    {
                        if (p.PropertyType.IsValueType)
                        {
                            decimal valor = Conversoes.ConverteValor<decimal>(p.GetValue(agrupamento[chave], null));
                            valor += Conversoes.ConverteValor<decimal>(p.GetValue(item, null));

                            // Seta o valor da propriedade somente se ela tiver permissão para isso.
                            if (p.CanWrite)
                                p.SetValue(agrupamento[chave], Conversoes.ConverteValor(p.PropertyType, valor), null);
                        }
                        else if (p.PropertyType == typeof(string))
                        {
                            string valorAtual = p.GetValue(agrupamento[chave], null) as string;
                            string valorNovo = p.GetValue(item, null) as string;

                            if (valorAtual == null) valorAtual = "";
                            if (valorNovo == null) valorNovo = "";

                            // Seta o valor da propriedade somente se ela tiver permissão para isso.
                            if (p.CanWrite)
                                p.SetValue(agrupamento[chave], (valorAtual + "," + valorNovo).TrimStart(','), null);
                        }
                    }
                }
            }

            T[] retorno = new T[agrupamento.Count];
            agrupamento.Values.CopyTo(retorno, 0);

            return retorno as IEnumerable<T>;
        }

        #endregion

        #region Método de ordenação de listas

        /// <summary>
        /// Ordena um IEnumerable&lt;T&gt;.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="itens"></param>
        /// <param name="propriedadeOrdenar"></param>
        /// <param name="direcao"></param>
        /// <returns></returns>
        public static IEnumerable<T> Ordenar<T>(IEnumerable<T> itens, params string[] propriedadesOrdenar)
        {
            return Ordenar(itens, SortDirection.Ascending, propriedadesOrdenar);
        }

        /// <summary>
        /// Ordena um IEnumerable&lt;T&gt;.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="itens"></param>
        /// <param name="propriedadeOrdenar"></param>
        /// <param name="direcao"></param>
        /// <returns></returns>
        public static IEnumerable<T> Ordenar<T>(IEnumerable<T> itens, SortDirection direcao, params string[] propriedadesOrdenar)
        {
            List<T> lista = new List<T>(itens ?? new T[0]);
            if (lista.Count == 0)
                return itens;

            PropertyInfo[] p = GetPropriedades(lista[0], propriedadesOrdenar);
            int comparar = direcao == SortDirection.Ascending ? 1 : -1;

            lista.Sort(
                delegate(T x, T y)
                {
                    int c = 0;
                    for (int i = 0; i < p.Length; i++)
                    {
                        object comp = typeof(Comparer<>).MakeGenericType(p[i].PropertyType).
                            GetProperty("Default").GetValue(null, null);

                        object valorX = p[i].GetValue(x, null);
                        object valorY = p[i].GetValue(y, null);

                        c = (int)comp.GetType().GetMethod("Compare").Invoke(comp, new object[] { valorX, valorY });
                        if (c != 0)
                            break;
                    }

                    return c * comparar;
                }
            );

            return lista as IEnumerable<T>;
        }

        #endregion

        #region Conversão de IEnumerable<> para vetor

        /// <summary>
        /// Converte um IEnumerable&lt;T&gt; para um vetor.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static T[] ToArray<T>(IEnumerable<T> list)
        {
            if (list == null)
                return new T[0];

            return new List<T>(list).ToArray();
        }

        #endregion
    }
}
