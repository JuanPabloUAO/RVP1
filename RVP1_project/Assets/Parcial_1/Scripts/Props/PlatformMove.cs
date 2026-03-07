using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMove : MonoBehaviour
{
    public float speed = 2f;

    public bool activatePlatform = false;

    private float waitTime;

    public Transform[] moveSpots;

    public float startWaitTime = 2f;

    private int i = 0;

    void Start()
    {
        waitTime = startWaitTime;
    }

    void Update()
    {
        // Si el botón no está activado la plataforma no se mueve
        if (!activatePlatform) return;

        transform.position = Vector2.MoveTowards(
            transform.position,
            moveSpots[i].position,
            speed * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, moveSpots[i].position) < 0.1f)
        {
            if (waitTime <= 0)
            {
                if (i < moveSpots.Length - 1)
                {
                    i++;
                }
                else
                {
                    i = 0;
                }

                waitTime = startWaitTime;
            }
            else
            {
                waitTime -= Time.deltaTime;
            }
        }
    }

    // Hace que el jugador se mueva con la plataforma
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            collision.collider.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            collision.collider.transform.SetParent(null);
        }
    }
}