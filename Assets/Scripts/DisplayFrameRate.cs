using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayFrameRate : MonoBehaviour
{
    [Header("DISPLAY POSITION")]
    [SerializeField]
    public alignmentType selectAlignment;
    public enum alignmentType
    {
        Nothing,
        UpperLeft,
        UpperCenter,
        UpperRight,
        MiddleLeft,
        MiddleCenter,
        MiddleRight,
        LowerLeft,
        LowerCenter,
        LowerRight
    }

    [Header("TEXT COLOR")]
    [SerializeField]
    public Color textColor = Color.cyan;

    [Header("TEXT SIZE")]
    [Range(10, 200)]
    [SerializeField]
    public int fontSize = 10;

    private TextAnchor textAnchor;

    private void Awake()
    {
        switch (selectAlignment)
        {
            case alignmentType.LowerLeft:
                textAnchor = TextAnchor.LowerLeft;
                break;
            case alignmentType.LowerCenter:
                textAnchor = TextAnchor.LowerCenter;
                break;
            case alignmentType.LowerRight:
                textAnchor = TextAnchor.LowerRight;
                break;

            case alignmentType.UpperLeft:
                textAnchor = TextAnchor.UpperLeft;
                break;
            case alignmentType.UpperCenter:
                textAnchor = TextAnchor.UpperCenter;
                break;
            case alignmentType.UpperRight:
                textAnchor = TextAnchor.UpperRight;
                break;

            case alignmentType.MiddleLeft:
                textAnchor = TextAnchor.MiddleLeft;
                break;
            case alignmentType.MiddleCenter:
                textAnchor = TextAnchor.MiddleCenter;
                break;
            case alignmentType.MiddleRight:
                textAnchor = TextAnchor.MiddleRight;
                break;

        }
    }

    private float deltaTime = 0.0f;

    void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
    }

    void OnGUI()
    {
        if (selectAlignment == alignmentType.Nothing)
            return;

        int width = Screen.width, height = Screen.height;

        GUIStyle style = new GUIStyle();

        Rect rect = new Rect(0, 0, width, height);
        style.alignment = textAnchor;
        style.fontSize = fontSize;
        style.normal.textColor = textColor;
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        string text = string.Format("{0:0.0} ms ({1:0.} FPS)", msec, fps);
        GUI.Label(rect, text, style);
    }
}

