using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform tutorialUI;
    [SerializeField] private GameObject XRRig;
    [SerializeField] private InputActionReference AButtonReference;

    [Header("Targets and Platforms")]
    [SerializeField] private Transform firstTargets;
    [SerializeField] private Transform secondTarget;
    [SerializeField] private GameObject secondTargetPlatforms;
    [SerializeField] private Transform finalTargets;
    [SerializeField] private GameObject finalTargetsPlatforms;

    private int tutorialSequenceNum = 0;

    private void Update()
    {
        switch (tutorialSequenceNum)
        {
            case 0: //welcome sign

                if (AButtonReference.action.triggered)
                {
                    tutorialSequenceNum = 1;
                    tutorialUI.GetChild(0).gameObject.SetActive(false); //turn off UI
                    XRRig.GetComponent<ActionBasedContinuousMoveProvider>().enabled = true; //enable movement
                    XRRig.GetComponent<ControllerInputManager>().canJump = true; //enable basic jumping
                }

                break;

            case 1: //once both staffs have been taken, bring up first tutorial sign

                if(XRRig.GetComponent<ControllerInputManager>().holdingStaffPieceL && XRRig.GetComponent<ControllerInputManager>().holdingStaffPieceR)
                {
                    tutorialSequenceNum = 2;
                    tutorialUI.GetChild(1).gameObject.SetActive(true); //turn on UI
                    XRRig.GetComponent<ActionBasedContinuousMoveProvider>().enabled = false; //disable movement
                    XRRig.GetComponent<ControllerInputManager>().canJump = false; //disable basic jumping
                    firstTargets.gameObject.SetActive(true); //turn on first targets
                }

                break;

            case 2: //target shooting time

                if (AButtonReference.action.triggered)
                {
                    tutorialSequenceNum = 3;
                    tutorialUI.GetChild(1).gameObject.SetActive(false); //turn off UI
                    XRRig.GetComponent<ControllerInputManager>().canShoot = true; //enable shooting projectiles
                    
                }

                break;

            case 3:

                if(firstTargets.childCount == 0) //all targets hit
                {
                    tutorialSequenceNum = 4;
                    tutorialUI.GetChild(2).gameObject.SetActive(true); //turn on UI
                    XRRig.GetComponent<ControllerInputManager>().canShoot = false; //disable shooting projectiles
                    firstTargets.gameObject.SetActive(false); //turn off first targets
                    secondTarget.gameObject.SetActive(true); //turn on second target
                    secondTargetPlatforms.SetActive(true); //turn on platforms for second target
                }

                break;

            case 4:

                if (AButtonReference.action.triggered) //platforming target time
                {
                    tutorialSequenceNum = 5;
                    tutorialUI.GetChild(2).gameObject.SetActive(false); //turn off UI
                    XRRig.GetComponent<ActionBasedContinuousMoveProvider>().enabled = true; //enable movement
                    XRRig.GetComponent<ControllerInputManager>().canShoot = true; //enable shooting projectiles
                    XRRig.GetComponent<ControllerInputManager>().canJump = true; //enable basic jumping
                    XRRig.GetComponent<ControllerInputManager>().canDoubleJump = true; //enable double jumping
                }

                break;

            case 5:

                if(secondTarget.childCount == 0) //platforming target hit
                {
                    tutorialSequenceNum = 6;
                    XRRig.transform.position = new Vector3(0, 0, 15.8f); //teleport player back to table
                    tutorialUI.GetChild(3).gameObject.SetActive(true); //turn on UI
                    XRRig.GetComponent<ActionBasedContinuousMoveProvider>().enabled = false; //disable movement
                    XRRig.GetComponent<ControllerInputManager>().canJump = false; //disable basic jumping
                    XRRig.GetComponent<ControllerInputManager>().canShoot = false; //disable shooting projectiles
                    secondTarget.gameObject.SetActive(false); //turn off second target
                    secondTargetPlatforms.SetActive(false); //turn off platforms for second target
                    finalTargets.gameObject.SetActive(true); //turn on final targets
                    finalTargetsPlatforms.SetActive(true); //turn on platforms for final targets
                }

                break;

            case 6:

                if(XRRig.transform.position != new Vector3(0, 0, 15.8f))
                {
                    XRRig.transform.position = new Vector3(0, 0, 15.8f); //teleport player back to table
                }

                if (AButtonReference.action.triggered) //final targets time
                {
                    tutorialSequenceNum = 7;
                    tutorialUI.GetChild(3).gameObject.SetActive(false); //turn off UI
                    XRRig.GetComponent<ActionBasedContinuousMoveProvider>().enabled = true; //enable movement
                    XRRig.GetComponent<ControllerInputManager>().canJump = true; //enable basic jumping
                    XRRig.GetComponent<ControllerInputManager>().canShoot = true; //enable shooting projectiles
                    XRRig.GetComponent<ControllerInputManager>().canShield = true; //enable using shields (THIS IS JUST EXTRA FEATURE AND NOT PART OF TUTORIAL)
                }

                break;

            case 7:

                if(finalTargets.childCount == 0) //all final targets hit
                {
                    tutorialSequenceNum = 8;
                    tutorialUI.GetChild(4).gameObject.SetActive(true); //turn on UI
                    XRRig.transform.position = new Vector3(0, 0, 15.8f); //teleport player back to table
                    XRRig.GetComponent<ActionBasedContinuousMoveProvider>().enabled = false; //disable movement
                    XRRig.GetComponent<ControllerInputManager>().canJump = false; //disable basic jumping
                    XRRig.GetComponent<ControllerInputManager>().canShoot = false; //disable shooting projectiles
                    finalTargets.gameObject.SetActive(false); //turn off final targets
                    finalTargetsPlatforms.SetActive(false); //turn off platforms for final targets
                }

                break;

            case 8:

                if (AButtonReference.action.triggered) //return to main menu
                {
                    SceneManager.LoadScene(0); 
                }

                break;

            default:

                Debug.Log("tutorial sequence number wrong");

                break;
        }
    }
}
