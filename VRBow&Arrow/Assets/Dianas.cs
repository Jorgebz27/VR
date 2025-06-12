using UnityEngine;

public class Dianas : MonoBehaviour
{
    public TargetManager manager;
    private bool hasHit = false;
    
    public void InicializarDiana(TargetManager manager)
    {
        this.manager = manager;
        this.hasHit = false;
        this.gameObject.SetActive(true);
    }
    
    public void ProcesarGolpe()
    {
        if (hasHit || manager == null || !this.gameObject.activeInHierarchy)
        {
            return;
        }

        hasHit = true;
        Debug.Log($"Diana {gameObject.name} ha sido golpeada.");
        
        manager.RegistrarDianaGolpeada(this.gameObject);
        gameObject.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (hasHit || !this.gameObject.activeSelf) return;

        if (other.gameObject.CompareTag("Arrow"))
        {
            Debug.Log($"Colisión de Diana {gameObject.name} con Flecha {other.gameObject.name}");
            ProcesarGolpe();
        }
    }
}
