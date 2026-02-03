using System;
using System.Collections.Generic;
using UnityEngine;

namespace OmniumLessons
{
    public class WindowsService : MonoBehaviour
    {
        [SerializeField] private Window[] _windows;

        private Dictionary<Type, Window> windowsDictionary;


        public void Initialize()
        {
            windowsDictionary = new Dictionary<Type, Window>();
            foreach (Window window in _windows)
            {
                windowsDictionary.Add(window.GetType(), window);
                window.Hide(true);
                window.Initialize();
            }

            ShowWindow<MainMenuWindow>(true);
        }

        public T GetWindow<T>() where T : Window
        {
            return windowsDictionary[typeof(T)] as T;
        }

        public void ShowWindow<T>(bool isImmediately) where T : Window
        {
            var window = windowsDictionary[typeof(T)] as T;
            if (window == null)
            {
                Debug.LogError("Not found window");
                return;
            }

            window.Show(isImmediately);
        }

        public void HideWindow<T>(bool isImmediately) where T : Window
        {
            var window = windowsDictionary[typeof(T)] as T;
            if (window == null)
            {
                Debug.LogError("Not found window");
                return;
            }

            window.Hide(isImmediately);
        }
    }
}