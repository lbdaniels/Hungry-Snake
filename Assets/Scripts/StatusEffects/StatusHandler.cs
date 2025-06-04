using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class StatusHandler : MonoBehaviour
{
    // Status Effects
    // Snake: Destroys tail segments over time | Else: Loses Health
    // Snake: Speeds up Bleed Out timer | Else: Loses Health and activates brief burn effect on snake that eats
    // Emits an AoE radiation effect with a range of one grid cell. 

    // States
    
    // Active Item Effect : Sets Resistance stat higher than reachable with stat boosts
    // Maxes out entity move speed
    // Used to immobilize entities that need to be stopped
    // Active Item Effect
    // Mice (Maybe Hunters)
    // Food Effect that can be activated by paths

    [SerializeReference]
    public List<BaseEffect> effects = new List<BaseEffect>();

    private void Update()
    {
        for (int i = effects.Count - 1; i >= 0; i--)
        {
            effects[i].UpdateEffect(gameObject);
            if (effects[i].IsExpired)
            {
                effects[i].Remove(gameObject);
                effects.RemoveAt(i);
            }
        }
    }

    public void ActivateEffect(BaseEffect newEffect)
    {
        newEffect.Active = true;
        newEffect.Apply(gameObject);
        effects.Add(newEffect);
    }

    public void DeactivateEffect(BaseEffect newEffect)
    {
        newEffect.TriggerExpire();
        newEffect.Active = false;
        newEffect.Remove(gameObject);
        effects.Remove(newEffect);
    }
}
