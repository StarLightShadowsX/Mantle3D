using SLS.StateMachineH;
using UnityEngine;

public class BatSwingerBasic : MonoBehaviour
{
    public static bool batActivated;

    public State allowedState;
    public State batSwingState;

    private void OnEnable() => Input.Ability.A.performed += Enact;
    private void OnDisable() => Input.Ability.A.performed -= Enact;

    void Enact(UnityEngine.InputSystem.InputAction.CallbackContext c)
    {
        if (!batActivated) return;
        if (!allowedState) return;
        batSwingState.Enter();
    }
}
