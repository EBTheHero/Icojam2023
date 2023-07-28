using UnityEngine;

public class CellSpriteColliderCall : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnMouseDown()
    {
        transform.parent.SendMessage("OnMouseDown", null, SendMessageOptions.DontRequireReceiver);
    }
}
