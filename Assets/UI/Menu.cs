using EditorAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
//using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// A Base Menu class intended to form an easy-to-use extensible Menu system. (Admittedly not up to snuff.)
/// 
/// PSEUDOCODE / PLAN:
/// - Introduce an internal "_frozen" flag and ensure Freeze()/UnFreeze() set that flag and toggle Selectable.interactable
///   (Frozen = uninteractable). Actual "allow focus while frozen" behavior will be implemented later; for now frozen
///   menus are marked and made uninteractable.
/// - Use the enum States: Focus, Background, BackgroundLocked, Available. The State setter will apply Freeze/UnFreeze
///   side-effects:
///     - Focus   => UnFreeze()
///     - Background / BackgroundLocked => Freeze()
///     - Available => UnFreeze()
///   Also keep the state stored in _state.
/// - When opening a menu:
///     - If menu is already active and not overriding, return.
///     - Remove duplicates in ActiveMenus (move to top if present).
///     - Before adding the opening menu, recompute states for all existing active menus based on rules:
///         - If the opening menu has lockAllPreviousMenus == true, all previous menus become BackgroundLocked.
///         - Otherwise, for each previous menu:
///             - If that menu.lockWhenBackground == true => BackgroundLocked
///             - Else => Background
///     - Add the new menu to ActiveMenus, set its State = Focus, set isActive, set GameObject active, and select defaultSelection.
///     - Update CurrentMenu pointer.
/// - When closing a menu:
///     - Remove it from ActiveMenus and clear its isActive/GameObject active, call OnClose.
///     - Recompute states for remaining ActiveMenus:
///         - The last menu (if any) becomes Focus
///         - For each other menu at index i:
///             - If any menu above it (index > i) has lockAllPreviousMenus == true => BackgroundLocked
///             - Else if that menu.lockWhenBackground == true => BackgroundLocked
///             - Else => Background
///     - Update CurrentMenu and ensure top menu (if any) has its defaultSelection selected.
/// - Provide helper to recompute states for all active menus to avoid code duplication.
/// 
/// Note: "Frozen" here means selectable/interactable toggled. Locking is represented by the BackgroundLocked state.
/// Focus swapping constraints (preventing selection of locked menus) are not fully enforced here; the state flags are set
/// and the Selectable.interactable values are toggled as a first step. Further behavior will be implemented later.
/// </summary>
public class Menu : MonoBehaviour
{
    #region Config

    [SerializeField] string ID;
    [DisableInPlayMode] public bool isActive;
    [DisableInPlayMode] public Menu parent;
    [SerializeField] Button defaultSelection;
    [SerializeField] Selectable[] allButtons;
    [SerializeField] private UnityEvent closeOverride;

    [SerializeField] bool lockWhenBackground = true;
    [SerializeField] bool lockAllPreviousMenus = false;

    #endregion

    #region Instance Data

    public enum States
    {
        Null = -1,
        Available = 0,
        BackgroundLocked = 1,
        Background = 2,
        Focus = 3
    }

    public States State
    {
        get => _state;
        set
        {
            _state = value;
            switch (_state)
            {
                case States.Available:
                    UnFreeze();
                    break;
                case States.BackgroundLocked:
                    Freeze();
                    break;
                case States.Background:
                    Freeze();
                    break;
                case States.Focus:
                    UnFreeze();
                    break;
            }
        }
    }
    protected States _state = States.Available;
    protected bool _frozen = false;

    public bool isFocus => State is States.Focus;
    public bool isSubMenu => parent != null;
    public bool isLabeled => !string.IsNullOrWhiteSpace(ID);

    #endregion

    #region Instance Behavior

    protected virtual void Awake()
    {
        if (isLabeled)
        {
            if (AvailableMenus.ContainsKey(ID))
                Debug.LogWarning($"Menu with ID {ID} already exists in AvailableMenus. Overwriting with new instance.");
            AvailableMenus[ID] = this;
        }


        if (isActive) Menu.Open(this, true);
        else gameObject.SetActive(false);
    }

    protected virtual void OnDestroy()
    {
        Menu.Close(this);

        if (isLabeled && AvailableMenus.ContainsKey(ID) && AvailableMenus[ID] == this)
            AvailableMenus.Remove(ID);

        isActive = false;
    }

    public void Open() => Menu.Open(this);

    public void Close(bool allowOverride = true)
    {
        if (allowOverride || closeOverride == null) Menu.Close(this);
        else closeOverride.Invoke();
    }

    // Freeze: make menu "uninteractable" (representing frozen). _frozen toggled and Selectable.interactable set accordingly.
    public void Freeze()
    {
        _frozen = true;
        if (allButtons == null) return;
        for (int i = 0; i < allButtons.Length; i++)
            if (allButtons[i] != null) allButtons[i].interactable = false;

        // If a CanvasGroup exists, block raycasts to further represent non-interactable UI
        var cg = GetComponent<CanvasGroup>();
        if (cg != null) cg.blocksRaycasts = false;
    }

