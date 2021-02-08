using System;
using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Autherization :MonoBehaviour
{
    static Autherization auth;
    static string LastScene = "";
    public static string token = "";
    public static SecureString authHead;
    public string _token = "";
    private static string uname = "";
    private static string id = "";

    // Start is called before the first frame update
    void Awake()
    {
        Start();
    }
    private void Start()
    {
        //set to most recent scene
        auth = this;
        if (token == "" && SceneManager.GetSceneAt(0).name != "Login")
        {
            RequestLoginScene();
        }
        _token = token;
    }

    public static void RequestLoginScene()
    {
        Debug.Log("hello loginscene");
        LastScene = SceneManager.GetSceneAt(0).name;
        SceneManager.LoadScene("Login");
    }

    public void OnSubmit()
    {
        List<InputField> inputs = new List<InputField>();
        Dictionary<string, string> values = new Dictionary<string, string>();
        inputs.AddRange(auth.GetComponentsInChildren<InputField>());
        inputs.ForEach((a) => { values.Add(a.gameObject.name, a.text); });

        RequestFactory.ServerPrefix = "http://"+values["ip"]+"/";
        uname = values["uname"];
        id = values["id"];
        authHead = new SecureString();
        foreach (var c in values["pw"])
        {
            authHead.AppendChar(c);
        }
        try
        {
            Token v = RequestFactory.GetWithAuth<Token>("loginadmin", "uname", uname, "id", id)[0];
            token = v.token;
            SceneManager.LoadScene(LastScene);
        }
        catch (AuthException ex)
        {
            Debug.LogError(ex.Message);
        }
    }
}
class Token
{
    public string token = "";

}