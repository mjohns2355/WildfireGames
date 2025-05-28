using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FF_Tree : FF_Plants
{
    public MeshRenderer burntModel;
    public AudioSource audioSource;
    public GameObject fireParticle;

    [SerializeField]private AudioClip chop1, chop2, fall;
    private MeshRenderer burntMesh;
    protected override void Start()
    {
        base.Start();
        audioSource = GetComponent<AudioSource>();
        OnBurning += HandleBurning;
        chop1 = ResourceManager.Instance.chop1;
        chop2 = ResourceManager.Instance.chop2;
        fall = ResourceManager.Instance.fall;
        fireParticle.SetActive(false);
    }



    public override void OnCombustibleClicked(GameObject obj)
    {
        if (obj == gameObject && isClickable)
        {
            //Debug.Log($"Clicked {gameObject.name}");
            HH_GameManager.Instance.uiManager.purchasePopup.confirmRemove.onClick.AddListener(RemoveTree);
            HH_GameManager.Instance.uiManager.ShowPurchasePopup(null, false,true);
        }
    }

    private void HandleBurning()
    {
        if (burntModel && burntMesh == null)
        {
            foreach (var mesh in meshes)
            {
               
                burntMesh = Instantiate(burntModel, transform);
                burntMesh.transform.position = mesh.transform.position;
                burntMesh.material = mesh.material;
                mesh.gameObject.SetActive(false);

            }
        }
    }

    protected override void HandleIgnite()
    {
        base.HandleIgnite();
        Debug.Log("Tree is ignited");
        fireParticle.SetActive(true);
    }

    protected override IEnumerator IgniteWithDelay()
    {
        if (isOnFire) yield break;
        yield return new WaitForSeconds(durability / 10 + baseBurnTime);
        isOnFire = true;
        burnTimer = 50f;
        //OnIgnite?.Invoke();
        StartCoroutine(Burn());
    }
    private void RemoveTree()
    {
        if (HH_GameManager.Instance.currentPlayer.budgetManager.SpendBudget(5000))
        {
            PlaySFXSequence();
            StartCoroutine(PlantClickedRoutine());
            
            HH_GameManager.Instance.uiManager.HidePurchasePopup(null,false);
        }
        else
        {
            HH_GameManager.Instance.uiManager.ShowPurchasePopup(null, true);
        }
        
    }

    public void PlaySFXSequence()
    {
        Sequence sfxSequence = DOTween.Sequence().SetLink(gameObject);

        sfxSequence.AppendCallback(() =>
        {
            audioSource.PlayOneShot(chop1);
        });
        sfxSequence.AppendInterval(chop1.length);

        sfxSequence.AppendCallback(() =>
        {
            audioSource.PlayOneShot(chop2);
        });
        sfxSequence.AppendInterval(chop2.length);

        sfxSequence.AppendCallback(() =>
        {
            audioSource.PlayOneShot(fall);
        });
    }
}
