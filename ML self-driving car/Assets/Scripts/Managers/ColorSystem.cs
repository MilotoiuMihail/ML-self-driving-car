using UnityEngine;

public class ColorSystem : MonoBehaviour
{
    [SerializeField] private ColorPalette _colorPalette;
    public static ColorPalette ColorPalette { get; private set; }

    private void Awake()
    {
        ColorPalette = _colorPalette;
    }
    public static bool IsClear(Color32 color)
    {
        return color.r == 0 && color.g == 0 && color.b == 0 && color.a == 0;
    }
}
