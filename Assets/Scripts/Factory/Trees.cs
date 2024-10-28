using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trees : Factory
{
    public override string propName => "Tree";
    public override void Activate()
    {
        Destroy(gameObject, durationTime);
    }
}
