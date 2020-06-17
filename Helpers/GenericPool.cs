using System.Collections.Generic;
using UnityEngine;

namespace Extensions.Generics.ItemPool
{
    /// <summary>
    /// Please save me as IItemPool
    /// </summary>
    /// <typeparam name="T">Class</typeparam>
    public class GenericPool<T> : IItemPool<T> where T : MonoBehaviour
    {
        private readonly T prefab;
        private readonly Transform parent;
        private List<T> poolList;

        /// <summary>
        /// GenericPool constructor
        /// </summary>
        /// <param name="prefab">Object of the item this pool will pool</param>
        public GenericPool(T prefab)
        {
            this.prefab = prefab;

            poolList = new List<T>();
            poolList.Add(CreateNew());
        }

        /// <summary>
        /// GenericPool constructor
        /// </summary>
        /// <param name="prefab">Object of the item this pool will pool</param>
        /// <param name="parent">Parent of the objects</param>
        public GenericPool(T prefab, Transform parent)
        {
            this.prefab = prefab;
            this.parent = parent;

            poolList = new List<T>();
            poolList.Add(CreateNew());
        }

        /// <summary>
        /// Gets you the pool item
        /// </summary>
        /// <returns>Item that has been pooled</returns>
        public T Get()
        {
            for (int i = 0; i < poolList.Count; i++)
            {
                if (!poolList[i].gameObject.activeInHierarchy)
                {
                    poolList[i].gameObject.SetActive(true);
                    return poolList[i];
                }
            }

            return CreateNew();
        }

        public List<T> GetActiveItems()
        {
            List<T> activeList = new List<T>();

            for (int i = 0; i < poolList.Count; i++)
            {
                if (!poolList[i].gameObject.activeInHierarchy) continue;

                activeList.Add(poolList[i]);
            }

            return activeList;
        }

        private T CreateNew()
        {
            T obj = (parent == null)? Object.Instantiate(prefab) : Object.Instantiate(prefab, parent);
            obj.gameObject.SetActive(false);
            poolList.Add(obj);
            return obj;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">Class</typeparam>
    public interface IItemPool<T>
    {
        /// <summary>
        /// Gets you the pool item
        /// </summary>
        /// <returns>Item that has been pooled</returns>
        T Get();

        /// <summary>
        /// Gets you all active items
        /// </summary>
        /// <returns>List of active items</returns>
        List<T> GetActiveItems();
    }
}
