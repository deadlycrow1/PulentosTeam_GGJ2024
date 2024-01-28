using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Mainmenu : MonoBehaviour
{
	public AudioSource equipoMusica;
	public AudioClip sonidoMenu;
	public AudioClip sonidoIntro;

	void Start()
	{
		//equipoMusica = GetComponent<AudioSource>();
	}

	public void Intro()
	{

		SceneManager.LoadScene("GAMEPLAY");
	//	SceneManager.LoadScene("Intro");
		//Destroy(SoundManager);
		//equipoMusica.PlayOneShot(sonidoIntro);
	}
	public void PlayGame()
	{
		SceneManager.LoadScene("GAMEPLAY");
		//equipoMusica.PlayOneShot(sonidoLvl01);
	}
	public void Creditos()
	{
		SceneManager.LoadScene("Creditos");
	}

	public void QuitGame()
	{
		Debug.Log("QUIT !");
		Application.Quit();
	}

	public void GoToMenu()
	{
		SceneManager.LoadScene("Menu");
	}
	
	void Update()
    {
    	if (Input.GetKeyDown(KeyCode.Return))
        {
            GoToMenu();
        }
    }
}