using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalBomb : MonoBehaviour
{
    private GameObject target;
    private float speed = 5f;
    private float power;

    public void SetTarget(float damage, GameObject target)
    {
        power = damage;
        this.target = target;
    }

    private void Update()
    {
        if (target != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);

            float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
            if (distanceToTarget < 0.5f)
            {
                Explode(); 
            }
        }
    }



    private void Explode()
    {
        var targetHp = target.GetComponent<ChampionBase>();
        if (targetHp != null)
        {
            targetHp.ChampionHpMpController.TakeDamage(power);
        }

        Destroy(gameObject); 
    }

}
