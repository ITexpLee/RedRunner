﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RedRunner.UI;
using System.Linq;

namespace RedRunner
{
    public enum UIScreenInfo
    {
        LOADING_SCREEN,
        START_SCREEN,
        END_SCREEN,
        PAUSE_SCREEN,
        IN_GAME_SCREEN
    }

    public class UIManager : MonoBehaviour
    {

        private static UIManager m_Singleton;

        public static UIManager Singleton
        {
            get
            {
                return m_Singleton;
            }
        }

        [SerializeField]
        private List<UIScreen> m_Screens;
        private UIScreen m_ActiveScreen;
        private UIWindow m_ActiveWindow;
        [SerializeField]
        private Texture2D m_CursorDefaultTexture;
        [SerializeField]
        private Texture2D m_CursorClickTexture;
        [SerializeField]
        private float m_CursorHideDelay = 1f;

        public List<UIScreen> UISCREENS
        {
            get
            {
                return m_Screens;
            }
        }

        void Awake()
        {
            if (m_Singleton != null)
            {
                Destroy(gameObject);
                return;
            }
            m_Singleton = this;
            Cursor.SetCursor(m_CursorDefaultTexture, Vector2.zero, CursorMode.Auto);
        }

        void Start()
        {
            OpenScreen(m_Screens[0]);
        }

        void Update()
        {
            if (Input.GetButtonDown("Cancel"))
            {
                //Added enumeration to store screen info, aka type, so it will be easier to understand it
                var pauseScreen = m_Screens.Find(el => el.ScreenInfo == UIScreenInfo.PAUSE_SCREEN);
                var ingameScreen = m_Screens.Find(el => el.ScreenInfo == UIScreenInfo.IN_GAME_SCREEN);

                //If the pause screen is not open : open it otherwise close it
                if (!pauseScreen.IsOpen)
                {
                    OpenScreen(pauseScreen);
                    GameManager.Singleton.StopGame();
                }
                else
                {
                    CloseScreen(pauseScreen);
                    OpenScreen(ingameScreen);
                    ////We are sure that we want to resume the game when we close a screen
                    GameManager.Singleton.ResumeGame();
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                Cursor.SetCursor(m_CursorClickTexture, Vector2.zero, CursorMode.Auto);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                Cursor.SetCursor(m_CursorDefaultTexture, Vector2.zero, CursorMode.Auto);
            }
        }

        public void OpenWindow(UIWindow window)
        {
            window.Open();
            m_ActiveWindow = window;
        }

        public void CloseWindow(UIWindow window)
        {
            if (m_ActiveWindow == window)
            {
                m_ActiveWindow = null;
            }
            window.Close();
        }

        public void CloseActiveWindow()
        {
            if (m_ActiveWindow != null)
            {
                CloseWindow(m_ActiveWindow);
            }
        }

        public void OpenScreen(UIScreen screen)
        {
            CloseAllScreens();
            if (m_ActiveScreen == null)
            {
                m_ActiveScreen = screen;
                m_ActiveScreen.UpdateScreenStatus(true);
                return;
            }
            m_ActiveScreen.UpdateScreenStatus(false);
            screen.UpdateScreenStatus(true);
            m_ActiveScreen = screen;
        }

        public void CloseScreen(UIScreen screen)
        {
            if (m_ActiveScreen == screen)
            {
                m_ActiveScreen = null;
            }
            screen.UpdateScreenStatus(false);
        }

        public void CloseAllScreens()
        {
            foreach (var screen in m_Screens)
                CloseScreen(screen);
        }
    }

}