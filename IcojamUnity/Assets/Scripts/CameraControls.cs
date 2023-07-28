using UnityEngine;

public class CameraControls : MonoBehaviour
{
    // Start is called before the first frame update
    public float TranslationSpeed;
    public float ScrollSpeed;
    Camera camera;
    float targetSize = 5f;
    void Start()
    {
        camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        var pos = transform.position;
        pos.x += Input.GetAxis("Horizontal") * TranslationSpeed * Time.deltaTime;
        pos.y += Input.GetAxis("Vertical") * TranslationSpeed * Time.deltaTime;

        targetSize -= Input.mouseScrollDelta.y * ScrollSpeed;

        if (targetSize <= 0)
            targetSize = 1;

        camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, targetSize, 0.04f);

        transform.position = pos;
    }
}
