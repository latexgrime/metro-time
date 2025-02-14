using UnityEngine;

public class Aim : MonoBehaviour
{
    public GameObject cameraTwo;
    private int counter = 1;
    
    // Update is called once per frame
    void Update()
    {
        ZoomIn();
    }

    private void ZoomIn()
    {
        if (Input.GetMouseButtonDown(1))
        {
            counter++;
        }

        if (counter % 2 == 0)
        {
            cameraTwo.SetActive(true);
        }
        else
        {
            cameraTwo.SetActive(false);

        }
        
    }
}
