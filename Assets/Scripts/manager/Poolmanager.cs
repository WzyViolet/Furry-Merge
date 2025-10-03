using System.Collections.Generic;
using UnityEngine;
public enum Game_type
{
    fruit_gam,font_txt
}
public class Poolmanager 
{
    private static Poolmanager instance;
    public static Poolmanager Instance
    {
        get { if (instance == null)
            {
                instance = new Poolmanager();
            }
            return instance;
        }
    }
    private Dictionary<Game_type, Queue<GameObject>> dic_gam;
    public GameObject Get_gam(Game_type type,GameObject gam)
    {
        if (dic_gam == null)
        {
            dic_gam = new Dictionary<Game_type, Queue<GameObject>>();
        }
        Queue<GameObject> temp;
        if (dic_gam.ContainsKey(type))
        {
            temp = dic_gam[type];
        }
        else
        {
            temp = new Queue<GameObject>();
            dic_gam[type] = temp;
        }
        if (temp.Count <= 0)
        {
            GameObject op = Object.Instantiate(gam);
            temp.Enqueue(op);
        }
        GameObject kl = temp.Dequeue();kl.SetActive(true);
        return kl;
    }
    public void Push_gam(Game_type type,GameObject gam)
    {
        if (dic_gam.ContainsKey(type))
        {
            dic_gam[type].Enqueue(gam);
            gam.SetActive(false);
        }
    }
}
