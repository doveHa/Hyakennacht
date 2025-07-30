using Character;
using UnityEngine;

public class AnimationEvent : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StopRoll()
    {
        Movement.MoveSpeed /= 2;
    }
}
