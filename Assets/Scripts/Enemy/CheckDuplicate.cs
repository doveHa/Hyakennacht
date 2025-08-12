using UnityEngine;

public class CheckDuplicate : MonoBehaviour
{
    public bool IsOverlap { get; private set; } = false;

    private void OnCollisionStay2D(Collision2D other)
    {
        Debug.Log("Duplicate");
        IsOverlap = true;
    }
}