using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player2_5DController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Transform model;

    private Rigidbody rb;
    private Vector3 input;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        input = new Vector3(h, 0, v).normalized;

        if (input != Vector3.zero)
            model.forward = input; // make the model face the direction
    }

    void FixedUpdate()
    {
        Vector3 move = input * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + move);
    }
}
