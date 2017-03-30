using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MovieHandler : MonoBehaviour {

    public MovieTexture movie;
    public AudioSource audio;
    public AudioClip a_clip;

	// Use this for initialization
	void Start () {
        GetComponent<RawImage>().texture = movie as MovieTexture;
        audio = GetComponent<AudioSource>();
        audio.clip = a_clip;
        movie.Play();
        audio.Play();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
