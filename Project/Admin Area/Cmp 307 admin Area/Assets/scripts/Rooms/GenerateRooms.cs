using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateRooms :MonoBehaviour
{
    public GameObject Prefab;




    // Start is called before the first frame update
    void Start()
    {
        var v = RequestFactory.Get<Room>("DATA/[token]/Twang.Room/", true);
        RectTransform rt = GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(0, 68 * (v.Length - 1));
        int ID = 0;
        foreach (var item in v)
        {
            GameObject g = Instantiate<GameObject>(Prefab, new Vector3(transform.position.x, -((64 * ID++)), 0), Quaternion.identity, transform);
            g.GetComponent<RoomEntryManager>().thisRoom = item;
        }


    }

}

