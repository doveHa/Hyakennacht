using UnityEngine;

[DefaultExecutionOrder(50)] 
public class FacingSyncLocalScale : MonoBehaviour
{
    public FacingProvider2D facing;   
    public bool affectChildren = false; 

    void Awake()
    {
        if (!facing) facing = GetComponent<FacingProvider2D>();
        if (!facing) facing = GetComponentInParent<FacingProvider2D>();
    }

    void LateUpdate()
    {
        if (!facing) return;

        var s = transform.localScale;
        float abs = Mathf.Abs(s.x);
        float sign = facing.FaceSign();      // -1(¿Þ), +1(¿À)
        transform.localScale = new Vector3(abs * sign, s.y, s.z);

        if (!affectChildren && TryGetComponent<SpriteRenderer>(out var sr))
            sr.flipX = false;
    }
}
