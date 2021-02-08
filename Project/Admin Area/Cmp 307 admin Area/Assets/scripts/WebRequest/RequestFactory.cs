using System;
using System.Threading;
using System.Web;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;
using UnityEngine.Networking;

public class RequestFactory : MonoBehaviour
{
    static RequestFactory rf;

    public static string ServerPrefix = "https://127.0.0.1/";


    private void Start()
    {
        rf = this;

    }

    private static bool ValidateInputs(string url, string[] inputs)
    {
        Uri trash = new Uri("https://www.google.com/");
        return !Uri.TryCreate(url, UriKind.Absolute, out trash) || inputs.Length % 2 == 1;
    }


    public static T[] Get<T>(string url, bool ErrorCheck, params string[] inputs)
    {
        T[] ret = null;
        try {
            ret = Get<T>(url, inputs);
        }
        catch (AuthException ex)
        {
            Debug.Log(ex.Message);
            Autherization.RequestLoginScene();
        }
        catch (Exception ex) {
            Debug.LogError(ex.Message);
        }
        return ret;
    }

    public static int Post(string url, ref bool? hadError, params string[] inputs)
    {
        int ret = 0;
        try
        {
            ret = Post(url, inputs);
        }
        catch (AuthException ex)
        {
            Autherization.RequestLoginScene();
            if (hadError != null)
            {
                hadError = true;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            if (hadError != null)
            {
                hadError = true;
            }
        }
        return ret;
    }


    public static int Put(string url,ref bool? hadError, params string[] inputs)
    {
        int ret = 0;
        try
        {
            ret = Put(url, inputs);
        }
        catch (AuthException ex)
        {
            Autherization.RequestLoginScene();
            if (hadError != null)
            {
                hadError = true;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            if (hadError != null)
            {
                hadError = true;
            }
        }
        return ret;
    }


    public static int Delete(string url,ref bool? hadError, params string[] inputs)
    {
        int ret = 0;
        try
        {
            ret = Delete(url, inputs);
        }
        catch (AuthException ex)
        {
            Autherization.RequestLoginScene();
            if (hadError != null) { 
                hadError = true;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            if (hadError != null)
            {
                hadError = true;
            }
        }
        return ret;
    }



    public static T[] GetWithAuth<T>(string url, params string[] inputs)
    {
        url = url.Replace("[token]", Autherization.token);
        if (ValidateInputs(ServerPrefix + url, inputs))
        {
            throw new System.Exception("invalid amount of params passed to request for url \"" + url + "\"");
        }
        var mr = new MakeRequest();
        var parms = new ParamArguments(inputs);
        var ret = mr.GetWithAuth(ServerPrefix + url, parms);
        return JsonDecoder.Parse<T>(ret);
    }
    public static T[] Get<T>(string url, params string[] inputs)
    {
        url = url.Replace("[token]", Autherization.token);
        if (ValidateInputs(ServerPrefix + url, inputs))
        {
            throw new System.Exception("invalid amount of params passed to request for url \"" + url + "\"");
        }
        var mr = new MakeRequest();
        var ret = mr.Get(ServerPrefix + url, new ParamArguments(inputs));
        return JsonDecoder.Parse<T>(ret);
    }

    public static int Post(string url, params string[] inputs)
    {
        url = url.Replace("[token]", Autherization.token);
        if (ValidateInputs(ServerPrefix + url, inputs))
        {
            throw new System.Exception("invalid amount of params passed to request for url \"" + url + "\"");
        }
        var mr = new MakeRequest();
        var ret = mr.Post(ServerPrefix + url, new ParamArguments(inputs));
        return int.Parse(ret);
    }
    public static int Put(string url, params string[] inputs)
    {
        url = url.Replace("[token]", Autherization.token);
        if (ValidateInputs(ServerPrefix + url, inputs))
        {
            throw new System.Exception("invalid amount of params passed to request for url \"" + url + "\"");
        }
        var mr = new MakeRequest();
        var ret = mr.Put(url, new ParamArguments(inputs));
        return int.Parse(ret);
    }

    public static int Delete(string url, params string[] inputs)
    {
        url = url.Replace("[token]", Autherization.token);
        if (ValidateInputs(ServerPrefix + url, inputs))
        {
            throw new System.Exception("invalid amount of params passed to request for url \"" + url + "\"");
        }
        var mr = new MakeRequest();
        var ret = mr.Delete(ServerPrefix + url, new ParamArguments(inputs));
        return int.Parse(ret) ;
    }

}
