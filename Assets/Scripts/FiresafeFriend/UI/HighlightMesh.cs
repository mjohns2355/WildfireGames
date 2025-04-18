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

    private List<Tween> activeTweens = new List<Tween>();
    private List<Material> panelMaterials = new List<Material>();
    private void Start()
    {
        
    }
    public void HighlightMeshes()
    {
        foreach(var meshRender in meshRenders)
        {
           
            Material panelMaterial = new Material(meshRender.material);
            panelMaterials.Add(panelMaterial);
            meshRender.material = panelMaterial;
            panelMaterial.EnableKeyword("_EMISSION");

            panelMaterial.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;

            panelMaterial.SetColor("_EmissionColor", highlightColor * baseIntensity);

            Tween tween = panelMaterial.DOColor(highlightColor * highlightIntensity, "_EmissionColor", flashDuration)
                .SetLoops(flashCount * 2, LoopType.Yoyo)
                .SetEase(Ease.InOutSine)
                 .OnComplete(() =>
                 {
                     panelMaterial.DisableKeyword("_EMISSION");

                 });
            activeTweens.Add(tween);
        }
        
        
    }

    public void StopHighlight()
    {
        foreach (var panelMaterial in panelMaterials)
        {

            panelMaterial.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;

            panelMaterial.SetColor("_EmissionColor", highlightColor * baseIntensity);

            panelMaterial.DisableKeyword("_EMISSION");

        }
    }

    private void OnDestroy()
    {
        Debug.Log("On destroy");
        Debug.Log($"Active tweens: {activeTweens.Count}");
        StopHighlight();
        foreach (var meshRender in meshRenders)
        {
            foreach (var tween in activeTweens)
            {
                tween.Kill();
            }
            activeTweens.Clear();
        }
    }
}
