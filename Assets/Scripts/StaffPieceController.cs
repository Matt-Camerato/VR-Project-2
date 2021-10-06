using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class StaffPieceController : MonoBehaviour
{
    [SerializeField] private float rotSpeed;
    [SerializeField] private AudioClip shimmeringSFX;

    private void Start()
    {
        StartShimmeringSFX();
    }

    private void Update()
    {
        if (!GetComponent<XRGrabInteractable>().isSelected)
        {
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            float rotation = rotSpeed * Time.deltaTime;
            transform.Rotate(3 * rotation, -3 * rotation, -4 * rotation);

            if(!Physics.Raycast(transform.position, Vector3.down, 0.5f))
            {
                transform.position += Vector3.down * 0.3f * Time.deltaTime;
            }
        }
    }

    public void StartShimmeringSFX()
    {
        GetComponent<AudioSource>().loop = true;
        GetComponent<AudioSource>().clip = shimmeringSFX;
        GetComponent<AudioSource>().Play();
    }
}
