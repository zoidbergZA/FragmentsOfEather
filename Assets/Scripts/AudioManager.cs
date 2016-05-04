using UnityEngine;
using System.Collections;
using FMOD.Studio;

public class AudioManager : MonoBehaviour
{

    [SerializeField] string eventName;
    string path = "event:/";
    string FXpath = "event:/FX/";
    private FMOD.Studio.EventInstance backGroundMusic;
    private FMOD.Studio.ParameterInstance enemiesAreComing;
    private FMOD.Studio.ParameterInstance enemiesAnnouncing;
    private FMOD.Studio.ParameterInstance transition1;
    private FMOD.Studio.ParameterInstance transition2;

    void Start()
    {
        eventName = path + eventName;
        backGroundMusic = FMOD_StudioSystem.instance.GetEvent(eventName);

        backGroundMusic.getParameter("Enemies are coming", out enemiesAreComing);
        backGroundMusic.getParameter("Enemies announcing", out enemiesAnnouncing);
        backGroundMusic.getParameter("Transition_1", out transition1);
        backGroundMusic.getParameter("Transition_2", out transition2);
        startBGMusic();
    }

    public void enterBattle()
    {
        enemiesAreComing.setValue(0.95f);
    }

    public void leaveBattle()
    {
        enemiesAreComing.setValue(0.05f);
    }

    public void enterEnemyAnnouncing()
    {
        enemiesAnnouncing.setValue(0.95f);
    }

    public void leaveEnemyAnnouncing()
    {
        enemiesAnnouncing.setValue(0.05f);
    }

    public void EnterEraOne()
    {
        transition1.setValue(0.95f);
    }

    public void EnterEraTwo()
    {
        transition2.setValue(0.95f);
    }

    private void startBGMusic()
    {
        backGroundMusic.start();
        backGroundMusic.setVolume(0.5f);
    }

    public void stopBGMusic()
    {
        backGroundMusic.stop(STOP_MODE.ALLOWFADEOUT);
    }

    public void playSFX(string sfxName)
    {
        FMOD_StudioSystem.instance.PlayOneShot(FXpath + sfxName, transform.position);
    }
}
