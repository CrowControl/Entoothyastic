﻿using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class Tooth : MonoBehaviour
{
    [SerializeField] private string _text;
    [SerializeField] private Color _matchedTextColor = Color.red;

    [SerializeField] private int _requiredRepetitions = 1;

    [SerializeField] private bool _disableOnSuccess = true;

    [SerializeField] private TMP_Text _textField;

    private int _characterIndex;

    private int _repetitions = 0;

    public string Text
    {
        get => _text;
        set
        {
            _text = value; 
            Reset();
        }
    }

    public bool TextActive
    {
        set => _textField.gameObject.SetActive(value);
    }

    [FormerlySerializedAs("OnInputSuccess")]
    public Event Matched;
    public Event RepetitionsCompleted;

    private void OnEnable()
    {
        TextActive = true;
        Reset();
    }

    private void OnDisable()
    {
        TextActive = false;
    }

    private void Reset()
    {
        ResetRepetitions();
        ResetMatching();
    }

    private void Update()
    {
        if (!Input.anyKeyDown || string.IsNullOrEmpty(Input.inputString))
            return;

        char input = Input.inputString.First();
        MatchCharacter(input);
    }

    private void UpdateText()
    {
        string matched = this.Text.Substring(0, _characterIndex);
        string notMatched = this.Text.Substring(_characterIndex);

        string colored = TMPHelper.WrapColor(matched, _matchedTextColor);

        _textField.text = colored + notMatched;
    }

    private void MatchCharacter(char input)
    {
        if (input.EqualsIgnoreCase(this.Text[_characterIndex]))
            OnCharacterMatched();
        else
            ResetMatching();
    }

    private void ResetMatching()
    {
        _characterIndex = 0;
        _textField.text = this.Text;

        this.enabled = string.IsNullOrEmpty(this.Text) == false;
    }

    private void ResetRepetitions()
    {
        _repetitions = 0;
    }

    private void OnCharacterMatched()
    {
        _characterIndex++;

        UpdateText();

        if (_characterIndex >= Text.Length)
            OnMatched();
    }

    private void OnMatched()
    {
        _repetitions++;
        Matched?.Invoke(this);

        if (_repetitions < _requiredRepetitions)
            ResetMatching();
        else
            OnRepetitionsCompleted();
    }

    private void OnRepetitionsCompleted()
    {
        RepetitionsCompleted?.Invoke(this);

        if (_disableOnSuccess)
            enabled = false;
    }

    [Serializable]
    public class Event : UnityEvent<Tooth> { }
}
