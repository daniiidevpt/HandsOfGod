using UnityEngine;

public class TodoList : MonoBehaviour
{
    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 800, 30), "Make villagers smarter with utility AI so if player grabs villager he executes a closest task, something the player might want");
        GUI.Label(new Rect(10, 20, 800, 30), "Make village building process, so player can create new villagers");
    }
}
