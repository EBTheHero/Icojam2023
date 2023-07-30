using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioMenu : MonoBehaviour
{

    public AudioSource menuSound;
    public AudioClip hoversound;

    public AudioClip clickSOund;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
   public void Hoversound()
    {
        menuSound.PlayOneShot(hoversound);
    }

    public void ClickSound()
    {
        menuSound.PlayOneShot(clickSOund);
    }

}
