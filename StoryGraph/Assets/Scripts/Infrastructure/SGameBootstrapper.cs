using UnityEngine;

namespace Infrastructure
{
    public class SGameBootstrapper : MonoBehaviour
    {
        [SerializeField] private GameBootstrapper gameBootstrapper;

        private void Awake()
        {
            GameBootstrapper boot = FindObjectOfType<GameBootstrapper>();

            if (boot == null)
                Instantiate(gameBootstrapper);
        }
    }
}