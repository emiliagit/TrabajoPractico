using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PropsManager : MonoBehaviour
{
    public DictionaryFactory porpsFactory;
    [SerializeField] private Transform Ground;

    private void Awake()
    {
        FindAnyObjectByType(typeof(DictionaryFactory));
    }

    public void ActivateProp(string propName)
    {
        Factory propToActivate = porpsFactory.CreateProp(propName, Ground);
        propToActivate.Activate();
    }
}
