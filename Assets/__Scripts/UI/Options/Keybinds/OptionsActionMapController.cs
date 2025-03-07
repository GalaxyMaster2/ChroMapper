﻿using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;

public class OptionsActionMapController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private OptionsInputActionController keybindPrefab;
    [SerializeField] private VerticalLayoutGroup layoutGroup;

    private InputActionMap actionMap;
    private bool hasInit = false;

    public void Init(string name, InputActionMap map)
    {
        if (hasInit) return;
        title.text = name;
        actionMap = map;
        foreach (InputAction action in actionMap.actions)
        {
            if (action.name.StartsWith("+")) continue; //Filter keybinds that should not be modified (Designated with + prefix)
            if (action.bindings.Any(x => x.isComposite))
            {
                string compositeName = action.bindings.First(x => x.isComposite).name;
                bool useCompositeName = action.bindings.Count(x => x.isComposite) > 1;
                List<InputBinding> bindings = new List<InputBinding>();
                for (int i = 0; i < action.bindings.Count; i++)
                {
                    if (action.bindings[i].isComposite && bindings.Any())
                    {
                        //Spawn a copy of the keybind object, and init them with input action data.
                        OptionsInputActionController keybind = Instantiate(keybindPrefab.gameObject, transform)
                            .GetComponent<OptionsInputActionController>();
                        keybind.Init(action, bindings, compositeName, useCompositeName);
                        
                        bindings.Clear();
                        compositeName = action.bindings[i].name;
                    }
                    else if (action.bindings[i].isPartOfComposite)
                    {
                        bindings.Add(action.bindings[i]);
                    }
                }
                OptionsInputActionController lastKeybind = Instantiate(keybindPrefab.gameObject, transform)
                    .GetComponent<OptionsInputActionController>();
                lastKeybind.Init(action, bindings, compositeName, useCompositeName);
            }
            else
            {
                //Spawn a copy of the keybind object, and init them with input action data.
                OptionsInputActionController keybind = Instantiate(keybindPrefab.gameObject, transform)
                    .GetComponent<OptionsInputActionController>();
                keybind.Init(action, action.bindings.ToList());
            }
        }
        keybindPrefab.gameObject.SetActive(false);
        layoutGroup.spacing = layoutGroup.spacing;
        hasInit = true;
    }
}
