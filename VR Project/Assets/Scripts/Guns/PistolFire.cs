using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class PistolFire : MonoBehaviour
{
    public GameObject slide;
    private XRGrabJoint slideGrab;
    public ParticleSystem fireParticles;
    public AudioSource audioSource;
    public AudioClip bulletFire;
    public AudioClip gunLoad;
    public AudioClip slideSound;
    public XRGrabInteractablePistol grabInteractable;
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
    private GameObject pistolMagazine;
    private bool hasSlide = true;
    public GameObject bullet;
    public GameObject casing;
    public Animator animator;
    public float bulletSpeed;
    public float recoilSpeed;
    private float fireButton;
    private float magRelease;

    public float fireCooldown = 0.5f;
    private float timeSinceLastShot = 0f;
    private bool triggerReleased = true;
    public bool slideRetracted = true;
    private void Start()
    {
        slideRetracted = true;
        slideGrab = slide.GetComponent<XRGrabJoint>();
    }
    public void FireBullet()
    {
        if(ammoCapacity > 0 && slideRetracted)
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

            Physics.IgnoreCollision(spawnedBullet.GetComponent<Collider>(), recoilBullet.GetComponent<Collider>());

            Rigidbody bulletRb = spawnedBullet.GetComponent<Rigidbody>();
            bulletRb.AddForce(-bulletFirePosition.up * bulletSpeed * Time.deltaTime);

            Rigidbody recoilRb = recoilBullet.GetComponent<Rigidbody>();
            recoilRb.AddForce(bulletFirePosition.up * recoilSpeed);

            Destroy(recoilBullet, 1);
            Destroy(spawnedBullet, 1);

            timeSinceLastShot = 0f;
        }
        if(slideRetracted == false)
        {
            SlideRetractTrigger();
        }
        if(ammoCapacity == 1)
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
        audioSource.PlayOneShot(slideSound);
        slideRetracted = true;
        hasSlide = false;
        Slide();
        slide.GetComponent<ConfigurableJoint>().targetPosition *= -1;
        slideGrab.selectEntered.RemoveListener(SlideRetract);
    }
    void Update()
    {
        timeSinceLastShot += Time.deltaTime;

        if (grabInteractable.rightHandGrabbing)
        {
            fireButton = fireInputSourceRight.action.ReadValue<float>();
            magRelease = magReleaseInputSourceRight.action.ReadValue<float>();
        }
        else if(grabInteractable.leftHandGrabbing)
        {
            fireButton = fireInputSourceLeft.action.ReadValue<float>();
            magRelease = magReleaseInputSourceLeft.action.ReadValue<float>();
        }
        if (fireButton > 0.1f && triggerReleased && timeSinceLastShot >= fireCooldown && (hasSlide || slideRetracted == false))
        {
            FireBullet();
            triggerReleased = false;
        }
        if (fireButton < 0.1f)
        {
            triggerReleased = true;
        }
        if(isInGun && magRelease > 0.1f)
        {
            animator.Play("Release");
            StartCoroutine(Delay());
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        pistolMagazine = other.gameObject;
        float angle = Quaternion.Angle(pistolMagazine.transform.rotation, transform.rotation);
        if (pistolMagazine.CompareTag("Magazine") && pistolMagazine.GetComponent<GunMagazine>())
        {
            if (angle < threshold && pistolMagazine.GetComponent<GunMagazine>().magazineName == magazineName && !isInGun && other.gameObject.GetComponent<XRGrabInteractableTwoAttach>().isSelected)
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
                ammoCapacity += pistolMagazine.GetComponent<GunMagazine>().ammoCapacity;
                animatedMagazine.SetActive(true);
                pistolMagazine.GetComponent<XRGrabInteractableTwoAttach>().controllerGrabbing.allowSelect = false;
                pistolMagazine.GetComponent<XRGrabInteractableTwoAttach>().controllerGrabbing.allowSelect = true;
                Destroy(pistolMagazine.gameObject);
                animator.Play("Reload");
                isInGun = true;
            }
        }
    }
    public void Slide()
    {
        if (!hasSlide)
        {
            hasSlide = true;
        }
        else if(ammoCapacity > 0 && hasSlide)
        {
            GameObject spawnedCasing = Instantiate(casing);
            spawnedCasing.transform.position = casingEjectPosition.position;
            spawnedCasing.transform.rotation = casingEjectPosition.rotation;

            Rigidbody casingRb = spawnedCasing.GetComponent<Rigidbody>();
            casingRb.AddForce(casingEjectPosition.right * 1000 * Time.deltaTime);
            Destroy(spawnedCasing, 10);
            ammoCapacity--;
        }
        if(slideGrab.isGrabbing)
        {
            audioSource.PlayOneShot(slideSound);
        }
    }

    public void ReleaseMagazine()
    {
        audioSource.PlayOneShot(gunLoad);
        animatedMagazine.SetActive(false);
        GameObject spawnedMagazine = Instantiate(magazine, animatedMagazine.transform.position, animatedMagazine.transform.rotation);
        spawnedMagazine.GetComponent<GunMagazine>().ammoCapacity = ammoCapacity;
        ammoCapacity = 0;
        if(spawnedMagazine.GetComponent<GunMagazine>().ammoCapacity > 0)
        {
            spawnedMagazine.GetComponent<GunMagazine>().ammoCapacity--;
            ammoCapacity++;
        }
    }
    public IEnumerator Delay()
    {
        yield return new WaitForSeconds(1);
        isInGun = false;
    }
}

