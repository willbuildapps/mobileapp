using System;
using System.Linq;
using System.Globalization;
using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Text;
using Android.Text.Style;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Java.Lang;
using Toggl.Foundation;
using Toggl.Giskard.Extensions;
using Toggl.Giskard.Views.EditDuration.Shapes;
using System.Reactive.Subjects;
using System.Reactive.Linq;
using System.Reactive;

namespace Toggl.Giskard.Views.EditDuration
{
    public partial class WheelDurationInput : EditText, ITextWatcher
    {
        private class InputFilter : Java.Lang.Object, IInputFilter
        {
            private Action<int> digitHandler;
            private Action deleteHandler;

            public InputFilter(Action<int> digitHandler, Action deleteHandler)
            {
                this.digitHandler = digitHandler;
                this.deleteHandler = deleteHandler;
            }

            public ICharSequence FilterFormatted(ICharSequence source, int start, int end, ISpanned dest, int dstart, int dend)
            {
                var empty = string.Empty.AsJavaString();
                var sourceLength = source.Length();

                if (sourceLength > 1)
                    return source.ToString().AsJavaString();

                if (sourceLength == 0)
                {
                    deleteHandler();
                    return "0".AsCharSequence();
                }

                var lastChar = source.CharAt(sourceLength - 1);

                if (char.IsDigit(lastChar))
                {
                    int digit = int.Parse(lastChar.ToString());
                    digitHandler(digit);

                    return empty;
                }

                return empty;
            }
        }
    }
}
