using BrazeKit;
using BrazeUI;
using CoreFoundation;

namespace MauiAppBraze4;

public partial class App : Application
{
    // Braze Properties
    public static Braze? braze = null;
    public static BrazeInAppMessageUI? inAppMessageUI = null;
    private BrazeSDKAuthDelegate? sdkAuthDelegate = null;

	public App()
	{
		InitializeComponent();
		SetupBraze();
		MainPage = new AppShell();
	}


    void SetupBraze()
    {
        // create the Braze configuration
        var configuration = new BRZConfiguration("77e1164b-3980-4398-a88e-702518177080", "sdk.fra-01.braze.eu");
        configuration.Logger.Level = BRZLoggerLevel.Debug;
        configuration.TriggerMinimumTimeInterval = 1.0;

        // // - Location / Geofences support
        // configuration.Location.BrazeLocationProvider = new BrazeLocationProvider();
        // configuration.Location.AutomaticLocationCollection = true;
        // configuration.Location.GeofencesEnabled = true;
        // configuration.Location.AutomaticGeofenceRequests = true;

        // - Automatic push notifications support
        configuration.Push.Automation = new BRZConfigurationPushAutomation(true);
        // configuration.Push.Automation.AutomaticSetup = true;
        // configuration.Push.Automation.RequestAuthorizationAtLaunch = true;
        // configuration.Push.Automation.SetNotificationCategories = true;
        configuration.Push.Automation.RegisterDeviceToken = true;
        // configuration.Push.Automation.HandleBackgroundNotification = true;
        // configuration.Push.Automation.HandleBackgroundNotification = true;
        // configuration.Push.Automation.WillPresentNotification = true;

        /*
    // Automatic push notification setup
    configuration.push.automation = .init(
      automaticSetup: true,
      requestAuthorizationAtLaunch: true,
      setNotificationCategories: true,
      registerDeviceToken: true,
      handleBackgroundNotification: true,
      handleNotificationResponse: true,
      willPresentNotification: true
    )
        */

        // - Universal link forwarding
        configuration.ForwardUniversalLinks = true;

        // - SDK Authentication support
        configuration.Api.SdkAuthentication = true;

        // - Miscellaneous
        configuration.Api.AddSDKMetadata(new[] { BRZSDKMetadata.Xamarin });

        // create the Braze instance and store it on the AppDelegate
        braze = new Braze(configuration);

        // register the BrazeInAppMessageUI as the In-App Message presenter
        inAppMessageUI = new BrazeInAppMessageUI();
        braze.InAppMessagePresenter = inAppMessageUI;

        // // add GIF support to BrazeUI (see below for SdWebImage implementation)
        // BRZGIFViewProvider.Shared = BRZGIFViewProviderExtensions.SdWebImage();

        // register the delegate for SDK authentication
        sdkAuthDelegate = new SDKAuthenticationDelegate();
        braze.SdkAuthDelegate = sdkAuthDelegate;

		// braze4だとこのくだりはいらないっぽい
        // UNUserNotificationCenter center = UNUserNotificationCenter.Current;
        // center.SetNotificationCategories(BRZNotifications.Categories);
        // center.RequestAuthorization(UNAuthorizationOptions.Alert | UNAuthorizationOptions.Sound | UNAuthorizationOptions.Badge, (granted, error) => {
        //     Console.WriteLine($"Notification authorization, granted: {granted}, error: {error?.ToString() ?? "None"}");
        //     // Braze.SharedInstance.PushAuthorizationFromUserNotificationCenter(granted);
        // });

    }
}
class SDKAuthenticationDelegate : BrazeSDKAuthDelegate
{
    public override void SdkAuthenticationFailedWithError(Braze braze, BRZSDKAuthenticationError error)
    {
        Console.WriteLine("Invalid SDK Authentication signature.");
        braze.User.IdOnQueue(DispatchQueue.MainQueue, (userId) => {
            string newSignature = "NEW_SDK_AUTH_SIGNATURE_FOR_USER_" + userId;
            braze.SetSDKAuthenticationSignature(newSignature);
        });
    }
}