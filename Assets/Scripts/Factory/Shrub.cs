using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shrub : Factory
{
    public override string propName => "Shrub";
    public override void Activate()
    {
        Destroy(gameObject, durationTime);
    }
}

