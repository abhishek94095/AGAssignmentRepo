using System;
using System.Collections;
using UnityEngine;

public class MemoryGameController : Singleton<MemoryGameController>
{
    [SerializeField] private float checkDelay = 0.1f; // wait a bit then check

    public static Action<bool> onTurnCompleted;
    private CardView firstCard;
    private CardView secondCard;
    private bool isBusy;

    public override void Awake()
    {
        base.Awake();
    }

    // called from grid when a card is created
    public void RegisterCard(CardView card)
    {
        // who can flip
        card.CanFlipCallback = CanFlipCard;

        // when flip animation finishes
        card.OnFlipCompleted += HandleCardFlipCompleted;
    }

    public void ResetState()
    {
        firstCard = null;
        secondCard = null;
        isBusy = false;
    }

    private bool CanFlipCard(CardView card)
    {
        if (isBusy) return false;          // currently resolving a pair
        if (card.IsFrontShowing) return false; // already face-up
        return true;
    }

    private void HandleCardFlipCompleted(CardView card)
    {
        // we only care when a card ends face-up
        if (!card.IsFrontShowing)
            return;

        if (firstCard == null)
        {
            firstCard = card;
        }
        else if (secondCard == null && card != firstCard)
        {
            secondCard = card;
            StartCoroutine(CheckMatchCoroutine());
        }
    }

    private IEnumerator CheckMatchCoroutine()
    {
        isBusy = true;

        // let the flip animation finish and show for a moment
        yield return new WaitForSeconds(checkDelay);

        if (firstCard != null && secondCard != null)
        {
            if (firstCard.CardId == secondCard.CardId)
            {
                // MATCH -> destroy both
                var a = firstCard.gameObject;
                var b = secondCard.gameObject;

                firstCard = null;
                secondCard = null;

                Destroy(a);
                Destroy(b);
            }
            else
            {
                // MISMATCH -> flip them back
                var a = firstCard;
                var b = secondCard;

                firstCard = null;
                secondCard = null;

                a.Flip();
                b.Flip();
            }
        }

        isBusy = false;
        onTurnCompleted?.Invoke(firstCard.CardId == secondCard.CardId);
    }
}
