using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParamArguments 
{
    Dictionary<string, string> arguments = new Dictionary<string, string>();
    
    
    
    public ParamArguments(params string[] arr)
    {
        if (arr.Length % 2 == 1) {
            throw new Exception("invalid amount of params passed to paramArguments");
        }
        for (int i = 0; i < arr.Length; i+=2)
        {
            AddKetValue(arr[i], arr[i + 1]);
        }
    }

    public void AddKetValue(string key, string value)
    {
        arguments[key] = value;
    }


    public string GetAsParams()
    {
        string ret = "?";
        foreach (var item in arguments.Keys)
        {
            ret += item + "=" + arguments[item] + "&";
        }
        return ret.Substring(0, ret.Length - 1);//trim final &
    }


    public WWWForm GetAsBody()
    {
        WWWForm form = new WWWForm();
        foreach (var item in arguments.Keys)
        {
            form.AddField(item, arguments[item]);
        }
        return form;
    }
}
