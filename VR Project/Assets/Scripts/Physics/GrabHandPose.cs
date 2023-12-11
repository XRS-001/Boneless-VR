using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GrabHandPose : MonoBehaviour
{
    public float poseTransitionDuration = 0.2f;
    public bool dynamic;

    public HandData rightHandPose;
    public HandData leftHandPose;

    XRBaseInteractable grabInteractable;

    private Vector3 startingHandPosition;
    private Vector3 finalHandPosition;
    private Quaternion startingHandRotation;
    private Quaternion finalHandRotation;

    private Quaternion[] startingFingerRotations;
    private Quaternion[] finalFingerRotations;
    private Vector3[] finalFingerPositions;

    // Start is called before the first frame update
    void Start()
    {
        grabInteractable = GetComponent<XRBaseInteractable>();

        grabInteractable.selectEntered.AddListener(SetupPose);
        grabInteractable.selectExited.AddListener(UnSetPose);

        leftHandPose.gameObject.SetActive(false);
        rightHandPose.gameObject.SetActive(false);
    }

    public void SetupPose(BaseInteractionEventArgs args)
    {
        if (args.interactorObject is XRDirectInteractor)
        {
            HandData handData = args.interactorObject.transform.GetComponentInChildren<ControllerInteractors>().handRig;

            handData.animator.enabled = false;

            if(handData.handType == HandData.HandModelType.right)
            {
                SetHandDataValues(handData, rightHandPose);
            }
            else
            {
                SetHandDataValues(handData, leftHandPose);
            }

            StartCoroutine(SetHandDataRoutine(handData, finalHandPosition, finalHandRotation, finalFingerRotations, finalFingerPositions, startingHandPosition, startingHandRotation, startingFingerRotations));
        }
    }
    public void UnSetPose(BaseInteractionEventArgs args)
    {
        if (args.interactorObject is XRDirectInteractor && gameObject.activeInHierarchy)
        {
            HandData handData = args.interactorObject.transform.GetComponentInChildren<ControllerInteractors>().handRig;
            handData.animator.enabled = true;

            SetHandData(handData, startingHandPosition, startingHandRotation, startingFingerRotations);
        }
    }
    public void SetHandDataValues(HandData h1, HandData h2)
    {
        startingHandPosition = new Vector3(h1.root.localPosition.x / h1.root.localScale.x, h1.root.localPosition.y / h1.root.localScale.y, h1.root.localPosition.z / h1.root.localScale.z);
        finalHandPosition = new Vector3(h2.root.localPosition.x / h2.root.localScale.x, h2.root.localPosition.y / h2.root.localScale.y, h2.root.localPosition.z / h2.root.localScale.z);

        startingHandRotation = h1.root.localRotation;
        finalHandRotation = h2.root.localRotation;

        startingFingerRotations = new Quaternion[h1.fingerBones.Length];
        finalFingerRotations = new Quaternion[h2.fingerBones.Length];
        finalFingerPositions = new Vector3[h2.fingerBones.Length];

        for (int i = 0; i < h1.fingerBones.Length; i++)
        {
            startingFingerRotations[i] = h1.fingerBones[i].localRotation;
            finalFingerRotations[i] = h2.fingerBones[i].localRotation;
            finalFingerPositions[i] = h2.fingerBones[i].position;
        }
    }

    public void SetHandData(HandData h, Vector3 newPosition, Quaternion newRotation, Quaternion[] newBonesRotation)
    {
        h.root.localPosition = newPosition;
        h.root.localRotation = newRotation;

        for (int i = 0; i < newBonesRotation.Length; i++)
        {
            h.fingerBones[i].localRotation = newBonesRotation[i];
        }
    }
    public IEnumerator SetHandDataRoutine(HandData h, Vector3 newPosition, Quaternion newRotation, Quaternion[] newBonesRotation, Vector3[] newBonesPosition, Vector3 startingPosition, Quaternion startingRotation, Quaternion[] startingBonesRotation)
    {
        if(dynamic)
        {
            float timer = 0;

            while (timer < poseTransitionDuration)
            {
                Quaternion r = Quaternion.Lerp(startingRotation, newRotation, timer / poseTransitionDuration);
                h.root.localRotation = r;

                for (int i = 0; i < newBonesRotation.Length; i++)
                {
                    bool checkSphere = true;
                    foreach (Collider collider in Physics.OverlapSphere(h.fingerBones[i].transform.position, 0.95f))
                    {
                        if (!collider.CompareTag("Interactable"))
                        {
                            checkSphere = false;
                        }
                        else
                        {
                            checkSphere = true;
                        }
                    }
                    if (!checkSphere)
                    {
                        float lerpSpeed = timer / poseTransitionDuration;
                        Quaternion boneRotation = Quaternion.Lerp(startingBonesRotation[i], newBonesRotation[i], lerpSpeed);
                        h.fingerBones[i].localRotation = boneRotation;
                    }
                }

                timer += Time.deltaTime;
                yield return null;
            }
        }
        else
        {
            float timer = 0;

            while (timer < poseTransitionDuration)
            {
                Quaternion r = Quaternion.Lerp(startingRotation, newRotation, timer / poseTransitionDuration);

                h.root.localRotation = r;

                for (int i = 0; i < newBonesRotation.Length; i++)
                {
                    h.fingerBones[i].localRotation = Quaternion.Lerp(startingBonesRotation[i], newBonesRotation[i], timer / poseTransitionDuration);
                }

                timer += Time.deltaTime;
                yield return null;
            }
        }
    }
#if UNITY_EDITOR

    [MenuItem("Tools/mirror selected right grab pose")]
    public static void MirrorRightPose()
    {
        GrabHandPose handPose = Selection.activeGameObject.GetComponent<GrabHandPose>();
        handPose.MirrorPose(handPose.leftHandPose, handPose.rightHandPose);
    }
#endif
    public void MirrorPose(HandData poseToMirror, HandData poseUsedToMirror)
    {
        Vector3 mirroredPosition = poseToMirror.root.localPosition;
        mirroredPosition.x *= -1;

        Quaternion mirroredQuaternion = poseUsedToMirror.root.localRotation;
        mirroredPosition.y *= -1;
        mirroredPosition.z *= -1;

        poseToMirror.root.localPosition = mirroredPosition;
        poseToMirror.root.rotation = mirroredQuaternion;

        for (int i = 0; i < poseUsedToMirror.fingerBones.Length; i++)
        {
            poseToMirror.fingerBones[i].localRotation = poseUsedToMirror.fingerBones[i].localRotation;
        }
    }
}
