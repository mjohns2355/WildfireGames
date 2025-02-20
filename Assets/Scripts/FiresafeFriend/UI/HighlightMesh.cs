using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class HighlightMesh : MonoBehaviour
{
    public List<Renderer> meshRenders;
    public Color highlightColor;
    public int flashCount = 2;
    public float baseIntensity;
    public float highlightIntensity;
    public float flashDuration;
    public void HighlightMeshes()
    {
        foreach(var meshRender in meshRenders)
        {
            Material panelMaterial = meshRender.material;
            panelMaterial.EnableKeyword("_EMISSION");

            panelMaterial.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;

            panelMaterial.SetColor("_EmissionColor", highlightColor * baseIntensity);

            panelMaterial.DOColor(highlightColor * highlightIntensity, "_EmissionColor", flashDuration)
                .SetLoops(flashCount * 2, LoopType.Yoyo)
                .SetEase(Ease.InOutSine)
                 .OnComplete(() =>
                 {
                     panelMaterial.DisableKeyword("_EMISSION");
                 });
        }

    }
}
