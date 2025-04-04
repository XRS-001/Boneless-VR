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
    public Transform leftTriggerFinger;
    public Transform leftInitialRotation;
    public InputActionProperty fireInputSourceRight;
    public Transform rightTriggerFinger;
    public Transform rightInitialRotation;
    public int maxCapacity;
    public int ammoCapacity;
    public Transform[] bulletFirePositions;
    public Transform recoilAngle;
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
    private float pitch;
    private void Start()
    {
        pitch = audioSource.pitch;
        shotgunSlide = slide.GetComponent<ShotgunSlide>();
    }
    public void FireBullet()
    {
        if (ammoCapacity > 0 && canFire && shotgunSlide.wasReachedEnd && hasSlide)
        {
            GameObject recoilBullet = Instantiate(bullet);
            recoilBullet.transform.position = recoilAngle.position;
            recoilBullet.transform.rotation = recoilAngle.rotation;
            recoilBullet.GetComponent<PlayEffectOnShot>().effect = null;
            recoilBullet.GetComponent<AudioSource>().enabled = false;
            foreach (Transform bulletFirePosition in bulletFirePositions)
            {
                GameObject spawnedBullet = Instantiate(bullet);
                spawnedBullet.transform.position = bulletFirePosition.position;
                spawnedBullet.transform.rotation = bulletFirePosition.rotation;

                Physics.IgnoreCollision(spawnedBullet.GetComponent<Collider>(), recoilBullet.GetComponent<Collider>());

                Rigidbody bulletRb = spawnedBullet.GetComponent<Rigidbody>();
                bulletRb.AddForce(bulletFirePosition.forward * bulletSpeed * Time.deltaTime);
            }
            audioSource.pitch = pitch;
            audioSource.PlayOneShot(bulletFire, 2);
            fireParticles.Play();

            Rigidbody recoilRb = recoilBullet.GetComponent<Rigidbody>();
            recoilRb.AddForce(recoilAngle.forward * recoilSpeed);

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
            rightTriggerFinger.localRotation = Quaternion.Euler(new Vector3(rightInitialRotation.localRotation.eulerAngles.x + (50 * fireButton), rightTriggerFinger.localRotation.eulerAngles.y, rightTriggerFinger.localRotation.eulerAngles.z));
        }
        else if (grabInteractable.leftHandGrabbing)
        {
            fireButton = fireInputSourceLeft.action.ReadValue<float>();
            leftTriggerFinger.localRotation = Quaternion.Euler(new Vector3(leftInitialRotation.localRotation.eulerAngles.x + (50 * fireButton), leftTriggerFinger.localRotation.eulerAngles.y, leftTriggerFinger.localRotation.eulerAngles.z));
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
        gunShell = other.gameObject;
        if (gunShell.gameObject.CompareTag("Magazine") && gunShell.GetComponent<ShotgunShell>())
        {
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
        if (!audioSource.isPlaying)
        {
            audioSource.volume = 0.5f;
            audioSource.PlayOneShot(slideSound);
        }
        if (!hasSlide)
        {
            if (ammoCapacity > 0)
            {
                hasSlide = true;
                GameObject spawnedCasing = Instantiate(casing);
                spawnedCasing.transform.position = casingEjectPosition.position;
                spawnedCasing.transform.rotation = casingEjectPosition.rotation;

                Rigidbody casingRb = spawnedCasing.GetComponent<Rigidbody>();
                casingRb.AddForce(casingEjectPosition.right * 1000 * Time.deltaTime);
                Destroy(spawnedCasing, 10);
            }
        }
    }
    public void Lock()
    {
        if(hasSlide && ammoCapacity > 0)
        {
            JointDrive zDrive = slide.GetComponent<ConfigurableJoint>().zDrive;
            zDrive.positionSpring = float.PositiveInfinity;
            slide.GetComponent<ConfigurableJoint>().zDrive = zDrive;
        }
    }
}
