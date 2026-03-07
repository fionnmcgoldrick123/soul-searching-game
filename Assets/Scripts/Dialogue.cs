using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue", menuName = "Scriptable Objects/Dialogue")]
public class Dialogue : ScriptableObject
{
    public string npcName;
    // public Sprite npcPortrait;
    public DialogueData[] dialogueData;
    public float typingSpeed = 0.05f;
    // public AudioClip voiceSound;
    // public float voicePitch = 1f;
}
