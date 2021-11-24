using System;
using UnityEngine;

using UnityEngine.UI;

namespace Scripts.Core.UI
{
    public class Promt : MonoBehaviour
    {
        [SerializeField] private Text _promt;
        private static Promt _instance;
        public static Promt Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<Promt>();
                    if (_instance == null)
                    {
                        throw new ArgumentNullException("there is no active GO with Promt");
                    }
                }
                return _instance;
            }
        }
        public static void PrintMessage(PromptType promptType)
        {
            switch (promptType)
            {
                case PromptType.None:
                    Instance._promt.text = "";
                    break;
                case PromptType.DrugIt:
                    Instance._promt.text = "Press Q for drug it";
                    break;
                case PromptType.DropIt:
                    Instance._promt.text = "Press Q for drop it";
                    break;
                case PromptType.TakeIt:
                    Instance._promt.text = "Press E for take it to inventory";
                    break;
                case PromptType.KickIt:
                    Instance._promt.text = "Press F for kick it";
                    break;
                default:
                    break;
            }
        }
    }

    public enum PromptType
    {
        None,
        DrugIt,
        DropIt,
        TakeIt,
        KickIt
    }
}
