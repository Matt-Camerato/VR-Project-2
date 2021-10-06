using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

//IN THE FUTURE, COULD MAKE A SEPARATE CLASS FOR STAFF WHICH WOULD ALLOW ACCESS TO POS + ROT, ALONG WITH OTHER THINGS
//DEFINTELY SPLIT THIS INTO MULTIPLE SCRIPTS IN THE FUTURE

public class ControllerInputManager : MonoBehaviour
{
    [Header("Right Controller Input")]
    [SerializeField] private XRBaseController controllerR;
    [SerializeField] private XRDirectInteractor directInteractorR;
    [SerializeField] private InputActionReference trigger_R;
    [SerializeField] private InputActionReference joystickPressed_R;
    [SerializeField] private InputActionReference jumpPressed;

    [Header("Left Controller Input")]
    [SerializeField] private XRBaseController controllerL;
    [SerializeField] private XRDirectInteractor directInteractorL;
    [SerializeField] private InputActionReference trigger_L;
    [SerializeField] private InputActionReference joystickPressed_L;

    [Header("Player Settings")]
    [SerializeField] private float jumpPower;
    [SerializeField] private float gravity = 9.8f;
    public bool canShoot = false;
    public bool canJump = false;
    public bool canDoubleJump = false;
    public bool canShield = false;

    [Header("Orb Prefab")]
    [SerializeField] private GameObject orbPrefab;

    private Vector3 moveDir = Vector3.zero;
    private bool grounded = false;

    public bool holdingStaffPieceR;
    public bool holdingStaffPieceL;
    private GameObject staffPieceR;
    private GameObject staffPieceL;

    private void OnEnable()
    {
        directInteractorR.selectEntered.AddListener(ObjectAttachedR);
        directInteractorR.selectExited.AddListener(ObjectDetachedR);

        directInteractorL.selectEntered.AddListener(ObjectAttachedL);
        directInteractorL.selectExited.AddListener(ObjectDetachedL);
    }

    private void OnDisable()
    {
        directInteractorR.selectEntered.RemoveListener(ObjectAttachedR);
        directInteractorR.selectExited.RemoveListener(ObjectDetachedR);

        directInteractorL.selectEntered.RemoveListener(ObjectAttachedL);
        directInteractorL.selectExited.RemoveListener(ObjectDetachedL);
    }

    private void ObjectAttachedR(SelectEnterEventArgs arg0)
    {
        if(arg0.interactable.CompareTag("Staff Piece"))
        { 
            holdingStaffPieceR = true;
            staffPieceR = arg0.interactable.gameObject;
            Vector3 tempPos = arg0.interactable.transform.GetChild(0).transform.localPosition;
            arg0.interactable.transform.GetChild(0).transform.localPosition = new Vector3(-0.5f, tempPos.y, tempPos.z); //adjust attach transform based on hand
        }
    }

    private void ObjectDetachedR(SelectExitEventArgs arg0)
    {
        if (arg0.interactable.CompareTag("Staff Piece"))
        { 
            holdingStaffPieceR = false;
            staffPieceR = null;
            if (arg0.interactable.transform.GetChild(2).gameObject.activeSelf) { arg0.interactable.transform.GetChild(2).gameObject.SetActive(false); }//turn off shield on drop in case it was left on
        }
    }

    private void ObjectAttachedL(SelectEnterEventArgs arg0)
    {
        if (arg0.interactable.CompareTag("Staff Piece"))
        {
            holdingStaffPieceL = true;
            staffPieceL = arg0.interactable.gameObject;
            Vector3 tempPos = arg0.interactable.transform.GetChild(0).transform.localPosition;
            arg0.interactable.transform.GetChild(0).transform.localPosition = new Vector3(0.5f, tempPos.y, tempPos.z); //adjust attach transform based on hand
        }
    }

    private void ObjectDetachedL(SelectExitEventArgs arg0)
    {
        if (arg0.interactable.CompareTag("Staff Piece"))
        {
            holdingStaffPieceL = false;
            staffPieceL = null;
            if (arg0.interactable.transform.GetChild(2).gameObject.activeSelf) { arg0.interactable.transform.GetChild(2).gameObject.SetActive(false); }//turn off shield on drop in case it was left on
        }
    }

    private void FixedUpdate()
    {
        if (GetComponent<CharacterController>().isGrounded) { grounded = true; }
        else { grounded = false; }
    }

    private bool activatingShieldR = false;
    private bool activatingShieldL = false;

