using UnityEngine;

public class PixelCamera : MonoBehaviour
{
    public Camera cam;
    public int width = 320;
    public int height = 180;

    void OnPreCull()
    {
        cam.rect = new Rect(0, 0, 1, 1);
    }

    void OnPostRender()
    {
        GL.Clear(true, true, Color.black);
    }
}