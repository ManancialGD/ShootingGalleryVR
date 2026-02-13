using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Haptics;
using UnityEngine.InputSystem.XR;
using UnityEngine.VFX;
using UnityEngine.XR.OpenXR.Input;

public class VRRifle : MonoBehaviour
{
    public Transform recoilPoint;
    public float recoilAmount = 5f;
    public float recoilRecoverySpeed = 10f;
    public float shootCooldown = .15f;

    private float currentRecoil = 0f;

    public Bullet bulletPrefab;
    public Transform shootPoint;
    public VisualEffect muzzleFlashEffect;

    public Transform rightHand;
    public Transform leftHand;

    public Transform AttachmentPoint;
    public Transform SecondaryAttachmentPoint;

    public InputActionReference rightTriggerAction;
    public InputActionReference leftTriggerAction;
    public InputActionReference swapHandsAction;

    public AudioClip[] shootSound;
    private AudioSource source;

    [Range(0f, 1f)] public float triggerSensitivity = 0.45f;

    private Transform mainHand;
    private Transform auxHand;
    private float timeLastShot = 0f;

    public bool isRightHanded = true;

    private void Awake()
    {
        AssignHands();
        source = GetComponentInChildren<AudioSource>();
    }

    private void AssignHands()
    {
        if (isRightHanded)
        {
            mainHand = rightHand;
            auxHand = leftHand;
        }
        else
        {
            mainHand = leftHand;
            auxHand = rightHand;
        }
    }

    private void OnEnable()
    {
        rightTriggerAction.action.performed += OnRightTriggerPressed;
        leftTriggerAction.action.performed += OnLeftTriggerPressed;
        swapHandsAction.action.performed += SwapHandsPerformed;
    }

    private void OnDisable()
    {
        rightTriggerAction.action.performed -= OnRightTriggerPressed;
        leftTriggerAction.action.performed -= OnLeftTriggerPressed;
        swapHandsAction.action.performed -= SwapHandsPerformed;
    }

    private void OnRightTriggerPressed(InputAction.CallbackContext context)
    {
        if (isRightHanded && context.ReadValue<float>() >= triggerSensitivity)
        {
            Shoot();
        }
    }

    private void OnLeftTriggerPressed(InputAction.CallbackContext context)
    {
        if (!isRightHanded && context.ReadValue<float>() >= triggerSensitivity)
        {
            Shoot();
        }
    }

    private void SwapHandsPerformed(InputAction.CallbackContext context) { if (context.performed) { SwapHands(); } }

    private void Shoot()
    {
        if (Time.time - timeLastShot < shootCooldown) return;
        timeLastShot = Time.time;

        Bullet bullet = Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation);
        bullet.Initialize(shootPoint.position, shootPoint.forward);

        currentRecoil += recoilAmount;

        if (shootSound != null && shootSound.Length >= 1 && source != null)
            source.PlayOneShot(shootSound[Random.Range(0, shootSound.Length - 1)]);

        if (muzzleFlashEffect != null)
            muzzleFlashEffect.Play();
    }

    public void SwapHands()
    {
        isRightHanded = !isRightHanded;
        AssignHands();
    }

    private void Update()
    {
        UpdateWeaponPosition();

        // Smoothly recover recoil
        currentRecoil = Mathf.Lerp(currentRecoil, 0f, recoilRecoverySpeed * Time.deltaTime);

        // Apply rotation
        recoilPoint.localRotation = Quaternion.Euler(-currentRecoil, 0f, 0f);
    }

    private void UpdateWeaponPosition()
    {
        if (mainHand == null || auxHand == null) return;

        if (AttachmentPoint == null)
        {
            transform.position = mainHand.position;

            if (mainHand.position != auxHand.position)
            {
                transform.LookAt(auxHand.position);
            }
            return;
        }

        Vector3 desiredForward = (auxHand.position - mainHand.position).normalized;

        if (desiredForward == Vector3.zero)
        {
            desiredForward = transform.forward;
        }

        Vector3 desiredUp = mainHand.up;

        Quaternion targetRotation;

        if (SecondaryAttachmentPoint == null)
        {
            targetRotation = Quaternion.LookRotation(desiredForward, desiredUp);
        }
        else
        {
            Vector3 localAimDir = (SecondaryAttachmentPoint.localPosition - AttachmentPoint.localPosition).normalized;

            Vector3 localUpDir = Vector3.Cross(localAimDir, Vector3.right);
            if (localUpDir == Vector3.zero)
                localUpDir = Vector3.Cross(localAimDir, Vector3.forward);
            localUpDir.Normalize();

            Quaternion worldRotation = Quaternion.LookRotation(desiredForward, desiredUp);
            Quaternion localRotation = Quaternion.LookRotation(localAimDir, localUpDir);
            targetRotation = worldRotation * Quaternion.Inverse(localRotation);
        }

        Vector3 worldOffset = targetRotation * AttachmentPoint.localPosition;
        transform.position = mainHand.position - worldOffset;
        transform.rotation = targetRotation;
    }
}
