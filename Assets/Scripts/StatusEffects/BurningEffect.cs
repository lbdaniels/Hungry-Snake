using UnityEngine;

public class BurningEffect : BaseEffect
{
    private float damagePerTick;
    private float tickInterval = 1f;
    private float tickTimer;

    public BurningEffect(float duration, float damage, float resistance) : base(duration)
    {
        damagePerTick = damage;
        tickInterval += (tickInterval * resistance);
        tickTimer = tickInterval;
    }

    public override void Apply(GameObject target)
    {
        Debug.Log($"{target.name} started burning! Taking {damagePerTick} damage per tick!");
    }

    public override void UpdateEffect(GameObject target)
    {
        base.UpdateEffect(target);

        // For debugging, log the current duration.
        Debug.Log("Remaining Duration: " + Duration);

        tickTimer -= Time.deltaTime;
        if (tickTimer <= 0)
        {
            Debug.Log($"{target.name} takes {damagePerTick} burning damage after {tickInterval} second(s)");
            tickTimer = tickInterval;
        }
    }

    public override void Remove(GameObject target)
    {
        Debug.Log($"{target.name} is no longer burning.");
    }
}
