using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Booking
{
    public int id;
    public int rid;
    public string title;
    public string starttime;
    public int duration;
    public string bookingTime;
    public int oid;


    public string GetHeader() { 
        return "id,room,title,starttime,duration,organiser,Day,Amount Booked";
    }

    public string GetRow()
    {
        Room r = RequestFactory.Get<Room>("DATA/[token]/Twang.Room/" + rid)[0];
        User org = RequestFactory.Get<User>("DATA/[token]/Twang.Users/" + oid)[0];
        int present = RequestFactory.Get<User>("DATA/[token]/Twang.UserBooking/","bid",""+id).Length;
        return "" +id+","+r.rname+","+title+","+ DateTime.Parse(starttime).ToShortTimeString() + ","+duration+","+ org.uname+","+DateTime.Parse(starttime).ToShortDateString()+","+present;
    }


}
