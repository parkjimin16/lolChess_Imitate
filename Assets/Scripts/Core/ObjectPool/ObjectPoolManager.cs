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
        new ObjectInfo("ChampionFrame", 20),
        new ObjectInfo("Capsule", 20),
        new ObjectInfo("NormalProjectile", 100),
        new ObjectInfo("ItemFrame", 80),
        //new ObjectInfo("Crip", 27)
    };

    private string objectName;

    public GameObject ObjpoolParent;

    private Dictionary<string, IObjectPool<GameObject>> poolDict = new Dictionary<string, IObjectPool<GameObject>>();

    #endregion

    #region Init

    public void Initialize()
    {
        ObjpoolParent = new GameObject("Object Polling List");

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
        poolGo.transform.SetParent(ObjpoolParent.transform);
        return poolGo;
    }

    private void OnGetProjectile(GameObject projectile)
    {
        projectile.SetActive(true);

        for (int i = 0; i < projectile.transform.childCount; i++)
        {
            projectile.transform.GetChild(i).gameObject.SetActive(true);
        }

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
