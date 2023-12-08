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
    public Transform leftTriggerFinger;
    public InputActionProperty fireInputSourceRight;
    public Transform rightTriggerFinger;
    public InputActionProperty magReleaseInputSourceRight;
    public InputActionProperty magReleaseInputSourceLeft;
    public int ammoCapacity;
    private bool isInGun;
    public Transform bulletFirePosition;
    public Transform recoilAngle;
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
    private bool gunFired = false;
    private float pitch;
    private bool triggerHeld = false;
    private void Start()
    {
        pitch = audioSource.pitch;
        slideRetracted = true;
        slideGrab = slide.GetComponent<XRGrabJoint>();
    }
    public void FireBullet()
    {
        if(ammoCapacity > 0 && slideRetracted)
        {
            gunFired = true;
            audioSource.pitch = pitch;
            audioSource.PlayOneShot(bulletFire);
            fireParticles.Play();

            GameObject spawnedBullet = Instantiate(bullet);
            spawnedBullet.transform.position = bulletFirePosition.position;
            spawnedBullet.transform.rotation = bulletFirePosition.rotation;

            GameObject recoilBullet = Instantiate(bullet);
            recoilBullet.transform.position = recoilAngle.position;
            recoilBullet.transform.rotation = recoilAngle.rotation;
            recoilBullet.GetComponent<PlayEffectOnShot>().effect = null;
            recoilBullet.GetComponent<AudioSource>().enabled = false;

            Physics.IgnoreCollision(spawnedBullet.GetComponent<Collider>(), recoilBullet.GetComponent<Collider>());

            Rigidbody bulletRb = spawnedBullet.GetComponent<Rigidbody>();
            bulletRb.AddForce(bulletFirePosition.forward * bulletSpeed * Time.deltaTime);

            Rigidbody recoilRb = recoilBullet.GetComponent<Rigidbody>();
            recoilRb.AddForce(recoilAngle.forward * recoilSpeed);

            GameObject spawnedCasing = Instantiate(casing);
            spawnedCasing.transform.position = casingEjectPosition.position;
            spawnedCasing.transform.rotation = casingEjectPosition.rotation;

            Rigidbody casingRb = spawnedCasing.GetComponent<Rigidbody>();
            casingRb.AddForce(casingEjectPosition.right * 1000 * Time.deltaTime);
            Destroy(spawnedCasing, 10);

            StartCoroutine(FireSlideForce());
            ammoCapacity--;
            Destroy(recoilBullet, 1);
            timeSinceLastShot = 0f;
        }
        if(slideRetracted == false)
        {
            SlideRetractTrigger();
        }
    }
    public IEnumerator FireSlideForce()
    {
        slide.GetComponent<ConfigurableJoint>().targetPosition *= -1;
        yield return new WaitForSeconds(0.05f);
        slide.GetComponent<ConfigurableJoint>().targetPosition *= -1;
        gunFired = false;
        if(ammoCapacity == 0)
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
            if (fireButton > 0 && !triggerHeld)
            {
                StartCoroutine(LerpRotation(50, leftTriggerFinger));
                triggerHeld = true;
            }
            if (fireButton <= 0 && triggerHeld)
            {
                StartCoroutine(LerpRotation(-50, leftTriggerFinger));
                triggerHeld = false;
            }
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
    public IEnumerator LerpRotation(float angle, Transform rotateObject)
    {
        rotateObject.localRotation = Quaternion.Euler(new Vector3(rotateObject.localRotation.eulerAngles.x + angle, rotateObject.localRotation.eulerAngles.y, rotateObject.localRotation.eulerAngles.z));
        yield return null;
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
        else if(ammoCapacity > 0 && hasSlide && !gunFired)
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
            if (!audioSource.isPlaying)
            {
                audioSource.volume = 0.5f;
                audioSource.PlayOneShot(slideSound);
            }
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

