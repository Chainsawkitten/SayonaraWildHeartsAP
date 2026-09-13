using System;
using System.Collections.Generic;
using UnityEngine;

namespace SayonaraWildHeartsRandomizer;

public class MessageDisplay
{
    private bool oneTimeChangesDone = false;
    private SGMenuText text = null;
    private SGGameUtils.SGVector4Curve colorCurve = null;

    private float fadeTime = 1.0f;
    private float displayTime = 4.0f;
    private float messageTime = 100f;
    private float alpha = 1.0f;

    private Queue<string> messageQueue = new Queue<string>();

    public void QueueMessage(string message)
    {
        messageQueue.Enqueue(message);
    }

    public void Update(float deltaTime)
    {
        PerformOneTimeChanges();

        if (text != null)
        {
            messageTime += deltaTime;

            if (messageTime < fadeTime)
            {
                alpha = messageTime / fadeTime;
            }
            else if (messageTime < fadeTime + displayTime)
            {
                alpha = 1.0f;
            }
            else if (messageTime < fadeTime + displayTime + fadeTime)
            {
                alpha = 1.0f - ((messageTime - (fadeTime + displayTime)) / fadeTime);
            }
            else
            {
                alpha = 0.0f;

                if (messageQueue.Count > 0)
                {
                    messageTime = 0.0f;
                    text.SetText(messageQueue.Dequeue());
                }
            }

            UpdateText(deltaTime);
        }
    }

    public void PerformOneTimeChanges()
    {
        if (oneTimeChangesDone)
        {
            return;
        }

        try
        {
            // Find the "Game Center" menu item text. We'll repurpose it to display our messages.
            GameObject menu = GameObject.Find("Menu");
            if (menu != null)
            {
                Transform gameCenterTransform = menu.transform.Find("mainmenu/titlescreen/sharedplatform/menuoptionparent6/menuoption_6_05/text");
                if (gameCenterTransform != null)
                {
                    gameCenterTransform.parent = menu.transform;

                    GameObject gameCenterText = gameCenterTransform.gameObject;
                    text = gameCenterText.GetComponent<SGMenuText>();

                    // Change the layer to the UI layer, so it will display everywhere.
                    gameCenterText.layer = 5;

                    colorCurve = SGFW.GameUtils.CreateColorCurve(SGFW.GameConfig.SCORECFG.secretBananaPopupRamp);

                    oneTimeChangesDone = true;
                }
            }
        }
        catch (Exception e)
        {
            Plugin.Logger.LogError(e.ToString());
        }
    }

    public void UpdateText(float deltaTime)
    {
        bool activeInHierarchy = text.m_hInst.hGameObject.activeInHierarchy;
        if (activeInHierarchy)
        {
            SGMenuHandler.Instance.QueueVisibleText(text);
        }

        for (int i = 0; i < text.m_hCharacterMeshes.Count; i++)
        {
            SGCharacterMesh sGCharacterMesh = text.m_hCharacterMeshes[i];
            sGCharacterMesh.m_nRenderLayer = text.m_hInst.hGameObject.layer;
            sGCharacterMesh.m_bVisible = activeInHierarchy && text.m_fCurrentAlpha > 0f;

            text.m_fFloatingTextTime = Time.time;
            float fFloatingTextTime = text.m_fFloatingTextTime;
            float delay = (float)i * text.floatingDelayBetweenChars;
            sGCharacterMesh.m_hTransitionData.vPosition.x = sGCharacterMesh.m_vInitialPos.x + Mathf.Sin((fFloatingTextTime - delay * 2f) * text.floatingFrequency.x) * text.floatingAmplitude.x;
            sGCharacterMesh.m_hTransitionData.vPosition.y = sGCharacterMesh.m_vInitialPos.y + Mathf.Sin((fFloatingTextTime - delay * 16f) * text.floatingFrequency.y) * text.floatingAmplitude.y;
            if (colorCurve != null)
            {
                float charTime = (fFloatingTextTime - (float)i * 0.01f) * 0.5f;
                charTime -= (float)(int)charTime;
                Color value = colorCurve.Evaluate(charTime);
                sGCharacterMesh.m_hMaterial.SetColor(SGShaderParam._ColorScheme, value);
            }

            sGCharacterMesh.m_hMaterial.SetFloat(SGShaderParam._AlphaScaler, alpha);
            sGCharacterMesh.m_vLocalPosition = sGCharacterMesh.m_hTransitionData.vPosition;
        }
    }
}
