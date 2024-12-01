using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityUtils;

public class EndSceneController : MonoBehaviour
{
   [SerializeField] private Image background;
   [SerializeField] private TMP_Text endText;
   
   public void End()
   {
      CoroutineController.Start(EndGame());
   }
   
   private IEnumerator EndGame()
   {
      InputManager.Instance.DisableInput();
      
      yield return new WaitForSeconds(1f);
      background.gameObject.SetActive(true);
      background.DOFade(1, 2f);
      yield return new WaitForSeconds(2f);
      
      endText.gameObject.SetActive(true);
      endText.DOFade(1, 3f);
      yield return new WaitForSeconds(3f);
      
      yield return new WaitForSeconds(2f);
      
      GameManager.Instance.GoToMainMenu();
      InputManager.Instance.EnableInput();
   }
}
