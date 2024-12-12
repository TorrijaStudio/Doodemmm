using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;


public class MenuhOJAS : MonoBehaviour
{

    [Header("Parameter Change")]

    [SerializeField] private string parameterName0;
    [SerializeField] private float parameterValue0;

    [SerializeField] private string parameterName1;
    [SerializeField] private float parameterValue1;

    public Button abajo; //señal Opciones
    public Button medio; //señal Tienda
    public Button arriba; //señal Jugar
    public Button back; //botón volver al menú ppal
    public Button host; //botón de host
    public Button client; //botón de cliente
    public TMP_InputField code; //cuadro para escirbir el codigo de sala
    public Button expansions; // compra de expansiones
    public Button skins; // tienda de cosmeticos
    public Button ad; // ver anunmcio para ganar recompensa
    public Button coinPurchase; // boton de + para comprar monedas (tienda dinero real)
    public Scrollbar general;
    public Scrollbar music;
    public Scrollbar sfx;

    public GameObject camara;

    public GameObject hojas1;
    private Animator anim1;
    
    public GameObject hojas2;
    private Animator anim2;
    
    public GameObject hojas3;
    private Animator anim3;
    
    public GameObject hojas4;
    private Animator anim4;
    
    public GameObject texto;
    private Animator animTexto;

    public GameObject parteAbajo;
    private Animator animAbajo;

    public GameObject parteMedia;
    private Animator animMedia;

    public GameObject parteArriba;
    private Animator animArriba;

    public GameObject torrijaLogo;
    private Animator animLogo;


    public GameObject senalAbajo;
    private Animator animSenalAbajo;


    public GameObject senalMedio;
    private Animator animSenalMedio;

    public GameObject senalArriba;
    private Animator animSenalArriba;

    public GameObject cartelDoodem;
    private Animator animCartelDoodem;

    public GameObject cartelMenus;
    private Animator animCartelMenus;
    private bool iniciado = false;



    /// COSAS ANUNCIO
    public GameObject anuncioUI;
    
    // COSAS EXPANSIONES
    public GameObject expansionesUI;
    
    // COSAS SKINS
    public GameObject skins_UI;
    
    // COSAS COMPRA MONEDAS
    public GameObject compraMonedasUI;
    
    
    // UI BASE DE LA TIENDA
    public GameObject shopBaseUI;
    
    // Flecha para volver al menu de la tienda
    public Button backShop;



    public GameObject coinManager;
    
    
    // Botones de compra de monedas
    public Button compra1;
    public Button compra2;
    public Button compra3;

    public GameObject paypalImage;

    public GameObject monedaTienda;
    
    public GameObject loadingScreen;


    public GameObject[] anuncios;

