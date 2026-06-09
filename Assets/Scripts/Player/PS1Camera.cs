using UnityEngine;

public class PS1Camera : MonoBehaviour
{
    public float positionSnap = 0.02f;
    public float rotationSnap = 0.1f;
    public float jitterAmount = 0.02f;

    void LateUpdate()
    {
        // jitter (PS1 unstable output)
        transform.position += new Vector3(
            Random.Range(-jitterAmount, jitterAmount),
            Random.Range(-jitterAmount, jitterAmount),
            0
        );

        // vertex snapping (low precision movement)
        Vector3 p = transform.position;
        p.x = Mathf.Round(p.x / positionSnap) * positionSnap;
        p.y = Mathf.Round(p.y / positionSnap) * positionSnap;
        p.z = Mathf.Round(p.z / positionSnap) * positionSnap;
        transform.position = p;

        Vector3 r = transform.eulerAngles;
        r.x = Mathf.Round(r.x / rotationSnap) * rotationSnap;
        r.y = Mathf.Round(r.y / rotationSnap) * rotationSnap;
        r.z = Mathf.Round(r.z / rotationSnap) * rotationSnap;
        transform.eulerAngles = r;
    }
}