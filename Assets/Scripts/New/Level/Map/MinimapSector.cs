using UnityEngine;
using UnityEngine.UI;

public class MinimapSector : MonoBehaviour
{
    public Image fog;

    [Header("Unique ID sektora")]
    public string sectorID;

    private bool discovered = false;

    private void Awake()
    {
        LoadState();
    }

    public void Reveal()
    {
        if (discovered)
            return;

        discovered = true;

        if (fog != null)
        {
            fog.enabled = false;
        }

        // ZAPIS
        PlayerPrefs.SetInt("MapSector_" + sectorID, 1);
        PlayerPrefs.Save();

        Debug.Log("Odkryto sektor: " + sectorID);
    }

    private void LoadState()
    {
        int saved = PlayerPrefs.GetInt("MapSector_" + sectorID, 0);

        if (saved == 1)
        {
            discovered = true;

            if (fog != null)
            {
                fog.enabled = false;
            }
        }
    }
}