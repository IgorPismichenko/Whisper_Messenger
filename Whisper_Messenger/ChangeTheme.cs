using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Whisper_Messenger
{
    class ChangeTheme
    {
        public static void ThemeChange(Uri themeuri)
        {
            ResourceDictionary Theme = new ResourceDictionary()
            {
                Source = themeuri,
            };
            App.Current.Resources.Clear();
            App.Current.Resources.MergedDictionaries.Add(Theme);
            


        }




    }

    
}
