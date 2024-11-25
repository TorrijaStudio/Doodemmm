using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class glacierBiome : ABiome
{

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void ActionBioma(GameObject o)
    {
        var entity = o.GetComponent<Entity>();
        if(entity.isFlying) return;
        entity.SpeedModifier -= 3;
    }

    public override void LeaveBiome(GameObject o)
    {
        var entity = o.GetComponent<Entity>();
        if(entity.isFlying) return;
        entity.SpeedModifier += 3;
    }
    
}
