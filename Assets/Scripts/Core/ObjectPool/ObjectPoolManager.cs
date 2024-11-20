using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolManager
{
    #region Field

    private class ObjectInfo
    {
        public string ObjectName;
        public int Size;

        public ObjectInfo(string name, int size)
        {
            ObjectName = name;
            Size = size;
        }
    }

    private ObjectInfo[] _poolList = new ObjectInfo[] {
        new ObjectInfo("ChampionFrame", 100),
        new ObjectInfo("Capsule", 20),
        new ObjectInfo("ProjectileFrame", 150)
    };

    private string objectName;

    private GameObject objpoolParent;

    private Dictionary<string, IObjectPool<GameObject>> poolDict = new Dictionary<string, IObjectPool<GameObject>>();

    #endregion

    #region Init

    public void Initialize()
    {
        objpoolParent = new GameObject("Object Polling List");

        for (int i = 0; i < _poolList.Length; i++)
        {
            IObjectPool<GameObject> pool = new ObjectPool<GameObject>(
                CreateProjectile,
                OnGetProjectile,
                OnReleaseProjectile,
                OnDestroyProjectile,
                maxSize: _poolList[i].Size
            );


            poolDict.Add(_poolList[i].ObjectName, pool);

            for (int j = 0; j < _poolList[i].Size; j++)
            {
                objectName = _poolList[i].ObjectName;
                ObjectPoolable poolGo = CreateProjectile().GetComponent<ObjectPoolable>();
                poolGo.Poolable.Release(poolGo.gameObject);
            }
        }
    }
    #endregion

    #region PoolMethod

    private GameObject CreateProjectile()
    {
        GameObject poolGo = Manager.Asset.InstantiatePrefab(objectName);
        poolGo.GetComponent<ObjectPoolable>().SetManagedPool(poolDict[objectName]);
        poolGo.transform.SetParent(objpoolParent.transform);
        return poolGo;
    }

    private void OnGetProjectile(GameObject projectile)
    {
        projectile.SetActive(true);
    }

    private void OnReleaseProjectile(GameObject projectile)
    {
        projectile.SetActive(false);
    }

    private void OnDestroyProjectile(GameObject projectile)
    {
        GameObject.Destroy(projectile);
    }

    public GameObject GetGo(string goName)
    {
        objectName = goName;

        return poolDict[goName].Get();
    }

    #endregion
}
