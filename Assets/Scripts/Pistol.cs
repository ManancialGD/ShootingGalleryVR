using UnityEngine;
using UnityEngine.VFX;

public class Pistol : MonoBehaviour
{
    public Bullet bulletPrefab;
    public Transform firePoint;
    public Transform recoilPoint;
    public float recoilAmount = 5f;
    public float recoilRecoverySpeed = 10f;
    public VisualEffect muzzleFlashEffect;

    public AudioClip[] shootSound;
    private AudioSource source;

    private float currentRecoil = 0f;


    private void Start()
    {
        source = GetComponentInChildren<AudioSource>();
    }

    public void FireBullet()
    {
        Bullet bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        bullet.Initialize(firePoint.position, firePoint.forward);

        currentRecoil += recoilAmount;

        if (shootSound != null && shootSound.Length >= 1 && source != null)
            source.PlayOneShot(shootSound[Random.Range(0, shootSound.Length - 1)]);

        if (muzzleFlashEffect != null)
            muzzleFlashEffect.Play();
    }

    private void Update()
    {
        currentRecoil = Mathf.Lerp(currentRecoil, 0f, recoilRecoverySpeed * Time.deltaTime);

        recoilPoint.localRotation = Quaternion.Euler(-currentRecoil, 0f, 0f);
    }
}
