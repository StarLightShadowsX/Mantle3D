using System;
using UnityEngine;
using UnityEngine.InputSystem;
using SLS.ListUtilities;
using SLS.Singletons;

[CreateAssetMenu(fileName = "Input", menuName = "Core/Input/Input")]
public class Input : GlobalAsset<Input>
{

    public InputActionReference in_move;
    public static InputActionReference MoveRef;
    public static Vector2 Move => MoveRef.action.ReadValue<Vector2>();

    public InputActionReference in_look;
    public static class Look
    {
        public static InputActionReference Ref;
        public static Vector2 Value => Ref.action.ReadValue<Vector2>();
    }

    public InputActionReference in_pan;
    public static class Pan
    {
        public static InputActionReference Ref;
        public static Vector2 Value => Ref.action.ReadValue<Vector2>();
    }

    public InputActionReference in_interact;
    public static InputActionReference InteractRef;
    public static InputAction Interact => InteractRef.action;

    public InputActionReference in_run;
    public static InputActionReference RunRef;
    public static InputAction Run => RunRef.action;

    public InputActionReference in_abilityA;
    public InputActionReference in_abilityB;
    public InputActionReference in_abilityC;
    public static class Ability
    {
        public static InputActionReference ARef;
        public static InputActionReference BRef;
        public static InputActionReference CRef;
        public static InputAction A => ARef.action;
        public static InputAction B => BRef.action;
        public static InputAction C => CRef.action;
    }

    public UI in_UI; [Serializable]
    public class UI
    {
        public InputActionReference in_ui_navigate;
        public static InputActionReference NavigateRef;
        public static Vector2 Navigate => NavigateRef.action.ReadValue<Vector2>();

        public InputActionReference in_ui_submit;
        public static InputActionReference SubmitRef;
        public static InputAction Submit => SubmitRef.action;

        public InputActionReference in_ui_cancel;
        public static InputActionReference CancelRef;
        public static InputAction Cancel => CancelRef.action;

    }

    public Debug in_Debug; [Serializable]
    public class Debug
    {
        public InputActionReference in_debug_console;
        public static InputActionReference ConsoleRef;
        public static InputAction Console => ConsoleRef.action;
    }

    public DictionaryS<string, Sprite> buttonIcons = new();
    public static DictionaryS<string, Sprite> ButtonIcons;

    public InputActionAsset in_Asset;
    public static InputActionAsset Asset;


    public override void OnInit()
    {
        Asset = in_Asset; 
        Asset.Enable();
        MoveRef = in_move;
        Look.Ref = in_look;
        Pan.Ref = in_pan;
        InteractRef = in_interact;
        RunRef = in_run;
        Ability.ARef = in_abilityA;
        Ability.BRef = in_abilityB;
        Ability.CRef = in_abilityC;
        UI.NavigateRef = in_UI.in_ui_navigate;
        UI.SubmitRef = in_UI.in_ui_submit;
        UI.CancelRef = in_UI.in_ui_cancel;
        Debug.ConsoleRef = in_Debug.in_debug_console;
        ButtonIcons = buttonIcons;
    }
}
