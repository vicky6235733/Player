using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PaperItemStruct
{
    public string PatternType; //形狀
    public string Color; //顏色
    public GameObject Paper; //紙
    public bool State; //0=未飄起
    public int ID; //編號
    public string GroupName; //群組名稱
    public Renderer _ren;

    public PaperItemStruct(string pt, string color, int id, string gn, GameObject gm, Renderer re)
    {
        this.PatternType = pt;
        this.Color = color;
        this.Paper = gm;
        this.State = false;
        this.ID = id;
        this.GroupName = gn;
        this._ren = re;
    }
}

public class PaperData : MonoBehaviour
{   public PaperItemStruct paper;
    public string pt; //形狀
    public string color; //顏色
    public int _id; //編號
    public string _group; //群組名稱

    void Awake()
    {
        

        if (pt != null && color != null && _id != 0)
        {
            _group = _group != null ? _group : "null";
            paper = new PaperItemStruct(pt,color,_id,_group,this.gameObject, this.gameObject.GetComponent<Renderer>());
        }
    }
}
