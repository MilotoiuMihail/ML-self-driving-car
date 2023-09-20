using UnityEngine;

[CreateAssetMenu(fileName = "NewColorPalette", menuName = "Custom/Color Palette")]
public class ColorPalette : ScriptableObject
{
    [SerializeField] private Color32 _default;
    [SerializeField] private Color32 _danger;
    [SerializeField] private Color32 _warning;
    [SerializeField] private Color32 _info;
    [SerializeField] private Color32 _success;

    public Color32 Default => _default;
    public Color32 Danger => _danger;
    public Color32 Warning => _warning;
    public Color32 Info => _info;
    public Color32 Success => _success;
}

