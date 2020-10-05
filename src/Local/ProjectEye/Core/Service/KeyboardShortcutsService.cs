using ProjectEye.ViewModels;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Input;

namespace ProjectEye.Core.Service
{
    public class KeyboardShortcutsService : IService
    {
        private Dictionary<string, Command> keycommands;
        public KeyboardShortcutsService()
        {
            keycommands = new Dictionary<string, Command>();
        }
        /// <summary>
        /// 设置快捷键命令
        /// </summary>
        /// <param name="key"></param>
        /// <param name="command"></param>
        public void Set(string key, Command command)
        {
            if (keycommands.ContainsKey(key))
            {
                keycommands[key] = command;
            }
            else
            {
                keycommands.Add(key, command);
            }
        }
        public void Init()
        {

        }
        /// <summary>
        /// 执行快捷键命令
        /// </summary>
        /// <param name="keyEventArgs"></param>
        public void Execute(KeyEventArgs keyEventArgs, object parameter = null)
        {
            string key;
            if (keyEventArgs.KeyboardDevice.Modifiers == ModifierKeys.Control && keyEventArgs.Key != Key.LeftCtrl)
            {
                key = "CTRL + " + keyEventArgs.Key.ToString();
            }
            else
            {
                key = keyEventArgs.Key.ToString();
            }
            if (keycommands.ContainsKey(key))
            {
                Debug.WriteLine("执行快捷键命令：" + key);
                keycommands[key].Execute(parameter);
            }
        }
    }
}
