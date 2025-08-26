using UnityEngine;

public class MapUIManager : MonoBehaviour
{
    private button PlayBtn;
    private button Card;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPlayBtnClick()
    {
        Debug.Log("Play button clicked!");
        // Add logic to start the game or load the next scene
    }

    public void OnCardClick()
    {
        Debug.Log("Card clicked!");
        // Add logic to show card details or perform an action
    }
}
