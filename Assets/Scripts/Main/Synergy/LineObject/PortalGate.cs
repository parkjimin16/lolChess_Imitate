using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalGate : MonoBehaviour
{
    private float totalPower;
    private float ratio;
    private Coroutine shootingCoroutine;

    [SerializeField] private GameObject bombObject;

    public void Init(int championLevel, GameObject target)
    {
        ratio = (1 + (championLevel * 0.08f));

        if (shootingCoroutine != null)
        {
            StopCoroutine(shootingCoroutine);
        }
        shootingCoroutine = StartCoroutine(ShootBombsAtInterval(championLevel, target));
    }
    private IEnumerator ShootBombsAtInterval(int championLevel, GameObject target)
    {
        while (true)
        {
            yield return new WaitForSeconds(3f);

            if (bombObject != null && target != null)
            {
                Debug.Log("Â÷¿ø¹® ÆøÅº");

                GameObject bomb = Instantiate(bombObject, transform.position, Quaternion.identity);

                PortalBomb portalBomb = bomb.GetComponent<PortalBomb>();
                if (portalBomb != null)
                {
                    portalBomb.SetTarget(ratio ,target);
                }
            }
        }
    }

    private void OnDisable()
    {
        if (shootingCoroutine != null)
        {
            StopCoroutine(shootingCoroutine);
            shootingCoroutine = null;
        }
    }
}
