using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileHandlerBase : ObjectPoolable
{
    [HideInInspector]
    public int Damage;
    public GameObject ProjectileObj;
    [HideInInspector]
    public DamageType DamageTypeValue = DamageType.Normal;

    public float Speed = 0.1f;

    public Vector3 TargetPosition;

    public LayerMask TargetLayerMask;

    private bool has;

    protected virtual void Start()
    {
        if (ProjectileObj != null)
        {
            GameObject go = Instantiate(ProjectileObj, transform.position, Quaternion.identity, gameObject.transform);
            int index = go.name.IndexOf("(Clone)");
            if (index > 0)
            {
                go.name = go.name.Substring(0, index);
            }
        }
    }

    protected void TrackingTarget(Vector3 targetPosition, float speed)
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed);
    }

    protected virtual void OnTriggerEnter(Collider collision)
    {
        if (TargetLayerMask.value == (TargetLayerMask.value | (1 << collision.gameObject.layer)) || Vector2.Distance(transform.position, TargetPosition) < Mathf.Epsilon)
        {
            ReleaseObject();
        }
    }

    protected virtual void OnTriggerExit(Collider collision)
    {
        if (TargetLayerMask.value == (TargetLayerMask.value | (1 << collision.gameObject.layer)))
        {
            collision.gameObject.GetComponent<IDamageable>().TakeDamage(Damage, DamageTypeValue);
        }
    }


    public void SetProjectile(GameObject _ProjectileObj, int Damage)
    {
        this.Damage = Damage;
        this.ProjectileObj = _ProjectileObj;

        has = false;
        if (ProjectileObj != null && transform.childCount > 0)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (this.transform.GetChild(i).gameObject.name == ProjectileObj.name)
                {
                    has = true;
                    this.transform.GetChild(i).gameObject.SetActive(true);
                    break;
                }
            }
            if (!has)
            {
                GameObject go = Instantiate(ProjectileObj, transform.position, Quaternion.identity, gameObject.transform);
                int index = go.name.IndexOf("(Clone)");
                if (index > 0)
                {
                    go.name = go.name.Substring(0, index);
                }
            }
        }
    }
}
