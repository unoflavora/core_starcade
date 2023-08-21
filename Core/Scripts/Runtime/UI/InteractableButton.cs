using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Agate.Starcade;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class InteractableButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private List<MaskableGraphic> _graphics;
    [SerializeField] private ButtonNotificationPopup _notification;
    [SerializeField] private string errorMessage;
    private Button _button;
    private bool _errorIsShown;
    
    void Start()
    {
        _button = GetComponent<Button>();
    }

    void Update()
    {
        foreach (var graphic in _graphics)
        {
            graphic.color = _button.IsInteractable() ? _button.colors.normalColor : _button.colors.disabledColor;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_button.interactable) return;
        
        if (_notification != null) _notification.DisplayError(errorMessage);
    }
}
