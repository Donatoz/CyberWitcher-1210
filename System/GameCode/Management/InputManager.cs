using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System;

using CallbackContext = UnityEngine.InputSystem.InputAction.CallbackContext;

namespace VovTech
{
    public class InputManager : MonoBehaviour
    {
        [Serializable]
        public struct ButtonPressedEvent
        {
            public KeyCode Button;
            public Action Action;
        }

        public static InputManager Instance => GameObject.Find("GameManagers").GetComponent<InputManager>();
        public Vector3 MouseWorldPosition;

        [Header("Actions")]
        public InputAction FireAction;
        public InputAction WalkAction;
        public InputAction SwitchWeaponAction;

        private Player localPlayer => MainManager.Instance.LocalPlayer;

        #region Input action init
        private void Awake()
        {
            WalkAction.performed += Walk;
            SwitchWeaponAction.performed += SwitchWeapon;
        }

        private void OnEnable()
        {
            FireAction.Enable();
            WalkAction.Enable();
            SwitchWeaponAction.Enable();
        }

        private void OnDisable()
        {
            FireAction.Disable();
            WalkAction.Disable();
            SwitchWeaponAction.Disable();
        }
        #endregion

        private void Update()
        {
            Weapon localCharWeapon = localPlayer.ControlledCharacter.EquipedWeapon;
            RaycastHit hit;
            int mask =~ LayerMask.GetMask("Zones");
            //TODO: Optimize input
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()), out hit, Mathf.Infinity, mask))
            {
                MouseWorldPosition = hit.point;
            }
            if (Input.GetMouseButton(0))
            {
                if(localCharWeapon != null && EventSystem.current.currentSelectedGameObject == null)
                    localPlayer.ControlledCharacter.Shoot();
            }
            if (Input.GetMouseButton(1))
            {
                localPlayer.ControlledCharacter.ReadyTimer = 0.04f;
            }
            if(Input.GetKeyDown(KeyCode.Alpha1))
            {
                localPlayer.ControlledCharacter.CastSkill(1);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                localPlayer.ControlledCharacter.CastSkill(2);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                localPlayer.ControlledCharacter.CastSkill(3);
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                localPlayer.ControlledCharacter.CastSkill(4);
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                localPlayer.ControlledCharacter.EquipWeapon(localPlayer.ControlledCharacter.WeaponSequence.MovePrevious);
            }
            if(Input.GetKeyDown(KeyCode.E))
            {
                localPlayer.ControlledCharacter.EquipWeapon(localPlayer.ControlledCharacter.WeaponSequence.MoveNext);
            }
        }
            
        #region Input callbacks

        public void Fire(CallbackContext ctx)
        {
            if (localPlayer.ControlledCharacter.EquipedWeapon != null)
                localPlayer.ControlledCharacter.Shoot();
        }

        public void Walk(CallbackContext ctx)
        {

        }

        public void SwitchWeapon(CallbackContext ctx)
        {

        }

        #endregion
    }
}