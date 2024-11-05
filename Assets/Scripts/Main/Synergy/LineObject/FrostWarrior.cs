using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrostWarrior : MonoBehaviour
{
    public float MaxHp;
    public float CurHp;
    public float Power;
    public float MoveSpeed = 3f;

    private void Update()
    {
        if(CurHp <= 0)
            Destroy(this);
    }

    public void Init(float maxHp, float power,GameObject target)
    {
        MaxHp = maxHp;
        CurHp = MaxHp;
        Power = power;

        MoveAndAttack(target);
    }

    public void MoveAndAttack(GameObject target)
    {
        StartCoroutine(MoveToTargetAndAttack(target));
    }

    private IEnumerator MoveToTargetAndAttack(GameObject target)
    {
        while (Vector3.Distance(transform.position, target.transform.position) > 1f)
        {
            Vector3 direction = (target.transform.position - transform.position).normalized;
            transform.position += direction * MoveSpeed * Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(0.3f);
        DealDamage(target);
        Destroy(this.gameObject);
    }

    private void DealDamage(GameObject target)
    {
        var targetHp = target.GetComponent<ChampionBase>(); 
        if (targetHp != null)
        {
            targetHp.ChampionHpMpController.TakeDamage(Power); 
        }
    }
}
