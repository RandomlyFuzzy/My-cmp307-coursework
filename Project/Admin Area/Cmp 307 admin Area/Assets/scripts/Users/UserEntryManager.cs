using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using System.Diagnostics;
using System.Threading;

public class UserEntryManager : MonoBehaviour
{
    public static UserEntryManager uem;
    public User _thisUser;
    public Text id;
    public Text uname;
    public User thisUser { get { return _thisUser; }set {
            id.text = ""+value.id;
            uname.text = value.uname;
            _thisUser = value;
        } 
    }

    private void Start()
    {
        UnityEngine.Debug.Log("hello");

        if (SceneManager.sceneCount > 1 && SceneManager.GetSceneAt(1).name == "UpdateUser")
        {
            StartCoroutine(UpdateUserPage());
        }

    }

    private IEnumerator UpdateUserPage() {

        while (!UserForm.SetUEM(this)) {
            yield return new WaitForSeconds(0.1f);
        }

            Destroy(gameObject);


    }


    /// <summary>
    /// Download all booking the users was involved in and saves it to a CSV in the users Download directory
    /// </summary>
    public void DownloadBookings()
    {
        var bookings = RequestFactory.Get<UserBooking>("DATA/[token]/Twang.UserBooking/",true,"uid",""+thisUser.id);

        if (bookings == null || bookings.Length == 0) return;


        string URL = System.Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%") + "/downloads/Users" + thisUser.uname + ".csv";
        StreamWriter sw = new StreamWriter(URL, false);
        sw.WriteLine(new Booking().GetHeader());
        foreach (var item in bookings)
        {
            Booking cur = RequestFactory.Get<Booking>("DATA/[token]/Twang.Booking/" + item.bid)[0];
            sw.WriteLine(cur.GetRow());
        }
        sw.Flush();
        sw.Close();
        Process.Start(URL);

    }


    public void UpdateUser()
    {
        //chained with a Dont destroy on load object
        SceneManager.LoadScene("UpdateUser");
    }

    //should probably make a prompt
    public void Delete()
    {
        var UB = RequestFactory.Get<UserBooking>("DATA/[token]/Twang.UserBooking/",true,"uid",""+thisUser.id);
        if (UB!=null &&UB.Length != 0) {
            UnityEngine.Debug.Log(UB.Length);

            foreach (var item in UB)
            {
               
                try
                {
                    UnityEngine.Debug.Log(item.id);
                    RequestFactory.Delete("DATA/[token]/Twang.UserBooking/" + item.id);
                }
                catch { }
            }
        }
        RequestFactory.Delete("DATA/[token]/Twang.Users/" + thisUser.id);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
