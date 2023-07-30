using UnityEngine;

public class Arrow : MonoBehaviour
{
    Transform from;
    Transform to;

    public GameObject ExplosionOBJ;

    public void HideArrow()
    {
        transform.position = new Vector2(-1000, -1000);
        from = null;
        to = null;
    }

    public void Pop()
    {
        var newobj = Instantiate(ExplosionOBJ);
        newobj.transform.position = transform.position;
    }

    void Update()
    {
        if (from != null && to != null)
        {
            transform.position = Vector2.MoveTowards(from.position, to.position, HexMetrics.outerRadius);
            transform.rotation = Quaternion.LookRotation(Vector3.forward, to.position - from.position);
        }
    }

    public void UpdateArrow(Transform pointFrom, Transform pointTo)
    {
        from = pointFrom;
        to = pointTo;
    }
}
