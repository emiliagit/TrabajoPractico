using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Factory : MonoBehaviour
{
    [SerializeField] protected float durationTime;
    public abstract string propName { get; }
    public abstract void Activate();
}
