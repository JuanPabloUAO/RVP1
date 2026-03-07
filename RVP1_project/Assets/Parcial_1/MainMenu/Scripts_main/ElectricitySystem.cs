using UnityEngine;
using System.Collections;

public class ElectricitySystem : MonoBehaviour
{
    public Material lightMaterial;
    public Material darkMaterial;

    public AudioSource shortCircuitSound;

    public Transform cameraTransform;
    public float cameraShakeIntensity = 0.05f;

    Renderer rend;
    Vector3 originalCameraPos;

    void Start()
    {
        rend = GetComponent<Renderer>();

        if (cameraTransform != null)
            originalCameraPos = cameraTransform.localPosition;

        StartCoroutine(ElectricLoop());
    }

    IEnumerator ElectricLoop()
    {
        while (true)
        {
            // Luz normal
            rend.material = lightMaterial;
            yield return new WaitForSeconds(Random.Range(5f, 7f));

            // Sonido de corto
            if (shortCircuitSound != null)
                shortCircuitSound.Play();

            // Parpadeo eléctrico
            for (int i = 0; i < Random.Range(4, 7); i++)
            {
                rend.material = darkMaterial;
                ShakeCamera();

                yield return new WaitForSeconds(Random.Range(0.05f, 0.15f));

                rend.material = lightMaterial;
                yield return new WaitForSeconds(Random.Range(0.05f, 0.15f));
            }

            // Apagón total
            rend.material = darkMaterial;

            yield return new WaitForSeconds(Random.Range(5f, 7f));

            ResetCamera();
        }
    }

    void ShakeCamera()
    {
        if (cameraTransform == null) return;

        cameraTransform.localPosition =
            originalCameraPos + Random.insideUnitSphere * cameraShakeIntensity;
    }

    void ResetCamera()
    {
        if (cameraTransform == null) return;

        cameraTransform.localPosition = originalCameraPos;
    }
}