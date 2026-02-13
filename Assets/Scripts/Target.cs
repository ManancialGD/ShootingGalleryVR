using UnityEngine;
public class Target : MonoBehaviour
{
    public AudioClip ActivateSound;
    public AudioSource audioSource;

    void OnEnable()
    {
        audioSource.PlayOneShot(ActivateSound);
    }

    public void Hit()
    {
        GameMaster.Instance.HitTarget(this);
    }
}
