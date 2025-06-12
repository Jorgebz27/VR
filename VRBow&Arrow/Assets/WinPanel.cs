using UnityEngine;

public class WinPanel : MonoBehaviour
{
    public GameObject panel;

    public void Victory()
    {
        panel.SetActive(true);
    }
}
