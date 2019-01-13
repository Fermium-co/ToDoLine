using System;
using System.Threading;
using ToDoLineApp.Resources.Strings;

namespace Acr.UserDialogs
{
    public static class IUserDialogsExtensions
    {
        public static IProgressDialog Loading(this IUserDialogs userDialogs, string title, out CancellationToken cancellationToken, string cancelText = null, Action<CancellationToken> onCancel = null)
        {
            cancelText = cancelText ?? Strings.Cancel;

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMinutes(45));

            cancellationToken = cancellationTokenSource.Token;

            return userDialogs.Loading(title: title, cancelText: cancelText, maskType: MaskType.Clear, onCancel: () =>
            {
                cancellationTokenSource.Cancel();
                onCancel?.Invoke(cancellationTokenSource.Token);
            });
        }
    }
}
