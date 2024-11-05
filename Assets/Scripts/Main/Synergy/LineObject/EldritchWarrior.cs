using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EldritchWarrior : MonoBehaviour
{
    [SerializeField] private TextMeshPro txt_WarriorName;


    private float maxHp;
    private float curHp;
    private float ap_Power;
    private List<ChampionBase> enemies;

    public void Init(string name, float hp, float ap_Power, List<ChampionBase> enemy, Vector3 spawnPosition)
    {
        txt_WarriorName.text = name;
        maxHp = curHp;
        curHp = hp;
        this.ap_Power = ap_Power;
        enemies = enemy;

        transform.position = spawnPosition;

        CoroutineHelper.StartCoroutine(MoveUpwardsAndDealDamage());
    }

    private IEnumerator MoveUpwardsAndDealDamage()
    {
        float targetHeight = transform.position.y + 10f;
        float moveSpeed = 2f;


        while (transform.position.y < targetHeight)
        {
            transform.position += new Vector3(0, moveSpeed * Time.deltaTime, 0);
            yield return null;
        }

        StartCoroutine(DealDamageOverTime());
    }

    private IEnumerator DealDamageOverTime()
    {
        float duration = 10f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            foreach (var enemy in enemies)
            {
                if (enemy != null)
                {
                    enemy.ChampionHpMpController.TakeDamage(ap_Power);
                }
            }

            yield return new WaitForSeconds(2f);
            elapsedTime += 2f;
        }

        Destroy(gameObject); 
    }
}
