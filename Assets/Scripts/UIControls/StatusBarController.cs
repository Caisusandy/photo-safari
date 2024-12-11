using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using Safari.Animals;

public class StatusBarController : MonoBehaviour
{
    public static StatusBarController instance;

    public TextMeshProUGUI butterflyCountText;
    public TextMeshProUGUI capybaraCountText;
    public TextMeshProUGUI frogCountText;
    public TextMeshProUGUI jaguarCountText;

    private void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
    }
}
