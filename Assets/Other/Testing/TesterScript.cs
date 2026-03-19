using System.Collections;
using System.Collections.Generic;
using AYellowpaper;
using EditorAttributes;
using UnityEngine;

public class TesterScript : MonoBehaviour
{
    [Button, ExecuteInEditMode]
    public void Test()
    {
        DisplaySituation("Editor Attributes Button");
    }

    public void DisplaySituation(string headerText)
    {
#if UNITY_EDITOR
        bool EDITOR = true;
#else
        bool EDITOR = false;
#endif

        Debug.Log($"{headerText}, Application.isEditor = {Application.isEditor}, Application.isPlaying = {Application.isPlaying}, UNITY_EDITOR = {EDITOR}");
    }

    private void Reset() => DisplaySituation("Reset");
    [ExecuteInEditMode]
    private void Awake() => DisplaySituation("Awake");
    private void OnEnable() => DisplaySituation("OnEnable");
    private void OnDisable() => DisplaySituation("OnDisable");
    [ExecuteInEditMode]
    private void OnDestroy() => DisplaySituation("OnDestroy");

    public InterfaceList<IRoomActor, Component> LIST;
    public InterfaceReference<IRoomActor, Component> i;

}
