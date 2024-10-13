using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChampionProjectileHandler : ProjectileHandlerBase
{
    protected override void Start()
    {
        base.Start();
    }

    private void FixedUpdate()
    {
        TrackingTarget(TargetPosition, Speed);

        if(Vector3.Distance(transform.position, TargetPosition) < Mathf.Epsilon)
        {
            ReleaseObject();
        }
    }
}
