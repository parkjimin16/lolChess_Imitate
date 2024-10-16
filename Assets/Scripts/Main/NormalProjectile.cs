using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalProjectile : MonoBehaviour
{
    public float speed = 10f;
    public int damage;
    private GameObject target;

    // 타겟을 설정하는 메서드
    public void SetTarget(GameObject target, int damage)
    {
        this.target = target;
        this.damage = damage;
    }

    private void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 direction = (target.transform.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == target) 
        {
            ChampionBase targetHealth = target.GetComponent<ChampionBase>();
            if (targetHealth != null && targetHealth.ChampionHealthController != null)
            {
                targetHealth.ChampionHealthController.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
    }
}
