using System;
using System.Collections.Generic;
using System.Collections;

namespace Glass.Data.Helper
{
    [Serializable()]
    public class RetalhoProducaoAuxiliarCollection : IList<RetalhoProducaoAuxiliar>
    {
        private readonly IList<RetalhoProducaoAuxiliar> _list = new List<RetalhoProducaoAuxiliar>();

        #region Implementation of IEnumerable

        public IEnumerator<RetalhoProducaoAuxiliar> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Implementation of ICollection<RetalhoProducaoAuxiliar>

        public void Add(RetalhoProducaoAuxiliar item)
        {
            _list.Add(item);
        }

        public void Clear()
        {
            _list.Clear();
        }

        public bool Contains(RetalhoProducaoAuxiliar item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(RetalhoProducaoAuxiliar[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public bool Remove(RetalhoProducaoAuxiliar item)
        {
            return _list.Remove(item);
        }

        public int Count
        {
            get { return _list.Count; }
        }

        public bool IsReadOnly
        {
            get { return _list.IsReadOnly; }
        }

        #endregion

        #region Implementation of IList<RetalhoProducaoAuxiliar>

        public List<RetalhoProducaoAuxiliar> GetList()
        {
            return (List<RetalhoProducaoAuxiliar>)_list;
        }

        public int IndexOf(RetalhoProducaoAuxiliar item)
        {
            return _list.IndexOf(item);
        }

        public void Insert(int index, RetalhoProducaoAuxiliar item)
        {
            _list.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
        }

        public RetalhoProducaoAuxiliar this[int index]
        {
            get { return _list[index]; }
            set { _list[index] = value; }
        }

        #endregion

        #region Your Added Stuff

        // Add new features to your collection.

        #endregion
    }
}
