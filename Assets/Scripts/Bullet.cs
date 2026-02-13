using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public float lifetime = 5f;
    public float radius = 0.1f;

    private Vector3 direction;

    float t = 0;
    bool isInitialized = false;
    private Rigidbody rb;
    private bool destroyed = false;
    private Collider coll;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<Collider>();
    }

    public void Initialize(Vector3 position, Vector3 direction)
    {
        transform.position = position;
        this.direction = direction.normalized;
        isInitialized = true;
        rb.linearVelocity = this.direction * speed;
    }

    private void Update()
    {
        if (!isInitialized || destroyed)
            return;

        t += Time.deltaTime;

        if (t > lifetime)
        {
            destroyed = true;
            rb.isKinematic = true;
            rb.Sleep();
            coll.enabled = false;
            Destroy(gameObject, .25f);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.TryGetComponent(out Target target))
        {
            target.Hit();
        }
        else
        {
            Target target1 = other.collider.GetComponentInParent<Target>();
            if (target1 != null)
            {
                target1.Hit();
            }
        }

        Destroy(gameObject);
    }
}
