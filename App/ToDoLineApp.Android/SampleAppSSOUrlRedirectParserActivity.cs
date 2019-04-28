using Android.App;
using Android.Content;
using Android.Content.PM;
using Bit.Android;

namespace ToDoLineApp.Droid
{
    [Activity(Label = nameof(SampleAppSSOUrlRedirectParserActivity), NoHistory = true, LaunchMode = LaunchMode.SingleTop)]
    [IntentFilter(new[] { Intent.ActionView }, Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable }, DataSchemes = new[] { "test" }, DataPath = "test://oauth2redirect")]
    public class SampleAppSSOUrlRedirectParserActivity : BitSSOUrlRedirectParserActivity
    {
    }
}