    // UnFreeze: make menu interactable again.
    public void UnFreeze()
    {
        _frozen = false;
        if (allButtons == null) return;
        for (int i = 0; i < allButtons.Length; i++)
            if (allButtons[i] != null) allButtons[i].interactable = true;

        var cg = GetComponent<CanvasGroup>();
        if (cg != null) cg.blocksRaycasts = true;
    }

    protected virtual void OnOpen()
    {
        //if (!openSound.IsNull)
        //    AudioManager.Get().PlayOneShot(openSound, transform.position);
    }

    protected virtual void OnClose()
    {
        //if (!closeSound.IsNull)
        //    AudioManager.Get().PlayOneShot(closeSound, transform.position);
    }

    #endregion

    #region Static Data

    public static Dictionary<string, Menu> AvailableMenus = new();
    public static List<Menu> ActiveMenus = new();
    public static Menu CurrentMenu;

    #endregion

    #region Static Behavior


    // safer currentMenu accessor

    public static void Open(Menu menu, bool overrideRedundancyCheck = false)
    {
        if (menu == null) return;
        if (menu.isActive && !overrideRedundancyCheck) return;

        // avoid duplicates; if already present move to top
        if (ActiveMenus.Contains(menu))
        {
            ActiveMenus.Remove(menu);
        }

        // Before adding the opening menu, compute states for existing menus based on the opening menu's lockAllPreviousMenus
        for (int i = 0; i < ActiveMenus.Count; i++)
        {
            var m = ActiveMenus[i];
            bool shouldLock = false;
            if (menu.lockAllPreviousMenus) shouldLock = true;
            else if (m.lockWhenBackground) shouldLock = true;

            m.State = shouldLock ? States.BackgroundLocked : States.Background;
        }

        ActiveMenus.Add(menu);

        menu.isActive = true;
        menu.gameObject.SetActive(true);

        // The newly opened menu becomes focus
        menu.State = States.Focus;

        // If there was a previous menu now second-to-last, ensure it's frozen/updated
        if (ActiveMenus.Count > 1 && ActiveMenus[^2] != null)
        {
            // If the previous one wasn't set already by the loop above (e.g., it was added earlier),
            // ensure it's in the correct background state. (Loop above already handled it.)
        }

        if (menu.defaultSelection != null)
            menu.defaultSelection.Select();

        menu.OnOpen();

        // Update CurrentMenu pointer
        CurrentMenu = ActiveMenus.Count > 0 ? ActiveMenus[^1] : null;
    }

    public static void Close(Menu menu)
    {
        if (menu == null) return;
        if (!menu.isActive)
        {
            // still try to remove if somehow present
            if (ActiveMenus.Contains(menu)) ActiveMenus.Remove(menu);
            return;
        }

        ActiveMenus.Remove(menu);

        menu.gameObject.SetActive(false);
        menu.isActive = false;

        menu.OnClose();

        // Recompute states for remaining active menus.
        // Rules:
        // - Last menu (top) => Focus
        // - For others: if any menu above has lockAllPreviousMenus == true => BackgroundLocked
        //               else if that menu.lockWhenBackground == true => BackgroundLocked
        //               else => Background
        for (int i = 0; i < ActiveMenus.Count; i++)
        {
            if (i == ActiveMenus.Count - 1)
            {
                ActiveMenus[i].State = States.Focus;
                continue;
            }

            bool shouldLock = false;
            // check if any menu above has lockAllPreviousMenus
            for (int j = i + 1; j < ActiveMenus.Count; j++)
            {
                if (ActiveMenus[j] != null && ActiveMenus[j].lockAllPreviousMenus)
                {
                    shouldLock = true;
                    break;
                }
            }

            if (!shouldLock && ActiveMenus[i] != null && ActiveMenus[i].lockWhenBackground) shouldLock = true;

            ActiveMenus[i].State = shouldLock ? States.BackgroundLocked : States.Background;
        }

        if (ActiveMenus.Count > 0 && ActiveMenus[^1] != null)
        {
            // Ensure the new top is interactable
            ActiveMenus[^1].UnFreeze();
            if (ActiveMenus[^1].defaultSelection != null)
                ActiveMenus[^1].defaultSelection.Select();
        }

        // Update CurrentMenu pointer
        CurrentMenu = ActiveMenus.Count > 0 ? ActiveMenus[^1] : null;
    }

    public static void CloseAllMenus()
    {
        for (int i = ActiveMenus.Count - 1; i >= 0; i--) Close(ActiveMenus[i]);
    }

    #endregion
}
