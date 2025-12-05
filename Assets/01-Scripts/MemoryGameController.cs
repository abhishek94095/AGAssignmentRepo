using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryGameController : Singleton<MemoryGameController>
{
    public static Action<bool> TurnCompleted;
    public static Action GameCompleted;

    private readonly List<CardView> FlippedCards = new List<CardView>();

    public override void Awake()
    {
        base.Awake();
    }

    public void RegisterCard(CardView cardView)
    {
        cardView.CanFlipCallback = CanFlipCard;
        cardView.FlipCompletedCallback += HandleCardFlipCompleted;
    }

    public void ResetState()
    {
        FlippedCards.Clear();
    }

    private bool CanFlipCard(CardView cardView)
    {
        return !cardView.IsFrontShowing;
    }

    private void HandleCardFlipCompleted(CardView cardView)
    {
        FlippedCards.Add(cardView);
        StartCoroutine(ResolveCards());
    }

    private IEnumerator ResolveCards()
    {
        yield return new WaitForSeconds(0.15f);

        if (FlippedCards.Count < 2)
        {
            yield break;
        }

        while (FlippedCards.Count >= 2)
        {
            CardView firstCard = FlippedCards[0];
            CardView secondCard = FlippedCards[1];

            bool cardsMatch = firstCard.CardId == secondCard.CardId;

            if (cardsMatch)
            {
                GameSaveManager.MarkCardDestroyed(firstCard.CardIndex);
                GameSaveManager.MarkCardDestroyed(secondCard.CardIndex);

                FlippedCards.Remove(firstCard);
                FlippedCards.Remove(secondCard);

                Destroy(firstCard.gameObject);
                Destroy(secondCard.gameObject);

                TurnCompleted?.Invoke(true);
                SoundManager.Instance?.Play(SoundType.Match);

                yield return new WaitForEndOfFrame();

                if (HasGameFinished())
                {
                    SoundManager.Instance?.Play(SoundType.GameOver);
                    GameCompleted?.Invoke();
                }
            }
            else
            {
                firstCard.Flip(true);
                secondCard.Flip(true);

                FlippedCards.Remove(firstCard);
                FlippedCards.Remove(secondCard);

                TurnCompleted?.Invoke(false);
                SoundManager.Instance?.Play(SoundType.Mismatch);

                yield return new WaitForSeconds(0.3f);
            }

            yield return null;
        }
    }

    private bool HasGameFinished()
    {
        CardView[] remainingCards = FindObjectsOfType<CardView>();
        return remainingCards.Length == 0;
    }
}