    private int anuncioElegido;
    // Start is called before the first frame update
   void Start()
    {
        AudioManager.PlayMusic(MusicType.MENU);
        anim1 = hojas1.GetComponent<Animator>();
        anim2 = hojas2.GetComponent<Animator>();
        anim3 = hojas3.GetComponent<Animator>();
        anim4 = hojas4.GetComponent<Animator>();
        
        animTexto = texto.GetComponent<Animator>();

        animAbajo = parteAbajo.GetComponent<Animator>();
        animMedia = parteMedia.GetComponent<Animator>();
        animArriba = parteArriba.GetComponent<Animator>();

        animLogo = torrijaLogo.GetComponent<Animator>();

        animSenalAbajo = senalAbajo.GetComponent<Animator>();
        animSenalMedio = senalMedio.GetComponent<Animator>();
        animSenalArriba = senalArriba.GetComponent<Animator>();
        animCartelDoodem = cartelDoodem.GetComponent<Animator>();
        animCartelMenus = cartelMenus.GetComponent<Animator>();

        backShop.gameObject.SetActive(false);
        coinPurchase.gameObject.SetActive(false);
        
        
        DesactivarBotonesSenales();
        DesactivarBotonesMenuJugar();
        DesactivarBotonesTienda();
        DesactivarBotonesOpciones();
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(0) && !iniciado)
        {
            iniciado = true;
            
            anim1.SetTrigger("Start");
            anim2.SetTrigger("Start");
            anim3.SetTrigger("Start");
            anim4.SetTrigger("Start");
            //AudioManager.instance.PlayOneShot(FMODEvents.instance.MoverHojas, camara.transform.position);
            //AudioManager.instance.SetMusicParameter(parameterName1, parameterValue1);
            AudioManager.PlaySound(SoundType.HOJAS);


            animTexto.SetTrigger("Start");

            Invoke("caerTotem", 1.29f);
            

            Invoke("aparecerLogo", 3.5f);
            Invoke("caerDoodem",4.2f);
            Invoke("ActivarBotonesSenales", 5.5f); 
            
        }

       
    }

    void caerTotem() {
        animAbajo.SetTrigger("Start");
        StartCoroutine(ReproducirCaidas());
        animMedia.SetTrigger("Start");
        animArriba.SetTrigger("Start");
        animSenalAbajo.SetTrigger("Start");
        animSenalMedio.SetTrigger("Start");
        animSenalArriba.SetTrigger("Start");
    }

    IEnumerator ReproducirCaidas()
    {
        yield return new WaitForSeconds(0.3f);
        AudioManager.PlaySound(SoundType.CAIDA_TOTEM);
        //AudioManager.instance.PlayOneShot(FMODEvents.instance.CaidaTotem, camara.transform.position);
        yield return new WaitForSeconds(0.4f);
        AudioManager.PlaySound(SoundType.CAIDA_TOTEM);
        //AudioManager.instance.PlayOneShot(FMODEvents.instance.CaidaTotem, camara.transform.position);
        yield return new WaitForSeconds(0.4f);
        AudioManager.PlaySound(SoundType.CAIDA_TOTEM);
        //AudioManager.instance.PlayOneShot(FMODEvents.instance.CaidaTotem, camara.transform.position);
        yield return new WaitForSeconds(0.3f);
        //AudioManager.instance.PlayOneShot(FMODEvents.instance.CaidaTotem, camara.transform.position);
    }

    void aparecerLogo()
    {

        //torrijaLogo.SetActive(true);
        animLogo.SetTrigger("Start");
        Debug.Log("saas");
    }


    public void PulsadoSenalAbajo(){
        Debug.Log("se ha pulsado la senal de abajo vale??");
        //AudioManager.instance.PlayOneShot(FMODEvents.instance.ClickMadera, camara.transform.position);
        AudioManager.PlaySound(SoundType.CLICK);

        monedaTienda.SetActive(false);
        
        StartCoroutine(ReproducirSenales());
        animCartelDoodem.SetTrigger("pulsar");
        animSenalAbajo.SetTrigger("Pulsado");
        animSenalMedio.SetTrigger("Pulsado");
        animSenalArriba.SetTrigger("Pulsado");
        animAbajo.SetTrigger("pulsar");
        animMedia.SetTrigger("pulsar");
        animArriba.SetTrigger("pulsar");
        animCartelMenus.SetTrigger("Start");

        torrijaLogo.SetActive(false);
        DesactivarBotonesSenales();
        
        Invoke("ActivarBotonesOpciones", 1.5f);    }
    IEnumerator ReproducirSenales()
    {
        //AudioManager.instance.PlayOneShot(FMODEvents.instance.CaidaCadenas, camara.transform.position);
        AudioManager.PlaySound(SoundType.CADENAS);
        yield return new WaitForSeconds(0.3f);
        ///AudioManager.instance.PlayOneShot(FMODEvents.instance.CaidaTotem, camara.transform.position);
        yield return new WaitForSeconds(0.4f);
        AudioManager.PlaySound(SoundType.CAIDA_TOTEM);
        //AudioManager.instance.PlayOneShot(FMODEvents.instance.CaidaCadenas, camara.transform.position);
        AudioManager.PlaySound(SoundType.CADENAS);
    }

    public void PulsadoSenalMedio(){
        Debug.Log("se ha pulsado la senal de abajo vale??");
        //AudioManager.instance.PlayOneShot(FMODEvents.instance.ClickMadera, camara.transform.position);
        AudioManager.PlaySound(SoundType.CLICK);

        StartCoroutine(ReproducirSenales());
        monedaTienda.SetActive(true);
        animCartelDoodem.SetTrigger("pulsar");
        animSenalAbajo.SetTrigger("Pulsado");
        animSenalMedio.SetTrigger("Pulsado");
        animSenalArriba.SetTrigger("Pulsado");
        animAbajo.SetTrigger("pulsar");
        animMedia.SetTrigger("pulsar");
        animArriba.SetTrigger("pulsar");
        animCartelMenus.SetTrigger("Start");
        torrijaLogo.SetActive(false);
        //animLogo.SetTrigger("Pulsado");
        DesactivarBotonesSenales();

        Invoke("ActivarBotonesTienda", 2f);
        //ActivarBotonesTienda();

    }

    public void PulsadoSenalArriba(){

        pantallaCarga();
        Invoke("comenzarJuego", 9f);
        
        
        
		//SceneManager.LoadScene("PrototypeMain");
       /* Debug.Log("se ha pulsado la senal de abajo vale??");
        //AudioManager.instance.PlayOneShot(FMODEvents.instance.ClickMadera, camara.transform.position);
        AudioManager.PlaySound(SoundType.CLICK);

        StartCoroutine(ReproducirSenales());
        animCartelDoodem.SetTrigger("pulsar");
        animSenalAbajo.SetTrigger("Pulsado");
        animSenalMedio.SetTrigger("Pulsado");
        animSenalArriba.SetTrigger("Pulsado");
        animAbajo.SetTrigger("pulsar");
        animMedia.SetTrigger("pulsar");
        animArriba.SetTrigger("pulsar");
        animCartelMenus.SetTrigger("Start");
        torrijaLogo.SetActive(false);
        //animLogo.SetTrigger("Pulsado");
        DesactivarBotonesSenales();

        Invoke("ActivarBotonesMenuJugar", 2.0f);
        //ActivarBotonesMenuJugar();
*/
    }

    public void comenzarJuego()
    {
        SceneManager.LoadScene("DECORACION PROTOTYPE MARIO");
    }
    
    public void pantallaCarga()
    {
        loadingScreen.SetActive(true);
    }

    public void ActivarBotonesSenales()
    {
        arriba.gameObject.SetActive(true);
        medio.gameObject.SetActive(true);
        abajo.gameObject.SetActive(true);
        torrijaLogo.SetActive(true);
    }

    public void DesactivarBotonesSenales(){
        arriba.gameObject.SetActive(false);
        medio.gameObject.SetActive(false);
        abajo.gameObject.SetActive(false);

    }

    private void ActivarBotonesMenuJugar(){
        back.gameObject.SetActive(true);
        host.gameObject.SetActive(true);
        client.gameObject.SetActive(true);
        code.gameObject.SetActive(true);
    }

    private void DesactivarBotonesMenuJugar(){
        back.gameObject.SetActive(false);
        host.gameObject.SetActive(false);
        client.gameObject.SetActive(false);
        code.gameObject.SetActive(false);
    }

    private void ActivarBotonesTienda(){
        back.gameObject.SetActive(true);
        expansions.gameObject.SetActive(true);
        skins.gameObject.SetActive(true);
        ad.gameObject.SetActive(true);
        coinPurchase.gameObject.SetActive(true);
    }

    private void DesactivarBotonesTienda(){
        back.gameObject.SetActive(false);
        expansions.gameObject.SetActive(false);
        skins.gameObject.SetActive(false);
        ad.gameObject.SetActive(false);
        
    }

    private void ActivarBotonesOpciones(){
        back.gameObject.SetActive(true);
        general.gameObject.SetActive(true);
        music.gameObject.SetActive(true);
        sfx.gameObject.SetActive(true);

    }

    private void DesactivarBotonesOpciones(){
        back.gameObject.SetActive(false);
        general.gameObject.SetActive(false);
        music.gameObject.SetActive(false);
        sfx.gameObject.SetActive(false);
    }

    public void caerDoodem(){
        animCartelDoodem.SetTrigger("Start");
        //AudioManager.instance.PlayOneShot(FMODEvents.instance.CaidaCadenas, camara.transform.position);
        AudioManager.PlaySound(SoundType.CADENAS);

    }

    public void BotonAtras()
    {
        // AudioManager.instance.PlayOneShot(FMODEvents.instance.ClickMadera, camara.transform.position);
        AudioManager.PlaySound(SoundType.CLICK);
        //AudioManager.instance.PlayOneShot(FMODEvents.instance.CaidaCadenas, camara.transform.position);
        AudioManager.PlaySound(SoundType.CADENAS);

       
        DesactivarBotonesOpciones();
        animCartelMenus.SetTrigger("Back");
        
        DesactivarBotonesMenuJugar();
        DesactivarBotonesTienda();
        coinPurchase.gameObject.SetActive(false);
        
        
        Invoke("ActivarBotonesSenales", 3.5f);
        //ActivarBotonesSenales();
        Invoke("reproducirAnimacionesMenu", 1f);
        

    }

    public void reproducirAnimacionesMenu()
    {
        monedaTienda.SetActive(false);
        caerTotem();
        aparecerLogo();
        Invoke("caerDoodem", 2f);
        //caerDoodem();
    }

    public void BotonHost(){
        Debug.Log("ahora estarias hosteando");
    }

    public void BotonClient(){
        Debug.Log("si hubiese code valido estarias de cliente");
    }

    public void ExpansionShop(){
        shopBaseUI.SetActive(false);
        expansionesUI.SetActive(true);
        back.gameObject.SetActive(false);
        backShop.gameObject.SetActive(true);
        DesactivarBotonesTienda();
        Debug.Log("esta es la tienda de expansiones mira q guapaaa");
    }

    public void SkinShop(){
        shopBaseUI.SetActive(false);
        skins_UI.SetActive(true);
        back.gameObject.SetActive(false);
        backShop.gameObject.SetActive(true);
        DesactivarBotonesTienda();
        Debug.Log("esta es la tienda de cosmeticos mira q guapaaa");
    }

    public void ViewAd(){
        anuncioElegido = Random.Range(0, 3);
        Debug.Log($"Anuncio elegido {anuncioElegido}");
        anuncioUI.SetActive(true);
        anuncios[anuncioElegido].SetActive(true);
        coinManager.GetComponent<Coins>().sumarCoins(25);
        Invoke("QuitAd", 5.0f);
    }

    public void QuitAd()
    {
        anuncios[anuncioElegido].SetActive(false);
        anuncioUI.SetActive(false);
        
    }

    public void volverTienda()
    {
        backShop.gameObject.SetActive(false);
        back.gameObject.SetActive(true);
        expansionesUI.SetActive(false);
        skins_UI.SetActive(false);
        compraMonedasUI.SetActive(false);
        shopBaseUI.SetActive(true);
        ActivarBotonesTienda();
    }

    public void coinShop(){
        shopBaseUI.SetActive(false);
        compraMonedasUI.SetActive(true);
        back.gameObject.SetActive(false);
        backShop.gameObject.SetActive(true);
        skins_UI.SetActive(false);
        expansionesUI.SetActive(false);
        DesactivarBotonesTienda();
        Debug.Log("esta es la tienda de monedicas");
    }

    public void appearPayPal()
    {
        paypalImage.SetActive(true);
    }
    
    public void disappearPayPal()
    {
        paypalImage.SetActive(false);
    }

}

