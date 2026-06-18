using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerCore
{
    public class PlayerController : MonoBehaviour
    {
        public static bool sprinting;

        private void OnEnable() => Input.RunToggle.performed += Toggle;


        private void OnDisable() => Input.RunToggle.performed -= Toggle;
        void Toggle(InputAction.CallbackContext _) => sprinting = !sprinting;
    }
}

