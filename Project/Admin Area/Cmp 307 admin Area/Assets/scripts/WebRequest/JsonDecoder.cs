using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JsonDecoder
{

    public static T[] Parse<T>(string input)
    {
        input = trim(input);
        Debug.Log(input);

        //this solution does not work for objects inside objects
        if (input[0]=='[' && input[input.Length-1]==']') {
            Debug.Log(input);
            List<T> ret = new List<T>();
            input = input.Substring(1, input.Length - 2);
            var next = input.IndexOf("},{");
            //all but last item
            while (next != -1) {
                string obj = input.Substring(0, next + 1);
                ret.Add(JsonUtility.FromJson<T>(obj));
                input = input.Substring(next + 2);
                next = input.IndexOf("},{");
            }
            Debug.Log(input);
            ret.Add(JsonUtility.FromJson<T>(input));
            return ret.ToArray();
        }
        return new T[1] { JsonUtility.FromJson<T>(input) };
    }

    private static string trim(string input) {
        input = input.Trim();
        input = input.Replace("\n", "");
        input = input.Replace("\r", "");
        input = input.Replace(" ,", ",");
        input = input.Replace(", ", ",");
        input = input.Replace("] ", "]");
        input = input.Replace(" ]", "]");
        input = input.Replace("[ ", "[");
        input = input.Replace(" [", "[");
        input = input.Replace("{ ", "{");
        input = input.Replace(" {", "{");
        input = input.Replace("} ", "}");
        input = input.Replace(" }", "}");
        input = input.Replace(":", ":");
        input = input.Replace(" :", ":");
        return input;
    }

}
