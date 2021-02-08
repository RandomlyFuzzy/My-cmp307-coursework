using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class MakeRequest
{

    private static UnityWebRequest GetData(string url, string Method, WWWForm form = null)
    {
        Debug.Log(url);
        UnityWebRequest webRequest = null;
        switch (Method)
        {
            case "AUTHGET":
                webRequest = UnityWebRequest.Get(url);


                byte[] hash;
                using (HashAlgorithm algorithm = SHA512.Create()) {
                    hash  = algorithm.ComputeHash(Encoding.UTF8.GetBytes(new System.Net.NetworkCredential(string.Empty, Autherization.authHead).Password));
                }
                webRequest.SetRequestHeader("authorization","Basic "+ System.Convert.ToBase64String(hash));
                Debug.Log(Autherization.authHead.ToString());
                break;
            case "GET":
                webRequest = UnityWebRequest.Get(url);
                break;
            case "POST":
                webRequest = UnityWebRequest.Post(url, form);
                break;
            case "PUT":
                webRequest = UnityWebRequest.Put(url+"?"+ Encoding.ASCII.GetString(form.data), form.data);
                break;
            case "DELETE":
                webRequest = UnityWebRequest.Delete(url);
                break;
            default:
                throw new Exception("invalid method passed");
        }
        if (webRequest == null){
            throw new Exception("invalid method passed");
        }
        webRequest.SendWebRequest();

        while (!webRequest.isDone) { Thread.Sleep(1); }

        return webRequest;
    }



    public void CheckForErrors(UnityWebRequest req) {

        if (req.responseCode >= 200 && req.responseCode < 300) {
            Autherization.authHead = new System.Security.SecureString();
            return;
        }
        switch (req.responseCode)
        {
            case 401:
                throw new AuthException();
            case 404:
                throw new NotFoundException();
            case 500:
            case 501:
            case 502:
            case 503:
            case 504:
            case 505:
                throw new InternalServerException();
            default:
                if (req.error != "")
                {
                    throw new Exception(req.responseCode+":"+req.error);
                }
                throw new Exception( "return with code "+req.responseCode);
        }
    }

    public string GetWithAuth(string URL, ParamArguments args)
    {
        UnityWebRequest req = GetData(URL + args.GetAsParams(), "AUTHGET");
        CheckForErrors(req);
        return req.downloadHandler.text;
    }
    public string Get(string URL, ParamArguments args)
    {
        UnityWebRequest req = GetData(URL + args.GetAsParams(), "GET");
        CheckForErrors(req);
        return req.downloadHandler.text;
    }
    public string Post(string URL, ParamArguments args)
    {
        UnityWebRequest req = GetData(URL, "POST",args.GetAsBody());
        CheckForErrors(req);
        return ""+req.responseCode;
    }
    public string Put(string URL, ParamArguments args)
    {
        UnityWebRequest req = GetData(URL, "PUT", args.GetAsBody());
        CheckForErrors(req);
        return ""+req.responseCode;
    }
    public string Delete(string URL, ParamArguments args)
    {
        UnityWebRequest req = GetData(URL, "DELETE");
        CheckForErrors(req);
        return ""+req.responseCode;
    }

}

public class AuthException : Exception { public AuthException() : base("unable to autherise user excetion") { } }
public class NotFoundException : Exception { public NotFoundException() : base("unable receieve a response excetion") { } }
public class InternalServerException : Exception { public InternalServerException() : base("Server error") { } }