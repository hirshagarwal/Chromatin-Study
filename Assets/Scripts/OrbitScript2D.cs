using UnityEngine;

[AddComponentMenu("Camera-Control/Mouse Orbit with zoom")]
public class OrbitScript2D : MonoBehaviour
{
    public Transform target;
    public float distance = 5.0f;
    public float xSpeed = 120.0f;
    public float ySpeed = 120.0f;

    public float yMinLimit = -20f;
    public float yMaxLimit = 80f;

    public float distanceMin = -.5f;
    public float distanceMax = -15f;

    private Rigidbody rigidbody;

    private float x = 0.0f;
    private float y = 0.0f;

    // Use this for initialization
    private void Start()
    {
    }

    private void LateUpdate()
    {
        if (target)
        {
            if (Input.GetMouseButton(0))
            {
                x += Input.GetAxis("Mouse X") * 0.2f;// * xSpeed * 0.02f;
                y += Input.GetAxis("Mouse Y") * 0.2f;// * ySpeed * 0.02f;
            }

            Vector3 negDistance = new Vector3(-x, -y, -distance);
            Vector3 position = negDistance + target.position;

            //transform.rotation = rotation;
            transform.position = position;
        }
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}