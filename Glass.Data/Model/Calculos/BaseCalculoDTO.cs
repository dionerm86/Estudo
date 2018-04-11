using Glass.Comum.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Data.Model.Calculos
{
    abstract class BaseCalculoDTO
    {
        /// <summary>
        /// Recupera um item a partir do cache, se existir. Caso não exista, cria um mecanismo para recuperação do
        /// dado a partir do banco e depois atualiza o cache.
        /// </summary>
        protected Lazy<T> ObterUsandoCache<T, ID>(CacheMemoria<T, ID> cache, ID id, Func<T> recuperarDoBanco)
            where T : class, new()
        {
            var itemCache = cache.RecuperarDoCache(id);

            if (itemCache != null)
            {
                return new Lazy<T>(() => itemCache);
            }

            return new Lazy<T>(() =>
            {
                itemCache = cache.RecuperarDoCache(id);

                if (itemCache == null)
                {
                    try
                    {
                        itemCache = recuperarDoBanco() ?? new T();
                    }
                    catch
                    {
                        itemCache = new T();
                    }

                    cache.AtualizarItemNoCache(itemCache, id);
                }

                return itemCache;
            });
        }
    }
}
