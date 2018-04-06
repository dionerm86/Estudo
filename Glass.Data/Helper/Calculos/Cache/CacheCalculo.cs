using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Reflection;

namespace Glass.Data.Helper.Calculos.Cache
{
    class CacheCalculo<T, ID>
    {
        private readonly MemoryCache cacheHashCode;
        private readonly Func<T, ID> idItem;
        private readonly Func<T, ID, string> transformaId;
        private int tempoExpiracaoSegundos;
        private IEnumerable<PropertyInfo> propriedades;

        public CacheCalculo(string nome, Func<T, ID> idItem, int tempoExpiracaoSegundos = 3)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new ArgumentException("Nome do cache não pode ser nulo ou vazio.", "nome");

            if (idItem == null)
                throw new ArgumentException("Identificador do objeto não pode ser nulo.", "idItem");

            Func<Type, string> nomeTipo = tipo => tipo.Name;
            var nomeTipoT = nomeTipo(typeof(T));

            cacheHashCode = new MemoryCache(string.Format("{0}:{1}:hashCode", nomeTipoT, nome));

            this.idItem = idItem;
            transformaId = (item, id) => string.Format("{0}:{1}", nomeTipo(item.GetType()), idItem(item));
            this.tempoExpiracaoSegundos = tempoExpiracaoSegundos;
        }

        public bool ItemEstaNoCache(T item)
        {
            var id = transformaId(item, idItem(item));
            var hashCodeCache = cacheHashCode.Get(id);

            if (hashCodeCache == null)
            {
                return false;
            }

            int hashCode = RecuperarHashCodeObjeto(item);
            return hashCode == (int)hashCodeCache;
        }

        public void AtualizarItemNoCache(T item)
        {
            var id = transformaId(item, idItem(item));
            int hashCode = RecuperarHashCodeObjeto(item);

            var politica = ObterPoliticaCache();
            cacheHashCode.Set(id, hashCode, politica);
        }

        public void AlterarTempoExpiracaoSegundos(int novoTempoExpiracaoSegundos)
        {
            tempoExpiracaoSegundos = novoTempoExpiracaoSegundos;
        }

        private int RecuperarHashCodeObjeto(T objeto)
        {
            var valoresParaHash = ObterPropriedades()
                .Select(propriedade => ObterValorPropriedade(propriedade, objeto))
                .ToList();

            return RecuperarHashCodeListaObjetos(valoresParaHash);
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
