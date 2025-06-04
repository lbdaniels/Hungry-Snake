using UnityEngine;

public abstract class BaseEffect
{
    public bool Active { get; set; }
    public bool UseDuration { get; protected set; }
    public float Duration { get; protected set; }

    protected bool expiredByTrigger = false;

    public BaseEffect(float duration)
    {
        Duration = duration;
        Active = false;
        UseDuration = true;
    }

    public BaseEffect()
    {
        Duration = 0f;
        Active = false;
        UseDuration = false;
    }

    public abstract void Apply(GameObject target);

    public virtual void UpdateEffect(GameObject target)
    {
        if (UseDuration)
        {
            Duration -= Time.deltaTime;
        }
    }

    public abstract void Remove(GameObject target);

    public virtual bool IsExpired
    {
        get { return UseDuration ? (Duration <= 0f) : expiredByTrigger; }
    }

    public virtual void TriggerExpire()
    {
        expiredByTrigger = true;
    }
}
