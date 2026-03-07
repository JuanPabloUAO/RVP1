using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trampoline : MonoBehaviour
{
    public Animator animator;
    public float jumpForce = 2f;

    public AudioSource audioSource;
    public AudioClip sonidoTrampolin;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<Rigidbody2D>().linearVelocity = Vector2.up * jumpForce;

            animator.Play("JumpTrampoline");

            if (sonidoTrampolin != null)
            {
                audioSource.PlayOneShot(sonidoTrampolin);
            }
        }
    }
}