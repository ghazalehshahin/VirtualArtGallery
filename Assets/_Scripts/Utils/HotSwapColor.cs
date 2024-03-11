using UnityEngine;

public class HotSwapColor : MonoBehaviour
{
    [SerializeField] private Color color;
    [SerializeField] private bool hasEmission;
    [SerializeField] private Color emissionColor;
    [SerializeField] private float emissionIntensity;
    [SerializeField] private MeshRenderer mr;
    [SerializeField] private SkinnedMeshRenderer smr;
    
    private MaterialPropertyBlock mpb;
    private static readonly int ShaderProp = Shader.PropertyToID("_Color");
    private static readonly int EmissionShaderProp = Shader.PropertyToID("_EmissionColor");

    private MaterialPropertyBlock Mpb => mpb ??= new MaterialPropertyBlock();

    private void OnEnable()
    {
        ApplyColor();
    }

    private void OnValidate()
    {
        ApplyColor();
    }

    private void ApplyColor()
    {
        Mpb.SetColor(ShaderProp, color);
        Color finalCol = emissionColor*emissionIntensity;
        // Remember to set the emission property to enabled in the material, and the value of emission hsv to 1
        Mpb.SetColor(EmissionShaderProp, hasEmission ? finalCol : Color.black);
        if(mr != null){
            mr.SetPropertyBlock(Mpb);
        }
        if(smr != null){
            smr.SetPropertyBlock(Mpb);
        }
    }
}