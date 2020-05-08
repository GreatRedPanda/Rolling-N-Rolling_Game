using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LevelButton : Button, IPointerDownHandler
{
    public  static int ClickCount;
    public static LevelButton LastBtnClicked;
    public ButtonClickedEvent onPressing;
    float timer = 0;

    public float PressTime = 1;

    bool startTimer = false;
    public override void OnPointerDown(PointerEventData eventData)
    {
        // base.OnPointerDown(eventData);

        startTimer = true;
    }
    private void Update()
    {
        if (!startTimer)
            return;
        //Debug.Log("Pressing");
        timer += Time.deltaTime;

        if (timer >= PressTime)
        {

            onPressing?.Invoke();
            timer = 0;
            startTimer = false;
        }
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        //base.OnPointerUp(eventData);
        timer = 0;
        startTimer = false;
    }
}
