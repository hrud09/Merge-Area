using UnityEngine;

public class CarDrive : MonoBehaviour
{
    Rigidbody rb;
    public float forwardSpeed = 10f;

    Vector3 lastMousePos;
    public float xRange = 4f;
    private Vector3 direction;
    public bool GameStart = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }



    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameStart = true;
            lastMousePos = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0))
        {
            direction = lastMousePos - Input.mousePosition;
        }
        Vector3 pos = transform.position;
        pos.x = transform.position.x + direction.x;
        pos.x = Mathf.Clamp(transform.position.x, -xRange, xRange);
        transform.position = Vector3.Lerp(transform.position,pos,0.5f);

        lastMousePos = Input.mousePosition;
    }

    private void FixedUpdate()
    {
        if (GameStart)
            Move();
        else
        {
            rb.velocity = Vector3.zero;
            rb.Sleep();
        }
    }

    private void Move()
    {
        rb.velocity = Vector3.forward * forwardSpeed;

    }

}
