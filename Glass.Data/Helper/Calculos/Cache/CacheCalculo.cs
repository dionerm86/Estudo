using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Caching;
using System.Reflection;

namespace Glass.Data.Helper.Calculos.Cache
{
    class CacheCalculo<T>
    {
        private readonly MemoryCache cache;
        private readonly Func<T, string> id;
        private int tempoExpiracaoSegundos;
        private IEnumerable<PropertyInfo> propriedades;

        public CacheCalculo(string nome, Func<T, string> id, int tempoExpiracaoSegundos = 10)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new ArgumentException("Nome do cache não pode ser nulo ou vazio.", "nome");

            if (id == null)
                throw new ArgumentException("Identificador do objeto não pode ser nulo.", "id");

            cache = new MemoryCache(nome);
            this.id = id;
            this.tempoExpiracaoSegundos = tempoExpiracaoSegundos;
        }

        public bool ItemEstaNoCache(T item)
        {
            var idItem = id(item);
            var itemCache = cache.Get(idItem);

            if (itemCache == null)
            {
                return false;
            }

            int hashCode = RecuperarHashCodeObjeto(item);
            return hashCode == (int)itemCache;
        }

        public void AtualizarItemNoCache(T item)
        {
            var idItem = id(item);
            int hashCode = RecuperarHashCodeObjeto(item);

            cache.Set(idItem, hashCode, ObterPoliticaCache());
        }

        public void AlterarTempoExpiracaoSegundos(int novoTempoExpiracaoSegundos)
        {
            this.tempoExpiracaoSegundos = novoTempoExpiracaoSegundos;
        }

        private int RecuperarHashCodeObjeto(T objeto)
        {
            var propriedades = ObterPropriedades()
                .Select(propriedade => ObterValorPropriedade(propriedade, objeto));

            return RecuperarHashCodeListaObjetos(propriedades);
        }

        private IEnumerable<PropertyInfo> ObterPropriedades()
        {
            if (propriedades == null)
            {
                propriedades = typeof(T)
                    .GetProperties()
                    .ToList();
            }

            return propriedades;
        }

        private object ObterValorPropriedade<U>(PropertyInfo propriedade, U inspect)
        {
            try
            {
                return propriedade.GetValue(inspect, null);
            }
            catch
            {
                return null;
            }
        }

        private int RecuperarHashCodeListaObjetos<U>(IEnumerable<U> sequence)
        {
            return sequence
                .Select(item => item != null
                    ? item.GetHashCode()
                    : 0)
                .Aggregate((total, nextCode) => total ^ nextCode);
        }

        private CacheItemPolicy ObterPoliticaCache()
        {
            return new CacheItemPolicy()
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(tempoExpiracaoSegundos)
            };
        }
    }
}