    private void Update()
    {
        //safety in case player falls out of map
        if (transform.position.y < -100)
        {
            transform.position = new Vector3(0, 3, 0);
        }

        HandleJump();

        if (canShield) { HandleShields(); }

        if (canShoot) { HandleProjectiles(); }

        /*
        if (holdingStaffPieceR && holdingStaffPieceL)
        {
            //Debug.Log(Vector3.Angle(staffPieceR.transform.up, Vector3.up));

            if (Vector3.Angle(staffPieceR.transform.up, Vector3.up) < 10f && Vector3.Angle(staffPieceL.transform.up, Vector3.up) < 10f //check if both staff pieces are upright
                && triggerR == 1 && triggerL == 1) //check if both triggers are pressed
            {
                Debug.Log("staff pieces upright and both triggers pressed!");

            }
        }
        */
    }

    private bool doubleJumpedL = false;
    private bool doubleJumpedR = false;

    private void HandleJump()
    {
        if (grounded)
        {
            moveDir.y = 0;

            doubleJumpedL = false;
            doubleJumpedR = false;

            if (jumpPressed.action.triggered && canJump) //inital jump check
            { 
                moveDir.y = jumpPower;


                //These make shields disabled when player is in air
                //if (staffPieceR != null) { staffPieceR.transform.GetChild(2).gameObject.SetActive(false); }
                //if (staffPieceL != null) { staffPieceL.transform.GetChild(2).gameObject.SetActive(false); }
            } 
        }
        else
        {
            if (canDoubleJump)
            {
                if (joystickPressed_R.action.triggered && holdingStaffPieceR && Vector3.Angle(staffPieceR.transform.up, Vector3.down) < 45f && !doubleJumpedR) //right double jump check
                {
                    doubleJumpedR = true;
                    Vector3 jumpDir = -staffPieceR.transform.up;
                    moveDir = jumpDir.normalized * (jumpPower + 1);

                    staffPieceR.GetComponent<StaffAudioController>().JumpSFX(); //play jump sound
                }
                else if (joystickPressed_L.action.triggered && holdingStaffPieceL && Vector3.Angle(staffPieceL.transform.up, Vector3.down) < 45f && !doubleJumpedL) //
                {
                    doubleJumpedL = true;
                    Vector3 jumpDir = -staffPieceL.transform.up;
                    moveDir = jumpDir.normalized * (jumpPower + 1);

                    staffPieceL.GetComponent<StaffAudioController>().JumpSFX(); //play jump sound
                }
            }
        }

        if(moveDir.x < 0)
        {
            moveDir.x += 8 * Time.deltaTime;
            if(moveDir.x >= 0)
            {
                moveDir.x = 0;
            }
        }
        else if(moveDir.x > 0)
        {
            moveDir.x -= 8 * Time.deltaTime;
            if (moveDir.x <= 0)
            {
                moveDir.x = 0;
            }
        }

        if (moveDir.z < 0)
        {
            moveDir.z += 8 * Time.deltaTime;
            if (moveDir.z >= 0)
            {
                moveDir.z = 0;
            }
        }
        else if (moveDir.z > 0)
        {
            moveDir.z -= 8 * Time.deltaTime;
            if (moveDir.z <= 0)
            {
                moveDir.z = 0;
            }
        }

        moveDir.y -= gravity * Time.deltaTime;

        GetComponent<CharacterController>().Move(moveDir * Time.deltaTime);
    }

    private Vector3 activateShieldDirR;
    private Vector3 activateShieldDirL;

    private void HandleShields()
    {
        if (grounded)
        {
            if (holdingStaffPieceR)
            {
                if (!activatingShieldR)
                {
                    if (Vector3.Angle(staffPieceR.transform.up, Vector3.up) < 15f && joystickPressed_R.action.triggered)
                    {
                        activatingShieldR = true;
                        activateShieldDirR = -staffPieceR.transform.right;
                    }
                }
                else
                {
                    if (joystickPressed_R.action.ReadValue<float>() < 1)
                    {
                        activatingShieldR = false;
                        if (Vector3.Angle(staffPieceR.transform.up, activateShieldDirR) < 15f)
                        {
                            staffPieceR.transform.GetChild(2).gameObject.SetActive(true);
                        }
                    }
                }

                //deactivate shield by pressing joystick again
                if (staffPieceR.transform.GetChild(2).gameObject.activeSelf && joystickPressed_R.action.triggered)
                {
                    staffPieceR.transform.GetChild(2).gameObject.SetActive(false);
                    activatingShieldR = false;
                }

            }
            else
            {
                activatingShieldR = false;
            }

            if (holdingStaffPieceL)
            {
                if (!activatingShieldL)
                {
                    if (Vector3.Angle(staffPieceL.transform.up, Vector3.up) < 15f && joystickPressed_L.action.triggered)
                    {
                        activatingShieldL = true;
                        activateShieldDirL = staffPieceL.transform.right;
                    }
                }
                else
                {
                    if (joystickPressed_L.action.ReadValue<float>() < 1)
                    {
                        activatingShieldL = false;
                        if (Vector3.Angle(staffPieceL.transform.up, activateShieldDirL) < 15f)
                        {
                            staffPieceL.transform.GetChild(2).gameObject.SetActive(true);
                        }
                    }
                }

                //deactivate shield by pressing joystick again
                if (staffPieceL.transform.GetChild(2).gameObject.activeSelf && joystickPressed_L.action.triggered)
                {
                    staffPieceL.transform.GetChild(2).gameObject.SetActive(false);
                    activatingShieldL = false;
                }
            }
            else
            {
                activatingShieldL = false;
            }
        }
        else
        {
            activatingShieldR = false;
            activatingShieldL = false;
        }
    }

