using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChampionAnimController : MonoBehaviour
{
    public Animator Anim;
    private ChampionBase cBase;

    public void Init(ChampionBase championBase)
    {
        cBase = championBase;
        Anim = GetComponentInChildren<Animator>();
        cBase.ChampionStateController.OnStateChanged += HandleStateChange;

    }
    private void HandleStateChange(ChampionState newState)
    {
        switch (newState)
        {
            case ChampionState.Idle:
                PlayAnimation("Idle");
                break;

            case ChampionState.Move:
                PlayAnimation("Run");
                break;

            case ChampionState.Attack:
                PlayAnimation("Attack");
                break;

            case ChampionState.Die:
                PlayAnimation("Death");
                break;

            default:
                Debug.LogWarning($"Unhandled state: {newState}");
                break;
        }
    }

    private void PlayAnimation(string animationName)
    {
        if (Anim != null)
        {
            Anim.Play(animationName);
        }
        else
        {
            Debug.LogError("Animator is not assigned.");
        }
    }

    private void OnDestroy()
    {
        // 상태 변경 이벤트에서 해제
        if (cBase != null && cBase.ChampionStateController != null)
        {
            cBase.ChampionStateController.OnStateChanged -= HandleStateChange;
        }
    }
}
