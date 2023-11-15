using UnityEngine;
using TMPro;
public class DialogueUI : MonoBehaviour
{
   [SerializeField] private TMP_Text textLabel;

   private void Start()
   {
        GetComponent<TypewriterEffect>().Run("Welcome to the game!\nHello.", textLabel);
   }
}
