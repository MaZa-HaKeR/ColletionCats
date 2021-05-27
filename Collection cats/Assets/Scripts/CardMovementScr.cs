 using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;


public class CardMovementScr : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public CardController CC;
    Camera MainCamera;
    Vector3 offset;
    public Transform DefaultParent, DefaultTempCardParent;
    GameObject TempCardGO;
    public bool IsDragable;
    int startID;

    void Awake()
    {
        MainCamera = Camera.allCameras[0];
        TempCardGO = GameObject.Find("TempCardGO");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        offset = transform.position - MainCamera.ScreenToWorldPoint(eventData.position);

        DefaultParent = DefaultTempCardParent = transform.parent;

        IsDragable = GameManagerScr.Instance.IsPlayerTurn &&
                    (
                        (DefaultParent.GetComponent<DropPlaceScr>().Type==FieldType.SELF_HAND &&
                        GameManagerScr.Instance.CurrentGame.Player.Mana >= CC.Card.Manacost) ||
                        (DefaultParent.GetComponent<DropPlaceScr>().Type == FieldType.SELF_FIELD &&
                        CC.Card.CanAttack)
                    );
        if (!IsDragable)
            return;

        startID = transform.GetSiblingIndex();

        if(CC.Card.IsSpell || CC.Card.CanAttack)
            GameManagerScr.Instance.HighlightTargets(CC, true);

        TempCardGO.transform.SetParent(DefaultParent);
        TempCardGO.transform.SetSiblingIndex(transform.GetSiblingIndex());

        transform.SetParent(DefaultParent.parent);
        GetComponent<CanvasGroup>().blocksRaycasts = false;

    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!IsDragable)
            return;

        Vector3 newPos = MainCamera.ScreenToWorldPoint(eventData.position);
        transform.position = newPos + offset;

        if (!CC.Card.IsSpell)
        {
            if (TempCardGO.transform.parent != DefaultTempCardParent)
                TempCardGO.transform.SetParent(DefaultTempCardParent);
        
            if(DefaultParent.GetComponent<DropPlaceScr>().Type != FieldType.SELF_FIELD)
                CheckPosition();
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!IsDragable)
            return;


        GameManagerScr.Instance.HighlightTargets(CC, false);
        

        transform.SetParent(DefaultParent);

        GetComponent<CanvasGroup>().blocksRaycasts = true;

        transform.SetSiblingIndex(TempCardGO.transform.GetSiblingIndex());

        TempCardGO.transform.SetParent(GameObject.Find("BG").transform);
        TempCardGO.transform.localPosition = new Vector3(3708, -557);


    }

    void CheckPosition()
    {
        int newIndex = DefaultTempCardParent.childCount;

        for (int i = 0; i < DefaultTempCardParent.childCount; i++)
        {
            if (transform.position.x < DefaultTempCardParent.GetChild(i).position.x)
            {
                newIndex = i;

                if (TempCardGO.transform.GetSiblingIndex() < newIndex)
                    newIndex--;

                break;
            }
        }
        
        if(TempCardGO.transform.parent == DefaultParent)
            newIndex = startID;
        
        TempCardGO.transform.SetSiblingIndex(newIndex);
    }

    public void MoveToField(Transform field)
    {
        transform.SetParent(GameObject.Find("BG").transform);
        transform.DOMove(field.position, .5f);
    }

    public void MoveToTarget(CardController card, Transform target)
    {
        StartCoroutine(MoveToTargetCor(card, target));
    }

    IEnumerator MoveToTargetCor(CardController card, Transform target)
    {
        Vector3 pos = transform.position;
        Transform parent = transform.parent;
        int index = transform.GetSiblingIndex();

        if (transform.parent.GetComponent<HorizontalLayoutGroup>())
            transform.parent.GetComponent<HorizontalLayoutGroup>().enabled = false;

        transform.SetParent(GameObject.Find("BG").transform);

        transform.DOMove(target.position, .25f);

        yield return new WaitForSeconds(.25f);

        if (!card.Card.IsSpell)
            transform.DOMove(pos, .25f);
        
        yield return new WaitForSeconds(.25f);

        transform.SetParent(parent);
        transform.SetSiblingIndex(index);
        
        if (transform.parent.GetComponent<HorizontalLayoutGroup>())
            transform.parent.GetComponent<HorizontalLayoutGroup>().enabled = true;
    }
}