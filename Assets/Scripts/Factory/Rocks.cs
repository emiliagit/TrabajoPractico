using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocks : Factory
{
    public override string propName => "Rock";
    public override void Activate()
    {
        Destroy(gameObject, durationTime);
    }
}
