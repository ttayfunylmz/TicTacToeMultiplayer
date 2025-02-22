﻿using UnityEngine;

public class UI_Blocker : MonoBehaviour
{
    private static UI_Blocker instance;

    private void Awake()
    {
        instance = this;

        GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        GetComponent<RectTransform>().sizeDelta = Vector2.zero;

        Hide_Static();
    }

    public static void Show_Static()
    {
        instance.gameObject.SetActive(true);
        instance.transform.SetAsLastSibling();
    }

    public static void Hide_Static()
    {
        instance.gameObject.SetActive(false);
    }
}
