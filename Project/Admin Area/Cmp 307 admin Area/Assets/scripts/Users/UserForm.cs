using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UserForm : MonoBehaviour
{
    static UserForm uf;
    public UserEntryManager uem;
    public int startId = 0;
    public string pkey = "";

    private void Start()
    {
        uf = this;
    }

    public static bool SetUEM(UserEntryManager uemfrom)
    {
        if (uemfrom == null||uf==null) return false;
        uf.uem = uemfrom;

        var form = formController.NameValues(uf.gameObject);

        form["id"].text = ""+(uf.startId = uf.uem.thisUser.id);
        form["uname"].text = uf.uem.thisUser.uname;
        uf.pkey = uf.uem.thisUser.pkey;
        Destroy(uf.uem.gameObject);
        return true;
    }
    public void Continue() {
        bool? error = false;
        //update 
        if (startId != 0)
        {
            var form = formController.NameValues(gameObject);
            RequestFactory.Post("DATA/[token]/Twang.Users/"+startId, ref error, "id", form["id"].text, "uname", form["uname"].text, "pkey", pkey);
        }
        //add
        else {
            var form = formController.NameValues(gameObject);
            RequestFactory.Put("DATA/[token]/Twang.Users/", ref error,"id", form["id"].text, "uname", form["uname"].text, "pkey", " ");
        }
        if (error==true) { 
            //do popup

        }
        SceneManager.LoadScene("Users");
    }
}
