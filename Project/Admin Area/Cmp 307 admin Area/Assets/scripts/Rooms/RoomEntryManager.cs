using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using System.Diagnostics;
using System.Threading;

public class RoomEntryManager:MonoBehaviour
{
    public static RoomEntryManager uem;
    public Room _thisRoom;
    public Text Roomname;
    public Room thisRoom
    {
        get { return _thisRoom; }
        set
        {
            Roomname.text = value.rname;
            _thisRoom = value;
        }
    }


    private void Start()
    {
        UnityEngine.Debug.Log("hello");

        if (SceneManager.sceneCount > 1 && SceneManager.GetSceneAt(1).name == "UpdateRoom")
        {
            StartCoroutine(UpdateRoomPage());
        }

    }

    private IEnumerator UpdateRoomPage()
    {

        while (!RoomForm.SetREM(this))
        {
            yield return new WaitForSeconds(0.1f);
        }

        Destroy(gameObject);


    }





    public void UpdateRoom()
    {
        //chained with a Dont destroy on load object
        SceneManager.LoadScene("UpdateRoom");
    }

    //should probably make a prompt
    public void Delete()
    {


        //Room -> Booking -> UserBooking
        var B = RequestFactory.Get<UserBooking>("DATA/[token]/Twang.Booking/", true, "rid", "" + thisRoom.id);
        if (B != null)
        {
            B.All((a) =>
            {
                if (a == null) return true;

                var UB = RequestFactory.Get<UserBooking>("DATA/[token]/Twang.UserBooking/", true, "bid", "" + a.id);
                UB.All((b) =>
                {
                    if (b == null) return true;

                    RequestFactory.Delete("DATA/[token]/Twang.UserBooking/" + b.id);
                    return true;
                });
                RequestFactory.Delete("DATA/[token]/Twang.Booking/" + a.id);
                return true;
            });
        }
        RequestFactory.Delete("DATA/[token]/Twang.Room/" + thisRoom.id);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
