using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BleedingEffect : BaseEffect
{
    private GameObject player;
    private TailController tailController;

    private float tickInterval = 5f;
    private float tickTimer;
    private List<GameObject> tailSegments = new List<GameObject>();
    private int tailSegmentCount;

    public BleedingEffect(float resistance) : base()
    {
        tickInterval += (tickInterval * resistance);
        tickTimer = tickInterval;
    }

    public override void Apply(GameObject target)
    {
        Debug.Log($"{target.name} started bleeding!");
        if (target.name == "Player" )
        {
            player = target;
            tailController = player.GetComponent<TailController>();
        }
    }

    public override void UpdateEffect(GameObject target)
    {
        tickTimer -= Time.deltaTime;
        if (tickTimer <= 0)
        {
            tailSegments = tailController.tailSegments;

            if (tailSegments.Count > 0)
            {
                int lastTailIndex = tailSegments.Count - 1;

                Debug.Log($"{tailSegments[lastTailIndex].name} being destroyed by bleed damage");
                UnityEngine.Object.Destroy(tailSegments[lastTailIndex]);
                tailController.RemoveTailSegment(tailSegments[lastTailIndex]);
            }

            else
            {
                Debug.Log($"{target.name} has no tail segments left and is killed by bleed damage");
            }

            tickTimer = tickInterval;
        }
    }

    public override void Remove(GameObject target)
    {
        Debug.Log($"{target.name} is no longer bleeding.");
    }

    public override bool IsExpired
    {
        get { return UseDuration ? (Duration <= 0f) : expiredByTrigger; }
    }

    public override void TriggerExpire()
    {
        expiredByTrigger = true;
    }
}
