using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class RifleFire : MonoBehaviour
{
    public GameObject slide;
    private XRGrabJoint slideGrab;
    public ParticleSystem fireParticles;
    public AudioSource audioSource;
    public AudioClip bulletFire;
    public AudioClip gunLoad;
    public AudioClip slideSound;
    public XRGrabInteractableRifle grabInteractable;
    private float threshold = 140f;
    public InputActionProperty fireInputSourceLeft;
    public InputActionProperty fireInputSourceRight;
    public InputActionProperty magReleaseInputSourceRight;
    public InputActionProperty magReleaseInputSourceLeft;
    public int ammoCapacity;
    private bool isInGun;
    public Transform bulletFirePosition;
    public Transform casingEjectPosition;
    public GameObject magazine;
    public string magazineName;
    public GameObject animatedMagazine;
    private GameObject gunMagazine;
    private bool hasSlide = true;
    public GameObject bullet;
    public GameObject casing;
    public Animator animator;
    public float bulletSpeed;
    public float recoilSpeed;
    private float fireButton;
    private float magRelease;

    public float fireCooldown = 0f;
    private float timeSinceLastShot = 0f;
    private float timeBetweenShots = 1.0f / 10.0f;
    private bool canFire = true;
    private bool isFiring = false;
    public bool slideRetracted = true;
    private bool canClick = true;
    private GameObject spawnedMagazine;
    private void Start()
    {
        slideGrab = slide.GetComponent<XRGrabJoint>();
    }
    public void FireBullet()
    {
        if (ammoCapacity > 0 && slideRetracted)
        {
            audioSource.PlayOneShot(bulletFire);
            fireParticles.Play();

            GameObject spawnedBullet = Instantiate(bullet);
            spawnedBullet.transform.position = bulletFirePosition.position;
            spawnedBullet.transform.rotation = bulletFirePosition.rotation;

            GameObject recoilBullet = Instantiate(bullet);
            recoilBullet.transform.position = bulletFirePosition.position;
            recoilBullet.transform.rotation = bulletFirePosition.rotation;
            recoilBullet.GetComponent<PlayParticleEffectOnCollision>().effect = null;
            recoilBullet.GetComponent<AudioSource>().enabled = false;

            GameObject spawnedCasing = Instantiate(casing);
            spawnedCasing.transform.position = casingEjectPosition.position;
            spawnedCasing.transform.rotation = casingEjectPosition.rotation;

            Rigidbody casingRb = spawnedCasing.GetComponent<Rigidbody>();
            casingRb.AddForce(casingEjectPosition.right * 1000 * Time.deltaTime);
            Destroy(spawnedCasing, 10);

            Physics.IgnoreCollision(spawnedBullet.GetComponent<Collider>(), recoilBullet.GetComponent<Collider>());

            Rigidbody bulletRb = spawnedBullet.GetComponent<Rigidbody>();
            bulletRb.AddForce(-bulletFirePosition.up * bulletSpeed * Time.deltaTime);

            Rigidbody recoilRb = recoilBullet.GetComponent<Rigidbody>();
            recoilRb.AddForce(bulletFirePosition.up * recoilSpeed);

            Destroy(recoilBullet, 1);
            Destroy(spawnedBullet, 5);

            timeSinceLastShot = 0f;
        }
        if (ammoCapacity > 0 && slideRetracted)
        {
            ammoCapacity--;
        }
        if (slideRetracted == false && canClick)
        {
            SlideRetractTrigger();
        }
        if (ammoCapacity == 0 && slideRetracted)
        {
            slideRetracted = false;
            slide.GetComponent<ConfigurableJoint>().targetPosition *= -1;
            slideGrab.selectEntered.AddListener(SlideRetract);
        }
    }
    public void SlideRetract(SelectEnterEventArgs args)
    {
        slideRetracted = true;
        hasSlide = false;
        Slide();
        slide.GetComponent<ConfigurableJoint>().targetPosition *= -1;
        slideGrab.selectEntered.RemoveListener(SlideRetract);
    }
    public void SlideRetractTrigger()
    {
        canClick = false;
        audioSource.PlayOneShot(slideSound);
        slideRetracted = true;
        hasSlide = false;
        Slide();
        slide.GetComponent<ConfigurableJoint>().targetPosition *= -1;
        slideGrab.selectEntered.RemoveListener(SlideRetract);
    }
    private void Update()
    {
        timeSinceLastShot += Time.deltaTime;

        if (grabInteractable.rightHandGrabbing)
        {
            fireButton = fireInputSourceRight.action.ReadValue<float>();
            magRelease = magReleaseInputSourceRight.action.ReadValue<float>();
        }
        else if (grabInteractable.leftHandGrabbing)
        {
            fireButton = fireInputSourceLeft.action.ReadValue<float>();
            magRelease = magReleaseInputSourceLeft.action.ReadValue<float>();
        }

        if (fireButton > 0.1f && canFire && (hasSlide || slideRetracted == false))
        {
            isFiring = true;
        }
        else
        {
            isFiring = false;
        }

        if (isFiring)
        {
            if (timeSinceLastShot >= timeBetweenShots)
            {
                FireBullet();
                timeSinceLastShot = 0f;
                canFire = false;
                StartCoroutine(EnableFire());
            }
        }

        if (isInGun && magRelease > 0.1f)
        {
            ReleaseMagazine();
        }
        if(fireButton == 0)
        {
            canClick = true;
        }
    }

    IEnumerator EnableFire()
    {
        yield return new WaitForSeconds(timeBetweenShots);
        canFire = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        gunMagazine = other.gameObject;
        float angle = Quaternion.Angle(gunMagazine.transform.rotation, transform.rotation);
        if (gunMagazine.CompareTag("Magazine"))
        {
            if (angle < threshold && gunMagazine.GetComponent<GunMagazine>().magazineName == magazineName && !isInGun && other.gameObject.GetComponent<XRGrabInteractableTwoAttach>().isSelected)
            {
                if (ammoCapacity > 0)
                {
                    hasSlide = true;
                }
                else
                {
                    hasSlide = false;
                }
                audioSource.PlayOneShot(gunLoad);
                ammoCapacity += gunMagazine.GetComponent<GunMagazine>().ammoCapacity;
                animatedMagazine.SetActive(true);
                gunMagazine.GetComponent<XRGrabInteractableTwoAttach>().controllerGrabbing.allowSelect = false;
                gunMagazine.GetComponent<XRGrabInteractableTwoAttach>().controllerGrabbing.allowSelect = true;
                Destroy(gunMagazine.gameObject);
                animator.Play("Reload");
                isInGun = true;
                animatedMagazine.GetComponent<XRSimpleInteractable>().selectEntered.AddListener(ReleaseMagazineGrabbed);
            }
        }
    }
    public void Slide()
    {
        if (!hasSlide)
        {
            hasSlide = true;
        }
        else if (ammoCapacity > 0 && hasSlide)
        {
            if(slideGrab.isGrabbing)
            {
                GameObject spawnedCasing = Instantiate(casing);
                spawnedCasing.transform.position = casingEjectPosition.position;
                spawnedCasing.transform.rotation = casingEjectPosition.rotation;

                Rigidbody casingRb = spawnedCasing.GetComponent<Rigidbody>();
                casingRb.AddForce(casingEjectPosition.right * 10000 * Time.deltaTime);
                Destroy(spawnedCasing, 10);
                ammoCapacity--;
            }
        }
        if (slideGrab.isGrabbing)
        {
            audioSource.PlayOneShot(slideSound);
        }
    }

    public void ReleaseMagazine()
    {
        animator.Play("Release");
        audioSource.PlayOneShot(gunLoad);
    }
    public void SpawnMagazine()
    {
        if(isInGun)
        {
            animatedMagazine.SetActive(false);
            spawnedMagazine = Instantiate(magazine, animatedMagazine.transform.position, animatedMagazine.transform.rotation);
            spawnedMagazine.GetComponent<GunMagazine>().ammoCapacity = ammoCapacity;
            ammoCapacity = 0;
            if (spawnedMagazine.GetComponent<GunMagazine>().ammoCapacity > 0 && hasSlide)
            {
                spawnedMagazine.GetComponent<GunMagazine>().ammoCapacity--;
                ammoCapacity++;
            }
        }
        isInGun = false;
    }
    public void ReleaseMagazineGrabbed(SelectEnterEventArgs args)
    {
        isInGun = false;
        audioSource.PlayOneShot(gunLoad);
        animatedMagazine.SetActive(false);
        animator.Play("Release");
        spawnedMagazine = Instantiate(magazine, animatedMagazine.transform.position, animatedMagazine.transform.rotation);
        spawnedMagazine.GetComponent<GunMagazine>().ammoCapacity = ammoCapacity;
        ammoCapacity = 0;
        if (spawnedMagazine.GetComponent<GunMagazine>().ammoCapacity > 0 && hasSlide)
        {
            spawnedMagazine.GetComponent<GunMagazine>().ammoCapacity--;
            ammoCapacity++;
        }
        XRInteractionManager interactionManager = args.interactorObject.transform.GetComponent<ControllerInteractors>().interactionManager;
        ControllerInteractors controller = args.interactorObject.transform.GetComponent<ControllerInteractors>();
        XRGrabInteractableTwoAttach magAttach = spawnedMagazine.GetComponent<XRGrabInteractableTwoAttach>();
        GrabHandPose magPose = spawnedMagazine.GetComponent<GrabHandPose>();
        magPose.SetupPose(args);
        if (controller.CompareTag("RightHand"))
        {
            magAttach.attachTransform = magAttach.rightAttach;
        }
        else
        {
            magAttach.attachTransform = magAttach.leftAttach;
        }
        interactionManager.SelectEnter(controller, spawnedMagazine.GetComponent<XRGrabInteractableTwoAttach>());
        animatedMagazine.GetComponent<XRSimpleInteractable>().selectEntered.RemoveListener(ReleaseMagazineGrabbed);
    }
}


