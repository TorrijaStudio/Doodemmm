using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class desertBiome : ABiome
{
    public int damageReduction;
    // Start is called before the first frame update
   

    // Update is called once per frame
    void Update()
    {
       
    }

    public override void ActionBioma(GameObject o)
    {
        var entity = o.GetComponent<Entity>();
        if(entity.isFlying) return;
        entity.DamageModifier -= 0.3f;
        // entity.SetCurrentDamage( entity.GetCurrentDamageModifier() - entity.damage * damageReduction/100f);
    }

    public override void LeaveBiome(GameObject o)
    {
        var entity = o.GetComponent<Entity>();
        if(entity.isFlying) return;
        entity.DamageModifier += 0.3f;
    }
 
}
