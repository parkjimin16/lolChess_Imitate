using System.Collections;
using UnityEngine;

public class NormalProjectile : ObjectPoolable
{
    public float speed = 10f;
    public int damage;
    [SerializeField] private GameObject target;

    public void SetTarget(GameObject target, int damage)
    {
        this.target = target;
        this.damage = damage;

        StartCoroutine(DestroyAfterDelay(3f));
    }

    private void Update()
    {
        if (target == null || !Manager.Stage.IsBattleOngoing)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 direction = (target.transform.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (target.gameObject.CompareTag($"{other.gameObject.tag}")) 
        {
            if (Manager.Stage.isCripRound)
            {
                Crip crip = target.GetComponent<Crip>();
                if(crip != null)
                {
                    crip.TakeDamage(damage);
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
