using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class ShotgunFire : MonoBehaviour
{
    public GameObject slide;
    private ShotgunSlide shotgunSlide;
    public ParticleSystem fireParticles;
    public AudioSource audioSource;
    public AudioClip bulletFire;
    public AudioClip gunLoad;
    public AudioClip slideSound;
    public XRGrabInteractableShotgun grabInteractable;
    public InputActionProperty fireInputSourceLeft;
    public InputActionProperty fireInputSourceRight;
    public int maxCapacity;
    public int ammoCapacity;
    public Transform[] bulletFirePositions;
    public Transform casingEjectPosition;
    public string shellName;
    public GameObject animatedShell;
    private GameObject gunShell;
    private bool hasSlide = false;
    public GameObject bullet;
    public GameObject casing;
    public Animator animator;
    public float bulletSpeed;
    public float recoilSpeed;
    private float fireButton;
    private bool triggerReleased = true;

    public float fireCooldown = 0f;
    private float timeSinceLastShot = 0f;
    private bool canFire = true;
    private void Start()
    {
        shotgunSlide = slide.GetComponent<ShotgunSlide>();
    }
    public void FireBullet()
    {
        if (ammoCapacity > 0 && canFire && shotgunSlide.wasReachedEnd && hasSlide)
        {
            GameObject recoilBullet = Instantiate(bullet);
            recoilBullet.transform.position = bulletFirePositions[0].position;
            recoilBullet.transform.rotation = bulletFirePositions[0].rotation;
            recoilBullet.GetComponent<PlayParticleEffectOnCollision>().effect = null;
            recoilBullet.GetComponent<AudioSource>().enabled = false;
            foreach (Transform bulletFirePosition in bulletFirePositions)
            {
                GameObject spawnedBullet = Instantiate(bullet);
                spawnedBullet.transform.position = bulletFirePosition.position;
                spawnedBullet.transform.rotation = bulletFirePosition.rotation;

                Physics.IgnoreCollision(spawnedBullet.GetComponent<Collider>(), recoilBullet.GetComponent<Collider>());

                Rigidbody bulletRb = spawnedBullet.GetComponent<Rigidbody>();
                bulletRb.AddForce(-bulletFirePosition.up * bulletSpeed * Time.deltaTime);
                Destroy(spawnedBullet, 5);
            }
            audioSource.PlayOneShot(bulletFire);
            fireParticles.Play();

            Rigidbody recoilRb = recoilBullet.GetComponent<Rigidbody>();
            recoilRb.AddForce(bulletFirePositions[0].up * recoilSpeed);

            Destroy(recoilBullet, 1);

            timeSinceLastShot = 0f;
            ammoCapacity--;
            hasSlide = false;
            JointDrive zDrive = slide.GetComponent<ConfigurableJoint>().zDrive;
            zDrive.positionSpring = 0;
            slide.GetComponent<ConfigurableJoint>().zDrive = zDrive;
        }
        else if(ammoCapacity == 0)
        {
            audioSource.PlayOneShot(slideSound);
        }
    }
    private void Update()
    {
        timeSinceLastShot += Time.deltaTime;

        if (grabInteractable.rightHandGrabbing)
        {
            fireButton = fireInputSourceRight.action.ReadValue<float>();
        }
        else if (grabInteractable.leftHandGrabbing)
        {
            fireButton = fireInputSourceLeft.action.ReadValue<float>();
        }
        if (fireButton > 0.1f && triggerReleased && timeSinceLastShot >= fireCooldown)
        {
            FireBullet();
            triggerReleased = false;
        }
        if (fireButton < 0.1f)
        {
            triggerReleased = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Magazine"))
        {
            gunShell = other.gameObject;
            if (gunShell.GetComponent<ShotgunShell>().shellName == shellName && other.gameObject.GetComponent<XRGrabInteractableTwoAttach>().isSelected && shotgunSlide.wasReached && ammoCapacity < maxCapacity)
            {
                audioSource.PlayOneShot(gunLoad);
                ammoCapacity += gunShell.GetComponent<ShotgunShell>().ammoCapacity;
                animatedShell.SetActive(true);
                gunShell.GetComponent<XRGrabInteractableTwoAttach>().controllerGrabbing.allowSelect = false;
                gunShell.GetComponent<XRGrabInteractableTwoAttach>().controllerGrabbing.allowSelect = true;
                Destroy(gunShell.gameObject);
                animator.Play("Reload");
                hasSlide = true;
            }
        }
    }
    public void Slide()
    {
        audioSource.PlayOneShot(slideSound);
        if (!hasSlide)
        {
            if (ammoCapacity > 0)
            {
                hasSlide = true;
                GameObject spawnedCasing = Instantiate(casing);
                spawnedCasing.transform.position = casingEjectPosition.position;
                spawnedCasing.transform.rotation = casingEjectPosition.rotation;

                Rigidbody casingRb = spawnedCasing.GetComponent<Rigidbody>();
                casingRb.AddForce(casingEjectPosition.right * 10000 * Time.deltaTime);
                Destroy(spawnedCasing, 10);
            }
        }
        else if (ammoCapacity > 0 && hasSlide)
        {
            hasSlide = true;
            GameObject spawnedCasing = Instantiate(casing);
            spawnedCasing.transform.position = casingEjectPosition.position;
            spawnedCasing.transform.rotation = casingEjectPosition.rotation;

            Rigidbody casingRb = spawnedCasing.GetComponent<Rigidbody>();
            casingRb.AddForce(casingEjectPosition.right * 10000 * Time.deltaTime);
            Destroy(spawnedCasing, 10);
            ammoCapacity--;
        }
    }
    public void Lock()
    {
        if(hasSlide)
        {
            JointDrive zDrive = slide.GetComponent<ConfigurableJoint>().zDrive;
            zDrive.positionSpring = float.PositiveInfinity;
            slide.GetComponent<ConfigurableJoint>().zDrive = zDrive;
        }
    }
}
