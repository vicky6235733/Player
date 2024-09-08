using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ColorPapaerStruct
{
    public string PatternType; //形狀
    public string Color; //顏色
    public string GroupName; //一起撕的貼紙
    public GameObject ColorPaper; //紙
    public bool Activity; //狀態
    public int ID; //編號

    public ColorPapaerStruct(
        string patternType,
        string color,
        string groupName,
        GameObject colorPaper,
        bool activity,
        int id
    )
    {
        this.PatternType = patternType;
        this.Color = color;
        this.GroupName = groupName;
        this.ColorPaper = colorPaper;
        this.Activity = activity;
        this.ID = id;
    }
}

public class ColorData : MonoBehaviour
{
    public string _patten; //形狀
    public string color; //顏色
    public string _group; //一起撕的貼紙
    public bool _act; //狀態
    public int id; //編號
    public ColorPapaerStruct colorPaper;

    void Awake()
    {
        _act = false;

        if (_patten != null && color != null && id != 0)
        {
            _group = _group != null ? _group : "null";
            colorPaper = new ColorPapaerStruct(_patten, color, _group, this.gameObject, _act, id);
        }
        this.gameObject.SetActive(_act);
    }
}
