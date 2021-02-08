using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunctions :MonoBehaviour
{
    public void LoadScene(string name)
    {
        SceneManager.LoadScene(name);
    }

    public void GenerateObject(GameObject go)
    {
        DontDestroyOnLoad(Instantiate<GameObject>(go));
    }
    public void AddUser(GameObject go)
    {
        SceneManager.LoadScene("UpdateUser");
    }
}
