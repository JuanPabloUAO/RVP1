using UnityEngine;

public class ButtonTrigger : MonoBehaviour
{
    public PlatformMove[] platforms;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            foreach (PlatformMove platform in platforms)
            {
                platform.activatePlatform = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            foreach (PlatformMove platform in platforms)
            {
                platform.activatePlatform = false;
            }
        }
    }
}