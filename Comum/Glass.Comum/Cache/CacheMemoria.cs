using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using System.Reflection;

namespace Glass.Comum.Cache
{
    public class CacheMemoria<T, ID>
    {
        private readonly MemoryCache cache;
        private readonly Func<ID, string> idItem;
        private int tempoExpiracaoSegundos;

        public CacheMemoria(string nome, int tempoExpiracaoSegundos = 3)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new ArgumentException("Nome do cache não pode ser nulo ou vazio.", "nome");
            
            cache = new MemoryCache(string.Format("{0}:{1}:cache", typeof(T).Name, nome));
            idItem = id => string.Format("{0}:{1}", typeof(T).Name, id);

            this.tempoExpiracaoSegundos = tempoExpiracaoSegundos;
        }

        public T RecuperarDoCache(ID id)
        {
            return (T)cache.Get(idItem(id));
        }

        public void AtualizarItemNoCache(T item, ID id)
        {
            cache.Set(idItem(id), item, ObterPoliticaCache());
        }

        public void AlterarTempoExpiracaoSegundos(int novoTempoExpiracaoSegundos)
        {
            tempoExpiracaoSegundos = novoTempoExpiracaoSegundos;
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
