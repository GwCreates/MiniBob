using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class CharacterInfoTab : MonoBehaviour
{
    private Button button;
    public RectTransform dropdownImage;
    public GameObject[] GameObjects;
    
    private PlayerInput controls;
    
    void Awake()
    {
        controls = new PlayerInput();

        controls.Player.CharacterInfoToggle.performed += context => TriggerCharacterInfo();
        
    }
    
    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent < Button>();
        button.onClick.AddListener(TriggerCharacterInfo);
    }

    void TriggerCharacterInfo()
    {
        foreach (var obj in GameObjects)
        {
            obj.SetActive(!obj.activeSelf);
        }

        dropdownImage.localScale = new Vector3(dropdownImage.localScale.x * -1, dropdownImage.localScale.y, dropdownImage.localScale.z);
    }

    protected virtual void OnEnable()
    {
        controls.Enable();
    }

    protected virtual void OnDisable()
    {
        controls.Disable();
    }
}
