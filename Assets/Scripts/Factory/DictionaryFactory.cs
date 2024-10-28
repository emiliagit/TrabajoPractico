using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class DictionaryFactory : MonoBehaviour
{
    [SerializeField] private Factory[] props;
    [SerializeField] private Transform propsParent;

    private Dictionary<string, Factory> propsByName;

    private void Awake()
    {
        propsByName = new Dictionary<string, Factory>();

        foreach (var prop in props)
        {
            propsByName.Add(prop.propName, prop);
        }
    }

    public Factory CreateProp(string propName, Transform position)
    {
        if(propsByName.TryGetValue(propName, out Factory propPrefab))
        {
            Factory propsInstance = Instantiate(propPrefab, position.position, Quaternion.identity, propsParent);
            return propsInstance;
        }
        else
        {
            Debug.LogWarning($"El objeto {propName} no existe en la base de datos de habilidades.");
            return null;
        }
    }
}
