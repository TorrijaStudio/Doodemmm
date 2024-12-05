using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace HelloWorld
{
    public class UIManager : MonoBehaviour
    {
        private string _joinCode = "";
        private const int maxConnections = 3;
        private bool isHost;

        public GameObject campoTexto;
        [SerializeField] public TMP_InputField joinCodeField;

        public GameObject fondo;
        public GameObject cuadro;

        public GameObject botonHost;
        public GameObject botonClient;
        
        public GameObject botonMenu;
        
        //
        public GameObject objetosEscena;

        public GameObject coinsManager;
        //
        
        
        
        async void Start()
        {
            campoTexto = GameObject.Find("CODE");
            joinCodeField = campoTexto.GetComponent<TMP_InputField>();
            
            
            fondo = GameObject.Find("jungla");
            cuadro = GameObject.Find("CARTEL_GRANDE");
            
            botonHost = GameObject.Find("ButtonHost");
            botonClient = GameObject.Find("ButtonClient");
            
            botonMenu = GameObject.Find("menu");
            botonMenu.SetActive(false);
            
            //

            //
            

            await UnityServices.InitializeAsync();

            AuthenticationService.Instance.SignedIn +=
                () => print($"New player {AuthenticationService.Instance.PlayerId} connected");

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        //-----------------------## ESTO SOBRA ##-------------------------------
        #region ESTO SOBRA

        void OnGUI()
        {
            //GUILayout.BeginArea(new Rect(10, 10, 300, 300));
            if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
            {
                StartButtons();
            }
            else
            {
                StatusLabels();
            }

            //GUILayout.EndArea();
        }

        void StartButtons()
        {
            //if (GUILayout.Button("Host")) StartHost();
            //if (GUILayout.Button("Client")) StartClient(_joinCode);
            //_joinCode = GUILayout.TextField(_joinCode);
            //if (GUILayout.Button("Paste code and join"))
            //{
                //StartClient(GUIUtility.systemCopyBuffer);
            //}
        }

        void StatusLabels()
        {
            var mode = NetworkManager.Singleton.IsHost ? "Host" :
                NetworkManager.Singleton.IsServer ? "Server" : "Client";

            GUILayout.Label("Transport: " +
                            NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name);
            GUILayout.Label("Mode: " + mode);
            GUILayout.Label(isHost ? "Host" : "Client");

            GUILayout.Label("Room: " + _joinCode);
            if(GUILayout.Button("Copy code")) GUIUtility.systemCopyBuffer = _joinCode;
        }
        
        private async void StartClient(string joinCodeS)
        {
            try
            {
                await UnityServices.InitializeAsync();
                if (!AuthenticationService.Instance.IsSignedIn)
                {
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                }

                var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode: joinCodeS);
                NetworkManager.Singleton.GetComponent<UnityTransport>()
                    .SetRelayServerData(new RelayServerData(joinAllocation, "wss"));
                NetworkManager.Singleton.StartClient();
            }
            catch (RelayServiceException e)
            {
                print(e);
            }
        }
        #endregion
        //TODO: SEIKAN NECESITAS CREAR UN TEXTFIELD Y ASIGNARLO AL QUE HE CREADO!!
        //TODO: PUEDES CREAR UNA FUNCION QUE SE LLAME DESDE STARTHOST Y START CLIENT para quitar esta interfaz de menu
        //TODO: SEIKAN ESTO GUARDA EL JOINCODE EN _joinCode !!!
        public async void StartHost()
        {
            isHost = true;
            try
            {
                await UnityServices.InitializeAsync();
                if (!AuthenticationService.Instance.IsSignedIn)
                {
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                }

                Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
                NetworkManager.Singleton.GetComponent<UnityTransport>()
                    .SetRelayServerData(new RelayServerData(allocation, "wss"));
                _joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
                
                fondo.SetActive(false);
                cuadro.SetActive(false);
                
                campoTexto.SetActive(false);
                botonHost.SetActive(false);
                botonClient.SetActive(false);
                
                botonMenu.SetActive(true);
                
                //
                objetosEscena.SetActive(true);
                //
        
                GUIUtility.systemCopyBuffer = _joinCode;
                NetworkManager.Singleton.StartHost();
            }
            catch (RelayServiceException e)
            {
                print(e);
            }
        }
        
        //TODO: SEIKAN AQUI EMPIEZAS EL CLIENTE !!
        public async void StartClient()
        {
            var jc = joinCodeField.text;
            try
            {
                await UnityServices.InitializeAsync();
                if (!AuthenticationService.Instance.IsSignedIn)
                {
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                }

                var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode: jc);
                NetworkManager.Singleton.GetComponent<UnityTransport>()
                    .SetRelayServerData(new RelayServerData(joinAllocation, "wss"));
                NetworkManager.Singleton.StartClient();
                
                fondo.SetActive(false);
                cuadro.SetActive(false);
                
                campoTexto.SetActive(false);
                botonHost.SetActive(false);
                botonClient.SetActive(false);
                
                botonMenu.SetActive(true);
                
                //
                objetosEscena.SetActive(true);
                //
       

            }
            catch (RelayServiceException e)
            {
                print(e);
            }
        }
        
        
    }
}