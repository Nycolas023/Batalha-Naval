using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CodigoInterfacePrincipal : MonoBehaviour
{
    [SerializeField] private GameObject layoutInicial;
    [SerializeField] private GameObject botoesIniciais;
    [SerializeField] private GameObject menuRegistro;
    [SerializeField] private GameObject menuLogin;
    [SerializeField] private GameObject erroRegistrarNome;
    [SerializeField] private GameObject erroRegistrarSenha;
    [SerializeField] private GameObject erroLogin;
    [SerializeField] private GameObject menuPrincipal;
    [SerializeField] private GameObject lojaMoedas;
    [SerializeField] private GameObject lojaTemas;
    [SerializeField] private GameObject lojaBombas;
    [SerializeField] private GameObject perfil;
    [SerializeField] private GameObject temas;
    [SerializeField] private GameObject lobby;
    [SerializeField] private GameObject sala;

    public void AbrirMenuRegistrar()
    { 
        layoutInicial.SetActive(false);
        menuLogin.SetActive(false);
        botoesIniciais.SetActive(false);
        menuRegistro.SetActive(true);
    }

    public void AbrirMenuLogin()
    {
        layoutInicial.SetActive(false);
        menuRegistro.SetActive(false);
        botoesIniciais.SetActive(false);
        menuLogin.SetActive(true);
    }

    public void FecharErroRegistro()
    { 
        erroRegistrarNome.SetActive(false);
        erroRegistrarSenha.SetActive(false);    
    }

    public void FecharErroLogin()
    {
        erroLogin.SetActive(false);

    }

    public void Jogar()
    {
        menuPrincipal.SetActive(false);
        lobby.SetActive(true);

    }

    public void AbrirPerifl()
    {
        menuPrincipal.SetActive(false);
        perfil.SetActive(true);
    }


    public void AbrirLojaMoedas()
    {
        menuPrincipal.SetActive(false);
        lojaTemas.SetActive(false);
        lojaBombas.SetActive(false);
        lojaMoedas.SetActive(true);
    }

    public void AbrirLojaTemas()
    {
        lojaBombas.SetActive(false);
        lojaMoedas.SetActive(false);
        lojaTemas.SetActive(true);
    }
    public void AbrirLojaBombas()
    {
        lojaMoedas.SetActive(false);
        lojaTemas.SetActive(false);
        lojaBombas.SetActive(true);
    }


    public void AbrirTemas()
    {
        menuPrincipal.SetActive(false);
        temas.SetActive(true);
    }

    public void VoltarMenuPrincipal()
    {
        lobby.SetActive(false);
        perfil.SetActive(false);
        lojaMoedas.SetActive(false);
        lojaTemas.SetActive(false);
        lojaBombas.SetActive(false);
        temas.SetActive(false);
        menuPrincipal.SetActive(true);
    }
 
    public void SairSala()
    {
        sala.SetActive(false);
        lobby.SetActive(true);
    }
}
