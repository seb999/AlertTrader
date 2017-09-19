﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Collections.ObjectModel;
using AlertTrader.Classes;

namespace AlertTrader.Misc
{
    public static class helper
    {
        /// <summary>
        /// Add a meesage in the list
        /// </summary>
        /// <param name="message">the message to add</param>
        /// <param name="messageList">the list of messages</param>
        /// <param name="color">the color of the message</param>
        internal static void DisplayUserMessage(string message, ObservableCollection<Information> messageList, SolidColorBrush color)
        {
            messageList.Add(new Information()
            {
                Date = DateTime.Now,
                DisplayInfo = message,
                DisplayColor = color
            });
        }
    }
}