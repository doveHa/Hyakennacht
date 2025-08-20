using UnityEngine;

public class ButtonTest : MonoBehaviour
{
    public void Select(string camp)
    {
        if (camp.Equals("witch"))
        {
            Debug.Log("마녀");
        }else if (camp.Equals("yokai"))
        {
            Debug.Log("요괴");
        }
    }

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }
}