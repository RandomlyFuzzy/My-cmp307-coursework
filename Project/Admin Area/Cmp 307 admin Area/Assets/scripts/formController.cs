using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class formController : MonoBehaviour
{
    public static Dictionary<string, InputField> NameValues(GameObject root) {
        List<InputField> inptus = new List<InputField>();
        Dictionary<string, InputField> valus = new Dictionary<string, InputField>();
        inptus.AddRange(root.GetComponentsInChildren<InputField>());
        inptus.ForEach((a) => { valus.Add(a.gameObject.name, a); });
        return valus;
    }
}
