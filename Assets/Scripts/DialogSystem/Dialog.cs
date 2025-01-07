using System.Collections.Generic;
using UnityEngine;

public class Dialog : MonoBehaviour
{
    [SerializeField] List<string> localizationKeys;

    public List<string> LocalizationKeys
    {
        get
        {
            return
                localizationKeys;
        }
    }
}
