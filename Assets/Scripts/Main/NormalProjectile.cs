using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalProjectile : MonoBehaviour
{
    public float speed = 10f;
    public int damage;
    private GameObject target;

    public void SetTarget(GameObject target, int damage)
    {
        this.target = target;
        this.damage = damage;

        StartCoroutine(DestroyAfterDelay(3f));
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
            if(Manager.Stage.isCripRound)
            {
                Crip crip = target.GetComponent<Crip>();
                if(crip != null)
                {
                    crip.TakeDamage(damage);
                    //Debug.Log(damage);
                    //Debug.Log(crip.CurrentHp);
                }
            }
            else
            {
                ChampionBase targetHealth = target.GetComponent<ChampionBase>();
                if (targetHealth != null && targetHealth.ChampionHpMpController != null)
                {
                    targetHealth.ChampionHpMpController.TakeDamage(damage);
                }
            }

            Destroy(gameObject);
        }
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
