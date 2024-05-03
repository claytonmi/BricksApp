using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfigManager : MonoBehaviour
{
    private const string FULLSCREEN_KEY = "fullscreen";
    private const string RESOLUTION_WIDTH_KEY = "resolution_width";
    private const string RESOLUTION_HEIGHT_KEY = "resolution_height";

    public Dropdown fullscreenDropdown;
    public Dropdown resolutionDropdown;

    private Resolution[] resolutions;

    void Start()
    {
        // Obtém todas as resoluções suportadas pelo dispositivo
        resolutions = Screen.resolutions;

        // Define a opção selecionada no Dropdown com base nas configurações salvas
        fullscreenDropdown.value = GetFullscreen() ? 0 : 1;
        resolutionDropdown.ClearOptions();
        List<string> resolutionOptions = new List<string>();
        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            resolutionOptions.Add(resolutions[i].width + "x" + resolutions[i].height);
            if (resolutions[i].width == GetResolutionWidth() && resolutions[i].height == GetResolutionHeight())
            {
                currentResolutionIndex = i;
            }
        }
        resolutionDropdown.AddOptions(resolutionOptions);

        resolutionDropdown.value = currentResolutionIndex;
        // Aplica as configurações salvas
        ApplyScreenSettings();
        // Ajusta automaticamente as configurações de resolução para o dispositivo
        AutoAdjustResolution();
    }

    // Função chamada quando a opção do dropdown é alterada
    public void OnDropdownValueChanged()
    {
        SetFullscreen(fullscreenDropdown.value == 0);
        ApplyScreenSettings();
    }

    public void OnResolutionDropdownValueChanged()
    {
        SetResolution(resolutions[resolutionDropdown.value].width, resolutions[resolutionDropdown.value].height);
        ApplyScreenSettings();
    }

    private void ApplyScreenSettings()
    {
        // Aplica as configurações de fullscreen
        Screen.fullScreen = GetFullscreen();

        // Aplica a resolução
        Screen.SetResolution(GetResolutionWidth(), GetResolutionHeight(), GetFullscreen());

        // Ajusta a escala do Canvas para a nova resolução
        CanvasScaler canvasScaler = GetComponent<CanvasScaler>();
        if (canvasScaler != null)
        {
            canvasScaler.referenceResolution = new Vector2(GetResolutionWidth(), GetResolutionHeight());
        }
        else
        {
            Debug.LogWarning("CanvasScaler not found!");
        }
    }

    public void SetFullscreen(bool isFullscreen)
    {
        PlayerPrefs.SetInt(FULLSCREEN_KEY, isFullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }

    public bool GetFullscreen()
    {
        return PlayerPrefs.GetInt(FULLSCREEN_KEY, 1) == 1;
    }

    public void SetResolution(int width, int height)
    {
        PlayerPrefs.SetInt(RESOLUTION_WIDTH_KEY, width);
        PlayerPrefs.SetInt(RESOLUTION_HEIGHT_KEY, height);
        PlayerPrefs.Save();
    }

    public int GetResolutionWidth()
    {
        return PlayerPrefs.GetInt(RESOLUTION_WIDTH_KEY, Screen.width);
    }

    public int GetResolutionHeight()
    {
        return PlayerPrefs.GetInt(RESOLUTION_HEIGHT_KEY, Screen.height);
    }

    private void AutoAdjustResolution()
    {
        // Obtém a resolução atual do dispositivo
        Resolution currentResolution = Screen.currentResolution;

        // Percorre as resoluções suportadas para encontrar a mais próxima
        int closestResolutionIndex = 0;
        float closestDistance = float.MaxValue;
        for (int i = 0; i < resolutions.Length; i++)
        {
            float distance = Vector2.Distance(new Vector2(currentResolution.width, currentResolution.height), new Vector2(resolutions[i].width, resolutions[i].height));
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestResolutionIndex = i;
            }
        }

        // Define a resolução mais próxima como a resolução atual
        resolutionDropdown.value = closestResolutionIndex;
        SetResolution(resolutions[closestResolutionIndex].width, resolutions[closestResolutionIndex].height);
    }
}
