using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaffAudioController : MonoBehaviour
{
    [SerializeField] private AudioClip jumpSFX;
    [SerializeField] private AudioClip spellFiredSFX;

    public void JumpSFX()
    {
        GetComponent<AudioSource>().pitch = Random.Range(0.8f, 1.2f);
        GetComponent<AudioSource>().PlayOneShot(jumpSFX);
    }

    public void SpellFiredSFX()
    {
        GetComponent<AudioSource>().pitch = Random.Range(0.8f, 1.2f);
        GetComponent<AudioSource>().PlayOneShot(spellFiredSFX);
    }
}
