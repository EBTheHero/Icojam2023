using UnityEngine;

public class CallParentOnClick : MonoBehaviour
{
    private void OnMouseDown()
    {
        transform.parent.SendMessage("OnMouseDown", null, SendMessageOptions.DontRequireReceiver);
    }
}
