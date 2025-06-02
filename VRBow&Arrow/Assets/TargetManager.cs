using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class TargetManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> Targets;
    [SerializeField] private GameObject stopTimeButton;

    // Estado interno
    private int remainingTargets;
    private bool challengeStart = false;

    void Start()
    {
        if (Targets == null || Targets.Count == 0)
        {
            enabled = false;
            return;
        }

        if (stopTimeButton == null)
        {
            enabled = false;
            return;
        }
        
        IniciarDesafio();
    }

    public void IniciarDesafio()
    {
        remainingTargets = 0;

        foreach (GameObject dianaGO in Targets)
        {
            if (dianaGO != null)
            {
                Dianas dianaScript = dianaGO.GetComponent<Dianas>();
                if (dianaScript != null)
                {
                    dianaScript.InicializarDiana(this); // Pasa la referencia de este gestor
                    remainingTargets++;
                }
                else
                {
                    Debug.LogWarning($"El GameObject {dianaGO.name} en la lista de dianas no tiene el script DianaInteractiva.");
                    dianaGO.SetActive(false); // Desactivar si no es una diana válida para este sistema
                }
            }
        }

        if (remainingTargets == 0)
        {
            challengeStart = false;
            return;
        }
        
        stopTimeButton.SetActive(false);
    }
    
    public void RegistrarDianaGolpeada(GameObject dianaQueFueGolpeada)
    {
        if (!challengeStart) return;

        remainingTargets--;
        Debug.Log($"Diana golpeada. Restantes: {remainingTargets}");

        if (remainingTargets <= 0)
        {
            CompletarDesafio();
        }
    }

    private void CompletarDesafio()
    {
        challengeStart = false;
        
        if (stopTimeButton != null)
        {
            stopTimeButton.SetActive(true);
        }
    }
    
    public void ReiniciarDesafio()
    {
        IniciarDesafio();
    }
}
