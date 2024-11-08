using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrostWarrior : MonoBehaviour
{
    private float maxHp;
    private float curHp;
    private float power;
    public float MoveSpeed = 3f;

    private void Update()
    {
        if(curHp <= 0)
            Destroy(this);
    }

    public void Init(float _maxHp, float _power,GameObject target)
    {
        maxHp = _maxHp;
        curHp = maxHp;
        power = _power;

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
        SetDamage(target);
        Destroy(this.gameObject);
    }

    private void SetDamage(GameObject target)
    {
        var targetHp = target.GetComponent<ChampionBase>(); 
        if (targetHp != null)
        {
            targetHp.ChampionHpMpController.TakeDamage(power); 
        }
    }
}
