using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Image FootUI;
    public List<Sprite> ColorFoot;
    Dictionary<string, Sprite> colorDict;

    string PreviousColor;
    string[] AllColor = { "null", "red", "yellow", "blue", "green", "orange", "purple" };

    // Start is called before the first frame update
    void Start()
    {
        colorDict = new Dictionary<string, Sprite>();
        PreviousColor = Player.ColorName;
        int i = 0;
        foreach (var color in ColorFoot)
        {
            colorDict.Add(AllColor[i++], color);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Player.ColorName != PreviousColor)
        {
            ChangeFootUI();
        }
    }

    void ChangeFootUI()
    {
        PreviousColor = Player.ColorName;
        if (FootUI != null && colorDict.ContainsKey(Player.ColorName)) // 確認UI和字典中是否有這個顏色
        {
            FootUI.sprite = colorDict[Player.ColorName]; // 更新圖片
        }
    }
}
