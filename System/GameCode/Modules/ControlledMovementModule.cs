using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

namespace VovTech
{
    public class ControlledMovementModule : Module
    {
        public CharacterController Controller;
        public bool LocalMovement;
        public Stat SpeedStat;
        public Stat GravityStat;
        public Stat AngularSpeedStat;
        public ParticleSystem SprintFx;
        public bool Sprinting;
        public float DriveTimer = 0;

        private Character character => GetComponent<Character>();

        public MainInputSystem InputSystem;

        private Vector3 verticalVelocity = Vector3.zero;

        /*
        private void Awake()
        {
            InputSystem = new MainInputSystem();
            InputSystem.Player.SprintIn.performed += delegate (InputAction.CallbackContext ctx)
            {
                Debug.Log("Pressed sprint");
                SpeedStat.AddModifier(SpeedStat.EffectiveValue * 1.4f, "sprint");
                SprintFx.Play();
            };

            InputSystem.Player.SprintOut.performed += delegate (InputAction.CallbackContext ctx)
            {
                Debug.Log("Pressed sprint");
                SpeedStat.RemoveModifier("sprint");
                SprintFx.Stop();
            };

            InputSystem.Player.Jump.performed += delegate (InputAction.CallbackContext ctx)
            {
                Debug.Log("Pressed jump");
                verticalVelocity += new Vector3(0, 22, 0);
            };
        }
        */

        private void Update()
        {
            if (character.IsDead) return;
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                SpeedStat.AddModifier(SpeedStat.EffectiveValue * 1.4f, "sprint");
                SprintFx.Play();
            }
            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                SpeedStat.RemoveModifier("sprint");
                SprintFx.Stop();
            }
            if(Input.GetKey(KeyCode.LeftShift))
            {
                Sprinting = true;
            } else
            {
                Sprinting = false;
            }
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                transform.Find("Drive").gameObject.GetComponent<ParticleSystem>().Play();
                Controller.enabled = false;
                GetComponent<Animator>().enabled = false;
                gameObject.transform.DOBlendableMoveBy(gameObject.transform.forward * 4, 0.15f).OnComplete(() =>
                {
                    transform.Find("Drive").gameObject.GetComponent<ParticleSystem>().Stop();
                    GetComponent<Animator>().enabled = true;
                    Controller.enabled = true;
                });
            }
            
            if (DriveTimer > 0)
            {
                transform.Find("Drive").GetComponent<ParticleSystem>().Play();
            } else
            {
                transform.Find("Drive").GetComponent<ParticleSystem>().Stop();
            }

            if (Input.GetKeyDown(KeyCode.Space) && Controller.isGrounded) verticalVelocity += new Vector3(0, 22, 0);
        }

        private void FixedUpdate()
        {
            if (Enabled && !character.IsDead)
            {
                DriveTimer = Mathf.Clamp(DriveTimer - Time.fixedDeltaTime, 0, int.MaxValue);
                float xAxis = Input.GetAxis("Horizontal");
                float zAxis = Input.GetAxis("Vertical");
                Vector3 deltaSpeed;
                if (LocalMovement)
                    deltaSpeed = transform.right * xAxis + transform.forward * zAxis;
                else
                {
                    Vector3 forward = Camera.main.transform.forward;
                    Vector3 right = Camera.main.transform.right;
                    forward.y = 0;
                    right.y = 0;
                    forward.Normalize();
                    right.Normalize();
                    deltaSpeed = forward * zAxis + right * xAxis;
                }
                Controller.Move(deltaSpeed * SpeedStat.EffectiveValue * Time.deltaTime);
                verticalVelocity.y += GravityStat.EffectiveValue / 10;
                verticalVelocity =
                        new Vector3(verticalVelocity.x, Mathf.Clamp(verticalVelocity.y, GravityStat.EffectiveValue, 1000), verticalVelocity.z);
                Controller.Move(verticalVelocity * Time.deltaTime);
                Quaternion lookRot = Quaternion.LookRotation(InputManager.Instance.MouseWorldPosition - transform.position);
                transform.rotation = Quaternion.Lerp(transform.rotation,
                    new Quaternion(0, lookRot.y, 0, lookRot.w), Time.deltaTime * AngularSpeedStat.EffectiveValue);
            }
        }
    }
}