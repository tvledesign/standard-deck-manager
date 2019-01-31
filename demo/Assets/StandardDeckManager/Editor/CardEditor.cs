﻿using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// CardEditor
/// Description: Custom editor to edit our cards in our Deck Manager.
/// </summary>

public class CardEditor : EditorWindow
{
    // reference this window
    public static CardEditor Instance { get; private set; }

    // reference to the DeckManager
    private static DeckManagerEditor deckManagerEditor;

    // check if this window is open or not
    public static bool IsOpen
    {
        get { return Instance != null; }
    }

    // create a variable to hold card information
    public int intCardIndex;
    public bool blnEditingCardFromDeck;
    public bool blnEditingCardFromDiscard;
    public bool blnEditingCardFromInUse;

    // on initialization
    [MenuItem("Standard Deck Manager/Card Editor")]
    private static void Init()
    {
        // get existing open window or if none, make a new one
        CardEditor window = (CardEditor)EditorWindow.GetWindow(typeof(CardEditor), false, "Card Editor");
        window.minSize = new Vector2(325, 140);
        window.Show();

        // set the reference to the current inspected object
        deckManagerEditor = DeckManagerEditor.Instance;
    }

    // when this is enabled and active
    private void OnEnable()
    {
        // set this instance
        Instance = this;

        // set the reference to the current inspected object
        deckManagerEditor = DeckManagerEditor.Instance;
    }

    // repaint the inspector if it gets updated
    private void OnInspectorUpdate()
    {
        Repaint();
    }

    // draw our fields onto the inspector
    private void DrawFields(List<Card> deck)
    {
        // try to draw our fields and if we can't turn them off
        try
        {
            // allow the user to edit the current selected card property
            if(!Application.isPlaying)
                Undo.RecordObjects(deckManagerEditor.targets, "Edit card properties.");
            deck[intCardIndex].color = (Card.Color)EditorGUILayout.EnumPopup("Color", deck[intCardIndex].color);
            deck[intCardIndex].rank = (Card.Rank)EditorGUILayout.EnumPopup("Rank", deck[intCardIndex].rank);
            deck[intCardIndex].suit = (Card.Suit)EditorGUILayout.EnumPopup("Suit", deck[intCardIndex].suit);
            deck[intCardIndex].value = EditorGUILayout.IntField("Value", deck[intCardIndex].value);
            deck[intCardIndex].card = (GameObject)EditorGUILayout.ObjectField("Card", deck[intCardIndex].card, typeof(GameObject), true);
        } catch
        {
            blnEditingCardFromDeck = false;
            blnEditingCardFromDiscard = false;
            blnEditingCardFromInUse = false;
            GUIUtility.ExitGUI();
        }
    }

    // draw the ui
    private void OnGUI()
    {
        // header styles
        GUIStyle styleRowHeader = new GUIStyle
        {
            padding = new RectOffset(0, 0, 3, 3)
        };
        styleRowHeader.normal.background = EditorStyle.SetBackground(1, 1, new Color(0.1f, 0.1f, 0.1f, 0.2f));

        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal(styleRowHeader);
        EditorGUILayout.LabelField("Edit Selected Card", EditorStyles.boldLabel);
        EditorGUILayout.EndHorizontal();

        // inform our user no card is selected;
        if (!blnEditingCardFromDeck && !blnEditingCardFromDiscard && !blnEditingCardFromInUse)
        {
            try
            {
                EditorGUILayout.HelpBox("There is no card currently selected. Please select a card from the Deck Manager to edit.", MessageType.Info);
            }
            catch
            {
                return;
            }
        }

        // if the deck manager editor is null 
        if (deckManagerEditor == null)
        {
            // try and find a reference to the deck manager editor
            if (DeckManagerEditor.Instance)
            {
                deckManagerEditor = DeckManagerEditor.Instance;

            }
            else
            {
                blnEditingCardFromDeck = false;
                blnEditingCardFromDiscard = false;
                blnEditingCardFromInUse = false;
                return;
            }
        }

        EditorGUILayout.Space();

        // if we are editing the card
        if (blnEditingCardFromDeck)
            DrawFields(deckManagerEditor.deckManager.deck);
        else if (blnEditingCardFromDiscard)
            DrawFields(deckManagerEditor.deckManager.discardPile);
        else if (blnEditingCardFromInUse)
            DrawFields(deckManagerEditor.deckManager.inUsePile);

        EditorGUILayout.Space();
    }
}