    private GameObject currentProjectileR = null;
    private GameObject currentProjectileL = null;

    private bool chargingProjectileR = false;
    private bool chargingProjectileL = false;

    private bool readyToFireR = false;
    private bool readyToFireL = false;

    private void HandleProjectiles()
    {
        if (holdingStaffPieceR && !staffPieceR.transform.GetChild(2).gameObject.activeSelf) //if right shield isnt active
        {
            if (trigger_R.action.ReadValue<float>() > 0.5)
            {
                if (!chargingProjectileR)
                {
                    chargingProjectileR = true;
                    currentProjectileR = Instantiate(orbPrefab, staffPieceR.transform);
                    currentProjectileR.transform.localPosition = new Vector3(0, 1.4f, 0);
                    currentProjectileR.transform.localScale = Vector3.zero;
                }
                else
                {
                    if (currentProjectileR != null && currentProjectileR.transform.localScale.x < 2.4f)
                    {
                        currentProjectileR.transform.localScale += new Vector3(2.4f, 0.5217391f, 2.4f) * 2.2f * Time.deltaTime; //grow orb until max size
                    }
                    else
                    {
                        readyToFireR = true;
                    }
                }
            }
            else
            {
                if (currentProjectileR != null)
                {
                    if (readyToFireR) //fire fully grown orb after releasing trigger
                    {
                        readyToFireR = false;

                        currentProjectileR.GetComponent<OrbController>().moveDir = staffPieceR.transform.up; //this line is NOT TRIGGERING!!!!!!!! current projectile reference doesn't exist
                        currentProjectileR.GetComponent<OrbController>().fired = true;
                        currentProjectileR.transform.parent = null;

                        staffPieceR.GetComponent<StaffAudioController>().SpellFiredSFX(); //play sound effect

                        currentProjectileR = null;
                    }
                    else
                    {
                        Destroy(currentProjectileR); //destroy projectile if trigger released before growing
                    }
                }

                if (chargingProjectileR) { chargingProjectileR = false; }
            }
        }

        if(holdingStaffPieceL && !staffPieceL.transform.GetChild(2).gameObject.activeSelf) //if left shield isnt active
        {
            if (trigger_L.action.ReadValue<float>() > 0.5)
            {
                if (!chargingProjectileL)
                {
                    chargingProjectileL = true;
                    currentProjectileL = Instantiate(orbPrefab, staffPieceL.transform);
                    currentProjectileL.transform.localPosition = new Vector3(0, 1.4f, 0);
                    currentProjectileL.transform.localScale = Vector3.zero;
                }
                else
                {
                    if (currentProjectileL != null && currentProjectileL.transform.localScale.x < 2.4f)
                    {
                        currentProjectileL.transform.localScale += new Vector3(2.4f, 0.5217391f, 2.4f) * 2.2f * Time.deltaTime; //grow orb until max size
                    }
                    else
                    {
                        readyToFireL = true;
                    }
                }
            }
            else
            {
                if (currentProjectileL != null)
                {
                    if (readyToFireL) //fire fully grown orb after releasing trigger
                    {
                        readyToFireL = false;

                        currentProjectileL.GetComponent<OrbController>().moveDir = staffPieceL.transform.up; //this line is NOT TRIGGERING!!!!!!!! current projectile reference doesn't exist
                        currentProjectileL.GetComponent<OrbController>().fired = true;
                        currentProjectileL.transform.parent = null;

                        staffPieceL.GetComponent<StaffAudioController>().SpellFiredSFX(); //play sound effect

                        currentProjectileL = null;
                    }
                    else
                    {
                        Destroy(currentProjectileL); //destroy projectile if trigger released before growing
                    }
                }

                if (chargingProjectileL) { chargingProjectileL = false; }
            }
        }
    }
}
