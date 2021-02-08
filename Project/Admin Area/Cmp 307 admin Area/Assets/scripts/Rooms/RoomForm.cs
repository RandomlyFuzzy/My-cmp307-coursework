using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomForm :MonoBehaviour
{
    static RoomForm rf;
    public RoomEntryManager rem;
    public int startId = 0;

    private void Start()
    {
        rf = this;
    }

    public static bool SetREM(RoomEntryManager uemfrom)
    {
        if (uemfrom == null || rf == null) return false;
        rf.rem = uemfrom;

        var form = formController.NameValues(rf.gameObject);

        form["id"].text = "" + (rf.startId = rf.rem.thisRoom.id);
        form["rname"].text = rf.rem.thisRoom.rname;
        form["capacity"].text = "" + rf.rem.thisRoom.capacity;
        Destroy(rf.rem.gameObject);
        return true;
    }
    public void Continue()
    {
        bool? error = false;
        //update 
        if (startId != 0)
        {
            var form = formController.NameValues(gameObject);
            RequestFactory.Post("DATA/[token]/Twang.Room/" + startId, ref error, "id", form["id"].text, "rname", form["rname"].text, "capacity", form["capacity"].text);
        }
        //add
        else
        {
            var form = formController.NameValues(gameObject);
            RequestFactory.Put("DATA/[token]/Twang.Room/", ref error, "id", form["id"].text, "rname", form["rname"].text, "capacity", form["capacity"].text);
        }
        if (error == true)
        {
            //do popup

        }
        SceneManager.LoadScene("Rooms");
    }
}
