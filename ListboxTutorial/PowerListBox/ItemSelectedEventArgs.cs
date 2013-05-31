using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Unique.Controls
{
    public class ItemSelectedEventArgs : EventArgs
    {
        public object NewSelectedItem { get; internal set; }
        public object OldSelectedItem { get; internal set; }
    }
}
